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

namespace Web.Tests.Functional.Unit.Areas.Admin.Application.Readings.Commands
{
    public class ChangeActivationStateCommandHandlerTest
    {
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;
        private readonly Mock<IMediator> _mediatorMock;

        public ChangeActivationStateCommandHandlerTest()
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
                var createReadingCommandHandler = new ChangeSensorActivationStateCommandHandler(
                    (IDataContextFactory<DataContext>) testArgsCurrentSet[0], (IMediator) testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_return_true()
        {
            var fakeSensor = new PortableSensor
            {
                Id = 1,
                IsActive = false
            };
            var fakeSensorDbSet = new List<Sensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeSensorActivationStateCommand(fakeSensor.Id, !fakeSensor.IsActive);
            var handler =
                new ChangeSensorActivationStateCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handler_should_throw_not_found_exception_if_sensor_doesnt_exist()
        {
            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeSensorActivationStateCommand(1, false);
            var handler =
                new ChangeSensorActivationStateCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            Task Act() => handler.Handle(command, cancellationToken);

            //Assert
            await Assert.ThrowsAsync<SensorNotFoundException>(Act);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData(1, false)]
        public async Task Handler_activation_state_should_be_changed(int sensorIndex, bool newActivationState)
        {
            //Check for both types of sensors
            var sensors = new List<Sensor>
            {
                new PortableSensor
                {
                    Id = 1,
                    IsActive = !newActivationState
                },
                new StaticSensor
                {
                    Id = 1,
                    IsActive = !newActivationState
                }
            };

            var fakeSensor = sensors[sensorIndex];
            var fakeSensorDbSet = new List<Sensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeSensorActivationStateCommand(fakeSensor.Id, newActivationState);
            var handler =
                new ChangeSensorActivationStateCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            Assert.Equal(newActivationState, fakeSensor.IsActive);
        }


        [Fact]
        public async Task Handler_database_should_be_updated()
        {
            var fakeSensor = new PortableSensor
            {
                Id = 1,
                IsActive = false
            };
            var fakeSensorDbSet = new List<Sensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeSensorActivationStateCommand(fakeSensor.Id, !fakeSensor.IsActive);
            var handler =
                new ChangeSensorActivationStateCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            _dataContextMock.Verify(x => x.SaveChangesAsync(It.Is<CancellationToken>(it => it == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handler_should_publish_notification()
        {
            var fakeSensor = new PortableSensor
            {
                Id = 1,
                IsActive = false
            };
            var fakeSensorDbSet = new List<Sensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new ChangeSensorActivationStateCommand(fakeSensor.Id, !fakeSensor.IsActive);
            var handler =
                new ChangeSensorActivationStateCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            await handler.Handle(command, cancellationToken);

            //Assert
            _mediatorMock.Verify(x =>
                    x.Publish(It.Is<SensorActivationStateChangedNotification>(it => it.SensorId == fakeSensor.Id),
                        It.Is<CancellationToken>(it => it == cancellationToken)),
                Times.Once());
        }
    }
}