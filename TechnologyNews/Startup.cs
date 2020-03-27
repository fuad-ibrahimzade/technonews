using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechnologyNews.Data;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Identity;
using TechnologyNews.Models;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using TechnologyNews.Services.Interfaces;
using TechnologyNews.Services;
using TechnologyNews.Helpers;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace TechnologyNews
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //var key = new byte[32];
            //using (var generator = RandomNumberGenerator.Create())
            //    generator.GetBytes(key);
            //string apiSecret = Convert.ToBase64String(key);
            //configuration["AppSettings:Secret"] = apiSecret;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
#pragma warning disable CS0414 // The field 'Startup.MyAllowSpecificOrigins' is assigned but its value is never used
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
#pragma warning restore CS0414 // The field 'Startup.MyAllowSpecificOrigins' is assigned but its value is never used


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddScoped<IUrlHelper>(x =>
            //{
            //    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
            //    var factory = x.GetRequiredService<IUrlHelperFactory>();
            //    return factory.GetUrlHelper(actionContext);
            //});
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCors();
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(MyAllowSpecificOrigins,
            //    builder =>
            //    {
            //        builder.WithOrigins("https://localhost:44341").AllowCredentials().AllowAnyHeader().AllowAnyMethod();
            //    });
            //});

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddHttpClient("freegeoip", client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            //services.AddTransient<IUserValidator<User>, CustomUserValidator>();

            if (services.GetHerokuPostgreSQLConnectionString() != null)
            {
                TechnologyNewsDbContext.HerokuPostgreSqlConnectionString = services.GetHerokuPostgreSQLConnectionString();
                services.AddEntityFrameworkNpgsql().AddDbContext<TechnologyNewsDbContext>(opt =>
                    opt.UseNpgsql(services.GetHerokuPostgreSQLConnectionString()));
                //services.AddDbContext<TechnologyNewsDbContext>(options =>
                //    options.UseNpgsql(services.GetHerokuPostgreSQLConnectionString()));
            }
            else
            {
                services.AddDbContext<TechnologyNewsDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("TechnologyNewsContext")));
            }


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
                options.User.RequireUniqueEmail = false;
            })
                .AddEntityFrameworkStores<TechnologyNewsDbContext>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ICloudniaryService, CloudinaryService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AppSettings:Secret"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            //services.AddScoped<IUserService, UserService>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            TechnologyNewsDbContext dbcontext)
        {
            //for seeding database
            //ApplicationDbInitializer.SeedData(userManager, roleManager, appSettings);
            //using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //{
            //    scope.ServiceProvider.GetService<TechnologyNewsDbContext>().Database.Migrate();
            //}
            dbcontext.MigrateDatabse("Initial");
            //ApplicationDbInitializer.SeedData(userManager, roleManager, appSettings);
            //ApplicationDbInitializer.SeedOtherTables(app.ApplicationServices.GetRequiredService<IUnitOfWork>());
            

            //app.UseMiddleware(typeof(CorsMiddleware));

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

            //var returnObject = string.Format("{0}://{1}{2}", HttpContext.Request.Scheme, HttpContext.Request.Host, Url.Content("~"));
            var URL = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();
            //string Url = new System.Uri(configuration[WebHostDefaults.ServerUrlsKey]).ToString();
            //UnitOfWork.BaseUrlStatic = configuration[WebHostDefaults.ServerUrlsKey]; ;
            //UnitOfWork.BaseUrlStatic = Microsoft.AspNetCore.Mvc.UrlHelperExtensions.Action(,"Edit", "Project");
            //UnitOfWork.BaseUrlStatic = string.Format("{0}://{1}", httpContextAccessor.HttpContext.Request.Scheme, httpContextAccessor.HttpContext.Request.Host);
           
            //// global cors policy
            //app.UseCors(x => x
            //    //.AllowAnyOrigin()
            //    .WithOrigins("https://localhost:44341")
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .AllowCredentials());
            //app.UseCors(MyAllowSpecificOrigins);
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });


            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

        }
    }
}
