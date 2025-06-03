using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Master.Models;
using Master.ViewModels;
using Serilog;

namespace Master.Views
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
            try
            {
                Log.Debug("Загрузка данных для калькулятора материалов");
                using var context = new ContosoPartnersContext();
                
                var products = context.Products.ToList();
                var materials = context.MaterialTypes.ToList();
                var productMaterials = context.ProductMaterials.ToList();

                Log.Debug("Загружено продуктов: {ProductsCount}, материалов: {MaterialsCount}, связей: {ProductMaterialsCount}", 
                    products.Count, materials.Count, productMaterials.Count);

                if (!products.Any())
                {
                    Log.Warning("Список продуктов пуст");
                    System.Windows.MessageBox.Show("Список продуктов пуст. Пожалуйста, добавьте продукты в базу данных.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!materials.Any())
                {
                    Log.Warning("Список материалов пуст");
                    System.Windows.MessageBox.Show("Список материалов пуст. Пожалуйста, добавьте материалы в базу данных.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!productMaterials.Any())
                {
                    Log.Warning("Список связей продуктов и материалов пуст");
                    var result = System.Windows.MessageBox.Show(
                        "Список связей продуктов и материалов пуст. Хотите создать связи для выбранного продукта?",
                        "Предупреждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        CreateProductMaterialLinks(context, products.First(), materials);
                    }
                    return;
                }

                ProductCombo.ItemsSource = products;
                MaterialCombo.ItemsSource = materials;
                
                Log.Information("Данные для калькулятора материалов успешно загружены");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке данных для калькулятора материалов");
                System.Windows.MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateProductMaterialLinks(ContosoPartnersContext context, Product product, System.Collections.Generic.List<MaterialType> materials)
        {
            try
            {
                Log.Debug("Создание связей для продукта {ProductId}", product.ProductId);
                foreach (var material in materials)
                {
                    var pm = new ProductMaterial
                    {
                        ProductId = product.ProductId,
                        MaterialId = material.MaterialId,
                        QuantityRequired = 1.0m // Значение по умолчанию
                    };
                    context.ProductMaterials.Add(pm);
                }
                context.SaveChanges();
                Log.Information("Связи для продукта {ProductId} успешно созданы", product.ProductId);
                System.Windows.MessageBox.Show("Связи успешно созданы. Теперь вы можете использовать калькулятор.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при создании связей для продукта {ProductId}", product.ProductId);
                System.Windows.MessageBox.Show($"Ошибка при создании связей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
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

                Log.Debug("Расчет материалов для продукта {ProductId} и материала {MaterialId}", 
                    selectedProduct.ProductId, selectedMaterial.MaterialId);

                using var context = new ContosoPartnersContext();
                var pm = context.ProductMaterials
                            .FirstOrDefault(x => x.ProductId.Trim() == selectedProduct.ProductId.Trim()
                                              && x.MaterialId.Trim() == selectedMaterial.MaterialId.Trim());

                if (pm == null)
                {
                    Log.Warning("Не найдена связь между продуктом {ProductId} и материалом {MaterialId}", 
                        selectedProduct.ProductId, selectedMaterial.MaterialId);
                    
                    var result = System.Windows.MessageBox.Show(
                        $"Параметры материала для выбранного продукта не найдены.\nПродукт: {selectedProduct.ProductName}\nМатериал: {selectedMaterial.MaterialType1}\n\nХотите создать связь?",
                        "Ошибка",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var newPm = new ProductMaterial
                        {
                            ProductId = selectedProduct.ProductId,
                            MaterialId = selectedMaterial.MaterialId,
                            QuantityRequired = 1.0m // Значение по умолчанию
                        };
                        context.ProductMaterials.Add(newPm);
                        context.SaveChanges();
                        pm = newPm;
                        Log.Information("Создана новая связь между продуктом {ProductId} и материалом {MaterialId}", 
                            selectedProduct.ProductId, selectedMaterial.MaterialId);
                    }
                    else
                    {
                        return;
                    }
                }

                // Calculate required material using decimal math
                decimal quantityRequired = pm.QuantityRequired ?? 0m;
                decimal rate = selectedMaterial.RejectRate ?? 0m;
                decimal totalRequired = quantityRequired * qty * (1 + rate);
                int needed = (int)decimal.Ceiling(totalRequired);
                ResultText.Text = $"Требуется материала: {needed}";
                Log.Information("Расчет материалов выполнен успешно. Требуется материала: {Quantity}", needed);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при расчете материалов");
                System.Windows.MessageBox.Show($"Ошибка при расчете: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
} 