
namespace FCG_Users.Application.Users.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T evt, string subject);
    }
}
