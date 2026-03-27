using ManagementCourse.Common;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq;

namespace ManagementCourse.Controllers
{
    public class CourseController : Controller
    {
        CourseRepository courseRepo;

        public CourseController(CourseRepository courseRepository)
        {
            courseRepo = courseRepository;
        }


        [HttpGet]
        public JsonResult GetListCourse(int id, string filterText, int departmentID, int catalogType)
        {
            //int departmentID = (int)HttpContext.Session.GetInt32("department_id");
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");


            int kpiPositionTypeID = 0;
            //DataTable kpiPositionType = LoadDataFromSP.GetDataTableSP("spGetKPIPositionTypeByEmployeeID",
            //            new string[] { "@EmployeeID" }, new object[] { employeeID });
            //if (kpiPositionType.Rows.Count > 0)
            //{
            //    kpiPositionTypeID = TextUtils.ToInt(kpiPositionType.Rows[0]["ID"]);
            //}

            var listCourse = courseRepo.ListCourses(departmentID, id, filterText, employeeID, catalogType, kpiPositionTypeID);

            return Json(listCourse, new System.Text.Json.JsonSerializerOptions());
        }

    }
}
