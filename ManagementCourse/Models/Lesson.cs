using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class Lesson
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string LessonTitle { get; set; }
        public string LessonContent { get; set; }
        public int? Duration { get; set; }
        public string VideoUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Stt { get; set; }
        public int? CourseId { get; set; }
        public int? FileCourseId { get; set; }
    }
}
