using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementCourse.Controllers
{
    public class CourseExamExerciseController : Controller
    {
        CourseExamResultRepository examResultRepo = new CourseExamResultRepository();
        CourseExamEvaluateRepo examValuateRepo = new CourseExamEvaluateRepo();

        [HttpPost]
        public async Task<JsonResult> CreatePracticeResult([FromBody] CourseExamResult data)
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
            List<CourseQuestion> data = SQLHelper<CourseQuestion>.SqlToList($"SELECT * FROM CourseQuestion WHERE CourseExamId = {courseExamId}");
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
    }
}
