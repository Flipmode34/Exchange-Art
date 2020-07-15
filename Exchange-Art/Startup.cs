using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Exchange_Art.Data;
using Exchange_Art.Models;
using Exchange_Art.IdentityPolicy;

namespace Exchange_Art
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IPasswordValidator<ApplicationUser>, CustomPasswordPolicy>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
               {
                   options.User.RequireUniqueEmail = true;
               }
            ).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();

            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            //await EnsuresRolesAndUsers(app);
        }

        private static async Task EnsuresRolesAndUsers(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                if (!await roleManager.RoleExistsAsync(Roles.TEACHER_ROLE))
                {
                    var teacherRole = new IdentityRole(Roles.TEACHER_ROLE);
                    await roleManager.CreateAsync(teacherRole);

                    var studentRole = new IdentityRole(Roles.STUDENT_ROLE);
                    await roleManager.CreateAsync(studentRole);

                    // Reference new Teacher users
                    var teacher0 = new ApplicationUser
                    {
                        UserName = "Henk@henk.nl",
                        Email = "Henk@henk.nl"
                    };

                    var teacher1 = new ApplicationUser
                    {
                        UserName = "Marie@marie.nl",
                        Email = "Marie@marie.nl"
                    };

                    var teacher2 = new ApplicationUser
                    {
                        UserName = "Erik@erik.nl",
                        Email = "Erik@erik.nl"
                    };

                    var teacher3 = new ApplicationUser
                    {
                        UserName = "Sara@sara.nl",
                        Email = "Sara@sara.nl"
                    };

                    // Create actual user with passwords
                    await userManager.CreateAsync(teacher0, "HenkIsDeB0m!");
                    await userManager.CreateAsync(teacher1, "MarieIsDeB0m!");
                    await userManager.CreateAsync(teacher2, "ErikIsDeB0m!");
                    await userManager.CreateAsync(teacher3, "SaraIsDeB0m!");

                    // Add teacher role to user
                    await userManager.AddToRoleAsync(teacher0, Roles.TEACHER_ROLE);
                    await userManager.AddToRoleAsync(teacher1, Roles.TEACHER_ROLE);
                    await userManager.AddToRoleAsync(teacher2, Roles.TEACHER_ROLE);
                    await userManager.AddToRoleAsync(teacher3, Roles.TEACHER_ROLE);
                }
            }
        }
    }
}