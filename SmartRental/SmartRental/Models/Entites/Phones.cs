using SmartRental.Models.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRental.Models.Entites
{
    public class Phones : BaseEntity
    {
        public string PhoneNumber { get; set; }
       public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
