using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class CourseExamResultDetail
    {
        public int Id { get; set; }
        public int? CourseQuestionId { get; set; }
        public int? CourseAnswerId { get; set; }
        public int? CourseExamResultId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
