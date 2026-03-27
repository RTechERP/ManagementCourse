using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Models.DTO
{
    public class CourseFileDTO
    {
        public int ID { get; set; }
        public string NameFile { get; set; }
        public int LessonID { get; set; }
        public string FileName { get; set; }
    }
}
