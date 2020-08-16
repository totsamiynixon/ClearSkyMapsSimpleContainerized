using AutoMapper;
using Web.Areas.Admin.Application.Emulation.Queries.DTO;
using Web.Areas.Admin.Application.Users.Queries.DTO;
using Web.Domain.Entities;
using Web.Domain.Entities.Identity;
using Web.Emulation;

namespace Web.Areas.Admin.Infrastructure.Mapping
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Sensor, Application.Readings.Commands.DTO.SensorDTO>();
            CreateMap<PortableSensor, Application.Readings.Commands.DTO.PortableSensorDTO>()
                .IncludeBase<Sensor, Application.Readings.Commands.DTO.SensorDTO>();
            CreateMap<StaticSensor, Application.Readings.Commands.DTO.StaticSensorDTO>()
                .IncludeBase<Sensor, Application.Readings.Commands.DTO.SensorDTO>();
            
            
            CreateMap<Sensor, Application.Readings.Queries.DTO.SensorDTO>();
            CreateMap<PortableSensor, Application.Readings.Queries.DTO.PortableSensorDTO>()
                .IncludeBase<Sensor, Application.Readings.Queries.DTO.SensorDTO>();
            CreateMap<StaticSensor, Application.Readings.Queries.DTO.StaticSensorDTO>()
                .IncludeBase<Sensor, Application.Readings.Queries.DTO.SensorDTO>();
            
            
            CreateMap<User, UserListItemDTO>();
            CreateMap<User, UserDetailsDTO>();
            
            CreateMap<SensorEmulator, EmulatorDeviceDTO>()
                .ForMember(f => f.Latitude, m => m.MapFrom((s, d) => s.Latitude))
                .ForMember(f => f.Longitude, m => m.MapFrom((s, d) => s.Longitude))
                .ForMember(f => f.Guid, m => m.MapFrom(s => s.GetGuid()))
                .ForMember(f => f.IsOn, m => m.MapFrom(s => s.IsPowerOn))
                .ForMember(f => f.ApiKey, m => m.MapFrom(s => s.ApiKey))
                .ForMember(f => f.Type, m => m.MapFrom(s => s.SensorType.Name));
        }
    }
}