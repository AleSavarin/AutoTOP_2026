namespace Autotop.Domain.Models;

public record HbmMeasurement
{
    public double[] ChannelValues { get; init; } = Array.Empty<double>();
    public DateTime Timestamp { get; init; } = DateTime.Now;
}