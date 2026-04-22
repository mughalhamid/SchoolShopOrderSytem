using SchoolShopOrderSystem.Application.DTO;
using SchoolShopOrderSystem.Domain.Models;

namespace SchoolShopOrderSystem.Application.Service.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> RequestOrder(int studentId, List<MenuItemRequest> menuItems);
        Task<OrderResponseDto> GetOrderDetails(int orderId);
    }
}
