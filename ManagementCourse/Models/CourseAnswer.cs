using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class CourseAnswer
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CourseQuestionId { get; set; }
        public int? AnswerNumber { get; set; }
    }
}
