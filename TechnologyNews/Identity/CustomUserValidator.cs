using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Models;

namespace TechnologyNews.Identity
{
    public class CustomUserValidator : IUserValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (!user.Email.ToLower().Equals("admin@admin.com"))
            {
                errors.Add(new IdentityError
                {
                    Description = "something went rong"
                });
            }

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
