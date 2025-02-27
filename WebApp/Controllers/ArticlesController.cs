using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.DataAccess;
using WebApp.Models;

namespace WebApp.Controllers
{

    [Authorize]
    public class ArticlesController : Controller
    {
        ArtifactRepository _artifactRepository;
        ArticleRepository _articleRepository;
        IWebHostEnvironment _host;
        public ArticlesController(ArticleRepository articleRepository, 
            ArtifactRepository artifactRepository, 
            IWebHostEnvironment host)
        {   //constructor injection
            
            _articleRepository = articleRepository;
            _artifactRepository = artifactRepository;
            _host = host;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet] //this is going to be called to load the page with empty fields
        public ActionResult Create()
        { return View(); }


        [HttpPost] //this is going to handle the post request made by the user injecting
                   //possibly malicious files
        public ActionResult Create(Article article, List<IFormFile> files)
        {
            //1. validation
            //1.1 validation of file size
            foreach (var file in files)
            {
                if(file.Length > 1024)
                {
                    TempData["error"] = "One of the files is bigger than 1 MB";
                    return View();
                }
            }


            //1.2 validation of file header (checking the file type) (pdfs & jpgs)

            foreach (var file in files)
            {
                if (System.IO.Path.GetExtension(file.FileName) != ".jpg")
                {
                    TempData["error"] = "file is not a jpg";
                    return View();
                }
            }


            //FF = 255
            //D8 = 216

            int[] jpgFileHeaderValues = { 255, 216 };

            foreach (var file in files)
            {
                byte[] f = System.IO.File.ReadAllBytes(file.FileName);

                for (int d = 0; d < jpgFileHeaderValues.Length; d++)
                {
                    if (jpgFileHeaderValues[d] != f[d])
                    {
                        TempData["error"] = $"File {file.FileName} being uploaded is not a jpg";
                        return View();
                    }
                }
            }

            //Transaction starts

            //2. create article in db
            article.Id = Guid.NewGuid();
            article.CreatedAt = DateTime.Now;
            article.UpdatedAt = DateTime.Now;
            article.Author = User.Identity.Name; //email!! of the currently logged in user
            article.PublicAccess = false;
            _articleRepository.AddArticle(article); //saves into db


            //3. create artifactS in db while mapping article id onto the artifact

            string absolutePath = _host.ContentRootPath + "//Data/UserFiles";
            foreach(var file in files)
            {
                Artifact a = new Artifact();
                a.ArticleIdFK = article.Id;

                //3.1 save the file in the file storage
                string uniqueFilename = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
                a.FilePath = "\\Data\\UserFiles\\" + uniqueFilename;
                
                //save the file using the absolutePath.


                //3.2 saves into db
                _artifactRepository.AddArtifact(a); //saves into db
            }

         
 

            //Transaction aborted/ rolled back

            return View();
        
        
        }



    }
}
