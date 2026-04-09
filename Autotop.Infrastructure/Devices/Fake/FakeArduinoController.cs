using Autotop.Domain.Interfaces;

namespace Autotop.Infrastructure.Devices.Fake;

public class FakeArduinoController : IArduinoController
{
    public bool IsConnected { get; private set; }

    public Task<bool> ConnectAsync(string portName, CancellationToken cancellationToken = default)
    {
        IsConnected = true;
        return Task.FromResult(true);
    }

    public Task DisconnectAsync()
    {
        IsConnected = false;
        return Task.CompletedTask;
    }

    public Task SetRampAsync(int position, CancellationToken cancellationToken = default)
    {
        // Simular cambio de velocidad
        return Task.CompletedTask;
    }

    public Task ResetWatchdogAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}