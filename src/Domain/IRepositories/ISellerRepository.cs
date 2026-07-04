using Domain.Entities;

namespace Domain.IRepositories;

public interface ISellerRepository
{
    Task<IEnumerable<Seller>> GetAllAsync(int page, int pageSize);
    Task<int> GetTotalAsync(CancellationToken ct);
    Task<Seller?> GetByIdAsync(Guid id);
    void Add(Seller seller);
    void Update(Seller seller);
    void Delete(Seller seller);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name, Guid id);
    Task<bool> ExistsByLoginAsync(string login);
    Task<bool> ExistsByLoginAsync(string login, Guid id);
}
