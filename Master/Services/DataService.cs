using System.Collections.Generic;
using System.Threading.Tasks;
using Master.Models;

namespace Master.Services
{
    public class DataService : IDataService
    {
        private readonly ContosoPartnersContext _context;

        public DataService(ContosoPartnersContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Partner>> GetPartnersAsync()
        {
            return await Task.FromResult(_context.Partners);
        }

        public async Task<Partner> GetPartnerByIdAsync(string id)
        {
            return await Task.FromResult(_context.Partners.Find(id));
        }

        public async Task<bool> SavePartnerAsync(Partner partner)
        {
            if (string.IsNullOrEmpty(partner.Id))
            {
                _context.Partners.Add(partner);
            }
            else
            {
                _context.Partners.Update(partner);
            }
            
            return await Task.FromResult(_context.SaveChanges() > 0);
        }

        public async Task<bool> DeletePartnerAsync(string id)
        {
            var partner = await GetPartnerByIdAsync(id);
            if (partner != null)
            {
                _context.Partners.Remove(partner);
                return await Task.FromResult(_context.SaveChanges() > 0);
            }
            return false;
        }

        public async Task<IEnumerable<Sale>> GetSalesHistoryAsync()
        {
            return await Task.FromResult(_context.Sales);
        }

        public async Task<bool> SaveSaleAsync(Sale sale)
        {
            _context.Sales.Add(sale);
            return await Task.FromResult(_context.SaveChanges() > 0);
        }
    }
} 