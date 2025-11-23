

namespace Serilog.Sinks.MySql.IntegrationTests
{
    public abstract class BaseTests
    {
        protected const string ConnectionString
            = "Server=localhost;Port=3306;Database=waternav_log;User=waternav_app;Password=waternav_pw;";
    }
}
