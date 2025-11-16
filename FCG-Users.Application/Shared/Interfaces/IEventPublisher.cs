namespace FCG_Users.Application.Shared.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T evt, string subject);
    }
}
