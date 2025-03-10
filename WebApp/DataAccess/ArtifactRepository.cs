using WebApp.Data;
using WebApp.Models;

namespace WebApp.DataAccess
{
    public class ArtifactRepository: BaseRepository
    {
       
        public ArtifactRepository(ApplicationDbContext context) : base(context)
        {
          
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
