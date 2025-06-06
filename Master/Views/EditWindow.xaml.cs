﻿using System;
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
using Master.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Master.Views
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
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
                // Pre-generate PartnerId for display: next numeric ID
                var maxId = _context.Partners
                    .AsEnumerable()
                    .Select(p => int.TryParse(p.PartnerId.Trim(), out var n) ? n : 0)
                    .DefaultIfEmpty(0)
                    .Max();
                _partner = new Partner
                {
                    PartnerId = (maxId + 1).ToString()
                };
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
            // Validation
            if (string.IsNullOrWhiteSpace(_partner.PartnerName))
            {
                System.Windows.MessageBox.Show("Поле 'Название партнёра' не должно быть пустым", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(_partner.Inn) || !Regex.IsMatch(_partner.Inn, "^\\d{10,12}$"))
            {
                System.Windows.MessageBox.Show("ИНН должен содержать 10 или 12 цифр", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(_partner.Phone) && !Regex.IsMatch(_partner.Phone, "^[\\d\\s\\-\\+]+$"))
            {
                System.Windows.MessageBox.Show("Телефон может содержать только цифры, пробелы, '+' и '-'.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (_isNew)
                {
                    // Auto-generate PartnerId: next numeric ID
                    var maxId = _context.Partners
                        .AsEnumerable()
                        .Select(p => int.TryParse(p.PartnerId.Trim(), out var n) ? n : 0)
                        .DefaultIfEmpty(0)
                        .Max();
                    _partner.PartnerId = (maxId + 1).ToString();
                    _context.Partners.Add(_partner);
                }
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
