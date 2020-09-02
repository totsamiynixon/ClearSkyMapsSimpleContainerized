using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Application.Readings.Commands;
using Web.Application.Readings.Commands.DTO;
using Web.Application.Readings.Exceptions;
using Web.Application.Readings.Notifications;
using Web.Domain.Entities;
using Web.Helpers;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Xunit;

namespace Web.UnitTests.Application.Readings.Commands
{
    public class CreateReadingCommandHandlerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IMapper> _mapperMock;

        public CreateReadingCommandHandlerTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _dataContextFactoryMock = new Mock<IDataContextFactory<DataContext>>();
            _dataContextMock = new Mock<DataContext>(new DbContextOptions<DataContext>());
            _mapperMock = new Mock<IMapper>();
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Handler_should_throw_exception_if_dependency_is_null(int paramsSetIndex)
        {
            //Arrage
            var testArgs = new[]
            {
                new[] {_mediatorMock.Object, (object) _dataContextFactoryMock.Object, (object) null},
                new[] {(object) _mediatorMock.Object, null, (object) _mapperMock.Object},
                new[] {(object) null, (object) _dataContextFactoryMock.Object, _mapperMock.Object}
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var createReadingCommandHandler = new CreateReadingCommandHandler((IMediator) testArgsCurrentSet[0],
                    (IDataContextFactory<DataContext>) testArgsCurrentSet[1], (IMapper) testArgsCurrentSet[2]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }


        [Fact]
        public async Task Handler_should_throw_exception_if_sensor_not_found()
        {
            //Arrange
            var apiKey = CryptoHelper.GenerateApiKey();
            var notExistingApiKey = apiKey.Reverse().ToString();
            var fakeStaticSensor = new StaticSensor {Id = 1, ApiKey = apiKey};
            var fakeReading = new SensorReadingDTO();

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor> {fakeStaticSensor});
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new CreateReadingCommand(fakeReading, notExistingApiKey);
            var handler = new CreateReadingCommandHandler(_mediatorMock.Object, _dataContextFactoryMock.Object,
                _mapperMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<SensorNotFoundException>(Act);
        }

        [Fact]
        public async Task Handler_return_true_if_reading_is_for_portable_sensor()
        {
            //Arrange
            var apiKey = CryptoHelper.GenerateApiKey();
            var fakePortableSensor = new PortableSensor() {Id = 1, ApiKey = apiKey};
            var fakeReading = new SensorReadingDTO();

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor> {fakePortableSensor});
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);


            //Act
            var cancellationToken = new CancellationToken();
            var command = new CreateReadingCommand(fakeReading, apiKey);
            var handler = new CreateReadingCommandHandler(_mediatorMock.Object, _dataContextFactoryMock.Object,
                _mapperMock.Object);
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handler_publish_notification_if_reading_is_for_portable_sensor()
        {
            //Arrange
            var apiKey = CryptoHelper.GenerateApiKey();
            var fakePortableSensor = new PortableSensor() {Id = 1, ApiKey = apiKey};
            var fakePortableSensorReading = new PortableSensorReading();
            var fakeReading = new SensorReadingDTO();

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor> {fakePortableSensor});
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<SensorReadingDTO, PortableSensorReading>(fakeReading))
                .Returns(fakePortableSensorReading);


            //Act
            var cancellationToken = new CancellationToken();
            var command = new CreateReadingCommand(fakeReading, apiKey);
            var handler = new CreateReadingCommandHandler(_mediatorMock.Object, _dataContextFactoryMock.Object,
                _mapperMock.Object);
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            _mediatorMock.Verify(
                x => x.Publish(It.Is<PortableReadingCreatedNotification>(it =>
                        it.SensorId == fakePortableSensor.Id && it.Reading == fakePortableSensorReading),
                    It.Is<CancellationToken>(it => it == cancellationToken)), Times.Once);
        }


        [Fact]
        public async Task Handler_return_true_if_reading_is_for_static_sensor()
        {
            //Arrange
            var apiKey = CryptoHelper.GenerateApiKey();
            var fakeStaticSensor = new StaticSensor {Id = 1, ApiKey = apiKey};
            var fakeStaticSensorReading = new StaticSensorReading();
            var fakeReadingDTO = new SensorReadingDTO();
            var fakeReadingsDbSet = new List<StaticSensorReading>();

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor> {fakeStaticSensor});
            _dataContextMock.Setup(x => x.StaticSensorReadings).ReturnsDbSet(fakeReadingsDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<SensorReadingDTO, StaticSensorReading>(fakeReadingDTO))
                .Returns(fakeStaticSensorReading);


            //Act
            var cancellationToken = new CancellationToken();
            var command = new CreateReadingCommand(fakeReadingDTO, apiKey);
            var handler = new CreateReadingCommandHandler(_mediatorMock.Object, _dataContextFactoryMock.Object,
                _mapperMock.Object);
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handler_update_database_if_reading_is_for_static_sensor()
        {
            //Arrange
            var apiKey = CryptoHelper.GenerateApiKey();
            var fakeStaticSensor = new StaticSensor {Id = 1, ApiKey = apiKey};
            var fakeStaticSensorReading = new StaticSensorReading();
            var fakeReadingDTO = new SensorReadingDTO();
            var fakeReadingsDbSet = new List<StaticSensorReading>();

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor> {fakeStaticSensor});
            _dataContextMock.Setup(x => x.StaticSensorReadings).ReturnsDbSet(fakeReadingsDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<SensorReadingDTO, StaticSensorReading>(fakeReadingDTO))
                .Returns(fakeStaticSensorReading);


            //Act
            var cancellationToken = new CancellationToken();
            var command = new CreateReadingCommand(fakeReadingDTO, apiKey);
            var handler = new CreateReadingCommandHandler(_mediatorMock.Object, _dataContextFactoryMock.Object,
                _mapperMock.Object);
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            _dataContextMock.Verify(x =>
                x.StaticSensorReadings.AddAsync(It.Is<StaticSensorReading>(it => it == fakeStaticSensorReading),
                    It.Is<CancellationToken>(it => it == cancellationToken)), Times.Once);
            _dataContextMock.Verify(x =>
                x.SaveChangesAsync(It.Is<CancellationToken>(it => it == cancellationToken)), Times.Once);
        }

        [Fact]
        public async Task Handler_publish_notification_if_reading_is_for_static_sensor()
        {
            //Arrange
            var apiKey = CryptoHelper.GenerateApiKey();
            var fakeStaticSensor = new StaticSensor {Id = 1, ApiKey = apiKey};
            var fakeStaticSensorReading = new StaticSensorReading();
            var fakeReadingDTO = new SensorReadingDTO();
            var fakeReadingsDbSet = new List<StaticSensorReading>();

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor> {fakeStaticSensor});
            _dataContextMock.Setup(x => x.StaticSensorReadings).ReturnsDbSet(fakeReadingsDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<SensorReadingDTO, StaticSensorReading>(fakeReadingDTO))
                .Returns(fakeStaticSensorReading);


            //Act
            var cancellationToken = new CancellationToken();
            var command = new CreateReadingCommand(fakeReadingDTO, apiKey);
            var handler = new CreateReadingCommandHandler(_mediatorMock.Object, _dataContextFactoryMock.Object,
                _mapperMock.Object);
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            _mediatorMock.Verify(
                x => x.Publish(
                    It.Is<StaticSensorReadingCreatedNotification>(it =>
                        it.SensorId == fakeStaticSensor.Id && it.Reading == fakeStaticSensorReading),
                    It.Is<CancellationToken>(it => it == cancellationToken)), Times.Once);
        }
    }
}