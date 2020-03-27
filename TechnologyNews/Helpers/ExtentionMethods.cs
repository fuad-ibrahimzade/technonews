using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnologyNews.Models;

namespace TechnologyNews.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<ApplicationUser> WithoutPasswords(this IEnumerable<ApplicationUser> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static ApplicationUser WithoutPassword(this ApplicationUser user)
        {
            //typeof(ApplicationUser).GetProperty
            if (user.GetType().GetProperty("Password") != null)
                user.GetType().GetProperty("Password").SetValue(user, null);
            if (user.GetType().GetProperty("PasswordHash") != null)
                //user.PasswordHash = null;
                user.GetType().GetProperty("PasswordHash").SetValue(user, null);
            return user;
        }

        public static IEnumerable Errors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return modelState.ToDictionary(kvp => kvp.Key,
                    kvp => kvp.Value.Errors
                                    .Select(e => e.ErrorMessage).ToArray())
                                    .Where(m => m.Value.Any());
            }
            return null;
        }

        public static string GetHerokuPostgreSQLConnectionString(this IServiceCollection services)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            //databaseUrl = "";
            if (String.IsNullOrEmpty(databaseUrl)) return null;

            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = Npgsql.SslMode.Prefer,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }

        public static IHost MigrateDatabase<T>(this IHost webHost) where T : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<T>();
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
            return webHost;
        }

        //public static byte[] GetSecret(this JwtBearerOptions option, IServiceCollection services, IConfiguration Configuration)
        //{
        //    var appSettingsSection = Configuration.GetSection("AppSettings");
        //    services.Configure<AppSettings>(appSettingsSection);

        //    // configure jwt authentication
        //    var appSettings = appSettingsSection.Get<AppSettings>();
        //    var key = Encoding.ASCII.GetBytes(appSettings.Secret);
        //    return key;
        //}
    }
}
