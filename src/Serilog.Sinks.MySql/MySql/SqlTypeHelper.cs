

using MySqlConnector;

namespace Serilog.Sinks.MySql.MySql
{
    public static class SqlTypeHelper
    {
        public const int DefaultBitColumnsLength = 8;

        public const int DefaultCharColumnsLength = 50;

        public const int DefaultVarcharColumnsLength = 50;

        public static string GetSqlTypeString(MySqlDbType dbType)
        {
            return dbType switch
            {
                MySqlDbType.Bool => "BOOLEAN",
                MySqlDbType.Decimal => "DECIMAL",
                MySqlDbType.Byte => "TINYINT",
                MySqlDbType.Int16 => "SMALLINT",
                MySqlDbType.Int32 => "INT",
                MySqlDbType.Float => "FLOAT",
                MySqlDbType.Double => "DOUBLE",
                MySqlDbType.Timestamp => "TIMESTAMP",
                MySqlDbType.Int64 => "BIGINT",
                MySqlDbType.Int24 => "MEDIUMINT",
                MySqlDbType.Date => "DATE",
                MySqlDbType.Time => "TIME",
                MySqlDbType.DateTime => "DATETIME",
                MySqlDbType.Year => "YEAR",
                MySqlDbType.Bit => "BIT",
                MySqlDbType.JSON => "JSON",
                MySqlDbType.TinyBlob => "TINYBLOB",
                MySqlDbType.MediumBlob => "MEDIUMBLOB",
                MySqlDbType.LongBlob => "LONGBLOB",
                MySqlDbType.VarChar => "VARCHAR(255)",
                MySqlDbType.String => "VARCHAR(255)",
                MySqlDbType.UByte => "TINYINT UNSIGNED",
                MySqlDbType.UInt16 => "SMALLINT UNSIGNED",
                MySqlDbType.UInt32 => "INT UNSIGNED",
                MySqlDbType.UInt64 => "BIGINT UNSIGNED",
                MySqlDbType.UInt24 => "MEDIUMINT UNSIGNED",
                MySqlDbType.TinyText => "TINYTEXT",
                MySqlDbType.MediumText => "MEDIUMTEXT",
                MySqlDbType.LongText => "LONGTEXT",
                MySqlDbType.Text => "TEXT",
                MySqlDbType.Guid => "VARCHAR(36)",
                _ => "",
            };
        }

        public static Type GetTypeForDbType(MySqlDbType dbType)
        {
            return dbType switch
            {
                MySqlDbType.Bool => typeof(bool),
                MySqlDbType.Decimal => typeof(decimal),
                MySqlDbType.Byte => typeof(byte),
                MySqlDbType.Int16 => typeof(short),
                MySqlDbType.Int32 => typeof(int),
                MySqlDbType.Float => typeof(float),
                MySqlDbType.Double => typeof(double),
                MySqlDbType.Null => typeof(DBNull),
                MySqlDbType.Timestamp => typeof(DateTime),
                MySqlDbType.Int64 => typeof(long),
                MySqlDbType.Int24 => typeof(int),
                MySqlDbType.Date => typeof(DateTime),
                MySqlDbType.Time => typeof(DateTime),
                MySqlDbType.DateTime => typeof(DateTime),
                MySqlDbType.Year => typeof(int),
                MySqlDbType.Newdate => typeof(DateTime),
                MySqlDbType.VarString => typeof(string),
                MySqlDbType.Bit => typeof(bool),
                MySqlDbType.JSON => typeof(string),
                MySqlDbType.NewDecimal => typeof(decimal),
                MySqlDbType.Enum => typeof(string),
                MySqlDbType.Set => typeof(string),
                MySqlDbType.TinyBlob => typeof(byte[]),
                MySqlDbType.MediumBlob => typeof(byte[]),
                MySqlDbType.LongBlob => typeof(byte[]),
                MySqlDbType.Blob => typeof(byte[]),
                MySqlDbType.VarChar => typeof(string),
                MySqlDbType.String => typeof(string),
                MySqlDbType.Geometry => typeof(DBNull),
                MySqlDbType.UByte => typeof(byte),
                MySqlDbType.UInt16 => typeof(ushort),
                MySqlDbType.UInt32 => typeof(uint),
                MySqlDbType.UInt64 => typeof(ulong),
                MySqlDbType.UInt24 => typeof(uint),
                MySqlDbType.Binary => typeof(byte[]),
                MySqlDbType.VarBinary => typeof(byte[]),
                MySqlDbType.TinyText => typeof(string),
                MySqlDbType.MediumText => typeof(string),
                MySqlDbType.LongText => typeof(string),
                MySqlDbType.Text => typeof(string),
                MySqlDbType.Guid => typeof(Guid),
                _ => typeof(DBNull),
            };
        }
    }
}
