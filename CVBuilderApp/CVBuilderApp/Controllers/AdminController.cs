using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CVBuilderApp.Data;
using CVBuilderApp.Models;
using System.Linq;

namespace CVBuilderApp.Controllers
{
    [Authorize(Roles = "Admin")] // Περιορισμός πρόσβασης μόνο στον διαχειριστή
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Έγκριση συμβούλων καριέρας
        [HttpGet]
        public IActionResult Index()
        {
            // Λίστα με τους συμβούλους που περιμένουν έγκριση
            var pendingCounselors = _context.Users
                .Where(u => u.Role == "Counselor" && u.IsApproved == false)
                .ToList();
            return View(pendingCounselors);
        }

        [HttpPost]
        public IActionResult ApproveCounselor(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.IsApproved = true; // Αλλαγή κατάστασης σε εγκεκριμένο
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Διαχείριση προτύπων για τα βιογραφικά
        [HttpGet]
        public IActionResult Templates()
        {
            // Φόρτωση των διαθέσιμων προτύπων από τη βάση δεδομένων
            var templates = _context.Templates.ToList();
            return View(templates);
        }

        [HttpPost]
        public IActionResult AddTemplate(string name, string description)
        {
            var newTemplate = new Template
            {
                Name = name,
                Description = description
            };
            _context.Templates.Add(newTemplate);
            _context.SaveChanges();
            return RedirectToAction("Templates");
        }

        // Ενημέρωση υπαρχόντων στοιχείων ενός προτύπου
        [HttpPost]
        public IActionResult EditTemplate(int id, string name, string description)
        {
            var template = _context.Templates.FirstOrDefault(t => t.Id == id);
            if (template != null)
            {
                template.Name = name;
                template.Description = description;
                _context.SaveChanges();
            }
            return RedirectToAction("Templates");
        }

        // Οριστική διαγραφή προτύπου από τη βάση
        [HttpPost]
        public IActionResult DeleteTemplate(int id)
        {
            var template = _context.Templates.FirstOrDefault(t => t.Id == id);
            if (template != null)
            {
                _context.Templates.Remove(template);
                _context.SaveChanges();
            }
            return RedirectToAction("Templates");
        }
        // Διαχείριση προκαθορισμένων λιστών
        [HttpGet]
        public IActionResult ManageLists()
        {
            // Φόρτωση όλων των προκαθορισμένων επιλογών
            var items = _context.PredefinedLists.ToList();
            return View(items);
        }

        [HttpPost]
        public IActionResult AddListItem(string name, string type)
        {
            var newItem = new PredefinedList
            {
                Name = name,
                Type = type
            };
            _context.PredefinedLists.Add(newItem);
            _context.SaveChanges();

            return RedirectToAction("ManageLists");
        }

        [HttpPost]
        public IActionResult EditListItem(int id, string name, string type)
        {
            var item = _context.PredefinedLists.FirstOrDefault(l => l.Id == id);
            if (item != null)
            {
                item.Name = name;
                item.Type = type;
                _context.SaveChanges();
            }
            return RedirectToAction("ManageLists");
        }

        [HttpPost]
        public IActionResult DeleteListItem(int id)
        {
            var item = _context.PredefinedLists.FirstOrDefault(l => l.Id == id);
            if (item != null)
            {
                _context.PredefinedLists.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageLists");
        }
        // Στατιστικά και Αναφορές Πλατφόρμας
        [HttpGet]
        public IActionResult Stats()
        {
            // Στατιστικά Χρηστών
            ViewBag.TotalUsers = _context.Users.Count(u => u.Role == "User");
            ViewBag.TotalCounselors = _context.Users.Count(u => u.Role == "Counselor" && u.IsApproved == true);
            ViewBag.PendingCounselors = _context.Users.Count(u => u.Role == "Counselor" && u.IsApproved == false);

            // Στατιστικά Βιογραφικών
            ViewBag.TotalResumes = _context.Resumes.Count();
            ViewBag.SavedResumes = _context.Resumes.Count(r => r.Status == "Αποθηκευμένο");
            ViewBag.PendingResumes = _context.Resumes.Count(r => r.Status == "Προς Έλεγχο");
            ViewBag.EvaluatedResumes = _context.Resumes.Count(r => r.Status == "Ελέγχθηκε");

            // Λοιπά Στατιστικά
            ViewBag.TotalFeedbacks = _context.Feedbacks.Count();
            ViewBag.TotalTemplates = _context.Templates.Count();
            ViewBag.TotalPredefinedLists = _context.PredefinedLists.Count();

            // 1. Υπολογισμός Μέσου Χρόνου Αξιολόγησης
            var evaluatedResumes = _context.Resumes
                .Where(r => r.SubmittedAt != null && r.EvaluatedAt != null)
                .ToList(); // Τα φέρνουμε στη μνήμη για να κάνουμε πράξεις με ημερομηνίες

            if (evaluatedResumes.Any())
            {
                // Μετατροπή του μέσου χρόνου σε Ticks (η μικρότερη μονάδα χρόνου) για απόλυτη ακρίβεια
                var avgTicks = evaluatedResumes.Average(r => (r.EvaluatedAt.Value - r.SubmittedAt.Value).Ticks);
                var avgTimeSpan = TimeSpan.FromTicks((long)avgTicks);

                // Εξαγωγή ωρών και λεπτών
                int hours = (int)avgTimeSpan.TotalHours;
                int minutes = avgTimeSpan.Minutes;

                ViewBag.AverageEvalTime = $"{hours} ώρες και {minutes} λεπτά";
            }
            else
            {
                ViewBag.AverageEvalTime = "Μη διαθέσιμο";
            }

            // 2. Πιο Δημοφιλή Πρότυπα (Top 3)
            var popularTemplateIds = _context.Resumes
                .GroupBy(r => r.TemplateId)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => new { TemplateId = g.Key, Count = g.Count() })
                .ToList();

            var popularTemplatesList = new System.Collections.Generic.List<string>();
            foreach (var pop in popularTemplateIds)
            {
                var template = _context.Templates.FirstOrDefault(t => t.Id == pop.TemplateId);
                if (template != null)
                {
                    popularTemplatesList.Add($"{template.Name} ({pop.Count} χρήσεις)");
                }
            }
            ViewBag.PopularTemplates = popularTemplatesList;

            return View();
        }
    }
}