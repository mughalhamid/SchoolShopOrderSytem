using FluentMigrator.Runner;
using Infrastructure.Data;
using Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using SchoolShopOrderSystem.Application.Service;
using SchoolShopOrderSystem.Application.Service.Interfaces;
using SchoolShopOrderSystem.Application.DTO;
using Microsoft.Data.SqlClient;
using FluentValidation.AspNetCore;
using FluentValidation;
static void EnsureDatabase(string connectionString)
{
    var builder = new SqlConnectionStringBuilder(connectionString);
    var dbName = builder.InitialCatalog;

    builder.InitialCatalog = "master";

    using var connection = new SqlConnection(builder.ConnectionString);
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = $@"
        IF DB_ID(N'{dbName}') IS NULL
        BEGIN
            CREATE DATABASE [{dbName}]
        END";

    command.ExecuteNonQuery();
}
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

EnsureDatabase(connectionString);

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(_001_InitialCreate).Assembly).For.Migrations())
    .AddLogging(lb =>
    {
        lb.AddFluentMigratorConsole();
        lb.SetMinimumLevel(LogLevel.Information);
    });

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope()) {
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapPost("/orders", async (
    HttpContext http,
    CreateOrderRequest request,
    IOrderService orderService,
    IIdempotencyService idempotencyService,
    ILogger<Program> logger) => {
    var key = http.Request.Headers["Idempotency-Key"].ToString();

    if (!string.IsNullOrWhiteSpace(key))
    {
        var cached = await idempotencyService
            .GetCachedResponseAsync<OrderResponseDto>(key);
        if (cached != null)
        {
            logger.LogInformation("Returning cached order for Idempotency-Key {Key}", key);
            return Results.Ok(cached);
        }
    }

    logger.LogInformation("Creating Order for Student {StudentId}", request.StudentId);
    var order = await orderService.RequestOrder(
        request.StudentId,
        request.MenuItems);
    if (order.OrderState.ToLower() == "confirmed")
    {
        logger.LogInformation("Order created successfully for Student {StudentId}", order.StudentId);
    }
    else
    {
        logger.LogInformation("Order cancelled for Student {StudentId}", order.StudentId);
    }

    if (!string.IsNullOrWhiteSpace(key))
    {
        await idempotencyService.SaveResponseAsync(key, order);
    }
    return Results.Ok(order);
});

app.MapGet("/order", async (int orderId, IOrderService orderService, ILogger<Program> logger) => {
    logger.LogInformation("Get Order Details of OrderId: {orderId}", orderId);
    var result = await orderService.GetOrderDetails(orderId);
    if (result.OrderState.ToLower().ToLower() == "confirmed")
    {
        logger.LogInformation("Order is successfully placed and status is {State}", result.OrderState);
    }
    else
    {
        logger.LogInformation("No Order is placed for OrderId: {orderId}", orderId);
    }
    return result;
});

app.Run();