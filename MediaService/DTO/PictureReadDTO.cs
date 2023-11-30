using System.ComponentModel.DataAnnotations;

namespace MediaService.DTO
{
    public class PictureReadDTO
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string PictureURL { get; set; }
    }
}
