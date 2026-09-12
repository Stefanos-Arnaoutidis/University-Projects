using System;
using System.Collections.Generic;

namespace UniSystem.Models;

public partial class Professor
{
    public int Afm { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string UsersUsername { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual User UsersUsernameNavigation { get; set; } = null!;
}
