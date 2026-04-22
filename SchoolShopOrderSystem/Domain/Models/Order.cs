using SchoolShopOrderSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolShopOrderSystem.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int ParentId { get; set; }
        public Parent Parent { get; set; } = null!;

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int CanteenId { get; set; }
        public Canteen Canteen { get; set; } = null!;

        public OrderState State { get; set; } = OrderState.Confirmed;

        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime FulfilmentDate { get; set; }
    }
}
