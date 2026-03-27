using ManagementCourse.Common;
using ManagementCourse.Models;
using System.Collections.Generic;
using System.Data;
using ManagementCourse.Models.DTO;
using System.Linq;

namespace ManagementCourse.Reposiory
{
    public class CourseRepository : GenericRepository<Course>
    {
        public List<CourseDTO> ListCourses(int departmentID, int courseCatalogID, string filterText, int employeeID, int catalogType, int kpiPositionTypeID)
        {
            //List<CourseDTO> list = new List<CourseDTO>();
            //DataSet dataSet = LoadDataFromSP.GetDataSetSP("spGetCourse",
            //                                                new string[] { "@DepartmentID", "@CourseCatalogID", "@FilterText", "@EmployeeID" },
            //                                                new object[] { departmentID, courseCatalogID, filterText, employeeID });
            //DataTable dt = dataSet.Tables[0];
            //if (dt.Rows.Count > 0)
            //{
            //    list = TextUtils.ConvertDataTable<CourseDTO>(dt);

            //}

            List<CourseDTO> listCourse = SQLHelper<CourseDTO>.ProcedureToList("spGetCourseNew",
                                                        new string[] { "@DepartmentID", "@CourseCatalogID", "@FilterText", "@EmployeeID", "@CatalogType", "@IsShowAll" },
                                                        new object[] { departmentID, courseCatalogID, filterText, employeeID, catalogType, 1});

            return listCourse;
        }
    }
}
