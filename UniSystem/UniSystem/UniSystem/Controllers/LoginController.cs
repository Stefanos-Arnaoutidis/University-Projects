using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSystem.Models;

namespace UniSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly UniversityGradesDbContext _context;

        public LoginController(UniversityGradesDbContext context)
        {
            _context = context;
        }

        // GET: Εμφανίζει τη φόρμα εισόδου
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            // Έλεγχος αν υπάρχει ο χρήστης
            var obj = await _context.Users
                .FirstOrDefaultAsync(m => m.Username == user.Username && m.Password == user.Password);

            if (obj != null)
            {
                // Αποθήκευση βασικών στοιχείων στο Session
                HttpContext.Session.SetString("username", obj.Username);
                HttpContext.Session.SetString("role", obj.Role);

                // Εύρεση Ονόματος και Επωνύμου
                string fullName = obj.Username; // βάζουμε το username ως default

                if (obj.Role == "Student")
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.UsersUsername == obj.Username);
                    if (student != null) fullName = student.Name + " " + student.Surname;
                }
                else if (obj.Role == "Professor")
                {
                    var prof = await _context.Professors.FirstOrDefaultAsync(p => p.UsersUsername == obj.Username);
                    if (prof != null) fullName = prof.Name + " " + prof.Surname;
                }
                else if (obj.Role == "Secretary")
                {
                    var secr = await _context.Secretaries.FirstOrDefaultAsync(s => s.UsersUsername == obj.Username);
                    if (secr != null) fullName = secr.Name + " " + secr.Surname;
                }

                // Αποθηκεύουμε το πλήρες όνομα στο Session για να το βλέπουμε παντού
                HttpContext.Session.SetString("fullname", fullName);

                return RedirectToAction("Index", "Home");
            }

            ViewData["Message"] = "Λάθος όνομα χρήστη ή κωδικός";
            return View("Index");
        }

        // Logout: Καθαρίζει το Session
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}