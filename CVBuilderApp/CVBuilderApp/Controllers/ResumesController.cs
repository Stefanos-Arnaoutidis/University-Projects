using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CVBuilderApp.Data;
using CVBuilderApp.Models;
using System.Security.Claims;
using System.Linq;

namespace CVBuilderApp.Controllers
{
    [Authorize(Roles = "User")] // Πρόσβαση μόνο σε συνδεδεμένους απλούς χρήστες
    public class ResumesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResumesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // ID του συνδεδεμένου χρήστη
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdString);

            // Φόρτωση μόνο των βιογραφικών του συγκεκριμένου χρήστη
            var myResumes = _context.Resumes.Where(r => r.UserId == userId).ToList();

            return View(myResumes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Λίστα προτύπων για εμφάνιση στο dropdown μενού της φόρμας
            ViewBag.Templates = _context.Templates.ToList();

            // Φόρτωση προκαθορισμένων λιστών για τις δεξιότητες
            ViewBag.PredefinedItems = _context.PredefinedLists.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Resume resume)
        {
            // Εξαίρεση πεδίων από τον έλεγχο εγκυρότητας καθώς ορίζονται αυτόματα παρακάτω
            ModelState.Remove("UserId");
            ModelState.Remove("Status");
            ModelState.Remove("User");
            ModelState.Remove("Template");

            // Έλεγχος εγκυρότητας των υποχρεωτικών πεδίων της φόρμας
            if (!ModelState.IsValid)
            {
                // Επαναφόρτωση των δεδομένων για αποφυγή σφάλματος στο μενού
                ViewBag.Templates = _context.Templates.ToList();
                ViewBag.PredefinedItems = _context.PredefinedLists.ToList();
                return View(resume);
            }

            // Αυτόματος ορισμός ID χρήστη και αρχικής κατάστασης βιογραφικού
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            resume.UserId = int.Parse(userIdString);

            // Το βιογραφικό αποθηκεύεται τοπικά χωρίς να σταλεί ακόμα για έλεγχο
            resume.Status = "Αποθηκευμένο";

            _context.Resumes.Add(resume);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Φόρτωση φόρμας επεξεργασίας βιογραφικού
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Αναζήτηση βιογραφικού με βάση το ID
            var resume = _context.Resumes.FirstOrDefault(r => r.Id == id);
            if (resume == null)
            {
                return NotFound();
            }

            // Έλεγχος για να διασφαλιστεί ότι ο χρήστης επεξεργάζεται μόνο το δικό του βιογραφικό
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (resume.UserId.ToString() != userIdString)
            {
                return Unauthorized();
            }

            // Απαγόρευση επεξεργασίας αν το βιογραφικό έχει ήδη σταλεί για έλεγχο
            if (resume.Status != "Αποθηκευμένο")
            {
                return RedirectToAction("Index");
            }

            // Λίστα προτύπων και προκαθορισμένων επιλογών για εμφάνιση στη φόρμα
            ViewBag.Templates = _context.Templates.ToList();
            ViewBag.PredefinedItems = _context.PredefinedLists.ToList();

            return View(resume);
        }

        // Επεξεργασία και αποθήκευση δεδομένων ανά ενότητα
        [HttpPost]
        public IActionResult Edit(int id, Resume updatedResume)
        {
            // Εξαίρεση πεδίων από τον έλεγχο εγκυρότητας
            ModelState.Remove("UserId");
            ModelState.Remove("Status");
            ModelState.Remove("User");
            ModelState.Remove("Template");

            // Έλεγχος εγκυρότητας των υποχρεωτικών πεδίων της φόρμας
            if (!ModelState.IsValid)
            {
                // Επαναφόρτωση των δεδομένων για αποφυγή σφάλματος
                ViewBag.Templates = _context.Templates.ToList();
                ViewBag.PredefinedItems = _context.PredefinedLists.ToList();
                return View(updatedResume);
            }

            // Αναζήτηση του υπάρχοντος βιογραφικού
            var existingResume = _context.Resumes.FirstOrDefault(r => r.Id == id);
            if (existingResume == null)
            {
                return NotFound();
            }

            // Έλεγχος ταυτοποίησης για λόγους ασφαλείας
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingResume.UserId.ToString() != userIdString)
            {
                return Unauthorized();
            }

            // Ενημέρωση των επιμέρους ενοτήτων του βιογραφικού
            existingResume.PersonalSummary = updatedResume.PersonalSummary;
            existingResume.WorkInfo = updatedResume.WorkInfo;
            existingResume.AcademicInfo = updatedResume.AcademicInfo;
            existingResume.Skills = updatedResume.Skills;
            existingResume.PhoneNumber = updatedResume.PhoneNumber;
            existingResume.LinkedIn = updatedResume.LinkedIn;
            existingResume.TemplateId = updatedResume.TemplateId;

            // Αποθήκευση των αλλαγών στη βάση δεδομένων
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Διαγραφή βιογραφικού από το σύστημα
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Αναζήτηση βιογραφικού με βάση το ID
            var resume = _context.Resumes.FirstOrDefault(r => r.Id == id);
            if (resume == null)
            {
                return NotFound();
            }

            // Έλεγχος ταυτοποίησης για λόγους ασφαλείας
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (resume.UserId.ToString() != userIdString)
            {
                return Unauthorized();
            }

            // Αφαίρεση του εγγράφου και αποθήκευση αλλαγών
            _context.Resumes.Remove(resume);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Αποστολή αιτήματος αξιολόγησης στον σύμβουλο
        [HttpPost]
        public IActionResult RequestEvaluation(int id)
        {
            // Αναζήτηση βιογραφικού με βάση το ID
            var resume = _context.Resumes.FirstOrDefault(r => r.Id == id);
            if (resume == null)
            {
                return NotFound();
            }

            // Έλεγχος ταυτοποίησης για λόγους ασφαλείας
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (resume.UserId.ToString() != userIdString)
            {
                return Unauthorized();
            }

            // Ενημέρωση της κατάστασης ώστε να εμφανιστεί στη λίστα των συμβούλων
            resume.Status = "Προς Έλεγχο";
            resume.SubmittedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            // Αναζήτηση βιογραφικού με βάση το ID
            var resume = _context.Resumes.FirstOrDefault(r => r.Id == id);
            if (resume == null)
            {
                return NotFound();
            }

            // Έλεγχος για να διασφαλιστεί ότι ο χρήστης βλέπει μόνο το δικό του βιογραφικό
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (resume.UserId.ToString() != userIdString)
            {
                return Unauthorized();
            }

            // Ανάκτηση ονόματος του επιλεγμένου προτύπου
            var template = _context.Templates.FirstOrDefault(t => t.Id == resume.TemplateId);
            ViewBag.TemplateName = template != null ? template.Name : "Κλασικό";

            // Στοιχεία επικοινωνίας του κατόχου του βιογραφικού
            var user = _context.Users.FirstOrDefault(u => u.Id == resume.UserId);
            ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";
            ViewBag.UserEmail = user.Email;

            // Αναζήτηση σχολίων αξιολόγησης από τον σύμβουλο
            var feedback = _context.Feedbacks.FirstOrDefault(f => f.ResumeId == id);
            ViewBag.Feedback = feedback;

            return View(resume);
        }
    }
}