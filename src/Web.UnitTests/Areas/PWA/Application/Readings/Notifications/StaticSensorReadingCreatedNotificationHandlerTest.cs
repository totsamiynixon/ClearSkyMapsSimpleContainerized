using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Application.Readings.Exceptions;
using Web.Application.Readings.Notifications;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Xunit;
using StaticSensorReadingCreatedNotificationHandler =
    Web.Areas.PWA.Application.Readings.Notifications.StaticSensorReadingCreatedNotificationHandler;

namespace Web.UnitTests.Areas.PWA.Application.Readings.Notifications
{
    public class StaticSensorReadingCreatedNotificationHandlerTest
    {
        private readonly Mock<IPWADispatchHelper> _pwaDispatchHelperMock;
        private readonly Mock<ISensorCacheHelper> _sensorCacheHelperMock;
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;

        public StaticSensorReadingCreatedNotificationHandlerTest()
        {
            _pwaDispatchHelperMock = new Mock<IPWADispatchHelper>();
            _dataContextFactoryMock = new Mock<IDataContextFactory<DataContext>>();
            _dataContextMock = new Mock<DataContext>(new DbContextOptions<DataContext>());
            _dataContextFactoryMock = new Mock<IDataContextFactory<DataContext>>();
            _sensorCacheHelperMock = new Mock<ISensorCacheHelper>();
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
                new[] {_pwaDispatchHelperMock.Object, (object) _sensorCacheHelperMock.Object, (object) null},
                new[] {_pwaDispatchHelperMock.Object, null, (object) _dataContextFactoryMock.Object},
                new[] {(object) null, (object) _sensorCacheHelperMock.Object, _dataContextFactoryMock.Object}
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var createReadingCommandHandler = new StaticSensorReadingCreatedNotificationHandler(
                    (IPWADispatchHelper) testArgsCurrentSet[0], (ISensorCacheHelper) testArgsCurrentSet[1],
                    (IDataContextFactory<DataContext>) testArgsCurrentSet[2]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }


        [Fact]
        public async Task Handler_should_throw_not_found_exception_if_sensor_doesnt_exist()
        {
            //Arrange
            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(new List<StaticSensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var notification = new StaticSensorReadingCreatedNotification(0, new StaticSensorReading());
            var handler = new StaticSensorReadingCreatedNotificationHandler(_pwaDispatchHelperMock.Object,
                _sensorCacheHelperMock.Object, _dataContextFactoryMock.Object);

            //Act
            Task Act() => handler.Handle(notification, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<SensorNotFoundException>(Act);
        }

        [Fact]
        public async Task Handler_dispatch_reading_if_sensor_available()
        {
            //Arrange
            var fakeSensor = new StaticSensor
            {
                Id = 1,
                Readings = new List<StaticSensorReading>
                {
                    new StaticSensorReading
                    {
                        StaticSensorId = 1
                    }
                },
                IsActive = true,
                IsVisible = true
            };

            var fakeSensorDbSet = new List<StaticSensor> {fakeSensor};

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _sensorCacheHelperMock.Setup(x => x.GetPollutionLevelAsync(fakeSensor.Id))
                .Returns(Task.FromResult(PollutionLevel.High));

            var cancellationToken = new CancellationToken();
            var notification = new StaticSensorReadingCreatedNotification(fakeSensor.Id, fakeSensor.Readings.First());
            var handler = new StaticSensorReadingCreatedNotificationHandler(_pwaDispatchHelperMock.Object,
                _sensorCacheHelperMock.Object, _dataContextFactoryMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _pwaDispatchHelperMock.Verify(
                x => x.DispatchReadingsForStaticSensor(It.Is<int>(it => it == fakeSensor.Id),
                    It.Is<PollutionLevel>(it => it == PollutionLevel.High),
                    It.Is<StaticSensorReading>(it => it == notification.Reading)),
                Times.Once);
        }


        [Fact]
        public async Task Handler_skip_dispatch_reading_if_sensor_not_available()
        {
            //Arrange
            var fakeSensor = new StaticSensor
            {
                Id = 1,
                Readings = new List<StaticSensorReading>
                {
                    new StaticSensorReading
                    {
                        StaticSensorId = 1
                    }
                },
                IsActive = false,
                IsVisible = true
            };

            var fakeSensorDbSet = new List<StaticSensor> {fakeSensor};

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _sensorCacheHelperMock.Setup(x => x.GetPollutionLevelAsync(fakeSensor.Id))
                .Returns(Task.FromResult(PollutionLevel.High));

            var cancellationToken = new CancellationToken();
            var notification = new StaticSensorReadingCreatedNotification(fakeSensor.Id, fakeSensor.Readings.First());
            var handler = new StaticSensorReadingCreatedNotificationHandler(_pwaDispatchHelperMock.Object,
                _sensorCacheHelperMock.Object, _dataContextFactoryMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _pwaDispatchHelperMock.Verify(
                x => x.DispatchReadingsForStaticSensor(fakeSensor.Id, PollutionLevel.High, notification.Reading),
                Times.Never);
        }
    }
}