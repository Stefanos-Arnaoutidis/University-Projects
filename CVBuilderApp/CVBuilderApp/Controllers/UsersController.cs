using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using CVBuilderApp.Data;
using CVBuilderApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVBuilderApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Βοηθητική μέθοδος για την κρυπτογράφηση των κωδικών με τον αλγόριθμο SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Φόρτωση φόρμας εγγραφής
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Επεξεργασία δεδομένων εγγραφής
        [HttpPost]
        public IActionResult Register(User user)
        {
            // Έλεγχος αν το email υπάρχει ήδη στη βάση
            var emailExists = _context.Users.Any(u => u.Email == user.Email);
            if (emailExists)
            {
                ViewBag.Error = "Αυτό το email χρησιμοποιείται ήδη από άλλον λογαριασμό!";
                return View(user);
            }

            // Κρυπτογράφηση του κωδικού πριν αποθηκευτεί στη βάση
            user.PasswordHash = HashPassword(user.PasswordHash);

            // Ορισμός κατάστασης έγκρισης ανάλογα με τον ρόλο
            if (user.Role == "Counselor")
            {
                // Οι σύμβουλοι χρειάζονται έγκριση από διαχειριστή
                user.IsApproved = false;
            }
            else
            {
                // Προεπιλεγμένος ρόλος και αυτόματη έγκριση για όλους τους άλλους
                user.Role = "User";
                user.IsApproved = true;

                // Καθαρισμός ειδικότητας για να το δεχτεί η βάση
                user.Specialty = "";
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        // Φόρτωση φόρμας σύνδεσης
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Διαδικασία αυθεντικοποίησης χρήστη
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // ΝΕΟ: Κρυπτογράφηση του κωδικού που πληκτρολόγησε ο χρήστης για να γίνει η σύγκριση
            var hashedPassword = HashPassword(password);

            // Αναζήτηση χρήστη με τα συγκεκριμένα διαπιστευτήρια
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);

            if (user != null)
            {
                // Έλεγχος έγκρισης λογαριασμού
                if (user.IsApproved == false)
                {
                    ViewBag.Error = "Ο λογαριασμός σας εκκρεμεί προς έγκριση από τον Διαχειριστή.";
                    return View();
                }

                // Δημιουργία Claims (ιδιοτήτων) για το Cookie αυθεντικοποίησης
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };
                // Προσθήκη ειδικότητας στα Claims αν υπάρχει (για τους Συμβούλους)
                if (!string.IsNullOrEmpty(user.Specialty))
                {
                    claims.Add(new Claim("Specialty", user.Specialty));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Ρύθμιση για να διατηρείται η σύνδεση ενεργή
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                // Δημιουργία συνόδου (Session/Cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            // Σφάλμα σε περίπτωση λανθασμένων στοιχείων
            ViewBag.Error = "Λάθος email ή κωδικός πρόσβασης!";
            return View();
        }

        // Διαδικασία αποσύνδεσης
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Διαγραφή του Cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Καθαρισμός τυχόν υπολειμμάτων cookies από τον browser
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            // Ανακατεύθυνση στην αρχική σελίδα
            return RedirectToAction("Index", "Home");
        }
    }
}