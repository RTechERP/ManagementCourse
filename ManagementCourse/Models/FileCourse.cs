using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class FileCourse
    {
        public int Id { get; set; }
        public string NameFile { get; set; }
        public int? CourseId { get; set; }
        public int? LessonId { get; set; }
    }
}
