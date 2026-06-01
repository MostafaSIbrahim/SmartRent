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
        private readonly IGenericRepository<Apartment> ApartmentRepo;

        public HomeController(ILogger<HomeController> logger , IGenericRepository<Apartment> _ApartmentRepo)
        {
            _logger = logger;
            ApartmentRepo = _ApartmentRepo;
        }

        public async Task<IActionResult> Index()
        {
            var Apartment = await ApartmentRepo.GetAllAsync();
            return View(Apartment);
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
