using Autotop.Domain.Interfaces;

namespace Autotop.Application.Services;

public interface IMacroService
{
    Task ExecuteMacroAsync(IEnumerable<string> lines, CancellationToken cancellationToken = default);
}

public class MacroService : IMacroService
{
    private readonly ILoadController _loadController;
    private readonly ILogService _logService;

    public MacroService(ILoadController loadController, ILogService logService)
    {
        _loadController = loadController;
        _logService = logService;
    }

    public async Task ExecuteMacroAsync(IEnumerable<string> lines, CancellationToken cancellationToken = default)
    {
        _logService.Info("Macro execution started (simulated).");
        foreach (var line in lines)
        {
            if (line.StartsWith("Carga:"))
            {
                if (double.TryParse(line[6..].Trim(), out var load))
                    await _loadController.GoToLoadAsync(load, cancellationToken);
            }
            else if (line == "Descarga")
            {
                await _loadController.GoToLoadAsync(0, cancellationToken);
            }
            else if (line == "Top")
            {
                _logService.Info("TOP (simulated)");
                await Task.Delay(500, cancellationToken);
            }
        }
        _logService.Info("Macro finished.");
    }
}