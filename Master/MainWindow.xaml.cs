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
using System.Data;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Master
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                LoadData();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Load Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoPartners"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Partners", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                MainDataGrid.ItemsSource = dt.DefaultView;
            }
        }
    }
}