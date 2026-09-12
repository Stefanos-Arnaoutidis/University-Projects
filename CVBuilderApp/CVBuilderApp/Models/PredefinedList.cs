using System.ComponentModel.DataAnnotations;

namespace CVBuilderApp.Models
{
    public class PredefinedList
    {
        [Key]
        public int Id { get; set; }

        // Το όνομα της επιλογής, π.χ. "C#", "Αγγλικά", "Python"
        [Required]
        public string Name { get; set; }

        // Ο τύπος της λίστας, π.χ. "Skill" για δεξιότητες, "Language" για ξένες γλώσσες
        [Required]
        public string Type { get; set; }
    }
}