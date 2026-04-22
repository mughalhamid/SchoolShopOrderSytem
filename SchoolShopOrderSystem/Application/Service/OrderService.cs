using IBM.Data.Db2;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SchoolShopOrderSystem.Application.DTO;
using SchoolShopOrderSystem.Application.Mappers;
using SchoolShopOrderSystem.Application.Service.Interfaces;
using SchoolShopOrderSystem.Domain.Enums;
using SchoolShopOrderSystem.Domain.Models;

namespace SchoolShopOrderSystem.Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger logger;
        public OrderService(AppDbContext dbContext, ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<OrderResponseDto> RequestOrder(int studentId, List<MenuItemRequest> menuItems)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                logger.LogInformation("Creating order for student {StudentId}", studentId);

                var student = await dbContext.Students
                    .Include(s => s.Parent)
                    .FirstOrDefaultAsync(x => x.Id == studentId);

                if (student == null)
                    logger.LogInformation("Student not found");
                bool cancelOrder = false;

                var parent = await dbContext.Parents.FirstAsync(x => x.Id == student.ParentId);

                var canteen = await dbContext.Canteens.FirstAsync();

                var menuItemIds = menuItems.Select(x => x.MenuItemId).ToList();

                var temp = await dbContext.MenuItems.ToListAsync();
                var items = await dbContext.MenuItems
                    .Where(x => menuItemIds.Contains(x.Id))
                    .ToListAsync();

                var itemMap = items.ToDictionary(x => x.Id);

                decimal total = 0;

                foreach (var req in menuItems)
                {
                    var item = itemMap[req.MenuItemId];

                    if (item.DailyStock.HasValue && req.Quantity > item.DailyStock.Value)
                    {
                        logger.LogInformation($"Insufficient stock for {item.Name}");
                        cancelOrder = true;
                    }

                    if (!string.IsNullOrEmpty(student.Allergy) && item.AllergenTags != null &&
                         item.AllergenTags.Equals(student.Allergy, StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogInformation($"Allergy violation: {item.Name}");
                        cancelOrder = true;
                    }

                    total += item.Price * req.Quantity;
                }

                if (parent.WalletBalance < total)
                {
                    logger.LogInformation("Insufficient wallet balance");
                    cancelOrder = true;
                }

                if (canteen.CutoffTime < DateTime.UtcNow.TimeOfDay)
                {
                    logger.LogInformation("Cutoff time exceeded");
                    cancelOrder = true;
                }
                if (cancelOrder)
                    return new OrderResponseDto() { StudentId = studentId, OrderState = "Cancelled"};

                var order = new Order
                {
                    ParentId = parent.Id,
                    StudentId = student.Id,
                    CanteenId = canteen.Id,
                    State = OrderState.Confirmed,
                    TotalAmount = total,
                    CreatedAt = DateTime.UtcNow,
                    FulfilmentDate = DateTime.Today,
                    Items = new List<OrderItem>()
                };

                foreach (var req in menuItems)
                {
                    var item = itemMap[req.MenuItemId];

                    order.Items.Add(new OrderItem
                    {
                        MenuItemId = item.Id,
                        Quantity = req.Quantity,
                        UnitPrice = item.Price
                    });

                    if (item.DailyStock.HasValue)
                        item.DailyStock -= req.Quantity;
                }

                parent.WalletBalance -= total;

                dbContext.Orders.Add(order);
                dbContext.Parents.Update(parent);
                dbContext.MenuItems.UpdateRange(items);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                logger.LogInformation("Order created successfully {OrderId}", order.Id);

                return OrderMapper.OrderDtoMapper(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                logger.LogError(ex, "Order creation failed for student {StudentId}", studentId);

                return new OrderResponseDto
                {
                    StudentId = studentId,
                    OrderState = OrderState.Cancelled.ToString()
                };
            }
        }

        public async Task<OrderResponseDto> GetOrderDetails(int orderId)
        {
            try
            {
                logger.LogInformation($"Getting Order Details of Order: {orderId}");
                var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
                if (order == null)
                {
                    logger.LogInformation($"No Order Found for this OrderId: {orderId}");
                    return new OrderResponseDto();
                }
                logger.LogInformation($"Order found and returned successfully for OrderId: {orderId}");
                return OrderMapper.OrderDtoMapper(order);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error Occurred while getting order details for OrderId: {orderId}");
                return new OrderResponseDto();
            }
        }
    }
}
