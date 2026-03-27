using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace ManagementCourse.Models.ViewModel
{
    public class CourseExamViewModel
    {
        public CourseExam CourseExam { get; set; }
        public List<CourseQuestion>CourseQuestion { get; set; }
        public List<CourseAnswer> CourseAnswer { get; set; }
        public CourseExamResult CourseExamResult { get; set; }
    }
}
