namespace SchoolShopOrderSystem.Domain.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Allergy { get; set; }

        public int ParentId { get; set; }
        public Parent Parent { get; set; } = null!;
    }
}
