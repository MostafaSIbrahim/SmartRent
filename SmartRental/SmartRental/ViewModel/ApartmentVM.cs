using SmartRental.Models.Entites;

namespace SmartRental.ViewModel
{
    public class ApartmentVM
    {
        public int Id { get; set; }

        public byte Rooms { get; set; }
        public byte Bathrooms { get; set; }
        public byte MaxTenants { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public Gender Gender { get; set; }
        public Status AvailabilityStatus { get; set; }
        public string City { get; set; }
        public string StreetName { get; set; }
        public int BuildingNumber { get; set; }
        public byte ApartmentNumber { get; set; }
        public byte FloorNumber { get; set; }
        public int OwnerId { get; set; }

        public List<IFormFile>? image { get; set; }

        public List<PhotoVM>? CurrentPhotos { get; set; }
    }
}