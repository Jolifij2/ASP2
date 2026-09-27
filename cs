using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

// Интерфейс кэша
public interface ICache
{
    void Store(string key, string value);
    string Retrieve(string key);
}

// Кэш в памяти
public class MemoryCache : ICache
{
    private readonly Dictionary<string, string> _cache = new();

    public void Store(string key, string value)
    {
        _cache[key] = value;
        Console.WriteLine($"Сохранено в память: {key} = {value}");
    }

    public string Retrieve(string key)
    {
        return _cache.TryGetValue(key, out var value) ? value : null;
    }
}

// Кэш в файле
public class FileCache : ICache
{
    public void Store(string key, string value)
    {
        Console.WriteLine($"Сохранено в файл: {key} = {value}");
    }

    public string Retrieve(string key)
    {
        Console.WriteLine($"Извлечено из файла: {key}");
        return $"значение для {key}";
    }
}

// Сервис, принимающий ICache через конструктор
public class CacheService
{
    private readonly ICache _cache;

    public CacheService(ICache cache)
    {
        _cache = cache;
    }

    public void CacheAndRetrieve(string key, string value)
    {
        _cache.Store(key, value);
        Console.WriteLine($"Получено из кэша: {_cache.Retrieve(key)}");
    }
}

public class Program
{
    public static void Main()
    {
        var services = new ServiceCollection();

        // Singleton — кэш должен быть общим для всего приложения
        services.AddSingleton<ICache, MemoryCache>();
        services.AddSingleton<CacheService>();

        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<CacheService>().CacheAndRetrieve("user:1", "Иван");
    }
}
