namespace Presentation
{
    public interface IUserNotification
    {
        void Notify();
    }

    public interface IUserNotification<TParameter>
    {
        void Notify(TParameter parameter);
    }
}
