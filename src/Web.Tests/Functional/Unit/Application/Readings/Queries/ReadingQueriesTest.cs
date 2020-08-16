using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Web.Application.Readings.Queries;
using Web.Application.Readings.Queries.DTO;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Helpers;
using Web.Helpers.Interfaces;
using Web.Models.Cache;
using Xunit;

namespace Web.Tests.Functional.Unit.Application.Readings.Queries
{
    public class ReadingQueriesTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISensorCacheHelper> _sensorCacheHelperMock;

        public ReadingQueriesTest()
        {
            _mapperMock = new Mock<IMapper>();
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
                new[] { _sensorCacheHelperMock.Object, (object) null},
                new[] {(object) null, (object) _mapperMock.Object}
            };

            var testArgsCurrentSet = testArgs[paramsSetIndex];

            //Act
            void Act()
            {
                var readingsQueries = new ReadingsQueries((ISensorCacheHelper) testArgsCurrentSet[0], (IMapper) testArgsCurrentSet[1]);
            }

            //Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public async Task Get_Static_Sensors_Success()
        {
            //Arrange
            var fakeApiKey = CryptoHelper.GenerateApiKey();
            var fakeStaticSensorReading = new StaticSensorReading {Id = 1, StaticSensorId = 1};
            var fakeStaticSensor = new StaticSensor
            {
                Id = 1,
                ApiKey = fakeApiKey,
                Readings = new List<StaticSensorReading> {fakeStaticSensorReading}
            };
            var fakeStaticSensorCacheItem =
                new SensorCacheItemModel(fakeStaticSensor, PollutionLevel.High);
            var fakeStaticSensorReadingsDTO = new StaticSensorReadingDTO {Id = 1};
            var fakeStaticSensorReadingsDTOList = new List<StaticSensorReadingDTO> {fakeStaticSensorReadingsDTO};
            var fakeStaticSensorsCacheList = new List<SensorCacheItemModel> {fakeStaticSensorCacheItem};

            _sensorCacheHelperMock.Setup(x => x.GetStaticSensorsAsync())
                .Returns(Task.FromResult(fakeStaticSensorsCacheList));
            _mapperMock.Setup((x =>
                    x.Map<List<StaticSensorReading>, List<StaticSensorReadingDTO>>(fakeStaticSensor.Readings)))
                .Returns(fakeStaticSensorReadingsDTOList);

            var readingQueries = new ReadingsQueries(_sensorCacheHelperMock.Object, _mapperMock.Object);

            //Act
            var result = await readingQueries.GetStaticSensorsAsync();

            //Assert
            Assert.NotNull(result.First(x => x.Id == fakeStaticSensor.Id && x.Readings == fakeStaticSensorReadingsDTOList));
        }
    }
}