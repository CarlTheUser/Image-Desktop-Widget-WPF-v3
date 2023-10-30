namespace Presentation.Models
{
    public class Dimension : BindObservable, IOriginator<Shared.Dimension>
    {
        private double _width;
        private double _height;

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public Dimension(double width, double height) => (Width, Height) = (width, height);

        public Shared.Dimension CreateMemento()
        {
            return new Shared.Dimension(
                Width: _width,
                Height: _height);
        }

        public void Restore(Shared.Dimension memento)
        {
            Width = memento.Width;
            Height = memento.Height;
        }
    }
}
