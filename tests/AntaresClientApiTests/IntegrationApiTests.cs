using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.Extensions;
using Xunit;

namespace AntaresClientApiTests
{
    public class IntegrationApiTests
    {
        public IntegrationApiTests()
        {
        }

        [Fact]
        public async Task PasswordHash()
        {
            var pass = "Qwer123456";
            var expected = "ab4a4ae8914b08326f4f8253434b140263cf3765e8d7722528a5249423ebe53c";
            var actual = pass.ToSha256();
            Assert.Equal(expected, actual);
        }
    }
}
