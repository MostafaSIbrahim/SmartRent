using Microsoft.EntityFrameworkCore;
using SmartRental.Data;
using SmartRental.Models.Entites;

namespace SmartRental.Repository
{
    public class UniverisityRepository : IUniverisityRepository
    {
        private readonly SmartRentalContext _context;
        public UniverisityRepository(SmartRentalContext context)
        {
            _context = context;
        }
        public async Task AddAsync(University entity)
        =>await _context.Universities.AddAsync(entity);

        public void Delete(University entity)
        => _context.Universities.Remove(entity);

        public async Task<IReadOnlyList<University>> GetAllAsync()
        => await _context.Universities.ToListAsync();

        public Task<University> GetByIdAsync(int id)
        => _context.Universities.FirstOrDefaultAsync(u => u.Id == id);


        public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

        public void Update(University entity)
       => _context.Universities.Update(entity);
    }
}
