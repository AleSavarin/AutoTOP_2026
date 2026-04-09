namespace Autotop.Domain.Interfaces;

public interface IArduinoController
{
    Task<bool> ConnectAsync(string portName, CancellationToken cancellationToken = default);
    Task DisconnectAsync();
    Task SetRampAsync(int position, CancellationToken cancellationToken = default);
    Task ResetWatchdogAsync(CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}