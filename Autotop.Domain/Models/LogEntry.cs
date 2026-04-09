namespace Autotop.Domain.Models;

public record LogEntry(DateTime Timestamp, string Message, LogLevel Level);

public enum LogLevel
{
    Info,
    Warning,
    Error
}