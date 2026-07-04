using System.Reflection;
using Domain.Entities;
using Memo.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Context;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<Seller> Sellers => Set<Seller>();
    
    public AppDbContext() {}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=memo.db;Foreign Keys=True;", o =>
        {
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await base.SaveChangesAsync(ct);
    }
}