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
    public class UsersController : Controller
    {
        private readonly UniversityGradesDbContext _context;

        public UsersController(UniversityGradesDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary")
            {
                return RedirectToAction("Index", "Login");
            }

            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Username == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            // Λίστα με τους Ρόλους
            var roles = new List<string> { "Student", "Professor", "Secretary", "Admin" };
            ViewData["RoleList"] = new SelectList(roles);

            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,Role")] User user)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Στέλνουμε τη λίστα με τους ρόλους στο View
            var roles = new List<string> { "Student", "Professor", "Secretary", "Admin" };
            ViewData["RoleList"] = new SelectList(roles);

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Username,Password,Role")] User user)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id != user.Username)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Username))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Username == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Username == id);
        }
    }
}