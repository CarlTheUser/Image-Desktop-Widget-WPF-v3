using Presentation.ViewModels;

namespace Presentation
{
    public interface IApplicationViewComponent<TViewModel> where TViewModel : ViewModelBase
    {
        TViewModel GetViewModel();

        virtual bool OnNavigateRequested()
        {
            return true;
        }
    }
}
