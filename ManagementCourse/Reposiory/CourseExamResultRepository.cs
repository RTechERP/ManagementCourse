using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using ManagementCourse.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ManagementCourse.Reposiory
{
    public class CourseExamResultRepository:GenericRepository<CourseExamResult>
    {
        RTCContext _context = new RTCContext();
        public CourseExamResultRepository()
        {
        }

        public int CreateExam(CourseExamResult examResult)
        {
            CourseExamResult result = new CourseExamResult()
            {
                CourseExamId = examResult.CourseExamId,
                EmployeeId = examResult.EmployeeId,
                TotalCorrect = 0,
                TotalIncorrect = 0,
                PercentageCorrect = 0,
                CreatedBy = "",
                CreatedDate = DateTime.Now,
                UpdatedBy = "",
                UpdatedDate = DateTime.Now

            };
            _context.CourseExamResults.Add(result);

            return _context.SaveChanges();

        }


        public List<CourseLessonDTO> GetLesson(int courseID, int employeeID)
        {
            
            List<CourseLessonDTO> listLesson = SQLHelper<CourseLessonDTO>.ProcedureToList("spGetCourseLessonByCourseID",
                                                                                            new string[] { "@CourseID", "@EmployeeID" },
                                                                                            new object[] { courseID, employeeID });

            return listLesson;
        }
    }
}
