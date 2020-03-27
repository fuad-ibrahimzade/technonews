using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Helpers;
using TechnologyNews.Models;

namespace TechnologyNews.Data
{
    public static class ApplicationDbInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appsettings)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager, appsettings);
            //SeedOtherTables(unitOfWork);
            //unitOfWork.SaveAsync();
        }
        public static void SeedUsers(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appsettings)
        {
            //try
            //{

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("{0} Exception caught.", e);
            //}
            try
            {
                if (userManager.FindByEmailAsync(appsettings.Value.AdminEmail).Result == null)
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        ApiToken = "some",
                        UserName = appsettings.Value.AdminEmail,
                        Email = appsettings.Value.AdminEmail,
                        EmailConfirmed = true,
                        DateAdd = DateTime.Now,
                        DateUpd = DateTime.Now
                    };

                    IdentityResult result = userManager.CreateAsync(user, appsettings.Value.AdminPassword).Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

        }
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            try
            {
                if (!roleManager.RoleExistsAsync("Adnin").Result)
                {
                    IdentityResult result = roleManager.CreateAsync(new IdentityRole
                    {
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    }).Result;
                    result = roleManager.CreateAsync(new IdentityRole
                    {
                        Name = "User",
                        NormalizedName = "USER"
                    }).Result;
                    result = roleManager.CreateAsync(new IdentityRole
                    {
                        Name = "Guest",
                        NormalizedName = "GUEST"
                    }).Result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

        }

        public static bool SeedOtherTables(IUnitOfWork unitOfWork)
        {
            return unitOfWork.CreateDefaults();
        }
    }
}
