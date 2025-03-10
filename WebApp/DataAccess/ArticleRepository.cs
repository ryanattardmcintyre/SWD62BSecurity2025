using WebApp.Data;
using WebApp.Models;

namespace WebApp.DataAccess
{
    public class ArticleRepository: BaseRepository
    {
  
        public ArticleRepository(ApplicationDbContext context) : base(context) {
             
        }

        public void AddArticle(Article a)
        {
            base._context .Articles.Add(a);
            base._context.SaveChanges();
        }

        public IQueryable<Article> GetArticles()
        {
           return base._context.Articles;
        }


        public IQueryable<Permission> GetPermissions(Guid id)
        {
            return _context.Permissions.Where(x=>x.ArticleFk == id);
        }

        public void AddPermission(Guid articleId, string userEmail, bool accessGranted = false )
        {
            _context.Permissions.Add(new Permission()
            { 
                 ArticleFk = articleId,
                  HasAccess = accessGranted,
                   User = userEmail
            });

            _context.SaveChanges();
        }

        public void DeletePermission(Guid articleId, string userEmail)
        {
            var permission = _context.Permissions.SingleOrDefault(x=>x.ArticleFk==articleId && x.User==userEmail);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
            }

            _context.SaveChanges();
        }

        public void UpdatePermission(Guid articleId, string userEmail, bool accessGranted=false)
        {
            var permission = _context.Permissions.SingleOrDefault(x => x.ArticleFk == articleId && x.User == userEmail);
            if (permission != null)
            {
                 permission.HasAccess = accessGranted;
            }

            _context.SaveChanges();
        }
    }
}
