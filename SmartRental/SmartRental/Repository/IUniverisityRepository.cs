using SmartRental.Models.Entites;

namespace SmartRental.Repository
{
    public interface IUniverisityRepository
    {
        public Task<University> GetByIdAsync(int id);
        public Task<IReadOnlyList<University>> GetAllAsync();
        public Task AddAsync(University entity);
        public void Update(University entity);
        public void Delete(University entity);
        Task<int> SaveChangesAsync();
    }
}
