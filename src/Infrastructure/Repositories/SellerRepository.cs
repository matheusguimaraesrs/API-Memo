using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class SellerRepository(AppDbContext context) : ISellerRepository
{
    public async Task<IEnumerable<Seller>> GetAllAsync(int page, int pageSize)
    {
        return await context.Set<Seller>()
            .OrderBy(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetTotalAsync(CancellationToken ct)
    {
        return await context.Set<Seller>().AsNoTracking().CountAsync(ct);
    }
    
    public async Task<Seller?> GetByIdAsync(Guid id)
    {
        return await context.Set<Seller>().FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Add(Seller seller)
    {
        context.Set<Seller>().Add(seller);
    }
    public void Update(Seller seller)
    {
        context.Set<Seller>().Update(seller);
    }
    public void Delete(Seller seller)
    {
        context.Set<Seller>().Remove(seller);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await context.Set<Seller>().AnyAsync(s => s.Name.ToLower() == name.ToLower());
    }
    
    public async Task<bool> ExistsByNameAsync(string name, Guid id)
    {
        return await context.Set<Seller>().AnyAsync(s => s.Name.ToLower() == name.ToLower() && s.Id != id);
    }
    
    public async Task<bool> ExistsByLoginAsync(string login)
    {
        return await context.Set<Seller>().AnyAsync(s => s.Login.ToLower() == login.ToLower());
    }
    
    public async Task<bool> ExistsByLoginAsync(string login, Guid id)
    {
        return await context.Set<Seller>().AnyAsync(s => s.Login.ToLower() == login.ToLower() && s.Id != id);
    }
}
