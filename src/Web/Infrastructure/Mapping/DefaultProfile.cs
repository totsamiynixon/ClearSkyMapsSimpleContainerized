using AutoMapper;
using Web.Models.API.Sensors;

namespace Web.Infrastructure.Mapping
{
    //TODO: think about naming
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<Web.Application.Readings.Queries.DTO.StaticSensorDTO, StaticSensorModel>();
            CreateMap<Web.Application.Readings.Queries.DTO.StaticSensorReadingDTO, StaticSensorReadingModel>();
        }
    }
}