using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class CourseFile
    {
        public int Id { get; set; }
        public string NameFile { get; set; }
        public int? CourseId { get; set; }
        public int? LessonId { get; set; }
        public string OriginPath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
