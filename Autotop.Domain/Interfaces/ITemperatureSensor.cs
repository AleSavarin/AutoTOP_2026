namespace Autotop.Domain.Interfaces;

public interface ITemperatureSensor
{
    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync();
    Task<double> ReadTemperatureAsync(int channel, CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}