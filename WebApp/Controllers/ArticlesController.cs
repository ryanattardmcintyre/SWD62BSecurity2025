using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using WebApp.ActionFilters;
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
        [ValidateAntiForgeryToken] //initiates the generation of a token on the server side which has to match
                                    // a token which is placed on the client side (inside the http form)
        public ActionResult Create(Article article, List<IFormFile> files)
        {

            IDbContextTransaction transaction = null; //this will be used to keep track of the db changes until i commit/abort
            try
            {
                //1. validation
                //1.1 validation of file size
                foreach (var file in files)
                {
                    if (file.Length > 1024 * 1024 * 1024)
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

                int[] jpgFileHeaderValues = { 255, 216 }; //jpg whitelist

                foreach (var file in files)
                {
                    byte[] f = new byte[2]; //here we are reading the first 2 bytes of the uploaded file
                    using (var s = file.OpenReadStream())
                    {
                        f[0] = (byte)s.ReadByte();
                        f[1] = (byte)s.ReadByte();

                    }

                    for (int d = 0; d < jpgFileHeaderValues.Length; d++) //comparing whitelist with actual uploaded bytes
                    {
                        if (jpgFileHeaderValues[d] != f[d])
                        {
                            TempData["error"] = $"File {file.FileName} being uploaded is not a jpg";
                            return View();
                        }
                    }
                }

                //Transaction starts

                //start a transaction
                transaction = _articleRepository._context.Database.BeginTransaction();

                //2. create article in db
                article.Id = Guid.NewGuid();
                article.CreatedAt = DateTime.Now;
                article.UpdatedAt = DateTime.Now;
                article.Author = User.Identity.Name; //email!! of the currently logged in user
                article.PublicAccess = false;
                _articleRepository.AddArticle(article); //<<<<<<<<<<<<<<<<<<<<<<   saves into db

               
                //3. create artifactS in db while mapping article id onto the artifact

                string absolutePath = _host.ContentRootPath + "//Data/UserFiles";
                foreach (var file in files)
                {
                    Artifact a = new Artifact();
                    a.ArticleIdFK = article.Id;

                    //3.1 save the file in the file storage
                    string uniqueFilename = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
                    a.FilePath = "\\Data\\UserFiles\\" + uniqueFilename;

                    //save the file using the absolutePath.

                    MemoryStream msIn = new MemoryStream();
                    file.CopyTo(msIn);
                    msIn.Position = 0;
                    System.IO.File.WriteAllBytes(absolutePath + "//" + uniqueFilename, msIn.ToArray());

                    //3.2 saves into db
                    a.Digest = ""; //temp solution

                    _artifactRepository.AddArtifact(a); //<<<<<<<<<<<<<<<<<<<<<<   saves into db

                }

                //stop the transaction
                transaction.Commit(); //signalling to the database that data can be changed to permanent mode
                TempData["success"] = "Article together with the files uploaded successfully";

               
            }
            catch (Exception ex)
            {
                TempData["error"] = "Operation failed, we are working on it. try again later";
                //abort the transaction;
                if (transaction != null) transaction.Rollback();
            }

            return View();
        }


        //this will download the file contained in the article
        public IActionResult Download (int id)
        {
            //var artifact = _artifactRepository.GetArtifacts()
            return View();
        }

        //This will open the Article to be viewed by the user
        [ArticleActionFilter]
        public IActionResult Details(Guid id)
        {
            var article = _articleRepository.GetArticles().SingleOrDefault(x => x.Id == id);
            return View(article);
        }


    }
}
