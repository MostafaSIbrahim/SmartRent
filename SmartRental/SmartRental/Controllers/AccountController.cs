using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartRental.DTO;
using SmartRental.Models.Entites;
using SmartRental.Models.Entites.Identity;
using SmartRental.Reporisitory;
using SmartRental.Repository;
using System.Data;
//using SmartRental.Services;

namespace SmartRental.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IGenericRepository<Owner> _ownerRepository;
        private readonly IGenericRepository<Tenant> _tenantRepository;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
    IGenericRepository<Owner> ownerRepository,
    IGenericRepository<Tenant> tenantRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _ownerRepository = ownerRepository;
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return View(registerDTO);
            if (await CheckEmail(registerDTO.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is already in use.");
                return View(registerDTO);
            }
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
                    .Select(p => new Phones
                    {
                        PhoneNumber = p
                    })
                    .ToList() ?? new List<Phones>()
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(registerDTO);
            }

            if (registerDTO.Role == "Owner")
                await _userManager.AddToRoleAsync(user, "Owner");
            else if (registerDTO.Role == "Tenant")
            {
                await _userManager.AddToRoleAsync(user, "Tenant");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            
            if (registerDTO.Role == "Owner")
            {
                var owner = new Owner
                {
                    AppUserId = user.Id,
                };
                await _ownerRepository.AddAsync(owner);
                await _ownerRepository.SaveChangesAsync();

            }
            else if (registerDTO.Role == "Tenant")
            {
                var tenant = new Tenant
                {
                    AppUserId = user.Id,
                    UniversityId = registerDTO.UniversityId.Value

                };
                await _tenantRepository.AddAsync(tenant);
                await _tenantRepository.SaveChangesAsync();  
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return View(loginDTO);

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(loginDTO);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginDTO.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(loginDTO);
            }
            var roles = await _userManager.GetRolesAsync(user);
            var appUserId = user.Id;
            if (roles.Contains("Owner"))
            {
                var owner = await _ownerRepository.GetFirstOrDefaultAsync(o => o.AppUserId == appUserId);
                HttpContext.Session.SetInt32("OwnerId", owner.Id);
            }
            else if (roles.Contains("Tenant"))
            {
                var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.AppUserId == appUserId);
                HttpContext.Session.SetInt32("TenantId", tenant.Id);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<bool> CheckEmail(string email)
              => await _userManager.FindByEmailAsync(email) is not null;
    }
}
