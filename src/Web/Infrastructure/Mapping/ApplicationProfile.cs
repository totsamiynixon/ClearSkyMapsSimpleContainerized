using AutoMapper;
using Web.Domain.Entities;

namespace Web.Infrastructure.Mapping
{
    //TODO: Think more about mapping organization
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Application.Readings.Commands.DTO.SensorReadingDTO, StaticSensorReading>();
            CreateMap<Application.Readings.Commands.DTO.SensorReadingDTO, PortableSensorReading>();


            CreateMap<Reading, Application.Readings.Queries.DTO.StaticSensorReadingDTO>();
        }
    }
}