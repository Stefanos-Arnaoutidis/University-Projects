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
    public class CoursesController : Controller
    {
        private readonly UniversityGradesDbContext _context;

        public CoursesController(UniversityGradesDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            // Κώδικας Ασφαλείας: Μόνο Γραμματεία
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var universityGradesDbContext = _context.Courses.Include(c => c.ProfessorsAfmNavigation);
            return View(await universityGradesDbContext.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.ProfessorsAfmNavigation)
                .FirstOrDefaultAsync(m => m.IdCourse == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            // Φορτώνουμε τη λίστα με τα ΑΦΜ των καθηγητών για το Dropdown
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Afm");
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCourse,CourseTitle,CourseSemester,ProfessorsAfm")] Course course)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            ModelState.Remove("ProfessorsAfmNavigation");

            // Ελέγχουμε αν υπάρχει ήδη μάθημα με ίδιο Τίτλο && Εξάμηνο && Καθηγητή
            bool exists = _context.Courses.Any(c => c.CourseTitle == course.CourseTitle
                                                 && c.CourseSemester == course.CourseSemester
                                                 && c.ProfessorsAfm == course.ProfessorsAfm);

            if (exists)
            {
                ModelState.AddModelError("", "Αυτό το μάθημα υπάρχει ήδη με τον ίδιο καθηγητή στο ίδιο εξάμηνο!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Afm", course.ProfessorsAfm);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Afm", course.ProfessorsAfm);
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCourse,CourseTitle,CourseSemester,ProfessorsAfm")] Course course)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id != course.IdCourse)
            {
                return NotFound();
            }

            // Fix για το Navigation Property
            ModelState.Remove("ProfessorsAfmNavigation");

            // Ελέγχουμε αν υπάρχει άλλο μάθημα με τα ίδια στοιχεία
            bool exists = _context.Courses.Any(c => c.CourseTitle == course.CourseTitle
                                                 && c.CourseSemester == course.CourseSemester
                                                 && c.ProfessorsAfm == course.ProfessorsAfm
                                                 && c.IdCourse != id);

            if (exists)
            {
                ModelState.AddModelError("", "Υπάρχει ήδη άλλη εγγραφή με αυτά τα στοιχεία!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.IdCourse))
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
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Afm", course.ProfessorsAfm);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.ProfessorsAfmNavigation)
                .FirstOrDefaultAsync(m => m.IdCourse == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Admin" && role != "Secretary") return RedirectToAction("Index", "Login");

            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.IdCourse == id);
        }
    }
}