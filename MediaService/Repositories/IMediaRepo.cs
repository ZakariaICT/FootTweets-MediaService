using MediaService.Model;

namespace MediaService.Repositories
{
    public interface IMediaRepo
    {
        bool saveChanges();

        void DeletePicturesByUserId(string UidAuth);

        IEnumerable<Pictures> GetAllPictures();
        Pictures GetPictureByID(int id);
        void CreatePicture(Pictures plat);
    }
}
