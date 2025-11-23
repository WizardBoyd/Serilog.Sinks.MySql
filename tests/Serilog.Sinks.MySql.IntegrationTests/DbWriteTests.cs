using MySqlConnector;
using Serilog.Context;
using Serilog.Sinks.MySql.IntegrationTests.Objects;
using Serilog.Sinks.MySql.MySql;
using Serilog.Sinks.MySql.MySql.ColumnWriters;

namespace Serilog.Sinks.MySql.IntegrationTests
{
    [TestClass]
    public class DbWriteTests : BaseTests
    {
        private readonly DbHelper databaseHelper = new(ConnectionString);

        [TestMethod]
        public async Task AutoCreateTableIsTrueShouldCreateTableInsert()
        {
            const string TableName = "Logs1";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
            var testObject2 = new TestObjectType2 { DateProp = DateTime.Now, NestedProp = testObject };

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "RaiseDate", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() },
            { "PropertyTest", new PropertiesColumnWriter(MySqlDbType.Text) },
            {
                "IntPropertyTest",
                new SinglePropertyColumnWriter("TestNo", PropertyWriteMethod.Raw, MySqlDbType.Int32)
            },
            { "MachineName", new SinglePropertyColumnWriter("MachineName", format: "l") }
        };

            var logger = new LoggerConfiguration().WriteTo.MySql(
                ConnectionString,
                TableName,
                columnProps,
                needAutoCreateTable: true,
                useBulkInsert: false).Enrich.WithMachineName().CreateLogger();

            const long RowsCount = 1;

            for (var i = 0; i < RowsCount; i++)
            {
                logger.Information(
                    "Test{TestNo}: {@TestObject} test2: {@TestObject2} testStr: {@TestStr:l}",
                    i,
                    testObject,
                    testObject2,
                    "stringValue");
            }

