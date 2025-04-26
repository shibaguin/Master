using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Master.Models;

namespace Master
{
    public partial class PartnersListPage : Page
    {
        public ObservableCollection<Partner> Partners { get; } = new ObservableCollection<Partner>();

        public PartnersListPage()
        {
            InitializeComponent();
            DataContext = this;
            // Load data when page is loaded/navigated
            Loaded += PartnersListPage_Loaded;
        }

        private void PartnersListPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            Partners.Clear();
            using var context = new ContosoPartnersContext();
            var partnersList = context.Partners.ToList();
            var sales = context.Sales.Where(s => s.PartnerId != null).ToList();
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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to edit page for new partner
            NavigationService.Navigate(new PartnerEditPage());
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (!(PartnersGrid.SelectedItem is Partner selectedPartner))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите партнёра для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Navigate to edit page for existing partner
            NavigationService.Navigate(new PartnerEditPage(selectedPartner));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
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
                using var context = new ContosoPartnersContext();
                var partner = context.Partners.Find(selectedPartner.PartnerId);
                if (partner != null)
                {
                    context.Partners.Remove(partner);
                    context.SaveChanges();
                }
                LoadData();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
} 