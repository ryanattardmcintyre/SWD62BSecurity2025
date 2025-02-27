using WebApp.Data;
using WebApp.Models;

namespace WebApp.DataAccess
{
    public class ArtifactRepository
    {
        ApplicationDbContext _context;
        public ArtifactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddArtifact(Artifact artifact)
        { 
            _context.Artifacts.Add(artifact);
            _context.SaveChanges();
        }

        public IQueryable<Artifact> GetArtifacts(Guid articleId)
        {
           return  _context.Artifacts.Where(x => x.ArticleIdFK == articleId);

        }
    }
}
