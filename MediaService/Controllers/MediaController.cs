using AutoMapper;
using MediaService.DTO;
using MediaService.Model;
using MediaService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RabbitMQ.Client;

namespace MediaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IMediaRepo _repository;
        private IMapper _mapper;

        public MediaController(IConfiguration configuration, IMediaRepo repository, IMapper mapper) 
        {
            _configuration = configuration;
            _mapper = mapper;
            _repository = repository;
        }
        
        [HttpGet("all")]    
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

        [HttpPost("post")]
        public ActionResult<PictureReadDTO> CreatePicture(PicturesDTO pictures)
        {
            var pictureModel =_mapper.Map<Pictures>(pictures);
            _repository.CreatePicture(pictureModel);
            _repository.saveChanges();

            var pictureDTO = _mapper.Map<PictureReadDTO>(pictureModel);

            // Send RabbitMQ message
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_configuration["RabbitMQConnection"])
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var rabbitMQService = new RabbitMQService(channel);
                rabbitMQService.SendMessage($"New user created: {pictureDTO.Text}");

                // Process the message immediately in the database
                ProcessMessageLocally(pictureDTO);
            }

            return CreatedAtRoute(nameof(GetPictureByID), new { Id = pictureDTO.Id }, pictureDTO);
        }

        private void ProcessMessageLocally(PictureReadDTO pictureDTO)
        {
            // Process the message (e.g., create a user in the database)
            Console.WriteLine($" [x] Received 'New picture created: {pictureDTO.Text}'");

            // Save the user to the database
            var pictureModel = _mapper.Map<Pictures>(pictureDTO);
            _repository.CreatePicture(pictureModel);
            _repository.saveChanges();
        }

    }
}
