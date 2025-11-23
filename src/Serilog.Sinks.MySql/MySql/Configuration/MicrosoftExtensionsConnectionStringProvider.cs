

using Microsoft.Extensions.Configuration;
using Serilog.Debugging;

namespace Serilog.Sinks.MySql.MySql.Configuration
{
    internal sealed class MicrosoftExtensionsConnectionStringProvider
        : IMicrosoftExtensionsConnectionStringProvider
    {
        public string GetConnectionString(string nameOrConnectionString, IConfiguration appConfiguration)
        {
            // If there is a `=`, we assume this is a raw connection string, not a named value.
            // If there are no `=`, attempt to pull the named value from config.
            if (nameOrConnectionString.IndexOf("=", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return nameOrConnectionString;
            }

            var result = appConfiguration?.GetConnectionString(nameOrConnectionString);

            if (string.IsNullOrWhiteSpace(result))
            {
                SelfLog.WriteLine($"The value {nameOrConnectionString} is not found in the `ConnectionStrings` settings and does not appear to be a raw connection string.");
            }

            return result ?? string.Empty;
        }
    }
}
