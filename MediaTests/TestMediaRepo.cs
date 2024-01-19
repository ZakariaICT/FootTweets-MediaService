using MediaService.Model;
using MediaService.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTests
{
    internal class TestMediaRepo : IMediaRepo
    {
        public List<Pictures> _pictureItems;

        public TestMediaRepo(List<Pictures> pictureItems)
        {
            _pictureItems = pictureItems;
        }
        public void CreatePicture(Pictures plat)
        {
            throw new NotImplementedException();
        }

        public void DeletePicturesByUserId(string Uid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Pictures> GetAllPictures()
        {
            return _pictureItems.ToList();
        }

        public Pictures GetPictureByID(int id)
        {
            return _pictureItems.FirstOrDefault(p => p.Id == id);

        }

        public bool saveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
