

using Microsoft.Extensions.Configuration;

namespace Serilog.Sinks.MySql.MySql.Configuration
{
    internal interface IMicrosoftExtensionsConnectionStringProvider
    {
        string GetConnectionString(string nameOrConnectionString, IConfiguration appConfiguration);
    }
}
