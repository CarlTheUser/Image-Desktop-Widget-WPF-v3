using CommunityToolkit.Mvvm.Input;
using Data.Common.Contracts;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Presentation.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IConfiguration _configuration;

        private readonly IAsyncQuery<IEnumerable<Color>, FileInfo> _colorsQuery;

        private ObservableCollection<Color> _colors = new();

        public IAsyncRelayCommand LoadColorsCommand { get; }

        public IRelayCommand SetColorCommand { get; }

        public ObservableCollection<Color> Colors
        {
            get => _colors;
            set
            {
                _colors = value;
                OnPropertyChanged(nameof(Colors));
            }
        }

        public SettingsPageViewModel(IConfiguration configuration, IAsyncQuery<IEnumerable<Color>, FileInfo> colorsQuery)
        {
            _configuration = configuration;
            _colorsQuery = colorsQuery;

            LoadColorsCommand = new AsyncRelayCommand(execute: LoadColors);

            SetColorCommand = new RelayCommand<Color>(execute: SetColor, canExecute: i => true);
        }

        private async Task LoadColors()
        {
            if (_colors.Any())
            {
                return;
            }

            IEnumerable<Color> colors = await _colorsQuery.ExecuteAsync(
                parameter: new FileInfo(fileName: _configuration["Application:Environment:Paths:ColorsSource"]),
                cancellationToken: CancellationToken.None);

            Colors = new ObservableCollection<Color>(colors);
        }

        private void SetColor(Color color)
        {
            Properties.Settings.Default.PrimaryColor = System.Drawing.Color.FromArgb(
                alpha: color.A,
                red: color.R,
                green: color.G,
                blue: color.B);

            Properties.Settings.Default.Save();
        }
    }
}
