using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Application.Readings.Exceptions;
using Web.Areas.Admin.Application.Readings.Notifications;
using Web.Domain.Entities;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Xunit;

namespace Web.Tests.Functional.Areas.Admin.Application.Readings.Notifications
{
    public class SensorActivationStateChangedNotificationHandlerTest
    {
        private readonly Mock<ISensorCacheHelper> _sensorCacheHelperMock;
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;

        public SensorActivationStateChangedNotificationHandlerTest()
        {
            _sensorCacheHelperMock = new Mock<ISensorCacheHelper>();
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
                new[] {(object) null, _dataContextFactoryMock.Object},
                new[] {_sensorCacheHelperMock.Object, (object) null},
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var handler = new SensorActivationStateChangedNotificationHandler(
                    (ISensorCacheHelper) testArgsCurrentSet[0],
                    (IDataContextFactory<DataContext>) testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_throw_not_found_exception_if_sensor_doesnt_exist()
        {
            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var notification = new SensorActivationStateChangedNotification(1);
            var handler =
                new SensorActivationStateChangedNotificationHandler(_sensorCacheHelperMock.Object,
                    _dataContextFactoryMock.Object);

            //Act
            Task Act() => handler.Handle(notification, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<SensorNotFoundException>(Act);
        }


        [Fact]
        public async Task Handler_should_update_cache_for_static_sensor()
        {
            var fakeStaticSensorReading = new StaticSensorReading
            {
                Id = 1,
                StaticSensorId = 1
            };
            var fakeStaticSensor = new StaticSensor
            {
                Id = 1,
                Readings = new List<StaticSensorReading> {fakeStaticSensorReading}
            };
            var fakeStaticSensorReadingDbSet = new List<StaticSensorReading> {fakeStaticSensorReading};
            var fakeStaticSensorDbSet = new List<Sensor> {fakeStaticSensor};

            _dataContextMock.Setup(x => x.StaticSensorReadings).ReturnsDbSet(fakeStaticSensorReadingDbSet);
            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeStaticSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var notification = new SensorActivationStateChangedNotification(fakeStaticSensor.Id);
            var handler =
                new SensorActivationStateChangedNotificationHandler(_sensorCacheHelperMock.Object,
                    _dataContextFactoryMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _sensorCacheHelperMock.Verify(
                x => x.UpdateStaticSensorCacheAsync(It.Is<StaticSensor>(it => it == fakeStaticSensor)), Times.Once);
        }

        [Fact]
        public async Task Handler_should_update_cache_for_static_sensor_if_available_with_latest_readings()
        {
            var fakeStaticSensorReading = new StaticSensorReading
            {
                Id = 1,
                StaticSensorId = 1
            };
            var fakeStaticSensor = new StaticSensor
            {
                Id = 1,
                IsActive = true,
                IsVisible = true
            };
            var fakeStaticSensorReadingDbSet = new List<StaticSensorReading> {fakeStaticSensorReading};
            var fakeStaticSensorDbSet = new List<Sensor> {fakeStaticSensor};

            _dataContextMock.Setup(x => x.StaticSensorReadings).ReturnsDbSet(fakeStaticSensorReadingDbSet);
            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeStaticSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var notification = new SensorActivationStateChangedNotification(fakeStaticSensor.Id);
            var handler =
                new SensorActivationStateChangedNotificationHandler(_sensorCacheHelperMock.Object,
                    _dataContextFactoryMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _sensorCacheHelperMock.Verify(
                x => x.UpdateStaticSensorCacheAsync(It.Is<StaticSensor>(it =>
                    it == fakeStaticSensor && it.Readings.SequenceEqual(fakeStaticSensorReadingDbSet))), Times.Once);
        }
    }
}