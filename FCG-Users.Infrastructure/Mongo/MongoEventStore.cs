using FCG.Shared.Contracts.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Text.Json;

namespace FCG_Users.Infrastructure.Mongo
{
    public class MongoEventStore : IEventStore
    {
        private readonly IMongoCollection<StoredEvent> _collection;

        public MongoEventStore(IMongoClient client, IConfiguration config)
        {
            var db = client.GetDatabase(config["MongoSettings:Database"]);
            _collection = db.GetCollection<StoredEvent>("events");

            var indexKeys = Builders<StoredEvent>.IndexKeys
                .Ascending(e => e.AggregateId)
                .Ascending(e => e.Version);

            var indexModel = new CreateIndexModel<StoredEvent>(
                indexKeys,
                new CreateIndexOptions { Unique = true });

            _collection.Indexes.CreateOne(indexModel);

        }

        public async Task AppendAsync<T>(string aggregateId, T evt, int version, string correlationId)
        {            
            var lastEvent = await _collection
                .Find(e => e.AggregateId == aggregateId)
                .SortByDescending(e => e.Version)
                .FirstOrDefaultAsync();

            var currentVersion = lastEvent?.Version ?? 0;
                        
            if (currentVersion != version)
                            throw new InvalidOperationException($"Conflito de concorrência. A versão esperada é {version}, mas a versão corrente é {currentVersion}");
            
            var nextVersion = version + 1;

            var stored = new StoredEvent
            {
                AggregateId = aggregateId,
                EventType = evt!.GetType().AssemblyQualifiedName!,
                Data = JsonSerializer.Serialize(evt),
                OccurredAt = DateTime.UtcNow,
                Version = nextVersion,
                CorrelationId = correlationId
            };

            await _collection.InsertOneAsync(stored);

        }

        public async Task<IReadOnlyList<IDomainEvent>> GetEventsAsync(string aggregateId)
        {
            var storedEvents = await _collection
                 .Find(e => e.AggregateId == aggregateId)
                 .SortBy(e => e.Version)
                 .ToListAsync();

            var domainEvents = new List<IDomainEvent>();

            foreach (var stored in storedEvents)
            {
                var eventType = Type.GetType(stored.EventType);

                if (eventType == null)
                    throw new InvalidOperationException($"Tipo de evento '{stored.EventType}' não encontrado.");

                var evt = (IDomainEvent)JsonSerializer.Deserialize(stored.Data, eventType)!;
                domainEvents.Add(evt);
            }

            return domainEvents;

        }
    }
}
