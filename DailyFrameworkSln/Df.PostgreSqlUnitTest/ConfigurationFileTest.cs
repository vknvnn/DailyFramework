using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Df.PostgreSqlUnitTest
{
    [TestClass]
    public class ConfigurationFileTest
    {
        [TestMethod]
        public void GetConnectionDfTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            Assert.AreEqual(configuration["connection:dftest"], "Host=localhost;Database=dftest;Username=postgres;Password=12345^");
        }
    }
}
