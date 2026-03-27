using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseLessonHistoryRepository:GenericRepository<CourseLessonHistory>
    {
        RTCContext _context;
        public CourseLessonHistoryRepository(RTCContext rTCContext )
        {
            _context = rTCContext;
        }
        public CourseLessonHistory GetCourseLessonHistory(int employeeid,int lessonid)
        {
            return _context.CourseLessonHistories.Where(c=>c.EmployeeId == employeeid && c.LessonId == lessonid).FirstOrDefault();
             
        }
    }
}
