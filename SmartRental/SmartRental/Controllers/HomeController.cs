using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartRental.Models;
using SmartRental.Models.Entites;
using SmartRental.Repository;

namespace SmartRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApartmentRepository apartmentRepo;

        public HomeController(ILogger<HomeController> logger , IApartmentRepository _apartmentRepo)
        {
            _logger = logger;
            apartmentRepo = _apartmentRepo;
        }

        public async Task<IActionResult> Index()
        {
            var apartments =
              await apartmentRepo.GetAllWithPhotosAsync();

            ViewBag.IsLoggedIn =
                User.Identity != null &&
                User.Identity.IsAuthenticated;

            return View(apartments);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
