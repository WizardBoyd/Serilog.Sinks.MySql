

using MySqlConnector;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.MySql.MySql.ColumnWriters;
using System;
using System.Data;
using System.Net.NetworkInformation;
using System.Text;

namespace Serilog.Sinks.MySql.MySql
{
    public sealed class SinkHelper
    {
        private bool IsTableCreated;
        private bool IsHeatBeatDetected;

        public SinkHelper(MySqlOptions options)
        {
            this.SinkOptions = options;
            this.IsTableCreated = !options.NeedAutoCreateTable;
        }

        public MySqlOptions SinkOptions { get; set; }

        public async Task Emit(IEnumerable<LogEvent> events)
        {
            string connectionString = string.Empty;
            if (this.SinkOptions.UseBulkInsert)
            {
                connectionString = GetConnectionStringWithLocalInFile(true);
            }
            else
            {
                connectionString = SinkOptions.ConnectionString;
            }

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

#if DEBUG
            connection.InfoMessage += (s, e) =>
            {
                foreach (var error in e.Errors)
                    Console.WriteLine(error.Message);
            };
#endif

            if(SinkOptions.NeedAutoCreateTable && !IsTableCreated && !string.IsNullOrEmpty(SinkOptions.TableName))
            {
                if(SinkOptions.ColumnOptions.All(c => c.Value.Order is not null))
                {
                    if(SinkOptions.OnCreateTable is null)
                    {
                        var columnOptions = SinkOptions.ColumnOptions.OrderBy(c => c.Value.Order)
                            .ToDictionary(c => c.Key, x => x.Value);
                        await TableCreator.CreateTable(connection, SinkOptions.TableName, columnOptions);
                    }
                    else
                    {
                        SinkOptions.OnCreateTable.Invoke(new EventArgs.CreateTableEventArgs());
                    }
                }
                else
                {
                    if(SinkOptions.OnCreateTable is null)
                    {
                        await TableCreator.CreateTable(connection, SinkOptions.TableName, SinkOptions.ColumnOptions);
                    }
                    else
                    {
                        SinkOptions.OnCreateTable.Invoke(new EventArgs.CreateTableEventArgs());
                    }
                }
                IsTableCreated = true;
            }

            if (this.SinkOptions.UseBulkInsert)
            {
                await ProcessEventsByBulkInsertStatements(events, connection);
            }
            else
            {
                await ProcessEventsByInsertStatements(events, connection);
            }

            if(SinkOptions.RetentionTime is not null && SinkOptions.RetentionTime > TimeSpan.Zero)
            {
                await DeleteOldLogEvents(connection);
            }
        }


        public async Task EmptyEmit()
        {
            IsHeatBeatDetected = await HeartBeatAsync();
        }

