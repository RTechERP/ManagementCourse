
using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using ManagementCourse.Models.DTO;

namespace ManagementCourse.Controllers
{
    public class HomeController : Controller
    {

        DepartmentRepository _departmentRepo;
        CourseCatalogRepository _courseCatalogRepo;
        CourseRepository _courseRepo;
        LessonRepository _lessonRepo;
        GenericRepository<CourseExam> _cousrseExam = new GenericRepository<CourseExam>();

        public HomeController(DepartmentRepository departmentRepository, CourseCatalogRepository courseCatalogRepository, CourseRepository courseRepository, LessonRepository lessonRepository)
        {
            _departmentRepo = departmentRepository;
            _courseCatalogRepo = courseCatalogRepository;
            _courseRepo = courseRepository;
            _lessonRepo = lessonRepository;
        }

        public IActionResult Index(int departmentID, int courseCatalogID, int catalogType = 1)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");

            int kpiPositionTypeID = 0;
            //DataTable kpiPositionType = LoadDataFromSP.GetDataTableSP("spGetKPIPositionTypeByEmployeeID",
            //            new string[] { "@EmployeeID" }, new object[] { employeeID });
            //if (kpiPositionType.Rows.Count > 0)
            //{
            //    kpiPositionTypeID = TextUtils.ToInt(kpiPositionType.Rows[0]["ID"]);
            //}


            ViewBag.CoureCatalogID = courseCatalogID;
            ViewBag.DepartmentID = departmentID;
            ViewBag.CatalogType = catalogType;

            if (departmentID <= 0 && kpiPositionTypeID <= 0)
            {
                var listCourse = new List<CourseDTO>();
                return View(listCourse);

            }
            var listCourseChile = new List<CourseDTO>();
            if (!(kpiPositionTypeID <= 0))
            {
                listCourseChile = _courseRepo.ListCourses(0, 0, "", employeeID, catalogType, kpiPositionTypeID);
            }


            if (departmentID <= 0)
            {
                return View(listCourseChile);

            }
            var newdepartmentID = departmentID <= 0 ? Convert.ToInt32(HttpContext.Session.GetInt32("department_id")) : departmentID;
            var listCourseParent = _courseRepo.ListCourses(newdepartmentID, courseCatalogID, "", employeeID, catalogType, 0);

            string departmentName = departmentID <= 0 ? "" : HttpContext.Session.GetString("department");
            if (listCourseParent.Count > 0)
            {
                ViewBag.TitleCourse = courseCatalogID == 0 ? $"danh sách khoá học {departmentName}" : $"danh sách khoá học {listCourseParent.FirstOrDefault().NameCourseCatalog}";
            }

            ViewBag.ListExam = _cousrseExam.GetAll().ToList();
            int leader = TextUtils.ToInt(HttpContext.Session.GetInt32("isleader"));
            int isAdmin = TextUtils.ToInt(HttpContext.Session.GetInt32("isAdmin"));

            for (int i = 0; i < listCourseParent.Count; i++)
            {

                if (i == 0) continue;

                CourseDTO course = listCourseParent[i - 1];

                if (course.CatalogType != 2)
                {
                    if ((course.NumberLesson == course.TotalHistoryLession && course.Evaluate == 1) || leader > 0 || isAdmin > 0 || employeeID == 55)
                    {
                        listCourseParent[i].Status = 1;
                    }
                    else
                    {
                        listCourseParent[i].Status = 0;
                    }
                }
            }


            if (!listCourseChile.Any(x => x.Evaluate <= 0))
            {
                return View(listCourseParent);
            }

            var childIds = new HashSet<int>(listCourseChile.Select(c => c.ID));

            foreach (var parent in listCourseParent)
            {
                if (parent.Evaluate == 1 || childIds.Contains(parent.ID) || leader > 0 || isAdmin > 0 || employeeID == 55)
                {
                    parent.Status = 1; // Đạt hoặc nằm trong listCourseChild → có thể làm
                }
                else
                {
                    parent.Status = 0; // Còn lại → không thể làm
                }
            }

            return View(listCourseParent);

        }


