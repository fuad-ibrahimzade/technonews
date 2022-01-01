using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TechnologyNews.Data;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Helpers;
using TechnologyNews.Models;
using TechnologyNews.Services.Interfaces;
using TechnologyNews.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechnologyNews.Controllers
{
    //[Authorize]
    //[Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase, IDisposable
    {
        // GET: /<controller>/
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IOptions<AppSettings> appSettings;
        private readonly ICloudniaryService cloudniaryService;

        public AdminController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings,
            ICloudniaryService cloudniaryService)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.appSettings = appSettings;
            this.cloudniaryService = cloudniaryService;
        }

        [AllowAnonymous]
        [HttpPost("api/login")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] UserViewModel model)
        {
            //UserViewModel modelConverted = (UserViewModel)model;
            //headerim = HttpContext.Request.Headers.ToList(),
            //return Ok(new { objecim= model,Modelstatinvaliddimi= ModelState.IsValid,errorlarim= ModelState.Errors() });
            //ModelState.AddModelError("", "Invalid Modellll");
            if (!ModelState.IsValid)
            {
                //return BadRequest(new { message = "Username or password is incorrect" });
                return BadRequest(new { Errors = ModelState.Errors() });
                //return Json(new { Errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }
            var user = await unitOfWork.UserService.AuthenticateAsync(model.Email, model.Password);
            //ApplicationUser user = null;
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            //return Ok(new { message = "Already logged in" });

            var data = new { jwt = new { access_token = user.ApiToken, email = user.Email } };

            return Ok(data);
            //user = await unitOfWork.UserManager.FindByEmailAsync(model.Email);
            //if (user != null &&
            //    await unitOfWork.UserManager.CheckPasswordAsync(user, model.Password))
            //{
            //    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            //    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            //    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
            //        new ClaimsPrincipal(identity));
            //    return Ok(user);
            //    //return RedirectToAction(nameof(AdminController.Index), "Home");
            //}
            //else
            //{
            //    ModelState.AddModelError("", "Invalid UserName or Password");
            //    //return View();
            //    if (user == null) return Ok("user not found");
            //    return Ok("password is wrong");
            //}
        }

        [HttpPost("api/logout")]
        public IActionResult Logout([FromBody] Dictionary<string, string> model)
        {
            if (!ModelState.IsValid || !model.ContainsKey("email"))
            {
                //return BadRequest(new { message = "Username or password is incorrect" });
                return BadRequest(new { Errors = ModelState.Errors() });
                //return Json(new { Errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
            }
            //bool loggedout = await unitOfWork.UserService.LogoutAsync(model["email"]);
            var data = new
            {
                message = "Logged out after updating tokens"
            };
            //if (!loggedout)
            //    data.GetType().GetProperty("message").SetValue(data, "Can not logout");

            return Ok(new { jwt = data });
        }

        [HttpGet("api/dashboard")]
        public IActionResult Dashboard()
        {
            IEnumerable<ApplicationUser> users = unitOfWork.UserManager.Users.ToList();
            if (users.Count() == 0)
            {
                return Ok(new { message = "No users found." });
            }
            return Ok(users);
        }

        [HttpGet("api/list-pages")]
        public async Task<IActionResult> ListPagesAsync()
        {
            var pages = await unitOfWork.PageRepository.GetAllAsync();
            return Ok(new { jwt = new { message = "Pages listed", pages } });
        }
        [HttpGet("api/read-page")]
        public async Task<IActionResult> ReadPage([FromQuery] int id)
        {
            var page = await unitOfWork.PageRepository.GetByIDAsync(id);
            if (page == null)
            {
                return Ok(new { jwt = new { message = "Problem Page not read" } });
            }
            return Ok(new { jwt = new { message = "Page read", page } });
        }
        [HttpPost("api/create-page")]
        public async Task<IActionResult> CreatePage([FromBody] Page page)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Page not created", errors = ModelState.Errors() } });
            }
            unitOfWork.PageRepository.Insert(page);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "Page created", page } });
        }
        [HttpPost("api/update-page")]
        public async Task<IActionResult> UpdatePage([FromBody] Page page)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Page not updated", errors = ModelState.Errors() } });
            }
            unitOfWork.PageRepository.Update(page);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "Page updated", page } });
        }
        [HttpPost("api/delete-page")]
        public async Task<IActionResult> DeletePage([FromBody] Page page)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Page not deleted", errors = ModelState.Errors() } });
            }
            unitOfWork.PageRepository.Delete(page.id);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "Page deleted" } });
        }
        [HttpPost("api/upload-image")]
        public IActionResult UploadImage()
        {
            //    [FromBody]
            //object model
            object data = new { message = "Problem File not deleted from cloudinary" };
            //var fileName = Path.GetFileName(file.FileName);

            IFormFileCollection files = HttpContext.Request.Form.Files;
            IFormFile file = files.Where(file => file.Name == "file").FirstOrDefault();
            if (file.Length > 0)
            {
                using (var filetream = file.OpenReadStream())
                {
                    string url = cloudniaryService.UploadImage(file.FileName, filetream);
                    data = new { message = "File uploaded to cloudinary", src = url };
                }
            }
            //var dirs = new List<string>();
            //for (int i = 0; i < files.Count; i++)
            //{
            //    dirs.Add(files[i].FileName);
            //}
            ////[FromBody] object model,IFormFile file
            ////
            //var filePath = Path.GetTempFileName();
            //return Ok(new { fileName, dirs, filePath });

            //string url = "https://res.cloudinary.com/dfebwmqmq/image/upload/v1573550127/lmhzv9dpiq3ducxykow9.jpg";

            return Ok(new { jwt = data });
        }
        [HttpGet("api/delete-image")]
        public IActionResult DeleteImageAsync([FromBody] string src)
        {
            //    [FromBody]
            //object model
            //var urlBuilder =
            //    new UriBuilder()
            //    {
            //        Path = "/",
            //        Query = null,
            //        Fragment = null
            //    };
            //string url = urlBuilder.ToString();

            //return Ok(new { baseurl = unitOfWork.BaseUrl, url });

            string result = cloudniaryService.DeleteImage(src);
            return Ok(new { jwt = new { message = result } });
        }
        [HttpGet("api/list-posts")]
        public async Task<IActionResult> ListPostsAsync()
        {
            var posts = await unitOfWork.PostRepository.GetAllAsync();
            return Ok(new { jwt = new { message = "Posts listed", posts } });
        }
        [HttpGet("api/list-featured-posts")]
        public IActionResult ListFeaturedPostsAsync()
        {
            var posts = unitOfWork.PostRepository.GetRandomLimit(2);
            var data = new { message = "Featured Posts listed", posts };

            return Ok(new { jwt = data });
        }
        [HttpGet("api/read-post")]
        public async Task<IActionResult> ReadPostAsync([FromQuery] int id)
        {
            var post = await unitOfWork.PostRepository.GetByIDAsync(id);
            if (post == null)
            {
                return Ok(new { jwt = new { message = "Problem Post not read" } });
            }
            return Ok(new { jwt = new { message = "Post read", post } });
        }
        [HttpPost("api/create-post")]
        public async Task<IActionResult> CreatePostAsync([FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Post not created", errors = ModelState.Errors() } });
            }
            IFormFileCollection files = HttpContext.Request.Form.Files;
            IFormFile authorPicture = files.Where(file => file.Name == "author_picture").FirstOrDefault();
            IFormFile postPicture = files.Where(file => file.Name == "post_picture").FirstOrDefault();
            if (authorPicture.Length > 0)
            {
                using (var authorPictureStream = authorPicture.OpenReadStream())
                {
                    string url = cloudniaryService.UploadImage(authorPicture.FileName, authorPictureStream);
                    post.author_picture = url;
                }
            }
            if (postPicture.Length > 0)
            {
                using (var postPictureStream = postPicture.OpenReadStream())
                {
                    string url = cloudniaryService.UploadImage(postPicture.FileName, postPictureStream);
                    post.post_picture = url;
                }
            }

            unitOfWork.PostRepository.Insert(post);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "Post created", post } });
        }
        [HttpPost("api/update-post")]
        public async Task<IActionResult> UpdatePostAsync([FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Post not updated", errors = ModelState.Errors() } });
            }

            //var post = await unitOfWork.PostRepository.GetByIDAsync(id);

            IFormFileCollection files = HttpContext.Request.Form.Files;
            IFormFile authorPicture = files.Where(file => file.Name == "author_picture").FirstOrDefault();
            IFormFile postPicture = files.Where(file => file.Name == "post_picture").FirstOrDefault();
            if (authorPicture.Length > 0)
            {
                cloudniaryService.DeleteImage(post.author_picture);
                using (var authorPictureStream = authorPicture.OpenReadStream())
                {
                    string url = cloudniaryService.UploadImage(authorPicture.FileName, authorPictureStream);
                    post.author_picture = url;
                }
            }
            if (postPicture.Length > 0)
            {
                cloudniaryService.DeleteImage(post.post_picture);
                using (var postPictureStream = postPicture.OpenReadStream())
                {
                    string url = cloudniaryService.UploadImage(postPicture.FileName, postPictureStream);
                    post.post_picture = url;
                }
            }
            unitOfWork.PostRepository.Update(post);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "Post updated", post } });
        }
        [HttpPost("api/delete-post")]
        public async Task<IActionResult> DeletePostAsync([FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Post not deleted", errors = ModelState.Errors() } });
            }

            var deleteMessageC = cloudniaryService.DeleteImage(post.author_picture);
            var deleteMessageC2 = cloudniaryService.DeleteImage(post.post_picture);
            var message = "Post deleted , cloudinary delete1: " + deleteMessageC + " , cloudinary delete2: " + deleteMessageC2;

            unitOfWork.PostRepository.Delete(post.id);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message } });
        }
        [HttpGet("api/read-distinct-tags")]
        public async Task<IActionResult> ReadDistinctTagsAsync()
        {
            //[FromBody] object model
            var post = await unitOfWork.PostRepository.GetDistinctColumnAsync("post_tags");
            if (post == null)
            {
                return Ok(new { jwt = new { message = "Problem Tags not read" } });
            }
            return Ok(new { jwt = new { message = "Tags read", post } });
        }
        [HttpGet("api/list-comments")]
        public async Task<IActionResult> ListCommentsAsync([FromQuery] int post_id)
        {
            var comments = await unitOfWork.CommentRepository.GetAsync(filter: comment => comment.post_id == post_id);
            return Ok(new { jwt = new { message = "Comments listed", comments } });
        }
        [HttpGet("api/read-comment")]
        public async Task<IActionResult> ReadCommentAsync([FromQuery] int id)
        {
            var comment = await unitOfWork.CommentRepository.GetByIDAsync(id);
            if (comment == null)
            {
                return Ok(new { jwt = new { message = "Problem Comment not read" } });
            }
            return Ok(new { jwt = new { message = "Comment read", comment } });
        }
        [HttpPost("api/create-comment")]
        public async Task<IActionResult> CreateCommentAsync([FromBody] Comment comment_received)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Comment not created", errors = ModelState.Errors() } });
            }
            if (comment_received.parent_comment_id != null && comment_received.parent_comment_id != "-1")
            {
                var comment_created = comment_received;
                var parentComments = await unitOfWork.CommentRepository.GetAsync(filter: item => item.parent_comment_id == comment_received.parent_comment_id);
                var parentComment = parentComments.FirstOrDefault();
                var child_comment_ids = parentComment.child_comment_ids;
                child_comment_ids.Add(comment_created.id.ToString());
                parentComment.child_comment_ids = child_comment_ids;
                unitOfWork.CommentRepository.Update(parentComment);

                unitOfWork.CommentRepository.Insert(comment_received);
                await unitOfWork.SaveAsync();

                return Ok(new { jwt = new { message = "Comment created", comment_received, comment_created } });
            }
            return Ok(new { jwt = new { message = "Problem Comment not created", comment_received } });
        }
        [HttpPost("api/update-comment")]
        public async Task<IActionResult> UpdateCommentAsync([FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Comment not updated", errors = ModelState.Errors() } });
            }
            unitOfWork.CommentRepository.Update(comment);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "Comment updated", comment } });
        }
        [HttpPost("api/delete-comment")]
        public async Task<IActionResult> DeleteCommentAsync([FromBody] Comment comment)
        {
            var data = new { };
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Comment not deleted", errors = ModelState.Errors() } });
            }
            if (comment.parent_comment_id != null && comment.parent_comment_id != "-1")
            {
                var parentComments = await unitOfWork.CommentRepository.GetAsync(filter: item => item.parent_comment_id == comment.parent_comment_id);
                var parentComment = parentComments.FirstOrDefault();

                var child_comment_idsTemp = parentComment.child_comment_ids;
                child_comment_idsTemp.Remove(comment.id.ToString());
                parentComment.child_comment_ids = child_comment_idsTemp;

                unitOfWork.CommentRepository.Update(parentComment);
            }
            var child_comment_ids = comment.child_comment_ids;
            if (child_comment_ids.Count() > 0)
            {
                comment.message = "deleted message";
                unitOfWork.CommentRepository.Update(comment);
                await unitOfWork.SaveAsync();
                return Ok(new { jwt = new { message = "parent comment replaced" } });
            }
            unitOfWork.CommentRepository.Delete(comment.id);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "Comment deleted" } });
        }
        [HttpGet("api/update-about")]
        public async Task<IActionResult> UpdateAboutAsync()
        {
            var abouts = await unitOfWork.AboutRepository.GetAllAsync();
            var about = abouts.FirstOrDefault();
            if (about == null)
            {
                return Ok(new { jwt = new { message = "Problem About not read" } });
            }
            return Ok(new { jwt = new { message = "About read", editordata = about.content } });
        }
        [HttpPost("api/update-about")]
        public async Task<IActionResult> UpdateAboutPostAsync([FromBody] About about)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem About not updated", errors = ModelState.Errors() } });
            }
            unitOfWork.AboutRepository.Update(about);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "About updated", about } });
        }
        [HttpGet("api/update-contacts")]
        public async Task<IActionResult> UpdateContactsAsync()
        {
            var contacts = await unitOfWork.ContactRepository.GetAllAsync();
            var contact = contacts.FirstOrDefault();
            if (contact == null)
            {
                return Ok(new { jwt = new { message = "Problem Contact not read" } });
            }
            return Ok(new { jwt = new { message = "Contact read", editordata = contact.content } });
        }
        [HttpPost("api/update-contacts")]
        public async Task<IActionResult> UpdateContactsPostAsync([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { jwt = new { message = "Problem Contact not updated", errors = ModelState.Errors() } });
            }
            unitOfWork.ContactRepository.Update(contact);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message = "Contact updated", contact } });
        }
        [HttpGet("api/update-social-links")]
        public IActionResult UpdateSocialLinks()
        {
            var social_links = new
            {
                facebook = unitOfWork.SocialLinksRepository.GetAsync(filter: item => item.link_type == "facebook")
                    .GetAwaiter().GetResult().FirstOrDefault().content,
                twitter = unitOfWork.SocialLinksRepository.GetAsync(filter: item => item.link_type == "twitter")
                    .GetAwaiter().GetResult().FirstOrDefault().content,
                telegram = unitOfWork.SocialLinksRepository.GetAsync(filter: item => item.link_type == "telegram")
                    .GetAwaiter().GetResult().FirstOrDefault().content
            };

            return Ok(new { jwt = new { social_links } });
        }
        [HttpPost("api/update-social-links")]
        public async Task<IActionResult> UpdateSocialLinksPostAsync([FromBody] JObject requestData)
        {
            var socialLinks = requestData["social_links"];
            var facebook = (await unitOfWork.SocialLinksRepository.GetAsync(filter: item => item.link_type == "facebook")).FirstOrDefault();
            var twitter = (await unitOfWork.SocialLinksRepository.GetAsync(filter: item => item.link_type == "twitter")).FirstOrDefault();
            var telegram = (await unitOfWork.SocialLinksRepository.GetAsync(filter: item => item.link_type == "telegram")).FirstOrDefault();
            if (socialLinks["facebook"] != null)
            {
                facebook.content = socialLinks["facebook"].ToString();
                unitOfWork.SocialLinksRepository.Update(facebook);
            }
            if (socialLinks["twitter"] != null)
            {
                twitter.content = socialLinks["twitter"].ToString();
                unitOfWork.SocialLinksRepository.Update(twitter);
            }
            if (socialLinks["telegram"] != null)
            {
                telegram.content = socialLinks["telegram"].ToString();
                unitOfWork.SocialLinksRepository.Update(telegram);
            }
            await unitOfWork.SaveAsync();

            return Ok(new { jwt = new { message = "Social Links updated" } });
        }
        [HttpGet("api/update-advertisements")]
        public async Task<IActionResult> UpdateAdvertisementsAsync([FromQuery] int id)
        {
            string message = "Advertisements Retrieved";
            var ads = await unitOfWork.AdvertisementRepository.GetByIDAsync(id);
            return Ok(new { jwt = new { message, ads } });
        }
        [HttpPost("api/update-advertisements")]
        public async Task<IActionResult> UpdateAdvertisementsPostAsync([FromBody] Advertisement advertisement)
        {
            bool baseUrlIsLocalHost = unitOfWork.BaseUrl.Contains("localhost");
            string message = "Advertisements updated";
            var ads = advertisement;
            var adsOLD = await unitOfWork.AdvertisementRepository.GetByIDAsync(advertisement.id);
            IFormFileCollection files = HttpContext.Request.Form.Files;
            IFormFile ads_picture_1 = files.Where(file => file.Name == "ads_picture_1").FirstOrDefault();
            IFormFile ads_picture_2 = files.Where(file => file.Name == "ads_picture_2").FirstOrDefault();
            if (ads_picture_1.Length > 0)
            {
                if (!baseUrlIsLocalHost)
                {
                    cloudniaryService.DeleteImage(adsOLD.ads_picture_1);
                }
                using var stream = ads_picture_1.OpenReadStream();
                ads.ads_picture_1 = cloudniaryService.UploadImage(ads_picture_1.FileName, stream);
            }
            if (ads_picture_2.Length > 0)
            {
                if (!baseUrlIsLocalHost)
                {
                    cloudniaryService.DeleteImage(adsOLD.ads_picture_2);
                }
                using var stream = ads_picture_2.OpenReadStream();
                ads.ads_picture_2 = cloudniaryService.UploadImage(ads_picture_2.FileName, stream);
            }
            unitOfWork.AdvertisementRepository.Update(ads);
            await unitOfWork.SaveAsync();
            return Ok(new { jwt = new { message } });
        }
        [HttpGet("api/list-user-visits")]
        public async Task<IActionResult> ListAnalyticsUserVisitsAsync([FromQuery] string analytics_type)
        {
            string message = "User Visits listed";
            var analytics_data = analytics_type != null ?
                (await unitOfWork.CustomAnalyticsRepository.GetAsync(filter: item => item.analytics_type == analytics_type))
                :
                (await unitOfWork.CustomAnalyticsRepository.GetAllAsync());
            //var analyticsString = JObject.FromObject(analytics_data).ToString(Newtonsoft.Json.Formatting.None);
            //analyticsString = Newtonsoft.Json.JsonConvert.SerializeObject(analyticsString);
            //analyticsString = System.Text.RegularExpressions.Regex.Unescape(analyticsString);
            return Ok(new { jwt = new { message, analytics_data } });
        }
        [HttpPost("api/create-user-visit")]
        public async Task<IActionResult> CreateAnalyticsUserVisitAsync([FromBody] Dictionary<string, string> requestData)
        {
            string message = "Problem Custom Analytics not created";
            if (requestData["user_ip"] != null || requestData["request_uri"] != null)
            {
                JObject locationInfoJson = await unitOfWork.GetLocationAsync();
                if (locationInfoJson != null && locationInfoJson["country_name"] != null)
                {
                    var analytics_data = new
                    {
                        visited_page_link = requestData["request_uri"].ToString(),
                        user_ip = locationInfoJson["ip"].ToString(),
                        country = locationInfoJson["country_name"].ToString(),
                        http_referer = unitOfWork.httpContextAccessor.HttpContext.Request.GetTypedHeaders().Referer.ToString(),
                        ip_data = locationInfoJson.ToString()
                    };
                    var newCustomAnalytics = new CustomAnalytics()
                    {
                        analytics_type = "link_click",
                        analytics_data = JObject.FromObject(analytics_data).ToString()
                    };
                    unitOfWork.CustomAnalyticsRepository.Insert(newCustomAnalytics);
                    await unitOfWork.SaveAsync();
                    message = "Custom Analytics created";
                    var ca_user_visit = newCustomAnalytics;
                    return Ok(new { jwt = new { message, ca_user_visit } });
                }
            }
            return Ok(new { jwt = new { message } });
        }

        [HttpGet("api/create-defaults")]
        public IActionResult CreateDefaults()
        {
            //unitOfWork.MigrateDatabse("Initial");
            ApplicationDbInitializer.SeedData(userManager, roleManager, appSettings);
            bool succseeded = ApplicationDbInitializer.SeedOtherTables(unitOfWork);
            //ApplicationDbInitializer.SeedData(userManager, roleManager, unitOfWork, appSettings);
            //var data = new
            //{
            //    message = "exampel response",
            //    requestInfo = HttpContext.Request.Headers.ToList()
            //};
            var data = new { message = "error, already exists" };
            if (succseeded) data = new { message = "ok" };

            return Ok(new { jwt = data });
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    unitOfWork.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
