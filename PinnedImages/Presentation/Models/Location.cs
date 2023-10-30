namespace Presentation.Models
{
    public class Location : BindObservable, IOriginator<Shared.Location>
    {
        private double _x;
        private double _y;

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        public Location(double x, double y) => (X, Y) = (x, y);

        public Shared.Location CreateMemento()
        {
            return new Shared.Location(
                X: _x,
                Y: _y);
        }

        public void Restore(Shared.Location memento)
        {
            X = memento.X;
            Y = memento.Y;
        }
    }
}
