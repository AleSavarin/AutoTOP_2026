using Autotop.Domain.Interfaces;

namespace Autotop.Application.Services;

public interface ILoadController
{
    Task GoToLoadAsync(double targetLoad, CancellationToken cancellationToken = default);
    Task StopAsync();
}

public class LoadController : ILoadController
{
    private readonly IArduinoController _arduino;
    private readonly ILogService _logService;

    public LoadController(IArduinoController arduino, ILogService logService)
    {
        _arduino = arduino;
        _logService = logService;
    }

    public async Task GoToLoadAsync(double targetLoad, CancellationToken cancellationToken = default)
    {
        _logService.Info($"GoToLoad {targetLoad} kN (simulated)");
        // En etapa 2 implementaremos la lógica de control real.
        await Task.Delay(1000, cancellationToken);
    }

    public Task StopAsync()
    {
        _logService.Info("Load control stopped.");
        return Task.CompletedTask;
    }
}