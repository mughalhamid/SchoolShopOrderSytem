using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SchoolShopOrderSystem.Application.DTO;
using SchoolShopOrderSystem.Application.Service;
using SchoolShopOrderSystem.Domain.Enums;
using SchoolShopOrderSystem.Domain.Models;
using SchoolShopOrderSystem.Tests.Fixtures;

namespace SchoolShopOrderSystem.Tests.Unit;

[TestFixture]
public class OrderServiceTests
{
    private SqliteTestFixture db;
    private OrderService orderService;

    [SetUp]
    public void Setup()
    {
        db = new SqliteTestFixture();

        orderService = new OrderService(
            db.Context,
            new LoggerFactory().CreateLogger<OrderService>());
    }

    [TearDown]
    public void TearDown()
    {
        db.Dispose();
    }

    [Test]
    public async Task Should_Cancel_Order_When_Wallet_Is_Insufficient()
    {
        var parent = new Parent { Name = "Williams", Email = "abc@gmail.com", WalletBalance = 110 };
        var student = new Student { Name = "John", Parent = parent };

        var item = new MenuItem
        {
            Name = "Burger",
            Price = 100,
            DailyStock = 10,
            AllergenTags = "none",
            CanteenId = 1
        };

        var canteen = new Canteen
        {
            Name = "Main",
            CutoffTime = new TimeSpan(23, 59, 0)
        };

        db.Context.AddRange(parent, student, item, canteen);
        await db.Context.SaveChangesAsync();

        var request = new List<MenuItemRequest>
        {
            new() { MenuItemId = item.Id, Quantity = 1 }
        };

        var result = await orderService.RequestOrder(student.Id, request);

        result.OrderState.Should().Be("Confirmed");
    }
}