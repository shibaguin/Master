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
            var list = context.Partners.ToList();
            foreach (var p in list)
                Partners.Add(p);
            MainDataGrid.ItemsSource = Partners;
        }

        private void EditPartners_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = MainDataGrid.SelectedItem as Partner;
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
    }
}