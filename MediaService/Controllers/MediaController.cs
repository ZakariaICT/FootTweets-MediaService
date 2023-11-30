using AutoMapper;
using MediaService.DTO;
using MediaService.Model;
using MediaService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MediaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private IMediaRepo _repository;
        private IMapper _mapper;

        public MediaController(IMediaRepo repository, IMapper mapper) 
        {
            _mapper = mapper;
            _repository = repository;
        }
        
        [HttpGet]    
        public ActionResult<IEnumerable<PictureReadDTO>> GetPictures()
        {
            Console.WriteLine("--> Getting Logins.....");

            var pictureItems = _repository.GetAllPictures();

            return Ok(_mapper.Map<IEnumerable<PictureReadDTO>>(pictureItems));
        }


        [HttpGet("{id}", Name = "GetPictureByID")]
        public ActionResult<PictureReadDTO> GetPictureByID(int id)
        {
            var pictureItem = _repository.GetPictureByID(id);
            if (pictureItem != null)
            {
                return Ok(_mapper.Map<PictureReadDTO>(pictureItem));
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult<PictureReadDTO> CreatePicture(PicturesDTO pictures)
        {
            var pictureModel =_mapper.Map<Pictures>(pictures);
            _repository.CreatePicture(pictureModel);
            _repository.saveChanges();

            var pictureDTO = _mapper.Map<PictureReadDTO>(pictureModel);

            return CreatedAtRoute(nameof(GetPictureByID), new { Id = pictureDTO.Id }, pictureDTO);
        }
    }
}
