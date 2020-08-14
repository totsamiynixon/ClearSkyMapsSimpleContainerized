using AutoMapper;
using Web.Domain.Entities;

namespace Web.Infrastructure.Mapping
{
    //TODO: Think more about mapping organization
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Application.Readings.Commands.DTO.StaticSensorReadingDTO, StaticSensorReading>();
            CreateMap<Application.Readings.Commands.DTO.StaticSensorReadingDTO, PortableSensorReading>();


            CreateMap<Reading, Application.Readings.Queries.DTO.StaticSensorReadingDTO>();
        }
    }
}