using Presentation.ViewModels;
using System.Windows.Controls;

namespace Presentation.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page, IApplicationViewComponent<SettingsPageViewModel>
    {
        public SettingsPageViewModel ViewModel { get; }

        public SettingsPage(SettingsPageViewModel settingsPageViewModel)
        {
            ViewModel = settingsPageViewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }

        public SettingsPageViewModel GetViewModel()
        {
            return ViewModel;
        }
    }
}
