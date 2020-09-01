using System.IO;
using AntaresClientApi.Common.Configuration;
using Newtonsoft.Json;

namespace AntaresClientApiTests
{
    public class TestConfig
    {
        public static AppConfig Load()
        {
            using (var reader = new StreamReader("appsettings.json"))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<AppConfig>(json);
            }
        }

    }
}
