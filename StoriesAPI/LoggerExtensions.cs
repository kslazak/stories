namespace StoriesAPI
{
    /// <summary>
    /// Provides extension methods for <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs specified message as Information using provided logger.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> instance to be used.</param>
        /// <param name="message">Message to be logged.</param>
        /// <param name="context">Current <see cref="HttpContext"/>.</param>
        public static void LogAsInformation(this ILogger logger, string message, HttpContext? context)
        {
            Log(logger, LogLevel.Information, message, context);
        }

        /// <summary>
        /// Logs specified message as Warning using provided logger.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> instance to be used.</param>
        /// <param name="message">Message to be logged.</param>
        /// <param name="context">Current <see cref="HttpContext"/>.</param>
        public static void LogAsWarning(this ILogger logger, string message, HttpContext? context)
        {
            Log(logger, LogLevel.Warning, message, context);
        }

        /// <summary>
        /// Logs specified message as Error using provided logger.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> instance to be used.</param>
        /// <param name="message">Message to be logged.</param>
        /// <param name="context">Current <see cref="HttpContext"/>.</param>
        public static void LogAsError(this ILogger logger, string message, HttpContext? context)
        {
            Log(logger, LogLevel.Error, message, context);
        }

        private static void Log(ILogger logger, LogLevel level, string message, HttpContext? context)
        {
            var trace = context != null ? $" (traceId: {context.TraceIdentifier})" : string.Empty;
            logger.Log(level, $"{DateTime.UtcNow:O} {message}{trace}");
        }
    }
}
