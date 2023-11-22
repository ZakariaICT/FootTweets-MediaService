using System.ComponentModel.DataAnnotations;

namespace MediaService.Model
{
    public class Pictures
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string PictureURL { get; set; }
    }
}
