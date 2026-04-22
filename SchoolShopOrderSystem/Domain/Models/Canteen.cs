namespace SchoolShopOrderSystem.Domain.Models
{
    public class Canteen
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan CutoffTime { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}
