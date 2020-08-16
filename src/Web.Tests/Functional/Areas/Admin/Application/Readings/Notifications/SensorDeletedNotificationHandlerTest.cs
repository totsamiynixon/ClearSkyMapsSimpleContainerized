using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Web.Areas.Admin.Application.Readings.Notifications;
using Web.Domain.Entities;
using Web.Helpers.Interfaces;
using Xunit;

namespace Web.Tests.Functional.Areas.Admin.Application.Readings.Notifications
{
    public class SensorDeletedNotificationHandlerTest
    {
        private readonly Mock<ISensorCacheHelper> _sensorCacheHelperMock;

        public SensorDeletedNotificationHandlerTest()
        {
            _sensorCacheHelperMock = new Mock<ISensorCacheHelper>();
        }

        [Fact]
        public async Task Handler_should_throw_exception_if_dependency_is_null()
        {
            //Arrage

            //Act
            void Act()
            {
                var command = new SensorDeletedNotificationHandler(null);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_remove_static_sensor_from_cache()
        {
            //Arrage

            var fakeStaticSensor = new StaticSensor
            {
                Id = 1
            };

            var cancellationToken = new CancellationToken();
            var notification = new SensorDeletedNotification(fakeStaticSensor);
            var handler = new SensorDeletedNotificationHandler(_sensorCacheHelperMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _sensorCacheHelperMock.Verify(
                x => x.RemoveStaticSensorFromCacheAsync(It.Is<int>(it => it == fakeStaticSensor.Id)), Times.Once);
        }
    }
}