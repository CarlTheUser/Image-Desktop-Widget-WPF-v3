using CommunityToolkit.Mvvm.Input;
using Presentation.Pages;

namespace Presentation.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ApplicationPage _currentPage;

        public ApplicationPage CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public RelayCommand<ApplicationPage> NavigateToPageCommand { get; }

        public MainWindowViewModel()
        {
            _currentPage = ApplicationPage.MainPage;

            NavigateToPageCommand = new RelayCommand<ApplicationPage>(
                execute: NavigateToPage,
                canExecute: CanNavigateTo);
        }

        private void NavigateToPage(ApplicationPage target)
        {
            CurrentPage = target;
            NavigateToPageCommand.NotifyCanExecuteChanged();
        }

        private bool CanNavigateTo(ApplicationPage page)
        {
            bool canNavigate = page != _currentPage;
            return canNavigate;
        }
    }
}
