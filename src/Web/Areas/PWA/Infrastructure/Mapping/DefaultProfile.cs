using AutoMapper;
using Web.Domain.Entities;
using Web.Models.Hub;

namespace Web.Areas.PWA.Infrastructure.Mapping
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<Reading, ReadingDispatchModel>();
        }
    }
}