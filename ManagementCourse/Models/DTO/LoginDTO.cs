using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Models.DTO
{
    public class LoginDTO
    {
        public string userName { get; set; }
        public string passwordHash { get; set; }
        public int registerIdeaTypeID  { get; set; }
        public int courseID  { get; set; }
    }
}
