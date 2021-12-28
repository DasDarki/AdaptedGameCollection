using System;
using System.IO;
using System.Threading;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SLogger = Serilog.Core.Logger;

namespace AdaptedGameCollection.Logging;

/// <summary>
/// The logger logs specific information to the console and a one-time file.
/// </summary>
public class Logger
{
    private readonly string _name;
    private readonly string _path;
    private readonly SLogger _internalLogger;
    private bool? _overwriteDebug = null;

    internal Logger(string name)
    {
        _name = name;
        _path = Directory.GetCurrentDirectory();
        _internalLogger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().WriteTo.Sink(new EventLogSink(this)).CreateLogger();
    }

    private string Format(string message)
    {
        return $"[{_name}] {message}";
    }

    /// <summary>
    /// Overwrites the debug flag of this logger.
    /// </summary>
    /// <param name="debug">The debug state</param>
    public void OverwriteDebugMode(bool debug)
    {
        _overwriteDebug = debug;
    }
    
    /// <summary>
    /// Prints an info message to the logging system.
    /// </summary>
    /// <param name="message">The message to be printed</param>
    /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
    public void Info(string message, params object[] args)
    {
        _internalLogger.Information(Format(message), args);
    }

    /// <summary>
    /// Prints a warning message to the logging system.
    /// </summary>
    /// <param name="message">The message to be printed</param>
    /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
    public void Warn(string message, params object[] args)
    {
        _internalLogger.Warning(Format(message), args);
    }

    /// <summary>
    /// Prints a verbose message to the logging system.
    /// </summary>
    /// <param name="message">The message to be printed</param>
    /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
    public void Verbose(string message, params object[] args)
    {
        _internalLogger.Verbose(Format(message), args);
    }

    /// <summary>
    /// Prints a debug message to the logging system.
    /// This message is only getting printed, when AGC is in Debug mode, or the plugin itself.
    /// </summary>
    /// <param name="message">The message to be printed</param>
    /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
    public void Debug(string message, params object[] args)
    {
        if(!(_overwriteDebug ?? LoggerFactory.IsDebug)) return;
        _internalLogger.Debug(Format(message), args);
    }

    /// <summary>
    /// Prints a fatal message to the logging system.
    /// </summary>
    /// <param name="message">The message to be printed</param>
    /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
    public void Fatal(string message, params object[] args)
    {
        _internalLogger.Fatal(Format(message), args);
    }

    /// <summary>
    /// Prints an error message to the logging system.
    /// </summary>
    /// <param name="exception">The exception which occurred</param>
    /// <param name="message">The message to be printed</param>
    /// <param name="args">Arguments which will be placed into the message by {PLACEHOLDERS}</param>
    public void Error(Exception exception, string message, params object[] args)
    {
        _internalLogger.Fatal(Format(message) + "\n" + exception, args);
    }
    
    private class EventLogSink : ILogEventSink
    {
        private readonly ReaderWriterLock _locker = new ReaderWriterLock();
        private readonly Logger _parent;

        internal EventLogSink(Logger parent)
        {
            _parent = parent;
        }

        public void Emit(LogEvent logEvent)
        {
            string line = $"[{DateTime.Now:hh:mm:ss} {LevelToSeverity(logEvent)}] " + logEvent.RenderMessage();
            try
            {
                _locker.AcquireWriterLock(int.MaxValue);
                File.AppendAllLines(Path.Combine(_parent._path, "log.txt"), new[] { line });
            }
            finally
            {
                _locker.ReleaseWriterLock();
            }
        }

        private string LevelToSeverity(LogEvent logEvent)
        {
            return logEvent.Level switch
            {
                LogEventLevel.Debug => "DBG",
                LogEventLevel.Error => "ERR",
                LogEventLevel.Fatal => "FTL",
                LogEventLevel.Verbose => "VRB",
                LogEventLevel.Warning => "WRN",
                _ => "INF"
            };
        }
    }
}