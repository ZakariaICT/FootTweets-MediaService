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

        // MediaService.Controllers

        [HttpPost("post")]
        public ActionResult<PictureReadDTO> CreatePicture(PicturesDTO pictures, [FromServices] IServiceProvider serviceProvider, [FromServices] IConfiguration configuration)
        {
            var pictureModel = _mapper.Map<Pictures>(pictures);

            // Step 1: Create the picture in the database
            ////_repository.CreatePicture(pictureModel);
            //////_repository.saveChanges();

            // Step 2: Get UID from RabbitMQ
            var uidListener = serviceProvider.GetRequiredService<RabbitMQListener>();
            var uid = uidListener.GetUidFromQueue(serviceProvider);

            if (!string.IsNullOrEmpty(uid))
            {
                // Step 3: Use the received UID to handle the creation of a new picture
                pictureModel.Uid = uid;
                _repository.CreatePicture(pictureModel);
                _repository.saveChanges();

                // Step 4: Send RabbitMQ message for UID
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(configuration["RabbitMQConnection"])
                };

                using (var connection = factory.CreateConnection())
                {
                    var channel = connection.CreateModel();
                    var rabbitMQService = new RabbitMQService(channel);
                    rabbitMQService.SendMessage($"New picture created: {pictureModel.Text}, {pictureModel.Uid}");
                }

                return CreatedAtRoute(nameof(GetPictureByID), new { Id = pictureModel.Id }, _mapper.Map<PictureReadDTO>(pictureModel));
            }
            else
            {
                // Handle the case where the UID is not received
                return StatusCode(500, "Failed to obtain UID from RabbitMQ.");
            }
        }









        private void ProcessMessageLocally(PictureReadDTO pictureDTO)
        {
            // Process the message locally (e.g., create a user in the database)
            Console.WriteLine($" [x] Received 'New picture created: {pictureDTO.Text}'");

            // Save the picture to the database
            var pictureModel = _mapper.Map<Pictures>(pictureDTO);
            _repository.CreatePicture(pictureModel);
            _repository.saveChanges();
        }

    }
}
