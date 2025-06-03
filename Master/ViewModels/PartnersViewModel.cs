using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Master.Models;
using Master.Services;

namespace Master.ViewModels
{
    public class PartnersViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private ObservableCollection<Partner> _partners;
        private Partner _selectedPartner;

        public PartnersViewModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadPartnersCommand = new RelayCommand(async () => await LoadPartners());
            SavePartnerCommand = new RelayCommand<Partner>(async (partner) => await SavePartner(partner));
            DeletePartnerCommand = new RelayCommand<Partner>(async (partner) => await DeletePartner(partner));
        }

        public ObservableCollection<Partner> Partners
        {
            get => _partners;
            set => SetProperty(ref _partners, value);
        }

        public Partner SelectedPartner
        {
            get => _selectedPartner;
            set => SetProperty(ref _selectedPartner, value);
        }

        public ICommand LoadPartnersCommand { get; }
        public ICommand SavePartnerCommand { get; }
        public ICommand DeletePartnerCommand { get; }

        private async Task LoadPartners()
        {
            var partners = await _dataService.GetPartnersAsync();
            Partners = new ObservableCollection<Partner>(partners);
        }

        private async Task SavePartner(Partner partner)
        {
            if (partner != null)
            {
                await _dataService.SavePartnerAsync(partner);
                await LoadPartners();
            }
        }

        private async Task DeletePartner(Partner partner)
        {
            if (partner != null)
            {
                await _dataService.DeletePartnerAsync(partner.Id);
                await LoadPartners();
            }
        }
    }
} 