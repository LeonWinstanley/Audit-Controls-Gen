using System;
using System.Threading.Tasks;
using FinalYearProject.Enums;
using FinalYearProject.Models;
using FinalYearProject.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;

namespace FinalYearProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMudServices();

            var builder = new SqlConnectionStringBuilder(
                Configuration.GetConnectionString("Database"));
            builder.Password = Configuration["DatabasePassword"];

            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.ConnectionString));
            services.AddScoped<UserService>();
            services.AddScoped<SignInOutService>();
            services.AddScoped<ControlsService>();
            services.AddScoped<EmailService>();
            services.AddIdentityCore<User>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
            services.AddScoped<PasswordHasher<User>>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "MyScheme";
            }).AddCookie("MyScheme", options =>
            {
                options.Cookie.Name = "AuditControlGenAuth";
            });
            
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });
            services.AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp => {
                var provider = (ServerAuthenticationStateProvider) sp.GetRequiredService<AuthenticationStateProvider>();
                return provider;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider ServiceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            CreateRoles(ServiceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider ServiceProvider)
        {
            //initializing custom roles 
            var roleManager = ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = ServiceProvider.GetRequiredService<UserManager<User>>();

            foreach (Role roleName in Enum.GetValues(typeof(Role)))
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName.ToString());
                if (!roleExist)
                {
                    //create the roles and seed them to the database.
                    await roleManager.CreateAsync(new IdentityRole(roleName.ToString()));
                }
            }
            
            // Create superUser. Password will be changed on setup
            var poweruser = new User
            {
                UserName = "admin",
                Email = "admin@admin",
                EmailConfirmed = true,
            };

            var tempUser = new User
            {
                FirstName = "temp",
                LastName = "temp",
                UserName = "temp",
                Email = "temp@temp",
                EmailConfirmed = true
            };

            string password = "!!P@assword123";
            var user = await UserManager.FindByEmailAsync("admin");
            var tUser = await UserManager.FindByEmailAsync("temp");
            if (user == null)
            {
                Console.WriteLine("Creating Admin Account...");
                var createPowerUser = await UserManager.CreateAsync(poweruser, password);
                if (createPowerUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(poweruser, Role.Developer.ToString());
                    Console.WriteLine("Admin Account Created...");
                }
            }
            else
            {
                Console.WriteLine("Admin Account Already Exists...");
            }

            if (tUser == null)
            {
                Console.WriteLine("Creating Temp User...");
                var createTempUser = await UserManager.CreateAsync(tempUser, password);
                if (createTempUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(tempUser, Role.Auditor.ToString());
                    Console.WriteLine("Temp User Created...");
                }
            }
            else
            {
                Console.WriteLine("Temp User Already Exists...");
            }
        }
    }
}
