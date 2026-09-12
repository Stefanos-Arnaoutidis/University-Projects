using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CVBuilderApp.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsApproved { get; set; }

        public string Specialty { get; set; } // Καταχώρηση ειδικότητας για τους συμβούλους καριέρας
    }
}