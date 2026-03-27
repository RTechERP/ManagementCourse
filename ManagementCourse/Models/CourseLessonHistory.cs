using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class CourseLessonHistory
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? Status { get; set; }
        public int? LessonId { get; set; }
        public DateTime? ViewDate { get; set; }
    }
}
