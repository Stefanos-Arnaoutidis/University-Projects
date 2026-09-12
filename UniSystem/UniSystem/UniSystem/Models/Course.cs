using System;
using System.Collections.Generic;

namespace UniSystem.Models;

public partial class Course
{
    public int IdCourse { get; set; }

    public string CourseTitle { get; set; } = null!;

    public string CourseSemester { get; set; } = null!;

    public int? ProfessorsAfm { get; set; }

    public virtual ICollection<CourseHasStudent> CourseHasStudents { get; set; } = new List<CourseHasStudent>();

    public virtual Professor? ProfessorsAfmNavigation { get; set; }
}
