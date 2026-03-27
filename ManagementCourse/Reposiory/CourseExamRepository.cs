using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseExamRepository : GenericRepository<CourseExam>
    {
        RTCContext _context;
        public CourseExamRepository(RTCContext rTCContext) 
        {
            _context = rTCContext;
        }
        public CourseExam GetCourseExam(int courseId)
        {
            return _context.CourseExams.Where(c => c.CourseId == courseId).FirstOrDefault();
        }
        public List<CourseQuestion> GetCourseQuestion(int examId)
        {
            return _context.CourseQuestions.Where(c => c.CourseExamId == examId).OrderBy(c => c.Stt).ToList();
        }
        public List<CourseAnswer> GetCourseAnswer()
        {
            return _context.CourseAnswers.ToList();
        }

       

        // Tạo đối tượng Random
        //Random rand = new Random();

        //// Tạo một danh sách gồm 10 số ngẫu nhiên từ 1 đến 100
        //List<int> randomList = new List<int>();
        //    for (int i = 0; i< 10; i++)
        //    {
        //        int num = rand.Next(1, 101); // tạo số ngẫu nhiên từ 1 đến 100
        //randomList.Add(num);
        //    }
        //    return _context.CourseAnswers.Where(c => c.CourseQuestionId == questionId).OrderBy(x => rand.Next()).ToList();


    }
}
