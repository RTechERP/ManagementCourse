using ManagementCourse.Common;
using ManagementCourse.Models;
using ManagementCourse.Models.Context;
using ManagementCourse.Reposiory;
using Microsoft.AspNetCore.Builder;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Extensions;
using AspNetCoreHero.ToastNotification.Notyf;

namespace ManagementCourse
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "DefaultOrigins",
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:26179/");
                                  });
            });


            services.AddControllersWithViews();
            services.AddSession(cfg =>
            {
                cfg.IdleTimeout = TimeSpan.FromHours(8);
                cfg.Cookie.HttpOnly = true;
                cfg.Cookie.IsEssential = true;
            });
            services.AddScoped<LessonRepository>();
            services.AddScoped<CourseRepository>();
            services.AddScoped<CourseCatalogRepository>();
            services.AddScoped<FileCourseRepository>();
            services.AddScoped<CourseLessonHistoryRepository>();
            services.AddScoped<DepartmentRepository>();
            services.AddScoped<RTCContext>();
            services.AddScoped<CourseExamRepository>();
            services.AddScoped<GenericRepository<CourseExamResult>>();
            services.AddScoped<GenericRepository<CourseExam>>();
            services.AddScoped<GenericRepository<CourseRightAnswer>>();
            services.AddScoped<GenericRepository<CourseExamResult>>();
            services.AddScoped<GenericRepository<CourseQuestion>>();
            services.AddScoped<GenericRepository<CourseLesson>>();
            services.AddScoped<GenericRepository<CourseLessonHistory>>();
            services.AddScoped<INotyfService, NotyfService>();
            services.AddNotyf(config =>
            {
                config.DurationInSeconds = 3;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopRight;
            });
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            
            app.UseRouting();
            //Session
            app.UseSession();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Configuration.GetValue<string>("FilePath")),
                RequestPath = new PathString("/FilePDF")
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(Configuration.GetValue<string>("FilePath")),
                RequestPath = new PathString("/FilePDF")
            });

            app.UseCors("DefaultOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Login}/{id?}");
            });

        }
    }
}
