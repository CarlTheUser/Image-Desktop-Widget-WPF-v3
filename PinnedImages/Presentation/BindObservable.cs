using System.ComponentModel;

namespace Presentation
{
    public abstract class BindObservable : INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(sender: this, e: new PropertyChangedEventArgs(propertyName));
        }
    }
}
