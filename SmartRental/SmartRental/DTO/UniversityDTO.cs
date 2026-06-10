using System.ComponentModel.DataAnnotations;

namespace SmartRental.DTO
{
    public class UniversityDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]

        public string City { get; set; }
        [Required]

        public string Area { get; set; }
    }
}
