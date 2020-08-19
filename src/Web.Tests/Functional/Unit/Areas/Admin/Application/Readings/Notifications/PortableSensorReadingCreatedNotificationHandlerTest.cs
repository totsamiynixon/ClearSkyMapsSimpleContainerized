using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Web.Application.Readings.Notifications;
using Web.Areas.Admin.Application.Readings.Notifications;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Domain.Entities;
using Xunit;

namespace Web.Tests.Functional.Unit.Areas.Admin.Application.Readings.Notifications
{
    public class PortableSensorReadingCreatedNotificationHandlerTest
    {
        private readonly Mock<IAdminDispatchHelper> _adminDispatchHelperMock;

        public PortableSensorReadingCreatedNotificationHandlerTest()
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
                var command = new PortableSensorReadingCreatedNotificationHandler(null);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_dispatch_reading_for_portable_sensor()
        {
            //Arrange
            var fakeSensor = new PortableSensor
            {
                Id = 1
            };
            var fakeSensorReading = new PortableSensorReading
            {
                Id = 1
            };

            var cancellationToken = new CancellationToken();
            var notification = new PortableReadingCreatedNotification(fakeSensor.Id, fakeSensorReading);
            var handler = new PortableSensorReadingCreatedNotificationHandler(_adminDispatchHelperMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _adminDispatchHelperMock.Verify(x =>
                x.DispatchReadingsForPortableSensorAsync(It.Is<int>(it => it == fakeSensor.Id),
                    It.Is<PortableSensorReading>(it => it == fakeSensorReading)), Times.Once);
        }

        [Fact]
        public async Task Handler_should_dispatch_coordinates_for_portable_sensor()
        {
            //Arrange
            var fakeSensor = new PortableSensor
            {
                Id = 1
            };
            var fakeSensorReading = new PortableSensorReading
            {
                Id = 1,
                Latitude = 53,
                Longitude = 53
            };

            var cancellationToken = new CancellationToken();
            var notification = new PortableReadingCreatedNotification(fakeSensor.Id, fakeSensorReading);
            var handler = new PortableSensorReadingCreatedNotificationHandler(_adminDispatchHelperMock.Object);

            //Act
            await handler.Handle(notification, cancellationToken);

            //Assert
            _adminDispatchHelperMock.Verify(x =>
                x.DispatchCoordinatesForPortableSensorAsync(It.Is<int>(it => it == fakeSensor.Id),
                    It.Is<double>(it => it == fakeSensorReading.Latitude),
                    It.Is<double>(it => it == fakeSensorReading.Longitude)), Times.Once);
        }
    }
}