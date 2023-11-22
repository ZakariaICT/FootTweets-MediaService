using MediaService.Data;
using MediaService.Model;

namespace MediaService.Repositories
{
    public class MediaRepo : IMediaRepo
    {
        private readonly AppDbContext _context;

        public MediaRepo(AppDbContext context)
        {
            _context = context;
        }
        public void CreatePicture(Pictures plat)
        {
            if (plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }

            _context.pictures.Add(plat);
        }

        public IEnumerable<Pictures> GetAllPictures()
        {
            return _context.pictures.ToList();

        }

        public Pictures GetPictureByID(int id)
        {
            return _context.pictures.FirstOrDefault(p => p.Id == id);
        }

        public bool saveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
