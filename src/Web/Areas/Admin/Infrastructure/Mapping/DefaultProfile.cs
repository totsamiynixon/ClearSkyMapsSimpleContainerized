using AutoMapper;
using Web.Application.Emulation.Queries.DTO;
using Web.Areas.Admin.Application.Readings.Commands;
using Web.Areas.Admin.Application.Readings.Queries.DTO;
using Web.Areas.Admin.Application.Users.Commands;
using Web.Areas.Admin.Application.Users.Queries.DTO;
using Web.Areas.Admin.Models.API.Emulator;
using Web.Areas.Admin.Models.API.Sensors;
using Web.Areas.Admin.Models.API.Users;
using Web.Areas.Admin.Models.Default.Emulator;
using Web.Areas.Admin.Models.Default.Sensors;
using Web.Domain.Entities;
using Web.Emulation;
using Web.Models.Hub;

namespace Web.Areas.Admin.Infrastructure.Mapping
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<Application.Readings.Queries.DTO.SensorDTO, SensorListItemModel>();
            CreateMap<Application.Readings.Queries.DTO.StaticSensorDTO, StaticSensorListItemModel>()
                .IncludeBase<Application.Readings.Queries.DTO.SensorDTO, SensorListItemModel>();
            {
                CreateMap<Web.Areas.Admin.Models.API.Sensors.CreateStaticSensorModel, CreateStaticSensorCommand>()
                    .ConstructUsing(z => new CreateStaticSensorCommand(z.ApiKey, z.Latitude, z.Longitude));
                CreateMap<Web.Areas.Admin.Models.API.Sensors.CreatePortableSensorModel, CreatePortableSensorCommand>()
                    .ConstructUsing(z => new CreatePortableSensorCommand(z.ApiKey));
                CreateMap<Web.Areas.Admin.Models.API.Sensors.DeleteSensorModel, DeleteSensorCommand>()
                    .ConstructUsing(z => new DeleteSensorCommand(z.Id.Value, z.IsCompletely));
                CreateMap<Web.Areas.Admin.Models.API.Sensors.ChangeActivationSensorModel, ChangeSensorActivationStateCommand>()
                    .ConstructUsing(z => new ChangeSensorActivationStateCommand(z.Id.Value, z.IsActive));
                CreateMap<Web.Areas.Admin.Models.API.Sensors.ChangeVisibilityStaticSensorModel, ChangeStaticSensorVisibilityStateCommand>()
                    .ConstructUsing(z => new ChangeStaticSensorVisibilityStateCommand(z.Id.Value, z.IsVisible));

                CreateMap<SensorDTO, SensorModel>();
                CreateMap<StaticSensorDTO, StaticSensorModel>()
                    .IncludeBase<SensorDTO, SensorModel>();
                CreateMap<PortableSensorDTO, PortableSensorModel>()
                    .IncludeBase<SensorDTO, SensorModel>();
                
                
                CreateMap<UserListItemDTO,  Web.Areas.Admin.Models.API.Users.UserListItemModel>();
                CreateMap<UserDetailsDTO,  Web.Areas.Admin.Models.API.Users.UserChangePasswordModel>();
                CreateMap<UserDetailsDTO,  Web.Areas.Admin.Models.API.Users.DeleteUserModel>();
                CreateMap<UserDetailsDTO,  Web.Areas.Admin.Models.API.Users.ActivateUserModel>();

                CreateMap<CreateUserModel, CreateUserCommand>()
                    .ConstructUsing(z => new CreateUserCommand(z.Email, z.Password));
                CreateMap<UserChangePasswordModel, ChangeUserPasswordCommand>()
                    .ConstructUsing(z => new ChangeUserPasswordCommand(z.Id, z.Email, z.Password, z.ConfirmPassword));
                CreateMap<DeleteUserModel, DeleteUserCommand>()
                    .ConstructUsing(z => new DeleteUserCommand(z.Id));
                CreateMap<ActivateUserModel, ChangeUserActivationStateCommand>()
                    .ConstructUsing(z => new ChangeUserActivationStateCommand(z.Id, z.IsActive));
                
                CreateMap<SensorEmulator, SensorEmulatorListItemViewModel>()
                    .ForMember(f => f.Latitude, m => m.MapFrom((s, d) => s.Latitude))
                    .ForMember(f => f.Longitude, m => m.MapFrom((s, d) => s.Longitude))
                    .ForMember(f => f.Guid, m => m.MapFrom(s => s.GetGuid()))
                    .ForMember(f => f.IsOn, m => m.MapFrom(s => s.IsPowerOn))
                    .ForMember(f => f.ApiKey, m => m.MapFrom(s => s.ApiKey))
                    .ForMember(f => f.Type, m => m.MapFrom(s => s.SensorType.Name));
                
                
                CreateMap<SensorDTO, SensorListItemViewModel>();
                CreateMap<StaticSensorDTO, StaticSensorListItemViewModel>()
                    .IncludeBase<SensorDTO, SensorListItemViewModel>();
                CreateMap<PortableSensorDTO, SensorListItemViewModel>()
                    .IncludeBase<SensorDTO, SensorListItemViewModel>();
                CreateMap<SensorDTO, SensorDetailsViewModel>();
                CreateMap<StaticSensorDTO, StaticSensorDetailsViewModel>()
                    .IncludeBase<SensorDTO, SensorDetailsViewModel>();

                CreateMap<Web.Areas.Admin.Models.Default.Sensors.CreateStaticSensorModel, CreateStaticSensorCommand>()
                    .ConstructUsing(z => new CreateStaticSensorCommand(z.ApiKey, z.Latitude, z.Longitude));
                CreateMap<Web.Areas.Admin.Models.Default.Sensors.CreatePortableSensorModel, CreatePortableSensorCommand>()
                    .ConstructUsing(z => new CreatePortableSensorCommand(z.ApiKey));
                CreateMap<Web.Areas.Admin.Models.Default.Sensors.DeleteSensorModel, DeleteSensorCommand>()
                    .ConstructUsing(z => new DeleteSensorCommand(z.Id.Value, z.IsCompletely));
                CreateMap<Web.Areas.Admin.Models.Default.Sensors.ChangeActivationSensorModel, ChangeSensorActivationStateCommand>()
                    .ConstructUsing(z => new ChangeSensorActivationStateCommand(z.Id.Value, z.IsActive));
                CreateMap<Web.Areas.Admin.Models.Default.Sensors.ChangeVisibilityStaticSensorModel, ChangeStaticSensorVisibilityStateCommand>()
                    .ConstructUsing(z => new ChangeStaticSensorVisibilityStateCommand(z.Id.Value, z.IsVisible));
                
                
                CreateMap<UserListItemDTO,  Web.Areas.Admin.Models.Default.Users.UserListItemViewModel>();
                CreateMap<UserDetailsDTO, Web.Areas.Admin.Models.Default.Users.UserChangePasswordModel>();
                CreateMap<UserDetailsDTO, Web.Areas.Admin.Models.Default.Users.DeleteUserModel>();
                CreateMap<UserDetailsDTO, Web.Areas.Admin.Models.Default.Users.ActivateUserModel>();

                CreateMap<Web.Areas.Admin.Models.Default.Users.CreateUserModel, CreateUserCommand>()
                    .ConstructUsing(z => new CreateUserCommand(z.Email, z.Password));
                CreateMap<Web.Areas.Admin.Models.Default.Users.UserChangePasswordModel, ChangeUserPasswordCommand>()
                    .ConstructUsing(z => new ChangeUserPasswordCommand(z.Id, z.Email, z.Password, z.ConfirmPassword));
                CreateMap<Web.Areas.Admin.Models.Default.Users.DeleteUserModel, DeleteUserCommand>()
                    .ConstructUsing(z => new DeleteUserCommand(z.Id));
                CreateMap<Web.Areas.Admin.Models.Default.Users.ActivateUserModel, ChangeUserActivationStateCommand>()
                    .ConstructUsing(z => new ChangeUserActivationStateCommand(z.Id, z.IsActive));
                
                
                CreateMap<Reading, StaticSensorReadingDispatchModel>();

                CreateMap<EmulatorDeviceDTO, SensorEmulatorListItemModel>();
                CreateMap<EmulatorDeviceDTO, SensorEmulatorListItemViewModel>();
            }
        }
    }
}