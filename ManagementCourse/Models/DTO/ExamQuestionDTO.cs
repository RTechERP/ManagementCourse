using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Models.DTO
{
    public class ExamQuestionDTO
    {
        public int ID { get; set; }
        public string QuestionText { get; set; }
        public int QuestionChosenID { get; set; }
        public string Image { get; set; }
        public List<ExamAnswerDTO> ExamAnswers { get; set; }
    }
}
