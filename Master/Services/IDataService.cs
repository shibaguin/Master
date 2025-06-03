using System.Collections.Generic;
using System.Threading.Tasks;
using Master.Models;

namespace Master.Services
{
    public interface IDataService
    {
        Task<IEnumerable<Partner>> GetPartnersAsync();
        Task<Partner> GetPartnerByIdAsync(string id);
        Task<bool> SavePartnerAsync(Partner partner);
        Task<bool> DeletePartnerAsync(string id);
        Task<IEnumerable<Sale>> GetSalesHistoryAsync();
        Task<bool> SaveSaleAsync(Sale sale);
    }
} 