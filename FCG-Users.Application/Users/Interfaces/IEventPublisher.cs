
namespace FCG_Users.Application.Users.Interfaces
{
    public record UserDeleted(Guid ItemId);
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T evt, string subject);
    }
}
