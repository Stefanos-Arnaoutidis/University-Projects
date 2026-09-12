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
    public class SecretariesController : Controller
    {
        private readonly UniversityGradesDbContext _context;

        public SecretariesController(UniversityGradesDbContext context)
        {
            _context = context;
        }

        // GET: Secretaries
        public async Task<IActionResult> Index()
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var universityGradesDbContext = _context.Secretaries.Include(s => s.UsersUsernameNavigation);
            return View(await universityGradesDbContext.ToListAsync());
        }

        // GET: Secretaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Phonenumber == id);
            if (secretary == null)
            {
                return NotFound();
            }

            return View(secretary);
        }

        // GET: Secretaries/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var takenUsernames = _context.Secretaries.Select(s => s.UsersUsername).ToList();

            var availableUsers = _context.Users
                .Where(u => (u.Role == "Secretary" || u.Role == "Admin") && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username");
            return View();
        }

        // POST: Secretaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Phonenumber,Name,Surname,Department,UsersUsername")] Secretary secretary)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            ModelState.Remove("UsersUsernameNavigation");

            // ΕΛΕΓΧΟΣ 1: Υπάρχει ήδη το τηλέφωνο;
            if (_context.Secretaries.Any(s => s.Phonenumber == secretary.Phonenumber))
            {
                ModelState.AddModelError("Phonenumber", "Αυτό το τηλέφωνο υπάρχει ήδη!");
            }

            // ΕΛΕΓΧΟΣ 2: Υπάρχει ήδη το Username;
            if (_context.Secretaries.Any(s => s.UsersUsername == secretary.UsersUsername))
            {
                ModelState.AddModelError("UsersUsername", "Αυτός ο χρήστης είναι ήδη Γραμματεία!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(secretary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var allSecr = _context.Users.Where(u => u.Role == "Secretary" || u.Role == "Admin");
            ViewData["UsersUsername"] = new SelectList(allSecr, "Username", "Username", secretary.UsersUsername);
            return View(secretary);
        }

        // GET: Secretaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null) return NotFound();

            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary == null) return NotFound();

            var takenUsernames = _context.Secretaries
                .Where(s => s.UsersUsername != secretary.UsersUsername)
                .Select(s => s.UsersUsername)
                .ToList();

            var availableUsers = _context.Users
                .Where(u => (u.Role == "Secretary" || u.Role == "Admin") && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username", secretary.UsersUsername);
            return View(secretary);
        }

        // POST: Secretaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Phonenumber,Name,Surname,Department,UsersUsername")] Secretary secretary)
        {
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id != secretary.Phonenumber) return NotFound();

            ModelState.Remove("UsersUsernameNavigation");

            if (_context.Secretaries.Any(s => s.UsersUsername == secretary.UsersUsername && s.Phonenumber != id))
            {
                ModelState.AddModelError("UsersUsername", "Αυτός ο χρήστης ανήκει ήδη σε άλλη γραμματεία!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(secretary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecretaryExists(secretary.Phonenumber)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var takenUsernames = _context.Secretaries
                .Where(s => s.UsersUsername != secretary.UsersUsername)
                .Select(s => s.UsersUsername).ToList();
            var availableUsers = _context.Users
                .Where(u => (u.Role == "Secretary" || u.Role == "Admin") && !takenUsernames.Contains(u.Username));

            ViewData["UsersUsername"] = new SelectList(availableUsers, "Username", "Username", secretary.UsersUsername);
            return View(secretary);
        }

        // GET: Secretaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Phonenumber == id);
            if (secretary == null)
            {
                return NotFound();
            }

            return View(secretary);
        }

        // POST: Secretaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary != null)
            {
                _context.Secretaries.Remove(secretary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecretaryExists(int id)
        {
            return _context.Secretaries.Any(e => e.Phonenumber == id);
        }
    }
}