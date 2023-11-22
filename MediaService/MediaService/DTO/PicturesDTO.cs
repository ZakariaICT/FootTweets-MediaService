using System.ComponentModel.DataAnnotations;

namespace MediaService.DTO
{
    public class PicturesDTO
    {

        [Required]
        public string PictureURL { get; set; }
    }
}
