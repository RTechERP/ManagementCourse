using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Models.DTO
{
    public class ExamAnswerDTO
    {
        public int ID { get; set; }
        public int CourseQuestionId { get; set; }
        public string AnswerText { get; set; }
        public int AnswerNumber { get; set; }
        public int CourseAnswerChosenID { get; set; }
    }
}
