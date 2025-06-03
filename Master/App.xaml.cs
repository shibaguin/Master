using System.Configuration;
using System.Data;
using System.Windows;
using Master.Models;
using Master.Services;
using Master.Views;
using Master.ViewModels;
using System.IO;
using System;

namespace Master
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IDataService DataService { get; private set; } = null!;
        private static readonly string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.log");

        private static void LogError(string message, Exception? ex = null)
        {
            try
            {
                var logMessage = $"{DateTime.Now}: {message}";
                if (ex != null)
                {
                    logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
                }
                File.AppendAllText(LogFile, logMessage + "\n\n");
            }
            catch { }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                LogError("Начало запуска приложения");
                base.OnStartup(e);
                
                LogError("Создание контекста базы данных");
                var context = new ContosoPartnersContext();
                
                LogError("Инициализация DataService");
                DataService = new DataService(context);
                
                LogError("Приложение успешно запущено");
            }
            catch (Exception ex)
            {
                LogError("Критическая ошибка при запуске приложения", ex);
                System.Windows.MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}\n\nПодробности в файле лога: {LogFile}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
