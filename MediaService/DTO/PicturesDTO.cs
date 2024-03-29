﻿using System.ComponentModel.DataAnnotations;

namespace MediaService.DTO
{
    public class PicturesDTO
    {

        [Required]
        public string Text { get; set; }

        [Required]
        public string PictureURL { get; set; }

        public string UidAuth { get; set; }
    }
}
