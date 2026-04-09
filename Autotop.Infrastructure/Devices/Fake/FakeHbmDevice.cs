using Autotop.Domain.Interfaces;
using Autotop.Domain.Models;

namespace Autotop.Infrastructure.Devices.Fake;

public class FakeHbmDevice : IHbmDevice
{
    private readonly Random _random = new();
    private double _currentValue = 0.0;
    private bool _connected = false;
    private readonly string _name;

    public FakeHbmDevice(string name = "FakeHBM") => _name = name;
    public bool IsConnected => _connected;

    public Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        _connected = true;
        return Task.FromResult(true);
    }

    public Task DisconnectAsync()
    {
        _connected = false;
        return Task.CompletedTask;
    }

    public Task<HbmMeasurement> ReadMeasurementAsync(CancellationToken cancellationToken = default)
    {
        if (!_connected) throw new InvalidOperationException("Device not connected");

        // Simular un pequeño drift
        _currentValue += (_random.NextDouble() - 0.5) * 0.0001;
        _currentValue = Math.Max(0, _currentValue);

        return Task.FromResult(new HbmMeasurement
        {
            ChannelValues = new[] { _currentValue },
            Timestamp = DateTime.Now
        });
    }

    public Task TareAsync(int channel, CancellationToken cancellationToken = default)
    {
        _currentValue = 0.0;
        return Task.CompletedTask;
    }

    public Task CalibrateAsync(CancellationToken cancellationToken = default)
    {
        // No hace nada en el fake
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _connected = false;
        return ValueTask.CompletedTask;
    }
}