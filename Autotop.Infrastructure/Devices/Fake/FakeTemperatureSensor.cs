using Autotop.Domain.Interfaces;

namespace Autotop.Infrastructure.Devices.Fake;

public class FakeTemperatureSensor : ITemperatureSensor
{
    private readonly Random _random = new();
    public bool IsConnected { get; private set; }

    public Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        IsConnected = true;
        return Task.FromResult(true);
    }

    public Task DisconnectAsync()
    {
        IsConnected = false;
        return Task.CompletedTask;
    }

    public Task<double> ReadTemperatureAsync(int channel, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(23.5 + _random.NextDouble() * 2.0);
    }
}