using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRental.Models.Entites;
using SmartRental.Repository;
using SmartRental.ViewModel;

namespace SmartRental.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerController : Controller
    {
        private readonly IApartmentRepository repo;
        private readonly IWebHostEnvironment env;

        public OwnerController(IApartmentRepository _repo,
                               IWebHostEnvironment _env)
        {
            repo = _repo;
            env = _env;
        }

        [HttpGet]

        public async Task<IActionResult> index()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }


            var appUserId = userIdClaim.Value;


            var owner = repo.GetOwnerByAppUserId(appUserId);
            if (owner == null)
            {
                return BadRequest("Owner record not found for this user.");
            }


            var apartments = repo.GetByOwnerId(owner.Id);
            if (apartments == null || !apartments.Any())
            {
                return View("OwnerIndex", new List<Apartment>());
            }
            return View(apartments);
        }
        
        public IActionResult OwnerIndex()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            
            var appUserId = userIdClaim.Value;

           
            var owner = repo.GetOwnerByAppUserId(appUserId); 
            if (owner == null)
            {
                return BadRequest("Owner record not found for this user.");
            }

      
            var apartments = repo.GetByOwnerId(owner.Id);
            if (apartments == null || !apartments.Any())
            {
                return View("OwnerIndex", new List<Apartment>()); 
            }
            return View(apartments);
          
        }

        public IActionResult AddApartment()
        {

            return View("AddApartment");
        }

        [HttpPost]
        public async Task<IActionResult> SaveAdd(ApartmentVM apartmentVM)
        {
            if (!ModelState.IsValid)
            {
                return View("AddApartment", apartmentVM);
            }
            var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (appUserId == null)
            {
                return Unauthorized();
            }
            var owner = repo.GetOwnerByAppUserId(appUserId);
            Apartment apartment = new Apartment()
            {
                Rooms = apartmentVM.Rooms,
                Bathrooms = apartmentVM.Bathrooms,
                MaxTenants = apartmentVM.MaxTenants,
                Description = apartmentVM.Description,
                Price = apartmentVM.Price,
                Gender = apartmentVM.Gender,
                AvailabilityStatus = apartmentVM.AvailabilityStatus,
                City = apartmentVM.City,
                StreetName = apartmentVM.StreetName,
                BuildingNumber = apartmentVM.BuildingNumber,
                ApartmentNumber = apartmentVM.ApartmentNumber,
                FloorNumber = apartmentVM.FloorNumber,
                OwnerId = owner.Id,
            };
            if (apartmentVM.image != null && apartmentVM.image.Count > 0)
            {
                foreach (var file in apartmentVM.image)
                {
                    string fileName =
                        Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string folderPath =
                     Path.Combine(env.WebRootPath, "Images");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath =
                        Path.Combine(folderPath, fileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    apartment.Photos.Add(new Photos()
                    {
                        PhotoUrl = "/Images/" + fileName
                    });
                }
            }

            repo.Add(apartment);

            await repo.SaveChangeAsync();


            return RedirectToAction("index");
        }


        [HttpGet]
        
        public async Task<IActionResult> Update(int id)
        {
            Apartment? apartment = await repo.GetByIdAsync(id);

            if (apartment == null)
                return NotFound();

            ApartmentVM vm = new ApartmentVM()
            {
                Id = apartment.Id,
                Rooms = apartment.Rooms,
                Bathrooms = apartment.Bathrooms,
                MaxTenants = apartment.MaxTenants,
                Description = apartment.Description,
                Price = apartment.Price,
                Gender = apartment.Gender,
                AvailabilityStatus = apartment.AvailabilityStatus,
                City = apartment.City,
                StreetName = apartment.StreetName,
                BuildingNumber = apartment.BuildingNumber,
                ApartmentNumber = apartment.ApartmentNumber,
                FloorNumber = apartment.FloorNumber,
                OwnerId = apartment.OwnerId,

                CurrentPhotos = apartment.Photos
                .Select(p => new PhotoVM()
                {
                    Id = p.Id,
                    Url = p.PhotoUrl
                })
                .ToList()
            };

            return View(vm);
        }
        [HttpPost]
        
        public async Task<IActionResult> Update(ApartmentVM apartmentVM)
        {
            if (!ModelState.IsValid)
                return View(apartmentVM);

            Apartment? apartment =
                await repo.GetByIdAsync(apartmentVM.Id);

            if (apartment == null)
                return NotFound();
            var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (appUserId == null)
            {
                return Unauthorized();
            }
            var owner = repo.GetOwnerByAppUserId(appUserId);
            apartment.Rooms = apartmentVM.Rooms;
            apartment.Bathrooms = apartmentVM.Bathrooms;
            apartment.MaxTenants = apartmentVM.MaxTenants;
            apartment.Description = apartmentVM.Description;
            apartment.Price = apartmentVM.Price;
            apartment.Gender = apartmentVM.Gender;
            apartment.AvailabilityStatus = apartmentVM.AvailabilityStatus;
            apartment.City = apartmentVM.City;
            apartment.StreetName = apartmentVM.StreetName;
            apartment.BuildingNumber = apartmentVM.BuildingNumber;
            apartment.ApartmentNumber = apartmentVM.ApartmentNumber;
            apartment.FloorNumber = apartmentVM.FloorNumber;
            apartment.OwnerId = owner.Id;

            if (apartmentVM.image != null && apartmentVM.image.Count > 0)
            {
                foreach (var file in apartmentVM.image)
                {
                    string fileName =
                        Guid.NewGuid().ToString() +
                        Path.GetExtension(file.FileName);

                    string folderPath =
                        Path.Combine(env.WebRootPath, "Images");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath =
                        Path.Combine(folderPath, fileName); 

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    apartment.Photos.Add(new Photos()
                    {
                        PhotoUrl = "/Images/" + fileName
                    });
                }
            }

            await repo.SaveChangeAsync();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await repo.DeleteAsync(id);
                await repo.SaveChangeAsync();
                TempData["SuccessMessage"] = "Apartment deleted successfully.";
                return RedirectToAction("OwnerIndex");
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("OwnerIndex");
            }
           
        }


    }
}
