namespace FitApp.Application.Interfaces;

using System;
using System.Threading.Tasks;

public interface IRedisCache
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan ttl);
}