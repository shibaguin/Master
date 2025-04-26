using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Linq;
using Master.Models;

namespace Master
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Partner> Partners { get; } = new ObservableCollection<Partner>();
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Load Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadData()
        {
            Partners.Clear();
            using var context = new ContosoPartnersContext();
            // Load all partners and sales, then compute totals and discounts
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
            MainDataGrid.ItemsSource = Partners;
        }

        private void EditPartners_Click(object sender, RoutedEventArgs e)
        {
            // Require selection before editing
            if (!(MainDataGrid.SelectedItem is Partner selectedPartner))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите партнёра для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var form = new EditWindow(selectedPartner);
            if (form.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            var form = new EditWindow();
            if (form.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            if (!(MainDataGrid.SelectedItem is Partner selectedPartner))
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
    }
}