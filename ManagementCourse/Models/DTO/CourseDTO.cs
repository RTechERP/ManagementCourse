namespace ManagementCourse.Models.DTO
{
    public class CourseDTO
    {
        public int ID { get; set; }
        public string NameCourse { get; set; }
        public string Instructor { get; set; }
        public bool DeleteFlag { get; set; }
        public string DeleteFlagText { get; set; } 
        public int NumberLesson { get; set; } 
        public int TotalHistoryLession { get; set; } 
        public decimal PercentageCorrect { get; set; } 
        public int Evaluate { get; set; } 
        public string EvaluateText { get; set; } 
        public string DepartmentName { get; set; } 
        public string NameCourseCatalog { get; set; } 
        public int Status { get; set; }
        public int CatalogID { get; set; }
        public decimal PracticePoints { get; set; }
        public bool IsPractice { get; set; }
        public decimal QuizPoints { get; set; }
        public decimal ExcercisePoints { get; set; }
        public decimal LeadTime { get; set; }
        public decimal TotalTimeLearned { get; set; }
        public decimal GoalMultiChoice { get; set; }
        public decimal GoalPractice { get; set; }
        public decimal GoalExercise { get; set; }
        public int CatalogType { get; set; }
        public int CourseTypeID { get; set; }
        public string CourseTypeName { get; set; }
        public bool IsLearnInTurn { get; set; }
    }
}
