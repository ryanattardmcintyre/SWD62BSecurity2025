using WebApp.Data;
using WebApp.Models;

namespace WebApp.DataAccess
{
    public class ArticleRepository
    {
        ApplicationDbContext _context;
        public ArticleRepository(ApplicationDbContext context) {
            _context = context;
        }

        public void AddArticle(Article a)
        {
            _context .Articles.Add(a);
            _context.SaveChanges();
        }

        public IQueryable<Article> GetArticles()
        {
           return _context.Articles;
        }
    }
}
