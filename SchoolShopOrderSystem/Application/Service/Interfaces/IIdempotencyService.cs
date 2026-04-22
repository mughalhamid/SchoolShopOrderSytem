namespace SchoolShopOrderSystem.Application.Service.Interfaces
{
    public interface IIdempotencyService
    {
        Task<T?> GetCachedResponseAsync<T>(string key);
        Task SaveResponseAsync<T>(string key, T response);
    }
}
