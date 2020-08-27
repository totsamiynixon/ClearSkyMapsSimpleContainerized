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
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Xunit;

namespace Web.UnitTests.Areas.Admin.Application.Readings.Commands
{
    public class DeleteSensorCommandHandlerTest
    {
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;
        private readonly Mock<IMediator> _mediatorMock;

        public DeleteSensorCommandHandlerTest()
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
                var command = new DeleteSensorCommandHandler((IDataContextFactory<DataContext>) testArgsCurrentSet[0],
                    (IMediator) testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Handler_should_throw_not_found_exception_if_sensor_doesnt_exist()
        {
            //Arrange
            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new DeleteSensorCommand(1, false);
            var handler =
                new DeleteSensorCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

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
        //TODO: think more about this test
        public async Task Handler_should_return_true(int dataSetIndex, bool isCompletely)
        {
            var sensors = new List<Sensor>()
            {
                new PortableSensor
                {
                    Id = 1,
                },
                new StaticSensor
                {
                    Id = 1,
                }
            };
            var fakeSensor = sensors[dataSetIndex];
            var fakeSensorDbSet = new List<Sensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new DeleteSensorCommand(fakeSensor.Id, isCompletely);
            var handler =
                new DeleteSensorCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        //TODO: think more about this test
        public async Task Handler_should_set_is_removed_to_true_if_not_removing_completely(int dataSetIndex)
        {
            //Arrange
            var sensors = new List<Sensor>()
            {
                new PortableSensor
                {
                    Id = 1,
                },
                new StaticSensor
                {
                    Id = 1,
                }
            };
            var fakeSensor = sensors[dataSetIndex];
            var fakeSensorDbSet = new List<Sensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new DeleteSensorCommand(fakeSensor.Id, false);
            var handler =
                new DeleteSensorCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            Assert.True(fakeSensor.IsDeleted);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        //TODO: think more about this test
        public async Task Handler_should_call_remove_if_removing_completely(int dataSetIndex)
        {
            //Arrange
            var sensors = new List<Sensor>()
            {
                new PortableSensor
                {
                    Id = 1,
                },
                new StaticSensor
                {
                    Id = 1,
                }
            };
            var fakeSensor = sensors[dataSetIndex];
            var fakeSensorDbSet = new List<Sensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new DeleteSensorCommand(fakeSensor.Id, true);
            var handler =
                new DeleteSensorCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            _dataContextMock.Verify(x => x.Sensors.Remove(It.Is<Sensor>(it => it == fakeSensor)), Times.Once);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData(1, false)]
        //TODO: think more about this test
        public async Task Handler_should_update_database_in_any_case(int dataSetIndex, bool isCompletely)
        {
            //Arrange
            var sensors = new List<Sensor>()
            {
                new PortableSensor
                {
                    Id = 1,
                },
                new StaticSensor
                {
                    Id = 1,
                }
            };
            var fakeSensor = sensors[dataSetIndex];
            var fakeSensorDbSet = new List<Sensor> {fakeSensor};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);

            var cancellationToken = new CancellationToken();
            var command = new DeleteSensorCommand(fakeSensor.Id, isCompletely);
            var handler =
                new DeleteSensorCommandHandler(_dataContextFactoryMock.Object, _mediatorMock.Object);

            //Act
            var result = await handler.Handle(command, cancellationToken);

            //Assert
            _dataContextMock.Verify(x => x.SaveChangesAsync(It.Is<CancellationToken>(it => it == cancellationToken)),
                Times.Once);
        }
    }
}