using AutoMapper;
using Web.Domain.Entities;

namespace Web.Areas.PWA.Infrastructure.Mapping
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Web.Application.Readings.Commands.DTO.StaticSensorReadingDTO, StaticSensorReading>();
        }
    }
}