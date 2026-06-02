using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SmartRental.Data;
using SmartRental.Models.Entites;

namespace SmartRental.Repository
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly SmartRentalContext context;

        public ApartmentRepository(SmartRentalContext _context)
        {
            context = _context;
        }

        public  void  Add(Apartment Apartment)
        {
            context.Apartments.Add(Apartment);
        }

        public  async Task DeleteAsync(int id)
        {
          Apartment? apartment = await context.Apartments.FirstOrDefaultAsync(o => o.Id == id);
          if (apartment != null)
          {
            context.Apartments.Remove(apartment);

          }
            else
            {
                throw new KeyNotFoundException("Apartment Not found ");
            }
        }

        public  async Task SaveChangeAsync()
        {
            await context.SaveChangesAsync();
        }

        public void Update(Apartment Apartment)
        {
            context.Apartments.Update(Apartment);
           
        }

        public async Task<Apartment?> GetByIdAsync(int id)
        {
            return await context.Apartments
                                .Include(a => a.Photos)
                                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public IEnumerable<Apartment> GetByOwnerId(int ownerId)
        {
            return context.Apartments
        .Where(a => a.OwnerId == ownerId)
        .ToList();
        }
    }
}
