using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Application.Readings.Exceptions;
using Web.Application.Readings.Notifications;
using Web.Domain.Entities;
using Web.Helpers;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Xunit;

namespace Web.UnitTests.Application.Readings.Notifications
{
    public class StaticSensorReadingCreatedNotificationHandlerTest
    {
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;
        private readonly Mock<ISensorCacheHelper> _sensorCacheHelperMock;

        public StaticSensorReadingCreatedNotificationHandlerTest()
        {
            _dataContextMock = new Mock<DataContext>(new DbContextOptions<DataContext>());
            _dataContextFactoryMock = new Mock<IDataContextFactory<DataContext>>();
            _sensorCacheHelperMock = new Mock<ISensorCacheHelper>();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Handler_should_throw_exception_if_dependency_is_null(int paramsSetIndex)
        {
            //Arrage
            var testArgs = new[]
            {
                new[] { _dataContextFactoryMock.Object, (object) null},
                new[] {(object) null, (object) _sensorCacheHelperMock.Object}
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var staticSensorReadingCreatedNotificationHandler = new StaticSensorReadingCreatedNotificationHandler((IDataContextFactory<DataContext>) testArgsCurrentSet[0],
                    (ISensorCacheHelper)testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_throw_exception_if_sensor_not_found()
        {
            //Arrange

            var fakeReading = new StaticSensorReading
            {
                Id = 1,
                StaticSensorId = 1
            };

            var fakeSensor = new StaticSensor
            {
                Id = 1,
                ApiKey = CryptoHelper.GenerateApiKey(),
                IsActive = true,
                IsVisible = true,
                Readings = new List<StaticSensorReading> {fakeReading}
            };
            var fakeSensorDbSet = new List<StaticSensor> ();

            _dataContextMock.Setup(x => x.Set<StaticSensor>()).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var notification = new StaticSensorReadingCreatedNotification(fakeSensor.Id, fakeReading);
            var handler =
                new StaticSensorReadingCreatedNotificationHandler(_dataContextFactoryMock.Object,
                    _sensorCacheHelperMock.Object);

            //Act
            Task Act() => handler.Handle(notification, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<SensorNotFoundException>(Act);
        }
        
        [Fact]
        public async Task Handler_update_cache()
        {
            //Arrange

            var fakeReading = new StaticSensorReading
            {
                Id = 1,
                StaticSensorId = 1
            };

            var fakeSensor = new StaticSensor
            {
                Id = 1,
                ApiKey = CryptoHelper.GenerateApiKey(),
                IsActive = true,
                IsVisible = true,
                Readings = new List<StaticSensorReading> {fakeReading}
            };
            var fakeSensorDbSet = new List<StaticSensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Set<StaticSensor>()).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var notification = new StaticSensorReadingCreatedNotification(fakeSensor.Id, fakeReading);
            var handler =
                new StaticSensorReadingCreatedNotificationHandler(_dataContextFactoryMock.Object,
                    _sensorCacheHelperMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _sensorCacheHelperMock.Verify(
                x => x.UpdateSensorCacheWithReadingAsync(It.Is<StaticSensorReading>(it => it == fakeReading)), Times.Once);
        }
    }
}