using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Master.Models;

namespace Master
{
    public partial class SalesHistoryPage : Page
    {
        private readonly string _partnerId;

        public SalesHistoryPage(Partner partner)
        {
            InitializeComponent();
            _partnerId = partner.PartnerId;
            LoadHistory();
        }

        private void LoadHistory()
        {
            using var context = new ContosoPartnersContext();
            var history = context.Sales
                .Where(s => s.PartnerId == _partnerId)
                .Join(context.Products,
                      sale => sale.ProductId,
                      prod => prod.ProductId,
                      (sale, prod) => new
                      {
                          ProductName = prod.ProductName,
                          Quantity = sale.Quantity ?? 0,
                          SaleDate = sale.SaleDate
                      })
                .ToList();
            SalesGrid.ItemsSource = history;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
} 