        private async Task<bool> HeartBeatAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(SinkOptions.ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT 1";
                        await command.ExecuteScalarAsync();
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private async Task DeleteOldLogEvents(MySqlConnection connection)
        {
            var cutoffDate = DateTime.UtcNow - this.SinkOptions.RetentionTime!;
            var sql = this.GetDeleteQuery();
            await using var command = new MySqlCommand(sql, connection: connection);
            command.Parameters.AddWithValue("@cutoffDate", cutoffDate);
            var deletedRows = await command.ExecuteNonQueryAsync();
            SelfLog.WriteLine($"Deleted {deletedRows} log entries older than {cutoffDate}.");
        }


        private async Task ProcessEventsByInsertStatements(IEnumerable<LogEvent> events, MySqlConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = GetInsertQuery();

            foreach (var logEvent in events)
            {
                command.Parameters.Clear();
                foreach (var columKey in ColumnNamesWithoutSkipped())
                {
                    var paramName = ClearColumnNameForParameterName(columKey);
                    var dbType = SinkOptions.ColumnOptions[columKey].DbType;
                    var value = SinkOptions.ColumnOptions[columKey].GetValue(logEvent, SinkOptions.FormatProvider) ?? DBNull.Value;
                    var parameter = command.Parameters.Add(paramName, dbType);
                    parameter.Value = value;
                }
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task ProcessEventsByBulkInsertStatements(IEnumerable<LogEvent> events, MySqlConnection connection)
        {
            var bulkCopy = new MySqlBulkCopy(connection);
            var table = GenerateDataTableForBulkInsert(events, bulkCopy);
            bulkCopy.DestinationTableName = SinkOptions.TableName;
            var result = await bulkCopy.WriteToServerAsync(table);
            if (SinkOptions.EmitBulkWarnings)
            {
                SelfLog.WriteLine($"Problem with bulk insert only inserted {result.RowsInserted}");
                foreach (var error in result.Warnings)
                {
                    SelfLog.WriteLine($"Code: {error.ErrorCode.ToString()} message: {error.Message}");
                }
                SelfLog.WriteLine(string.Empty);
            }
        }

        private static string ClearColumnNameForParameterName(string columnName)
        {
            return columnName?.Replace("`", string.Empty) ?? string.Empty;
        }

        private IEnumerable<string> ColumnNamesWithoutSkipped()
        {
            return this.SinkOptions
                .ColumnOptions.Where(c => !c.Value.SkipOnInsert)
                .Select(c => c.Key);
        }

        private IEnumerable<ColumnWriterBase> ColumnWritersWithoutSkipped()
        {
            return this.SinkOptions
                .ColumnOptions.Where(c => !c.Value.SkipOnInsert)
                .Select(c => c.Value);
        }

        private string GetInsertQuery()
        {
            var columns = "`" + string.Join("`, `", this.ColumnNamesWithoutSkipped()) + "`";

            var parameters = string.Join(
                ", ",
                this.ColumnNamesWithoutSkipped().Select(cn => "@" + ClearColumnNameForParameterName(cn)));

            var builder = new StringBuilder();
            builder.Append("INSERT INTO ");

            builder.Append('`');
            builder.Append(this.SinkOptions.TableName);
            builder.Append('`');

            builder.Append(" (");
            builder.Append(columns);
            builder.Append(") VALUES (");
            builder.Append(parameters);
            builder.Append(");");
            return builder.ToString();
        }

        private string GetDeleteQuery()
        {
            var timestampColumnName = this.SinkOptions.ColumnOptions.FirstOrDefault(c => c.Value is TimestampColumnWriter).Key;

            if (string.IsNullOrWhiteSpace(timestampColumnName))
            {
                throw new ArgumentException("No timestamp column found.");
            }

            var builder = new StringBuilder();
            builder.Append("DELETE FROM ");

            builder.Append('`');
            builder.Append(this.SinkOptions.TableName);
            builder.Append('`');

            builder.Append(" WHERE ");
            builder.Append(timestampColumnName);
            builder.Append(" < @cutoffDate;");
            return builder.ToString();
        }

        private string GetConnectionStringWithLocalInFile(bool value = true)
        {
            string connectionString = SinkOptions.ConnectionString;
            if(connectionString.IndexOf("AllowLoadLocalInfile=", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                connectionString = System.Text.RegularExpressions.Regex.Replace(
                            connectionString,
                            @"(?i)AllowLoadLocalInfile\s*=\s*[^;]*",
                            $"AllowLoadLocalInfile={value}");
            }
            else
            {
                if (!connectionString.EndsWith(";"))
                {
                    connectionString += ";";
                }
                connectionString += $"AllowLoadLocalInfile={value}";
            }
            return connectionString;
        }


        private DataTable GenerateDataTable(IEnumerable<LogEvent> events, Dictionary<string, ColumnWriterBase> columns)
        {
            DataTable dt = new DataTable();
            foreach (var kvp in columns)
            {
                if (!kvp.Value.SkipOnInsert)
                {
                    dt.Columns.Add(kvp.Key, kvp.Value.GetNetType());
                }
            }

            List<object?> parameters = new List<object?>();
            foreach (var logEvent in events)
            {
                foreach (var kvp in columns)
                {
                    if (!kvp.Value.SkipOnInsert)
                    {
                        parameters.Add(kvp.Value.GetValue(logEvent, SinkOptions.FormatProvider));
                    }
                }
                dt.Rows.Add(parameters.ToArray());
                parameters.Clear();
            }
            return dt;
        }

        private DataTable GenerateDataTableForBulkInsert(IEnumerable<LogEvent> events, MySqlBulkCopy sqlBulkCopy)
        {
            var columnOptions = SinkOptions.ColumnOptions.OrderBy(c => c.Value.Order)
                           .ToDictionary(c => c.Key, x => x.Value);
            int index = 0;
            foreach (var kvp in columnOptions)
            {
                if (!kvp.Value.SkipOnInsert)
                {
                    MySqlBulkCopyColumnMapping mapping = new MySqlBulkCopyColumnMapping()
                    {
                        SourceOrdinal = index,
                        DestinationColumn = kvp.Key
                    };
                    sqlBulkCopy.ColumnMappings.Add(mapping);
                    index++;
                }
            }

            var table = GenerateDataTable(events, columnOptions);
            
            return table;
        }
    }
}
