using Autotop.Application.Services;
using Autotop.Domain.Interfaces;
using Autotop.Infrastructure.Devices.Fake;
using Autotop.Infrastructure.Logging;
using Autotop.Infrastructure.Repositories;
using Autotop.Wpf.ViewModels;
using Autotop.Wpf.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;

namespace Autotop.Wpf;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Registrar ViewModels
                services.AddSingleton<MainViewModel>();

                // Registrar servicios de aplicación
                services.AddSingleton<MeasurementLoopService>();
                services.AddSingleton<ILoadController, LoadController>();
                services.AddSingleton<IMacroService, MacroService>();

                // Registrar dispositivos (fake por ahora)
                services.AddSingleton<IHbmDevice>(sp => new FakeHbmDevice("Referencia"));
                services.AddSingleton<IHbmDevice>(sp => new FakeHbmDevice("Incógnita"));
                services.AddSingleton<IArduinoController, FakeArduinoController>();
                services.AddSingleton<ITemperatureSensor, FakeTemperatureSensor>();
                services.AddSingleton<IPhotoCapturer, FakePhotoCapturer>();

                // Registrar repositorio
                services.AddSingleton<ITransducerRepository, TransducerRepository>();

                // Configurar LogService
                var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", $"session_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                services.AddSingleton<ILogService>(sp =>
                {
                    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<LogService>();
                    return new LogService(logger, logFilePath);
                });

                // Registrar BackgroundService
                services.AddHostedService<MeasurementLoopService>();

                // Registrar ventana principal
                services.AddSingleton<MainWindow>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        // Configurar messenger para recibir logs en el ViewModel
        var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
        WeakReferenceMessenger.Default.Register<LogMessage>(this, (r, m) =>
        {
            Application.Current.Dispatcher.Invoke(() => mainViewModel.AddLogEntry(m.Entry));
        });

        WeakReferenceMessenger.Default.Register<MeasurementUpdatedMessage>(this, (r, m) =>
        {
            Application.Current.Dispatcher.Invoke(() => mainViewModel.UpdateMeasurement(m));
        });

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = mainViewModel;
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}