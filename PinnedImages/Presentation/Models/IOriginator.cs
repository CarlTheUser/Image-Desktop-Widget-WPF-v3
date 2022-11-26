namespace Presentation.Models
{
    public interface IOriginator<TMemento>
    {
        TMemento CreateMemento();

        void Restore(TMemento memento);
    }
}
