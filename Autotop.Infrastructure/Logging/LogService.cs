using Autotop.Domain.Interfaces;
using Autotop.Domain.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace Autotop.Infrastructure.Logging;

public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;
    private readonly string _logFilePath;

    public LogService(ILogger<LogService> logger, string logFilePath)
    {
        _logger = logger;
        _logFilePath = logFilePath;
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);
    }

    public void Info(string message)
    {
        Log(LogLevel.Info, message);
    }

    public void Warning(string message)
    {
        Log(LogLevel.Warning, message);
    }

    public void Error(string message)
    {
        Log(LogLevel.Error, message);
    }

    private void Log(LogLevel level, string message)
    {
        var entry = new LogEntry(DateTime.Now, message, level);
        // Escribir en archivo
        File.AppendAllText(_logFilePath, $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}");
        // Enviar mensaje MVVM para UI
        WeakReferenceMessenger.Default.Send(new LogMessage(entry));
        // También loguear con ILogger
        _logger.Log(level == LogLevel.Info ? LogLevel.Information : (level == LogLevel.Warning ? LogLevel.Warning : LogLevel.Error), message);
    }
}

public record LogMessage(LogEntry Entry);