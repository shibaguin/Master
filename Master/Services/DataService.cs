using System.Collections.Generic;
using System.Threading.Tasks;
using Master.Models;
using Serilog;

namespace Master.Services
{
    public class DataService : IDataService
    {
        private readonly ContosoPartnersContext _context;

        public DataService(ContosoPartnersContext context)
        {
            _context = context;
            Log.Information("DataService инициализирован");
        }

        public async Task<IEnumerable<Partner>> GetPartnersAsync()
        {
            Log.Debug("Получение списка партнеров");
            var partners = await Task.FromResult(_context.Partners);
            Log.Information("Получено {Count} партнеров", partners.Count());
            return partners;
        }

        public async Task<Partner> GetPartnerByIdAsync(string id)
        {
            Log.Debug("Получение партнера по ID: {PartnerId}", id);
            var partner = await Task.FromResult(_context.Partners.Find(id));
            if (partner == null)
            {
                Log.Warning("Партнер с ID {PartnerId} не найден", id);
            }
            else
            {
                Log.Information("Партнер {PartnerName} (ID: {PartnerId}) успешно найден", partner.PartnerName, id);
            }
            return partner;
        }

        public async Task<bool> SavePartnerAsync(Partner partner)
        {
            Log.Debug("Сохранение партнера {PartnerName} (ID: {PartnerId})", partner.PartnerName, partner.Id);
            if (string.IsNullOrEmpty(partner.Id))
            {
                Log.Information("Добавление нового партнера {PartnerName}", partner.PartnerName);
                _context.Partners.Add(partner);
            }
            else
            {
                Log.Information("Обновление существующего партнера {PartnerName} (ID: {PartnerId})", partner.PartnerName, partner.Id);
                _context.Partners.Update(partner);
            }
            
            var result = await Task.FromResult(_context.SaveChanges() > 0);
            Log.Information("Сохранение партнера {PartnerName} {Result}", partner.PartnerName, result ? "успешно" : "не удалось");
            return result;
        }

        public async Task<bool> DeletePartnerAsync(string id)
        {
            Log.Debug("Удаление партнера с ID: {PartnerId}", id);
            var partner = await GetPartnerByIdAsync(id);
            if (partner != null)
            {
                Log.Information("Удаление партнера {PartnerName} (ID: {PartnerId})", partner.PartnerName, id);
                _context.Partners.Remove(partner);
                var result = await Task.FromResult(_context.SaveChanges() > 0);
                Log.Information("Удаление партнера {PartnerName} {Result}", partner.PartnerName, result ? "успешно" : "не удалось");
                return result;
            }
            Log.Warning("Попытка удаления несуществующего партнера с ID: {PartnerId}", id);
            return false;
        }

        public async Task<IEnumerable<Sale>> GetSalesHistoryAsync()
        {
            Log.Debug("Получение истории продаж");
            var sales = await Task.FromResult(_context.Sales);
            Log.Information("Получено {Count} записей продаж", sales.Count());
            return sales;
        }

        public async Task<bool> SaveSaleAsync(Sale sale)
        {
            Log.Debug("Сохранение продажи для партнера {PartnerId}", sale.PartnerId);
            _context.Sales.Add(sale);
            var result = await Task.FromResult(_context.SaveChanges() > 0);
            Log.Information("Сохранение продажи {Result}", result ? "успешно" : "не удалось");
            return result;
        }
    }
} 