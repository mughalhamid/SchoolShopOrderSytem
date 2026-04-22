using SchoolShopOrderSystem.Domain.Models;
using SchoolShopOrderSystem.Application.DTO;

namespace SchoolShopOrderSystem.Application.Mappers
{
    public static class OrderMapper
    {
        public static OrderResponseDto OrderDtoMapper(Order order)
        {
            if (order == null) return null;

            return new OrderResponseDto
            {
                OrderId = order.Id,
                CanteenId = order.CanteenId,
                StudentId = order.StudentId,
                OrderState = order.State.ToString(),
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            };
        }
    }
}