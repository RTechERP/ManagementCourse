using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class CourseExamResult
    {
        public int Id { get; set; }
        public int? CourseExamId { get; set; }
        public int? EmployeeId { get; set; }
        public int? TotalCorrect { get; set; }
        public int? TotalIncorrect { get; set; }
        public decimal? PercentageCorrect { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal? PracticePoints { get; set; }
        public bool? Evaluate { get; set; }
        public string Note { get; set; }
        public int? LessonId { get; set; }
    }
}
