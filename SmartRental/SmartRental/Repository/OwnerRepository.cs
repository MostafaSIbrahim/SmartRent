using Microsoft.EntityFrameworkCore;
using SmartRental.Data;
using SmartRental.Models.Entites;

namespace SmartRental.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly SmartRentalContext _context;
        public OwnerRepository(SmartRentalContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Owner entity)
        {
           await _context.Owners.AddAsync(entity);
        }

        public void Delete(Owner entity)
        {
            _context.Owners.Remove(entity);
        }

        public async Task<IReadOnlyList<Owner>> GetAllAsync()
        {
         return await  _context.Owners.ToListAsync();
        }

        public async Task<Owner> GetByIdAsync(int id)
       => await _context.Owners.FindAsync(id);

        public async Task<int> SaveChangesAsync()
          => await _context.SaveChangesAsync();

        public void Update(Owner entity)
           => _context.Owners.Update(entity);
    }
}
