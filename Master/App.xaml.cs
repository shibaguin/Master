using System.Configuration;
using System.Data;
using System.Windows;
using Master.Models;
using Master.Services;
using Master.Views;
using Master.ViewModels;
using System.IO;
using System;
using Serilog;
using Serilog.Events;

namespace Master
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IDataService DataService { get; private set; } = null!;
        private static readonly string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app-.log");

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                ConfigureSerilog();
                Log.Information("Начало запуска приложения");
                base.OnStartup(e);
                
                Log.Information("Создание контекста базы данных");
                var context = new ContosoPartnersContext();
                
                Log.Information("Инициализация DataService");
                DataService = new DataService(context);
                
                Log.Information("Приложение успешно запущено");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Критическая ошибка при запуске приложения");
                System.Windows.MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}\n\nПодробности в файле лога: {LogFile}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureSerilog()
        {
            var logDirectory = Path.GetDirectoryName(LogFile);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory!);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.File(LogFile,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Завершение работы приложения");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
