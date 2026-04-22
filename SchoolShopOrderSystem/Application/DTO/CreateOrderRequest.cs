namespace SchoolShopOrderSystem.Application.DTO
{
    public record CreateOrderRequest(int StudentId, List<MenuItemRequest> MenuItems);

    public class MenuItemRequest
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}
