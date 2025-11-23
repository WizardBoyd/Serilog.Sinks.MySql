

using MySqlConnector;
using Serilog.Sinks.MySql.MySql;
using Serilog.Sinks.MySql.MySql.ColumnWriters;
using System.Text;

namespace Serilog.Sinks.MySql.IntegrationTests
{
    public sealed class DbHelper
    {
        private readonly string connectionString;

        public DbHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task ClearTable(string schemaName, string tableName)
        {
            
            tableName = tableName.Replace("`", string.Empty);

            var builder = new StringBuilder();
            builder.Append("TRUNCATE ");

            builder.Append("`");
            builder.Append(tableName);
            builder.Append("`;");

            using var connection = new MySqlConnection(this.connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = builder.ToString();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<long> GetTableRowsCount(string schemaName, string tableName)
        {
            tableName = tableName.Replace("`", string.Empty);

            var builder = new StringBuilder();
            builder.Append("SELECT count(*) FROM ");


            builder.Append('`');
            builder.Append(tableName);
            builder.Append("`;");

            using var connection = new MySqlConnection(this.connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = builder.ToString();
            var result = await command.ExecuteScalarAsync();
            return (long?)result ?? 0;
        }

        public async Task RemoveTable(string tableName)
        {
            
            tableName = tableName.Replace("`", string.Empty);

            var builder = new StringBuilder();
            builder.Append("DROP TABLE IF EXISTS ");

          

            builder.Append('`');
            builder.Append(tableName);
            builder.Append("`;");

            using var connection = new MySqlConnection(this.connectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = builder.ToString();
            await command.ExecuteNonQueryAsync();
        }

        public async Task CreateTable(string schemaName, string tableName, IDictionary<string, ColumnWriterBase> columnsInfo)
        {
            
            tableName = tableName.Replace("`", string.Empty);
            using var connection = new MySqlConnection(this.connectionString);
            await connection.OpenAsync();
            await TableCreator.CreateTable(connection, tableName, ClearQuotationMarksFromColumnOptions(columnsInfo));
        }

        private static IDictionary<string, ColumnWriterBase> ClearQuotationMarksFromColumnOptions(
        IDictionary<string, ColumnWriterBase> columnOptions)
        {
            var result = new Dictionary<string, ColumnWriterBase>(columnOptions);

            foreach (var keyValuePair in columnOptions)
            {
                if (!keyValuePair.Key.Contains('`'))
                {
                    continue;
                }

                result.Remove(keyValuePair.Key);
                result[keyValuePair.Key.Replace("`", string.Empty)] = keyValuePair.Value;
            }

            return result;
        }

    }
}
