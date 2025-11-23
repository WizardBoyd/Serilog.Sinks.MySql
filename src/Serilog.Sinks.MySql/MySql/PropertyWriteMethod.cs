

namespace Serilog.Sinks.MySql.MySql
{
    public enum PropertyWriteMethod
    {
        /// <summary>
        ///     The raw method.
        /// </summary>
        Raw,

        /// <summary>
        ///     The to string method.
        /// </summary>
        ToString,

        /// <summary>
        ///     The json method.
        /// </summary>
        Json
    }
}
