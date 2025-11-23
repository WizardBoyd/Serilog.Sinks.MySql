

using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog.Sinks.MySql.IntegrationTests.Objects;

namespace Serilog.Sinks.MySql.IntegrationTests
{
    [TestClass]
    public sealed class JsonConfigTestNamedConnectionString : BaseTests
    {
        private const string TableName = "TestLogsNamedConnectionString";

        private readonly DbHelper dbHelper = new(ConnectionString);

        [TestMethod]
        public async Task ShouldCreateLoggerFromConfig()
        {
            await this.dbHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(".\\MySqlSinkConfigurationConnectionString.json", false, true)
                .Build();

            var logger = new LoggerConfiguration().WriteTo.MySql(
                ConnectionString,
                TableName,
                null,
                LogEventLevel.Verbose,
                null,
                null,
                30,
                1000,
                null,
                false,
                true,
                configuration).CreateLogger();

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
            var actualRowsCount = await this.dbHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }
    }
}
