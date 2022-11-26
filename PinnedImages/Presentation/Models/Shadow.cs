namespace Presentation.Models
{
    public class Shadow : BindObservable, IOriginator<Shared.Shadow>
    {
        private double _opacity;
        private double _depth;
        private double _direction;
        private double _blurRadius;
        private bool _visible;

        public Shadow(double opacity, double depth, double direction, double blurRadius, bool visible)
        {
            _opacity = opacity;
            _depth = depth;
            _direction = direction;
            _blurRadius = blurRadius;
            _visible = visible;
        }

        public double Opacity
        {
            get => _visible ? _opacity : 0;
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }
        public double Depth
        {
            get => _depth;
            set
            {
                _depth = value;
                OnPropertyChanged(nameof(Depth));
            }
        }
        public double Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                OnPropertyChanged(nameof(Direction));
            }
        }
        public double BlurRadius
        {
            get => _blurRadius;
            set
            {
                _blurRadius = value;
                OnPropertyChanged(nameof(BlurRadius));
            }
        }
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                OnPropertyChanged(nameof(Visible));
                OnPropertyChanged(nameof(Opacity));
            }
        }

        public Shared.Shadow CreateMemento()
        {
            return new Shared.Shadow(
                Opacity: _opacity,
                Depth: _depth,
                Direction: _depth,
                BlurRadius: _blurRadius,
                IsVisible: _visible);
        }

        public void Restore(Shared.Shadow memento)
        {
            Opacity = memento.Opacity;
            Depth = memento.Depth;
            Direction = memento.Direction;
            BlurRadius = memento.BlurRadius;
            Visible = memento.IsVisible;
        }
    }
}
