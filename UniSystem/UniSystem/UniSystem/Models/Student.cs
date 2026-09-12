using System;
using System.Collections.Generic;

namespace UniSystem.Models;

public partial class Student
{
    public int RegistrationNumber { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string UsersUsername { get; set; } = null!;

    public virtual ICollection<CourseHasStudent> CourseHasStudents { get; set; } = new List<CourseHasStudent>();

    public virtual User UsersUsernameNavigation { get; set; } = null!;
}
