using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Master.Models;
using Serilog;
using System.Text;
using System.Collections.Generic;

namespace Master.Views
{
    public class SalesHistoryItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public DateOnly? SaleDate { get; set; }
    }

    public partial class SalesHistoryPage : Page
    {
        private readonly string _partnerId;
        private static readonly Encoding Windows1251 = Encoding.GetEncoding(1251);

        public SalesHistoryPage(Partner partner)
        {
            Log.Debug("Инициализация страницы истории продаж для партнера {PartnerName} (ID: {PartnerId})", partner.PartnerName, partner.PartnerId);
            InitializeComponent();
            _partnerId = partner.PartnerId;
            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                Log.Debug("Загрузка истории продаж для партнера {PartnerId}", _partnerId);
                using var context = new ContosoPartnersContext();
                var history = context.Sales
                    .Where(s => s.PartnerId == _partnerId)
                    .Join(context.Products,
                          sale => sale.ProductId,
                          prod => prod.ProductId,
                          (sale, prod) => new SalesHistoryItem
                          {
                              ProductName = prod.ProductName ?? string.Empty,
                              Quantity = sale.Quantity ?? 0,
                              SaleDate = sale.SaleDate
                          })
                    .ToList();

                // Применяем кодировку к результатам после загрузки из БД
                foreach (var item in history)
                {
                    if (!string.IsNullOrEmpty(item.ProductName))
                    {
                        try
                        {
                            var bytes = Windows1251.GetBytes(item.ProductName);
                            item.ProductName = Encoding.UTF8.GetString(bytes);
                            Log.Debug("Успешно преобразовано название продукта: {ProductName}", item.ProductName);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Ошибка при преобразовании кодировки для продукта: {ProductName}", item.ProductName);
                        }
                    }
                }

                SalesGrid.ItemsSource = history;
                Log.Information("История продаж успешно загружена. Загружено {Count} записей", history.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке истории продаж для партнера {PartnerId}", _partnerId);
                System.Windows.MessageBox.Show($"Ошибка при загрузке истории продаж: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Log.Debug("Возврат к списку партнеров");
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
} 