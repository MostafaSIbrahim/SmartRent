using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRental.Models.Entites
{
    public class Owner : BaseEntity
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } 
        public ICollection<Apartment> Apartments { get; set; } = new HashSet<Apartment>();
    }
}
