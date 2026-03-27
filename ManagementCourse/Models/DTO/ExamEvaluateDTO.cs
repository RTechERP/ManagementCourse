using System;

namespace ManagementCourse.Models.DTO
{
    public class ExamEvaluateDTO : CourseExamEvaluate
    {
        public string QuestionText { get; set; }
        public long STT { get; set; }
        public int QuestionID { get; set; }
        public int CourseId { get; set; }
        public int CourseExamId { get; set; }
        public string NameExam { get; set; }
        public string Image { get; set; }
    }
}
