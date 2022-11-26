using Presentation.ViewModels;
using System.Windows.Controls;

namespace Presentation.Pages
{
    /// <summary>
    /// Interaction logic for LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page, IApplicationViewComponent<LibraryPageViewModel>
    {
        public LibraryPageViewModel ViewModel { get; }

        public LibraryPage(LibraryPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        public LibraryPageViewModel GetViewModel()
        {
            return ViewModel;
        }
    }
}
