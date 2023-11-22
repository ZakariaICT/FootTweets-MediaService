using AutoMapper;
using MediaService.DTO;
using MediaService.Model;

namespace MediaService.Mapper
{
    public class MediaMapper : Profile
    {
        public MediaMapper()
        {
            // Source --> Target
            CreateMap<Pictures, PictureReadDTO>();
            CreateMap<PicturesDTO, Pictures>();
        }
    }
}
