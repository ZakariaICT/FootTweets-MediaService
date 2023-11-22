using MediaService.Model;

namespace MediaService.Repositories
{
    public interface IMediaRepo
    {
        bool saveChanges();

        IEnumerable<Pictures> GetAllPictures();
        Pictures GetPictureByID(int id);
        void CreatePicture(Pictures plat);
    }
}
