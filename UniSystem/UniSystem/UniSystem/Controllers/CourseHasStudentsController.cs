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
    public class CourseHasStudentsController : Controller
    {
        private readonly UniversityGradesDbContext _context;

        public CourseHasStudentsController(UniversityGradesDbContext context)
        {
            _context = context;
        }

        // GET: CourseHasStudents
        public async Task<IActionResult> Index(int? courseId)
        {
            // Κώδικας Ασφαλείας: Επιτρέπεται σε Καθηγητές (για βαθμολόγηση) και Γραμματεία
            var role = HttpContext.Session.GetString("role");
            if (role != "Professor" && role != "Secretary" && role != "Admin")
            {
                return RedirectToAction("Index", "Login");
            }

            var grades = _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Include(c => c.StudentsRegistrationNumberNavigation)
                .AsQueryable();

            // Αν μας έδωσαν συγκεκριμένο μάθημα (π.χ. πάτησε το κουμπί ο καθηγητής)
            if (courseId != null)
            {
                grades = grades.Where(x => x.CourseIdCourse == courseId);
            }

            return View(await grades.ToListAsync());
        }

        // GET: CourseHasStudents/Details
        public async Task<IActionResult> Details(int? courseId, int? studentId)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role == "Student" || role == null) return RedirectToAction("Index", "Login");

            if (courseId == null || studentId == null)
            {
                return NotFound();
            }

            var courseHasStudent = await _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Include(c => c.StudentsRegistrationNumberNavigation)
                .FirstOrDefaultAsync(m => m.CourseIdCourse == courseId && m.StudentsRegistrationNumber == studentId);

            if (courseHasStudent == null)
            {
                return NotFound();
            }

            return View(courseHasStudent);
        }

        // GET: CourseHasStudents/Create
        public IActionResult Create()
        {
            // Κώδικας Ασφαλείας: Μόνο η Γραμματεία δηλώνει μαθήματα
            var role = HttpContext.Session.GetString("role");
            if (role != "Secretary" && role != "Admin") return RedirectToAction("Index", "Login");

            // Εδώ γεμίζουμε τα Dropdowns με Μαθήματα και Φοιτητές
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "CourseTitle");
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber");
            return View();
        }

        // POST: CourseHasStudents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseIdCourse,StudentsRegistrationNumber,GradeCourseStudent")] CourseHasStudent courseHasStudent)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Secretary" && role != "Admin") return RedirectToAction("Index", "Login");

            // Αγνοούμε τα Navigation properties
            ModelState.Remove("CourseIdCourseNavigation");
            ModelState.Remove("StudentsRegistrationNumberNavigation");

            if (ModelState.IsValid)
            {
                // Έλεγχος αν υπάρχει ήδη αυτή η εγγραφή
                var exists = await _context.CourseHasStudents.AnyAsync(
                    x => x.CourseIdCourse == courseHasStudent.CourseIdCourse &&
                         x.StudentsRegistrationNumber == courseHasStudent.StudentsRegistrationNumber);

                if (exists)
                {
                    ModelState.AddModelError("", "This student is already enrolled in this course.");
                }
                else
                {
                    _context.Add(courseHasStudent);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "CourseTitle", courseHasStudent.CourseIdCourse);
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber", courseHasStudent.StudentsRegistrationNumber);
            return View(courseHasStudent);
        }

        // GET: CourseHasStudents/Edit
        public async Task<IActionResult> Edit(int? courseId, int? studentId)
        {
            // Κώδικας Ασφαλείας: Καθηγητές (βαθμολογία) και Γραμματεία
            var role = HttpContext.Session.GetString("role");
            if (role != "Professor" && role != "Secretary" && role != "Admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (courseId == null || studentId == null)
            {
                return NotFound();
            }

            var courseHasStudent = await _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Include(c => c.StudentsRegistrationNumberNavigation)
                .FirstOrDefaultAsync(m => m.CourseIdCourse == courseId && m.StudentsRegistrationNumber == studentId);

            if (courseHasStudent == null)
            {
                return NotFound();
            }
            return View(courseHasStudent);
        }

        // POST: CourseHasStudents/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int courseId, int studentId, [Bind("CourseIdCourse,StudentsRegistrationNumber,GradeCourseStudent")] CourseHasStudent courseHasStudent)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Professor" && role != "Secretary" && role != "Admin")
            {
                return RedirectToAction("Index", "Login");
            }

            ModelState.Remove("CourseIdCourseNavigation");
            ModelState.Remove("StudentsRegistrationNumberNavigation");

            // Έλεγχος Βαθμολογίας (0-10)
            if (courseHasStudent.GradeCourseStudent != null)
            {
                if (courseHasStudent.GradeCourseStudent < 0 || courseHasStudent.GradeCourseStudent > 10)
                {
                    ModelState.AddModelError("GradeCourseStudent", "Ο βαθμός πρέπει να είναι ακέραιος αριθμός από 0 έως 10.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseHasStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseHasStudentExists(courseHasStudent.CourseIdCourse, courseHasStudent.StudentsRegistrationNumber))
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
            return View(courseHasStudent);
        }

        // GET: CourseHasStudents/Delete
        public async Task<IActionResult> Delete(int? courseId, int? studentId)
        {
            // Κώδικας Ασφαλείας: Μόνο Γραμματεία διαγράφει δηλώσεις
            var role = HttpContext.Session.GetString("role");
            if (role != "Secretary" && role != "Admin") return RedirectToAction("Index", "Login");

            if (courseId == null || studentId == null)
            {
                return NotFound();
            }

            var courseHasStudent = await _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Include(c => c.StudentsRegistrationNumberNavigation)
                .FirstOrDefaultAsync(m => m.CourseIdCourse == courseId && m.StudentsRegistrationNumber == studentId);

            if (courseHasStudent == null)
            {
                return NotFound();
            }

            return View(courseHasStudent);
        }

        // POST: CourseHasStudents/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int courseId, int studentId)
        {
            // Κώδικας Ασφαλείας
            var role = HttpContext.Session.GetString("role");
            if (role != "Secretary" && role != "Admin") return RedirectToAction("Index", "Login");

            var courseHasStudent = await _context.CourseHasStudents
                .FirstOrDefaultAsync(m => m.CourseIdCourse == courseId && m.StudentsRegistrationNumber == studentId);

            if (courseHasStudent != null)
            {
                _context.CourseHasStudents.Remove(courseHasStudent);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CourseHasStudentExists(int courseId, int studentId)
        {
            return _context.CourseHasStudents.Any(e => e.CourseIdCourse == courseId && e.StudentsRegistrationNumber == studentId);
        }
    }
}