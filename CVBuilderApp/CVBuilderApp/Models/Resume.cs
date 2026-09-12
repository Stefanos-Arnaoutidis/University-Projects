using System;
using System.ComponentModel.DataAnnotations;

namespace CVBuilderApp.Models
{
    public class Resume
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Η επιλογή προτύπου είναι υποχρεωτική!")]
        public int TemplateId { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "Τα Ακαδημαϊκά Στοιχεία είναι υποχρεωτικά!")]
        [MinLength(10, ErrorMessage = "Παρακαλώ γράψτε κάτι πιο αναλυτικό (τουλάχιστον 10 χαρακτήρες).")]
        public string AcademicInfo { get; set; }

        [Required(ErrorMessage = "Η Εργασιακή Εμπειρία είναι υποχρεωτική!")]
        [MinLength(10, ErrorMessage = "Παρακαλώ γράψτε κάτι πιο αναλυτικό (τουλάχιστον 10 χαρακτήρες).")]
        public string WorkInfo { get; set; }

        // Έλεγχος για έγκυρο αριθμό τηλεφώνου (π.χ. 10 ψηφία)
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Το τηλέφωνο πρέπει να αποτελείται από ακριβώς 10 ψηφία!")]
        public string? PhoneNumber { get; set; }

        public string? LinkedIn { get; set; }

        [StringLength(500, ErrorMessage = "Το προσωπικό προφίλ δεν μπορεί να ξεπερνάει τους 500 χαρακτήρες.")]
        public string? PersonalSummary { get; set; }

        public string? Skills { get; set; }

        // Νέα πεδία για καταγραφή χρόνου (nullable γιατί αρχικά είναι κενά)
        public DateTime? SubmittedAt { get; set; }
        public DateTime? EvaluatedAt { get; set; }
    }
}