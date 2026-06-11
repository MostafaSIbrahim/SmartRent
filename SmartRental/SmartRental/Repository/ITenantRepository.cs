using SmartRental.Models.Entites;

namespace SmartRental.Repository
{
    public interface ITenantRepository

    {
        public Task<Tenant> GetByIdAsync(int id);
        public Task<IReadOnlyList<Tenant>> GetAllAsync();
        public Task AddAsync(Tenant entity);
        public void Update(Tenant entity);
        public void Delete(Tenant entity);
        Task<int> SaveChangesAsync();
        Task<Tenant?> GetByAppUserIdAsync(string appUserId);
    }
}
