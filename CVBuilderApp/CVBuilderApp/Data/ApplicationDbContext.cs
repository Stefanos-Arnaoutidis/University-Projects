using CVBuilderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CVBuilderApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Οι πίνακες που δημιουργήθηκαν στη βάση
        public DbSet<User> Users { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        // Πίνακας για τις προκαθορισμένες επιλογές δεξιοτήτων και γλωσσών
        public DbSet<PredefinedList> PredefinedLists { get; set; }
    }
}