        #region Login    
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập username và mật khẩu !";
                return View();
            }
            password = string.IsNullOrEmpty(password) ? "" : Common.MaHoaMD5.EncryptPassword(password);
            DataTable user = LoadDataFromSP.GetDataTableSP("spLogin",
                                            new string[] { "@LoginName", "@Password" }, new object[] { username, password });
            if (user.Rows.Count > 0)
            {
                //int isAdmin = Convert.ToInt32(TextUtils.ToInt(user.Rows[0]["IsAdmin"]));
                HttpContext.Session.SetInt32("userid", TextUtils.ToInt(user.Rows[0]["ID"]));
                HttpContext.Session.SetInt32("employeeid", TextUtils.ToInt(user.Rows[0]["EmployeeID"]));
                HttpContext.Session.SetString("loginname", TextUtils.ToString(user.Rows[0]["LoginName"]));
                HttpContext.Session.SetString("fullname", TextUtils.ToString(user.Rows[0]["FullName"]));
                HttpContext.Session.SetInt32("isAdmin", TextUtils.ToInt(user.Rows[0]["IsAdmin"]));
                //HttpContext.Session.SetInt32("isAdminSale", TextUtils.ToInt(user.Rows[0]["IsAdminSale"]));
                HttpContext.Session.SetInt32("department_id", TextUtils.ToInt(user.Rows[0]["DepartmentID"]));
                HttpContext.Session.SetString("department", TextUtils.ToString(user.Rows[0]["DepartmentName"]));
                //HttpContext.Session.SetInt32("headOfDepartment", TextUtils.ToInt(user.Rows[0]["HeadofDepartment"]));
                //HttpContext.Session.SetInt32("role_id", TextUtils.ToInt(user.Rows[0]["RoleID"]));
                //HttpContext.Session.SetString("role", TextUtils.ToString(user.Rows[0]["RoleName"]));
                //HttpContext.Session.SetString("img", TextUtils.ToString(user.Rows[0]["AnhCBNV"]));
                //HttpContext.Session.SetInt32("userteamid", TextUtils.ToInt(user.Rows[0]["UserTeamID"]));
                HttpContext.Session.SetInt32("isleader", TextUtils.ToInt(user.Rows[0]["IsLeader"]));
                HttpContext.Session.SetInt32("UserGroupID", TextUtils.ToInt(user.Rows[0]["UserGroupID"]));
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu !";
            }

            return View(user);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
        #endregion
        [HttpPost]
        public IActionResult CheckNullCourse(int id)
        {
            var lstLess = _lessonRepo.GetAllList().Where(c => c.CourseId == id);
            if (lstLess.Count() < 1)
                return Json(0, new System.Text.Json.JsonSerializerOptions());
            return Json(1, new System.Text.Json.JsonSerializerOptions());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(ErrorViewModel errorView)
        {
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return View(errorView);
        }

        [HttpPost]
        public IActionResult LoginToCourse(string userName, string passwordHash, int registerIdeaTypeID, int courseID)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passwordHash))
            {
                return RedirectToAction("Login", "Home");
            }
            //string password = MaHoaMD5.DecryptPassword(login.PasswordHash);
            DataTable user = LoadDataFromSP.GetDataTableSP("spLogin",
                                            new string[] { "@LoginName", "@Password" }, new object[] { userName, passwordHash });
            if (user.Rows.Count > 0)
            {
                HttpContext.Session.SetInt32("userid", TextUtils.ToInt(user.Rows[0]["ID"]));
                HttpContext.Session.SetInt32("employeeid", TextUtils.ToInt(user.Rows[0]["EmployeeID"]));
                HttpContext.Session.SetString("loginname", TextUtils.ToString(user.Rows[0]["LoginName"]));
                HttpContext.Session.SetString("fullname", TextUtils.ToString(user.Rows[0]["FullName"]));
                HttpContext.Session.SetInt32("isAdmin", TextUtils.ToInt(user.Rows[0]["IsAdmin"]));
                HttpContext.Session.SetInt32("department_id", TextUtils.ToInt(user.Rows[0]["DepartmentID"]));
                HttpContext.Session.SetString("department", TextUtils.ToString(user.Rows[0]["DepartmentName"]));
                HttpContext.Session.SetInt32("isleader", TextUtils.ToInt(user.Rows[0]["IsLeader"]));
                HttpContext.Session.SetInt32("UserGroupID", TextUtils.ToInt(user.Rows[0]["UserGroupID"]));
                DataTable kpiPositionType = LoadDataFromSP.GetDataTableSP("spGetKPIPositionTypeByEmployeeID",
                                            new string[] { "@EmployeeID" }, new object[] { user.Rows[0]["ID"] });
                if (kpiPositionType.Rows.Count > 0)
                {
                    HttpContext.Session.SetInt32("KPIPositionTypeID", TextUtils.ToInt(kpiPositionType.Rows[0]["ID"]));
                }
                else
                {
                    HttpContext.Session.SetInt32("KPIPositionTypeID", TextUtils.ToInt(0));
                }

                if (courseID <= 0)
                {
                    CourseCatalog catalogModel = _courseCatalogRepo.GetByID(registerIdeaTypeID) ?? new CourseCatalog();
                    return Redirect($"Index?departmentID={catalogModel.DepartmentId}&courseCatalogID={catalogModel.Id}&catalogType={catalogModel.CatalogType}");
                }
                else
                {
                    return RedirectToAction("Index", "Lesson", new { courseId = courseID, catalogId = registerIdeaTypeID });
                }
            }
            return RedirectToAction("Login", "Home");
        }


    }
}
