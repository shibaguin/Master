using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Master.Models;
using Microsoft.EntityFrameworkCore;

namespace Master
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private readonly ContosoPartnersContext _context;
        private readonly Partner _partner;
        private readonly bool _isNew;

        public EditWindow(Partner partner = null)
        {
            InitializeComponent();
            _context = new ContosoPartnersContext();
            if (partner == null)
            {
                _isNew = true;
                _partner = new Partner();
            }
            else
            {
                _isNew = false;
                _partner = _context.Partners.Find(partner.PartnerId) ?? throw new InvalidOperationException("Partner not found: " + partner.PartnerId);
            }
            DataContext = _partner;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isNew)
                    _context.Partners.Add(_partner);
                _context.SaveChanges();
                DialogResult = true;
            }
            catch (DbUpdateException ex)
            {
                System.Windows.MessageBox.Show("Ошибка базы данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
