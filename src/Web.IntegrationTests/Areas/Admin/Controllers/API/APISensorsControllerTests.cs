using System.Threading.Tasks;
using Xunit;

namespace Web.IntegrationTests.Areas.Admin.Controllers.API
{
    [Collection("Integration: Sequential")]
    public class APISensorsControllerTests : BaseScenario
    {
        
        [Fact]
        public async Task Get_get_all_sensors_and_response_ok_status_code()
        {
            //Arrange

            using var server = new TestServerBuilder()
                .Build();
            var client = server.CreateClient();

            //Act
            var response = await client.GetAsync("api/admin/sensors");

            //Assert
            response.EnsureSuccessStatusCode();
        }
    }
}