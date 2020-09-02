using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Web.Areas.Admin.Application.Readings.Queries;
using Web.Areas.Admin.Application.Readings.Queries.DTO;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Xunit;

namespace Web.UnitTests.Areas.Admin.Application.Readings.Queries
{
    public class ReadingQueriesTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<DataContext> _dataContextMock;
        private readonly Mock<IDataContextFactory<DataContext>> _dataContextFactoryMock;

        public ReadingQueriesTest()
        {
            _mapperMock = new Mock<IMapper>();
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
                new[] {(object) null, _mapperMock.Object},
                new[] {_dataContextFactoryMock.Object, (object) null},
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var queires = new ReadingsQueries((IDataContextFactory<DataContext>) testArgsCurrentSet[0],
                    (IMapper) testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }


        [Fact]
        public async Task Get_All_Async_should_return_not_deleted_sensors_dto()
        {
            //Arrage
            var sensorsDbSet = new List<Sensor>
            {
                new StaticSensor {Id = 1},
                new StaticSensor {Id = 2, IsDeleted = true},
                new PortableSensor {Id = 3},
                new PortableSensor {Id = 4, IsDeleted = true}
            };

            var nonDeletedSensorIds = sensorsDbSet.Where(z => !z.IsDeleted).Select(z => z.Id).ToArray();

            _dataContextMock.Setup(x => x.Set<Sensor>()).ReturnsDbSet(sensorsDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<List<Sensor>, List<SensorDTO>>(It.IsAny<List<Sensor>>())).Returns(
                (List<Sensor> sensors) =>
                {
                    var staticSensorsDtos = sensors.OfType<StaticSensor>().Select(s => new StaticSensorDTO {Id = s.Id});
                    var portableSensorsDtos =
                        sensors.OfType<PortableSensor>().Select(s => new PortableSensorDTO {Id = s.Id});
                    return new List<SensorDTO>().Concat(staticSensorsDtos).Concat(portableSensorsDtos).ToList();
                });

            var queries = new ReadingsQueries(_dataContextFactoryMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetSensorsAsync();

            //Assert
            Assert.True(result.Select(p => p.Id).ToArray().SequenceEqual(nonDeletedSensorIds));
        }

        [Fact]
        public async Task Get_Sensor_By_Id_should_return_requested_sensor_dto()
        {
            //Arrange
            var fakeStaticSensor = new StaticSensor {Id = 1};
            var fakeStaticSensorDbSet = new List<Sensor> {fakeStaticSensor};
            var fakeStaticSensorDto = new StaticSensorDTO {Id = fakeStaticSensor.Id};

            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(fakeStaticSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<Sensor, SensorDTO>(fakeStaticSensor))
                .Returns(fakeStaticSensorDto);

            var queries = new ReadingsQueries(_dataContextFactoryMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetSensorByIdAsync(fakeStaticSensor.Id);

            //Arrange
            Assert.Equal(fakeStaticSensorDto, result);
        }

        [Fact]
        public async Task Get_Sensor_By_Id_should_return_null_if_database_is_empty()
        {
            //Arrange
            _dataContextMock.Setup(x => x.Sensors).ReturnsDbSet(new List<Sensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<Sensor, SensorDTO>(It.Is<Sensor>(it => it == null)))
                .Returns((() => null));

            var queries = new ReadingsQueries(_dataContextFactoryMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetSensorByIdAsync(1);

            //Arrange
            Assert.Null(result);
        }

        [Fact]
        public async Task Get_Static_Sensor_By_Id_should_return_null_if_database_is_empty()
        {
            //Arrange
            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(new List<StaticSensor>());
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x => x.Map<StaticSensor, StaticSensorDTO>(It.Is<StaticSensor>(it => it == null)))
                .Returns((() => null));

            var queries = new ReadingsQueries(_dataContextFactoryMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetStaticSensorByIdAsync(1);

            //Arrange
            Assert.Null(result);
        }


        [Fact]
        public async Task Get_Static_Sensor_By_Id_should_return_sensor_by_dto_with_requested_id()
        {
            //Arrange
            var fakeStaticSensor = new StaticSensor {Id = 1};
            var fakeStaticSensorDbSet = new List<StaticSensor> {fakeStaticSensor};

            _dataContextMock.Setup(x => x.StaticSensors).ReturnsDbSet(fakeStaticSensorDbSet);
            _dataContextFactoryMock.Setup(x => x.Create()).Returns(_dataContextMock.Object);
            _mapperMock.Setup(x =>
                    x.Map<StaticSensor, StaticSensorDTO>(It.Is<StaticSensor>(it => it.Id == fakeStaticSensor.Id)))
                .Returns(((StaticSensor v) => new StaticSensorDTO {Id = v.Id}));

            var queries = new ReadingsQueries(_dataContextFactoryMock.Object, _mapperMock.Object);

            //Act
            var result = await queries.GetStaticSensorByIdAsync(fakeStaticSensor.Id);

            //Arrange
            Assert.Equal(fakeStaticSensor.Id, result.Id);
        }
    }
}