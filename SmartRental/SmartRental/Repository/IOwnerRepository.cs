using SmartRental.Models.Entites;

namespace SmartRental.Repository
{
    public interface IOwnerRepository
    {
        public Task<Owner> GetByIdAsync(int id);
        public Task<IReadOnlyList<Owner>> GetAllAsync();
        public Task AddAsync(Owner entity);
        public void Update(Owner entity);
        public void Delete(Owner entity);
        Task<int> SaveChangesAsync();
    }
}
