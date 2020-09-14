﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Domain.Entities;
using Web.Helpers;
using Xunit;

namespace Web.IntegrationTests.Controllers.API
{
    [Collection("Integration: Sequential")]
    public class IntegrationControllerTest : BaseScenario
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task Send_reading_and_response_ok_status_code(int dataSetIndex)
        {
            //Arrange
            var latitude = 53.333333;
            var longitude = 53.333333;

            var dataSet = new List<Sensor>
            {
                new StaticSensor
                {
                    ApiKey = CryptoHelper.GenerateApiKey(),
                    Latitude = latitude, 
                    Longitude = longitude,
                    IsActive = true, 
                    IsVisible = true,
                    Readings = new List<StaticSensorReading>()
                },
                new PortableSensor
                {
                    ApiKey = CryptoHelper.GenerateApiKey(),
                    IsActive = true
                }
            };
            var currentSensor = dataSet[dataSetIndex];
            
            using var server = new TestServerBuilder()
                .UseSensors(currentSensor)
                .Build();
            var client = server.CreateClient();

            //Act
            var response =
                await client.GetAsync(
                    $"api/integration/getaspost?data={WebUtility.UrlEncode(GenerateData(currentSensor.ApiKey, latitude, longitude))}");

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "123")]
        [InlineData("123", null)]
        public async Task Send_reading_and_response_bad_request_status_code(string _apiKey, string _data)
        {
            //Arrange
            var apiKey = _apiKey ?? string.Empty;
            var data = _data ?? string.Empty;

            var dataQuery = new List<string> {apiKey, data}.Where(z => !string.IsNullOrEmpty(z)).ToArray();
            
            using var server = new TestServerBuilder()
                .Build();
            var client = server.CreateClient();

            //Act
            var response =
                await client.GetAsync(
                    $"api/integration/getaspost?data={string.Join(",", dataQuery)}");

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private string GenerateData(string apiKey, double latitude, double longitude)
        {
            var random = new Random();

            var temp = (float) Math.Round((float) random.NextDouble() * 350, 3);
            var co2 = (float) Math.Round((float) random.NextDouble() * 350, 3);
            var lpg = (float) Math.Round((float) random.NextDouble() * 350, 3);
            var co = (float) Math.Round((float) random.NextDouble() * 4, 3);
            var ch4 = (float) Math.Round((float) random.NextDouble() * 0.716, 3);
            var dust = (float) Math.Round((float) random.NextDouble() * 350, 3);
            var preassure = (float) Math.Round((float) random.NextDouble() * 20, 3);
            var hum = (float) Math.Round((float) random.NextDouble() * 40, 3);

            return
                @$"{string.Join(",", apiKey, temp.ToString(CultureInfo.InvariantCulture), hum.ToString(CultureInfo.InvariantCulture),
                    preassure.ToString(CultureInfo.InvariantCulture), co2.ToString(CultureInfo.InvariantCulture),
                    lpg.ToString(CultureInfo.InvariantCulture), co.ToString(CultureInfo.InvariantCulture),
                    ch4.ToString(CultureInfo.InvariantCulture), dust.ToString(CultureInfo.InvariantCulture),
                    latitude.ToString(CultureInfo.InvariantCulture), longitude.ToString(CultureInfo.InvariantCulture))};";
        }
    }
}