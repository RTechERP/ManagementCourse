using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Models.DTO
{
    public class CourseExamResultDTO
    {
        public int ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal PracticePoints { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public string FullName { get; set; }
        public string NameCourse { get; set; }
        public string NameExam { get; set; }
        public int TotalQuestion { get; set; }
        public decimal Goal { get; set; }
        public string StatusText { get; set; }

        //public int ID { get; set; }
        public int CourseExamId { get; set; }
        public int EmployeeId { get; set; }
        //public string NameCourse { get; set; }
        //public string NameExam { get; set; }
        //public int TotalQuestion { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalIncorrect { get; set; }
        public decimal PercentageCorrect { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public int Status { get; set; }
    }
}
