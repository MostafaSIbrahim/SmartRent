using Microsoft.EntityFrameworkCore;
using SmartRental.Data;
using SmartRental.Models.Entites;
using SmartRental.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRental.Reporisitory
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly SmartRentalContext _context;
        public GenericRepository(SmartRentalContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
        => await _context.Set<T>().AddAsync(entity);
        public void Delete(T entity)
        => _context.Set<T>().Remove(entity);
        public void Update(T entity)
          => _context.Set<T>().Update(entity);
        public async Task<IReadOnlyList<T>> GetAllAsync()
         => await _context.Set<T>().ToListAsync();
        public async Task<T> GetByIdAsync(int id)
        => await _context.Set<T>().FindAsync(id);
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

    }
}