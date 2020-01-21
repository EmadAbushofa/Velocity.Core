namespace Velocity.Core
{
    public interface IUnitOfWorkFactory<out TUnitOfWork>
    {
        TUnitOfWork NewUnitOfWork();
    }
}
