using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Master.Models;
using Master.ViewModels;

namespace Master.Views
{
    public partial class PartnersListPage : Page
    {
        public ObservableCollection<Partner> Partners { get; } = new ObservableCollection<Partner>();

        public PartnersListPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += PartnersListPage_Loaded;
        }

        private void PartnersListPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartnerEditPage());
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (!(PartnersGrid.SelectedItem is Partner selectedPartner))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите партнёра для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NavigationService.Navigate(new PartnerEditPage(selectedPartner));
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(PartnersGrid.SelectedItem is Partner selectedPartner))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите партнёра для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = System.Windows.MessageBox.Show($"Действительно удалить партнёра '{selectedPartner.PartnerName}'?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                if (await App.DataService.DeletePartnerAsync(selectedPartner.PartnerId))
                {
                    LoadData();
                }
                else
                {
                    System.Windows.MessageBox.Show("Не удалось удалить партнёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
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