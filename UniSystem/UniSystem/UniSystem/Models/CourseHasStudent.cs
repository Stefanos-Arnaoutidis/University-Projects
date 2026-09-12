using System;
using System.Collections.Generic;

namespace UniSystem.Models;

public partial class CourseHasStudent
{
    public int CourseIdCourse { get; set; }

    public int StudentsRegistrationNumber { get; set; }

    public int? GradeCourseStudent { get; set; }

    public virtual Course CourseIdCourseNavigation { get; set; } = null!;

    public virtual Student StudentsRegistrationNumberNavigation { get; set; } = null!;
}
