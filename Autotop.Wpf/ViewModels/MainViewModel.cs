using Autotop.Application.Services;
using Autotop.Domain.Interfaces;
using Autotop.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Autotop.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ILogService _logService;
    private readonly MeasurementLoopService? _measurementLoop; // Se accede vía DI pero no se inicia aquí
    private readonly IMacroService _macroService;
    private readonly ILoadController _loadController;

    [ObservableProperty] private double _referenceMvV;
    [ObservableProperty] private double _dutMvV;
    [ObservableProperty] private int _stabilityPpm;
    [ObservableProperty] private string _statusText = "Listo";
    [ObservableProperty] private bool _continuousLogging;

    public ObservableCollection<LogEntryViewModel> LogEntries { get; } = new();

    public MainViewModel(ILogService logService, IMacroService macroService, ILoadController loadController)
    {
        _logService = logService;
        _macroService = macroService;
        _loadController = loadController;

        _logService.Info("Aplicación iniciada (modo simulación).");
    }

    [RelayCommand]
    private async Task ConnectAllAsync()
    {
        StatusText = "Conectando...";
        // Aquí llamaríamos a los métodos ConnectAsync de los dispositivos reales.
        // Por ahora solo simulamos.
        await Task.Delay(500);
        _logService.Info("Todos los dispositivos conectados (simulado).");
        StatusText = "Conectado";
    }

    [RelayCommand]
    private async Task StartMacroAsync()
    {
        var macroLines = new[]
        {
            "Carga: 10",
            "Carga: 20",
            "Carga: 30",
            "Descarga",
            "Top"
        };
        StatusText = "Ejecutando macro...";
        await _macroService.ExecuteMacroAsync(macroLines);
        StatusText = "Macro finalizada";
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        await _loadController.StopAsync();
        StatusText = "Detenido";
    }

    public void AddLogEntry(LogEntry entry)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            LogEntries.Add(new LogEntryViewModel(entry));
            // Limitar a 1000 entradas
            if (LogEntries.Count > 1000)
                LogEntries.RemoveAt(0);
        });
    }

    public void UpdateMeasurement(MeasurementUpdatedMessage msg)
    {
        ReferenceMvV = msg.ReferenceMvV;
        DutMvV = msg.DutMvV;
        // Estabilidad simulada
        StabilityPpm = new Random().Next(5, 30);
    }
}

public class LogEntryViewModel
{
    public string DisplayText => $"{Entry.Timestamp:HH:mm:ss} [{Entry.Level}] {Entry.Message}";
    public Brush ColorBrush => Entry.Level switch
    {
        LogLevel.Warning => Brushes.Yellow,
        LogLevel.Error => Brushes.Red,
        _ => Brushes.LightGreen
    };
    public LogEntry Entry { get; }

    public LogEntryViewModel(LogEntry entry) => Entry = entry;
}