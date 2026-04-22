using System.ComponentModel.DataAnnotations;

namespace SchoolShopOrderSystem.Domain.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public int? DailyStock { get; set; }

        public string? AllergenTags { get; set; }

        public int CanteenId { get; set; }
        public Canteen Canteen { get; set; } = null!;
    }
}
