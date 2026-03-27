using System;

namespace ManagementCourse.Models
{
    public class ErrorViewModel
    {
        public string RequestId = "Thông báo!";

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Message { get; set; }
    }
}
