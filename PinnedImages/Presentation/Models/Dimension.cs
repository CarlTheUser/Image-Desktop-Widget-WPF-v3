namespace Presentation.Models
{
    public class Dimension : BindObservable, IOriginator<Shared.Dimension>
    {
        private double _width;
        private double _height;

        public Dimension(double width, double height)
        {
            _width = width;
            _height = height;
        }

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

        public Shared.Dimension CreateMemento()
        {
            return new Shared.Dimension(
                Width: Width,
                Height: Height);
        }

        public void Restore(Shared.Dimension memento)
        {
            Width = memento.Width;
            Height = memento.Height;
        }
    }
}
