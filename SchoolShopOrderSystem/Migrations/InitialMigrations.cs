using FluentMigrator;
namespace Infrastructure.Migrations;

[Migration(1)]
public class _001_InitialCreate : Migration
{
    public override void Up()
    {
        Create.Table("Parents")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Email").AsString(200).NotNullable()
            .WithColumn("WalletBalance").AsDecimal(10, 2).NotNullable();

        Create.Table("Students")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Allergy").AsString(500).Nullable()
            .WithColumn("ParentId").AsInt32().NotNullable()
                .ForeignKey("FK_Students_Parents", "Parents", "Id");

        Create.Table("Canteens")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("DayOfWeek").AsInt32().NotNullable()
            .WithColumn("CutoffTime").AsTime().NotNullable();

        Create.Table("MenuItems")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Price").AsDecimal(10, 2).NotNullable()
            .WithColumn("DailyStock").AsInt32().Nullable()
            .WithColumn("AllergenTags").AsString(500).Nullable()
            .WithColumn("CanteenId").AsInt32().NotNullable()
                .ForeignKey("FK_MenuItems_Canteens", "Canteens", "Id");

        Create.Table("Orders")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ParentId").AsInt32().NotNullable()
                .ForeignKey("FK_Orders_Parents", "Parents", "Id")
            .WithColumn("StudentId").AsInt32().NotNullable()
                .ForeignKey("FK_Orders_Students", "Students", "Id")
            .WithColumn("CanteenId").AsInt32().NotNullable()
                .ForeignKey("FK_Orders_Canteens", "Canteens", "Id")
            .WithColumn("FulfilmentDate").AsDate().NotNullable()
            .WithColumn("State").AsInt32().NotNullable()
            .WithColumn("TotalAmount").AsDecimal(10, 2).NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable();

        Create.Table("OrderItems")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("OrderId").AsInt32().NotNullable()
                .ForeignKey("FK_OrderItems_Orders", "Orders", "Id")
            .WithColumn("MenuItemId").AsInt32().NotNullable()
                .ForeignKey("FK_OrderItems_MenuItems", "MenuItems", "Id")
            .WithColumn("Quantity").AsInt32().NotNullable()
            .WithColumn("UnitPrice").AsDecimal(10, 2).NotNullable();

        Create.Table("IdempotencyRecords")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Key").AsString(200).NotNullable().Unique()
            .WithColumn("ResponseJson").AsString(int.MaxValue).NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("ExpireAt").AsDateTime().NotNullable();

        Insert.IntoTable("Parents").Row(new
        {
            Name = "John Wick",
            Email = "abc@gmail.com",
            WalletBalance = 500.00m
        });

        Insert.IntoTable("Students").Row(new
        {
            Name = "Jason Wills",
            ParentId = 1,
            allergy = "dairy"
        });

        Insert.IntoTable("Canteens").Row(new
        {
            Name = "School Canteen",
            DayOfWeek = 1,
            CutoffTime = new TimeSpan(9, 30, 0)
        });

        Insert.IntoTable("MenuItems").Row(new
        {
            Name = "Chicken Burger",
            Price = 150.00m,
            DailyStock = 20,
            AllergenTags = "gluten",
            CanteenId = 1
        });

        Insert.IntoTable("MenuItems").Row(new
        {
            Name = "Fries",
            Price = 80.00m,
            DailyStock = 30,
            AllergenTags = "",
            CanteenId = 1
        });

        Insert.IntoTable("MenuItems").Row(new
        {
            Name = "Sandwich",
            Price = 120.00m,
            DailyStock = 25,
            AllergenTags = "dairy",
            CanteenId = 1
        });

        Insert.IntoTable("MenuItems").Row(new
        {
            Name = "Juice",
            Price = 60.00m,
            DailyStock = 40,
            AllergenTags = "",
            CanteenId = 1
        });

        Insert.IntoTable("MenuItems").Row(new
        {
            Name = "Water Bottle",
            Price = 30.00m,
            DailyStock = 100,
            AllergenTags = "",
            CanteenId = 1
        });
    }

    public override void Down()
    {
        Delete.Table("OrderItems");
        Delete.Table("Orders");
        Delete.Table("MenuItems");
        Delete.Table("CanteenOpenings");
        Delete.Table("Students");
        Delete.Table("Canteens");
        Delete.Table("Parents");
        Delete.Table("IdempotencyRecords");
    }
}