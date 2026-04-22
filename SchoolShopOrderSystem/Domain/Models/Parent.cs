using System.ComponentModel.DataAnnotations;

namespace SchoolShopOrderSystem.Domain.Models
{
    public class Parent
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public decimal WalletBalance { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
