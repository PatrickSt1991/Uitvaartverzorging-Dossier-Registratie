using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Interfaces;
using Dossier_Registratie.Views;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationBeheerViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private ImageSource _imageSource;
        private bool _isAdminButtonActive = false;
        private object _currentView;
        private string _windowTitle;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }
        public bool IsAdminButtonActive
        {
            get { return _isAdminButtonActive; }
            set
            {
                if (_isAdminButtonActive != value)
                {
                    _isAdminButtonActive = value;
                    OnPropertyChanged(nameof(IsAdminButtonActive));
                }
            }
        }
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value; OnPropertyChanged(nameof(CurrentView));
            }
        }
        public string WindowTitle
        {
            get => _windowTitle;
            set { _windowTitle = value; OnPropertyChanged(nameof(WindowTitle)); }
        }

        public ICommand ShowWerknemersViewCommand { get; }
        public ICommand ShowVerzekeraarsViewCommand { get; }
        public ICommand ShowOverlijdenLocatiesViewCommand { get; }
        public ICommand ShowLeveranciersViewCommand { get; }
        public ICommand ShowKistenViewCommand { get; }
        public ICommand ShowAsbestemmingenViewCommand { get; }
        public ICommand ShowFinancieelViewCommand { get; }
        public ICommand ShowRapportagesViewCommand { get; }
        public ICommand ShowPriceComponentsViewCommand { get; }
        public ICommand ShowInstellingenViewCommand { get; }
        public ICommand ShowRouwbrievenViewCommand { get; }
        public ConfigurationBeheerViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            ShowWerknemersViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationWerknemersView());
            ShowVerzekeraarsViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationVerzekeraarsView());
            ShowLeveranciersViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationLeveranciersView());
            ShowOverlijdenLocatiesViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationOverlijdenLocatiesView());
            ShowKistenViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationKistenView());
            ShowAsbestemmingenViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationAsbestemmingenView());
            ShowFinancieelViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationFinancieelView());
            ShowRapportagesViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationRapportagesView());
            ShowPriceComponentsViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationPriceComponentsView());
            ShowInstellingenViewCommand = new AdminRelayCommand(() => CurrentView = new GeneralSetup());
            ShowRouwbrievenViewCommand = new AdminRelayCommand(() => CurrentView = new ConfigurationRouwbrievenView());

            CurrentView = new ConfigurationWerknemersView();
            WindowTitle = DataProvider.OrganizationName + " - " + DataProvider.SystemTitle + " - " + "Beheer";
            LoadImageFromDatabase();

            if (Globals.PermissionLevelName == "System" || Globals.PermissionLevelName == "Financieel")
                IsAdminButtonActive = true;
        }
        public void LoadImageFromDatabase()
        {
            var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Backend");

            if (documentData != null && documentData.Length > 0)
            {
                using (var stream = new MemoryStream(documentData))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImageSource = bitmap;
                }
            }
        }
    }
}