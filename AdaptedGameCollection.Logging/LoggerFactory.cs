using System.Runtime.InteropServices;

namespace AdaptedGameCollection.Logging;

/// <summary>
/// The logger factory creates a new logger for a specific format and manages the behaviour of all
/// created loggers.
/// </summary>
public static class LoggerFactory
{
    /// <summary>
    /// Whether the loggers are in debug mode or not. If <see cref="IsDebug"/> is true, debug messages gets printed,
    /// too otherwise they will be omitted.
    /// </summary>
    public static bool IsDebug { get; set; }
    
    /// <summary>
    /// Creates a new logger for the given type.
    /// </summary>
    /// <typeparam name="T">The type the logger is owned by</typeparam>
    /// <returns>The created logger</returns>
    public static Logger Create<T>(string suffix = "")
    {
        return new Logger(typeof(T).Name + (string.IsNullOrEmpty(suffix) ? "" : "-" + suffix));
    }

    /// <summary>
    /// Opens a console windows either if the application is running as a windows forms application.
    /// </summary>
    public static void OpenConsole()
    {
        AllocConsole();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();
}