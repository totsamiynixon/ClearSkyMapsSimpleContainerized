using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Application.Readings.Exceptions;
using Web.Areas.Admin.Application.Readings.Commands;
using Web.Areas.Admin.Application.Readings.Notifications;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Xunit;

namespace Web.UnitTests.Areas.Admin.Application.Readings.Commands
{
    public class ChangeStaticSensorVisibilityStateCommandHandlerTest
    {
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;
        private readonly Mock<IMediator> _mediatorMock;

        public ChangeStaticSensorVisibilityStateCommandHandlerTest()
        {
            _dataContextMock = new Mock<DataContext>(new DbContextOptions<DataContext>());
            _dataContextFactoryMock = new Mock<IDataContextFactory<DataContext>>();
            _mediatorMock = new Mock<IMediator>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Handler_should_throw_exception_if_dependency_is_null(int paramsSetIndex)
        {
            //Arrage
            var testArgs = new[]
            {
                new[] {(object) null, _mediatorMock.Object},
                new[] {_dataContextFactoryMock.Object, (object) null},
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var handler = new ChangeStaticSensorVisibilityStateCommandHandler(
                    (IDataContextFactory<DataContext>) testArgsCurrentSet[0],
                    (IMediator) testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_return_true()
        {
            var fakeSensor = new StaticSensor()
            {
                Id = 1,
                IsVisible = false
            };
            var fakeSensorDbSet = new List<StaticSensor> {fakeSensor};

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeStaticSensorVisibilityStateCommand(fakeSensor.Id, !fakeSensor.IsVisible);
            var handler =
                new ChangeStaticSensorVisibilityStateCommandHandler(_dataContextFactoryMock.Object,
                    _mediatorMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handler_should_throw_not_found_exception_if_sensor_doesnt_exist()
        {
            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(new List<StaticSensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeStaticSensorVisibilityStateCommand(1, false);
            var handler =
                new ChangeStaticSensorVisibilityStateCommandHandler(_dataContextFactoryMock.Object,
                    _mediatorMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<SensorNotFoundException>(Act);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handler_activation_state_should_be_changed(bool newVisibilityState)
        {
            var fakeSensor = new StaticSensor()
            {
                Id = 1,
                IsVisible = !newVisibilityState
            };
            var fakeSensorDbSet = new List<StaticSensor> {fakeSensor};

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeStaticSensorVisibilityStateCommand(fakeSensor.Id, newVisibilityState);
            var handler =
                new ChangeStaticSensorVisibilityStateCommandHandler(_dataContextFactoryMock.Object,
                    _mediatorMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            Assert.Equal(newVisibilityState, fakeSensor.IsVisible);
        }


        [Fact]
        public async Task Handler_database_should_be_updated()
        {
            var fakeSensor = new StaticSensor()
            {
                Id = 1,
                IsVisible = false
            };
            var fakeSensorDbSet = new List<StaticSensor> {fakeSensor};

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeStaticSensorVisibilityStateCommand(fakeSensor.Id, !fakeSensor.IsVisible);
            var handler =
                new ChangeStaticSensorVisibilityStateCommandHandler(_dataContextFactoryMock.Object,
                    _mediatorMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            _dataContextMock.Verify(x => x.SaveChangesAsync(It.Is<CancellationToken>(it => it == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handler_should_publish_notification()
        {
            var fakeSensor = new StaticSensor
            {
                Id = 1,
                IsActive = false
            };
            var fakeSensorDbSet = new List<StaticSensor> {fakeSensor};

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeStaticSensorVisibilityStateCommand(fakeSensor.Id, !fakeSensor.IsActive);
            var handler =
                new ChangeStaticSensorVisibilityStateCommandHandler(_dataContextFactoryMock.Object,
                    _mediatorMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            _mediatorMock.Verify(x =>
                    x.Publish(It.Is<StaticSensorVisibilityStateChangedNotification>(it => it.SensorId == fakeSensor.Id),
                        It.Is<CancellationToken>(it => it == cancellationToken)),
                Times.Once());
        }
    }
}