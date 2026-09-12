using System.Diagnostics;
using CVBuilderApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CVBuilderApp.Controllers
{
    // Ο βασικός Controller που διαχειρίζεται τις δημόσιες σελίδες του site
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Φορτώνει την αρχική  σελίδα
        public IActionResult Index()
        {
            return View();
        }

        // Σελίδα με τους όρους χρήσης
        public IActionResult Privacy()
        {
            return View();
        }

        // Πιάνει τα λάθη του συστήματος και δείχνει μια οθόνη σφάλματος
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}