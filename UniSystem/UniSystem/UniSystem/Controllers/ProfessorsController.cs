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
    public class ProfessorsController : Controller
    {
        private readonly UniversityGradesDbContext _context;

        public ProfessorsController(UniversityGradesDbContext context)
        {
            _context = context;
        }

        // GET: Professors
        public async Task<IActionResult> Index()
        {
            // Κώδικας Ασφαλείας: Μόνο η Γραμματεία βλέπει τη λίστα όλων των καθηγητών
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var universityGradesDbContext = _context.Professors.Include(p => p.UsersUsernameNavigation);
            return View(await universityGradesDbContext.ToListAsync());
        }

        // GET: Professors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .Include(p => p.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Afm == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // GET: Professors/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            // Βρίσκουμε ποια Usernames είναι πιασμένα
            var takenUsernames = _context.Professors.Select(p => p.UsersUsername).ToList();

            // Μόνο Καθηγητές που είναι ελεύθεροι
            var availableUsers = _context.Users
                .Where(u => u.Role == "Professor" && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username");
            return View();
        }

        // POST: Professors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Afm,Name,Surname,Department,UsersUsername")] Professor professor)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            ModelState.Remove("UsersUsernameNavigation");

            // ΕΛΕΓΧΟΣ 1: Υπάρχει ήδη αυτό το ΑΦΜ;
            if (_context.Professors.Any(p => p.Afm == professor.Afm))
            {
                ModelState.AddModelError("Afm", "Αυτό το ΑΦΜ υπάρχει ήδη!");
            }

            // ΕΛΕΓΧΟΣ 2: Υπήρχε ήδη το Username;
            if (_context.Professors.Any(p => p.UsersUsername == professor.UsersUsername))
            {
                ModelState.AddModelError("UsersUsername", "Αυτός ο χρήστης είναι ήδη Καθηγητής!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(professor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var allProfs = _context.Users.Where(u => u.Role == "Professor");
            ViewData["UsersUsername"] = new SelectList(allProfs, "Username", "Username", professor.UsersUsername);
            return View(professor);
        }

        // GET: Professors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null) return NotFound();

            var professor = await _context.Professors.FindAsync(id);
            if (professor == null) return NotFound();

            // Πιασμένα από άλλους καθηγητές
            var takenUsernames = _context.Professors
                .Where(p => p.UsersUsername != professor.UsersUsername)
                .Select(p => p.UsersUsername)
                .ToList();

            var availableUsers = _context.Users
                .Where(u => u.Role == "Professor" && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username", professor.UsersUsername);
            return View(professor);
        }

        // POST: Professors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Afm,Name,Surname,Department,UsersUsername")] Professor professor)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id != professor.Afm) return NotFound();

            ModelState.Remove("UsersUsernameNavigation");

            // Έλεγχος
            if (_context.Professors.Any(p => p.UsersUsername == professor.UsersUsername && p.Afm != id))
            {
                ModelState.AddModelError("UsersUsername", "Αυτός ο χρήστης ανήκει ήδη σε άλλον καθηγητή!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(professor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfessorExists(professor.Afm)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var takenUsernames = _context.Professors
                .Where(p => p.UsersUsername != professor.UsersUsername)
                .Select(p => p.UsersUsername).ToList();
            var availableUsers = _context.Users
                .Where(u => u.Role == "Professor" && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username", professor.UsersUsername);
            return View(professor);
        }

        // GET: Professors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null) return NotFound();

            var professor = await _context.Professors
                .FirstOrDefaultAsync(m => m.Afm == id);

            if (professor == null) return NotFound();

            return View(professor);
        }

        // POST: Professors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var professor = await _context.Professors.FindAsync(id);
            if (professor != null)
            {
                _context.Professors.Remove(professor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProfessorExists(int id)
        {
            return _context.Professors.Any(e => e.Afm == id);
        }

        // GET: Professors/MyCourses
        public async Task<IActionResult> MyCourses()
        {
            var username = HttpContext.Session.GetString("username");
            var role = HttpContext.Session.GetString("role");

            // Αν δεν είναι συνδεδεμένος ή δεν είναι καθηγητής
            if (username == null || role != "Professor") return RedirectToAction("Index", "Login");

            // Βρίσκουμε τον καθηγητή με βάση το username
            var professor = await _context.Professors
                .FirstOrDefaultAsync(m => m.UsersUsername == username);

            // Αν υπάρχει username αλλά δεν υπάρχει εγγραφή στον πίνακα Professors
            if (professor == null)
            {
                // Επιστρέφουμε την σελίδα σφάλματος
                return View("ProfilePending");
            }

            // Βρίσκουμε τα μαθήματα του συγκεκριμένου καθηγητή
            var myCourses = await _context.Courses
                .Where(c => c.ProfessorsAfm == professor.Afm)
                .ToListAsync();

            return View(myCourses);
        }
    }
}