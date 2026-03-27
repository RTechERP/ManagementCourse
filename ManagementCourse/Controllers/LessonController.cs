using AspNetCoreHero.ToastNotification.Abstractions;
using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Models.DTO;
using ManagementCourse.Models.ViewModel;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;


namespace ManagementCourse.Controllers
{
    public class LessonController : Controller
    {
        LessonRepository _lessonRepo;
        CourseRepository _courseRepo;
        FileCourseRepository _fileCourseRepo;
        CourseLessonHistoryRepository _lessonHistoryRepo;
        GenericRepository<CourseExamResult> _cousrseExamResult = new GenericRepository<CourseExamResult>();
        GenericRepository<CourseExam> _cousrseExam = new GenericRepository<CourseExam>();

        CourseExamRepository _examRepo = new CourseExamRepository(null);
        public INotyfService _notyfService { get; set; }
        public LessonController(LessonRepository lessonRepository, CourseRepository courseRepository,
            FileCourseRepository fileCourseRepository, CourseLessonHistoryRepository lessonHistoryRepo, INotyfService notyfService)
        {
            _lessonRepo = lessonRepository;
            _courseRepo = courseRepository;
            _fileCourseRepo = fileCourseRepository;
            _lessonHistoryRepo = lessonHistoryRepo;
            _notyfService = notyfService;
        }

        public IActionResult Index(int courseId)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.CourseID = courseId;
            //ViewBag.CatalogID = catalogId;
            return View();

            //try
            //{
            //    string[] nameFileShow;
            //    string[] idFileShow;
            //    int empoyeeId = (int)HttpContext.Session.GetInt32("employeeid");
            //    var nameCourse = _courseRepo.GetByID(courseId)?.NameCourse;
            //    CourseLessonViewModel courseLessonViewModel = new CourseLessonViewModel();
            //    if (lessionId == 0)
            //    {
            //        courseLessonViewModel = _lessonRepo.GetCourseLessonFromCourse(courseId, empoyeeId);
            //    }
            //    else
            //    {
            //        courseLessonViewModel = _lessonRepo.GetCourseLessonById(courseId, empoyeeId, lessionId);
            //    }

            //    if (courseLessonViewModel == null)
            //        return Redirect("~/Home/Error");

            //    var allLessons = _lessonRepo.ListCourseLessonFromCourse(courseId, empoyeeId);
            //    ViewBag.ListLesson = allLessons;
            //    ViewBag.NameCourse = nameCourse;

            //    ViewBag.LessonID = lessionId;

            //    //DataTable dt = LoadDataFromSP.GetDataTableSP("spGetNameFileLesson", new string[] { "@LessonID" }, new object[] { courseLessonViewModel.Id });
            //    //if (dt.Rows.Count > 0)
            //    //{
            //    //    string allNameFile = dt.Rows[0]["NameFile"].ToString();
            //    //    string allIdFile = dt.Rows[0]["ID"].ToString();
            //    //    if (allNameFile != "" && allIdFile != null)
            //    //    {
            //    //        nameFileShow = allNameFile.Split(',');
            //    //        idFileShow = allIdFile.Split(',');
            //    //        List<FileShow> fileShows = new List<FileShow>();

            //    //        for (int i = 0; i < nameFileShow.Length && i < idFileShow.Length; i++)
            //    //        {
            //    //            fileShows.Add(new FileShow { ID = idFileShow[i] + Path.GetExtension(nameFileShow[i]), NameFile = nameFileShow[i] });
            //    //        }

            //    //        ViewBag.FileCourse = fileShows;
            //    //    }
            //    //}

            //    if (_cousrseExam.GetAll().FirstOrDefault(p => p.CourseId == courseId) != null)
            //        ViewBag.CourseExamID = _cousrseExam.GetAll().FirstOrDefault(p => p.CourseId == courseId).Id;
            //    else ViewBag.CourseExamID = 0;

            //    ViewBag.EmployeeID = HttpContext.Session.GetInt32("employeeid");

            //    //Check đã học xong các bài học chưa
            //    DataTable dtLessonHistory = LoadDataFromSP.GetDataTableSP("spGetCourseLessonHistory", new string[] { "@CourseID", "@EmployeeID" }, new object[] {courseId,empoyeeId });
            //    var statusHistory = dtLessonHistory.Select("Status = 0");
            //    ViewBag.StatusHistory = dtLessonHistory.Rows.Count <= 0 ? "disabled" : (statusHistory.Length > 0 ? "disabled":"");


            //}
            //catch
            //{

