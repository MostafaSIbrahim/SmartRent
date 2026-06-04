using SmartRental.Models.Entites;

namespace SmartRental.Repository
{
    public interface IApartmentRepository
    {
        void Add(Apartment Apartment);
        void Update(Apartment Apartment);
        public Task DeleteAsync(int id);
        public Task SaveChangeAsync();
        Task<Apartment?> GetByIdAsync(int id);
        IEnumerable<Apartment> GetByOwnerId(int ownerId);
        Owner GetOwnerByAppUserId(string appUserId);
    }
}
