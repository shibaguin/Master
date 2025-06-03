using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Master.Models;
using Master.ViewModels;
using Serilog;

namespace Master.Views
{
    public partial class PartnersListPage : Page
    {
        public ObservableCollection<Partner> Partners { get; } = new ObservableCollection<Partner>();

        public PartnersListPage()
        {
            Log.Debug("Инициализация страницы списка партнеров");
            InitializeComponent();
            DataContext = this;
            Loaded += PartnersListPage_Loaded;
        }

        private void PartnersListPage_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("Загрузка страницы списка партнеров");
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                Log.Debug("Начало загрузки данных партнеров");
                Partners.Clear();
                var partnersList = (await App.DataService.GetPartnersAsync()).ToList();
                var sales = (await App.DataService.GetSalesHistoryAsync()).Where(s => s.PartnerId != null).ToList();
                var salesByPartner = sales
                    .GroupBy(s => s.PartnerId)
                    .ToDictionary(g => g.Key!, g => g.Sum(s => s.Quantity ?? 0));

                foreach (var p in partnersList)
                {
                    var total = salesByPartner.TryGetValue(p.PartnerId, out var qty) ? qty : 0;
                    p.ComputeDiscount(total);
                    Partners.Add(p);
                }
                Log.Information("Данные партнеров успешно загружены. Загружено {Count} партнеров", Partners.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке данных партнеров");
                System.Windows.MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Log.Debug("Переход на страницу добавления нового партнера");
            NavigationService.Navigate(new PartnerEditPage());
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (!(PartnersGrid.SelectedItem is Partner selectedPartner))
            {
                Log.Warning("Попытка редактирования без выбранного партнера");
                System.Windows.MessageBox.Show("Пожалуйста, выберите партнёра для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Log.Debug("Переход на страницу редактирования партнера {PartnerName} (ID: {PartnerId})", selectedPartner.PartnerName, selectedPartner.PartnerId);
            NavigationService.Navigate(new PartnerEditPage(selectedPartner));
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(PartnersGrid.SelectedItem is Partner selectedPartner))
            {
                Log.Warning("Попытка удаления без выбранного партнера");
                System.Windows.MessageBox.Show("Пожалуйста, выберите партнёра для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Log.Debug("Запрос подтверждения удаления партнера {PartnerName} (ID: {PartnerId})", selectedPartner.PartnerName, selectedPartner.PartnerId);
            var result = System.Windows.MessageBox.Show($"Действительно удалить партнёра '{selectedPartner.PartnerName}'?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                Log.Information("Начало удаления партнера {PartnerName} (ID: {PartnerId})", selectedPartner.PartnerName, selectedPartner.PartnerId);
                if (await App.DataService.DeletePartnerAsync(selectedPartner.PartnerId))
                {
                    Log.Information("Партнер {PartnerName} успешно удален", selectedPartner.PartnerName);
                    LoadData();
                }
                else
                {
                    Log.Warning("Не удалось удалить партнера {PartnerName}", selectedPartner.PartnerName);
                    System.Windows.MessageBox.Show("Не удалось удалить партнёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении партнера {PartnerName}", selectedPartner.PartnerName);
                System.Windows.MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            if (!(PartnersGrid.SelectedItem is Partner selectedPartner))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите партнёра для просмотра истории.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NavigationService.Navigate(new SalesHistoryPage(selectedPartner));
        }

        private void Materials_Click(object sender, RoutedEventArgs e)
        {
            if (!(PartnersGrid.SelectedItem is Partner selectedPartner))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите партнёра для расчета материалов.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NavigationService.Navigate(new MaterialCalculatorPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
} 