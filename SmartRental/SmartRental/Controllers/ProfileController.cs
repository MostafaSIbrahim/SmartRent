using ApartmentRental.Reporisitory.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRental.Models.Entites.Identity;
using SmartRental.Repository;
using SmartRental.ViewModel;

namespace SmartRental.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly AppIdentityDbContext _identityContext;

        public ProfileController(
            UserManager<AppUser> userManager,
            ITenantRepository tenantRepository,
            AppIdentityDbContext identityContext)
        {
            _userManager = userManager;
            _tenantRepository = tenantRepository;
            _identityContext = identityContext;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser =
                await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return NotFound();

            var user =
                await _identityContext.Users
                .Include(u => u.Phones)
                .FirstOrDefaultAsync(u => u.Id == currentUser.Id);

            if (user == null)
                return NotFound();

            var roles =
                await _userManager.GetRolesAsync(user);

            string role =
                roles.FirstOrDefault() ?? "";

            string universityName = "";

            if (role == "Tenant")
            {
                var tenant =
                    await _tenantRepository
                    .GetByAppUserIdAsync(user.Id);

                if (tenant != null)
                {
                    universityName =
                        tenant.University?.Name ?? "";
                }
            }

            ProfileVM vm = new()
            {
                Name = user.UserName,
                Email = user.Email,
                NationalID = user.NationalID,
                Photo = user.Photo,

                Phones = user.Phones
                    .Select(p => p.PhoneNumber)
                    .ToList(),

                Role = role,

                UniversityName = universityName
            };

            Console.WriteLine($"Phones Count = {user.Phones.Count}");

            return View(vm);
        }
    }
}