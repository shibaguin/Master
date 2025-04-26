using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Master.Models;

namespace Master
{
    public partial class MaterialCalculatorPage : Page
    {
        public MaterialCalculatorPage()
        {
            InitializeComponent();
            Loaded += MaterialCalculatorPage_Loaded;
        }

        private void MaterialCalculatorPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Load products and materials into ComboBoxes
            using var context = new ContosoPartnersContext();
            ProductCombo.ItemsSource = context.Products.ToList();
            MaterialCombo.ItemsSource = context.MaterialTypes.ToList();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (ProductCombo.SelectedItem is not Product selectedProduct)
            {
                System.Windows.MessageBox.Show("Выберите продукт", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MaterialCombo.SelectedItem is not MaterialType selectedMaterial)
            {
                System.Windows.MessageBox.Show("Выберите материал", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(QuantityText.Text, out var qty) || qty <= 0)
            {
                System.Windows.MessageBox.Show("Введите корректное количество продукта", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new ContosoPartnersContext();
            var pm = context.ProductMaterials
                        .FirstOrDefault(x => x.ProductId == selectedProduct.ProductId && x.MaterialId == selectedMaterial.MaterialId);
            if (pm == null)
            {
                System.Windows.MessageBox.Show("Параметры материала для выбранного продукта не найдены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Calculate required material using decimal math
            decimal quantityRequired = pm.QuantityRequired ?? 0m;
            decimal rate = selectedMaterial.RejectRate ?? 0m;
            decimal totalRequired = quantityRequired * qty * (1 + rate);
            int needed = (int)decimal.Ceiling(totalRequired);
            ResultText.Text = $"Требуется материала: {needed}";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
} 