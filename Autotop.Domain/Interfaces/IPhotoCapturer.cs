namespace Autotop.Domain.Interfaces;

public interface IPhotoCapturer
{
    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync();
    Task<string?> CapturePhotoAsync(string? filename = null, CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}