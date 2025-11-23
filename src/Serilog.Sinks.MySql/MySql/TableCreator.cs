

using MySqlConnector;
using Serilog.Sinks.MySql.MySql.ColumnWriters;
using System.Text;

namespace Serilog.Sinks.MySql.MySql
{
    public static class TableCreator
    {
        public static async Task CreateTable(
           MySqlConnection connection,
           string tableName,
           IDictionary<string, ColumnWriterBase> columnsInfo)
        {
            using var command = connection.CreateCommand();
            command.CommandText = GetCreateTableQuery(tableName, columnsInfo);
            await command.ExecuteNonQueryAsync();
        }


        private static string GetCreateTableQuery(string tableName, IDictionary<string, ColumnWriterBase> columnsInfo)
        {
            tableName = tableName.Replace("`", string.Empty);

            var builder = new StringBuilder("CREATE TABLE IF NOT EXISTS ");

            builder.Append('`');
            builder.Append(tableName);
            builder.Append('`');
            builder.AppendLine(" (");

            builder.AppendLine(
                string.Join(",\n", columnsInfo.Select(r => $" `{r.Key}` {r.Value.GetSqlType()}")));

            builder.AppendLine(");");

            return builder.ToString();
        }
    }
}
