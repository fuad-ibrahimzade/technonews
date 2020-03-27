using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnologyNews.Data.Interfaces;
using TechnologyNews.Helpers;
using TechnologyNews.Models;
using TechnologyNews.Models.Interfaces;
using TechnologyNews.Data.Repositories;
using TechnologyNews.Data.Repositories.Interfaces;
using TechnologyNews.Services;
using TechnologyNews.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace TechnologyNews.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool _disposed = false;
        public TechnologyNewsDbContext _context { get; set; }
        public string BaseUrl { get; set; }
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }
        public IOptions<AppSettings> appSettings { get; set; }
        private UserService userService;
        public UserService UserService
        { 
            get 
            {
                if (this.userService == null)
                {
                    this.userService = new UserService(appSettings, this);
                }
                return userService;
            } 
        }

        private AboutRepository aboutRepository;
        public AboutRepository AboutRepository
        {
            get
            {
                if (this.aboutRepository == null)
                {
                    this.aboutRepository = new AboutRepository(_context);
                }
                return aboutRepository;
            }
        }
        private GenericRepository<Page> pageRepository;
        public GenericRepository<Page> PageRepository
        {
            get
            {
                if (this.pageRepository == null)
                {
                    this.pageRepository = new GenericRepository<Page>(_context);
                }
                return pageRepository;
            }
        }
        private GenericRepository<Post> postRepository;
        public GenericRepository<Post> PostRepository
        {
            get
            {
                if (this.postRepository == null)
                {
                    this.postRepository = new GenericRepository<Post>(_context);
                }
                return postRepository;
            }
        }
        private GenericRepository<Comment> commentRepository;
        public GenericRepository<Comment> CommentRepository
        {
            get
            {
                if (this.commentRepository == null)
                {
                    this.commentRepository = new GenericRepository<Comment>(_context);
                }
                return commentRepository;
            }
        }
        private GenericRepository<Contact> contactRepository;
        public GenericRepository<Contact> ContactRepository
        {
            get
            {
                if (this.contactRepository == null)
                {
                    this.contactRepository = new GenericRepository<Contact>(_context);
                }
                return contactRepository;
            }
        }
        private GenericRepository<Advertisement> advertisementRepository;
        public GenericRepository<Advertisement> AdvertisementRepository
        {
            get
            {
                if (this.advertisementRepository == null)
                {
                    this.advertisementRepository = new GenericRepository<Advertisement>(_context);
                }
                return advertisementRepository;
            }
        }
        private GenericRepository<CustomAnalytics> customAnalyticsRepository;
        public GenericRepository<CustomAnalytics> CustomAnalyticsRepository
        {
            get
            {
                if (this.customAnalyticsRepository == null)
                {
                    this.customAnalyticsRepository = new GenericRepository<CustomAnalytics>(_context);
                }
                return customAnalyticsRepository;
            }
        }
        private GenericRepository<SocialLinks> socialLinksRepository;
        public GenericRepository<SocialLinks> SocialLinksRepository
        {
            get
            {
                if (this.socialLinksRepository == null)
                {
                    this.socialLinksRepository = new GenericRepository<SocialLinks>(_context);
                }
                return socialLinksRepository;
            }
        }

        public  IHttpContextAccessor httpContextAccessor { get; }

        private IHttpClientFactory clientFactory { get; }

        //private IUrlHelper urlHelper;

        public UnitOfWork(TechnologyNewsDbContext context, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory clientFactory)
        {
            _context = context;
            UserManager = userManager;
            SignInManager = signInManager;
            this.appSettings = appSettings;
            this.httpContextAccessor = httpContextAccessor;
            this.clientFactory = clientFactory;
            //this.urlHelper = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelper>();
            //UrlHelperExtensions.Action("Index", "AdminController");
            this.BaseUrl = string.Format("{0}://{1}/", httpContextAccessor.HttpContext.Request.Scheme, httpContextAccessor.HttpContext.Request.Host);
            //BaseUrlStatic = httpContextAccessor.ToString();
            //this.BaseUrl = env.WebRootPath;
            //this.BaseUrl = UrlHelperExtensions.Action(urlHelper,"Dashboard", "AdminController");
        }

        public void DeleteUsers()
        {
            UserManager.Users.ToList().ForEach(async (user) =>
            {
                var logins = await UserManager.GetLoginsAsync(user);
                var rolesForUser = await UserManager.GetRolesAsync(user);

                using (var transaction = _context.Database.BeginTransaction())
                {
                    IdentityResult result = IdentityResult.Success;
                    foreach (var login in logins)
                    {
                        result = await UserManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                        if (result != IdentityResult.Success)
                            break;
                    }
                    if (result == IdentityResult.Success)
                    {
                        foreach (var item in rolesForUser)
                        {
                            result = await UserManager.RemoveFromRoleAsync(user, item);
                            if (result != IdentityResult.Success)
                                break;
                        }
                    }
                    if (result == IdentityResult.Success)
                    {
                        result = await UserManager.DeleteAsync(user);
                        if (result == IdentityResult.Success)
                            transaction.Commit(); //only commit if user and all his logins/roles have been deleted  
                    }
                }
            });


        }

        public bool CreateDefaults()
        {
            bool aboutIsEmpty = AboutRepository.IsEmpty().GetAwaiter().GetResult();
            if (aboutIsEmpty)
            {
                try
                {
                    var newAbout = new About
                    {
                        date_add = DateTime.Now,
                        date_upd = DateTime.Now,
                        content = $@"<div class='section-row' style ='margin -bottom: 40px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;' ><p style ='margin -bottom: 20px;' > Lorem ipsum dolor sit amet, ea eos tibique expetendis, tollit viderer ne nam.No ponderum accommodare eam, purto nominavi cum ea, sit no dolores tractatos.Scripta principes quaerendum ex has, ea mei omnes eruditi. Nec ex nulla mandamus, quot omnesque mel et. Amet habemus ancillae id eum, justo dignissim mei ea, vix ei tantas aliquid. Cu laudem impetus conclusionemque nec, velit erant persius te mel.Ut eum verterem perpetua scribentur.</p><figure class='figure -img' style ='margin -bottom: 20px;' ><img alt ='' class='img-responsive' src ='{this.BaseUrl}/assets/img/about-1.jpg' ></figure><p style ='margin -bottom: 20px;' > Vix mollis admodum ei, vis legimus voluptatum ut, vis reprimique efficiendi sadipscing ut.Eam ex animal assueverit consectetuer, et nominati maluisset repudiare nec.Rebum aperiam vis ne, ex summo aliquando dissentiunt vim.Quo ut cibo docendi.Suscipit indoctum ne quo, ne solet offendit hendrerit nec.Case malorum evertitur ei vel.</p></div><div class='row section-row' style='margin-bottom: 40px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;'><div class='col-md-6' style='float: left; width: 390px;'><figure class='figure-img' style='margin-bottom: 20px;'><img alt='' class='img-responsive' src='{this.BaseUrl}/assets/img/about-2.jpg'></figure></div><div class='col-md-6' style='float: left; width: 390px;'><h3 style='font-family: &quot;Nunito Sans&quot;, sans-serif; font-weight: 700; color: rgb(33, 38, 49); margin: 0px 0px 15px; font-size: 23px;'>Our Mission</h3><p style= 'margin-bottom: 20px;' > Id usu mutat debet tempor, fugit omnesque posidonium nec ei.An assum labitur ocurreret qui, eam aliquid ornatus tibique ut.</p><ul class='list-style' style='margin-right: 0px; margin-left: 0px; padding: 0px 0px 0px 15px; list-style-position: initial; list-style-image: initial;'><li><p style='margin-bottom: 20px;'>Vix mollis admodum ei, vis legimus voluptatum ut.</p></li><li><p style='margin-bottom: 20px;'>Cu cum alia vide malis.Vel aliquid facilis adolescens.</p></li><li><p style= 'margin-bottom: 20px;' > Laudem rationibus vim id.Te per illum ornatus.</p></li></ul></div></div>"
                    };
                    AboutRepository.Insert(newAbout);
                    SaveAsync().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }

            }
            bool pageIsEmpty = PageRepository.IsEmpty().GetAwaiter().GetResult();
            if (pageIsEmpty)
            {
                try
                {
                    string[] pagesTitles = new string[] { "News", "Popular", "Web Design", "JavaScript", "Css", "Jquery" };
                    for (int i = 0; i < pagesTitles.Length; i++)
                    {
                        Page newPage = new Page
                        {
                            date_add = DateTime.Now,
                            date_upd = DateTime.Now,
                            category_color = this.RandomColor(),
                            name = pagesTitles[i],
                            posts_count = 0,
                            show_at_home = true,
                            showing_order = i + 1
                        };
                        PageRepository.Insert(newPage);
                    }
                    SaveAsync().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }

            }
            bool postsIsEmpty = PostRepository.IsEmpty().GetAwaiter().GetResult();
            if (postsIsEmpty)
            {
                try
                {
                    string[] pages = new string[] { "News", "Popular", "Web Design", "JavaScript", "Css", "Jquery" };
                    //string postTitle = "Ask HN: Does Anybody Still Use JQuery?";
                    string[] postTitles = new string[] {
                    "Tell-A-Tool: Guide To Web Design And Development Tools",
                    "Why Node.js Is The Coolest Kid On The Backend Development Block!",
                    "Pagedraw UI Builder Turns Your Website Design Mockup Into Code Automatically",
                    "Chrome Extension Protects Against JavaScript-Based CPU Side-Channel Attacks",
                    "CSS Float: A Tutorial",
                    "Ask HN: Does Anybody Still Use JQuery?"
                };
                    string postDescription = @"
                    Do you like Cheese Whiz? Spray tan? Fake eyelashes? That's what is Lorem Ipsum to many—it rubs them the wrong way, all the way. It's unreal, uncanny
                ";
                    string[][] postTags = new string[][] { new string[]{"Chrome", "CSS", "Tutorial"}, new string[]{"Backend", "JQuery", "Design"},
                    new string[]{"Development", "JavaScript", "Website" }, new string[]{ "CSS", "Tutorial", "Backend" },
                    new string[]{ "Design", "Development", "JavaScript" }, new string[]{ "Chrome", "JQuery", "Website" } };



                    string[] postPictures = new string[6];
                    for (int i = 0; i < 6; i++)
                    {
                        // frontend/dist/frontend/
                        postPictures[i] = this.BaseUrl + "/assets/img/post-" + (i + 1) + ".jpg";
                    }

                    string defaultContent = ($@"
                    <h3 style = 'font-family: &quot;Nunito Sans&quot;, sans-serif; font-weight: 700; color: rgb(33, 38, 49); margin: 0px 0px 15px; font-size: 23px;' > Lorem Ipsum: when, and when not to use it</ h3 ><p style = 'margin-bottom: 20px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;' > Do you like Cheese Whiz? Spray tan? Fake eyelashes? That's what is Lorem Ipsum to many—it rubs them the wrong way, all the way. It's unreal, uncanny, makes you wonder if something is wrong, it seems to seek your attention for all the wrong reasons. Usually, we prefer the real thing, wine without sulfur based preservatives, real butter, not margarine, and so we'd like our layouts and designs to be filled with real words, with thoughts that count, information that has value.</p><p style='margin-bottom: 20px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;'>The toppings you may chose for that TV dinner pizza slice when you forgot to shop for foods, the paint you may slap on your face to impress the new boss is your business. But what about your daily bread? Design comps, layouts, wireframes—will your clients accept that you go about things the facile way? Authorities in our business will tell in no uncertain terms that Lorem Ipsum is that huge, huge no no to forswear forever. Not so fast, I'd say, there are some redeeming factors in favor of greeking text, as its use is merely the symptom of a worse problem to take into consideration.</p><figure class='figure-img' style='margin-bottom: 20px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;'><img alt='' class='img-responsive' src='{this.BaseUrl}/assets/img/post-4.jpg'><figcaption style='padding-top: 5px; font-size: 13px; font-weight: 600;'>So Lorem Ipsum is bad (not necessarily)</figcaption></figure><p style='margin-bottom: 20px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;'>You begin with a text, you sculpt information, you chisel away what's not needed, you come to the point, make things clear, add value, you're a content person, you like words. Design is no afterthought, far from it, but it comes in a deserved second. Anyway, you still use Lorem Ipsum and rightly so, as it will always have a place in the web workers toolbox, as things happen, not always the way you like it, not always in the preferred order. Even if your less into design and more into content strategy you may find some redeeming value with, wait for it, dummy copy, no less.</p><p style='margin-bottom: 20px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;'>There's lot of hate out there for a text that amounts to little more than garbled words in an old language. The villagers are out there with a vengeance to get that Frankenstein, wielding torches and pitchforks, wanting to tar and feather it at the least, running it out of town in shame.</p><p style='margin-bottom: 20px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;'>One of the villagers, Kristina Halvorson from Adaptive Path, holds steadfastly to the notion that design can’t be tested without real content:</p><blockquote class='blockquote' style='padding-top: 20px; padding-bottom: 20px; margin-bottom: 10px; border-left: 0px; position: relative; font-weight: 600; color: rgb(61, 69, 92); font-family: Nunito, sans-serif;'>I’ve heard the argument that “lorem ipsum” is effective in wireframing or design because it helps people focus on the actual layout, or color scheme, or whatever. What kills me here is that we’re talking about creating a user experience that will (whether we like it or not) be DRIVEN by words. The entire structure of the page or app flow is FOR THE WORDS.</blockquote><p style='margin-bottom: 20px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;'>If that's what you think how bout the other way around? How can you evaluate content without design? No typography, no colors, no layout, no styles, all those things that convey the important signals that go beyond the mere textual, hierarchies of information, weight, emphasis, oblique stresses, priorities, all those subtle cues that also have visual and emotional appeal to the reader. Rigid proponents of content strategy may shun the use of dummy copy but then designers might want to ask them to provide style sheets with the copy decks they supply that are in tune with the design direction they require.</p><h3 style='font-family: &quot;Nunito Sans&quot;, sans-serif; font-weight: 700; color: rgb(33, 38, 49); margin: 0px 0px 15px; font-size: 23px;'>Summing up, if the copy is diverting attention from the design it’s because it’s not up to task.</h3><p style='margin-bottom: 20px; color: rgb(61, 69, 92); font-family: Nunito, sans-serif; font-size: 16px;'>Typographers of yore didn\'t come up with the concept of dummy copy because people thought that content is inconsequential window dressing, only there to be used by designers who can’t be bothered to read. Lorem Ipsum is needed because words matter, a lot. Just fill up a page with draft copy about the client’s business and they will actually read it and comment on it. They will be drawn to it, fiercely. Do it the wrong way and draft copy can derail your design review.</p>                
                ");
                    // frontend/dist/frontend/
                    var author = new
                    {
                        author_name = "John Doe",
                        author_picture = $"{this.BaseUrl}/assets/img/author.png",
                        author_desc = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."
                    };
                    var authorSocialLinks = new { facebook = "facebook", twitter = "twitter", instagram = "instagram" };
                    for (int index = 0; index < 8; index++)
                    {
                        for (int i = 0; i < pages.Length; i++)
                        {
                            List<Page> pageList = ((List<Page>)PageRepository.GetAsync(e => e.name == pages[i]).GetAwaiter().GetResult());
                            Page page = pageList[0];
                            page.posts_count += 1;
                            PageRepository.Update(page);
                            //await SaveAsync();

                            Post newPost = new Post
                            {
                                post_category = postTitles[i],
                                post_title = postTitles[i],
                                post_tags = postTags[i].ToString(),
                                post_picture = postPictures[i],
                                content = defaultContent,
                                post_desc = postDescription,
                                author_name = author.author_name,
                                author_desc = author.author_desc,
                                author_picture = author.author_picture,
                                author_social_links = JObject.FromObject(authorSocialLinks).ToString()
                            };
                            PostRepository.Insert(newPost);
                            //await SaveAsync();
                        }
                    }
                    //await SaveAsync();
                    SaveAsync().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }

            }
            bool commentsIsEmpty = CommentRepository.IsEmpty().GetAwaiter().GetResult();
            if (commentsIsEmpty)
            {
                try
                {
                    string comment_message = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
                    var posts = (List<Post>)PostRepository.GetAllAsync().GetAwaiter().GetResult();
                    for (int i = 0; i < posts.Count(); i++)
                    {
                        Comment newComment = new Comment
                        {
                            name = "John Doe",
                            email = "some_email@email.com",
                            message = comment_message,
                            post_id = posts[i].id,
                            parent_comment_id = null,
                            child_comment_ids = new List<string>()
                        };

                        CommentRepository.Insert(newComment);
                        SaveAsync().GetAwaiter().GetResult();

                        Comment childComment = new Comment
                        {
                            name = "John Doe",
                            email = "some_email@email.com",
                            message = comment_message,
                            post_id = posts[i].id,
                            parent_comment_id = newComment.id.ToString(),
                            child_comment_ids = new List<string>()
                        };

                        CommentRepository.Insert(childComment);
                        SaveAsync().GetAwaiter().GetResult();

                        newComment.child_comment_ids.Add(childComment.id.ToString());
                        CommentRepository.Update(newComment);
                        SaveAsync().GetAwaiter().GetResult();

                        Comment newComment2 = new Comment
                        {
                            name = "John Doe",
                            email = "some_email@email.com",
                            message = comment_message,
                            post_id = posts[i].id,
                            parent_comment_id = null,
                            child_comment_ids = new List<string>()
                        };

                        CommentRepository.Insert(newComment2);
                        SaveAsync().GetAwaiter().GetResult();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }

            bool advertisementIsEmpty = AdvertisementRepository.IsEmpty().GetAwaiter().GetResult();
            if (advertisementIsEmpty)
            {
                try
                {
                    var ads1 = new { name = "ad1", picture_url = this.BaseUrl + "/assets/img/ad-1.jpg" };
                    var ads2 = new { name = "ad2", picture_url = this.BaseUrl + "/assets/img/ad-2.jpg" };

                    var newAdvertisement = new Advertisement()
                    {
                        ads_name_1 = ads1.name,
                        ads_name_2 = ads2.name,
                        ads_picture_1 = ads1.picture_url,
                        ads_picture_2 = ads2.picture_url
                    };

                    AdvertisementRepository.Insert(newAdvertisement);
                    SaveAsync().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }

                //$baseUrl = 'https://'.$_SERVER['SERVER_NAME'].'/';
                //$adds =[];
                //for ($i = 0; $i < 2 ; $i++) {
                //    $adds[]=['name'=>'ad'.($i + 1), 'picture_url'=>$baseUrl.'assets/img/ad-'.($i + 1).'.jpg'];
                //    // evvel buda elave prefix idi: frontend/dist/frontend/
                //}

            }

            bool contactsIsEmpty = ContactRepository.IsEmpty().GetAwaiter().GetResult();
            if (contactsIsEmpty)
            {
                try
                {
                    var newContact = new Contact()
                    {
                        content = @"
                    <h3>Contact Information</h3><p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</ p ><ul class='list-style'><li><p><strong>Email:</strong><a href='#'>Webmag @email.com</a></p></li><li><p><strong> Phone:</strong> 213-520-7376</p></li><li><p><strong> Address:</strong> 3770 Oliver Street</p></li></ul>
                    "
                    };

                    ContactRepository.Insert(newContact);
                    SaveAsync().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }

            bool socialLinksIsEmpty = SocialLinksRepository.IsEmpty().GetAwaiter().GetResult();
            if (socialLinksIsEmpty)
            {
                try
                {
                    var social_links = new string[] { "facebook", "twitter", "telegram" };
                    for (int i = 0; i < social_links.Length; i++)
                    {
                        var newSocialLink = new SocialLinks()
                        {
                            link_type = social_links[i],
                            content = "https://default-profile"
                        };
                        SocialLinksRepository.Insert(newSocialLink);
                    }
                    SaveAsync().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }

            bool customAnalyticsIsEmpty = CustomAnalyticsRepository.IsEmpty().GetAwaiter().GetResult();
            if (customAnalyticsIsEmpty)
            {
                try
                {
                    string user_ip = ClientIpAddress();
                    var locationInfoJson = GetLocationAsync().GetAwaiter().GetResult();
                    if (locationInfoJson != null && locationInfoJson["country_name"] != null)
                    {
                        var analytics_data = new
                        {
                            visited_page_link = UriHelper.GetDisplayUrl(httpContextAccessor.HttpContext.Request),
                            user_ip = locationInfoJson["ip"].ToString(),
                            country = locationInfoJson["country_name"].ToString(),
                            http_referer = httpContextAccessor.HttpContext.Request.GetTypedHeaders().Referer.ToString(),
                            ip_data = locationInfoJson
                        };

                        var newCustomAnalytics = new CustomAnalytics()
                        {
                            analytics_type = "link_click",
                            analytics_data = JObject.FromObject(analytics_data).ToString()
                        };

                        CustomAnalyticsRepository.Insert(newCustomAnalytics);
                        SaveAsync().GetAwaiter().GetResult();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }

            if (advertisementIsEmpty || contactsIsEmpty || socialLinksIsEmpty || customAnalyticsIsEmpty || commentsIsEmpty || postsIsEmpty || pageIsEmpty || aboutIsEmpty)
                return true;
            else
                return false;
            //bool userIsEmpty = UserManager.Users.ToArray().Length == 0;
            //if (userIsEmpty)
            //{
            //    var user = new ApplicationUser { 
            //        ApiToken = "some",
            //        UserName = "admin@admin.com",
            //        Email = "admin@admin.com",
            //        EmailConfirmed = true,
            //        DateAdd = DateTime.Now,
            //        DateUpd = DateTime.Now
            //    };
            //    await UserManager.CreateAsync(user, "admin");
            //    //await SaveAsync();
            //    //new SignInManager<User>().SignInAsync
            //    //UserManager.Users.ToList().ForEach( async(user)=> {
            //    //    await _userManager.DeleteAsync(user);
            //    //});
            //}
        }

        public string RandomColor()
        {
            var random = new Random();
            var color = String.Format("#{0:X6}", random.Next(0x1000000));
            return color;
        }

        public string ClientIpAddress()
        {
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public async Task<JObject> GetLocationAsync()
        {
            JObject decodedLocation = null;
            //string  location = "";
            var client = clientFactory.CreateClient("freegeoip");
            var response = await client.GetAsync("https://freegeoip.app/json/");
            //"https://freegeoip.app/json/"
            //client.BaseAddress = new Uri("https://www.metaweather.com/");
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                decodedLocation = JObject.Parse(responseString);
            }

            //if (location.Contains("cURL Error #:") == false)
            //{
            //    decodedLocation = location;
            //}
            //else
            //{
            //    decodedLocation = "error";
            //}

            return decodedLocation;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void MigrateDatabse(string targetMigration)
        {
            _context.GetInfrastructure().GetService<IMigrator>().Migrate(targetMigration);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
