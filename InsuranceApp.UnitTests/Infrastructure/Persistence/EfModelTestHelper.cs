using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace InsuranceApp.UnitTests.Infrastructure.Persistence;

internal static class EfModelTestHelper
{
    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    }

    public static IMutableModel BuildModel<TEntity>(Action<EntityTypeBuilder<TEntity>> configure)
        where TEntity : class
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        using var ctx = new TestDbContext(options);

        var conventions = ConventionSet.CreateConventionSet(ctx);
        var modelBuilder = new ModelBuilder(conventions);

        var builder = modelBuilder.Entity<TEntity>();
        configure(builder);

        return modelBuilder.Model;
    }
}
