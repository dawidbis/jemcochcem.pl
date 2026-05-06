namespace FitApp.Infrastructure.Cache;

using FitApp.Application.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

public class RedisCacheService : IRedisCache
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public Task<T> GetAsync<T>(string key) => throw new NotImplementedException();
    public Task SetAsync<T>(string key, T value, TimeSpan ttl) => throw new NotImplementedException();
}