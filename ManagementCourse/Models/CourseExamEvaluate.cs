using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class CourseExamEvaluate
    {
        public int Id { get; set; }
        public int? CourseExamResultId { get; set; }
        public int? CourseQuestionId { get; set; }
        public decimal? Point { get; set; }
        public bool Evaluate { get; set; }
        public string Note { get; set; }
        public DateTime? DateCompleted { get; set; }
        public DateTime? DateEvaluate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
