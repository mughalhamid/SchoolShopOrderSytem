using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using SchoolShopOrderSystem.Domain.Models;
using SchoolShopOrderSystem.Application.Service.Interfaces;

public class IdempotencyService : IIdempotencyService
{
    private readonly AppDbContext dbContext;

    public IdempotencyService(AppDbContext db)
    {
        dbContext = db;
    }

    public async Task<T?> GetCachedResponseAsync<T>(string key)
    {
        var record = await dbContext.IdempotencyRecords
            .FirstOrDefaultAsync(x => x.Key == key && x.ExpireAt > DateTime.UtcNow);

        if (record == null)
            return default;

        return JsonSerializer.Deserialize<T>(record.ResponseJson);
    }

    public async Task SaveResponseAsync<T>(string key, T response)
    {
        var record = new IdempotencyRecord
        {
            Key = key,
            ResponseJson = JsonSerializer.Serialize(response),
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddHours(24)
        };

        dbContext.IdempotencyRecords.Add(record);
        await dbContext.SaveChangesAsync();
    }
}