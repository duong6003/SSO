using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Contexts
{
    public interface IRedisDatabaseProvider
    {
        IDatabase GetDatabase();
    }
    public class RedisDBContext : IRedisDatabaseProvider
    {
        private ConnectionMultiplexer? ConnectionMultiplexer;
        private readonly IConfiguration Configuration;
        public RedisDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IDatabase GetDatabase()
        {
            if (ConnectionMultiplexer == null)
            {
                string connection = Configuration["DatabaseSettings:RedisSettings:Connectionstrings:DefaultConnection"];
                ConnectionMultiplexer = ConnectionMultiplexer.Connect(connection);
            }
            return ConnectionMultiplexer.GetDatabase();
        }
    }
    public static class RedisExtensions
    {
        public static async Task SetRecordAsync<T>(this IDatabase database, string? key, T? data, TimeSpan? expiredTime)
        {
            string jsondata = JsonConvert.SerializeObject(data, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            await database.StringSetAsync(key, jsondata, expiredTime);
        }
        public static async Task<T?> GetRecordAsync<T>(this IDatabase database, string? key)
        {
            if(key is null)
                return default(T);
            string jsonData = await database.StringGetAsync(key);
            if (jsonData is null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(jsonData); ;
        }
        public static async Task DeleteRecordAsync(this IDatabase database, string? key)
        {
            if (key is null)
                return;
                await database.KeyDeleteAsync(key);
        }
    }
}