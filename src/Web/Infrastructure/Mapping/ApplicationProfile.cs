using AutoMapper;
using Web.Domain.Entities;

namespace Web.Infrastructure.Mapping
{
    //TODO: Think more about mapping organization
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Web.Application.Readings.Commands.DTO.SensorReadingDTO, StaticSensorReading>();
            CreateMap<Web.Application.Readings.Commands.DTO.SensorReadingDTO, PortableSensorReading>();


            CreateMap<Reading, Web.Application.Readings.Queries.DTO.StaticSensorReadingDTO>();
        }
    }
}