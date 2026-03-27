using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class CourseExamExcerciseFile
    {
        public int Id { get; set; }
        public int? CourseExamResultId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
