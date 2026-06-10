using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartRental.DTO;
using SmartRental.Models.Entites;
using SmartRental.Models.Entites.Identity;
using SmartRental.Repository;

namespace SmartRental.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IOwnerRepository ownerRepository;
        private readonly ITenantRepository tenantRepository;
        private readonly IApartmentRepository _apartmentRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUniverisityRepository _universityRepository;

        public AdminController(
              UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
           IOwnerRepository _ownerRepo,
            ITenantRepository tenantRepo,
            IApartmentRepository apartmentRepo,
           
            IUniverisityRepository universityRepository)
        {
            ownerRepository = _ownerRepo;
            tenantRepository = tenantRepo;
            _apartmentRepo = apartmentRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _universityRepository = universityRepository;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            ViewBag.OwnersCount = (await ownerRepository.GetAllAsync()).Count;
            ViewBag.TenantsCount = (await tenantRepository.GetAllAsync()).Count;
            ViewBag.ApartmentsCount = (await _apartmentRepo.GetAllWithPhotosAsync()).Count;
            ViewBag.UniversitiesCount = (await _universityRepository.GetAllAsync()).Count;

            return View();
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Owners()
        {
            var owners = await ownerRepository.GetAllAsync();
            return View("Owners", owners);
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var owner = await ownerRepository.GetByIdAsync(id);

            if (owner == null)
            {
                return NotFound();
            }

            var appUserId = owner.AppUserId;

            ownerRepository.Delete(owner);
            await ownerRepository.SaveChangesAsync();

            if (!string.IsNullOrEmpty(appUserId))
            {
                var user = await _userManager.FindByIdAsync(appUserId);

                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);

                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                }
            }

            return RedirectToAction("Owners");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Tenants()
        {
            var tenants = await tenantRepository.GetAllAsync();
            return View("Tenants", tenants);

        }
        //[Authorize(Roles = "Admin")]

        //public async Task<IActionResult> DeleteTenant(int id)
        //{
        //    var tenant = await tenantRepository.GetByIdAsync(id);

        //    if (tenant == null)
        //    {
        //        return NotFound();
        //    }

        //    var appUserId = tenant.AppUserId;

        //    tenantRepository.Delete(tenant);
        //    await tenantRepository.SaveChangesAsync();

        //    if (!string.IsNullOrEmpty(appUserId))
        //    {
        //        var user = await _userManager.FindByIdAsync(appUserId);

        //        if (user != null)
        //        {
        //            var result = await _userManager.DeleteAsync(user);

        //            if (!result.Succeeded)
        //            {
        //                return BadRequest(result.Errors);
        //            }
        //        }
        //    }

        //    return RedirectToAction("Owners");
        //}
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return View(registerDTO);

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already in use.");
                return View(registerDTO);
            }

            // Create AppUser
            var user = new AppUser
            {
                UserName = new string(registerDTO.Name
                    .Where(char.IsLetterOrDigit)
                    .ToArray()),
                Email = registerDTO.Email,
                NationalID = registerDTO.NationalID,
                Photo = registerDTO.Photo,
                CreatedAt = DateTime.UtcNow,
                Phones = registerDTO.PhoneNumber?
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => new Phones { PhoneNumber = p })
                    .ToList() ?? new List<Phones>()
            };

            // Create user in Identity
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(registerDTO);
            }

            // Assign role
            if (registerDTO.Role == "Owner")
                await _userManager.AddToRoleAsync(user, "Owner");
            else if (registerDTO.Role == "Tenant")
                await _userManager.AddToRoleAsync(user, "Tenant");
            else
                await _userManager.AddToRoleAsync(user, "Admin");

            if (registerDTO.Role == "Owner")
            {
                var owner = new Owner { AppUserId = user.Id };
                await ownerRepository.AddAsync(owner);
                await ownerRepository.SaveChangesAsync();
            }
            else if (registerDTO.Role == "Tenant")
            {
                if (!registerDTO.UniversityId.HasValue)
                {
                    ModelState.AddModelError(string.Empty, "University is required for tenants.");
                    return View(registerDTO);
                }

                var tenant = new Tenant
                {
                    AppUserId = user.Id,
                    UniversityId = registerDTO.UniversityId.Value
                };
                await tenantRepository.AddAsync(tenant);
                await tenantRepository.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddUniversity()
        {
            return View("AddUniversity");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUniversity(UniversityDTO universityDTO)
        {
            if (!ModelState.IsValid)
                return View("AddUniversity", universityDTO);

            var university = new University
            {
                Name = universityDTO.Name,
                City = universityDTO.City,
                Area = universityDTO.Area
            };

            await _universityRepository.AddAsync(university);
            await _universityRepository.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
