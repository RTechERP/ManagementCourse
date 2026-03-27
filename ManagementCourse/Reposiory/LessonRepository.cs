using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using ManagementCourse.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class LessonRepository : GenericRepository<CourseLesson>
    {
        RTCContext _context;
        CourseLessonHistoryRepository _historyRepository;
        public LessonRepository(RTCContext rTCContext, CourseLessonHistoryRepository courseLessonHistoryRepository)
        {
            _context = rTCContext;
            _historyRepository = courseLessonHistoryRepository;
        }

        //Lấy tất cả bài học trong khóa học đẩy để hiển thị 
        public List<CourseLessonViewModel> ListCourseLessonFromCourse(int courseId, int employeeId)
        {

            var lessons = from cl in _context.CourseLessons
                          join c in _context.Courses on cl.CourseId equals c.Id
                          where cl.CourseId == courseId
                          let status = _context.CourseLessonHistories.Where(c => c.EmployeeId == employeeId && c.LessonId == cl.Id).FirstOrDefault()
                          select new CourseLessonViewModel
                          {
                              Id = cl.Id,
                              Stt = cl.Stt,
                              LessonTitle = cl.LessonTitle,
                              Status = (status == null) ? 0 : status.Status,
                              CourseId = c.Id,
                              Code = cl.Code,
                              LessonContent = cl.LessonContent,
                              Duration = cl.Duration,
                              VideoUrl = cl.VideoUrl,
                              CreatedBy = cl.CreatedBy,
                              CreatedDate = cl.CreatedDate,
                              UpdatedBy = cl.UpdatedBy,
                              UpdatedDate = cl.UpdatedDate,
                              FileCourseId = cl.FileCourseId,
                              UrlPdf = cl.UrlPdf,

                          };
            return lessons.OrderBy(c => c.Stt).ToList();

        }

        //Lấy bài học đầu tiên trong khóa học
        public CourseLessonViewModel GetCourseLessonFromCourse(int courseId,int employeeId)
        {
            var xxx = _context.CourseLessonHistories.Where(c => c.EmployeeId == employeeId && c.LessonId == 23).FirstOrDefault();
            var lessons = from cl in _context.CourseLessons
                          join c in _context.Courses on cl.CourseId equals c.Id
                          where cl.CourseId == courseId
                          let status = _context.CourseLessonHistories.Where(c => c.EmployeeId == employeeId && c.LessonId == cl.Id).FirstOrDefault()
                          select new CourseLessonViewModel
                          {
                              Id = cl.Id,
                              Stt = cl.Stt,
                              LessonTitle = cl.LessonTitle,
                              Status = (status == null) ? 0 : status.Status,
                              CourseId = c.Id,
                              Code = cl.Code,
                              LessonContent = cl.LessonContent,
                              Duration = cl.Duration,
                              VideoUrl = cl.VideoUrl,
                              CreatedBy = cl.CreatedBy,
                              CreatedDate = cl.CreatedDate,
                              UpdatedBy = cl.UpdatedBy,
                              UpdatedDate = cl.UpdatedDate,
                              FileCourseId = cl.FileCourseId,
                              UrlPdf = cl.UrlPdf,

                          };
            return lessons.OrderBy(c => c.Stt).FirstOrDefault();

        }
    
        public CourseLessonViewModel GetCourseLessonById(int courseId, int employeeId,int lessonId)
        {
            var lessons = from cl in _context.CourseLessons
                          join c in _context.Courses on cl.CourseId equals c.Id
                          where cl.CourseId == courseId && cl.Id == lessonId
                          let status = _context.CourseLessonHistories.Where(c => c.EmployeeId == employeeId && c.LessonId == lessonId).FirstOrDefault()
                          select new CourseLessonViewModel
                          {
                              Id = cl.Id,
                              Stt = cl.Stt,
                              LessonTitle = cl.LessonTitle,
                              Status = (status == null) ? 0 : status.Status,
                              CourseId = c.Id,
                              Code = cl.Code,
                              LessonContent = cl.LessonContent,
                              Duration = cl.Duration,
                              VideoUrl = cl.VideoUrl,
                              CreatedBy = cl.CreatedBy,
                              CreatedDate = cl.CreatedDate,
                              UpdatedBy = cl.UpdatedBy,
                              UpdatedDate = cl.UpdatedDate,
                              FileCourseId = cl.FileCourseId,
                              UrlPdf = cl.UrlPdf,

                          };
            return lessons.FirstOrDefault();

        }
    }
}
