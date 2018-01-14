using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Df.JsonConfiguration
{
    public interface IJsonConfig
    {
        string GetConnection(string key);
    }

    public class JsonConfig: IJsonConfig
    {
        private readonly IConfigurationRoot _configuration;
        public JsonConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            _configuration = builder.Build();
        }

        public string GetConnection(string key)
        {
            return _configuration[$"connection:{key}"];
        }
    }
}
