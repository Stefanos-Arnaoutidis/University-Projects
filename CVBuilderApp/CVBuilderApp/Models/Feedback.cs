namespace CVBuilderApp.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int ResumeId { get; set; } // Για ποιο βιογραφικό είναι το σχόλιο
        public int CounselorId { get; set; } // Ποιος σύμβουλος το έγραψε

        public string GeneralComments { get; set; } // Συνολική αξιολόγηση του βιογραφικού
        public string AcademicComments { get; set; } // Διορθωτικές προτάσεις για την εκπαίδευση
        public string WorkExperienceComments { get; set; } // Παρατηρήσεις επί της επαγγελματικής εμπειρίας
        public string SkillsComments { get; set; } // Σχόλια σχετικά με την καταγραφή των δεξιοτήτων
    }
}