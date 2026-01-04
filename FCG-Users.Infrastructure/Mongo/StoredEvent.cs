namespace FCG_Users.Infrastructure.Mongo
{
    public class StoredEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string AggregateId { get; set; } = default!;         
        public string EventType { get; set; } = default!;           
        public string Data { get; set; } = default!; 
        public DateTime OccurredAt { get; set; }
        public int Version { get; set; }
        public string? CorrelationId { get; set; }
    }
}
