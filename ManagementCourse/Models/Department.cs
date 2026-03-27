using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class Department
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }
        public string Email { get; set; }
        public int? HeadofDepartment { get; set; }
        public bool? IsShowHotline { get; set; }
        public string Pid { get; set; }
        public int? Stt { get; set; }
    }
}
