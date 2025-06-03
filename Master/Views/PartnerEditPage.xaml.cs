using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Master.ViewModels;
using Master.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Master.Views
{
    public partial class PartnerEditPage : Page
    {
        private readonly ContosoPartnersContext _context;
        private readonly Partner _partner;
        private readonly bool _isNew;

        public PartnerEditPage(Partner partner = null)
        {
            Log.Debug("Инициализация страницы редактирования партнера");
            InitializeComponent();
            _context = new ContosoPartnersContext();
            if (partner == null)
            {
                Log.Information("Создание нового партнера");
                _isNew = true;
                // Auto-generate PartnerId
                var maxId = _context.Partners
                    .AsEnumerable()
                    .Select(p => int.TryParse(p.PartnerId.Trim(), out var n) ? n : 0)
                    .DefaultIfEmpty(0)
                    .Max();
                _partner = new Partner { PartnerId = (maxId + 1).ToString() };
                Log.Debug("Сгенерирован новый ID партнера: {PartnerId}", _partner.PartnerId);
            }
            else
            {
                Log.Information("Редактирование существующего партнера {PartnerName} (ID: {PartnerId})", partner.PartnerName, partner.PartnerId);
                _isNew = false;
                _partner = _context.Partners.Find(partner.PartnerId)
                    ?? throw new InvalidOperationException("Partner not found: " + partner.PartnerId);
            }
            DataContext = _partner;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Log.Debug("Начало валидации данных партнера {PartnerName}", _partner.PartnerName);
            // Validation
            if (string.IsNullOrWhiteSpace(_partner.PartnerName))
            {
                Log.Warning("Попытка сохранения партнера с пустым названием");
                System.Windows.MessageBox.Show("Поле 'Название партнёра' не должно быть пустым", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(_partner.Inn) || !Regex.IsMatch(_partner.Inn, "^\\d{10,12}$"))
            {
                Log.Warning("Попытка сохранения партнера с некорректным ИНН: {Inn}", _partner.Inn);
                System.Windows.MessageBox.Show("ИНН должен содержать 10 или 12 цифр", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(_partner.Phone) && !Regex.IsMatch(_partner.Phone, "^[\\d\\s\\-\\+]+$"))
            {
                Log.Warning("Попытка сохранения партнера с некорректным телефоном: {Phone}", _partner.Phone);
                System.Windows.MessageBox.Show("Телефон может содержать только цифры, пробелы, '+' и '-'.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Log.Information("Сохранение партнера {PartnerName} (ID: {PartnerId})", _partner.PartnerName, _partner.PartnerId);
                if (_isNew)
                {
                    Log.Debug("Добавление нового партнера в базу данных");
                    _context.Partners.Add(_partner);
                }
                _context.SaveChanges();
                Log.Information("Партнер {PartnerName} успешно сохранен", _partner.PartnerName);
                NavigationService.GoBack();
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex, "Ошибка базы данных при сохранении партнера {PartnerName}", _partner.PartnerName);
                System.Windows.MessageBox.Show("Ошибка базы данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Log.Debug("Отмена редактирования партнера {PartnerName}", _partner.PartnerName);
            NavigationService.GoBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Log.Debug("Возврат к списку партнеров");
            NavigationService.GoBack();
        }
    }
} 