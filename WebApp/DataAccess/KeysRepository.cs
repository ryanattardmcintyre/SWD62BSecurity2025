using WebApp.Data;
using WebApp.Models;

namespace WebApp.DataAccess
{
    public class KeysRepository: BaseRepository
    {
        public KeysRepository(ApplicationDbContext context) : base(context)
        {

        }

        public void AddKeysToDatabase(string username, DBAsymmetricKeys keys)
        {
            _context.DBAsymmetricKeys.Add(keys);
            _context.SaveChanges();
        }
    }
}
