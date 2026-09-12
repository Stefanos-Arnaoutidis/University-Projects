using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CVBuilderApp.Data;
using CVBuilderApp.Models;
using System.Linq;

namespace CVBuilderApp.Controllers
{
    [Authorize(Roles = "Counselor")] // Πρόσβαση μόνο σε χρήστες με ρόλο Counselor
    public class CounselorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CounselorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Αρχική σελίδα συμβούλου με τα εκκρεμή βιογραφικά
        [HttpGet]
        public IActionResult Index()
        {
            // Φόρτωση των βιογραφικών που είναι προς έλεγχο
            var pendingResumes = _context.Resumes.Where(r => r.Status == "Προς Έλεγχο").ToList();
            return View(pendingResumes);
        }

        // Σελίδα προβολής του βιογραφικού και της φόρμας σχολίων
        [HttpGet]
        public IActionResult Evaluate(int id)
        {
            // Αναζήτηση του βιογραφικού στη βάση
            var resume = _context.Resumes.FirstOrDefault(r => r.Id == id);
            if (resume == null)
            {
                return NotFound();
            }

            // Λήψη στοιχείων του φοιτητή για εμφάνιση του ονόματος στην οθόνη
            var user = _context.Users.FirstOrDefault(u => u.Id == resume.UserId);
            ViewBag.StudentName = $"{user.FirstName} {user.LastName}";

            return View(resume);
        }

        // Αποθήκευση σχολίων ανά ενότητα και αλλαγή κατάστασης βιογραφικού
        [HttpPost]
        public IActionResult SaveFeedback(int resumeId, string generalComments, string academicComments, string workExperienceComments, string skillsComments)
        {
            // Εύρεση του Id του συνδεδεμένου συμβούλου
            var counselorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int counselorId = int.Parse(counselorIdString);

            // Προσθήκη των σχολίων στον πίνακα Feedbacks ανά ενότητα
            var feedback = new Feedback
            {
                ResumeId = resumeId,
                CounselorId = counselorId,
                GeneralComments = generalComments,
                AcademicComments = academicComments,
                WorkExperienceComments = workExperienceComments,
                SkillsComments = skillsComments
            };
            _context.Feedbacks.Add(feedback);

            // Εύρεση του βιογραφικού και αλλαγή της κατάστασης σε Ελέγχθηκε
            var resume = _context.Resumes.FirstOrDefault(r => r.Id == resumeId);
            if (resume != null)
            {
                resume.Status = "Ελέγχθηκε";
                resume.EvaluatedAt = DateTime.Now;
            }

            // Αποθήκευση των αλλαγών στη βάση δεδομένων
            _context.SaveChanges();

            // Ανακατεύθυνση στην αρχική σελίδα
            return RedirectToAction("Index");
        }
    }
}