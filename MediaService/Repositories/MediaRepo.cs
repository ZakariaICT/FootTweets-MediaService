﻿using MediaService.Data;
using MediaService.Model;

namespace MediaService.Repositories
{
    public class MediaRepo : IMediaRepo
    {
        private readonly AppDbContext _context;
        private List<Pictures> pictureItems;

        public MediaRepo(AppDbContext context)
        {
            _context = context;
        }

        public MediaRepo(List<Pictures> pictureItems)
        {
            this.pictureItems = pictureItems;
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

        public void DeletePicturesByUserId(string Uid)
        {
            var picturesToDelete = _context.pictures.Where(p => p.UidAuth == Uid).ToList();

            foreach (var picture in picturesToDelete)
            {
                _context.pictures.Remove(picture);
            }

            // Save changes to commit the deletions
            _context.SaveChanges();
        }

        public bool saveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