            //    return Redirect("~/Home/Error");
            //}

        }
        [HttpPost]
        public IActionResult CheckHistoryLesson(int lessonId)
        {
            try
            {
                var empoyeeId = HttpContext.Session.GetInt32("employeeid").Value;
                if (lessonId == 0/* || empoyeeId == 0*/)
                {
                    return Json(-1);
                }
                CourseLessonHistory courseLessonHis = _lessonHistoryRepo.GetCourseLessonHistory(empoyeeId, lessonId);
                if (courseLessonHis == null)
                {
                    CourseLessonHistory lessonHistory = new CourseLessonHistory();
                    lessonHistory.ViewDate = DateTime.Now;
                    lessonHistory.EmployeeId = empoyeeId;
                    lessonHistory.LessonId = lessonId;
                    lessonHistory.Status = 1;
                    _lessonHistoryRepo.Create(lessonHistory);
                    return Json(1);  //bằng 1 là đã  xem
                }
                else
                {
                    courseLessonHis.ViewDate = DateTime.Now;
                    courseLessonHis.EmployeeId = empoyeeId;
                    courseLessonHis.LessonId = lessonId;
                    courseLessonHis.Status = courseLessonHis.Status == 1 ? 0 : 1;  //Đã xem
                    _lessonHistoryRepo.Update(courseLessonHis);
                    return Json(courseLessonHis.Status);  //bằng 1 là đã  xem, bằng 0 là chưa xem
                }
            }
            catch
            {
                return Json(-1);
            }


        }
        public class FileShow
        {
            public string ID { get; set; }
            public string NameFile { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetBlobDownload(string file_name)
        {
            try
            {
                //file_name = "6.pdf";
                var tempPath = "http://192.168.1.2:8083/api/Upload/Course/" + file_name;
                int idFile = TextUtils.ToInt(Path.GetFileNameWithoutExtension(file_name));
                CourseFile fileCourse = _fileCourseRepo.GetAllList().FirstOrDefault(c => c.Id == idFile);
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(tempPath))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = new StreamContent(await response.Content.ReadAsStreamAsync());
                            var contentType = "APPLICATION/octet-stream";
                            return File(await content.ReadAsByteArrayAsync(), contentType, fileCourse.NameFile);
                        }
                        else
                        {
                            ErrorViewModel errorView = new ErrorViewModel()
                            {
                                Message = $"{response.StatusCode.ToString()} [{fileCourse.NameFile}]",
                            };
                            return RedirectToAction("Error", "Home", errorView);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = ex.Message
                };
                return RedirectToAction("Error", "Home", errorView);
            }

        }
        public JsonResult CreateExamResult([FromBody] CourseExamResult examResult)
        {
            var employeeID = HttpContext.Session.GetInt32("employeeid");
            var fullname = HttpContext.Session.GetString("fullname");
            var model = _cousrseExamResult.GetAll().FirstOrDefault(p => p.CourseExamId == examResult.CourseExamId && p.EmployeeId == employeeID);
            JsonResult jsonResult;

            if (model != null)
            {
                model.UpdatedBy = fullname;
                model.UpdatedDate = DateTime.Now;
                var obj = _cousrseExamResult.Update(model);
                jsonResult = Json(obj, new System.Text.Json.JsonSerializerOptions());
            }
            else
            {
                CourseExamResult result = new CourseExamResult()
                {
                    CourseExamId = examResult.CourseExamId,
                    EmployeeId = employeeID,
                    TotalCorrect = 0,
                    TotalIncorrect = 0,
                    PercentageCorrect = 0,
                    CreatedBy = fullname,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = fullname,
                    UpdatedDate = DateTime.Now
                };
                var obj = _cousrseExamResult.Create(result);

                jsonResult = Json(obj, new System.Text.Json.JsonSerializerOptions());
            }

            return jsonResult;

        }
        public JsonResult GetExamResult(int courseID, int employeeID)
        {
            return Json(_cousrseExamResult.GetAll().FirstOrDefault(p => p.CourseExamId == courseID && p.EmployeeId == employeeID));
        }

        //Get danh sách bài học
        public JsonResult GetCourseLesson(int courseId)
        {
            //try
            //{
            //    int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            //    List<CourseLessonDTO> listLesson = SQLHelper<CourseLessonDTO>.ProcedureToList("spGetCourseLessonByCourseID",
            //                                                                                    new string[] { "@CourseID", "@EmployeeID" },
            //                                                                                    new object[] { courseId, employeeID });
            //    return Json(listLesson, new System.Text.Json.JsonSerializerOptions());
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            try
            {
                int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
                List<CourseLessonDTO> listLesson = SQLHelper<CourseLessonDTO>.ProcedureToList("spGetCourseLessonByCourseID",
                                                                                                new string[] { "@CourseID", "@EmployeeID" },
                                                                                                new object[] { courseId, employeeID });
                //========================================================= lee min khooi update 19/09/2024 ===========================================
                List<object> lstData = new List<object>();
                foreach (CourseLessonDTO item in listLesson)
                {
                    List<CourseExam> lstExam = _examRepo.GetAll().Where(p => p.LessonId == item.ID).ToList();

                    lstData.Add(new { lstLesson = item, lstExam = lstExam });
                }




                return Json(lstData, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get danh sách file đính kèm
        public JsonResult GetCourseFile(int lessonID)
        {
            try
            {
                List<CourseFileDTO> listFile = SQLHelper<CourseFileDTO>.SqlToList($"SELECT * FROM dbo.CourseFile WHERE LessonID = {lessonID} AND IsDeleted <> 1");
                if (listFile.Count > 0)
                {
                    foreach (var item in listFile)
                    {
                        item.FileName = item.ID + Path.GetExtension(item.NameFile);
                    }
                }

                return Json(listFile, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IActionResult GetCourseExam(int courseId)
        {
            try
            {
                var list = _cousrseExam.GetAll().Where(p => p.CourseId == courseId);
                return Json(list, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
    }
}
