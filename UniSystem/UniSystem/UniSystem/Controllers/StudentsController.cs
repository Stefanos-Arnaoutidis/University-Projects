using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniSystem.Models;
using Microsoft.AspNetCore.Http;

namespace UniSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityGradesDbContext _context;

        public StudentsController(UniversityGradesDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            // Κώδικας Ασφαλείας: Μόνο Γραμματεία βλέπει τη λίστα όλων
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var universityGradesDbContext = _context.Students.Include(s => s.UsersUsernameNavigation);
            return View(await universityGradesDbContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            // Βρίσκουμε ποια Usernames χρησιμοποιούνται ήδη από άλλους φοιτητές
            var takenUsernames = _context.Students.Select(s => s.UsersUsername).ToList();

            // Φέρνουμε χρήστες που είναι "Student" και δεν είναι στη λίστα των takenUsernames
            var availableUsers = _context.Users
                .Where(u => u.Role == "Student" && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username");
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationNumber,Name,Surname,Department,UsersUsername")] Student student)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            ModelState.Remove("UsersUsernameNavigation");

            // Υπάρχει ήδη αυτό το ΑΜ;
            if (_context.Students.Any(s => s.RegistrationNumber == student.RegistrationNumber))
            {
                ModelState.AddModelError("RegistrationNumber", "Αυτό το ΑΜ υπάρχει ήδη!");
            }

            // Υπάρχει ήδη αυτό το Username σε άλλον φοιτητή;
            if (_context.Students.Any(s => s.UsersUsername == student.UsersUsername))
            {
                ModelState.AddModelError("UsersUsername", "Αυτός ο χρήστης έχει ήδη συνδεθεί με φοιτητή!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Αν αποτύχει, ξαναγεμίζουμε τη λίστα
            var allStudents = _context.Users.Where(u => u.Role == "Student");
            ViewData["UsersUsername"] = new SelectList(allStudents, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            // Βρίσκουμε τα Usernames που είναι πιασμένα από άλλους
            var takenUsernames = _context.Students
                .Where(s => s.UsersUsername != student.UsersUsername) // Εξαιρούμε τον εαυτό μας
                .Select(s => s.UsersUsername)
                .ToList();

            // Φέρνουμε τους διαθέσιμους φοιτητές που δεν είναι στη λίστα taken
            var availableUsers = _context.Users
                .Where(u => u.Role == "Student" && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistrationNumber,Name,Surname,Department,UsersUsername")] Student student)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            // Αν άλλαξε το RegistrationNumber στη φόρμα, πετάει σφάλμα ασφάλεια
            if (id != student.RegistrationNumber) return NotFound();

            ModelState.Remove("UsersUsernameNavigation");

            // Αν το νέο Username που διάλεξε το έχει άλλος;
            var usernameTaken = _context.Students.Any(s => s.UsersUsername == student.UsersUsername && s.RegistrationNumber != id);
            if (usernameTaken)
            {
                ModelState.AddModelError("UsersUsername", "Αυτός ο χρήστης ανήκει ήδη σε άλλον φοιτητή!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.RegistrationNumber)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Αν κάτι πήγε λάθος, ξαναγεμίζουμε τη λίστα
            var takenUsernames = _context.Students
                .Where(s => s.UsersUsername != student.UsersUsername)
                .Select(s => s.UsersUsername).ToList();
            var availableUsers = _context.Users
                .Where(u => u.Role == "Student" && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.RegistrationNumber == id);
        }

        // GET: Students/MyGrades
        public async Task<IActionResult> MyGrades()
        {
            var username = HttpContext.Session.GetString("username");
            var role = HttpContext.Session.GetString("role");

            // Έλεγχος αν είναι φοιτητής
            if (username == null || role != "Student") return RedirectToAction("Index", "Login");

            // Βρίσκουμε τον φοιτητή
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UsersUsername == username);

            // Αν υπάρχει username αλλά δεν έχει συνδεθεί με φοιτητή
            if (student == null)
            {
                // Επιστρέφουμε την σελίδα σφάλματος
                return View("ProfilePending");
            }

            // Βρίσκουμε τους βαθμούς του
            var myGrades = await _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Where(s => s.StudentsRegistrationNumber == student.RegistrationNumber)
                .ToListAsync();

            return View(myGrades);
        }
    }
}