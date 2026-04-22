namespace SchoolShopOrderSystem.Domain.Models
{
    public class IdempotencyRecord
    {
        public int Id { get; set; }

        public string Key { get; set; }
        public string ResponseJson { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}