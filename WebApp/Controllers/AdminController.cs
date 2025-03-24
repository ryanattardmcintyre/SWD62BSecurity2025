using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.DataAccess;
using Microsoft.AspNetCore.Identity;
using WebApp.Models;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles ="Admin")] //actionfilter built-in
    public class AdminController : Controller
    {
        ArticleRepository _articleRepository;
        UserManager<IdentityUser> _userManager;
        public AdminController(ArticleRepository articleRepository, UserManager<IdentityUser> userManager)
        { 
            _userManager = userManager; 
            _articleRepository = articleRepository;
        }
        public IActionResult Index()
        {
            //here i'm returning only the needed info
            //1) to compress the response size as much as i can
            //2) to NOT compromise any unwanted but confidential data e.g. PasswordHashes, Mobile phones, ...

            var articles = _articleRepository.GetArticles().Select(x=>new Article() {Id=x.Id, Title=x.Title });
            List<string> users = _userManager.Users.Select(x=>x.Email).ToList();


            return View(new CreatePermissionViewModel() { Articles=articles.ToList(), Emails=users });
        }

        public IActionResult AssignPermission(List<string> selectedEmails, List<string> selectedArticles)
        {
            foreach (var email in selectedEmails)
            {
                foreach(var article in selectedArticles)
                {
                    _articleRepository.AddPermission(new Guid(article), email, true);
                }
            }
            TempData["success"] = "Permissions assigned";
            return RedirectToAction("Index");
        }

    }
}