            Log.CloseAndFlush();
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }


        [TestMethod]
        public async Task AutoCreateTableIsTrueShouldCreateTableCopy()
        {
            const string TableName = "Logs2";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
            var testObject2 = new TestObjectType2 { DateProp = DateTime.Now, NestedProp = testObject };

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "RaiseDate", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() },
            { "PropertyTest", new PropertiesColumnWriter(MySqlDbType.Text) },
            {
                "IntPropertyTest",
                new SinglePropertyColumnWriter("TestNo", PropertyWriteMethod.Raw, MySqlDbType.Int32)
            },
            { "MachineName", new SinglePropertyColumnWriter("MachineName", format: "l") }
        };

            var logger = new LoggerConfiguration().WriteTo
                .MySql(ConnectionString, TableName, columnProps, needAutoCreateTable: true).Enrich
                .WithMachineName().CreateLogger();

            const long RowsCount = 1;

            for (var i = 0; i < RowsCount; i++)
            {
                logger.Information(
                    "Test{TestNo}: {@TestObject} test2: {@TestObject2} testStr: {@TestStr:l}",
                    i,
                    testObject,
                    testObject2,
                    "stringValue");
            }

            Log.CloseAndFlush();
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }



        [TestMethod]
        public async Task PropertyForSinglePropertyColumnWriterDoesNotExistsWithInsertStatementsShouldInsertEventToDb()
        {
            const string TableName = "Logs3";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "RaiseDate", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() },
            { "PropertyTest", new PropertiesColumnWriter(MySqlDbType.Text) },
            { "MachineName", new SinglePropertyColumnWriter("MachineName", format: "l") }
        };

            await this.databaseHelper.CreateTable(string.Empty, TableName, columnProps);

            var logger = new LoggerConfiguration().WriteTo
                .MySql(ConnectionString, TableName, columnProps, useBulkInsert: false).CreateLogger();

            logger.Information("Test: {@TestObject} testStr: {@TestStr:l}", testObject, "stringValue");

            Log.CloseAndFlush();
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(1, actualRowsCount);
        }


        [TestMethod]
        public async Task QuotedColumnNamesWithInsertStatementsShouldInsertEventToDb()
        {
            const string TableName = "Logs4";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "`MessageTemplate`", new MessageTemplateColumnWriter() },
            { "`Level`", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "RaiseDate", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() },
            { "PropertyTest", new PropertiesColumnWriter(MySqlDbType.Text) }
        };

            await this.databaseHelper.CreateTable(string.Empty, TableName, columnProps);

            var logger = new LoggerConfiguration().WriteTo
                .MySql(ConnectionString, TableName, columnProps, useBulkInsert: false).CreateLogger();

            logger.Information("Test: {@TestObject} testStr: {@TestStr:l}", testObject, "stringValue");

            Log.CloseAndFlush();
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(1, actualRowsCount);
        }



        [TestMethod]
        public async Task Write50EventsShouldInsert50EventsToDb()
        {
            const string TableName = "Logs5";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
            var testObject2 = new TestObjectType2 { DateProp = DateTime.Now, NestedProp = testObject };

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "RaiseDate", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() },
            { "PropertyTest", new PropertiesColumnWriter(MySqlDbType.Text) },
            { "MachineName", new SinglePropertyColumnWriter("MachineName", format: "l") }
        };

            await this.databaseHelper.CreateTable(string.Empty, TableName, columnProps);

            var logger = new LoggerConfiguration().WriteTo.MySql(ConnectionString, TableName, columnProps).Enrich
                .WithMachineName().CreateLogger();

            const long RowsCount = 50;

            for (var i = 0; i < RowsCount; i++)
            {
                logger.Information(
                    "Test{TestNo}: {@TestObject} test2: {@TestObject2} testStr: {@TestStr:l}",
                    i,
                    testObject,
                    testObject2,
                    "stringValue");
            }

            Log.CloseAndFlush();
            await Task.Delay(10000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }


        [TestMethod]
        public async Task WriteEventWithZeroCodeCharInJsonShouldInsertEventToDb()
        {
            const string TableName = "Logs6";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test\\u0000" };

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "RaiseDate", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() },
            { "PropertyTest", new PropertiesColumnWriter(MySqlDbType.Text) }
        };

            await this.databaseHelper.CreateTable(string.Empty, TableName, columnProps);

            var logger = new LoggerConfiguration().WriteTo.MySql(ConnectionString, TableName, columnProps)
                .CreateLogger();

            logger.Information("Test: {@TestObject} testStr: {@TestStr:l}", testObject, "stringValue");

            Log.CloseAndFlush();
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(1, actualRowsCount);
        }


        [TestMethod]
        public async Task AuditSinkNoSchemaShouldInsertLog()
        {
            const string TableName = "Logs7";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
            var testObject2 = new TestObjectType2 { DateProp = DateTime.Now, NestedProp = testObject };

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "RaiseDate", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new LogEventSerializedColumnWriter() },
            { "PropertyTest", new PropertiesColumnWriter(MySqlDbType.Text) },
            {
                "IntPropertyTest",
                new SinglePropertyColumnWriter("testNo", PropertyWriteMethod.Raw, MySqlDbType.Int32)
            },
            { "MachineName", new SinglePropertyColumnWriter("MachineName", format: "l") }
        };

            var logger = new LoggerConfiguration().AuditTo
                .MySql(ConnectionString, TableName, columnProps, needAutoCreateTable: true).Enrich
                .WithMachineName().CreateLogger();

            const long RowsCount = 10;

            for (var i = 0; i < RowsCount; i++)
            {
                logger.Information(
                    "Test{TestNo}: {@TestObject} test2: {@TestObject2} testStr: {@TestStr:l}",
                    10,
                    testObject,
                    testObject2,
                    "stringValue");
            }

            Log.CloseAndFlush();
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }


        [TestMethod]
        public async Task IncorrectDatabaseConnectionStringNoSchemaLogShouldThrowException()
        {
            const string TableName = "Logs8";

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
            var testObject2 = new TestObjectType2 { DateProp = DateTime.Now, NestedProp = testObject };

            var columnProps = new Dictionary<string, ColumnWriterBase>();

            var invalidConnectionString = ConnectionString.Replace("Database=", "Database=A");
            var logger = new LoggerConfiguration().AuditTo
                .MySql(invalidConnectionString, TableName, columnProps, needAutoCreateTable: true).Enrich
                .WithMachineName().CreateLogger();

            logger.Information(
                "Test{TestNo}: {@TestObject} test2: {@TestObject2} testStr: {@TestStr:l}",
                1,
                testObject,
                testObject2,
                "stringValue");
            Log.CloseAndFlush();
#pragma warning disable MSTEST0039 // Use newer methods to assert exceptions
            await Assert.ThrowsExceptionAsync<MySqlConnector.MySqlException>(async () => await Task.Delay(1000));
#pragma warning restore MSTEST0039 // Use newer methods to assert exceptions
        }


        [TestMethod]
        public async Task AutoCreateTableIsTrueShouldCreateTableInsertWithOrders()
        {
            const string TableName = "Logs9";
            await this.databaseHelper.RemoveTable(TableName);

            var testObject = new TestObjectType1 { IntProp = 42, StringProp = "Test" };
            var testObject2 = new TestObjectType2 { DateProp = DateTime.Now, NestedProp = testObject };

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter(order: 8) },
            { "MessageTemplate", new MessageTemplateColumnWriter(order: 1) },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text, 2) },
            { "RaiseDate", new TimestampColumnWriter(order: 3) },
            { "Exception", new ExceptionColumnWriter(order: 4) },
            { "Properties", new LogEventSerializedColumnWriter(order: 5) },
            { "PropertyTest", new PropertiesColumnWriter(MySqlDbType.Text, 6) },
            {
                "IntPropertyTest",
                new SinglePropertyColumnWriter("TestNo", PropertyWriteMethod.Raw, MySqlDbType.Int32, order: 7)
            },
            { "MachineName", new SinglePropertyColumnWriter("MachineName", format: "l", order: 0) }
        };

            var logger = new LoggerConfiguration().WriteTo.MySql(
                ConnectionString,
                TableName,
                columnProps,
                needAutoCreateTable: true,
                useBulkInsert: false).Enrich.WithMachineName().CreateLogger();

            const long RowsCount = 1;

            for (var i = 0; i < RowsCount; i++)
            {
                logger.Information(
                    "Test{TestNo}: {@TestObject} test2: {@TestObject2} testStr: {@TestStr:l}",
                    i,
                    testObject,
                    testObject2,
                    "stringValue");
            }

            Log.CloseAndFlush();
            await Task.Delay(1000);
            var actualRowsCount = await this.databaseHelper.GetTableRowsCount(string.Empty, TableName);
            Assert.AreEqual(RowsCount, actualRowsCount);
        }


        [TestMethod]
        public async Task TestIssue32()
        {
            const string TableName = "Logs10";
            await this.databaseHelper.RemoveTable(TableName);

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter(MySqlDbType.Text) },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "TimeStamp", new TimestampColumnWriter(MySqlDbType.Timestamp) },
            { "Exception", new ExceptionColumnWriter(MySqlDbType.Text) },
            { "UserName", new SinglePropertyColumnWriter("UserName", PropertyWriteMethod.ToString, MySqlDbType.Text) },
        };

            var logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.MySql(
                ConnectionString,
                TableName,
                columnProps,
                needAutoCreateTable: true
              ).CreateLogger();

            LogContext.PushProperty("UserName", "Hans");

            logger.Information("A test error occured.");

            Log.CloseAndFlush();
            await Task.Delay(1000);
        }


        [TestMethod]
        public async Task TestIssue52()
        {
            const string TableName = "Logs11";
            await this.databaseHelper.RemoveTable(TableName);

            var columnProps = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter(MySqlDbType.Text) },
            { "Level", new LevelColumnWriter(true, MySqlDbType.Text) },
            { "TimeStamp", new TimestampColumnWriter(MySqlDbType.Timestamp) },
            { "Exception", new ExceptionColumnWriter(MySqlDbType.Text) },
            { "EnumTest", new SinglePropertyColumnWriter("EnumTest", PropertyWriteMethod.Raw, MySqlDbType.Int32) }
        };

            var logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.MySql(
                ConnectionString,
                TableName,
                columnProps,
                needAutoCreateTable: true
              ).CreateLogger();

            LogContext.PushProperty("EnumTest", DummyEnum.Test2);

            logger.Information("A test error occured.");

            Log.CloseAndFlush();
            await Task.Delay(1000);
        }
    }
}