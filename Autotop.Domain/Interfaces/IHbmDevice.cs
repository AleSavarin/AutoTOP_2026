namespace Autotop.Domain.Interfaces;

public interface IHbmDevice : IAsyncDisposable
{
    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync();
    Task<HbmMeasurement> ReadMeasurementAsync(CancellationToken cancellationToken = default);
    Task TareAsync(int channel, CancellationToken cancellationToken = default);
    Task CalibrateAsync(CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}