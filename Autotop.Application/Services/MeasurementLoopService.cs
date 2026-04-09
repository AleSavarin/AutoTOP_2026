using Autotop.Domain.Interfaces;
using Autotop.Domain.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Autotop.Application.Services;

public class MeasurementLoopService : BackgroundService
{
    private readonly IHbmDevice _referenceDevice;
    private readonly IHbmDevice? _dutDevice;
    private readonly IArduinoController _arduino;
    private readonly IPhotoCapturer? _photoCapturer;
    private readonly ILogService _logService;
    private readonly ILogger<MeasurementLoopService> _logger;
    private readonly int _sampleRateHz = 10; // Configurable luego

    public MeasurementLoopService(
        IHbmDevice referenceDevice,
        IHbmDevice? dutDevice,
        IArduinoController arduino,
        IPhotoCapturer? photoCapturer,
        ILogService logService,
        ILogger<MeasurementLoopService> logger)
    {
        _referenceDevice = referenceDevice;
        _dutDevice = dutDevice;
        _arduino = arduino;
        _photoCapturer = photoCapturer;
        _logService = logService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logService.Info("Measurement loop started (simulated).");
        var interval = TimeSpan.FromMilliseconds(1000.0 / _sampleRateHz);

        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                // Leer referencia
                var refData = await _referenceDevice.ReadMeasurementAsync(stoppingToken);
                HbmMeasurement? dutData = null;
                if (_dutDevice != null)
                    dutData = await _dutDevice.ReadMeasurementAsync(stoppingToken);

                // Publicar mensaje MVVM para actualizar UI
                WeakReferenceMessenger.Default.Send(new MeasurementUpdatedMessage(
                    refData.ChannelValues.FirstOrDefault(),
                    dutData?.ChannelValues.FirstOrDefault() ?? 0,
                    refData.Timestamp));

                // Simular watchdog del Arduino
                await _arduino.ResetWatchdogAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in measurement loop");
                _logService.Error($"Measurement error: {ex.Message}");
            }

            var elapsed = sw.Elapsed;
            if (elapsed < interval)
                await Task.Delay(interval - elapsed, stoppingToken);
            else
                _logger.LogWarning("Loop overrun: {Elapsed}ms", elapsed.TotalMilliseconds);
        }

        _logService.Info("Measurement loop stopped.");
    }
}

public record MeasurementUpdatedMessage(double ReferenceMvV, double DutMvV, DateTime Timestamp);