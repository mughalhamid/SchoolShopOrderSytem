using Microsoft.AspNetCore.Http.HttpResults;
using SchoolShopOrderSystem.Domain.Enums;
using SchoolShopOrderSystem.Domain.Models;

namespace SchoolShopOrderSystem.Application.DTO
{
    public class OrderResponseDto
    {
        public int StudentId { get; set; }
        public int CanteenId { get; set; }
        public int OrderId { get; set; }
        public string OrderState { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
