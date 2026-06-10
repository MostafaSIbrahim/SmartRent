using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartRental.DTO;
using SmartRental.Models.Entites;
using SmartRental.Models.Entites.Identity;
using SmartRental.Repository;
//using SmartRental.Services;

namespace SmartRental.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ITenantRepository _tenantRepository;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IOwnerRepository ownerRepository,
            ITenantRepository tenantRepository)
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

            // Sign in user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Add domain-specific entity (Owner or Tenant)
            if (registerDTO.Role == "Owner")
            {
                var owner = new Owner { AppUserId = user.Id };
                await _ownerRepository.AddAsync(owner);
                await _ownerRepository.SaveChangesAsync();
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

            return RedirectToAction("Index", "Home");
        }
        public async Task<bool> CheckEmail(string email)
              => await _userManager.FindByEmailAsync(email) is not null;
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }

}
