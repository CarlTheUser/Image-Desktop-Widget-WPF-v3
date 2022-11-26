using Presentation.ViewModels;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IApplicationViewComponent<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindowViewModel GetViewModel()
        {
            return VM;
        }
    }
}
