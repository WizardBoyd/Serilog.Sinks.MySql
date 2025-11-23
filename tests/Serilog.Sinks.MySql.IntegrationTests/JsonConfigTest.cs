

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Sinks.MySql.IntegrationTests.Objects;

namespace Serilog.Sinks.MySql.IntegrationTests
{
    [TestClass]
    public sealed class JsonConfigTest : BaseTests
    {
        private readonly DbHelper databaseHelper = new(ConnectionString);


        [TestMethod]
        public async Task ShouldCreateLoggerFromConfig()
        {
            const string TableName = "ConfigLogs1";


            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(".\\MySqlSinkConfiguration.json", false, true)
                .Build();

            const long RowsCount = 2;
            await using (Logger logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger())
            {
                for (var i = 0; i < RowsCount; i++)
                {
                    logger.Information(
                        "{@LogEvent} {TestProperty}",
                        testObject,
                        "TestValue");
                }
            }
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }


        [TestMethod]
        public async Task ShouldCreateLoggerFromConfigWithLevelAsText()
        {
            const string TableName = "ConfigLogs2";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(".\\MySqlSinkConfiguration.Level.json", false, true)
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            const long RowsCount = 2;

            for (var i = 0; i < RowsCount; i++)
            {
                logger.Information(
                    "{@LogEvent} {TestProperty}",
                    testObject,
                    "TestValue");
            }

            Log.CloseAndFlush();
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }

        [TestMethod]
        public async Task ShouldCreateLoggerFromConfigWithOrderedColumns()
        {
            const string TableName = "ConfigLogs3";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(".\\MySqlSinkConfigurationOrderedColumns.json", false, true)
                .Build();

            const long RowsCount = 2;
            await using (Logger logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger())
            {
                for (var i = 0; i < RowsCount; i++)
                {
                    logger.Information(
                        "{@LogEvent} {TestProperty}",
                        testObject,
                        "TestValue");
                }
            }
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }

    }
}
