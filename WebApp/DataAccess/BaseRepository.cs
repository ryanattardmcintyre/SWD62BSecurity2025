using WebApp.Data;

namespace WebApp.DataAccess
{
    public class BaseRepository
    {
        public ApplicationDbContext _context { get; set; }
        public BaseRepository(ApplicationDbContext context) {
            _context = context;
        }

        
    }
}
