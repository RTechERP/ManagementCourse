using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Models.DTO
{
    public class CourseLessonDTO
    {
        public int ID { get; set; }
        public int STT { get; set; }
        public string NameCourse { get; set; }
        public string LessonTitle { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public int CourseID { get; set; }
        public string LessonContent { get; set; }
        public int Duration { get; set; }
        public string VideoURL { get; set; }
        public string UrlPDF { get; set; }
    }
}
