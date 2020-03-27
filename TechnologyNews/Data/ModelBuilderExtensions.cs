using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Models;

namespace TechnologyNews.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" },
                new IdentityRole { Name = "Guest", NormalizedName = "GUEST" }
            );
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    ApiToken = "some",
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    DateAdd = DateTime.Now,
                    DateUpd = DateTime.Now,
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "admin")
                }
            );
            modelBuilder.Entity<About>().HasData(
                new About
                {
                    id = 1,
                    date_add = DateTime.Now,
                    date_upd = DateTime.Now,
                    content = "< div _ngcontent - yis - c6 = \"\" class=\"section-row\" style =\"margin -bottom: 40px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;\" ><p _ngcontent-yis-c6=\"\" style =\"margin -bottom: 20px;\" > Lorem ipsum dolor sit amet, ea eos tibique expetendis, tollit viderer ne nam.No ponderum accommodare eam, purto nominavi cum ea, sit no dolores tractatos.Scripta principes quaerendum ex has, ea mei omnes eruditi. Nec ex nulla mandamus, quot omnesque mel et. Amet habemus ancillae id eum, justo dignissim mei ea, vix ei tantas aliquid. Cu laudem impetus conclusionemque nec, velit erant persius te mel.Ut eum verterem perpetua scribentur.</p><figure _ngcontent-yis-c6= \"\" class=\"figure -img\" style =\"margin -bottom: 20px;\" ><img _ngcontent-yis-c6=\"\" alt =\"\" class=\"img -responsive\" src =\"'.$baseUrl.'assets/img/about-1.jpg\" ></figure><p _ngcontent-yis-c6=\"\" style =\"margin -bottom: 20px;\" > Vix mollis admodum ei, vis legimus voluptatum ut, vis reprimique efficiendi sadipscing ut.Eam ex animal assueverit consectetuer, et nominati maluisset repudiare nec.Rebum aperiam vis ne, ex summo aliquando dissentiunt vim.Quo ut cibo docendi.Suscipit indoctum ne quo, ne solet offendit hendrerit nec.Case malorum evertitur ei vel.</p></div><div _ngcontent-yis-c6= \"\" class=\"row section-row\" style=\"margin-bottom: 40px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;\"><div _ngcontent-yis-c6=\"\" class=\"col-md-6\" style=\"float: left; width: 390px;\"><figure _ngcontent-yis-c6=\"\" class=\"figure-img\" style=\"margin-bottom: 20px;\"><img _ngcontent-yis-c6=\"\" alt=\"\" class=\"img-responsive\" src=\"'.$baseUrl.'assets/img/about-2.jpg\"></figure></div><div _ngcontent-yis-c6=\"\" class=\"col-md-6\" style=\"float: left; width: 390px;\"><h3 _ngcontent-yis-c6=\"\" style=\"font-family: &quot;Nunito Sans&quot;, sans-serif; font-weight: 700; color: rgb(33, 38, 49); margin: 0px 0px 15px; font-size: 23px;\">Our Mission</h3><p _ngcontent-yis-c6= \"\" style= \"margin-bottom: 20px;\" > Id usu mutat debet tempor, fugit omnesque posidonium nec ei.An assum labitur ocurreret qui, eam aliquid ornatus tibique ut.</p><ul _ngcontent-yis-c6= \"\" class=\"list-style\" style=\"margin-right: 0px; margin-left: 0px; padding: 0px 0px 0px 15px; list-style-position: initial; list-style-image: initial;\"><li _ngcontent-yis-c6=\"\"><p _ngcontent-yis-c6=\"\" style=\"margin-bottom: 20px;\">Vix mollis admodum ei, vis legimus voluptatum ut.</p></li><li _ngcontent-yis-c6=\"\"><p _ngcontent-yis-c6=\"\" style=\"margin-bottom: 20px;\">Cu cum alia vide malis.Vel aliquid facilis adolescens.</p></li><li _ngcontent-yis-c6= \"\" >< p _ngcontent-yis-c6= \"\" style= \"margin-bottom: 20px;\" > Laudem rationibus vim id.Te per illum ornatus.</p></li></ul></div></div>"
                }
            );


        }
    }
}
