using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Areas.Admin.Application.Readings.Commands;
using Web.Areas.Admin.Application.Readings.Commands.DTO;
using Web.Domain.Entities;
using Web.Helpers;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Xunit;

namespace Web.Tests.Functional.Areas.Admin.Application.Readings.Commands
{
    public class CreateStaticSensorCommandHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;

        public CreateStaticSensorCommandHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _dataContextMock = new Mock<DataContext>(new DbContextOptions<DataContext>());
            _dataContextFactoryMock = new Mock<IDataContextFactory<DataContext>>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Handler_should_throw_exception_if_dependency_is_null(int paramsSetIndex)
        {
            //Arrage
            var testArgs = new[]
            {
                new[] {_dataContextFactoryMock.Object, (object) null},
                new[] {(object) null, _mapperMock.Object},
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var handler = new CreateStaticSensorCommandHandler(
                    (IDataContextFactory<DataContext>) testArgsCurrentSet[0], (IMapper) testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }
        
        [Fact]
        public async Task Handler_should_update_database_with_sensor_with_provided_params()
        {
            //Arrange
            var apiKey = CryptoHelper.GenerateApiKey();

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(new List<StaticSensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new CreateStaticSensorCommand(apiKey, 53, 53);
            var handler = new CreateStaticSensorCommandHandler(_dataContextFactoryMock.Object, _mapperMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            //Multiple assert because test is the same for both verified methods and they are very closely related
            _dataContextMock.Verify(x => x.StaticSensors.AddAsync(It.Is<StaticSensor>(it => it.ApiKey == apiKey && it.Latitude == command.Latitude && it.Longitude == command.Longitude),
                It.Is<CancellationToken>(it => it == cancellationToken)), Times.Once);
            _dataContextMock.Verify(x => x.SaveChangesAsync(
                It.Is<CancellationToken>(it => it == cancellationToken)), Times.Once);
        }
        
        [Fact]
        public async Task Handler_should_return_created_sensor_dto_with_provided_params()
        {
            //Arrange
            var apiKey = CryptoHelper.GenerateApiKey();

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(new List<StaticSensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<StaticSensor, StaticSensorDTO>(It.IsAny<StaticSensor>())).Returns(
                (StaticSensor v) => new StaticSensorDTO
                {
                    ApiKey = v.ApiKey,
                    Latitude = v.Latitude,
                    Longitude = v.Longitude
                });

            var cancellationToken = new CancellationToken();
            var command = new CreateStaticSensorCommand(apiKey, 53, 53);
            var handler = new CreateStaticSensorCommandHandler(_dataContextFactoryMock.Object, _mapperMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.Equal(apiKey, result.ApiKey);
            Assert.Equal(command.Latitude, result.Latitude);
            Assert.Equal(command.Longitude, result.Longitude);
        }
    }
}