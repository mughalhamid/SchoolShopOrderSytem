using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace SchoolShopOrderSystem.Tests.Fixtures;

public class SqliteTestFixture : IDisposable
{
    public AppDbContext Context { get; }

    private readonly SqliteConnection connection;

    public SqliteTestFixture()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .ConfigureWarnings(w =>
                w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        connection.Close();
    }
}