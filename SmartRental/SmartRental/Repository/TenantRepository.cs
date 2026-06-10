using Microsoft.EntityFrameworkCore;
using SmartRental.Data;
using SmartRental.Models.Entites;

namespace SmartRental.Repository
{
    public class TenantRepository : ITenantRepository
    {
        private readonly SmartRentalContext _context;
        public TenantRepository(SmartRentalContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Tenant entity)
        {
            await _context.Tenants.AddAsync(entity);
        }

        public void Delete(Tenant entity)
        {
            _context.Tenants.Remove(entity);
        }

        public async Task<IReadOnlyList<Tenant>> GetAllAsync()
        {
            return await _context.Tenants.ToListAsync();
        }

        public async Task<Tenant> GetByIdAsync(int id)
       => await _context.Tenants.FindAsync(id);

        public async Task<int> SaveChangesAsync()
          => await _context.SaveChangesAsync();

        public void Update(Tenant entity)
           => _context.Tenants.Update(entity);
    }
}
