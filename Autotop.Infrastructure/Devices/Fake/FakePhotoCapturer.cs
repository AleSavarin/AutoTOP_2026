using Autotop.Domain.Interfaces;

namespace Autotop.Infrastructure.Devices.Fake;

public class FakePhotoCapturer : IPhotoCapturer
{
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

    public Task<string?> CapturePhotoAsync(string? filename = null, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(Path.GetTempPath(), $"fake_photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
        File.WriteAllText(path, "fake image data");
        return Task.FromResult<string?>(path);
    }
}