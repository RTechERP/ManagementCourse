using System;
using System.Collections.Generic;

#nullable disable

namespace ManagementCourse.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string LoginName { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthOfDate { get; set; }
        public int? Sex { get; set; }
        public string Qualifications { get; set; }
        public string BankAccount { get; set; }
        public string Bhyt { get; set; }
        public string Mst { get; set; }
        public string Bhxh { get; set; }
        public string Cmtnd { get; set; }
        public string JobDescription { get; set; }
        public string Telephone { get; set; }
        public string HandPhone { get; set; }
        public string HomeAddress { get; set; }
        public string Resident { get; set; }
        public string PostalCode { get; set; }
        public int? DepartmentId { get; set; }
        public int? Status { get; set; }
        public string Communication { get; set; }
        public DateTime? PassExpireDate { get; set; }
        public bool? IsCashier { get; set; }
        public int? CashierNo { get; set; }
        public string EmailCom { get; set; }
        public string Email { get; set; }
        public DateTime? StartWorking { get; set; }
        public int? UserGroupId { get; set; }
        public int? UserGroupSxid { get; set; }
        public int? MainViewId { get; set; }
        public string Position { get; set; }
        public bool? IsSetupFunction { get; set; }
        public string ImagePath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsAdmin { get; set; }
        public int? IsAdminSale { get; set; }
        public int? RoleId { get; set; }
        public int? TeamId { get; set; }
        public int? Leader { get; set; }
    }
}
