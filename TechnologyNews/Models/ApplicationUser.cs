using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Models.Interfaces;

namespace TechnologyNews.Models
{
    public class ApplicationUser: IdentityUser
    {
        //public int UserId { get; set; }
        public DateTime DateAdd { get; set; }
        public DateTime DateUpd { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }
        public string ApiToken { get; set; }
    }
}
