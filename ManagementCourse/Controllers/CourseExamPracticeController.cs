using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Models.DTO;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Controllers
{
    public class CourseExamPracticeController : Controller
    {
        CourseExamResultRepository examResultRepo = new CourseExamResultRepository();
        CourseExamEvaluateRepo examValuateRepo = new CourseExamEvaluateRepo();

        [HttpPost]
        public async Task<JsonResult> CreatePracticeResult([FromBody]  CourseExamResult data)
        {
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            data.EmployeeId = employeeID;
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = HttpContext.Session.GetString("loginname");
            data.Status = 0;
            await examResultRepo.CreateAsync(data);
            return Json(data);
        }
        public JsonResult GetQuesstion(int courseExamId)
        {
            List<CourseQuestion> data = SQLHelper<CourseQuestion>.SqlToList($"SELECT * FROM CourseQuestion WHERE CourseExamId = {courseExamId}")
                                                                 .OrderBy(x=>x.Stt).ToList();
            return Json(data);
        }

        [HttpPost]
        public CourseExamResult ConfirmPractice([FromBody] CourseExamResult data)
        {
            CourseExamResult examResult = examResultRepo.GetByID(data.Id);
            if (examResult == null) return null;
            examResult.Status = 1;
            examResultRepo.Update(examResult);
            return examResult;
        }


        [HttpPost]
        public async Task<bool> CreateListExamValuate([FromBody] List<CourseExamEvaluate> listData)
        {
            string loginName = HttpContext.Session.GetString("loginname");
            foreach (var item in listData)
            {
                CourseExamEvaluate newExamValuate = new CourseExamEvaluate();

                newExamValuate.CreatedDate = DateTime.Now;
                newExamValuate.CreatedBy = loginName;
                newExamValuate.CourseExamResultId = item.CourseExamResultId;
                newExamValuate.CourseQuestionId = item.CourseQuestionId;
                await examValuateRepo.CreateAsync(newExamValuate);
            }
            return true;
        }



        public JsonResult GetHistoryResultPractice(int courseExamId)
        {
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            List<CourseExamResultDTO> data = SQLHelper<CourseExamResultDTO>.ProcedureToList("spGetResultHistoryPractice", 
                                            new string[] { "@EmployeeId", "@CourseExamId" }, 
                                            new object[] { employeeID, courseExamId  });
            return Json(data);
        }
        public JsonResult GetResultPractice(int courseResultId)
        {
            int employeeID = (int)HttpContext.Session.GetInt32("employeeid");
            List<ExamEvaluateDTO> data = SQLHelper<ExamEvaluateDTO>.ProcedureToList("spGetResultHistoryByPractice", new string[] { "@EmployeeId", "@CourseResultId" }, new object[] { employeeID , courseResultId });
            return Json(data);
        }
    }
}
