using ManagementCourse.Models;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManagementCourse.Component
{
    public class MenuViewComponent:ViewComponent
    {
        DepartmentRepository departmentRepo = new DepartmentRepository();
        CourseCatalogRepository courseCatalogRepo = new CourseCatalogRepository();
        private readonly IConfiguration _configuration;

        public MenuViewComponent(IConfiguration  configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            //var idDepartment = HttpContext.Session.GetInt32("department_id") ?? 0;
            //ViewBag.ListDepartment =  departmentRepo.GetByID(idDepartment);

            var listCourseCatalogs = courseCatalogRepo.GetAllList().Where(c => c.DeleteFlag == true).OrderBy(x => x.Stt).ToList();


            //TN.Binh update 17/12/2025
            // Lấy loginName từ Session
            var loginName = HttpContext.Session.GetString("loginname");

            // Lấy cấu hình từ appsettings
            List<string> arrLoginNameConfig = new List<string>();
            List<int> arrCourseCatalogConfig = new List<int>();

            // Đọc mảng ArrLoginName từ appsettings
            try
            {
                var arrLoginNameSection = _configuration.GetSection("SpecialUser:ArrLoginName");
                if (arrLoginNameSection.Exists())
                {
                    arrLoginNameConfig = arrLoginNameSection.Get<string[]>()?.ToList() ?? new List<string>();
                }
            }
            catch
            {
                arrLoginNameConfig = new List<string>();
            }

            // Đọc mảng ArrCourseCatalog từ appsettings
            try
            {
                var arrCourseCatalogSection = _configuration.GetSection("SpecialUser:ArrCourseCatalog");
                if (arrCourseCatalogSection.Exists())
                {
                    arrCourseCatalogConfig = arrCourseCatalogSection.Get<int[]>()?.ToList() ?? new List<int>();
                }
            }
            catch
            {
                arrCourseCatalogConfig = new List<int>();
            }

            // Nếu loginName có trong ArrLoginName, lọc theo ArrCourseCatalog
            bool isUserSpecial = false;
            if (!string.IsNullOrEmpty(loginName) &&
                arrLoginNameConfig != null && arrLoginNameConfig.Count > 0 &&
                arrLoginNameConfig.Any(x => x.Equals(loginName, StringComparison.OrdinalIgnoreCase)) &&
                arrCourseCatalogConfig != null && arrCourseCatalogConfig.Count > 0)
            {
                isUserSpecial = true;
                listCourseCatalogs = listCourseCatalogs.Where(c => arrCourseCatalogConfig.Contains(c.Id)).ToList();
            }
            //end

            ViewBag.ListcourseCatalog = listCourseCatalogs;
            ViewBag.ListDepartment = listCourseCatalogs.Select(x => x.DepartmentId).Distinct().ToList();
            ViewBag.Department = HttpContext.Session.GetString("department");

            List<int?> listDepartmentCatalogs = listCourseCatalogs.Select(x => x.DepartmentId).Distinct().ToList();
            var listDepartments = departmentRepo.GetAll().Where(x => listDepartmentCatalogs.Contains(x.Id)).ToList();
            var tuple = new Tuple<List<Department>, List<CourseCatalog>,bool>(listDepartments, listCourseCatalogs, isUserSpecial);
            return View(tuple);
        }
        //public IViewComponentResult Invoke()
        //{
        //    //var idDepartment = HttpContext.Session.GetInt32("department_id") ?? 0;
        //    //ViewBag.ListDepartment =  departmentRepo.GetByID(idDepartment);

        //    var listCourseCatalogs = courseCatalogRepo.GetAllList().Where(c => c.DeleteFlag == true).OrderBy(x => x.Stt).ToList();
        //    ViewBag.ListcourseCatalog = listCourseCatalogs;
        //    ViewBag.ListDepartment = listCourseCatalogs.Select(x => x.DepartmentId).Distinct().ToList();
        //    ViewBag.Department = HttpContext.Session.GetString("department");

        //    List<int?> listDepartmentCatalogs = listCourseCatalogs.Select(x => x.DepartmentId).Distinct().ToList();
        //    var listDepartments = departmentRepo.GetAll().Where(x => listDepartmentCatalogs.Contains(x.Id)).ToList();
        //    var tuple = new Tuple<List<Department>, List<CourseCatalog>>(listDepartments, listCourseCatalogs);
        //    return View(tuple);


        //}
    }
}
