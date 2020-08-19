using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Web.Application.Readings.Notifications;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Domain.Entities;
using Xunit;
using StaticSensorReadingCreatedNotificationHandler = Web.Areas.Admin.Application.Readings.Notifications.StaticSensorReadingCreatedNotificationHandler;

namespace Web.Tests.Functional.Unit.Areas.Admin.Application.Readings.Notifications
{
    public class StaticSensorReadingCreatedNotificationHandlerTest
    {
        private readonly Mock<IAdminDispatchHelper> _adminDispatchHelperMock;
        
        public StaticSensorReadingCreatedNotificationHandlerTest()
        {
            _adminDispatchHelperMock = new Mock<IAdminDispatchHelper>();
        }
        
        [Fact]
        public async Task Handler_should_throw_exception_if_dependency_is_null()
        {
            //Arrage

            //Act
            void Act()
            {
                var command = new StaticSensorReadingCreatedNotificationHandler(null);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }
        
        [Fact]
        public async Task Handler_should_dispatch_reading_for_portable_sensor()
        {
            //Arrange
            var fakeSensor = new StaticSensor
            {
                Id = 1
            };
            var fakeSensorReading = new StaticSensorReading
            {
                Id = 1
            };

            var cancellationToken = new CancellationToken();
            var notification = new StaticSensorReadingCreatedNotification(fakeSensor.Id, fakeSensorReading);
            var handler = new StaticSensorReadingCreatedNotificationHandler(_adminDispatchHelperMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _adminDispatchHelperMock.Verify(x =>
                x.DispatchReadingsForStaticSensorAsync(It.Is<int>(it => it == fakeSensor.Id),
                    It.Is<StaticSensorReading>(it => it == fakeSensorReading)), Times.Once);
        }
    }
}