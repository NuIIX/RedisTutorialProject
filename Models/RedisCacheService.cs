using StackExchange.Redis;

namespace SimpleExampleUsingRedis.Models
{
    public interface IRedisCacheService
    {
        Task<string> GetAsync(string key);
        Task SetAsync(string key, string value);
    }

    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService(IConfiguration configuration)
        {
            var redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));
            _database = redis.GetDatabase();
        }

        public async Task<string> GetAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task SetAsync(string key, string value)
        {
            await _database.StringSetAsync(key, value);
        }
    }

}
