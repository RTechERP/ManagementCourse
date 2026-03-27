using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Models.DTO;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagementCourse.Controllers
{
    public class CourseExamResultController : Controller
    {
        CourseExamResultRepository examResultRepo = new CourseExamResultRepository();
        CourseExamResultDetailRepository resultDetailRepo = new CourseExamResultDetailRepository();
        CourseRepository courseRepo = new CourseRepository();
        CourseQuestionRepository questionRepo = new CourseQuestionRepository();
        CourseExamExcerciseFileRepo fileRepo = new CourseExamExcerciseFileRepo();
        CourseExamEvaluateRepo courseExamEvaluateRepo = new CourseExamEvaluateRepo();

        CourseLessonRepository lessonRepo = new CourseLessonRepository();
        CourseLessonHistoryRepository courseLessonHistoryRepo = new CourseLessonHistoryRepository(null);

        public IActionResult Index(int courseId)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            Course course = courseRepo.GetByID(courseId);
            if (course == null)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Không tìm thấy khoá học!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }
            CourseExam courseExam = SQLHelper<CourseExam>.SqlToModel($"SELECT TOP 1 * FROM dbo.CourseExam WHERE CourseId = {courseId} and ExamType = 1");

            if (courseExam.Id <= 0)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Khoá học [{course.NameCourse}] chưa có bài kiểm tra!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }
            else
            {
                ViewBag.CourseId = courseId;
                ViewBag.CourseName = TextUtils.ToString(course.NameCourse);
                ViewBag.CourseExamID = courseExam.Id;
                ViewBag.TestTime = TextUtils.ToInt(courseExam.TestTime);
                int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
                List<CourseLessonDTO> listLesson = examResultRepo.GetLesson(courseId, employeeID);
                var status = listLesson.Where(x => x.Status == 0).ToList();
                if (status.Count > 0)
                {
                    ErrorViewModel errorView = new ErrorViewModel()
                    {
                        Message = $"Bạn phải học hết tất cả các bài học trước!"
                    };
                    return RedirectToAction("Error", "Home", errorView);
                }
            }
            return View();
        }

        public JsonResult GetExamResult(int courseExamID)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return Json("Phiên đăng nhập đã hết.\nVui lòng load lại trang để đăng nhập lại!", new System.Text.Json.JsonSerializerOptions());
            }

            int employeeId = (int)HttpContext.Session.GetInt32("employeeid");
            List<CourseExamResultDTO> listExamResult = SQLHelper<CourseExamResultDTO>.ProcedureToList("spGetCourseExamResult",
                                                                                            new string[] { "@CourseExamID", "@EmployeeID", "@OrderNumber" },
                                                                                            new object[] { courseExamID, employeeId, 0 });

            return Json(listExamResult, new System.Text.Json.JsonSerializerOptions());
        }

        [HttpPost]
        public JsonResult CreateExamResult([FromBody] CourseExamResult examResult)
        {
            try
            {
                if (HttpContext.Session.GetInt32("userid") == null)
                {
                    return Json("Phiên đăng nhập đã hết.\nVui lòng load lại trang để đăng nhập lại!", new System.Text.Json.JsonSerializerOptions());
                }

                CourseExamResult courseExamResult = new CourseExamResult();
                courseExamResult.CourseExamId = examResult.CourseExamId;
                courseExamResult.EmployeeId = HttpContext.Session.GetInt32("employeeid");
                courseExamResult.TotalCorrect = courseExamResult.TotalIncorrect = 0;
                courseExamResult.PercentageCorrect = 0;
                courseExamResult.Status = 0;
                courseExamResult.CreatedBy = courseExamResult.UpdatedBy = HttpContext.Session.GetString("loginname");
                courseExamResult.CreatedDate = courseExamResult.UpdatedDate = DateTime.Now;

                int id = 0;

                if (examResultRepo.Create(courseExamResult) == 1)
                {
                    id = courseExamResult.Id;
                    return Json(id, new System.Text.Json.JsonSerializerOptions());
                }
                else
                {
                    return Json(id, new System.Text.Json.JsonSerializerOptions());
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        //public JsonResult GetExamQuestion(int courseId, int courseExamResultID, int examType = 1)
        //{
        //    var list = questionRepo.ListExamQuestion(courseId, courseExamResultID, examType);
        //    return Json(list, new System.Text.Json.JsonSerializerOptions());
        //}

        [HttpPost]
        public JsonResult CreateExamResultDetail([FromBody] List<CourseExamResultDetail> details)
        {

            int questionId = 0;
            List<int> listAnswerId = new List<int>();
            int employeeId = (int)HttpContext.Session.GetInt32("employeeid");
            try
            {
                if (HttpContext.Session.GetInt32("userid") == null)
                {
                    return Json("Phiên đăng nhập đã hết.\nVui lòng load lại trang để đăng nhập lại!", new System.Text.Json.JsonSerializerOptions());
                }

                if (details.Count <= 0)
                {
                    return Json(1, new System.Text.Json.JsonSerializerOptions());
                }
                var existingResultDetails = resultDetailRepo.GetAll().Where(p => p.CourseExamResultId == details.First().CourseExamResultId && p.CourseQuestionId == details.First().CourseQuestionId).ToList();
                resultDetailRepo.RemoveRange(existingResultDetails);
                foreach (CourseExamResultDetail item in details)
                {
                    CourseExamResultDetail detail = item;
                    detail.CreatedDate = item.UpdatedDate = DateTime.Now;
                    detail.CreatedBy = item.UpdatedBy = HttpContext.Session.GetString("loginname");
                    if (resultDetailRepo.Create(detail) == 1)
                    {
                        questionId = (int)detail.CourseQuestionId;
                        listAnswerId.Add((int)detail.CourseAnswerId);
                    }
                }

                var tuple = new Tuple<int, List<int>>(questionId, listAnswerId);
                return Json(tuple, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IActionResult GetQuestionAnswerRight(int courseExamResultId)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return Json("Phiên đăng nhập đã hết.\nVui lòng load lại trang để đăng nhập lại!", new System.Text.Json.JsonSerializerOptions());
            }

            int employeeId = (int)HttpContext.Session.GetInt32("employeeid");
            var listQuestionRight = SQLHelper<CourseExamResultDetail>.ProcedureToList("spGetQuestionAnswerRight", new string[] { "@EmployeeID", "@CourseResultID" }, new object[] { employeeId, courseExamResultId });
            return Json(listQuestionRight, new System.Text.Json.JsonSerializerOptions());
        }

        public IActionResult Practice(int courseId = 0, int lessonID = 0)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            CourseLesson lesson = new CourseLesson();
            Course course = new Course();

            string tableName = lessonID > 0 ? "Bài học" : "Khóa học";
            string fatherID = lessonID > 0 ? "LessonID" : "CourseID";

            if (lessonID > 0) lesson = lessonRepo.GetByID(lessonID) ?? new CourseLesson();
            else if (courseId > 0) course = courseRepo.GetByID(courseId) ?? new Course();

            if (lesson.Id <= 0 && course.Id <= 0)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Không tìm thấy {tableName}!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }
            string examName = lessonID > 0 ? lesson.LessonTitle : course.NameCourse;
            int id = lessonID > 0 ? lessonID : courseId;
            CourseExam courseExam = SQLHelper<CourseExam>.SqlToModel($"SELECT TOP 1 * FROM dbo.CourseExam WHERE {fatherID} = {id} and ExamType = 2");
            if (courseExam.Id <= 0)
            {

                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"{tableName} [{examName}] chưa có bài kiểm tra!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }

            ViewBag.TestTime = TextUtils.ToInt(courseExam.TestTime);
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");

            List<CourseLessonDTO> listLesson = examResultRepo.GetLesson(courseId, employeeID).Where(x => x.Status == 0).ToList();
            CourseLessonHistory isStudy = courseLessonHistoryRepo.GetAll().FirstOrDefault(p => p.LessonId == lessonID && p.EmployeeId == employeeID && p.Status == 1);
            bool isNotValid = (lessonID > 0 && isStudy == null) || (listLesson.Count > 0);
            if (isNotValid)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Bạn phải hoàn thành học {tableName} trước!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }

            ViewBag.TableName = tableName;
            ViewBag.CourseId = courseId;
            ViewBag.LessonId = lessonID;
            ViewBag.CourseName = TextUtils.ToString(examName);
            ViewBag.CourseExamID = courseExam.Id;

            //if (HttpContext.Session.GetInt32("userid") == null)
            //{
            //    return RedirectToAction("Login", "Home");
            //}

            //Course course = courseRepo.GetByID(courseId);
            //if (course == null)
            //{
            //    ErrorViewModel errorView = new ErrorViewModel()
            //    {
            //        Message = $"Không tìm thấy khoá học!"
            //    };
            //    return RedirectToAction("Error", "Home", errorView);
            //}
            //CourseExam courseExam = SQLHelper<CourseExam>.SqlToModel($"SELECT TOP 1 * FROM dbo.CourseExam WHERE CourseId = {courseId} and ExamType = 2");

            //if (courseExam.Id <= 0)
            //{
            //    ErrorViewModel errorView = new ErrorViewModel()
            //    {
            //        Message = $"Khoá học [{course.NameCourse}] chưa có bài kiểm tra!"
            //    };
            //    return RedirectToAction("Error", "Home", errorView);
            //}
            //else
            //{
            //    ViewBag.CourseId = courseId;
            //    ViewBag.CourseName = TextUtils.ToString(course.NameCourse);
            //    ViewBag.CourseExamID = courseExam.Id;
            //    ViewBag.TestTime = TextUtils.ToInt(courseExam.TestTime);
            //    int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            //    List<CourseLessonDTO> listLesson = examResultRepo.GetLesson(courseId, employeeID);
            //    var status = listLesson.Where(x => x.Status == 0).ToList();
            //    if (status.Count > 0)
            //    {
            //        ErrorViewModel errorView = new ErrorViewModel()
            //        {
            //            Message = $"Bạn phải học hết tất cả các bài học trước!"
            //        };
            //        return RedirectToAction("Error", "Home", errorView);
            //    }
            //   /* var examResult = examResultRepo.Find(p => p.CourseExamId == courseExam.Id && p.EmployeeId == employeeID).OrderByDescending(p => p.CreatedDate).FirstOrDefault();
            //    if (examResult == null)
            //    {
            //        examResult = new CourseExamResult();
            //        examResult.CourseExamId = courseExam.Id;
            //        examResult.UpdatedDate = DateTime.Now;
            //        examResult.CreatedDate = DateTime.Now;
            //        examResult.CreatedBy = HttpContext.Session.GetString("loginname");
            //        examResult.UpdatedBy = HttpContext.Session.GetString("loginname");
            //        examResult.EmployeeId = employeeID;
            //        examResultRepo.Create(examResult);
            //    }
            //    ViewBag.ExamResultID = examResult.Id;
            //    ViewBag.DateExamResult = examResult.CreatedDate;*/
            //}
            return View();
        }
        public IActionResult Exercise(int courseId = 0, int lessonID = 0)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            CourseLesson lesson = new CourseLesson();
            Course course = new Course();

            string tableName = lessonID > 0 ? "Bài học" : "Khóa học";
            string fatherID = lessonID > 0 ? "LessonID" : "CourseID";

            if (lessonID > 0) lesson = lessonRepo.GetByID(lessonID) ?? new CourseLesson();
            else if (courseId > 0) course = courseRepo.GetByID(courseId) ?? new Course();

            if (lesson.Id <= 0 && course.Id <= 0)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Không tìm thấy {tableName}!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }
            string examName = lessonID > 0 ? lesson.LessonTitle : course.NameCourse;
            int id = lessonID > 0 ? lessonID : courseId;
            CourseExam courseExam = SQLHelper<CourseExam>.SqlToModel($"SELECT TOP 1 * FROM dbo.CourseExam WHERE {fatherID} = {id} and ExamType = 3");
            if (courseExam.Id <= 0)
            {

                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"{tableName} [{examName}] chưa có bài kiểm tra!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }

            ViewBag.TestTime = TextUtils.ToInt(courseExam.TestTime);
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");

            List<CourseLessonDTO> listLesson = examResultRepo.GetLesson(courseId, employeeID).Where(x => x.Status == 0).ToList();
            CourseLessonHistory isStudy = courseLessonHistoryRepo.GetAll().FirstOrDefault(p => p.LessonId == lessonID && p.EmployeeId == employeeID && p.Status == 1);
            bool isNotValid = (lessonID > 0 && isStudy == null) || (listLesson.Count > 0);
            if (isNotValid)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Bạn phải hoàn thành học {tableName} trước!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }

            ViewBag.TableName = tableName;
            ViewBag.CourseId = courseId;
            ViewBag.LessonId = lessonID;
            ViewBag.CourseName = TextUtils.ToString(examName);
            ViewBag.CourseExamID = courseExam.Id;

            //if (HttpContext.Session.GetInt32("userid") == null)
            //{
            //    return RedirectToAction("Login", "Home");
            //}

            //Course course = courseRepo.GetByID(courseId);
            //if (course == null)
            //{
            //    ErrorViewModel errorView = new ErrorViewModel()
            //    {
            //        Message = $"Không tìm thấy khoá học!"
            //    };
            //    return RedirectToAction("Error", "Home", errorView);
            //}
            //CourseExam courseExam = SQLHelper<CourseExam>.SqlToModel($"SELECT TOP 1 * FROM dbo.CourseExam WHERE CourseId = {courseId} and ExamType = 3");

            //if (courseExam.Id <= 0)
            //{
            //    ErrorViewModel errorView = new ErrorViewModel()
            //    {
            //        Message = $"Khoá học [{course.NameCourse}] chưa có bài kiểm tra!"
            //    };
            //    return RedirectToAction("Error", "Home", errorView);
            //}
            //else
            //{
            //    ViewBag.CourseId = courseId;
            //    ViewBag.CourseName = TextUtils.ToString(course.NameCourse);
            //    ViewBag.CourseExamID = courseExam.Id;
            //    ViewBag.TestTime = TextUtils.ToInt(courseExam.TestTime);
            //    int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            //    List<CourseLessonDTO> listLesson = examResultRepo.GetLesson(courseId, employeeID);
            //    var status = listLesson.Where(x => x.Status == 0).ToList();
            //    if (status.Count > 0)
            //    {
            //        ErrorViewModel errorView = new ErrorViewModel()
            //        {
            //            Message = $"Bạn phải học hết tất cả các bài học trước!"
            //        };
            //        return RedirectToAction("Error", "Home", errorView);
            //    }
            //}
            return View();
        }


        public async Task<IActionResult> UploadFile(int examResultID)
        {
            try
            {
                if (HttpContext.Session.GetInt32("userid") == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                string pathUpload = "\\\\192.168.1.190\\Common\\08. SOFTWARES\\LeTheAnh\\BaiTap";
                var files = Request.Form.Files;
                List<CourseExamExcerciseFile> listFiles = new List<CourseExamExcerciseFile>();
                foreach (var file in files)
                {
                    CourseExamExcerciseFile fileExam = new CourseExamExcerciseFile()
                    {
                        CourseExamResultId = examResultID,
                        FileName = file.FileName,
                        FilePath = pathUpload,
                        CreatedBy = HttpContext.Session.GetString("loginname"),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = HttpContext.Session.GetString("loginname"),
                        UpdatedDate = DateTime.Now
                    };
                    fileRepo.Create(fileExam);
                    fileExam.FilePath = Path.Combine(pathUpload, fileExam.Id.ToString() + Path.GetExtension(file.FileName));
                    fileRepo.Update(fileExam);
                    using (var client = new HttpClient())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            if (file.Length < 0)
                            {
                                continue;
                            }
                            using var fileStream = file.OpenReadStream();
                            byte[] bytes = new byte[file.Length];
                            fileStream.Read(bytes, 0, (int)file.Length);
                            var byteArrayContent = new ByteArrayContent(bytes);
                            content.Add(byteArrayContent, "file", fileExam.Id.ToString() + Path.GetExtension(file.FileName));
                            var url = $"http://113.190.234.64:8083/api/Home/uploadfile?path={pathUpload}";
                            var result = await client.PostAsync(url, content);
                        }
                    }
                }
                return Ok(new { status = 1, message = "Upload file thành công" });
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                return BadRequest(new { status = 1, message = ex.Message });
            }
        }

        public IActionResult GetQuestionPracticeAndExercise(int examResultID, int examType)
        {
            var listQuestion = SQLHelper<ExamEvaluateDTO>.ProcedureToList("spGetQuestionPracticeAndExercise", new string[] { "@CourseExamResultID", "@ExamType" }, new object[] { examResultID, examType });
            return Json(listQuestion, new JsonSerializerOptions());
        }


        public IActionResult UpdateCompleted(int id, int quesID, int examResultID)
        {
            if (id > 0)
            {
                CourseExamEvaluate examEvaluate = courseExamEvaluateRepo.GetByID(id);
                if (examEvaluate.Evaluate) return Json(new { status = 0, message = "Bài thực hành đã được duyệt!" });
                courseExamEvaluateRepo.Update(examEvaluate);
            }
            else
            {
                CourseExamEvaluate examEvaluate = new CourseExamEvaluate();
                examEvaluate.DateEvaluate = DateTime.Now;
                examEvaluate.CourseQuestionId = quesID;
                examEvaluate.CourseExamResultId = examResultID;
                courseExamEvaluateRepo.Create(examEvaluate);
            }
            return Json(new { status = 1, message = "" });
        }
        //=============================================================  LMK update ======================================================================================
        public IActionResult QuestionDetails(int questionId = 0, int courseId = 0)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            CourseQuestion coursePractice = questionRepo.GetByID(questionId);
            if (coursePractice == null)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Không tìm thấy câu hỏi!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }
            Course course = courseRepo.GetByID(courseId) ?? new Course();
            ViewBag.NameCourse = course.NameCourse;
            ViewBag.STT = coursePractice.Stt;
            ViewBag.QuestionText = coursePractice.QuestionText;

            //if (string.IsNullOrEmpty(coursePractice.Image)) ViewBag.Image = "https://i.kym-cdn.com/entries/icons/original/000/029/857/Shirogane_learns_Volleyball_2-3_screenshot.png";
            if (string.IsNullOrWhiteSpace(coursePractice.Image)) ViewBag.Image = "";
            else ViewBag.Image = "http://113.190.234.64:8083/api/Upload/Images/Courses/" + coursePractice.Image;

            return View();
        }



        //================================================= lee min khooi update 19/09/2024 =================================================
        public IActionResult LessonExamQuiz(int lessonID)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            CourseLesson lesson = lessonRepo.GetByID(lessonID);
            if (lesson == null)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Không tìm thấy bài học!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }
            CourseExam courseExam = SQLHelper<CourseExam>.SqlToModel($"SELECT TOP 1 * FROM dbo.CourseExam WHERE LessonID = {lesson.Id} and ExamType = 1");

            if (courseExam.Id <= 0)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Bài học [{lesson.LessonTitle}] chưa có bài kiểm tra!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }



            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            CourseLessonHistory listLesson = courseLessonHistoryRepo.GetAll().Where(p => p.Status == 1 && p.LessonId == lesson.Id && p.EmployeeId == employeeID).FirstOrDefault();
            if (listLesson == null)
            {
                ErrorViewModel errorView = new ErrorViewModel()
                {
                    Message = $"Bạn phải học bài học [{lesson.LessonTitle}] trước khi thi!"
                };
                return RedirectToAction("Error", "Home", errorView);
            }

            ViewBag.LessonID = lesson.Id;
            ViewBag.LessonName = TextUtils.ToString(lesson.LessonTitle);
            ViewBag.CourseExamID = courseExam.Id;
            ViewBag.TestTime = TextUtils.ToInt(courseExam.TestTime);
            return View();
        }


        // lee min khooi update 20/09/2024
        public JsonResult GetExamQuestion(int courseId = 0, int courseExamResultID = 0, int examType = 1, int lessonID = 0)
        {
            var list = questionRepo.ListExamQuestion(courseId, courseExamResultID, examType, lessonID);
            return Json(list, new System.Text.Json.JsonSerializerOptions());
        }

    }
}
