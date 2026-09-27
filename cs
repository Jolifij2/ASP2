using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

// Интерфейс ICache — контракт для работы с кэшем.
// Любая реализация должна уметь сохранять значение по ключу и извлекать его.
public interface ICache
{
    // Сохраняет значение value по ключу key.
    void Store(string key, string value);

    // Возвращает значение по ключу key или null, если ключ не найден.
    string Retrieve(string key);
}

// Реализация кэша в оперативной памяти.
// Данные хранятся в обычном словаре Dictionary.
// Выбран Singleton, потому что кэш должен быть общим для всего приложения:
// если бы он был Scoped или Transient, каждый запрос получал бы свой пустой кэш,
// и кэширование теряло бы смысл.
public class MemoryCache : ICache
{
    // Словарь для хранения пар "ключ — значение".
    private readonly Dictionary<string, string> _cache = new();

    // Сохраняем значение в словарь.
    public void Store(string key, string value)
    {
        _cache[key] = value;
        Console.WriteLine($"Сохранено в память: {key} = {value}");
    }

    // Извлекаем значение по ключу. Если ключа нет — возвращаем null.
    public string Retrieve(string key)
    {
        return _cache.TryGetValue(key, out var value) ? value : null;
    }
}

// Реализация файлового кэша.
// Данные сохраняются во временный файл (имитация — вывод в консоль).
// Выбран Singleton, так как файловый кэш также должен быть единым для приложения,
// чтобы разные части программы работали с одним и тем же хранилищем.
public class FileCache : ICache
{
    // Сохраняем значение "во временный файл" (имитация через консоль).
    public void Store(string key, string value)
    {
        Console.WriteLine($"Сохранено в файл: {key} = {value}");
    }

    // Извлекаем значение "из временного файла" (имитация через консоль).
    public string Retrieve(string key)
    {
        Console.WriteLine($"Извлечено из файла: {key}");
        return $"значение для {key}";
    }
}

// Сервис CacheService — принимает ICache через конструктор.
// Это и есть внедрение зависимости (DI): сервис не создаёт кэш сам,
// а получает его извне от DI-контейнера.
public class CacheService
{
    // Приватное поле для хранения зависимости.
    private readonly ICache _cache;

    // Конструктор, через который контейнер передаёт реализацию ICache.
    public CacheService(ICache cache)
    {
        _cache = cache;
    }

    // Метод, который сохраняет значение и сразу извлекает его.
    public void CacheAndRetrieve(string key, string value)
    {
        // Сохраняем значение в кэш.
        _cache.Store(key, value);

        // Извлекаем значение из кэша.
        var retrieved = _cache.Retrieve(key);

        // Выводим результат.
        Console.WriteLine($"Получено из кэша: {retrieved}");
    }
}

// Точка входа в программу.
public class Program
{
    public static void Main(string[] args)
    {
        // Создаём коллекцию сервисов — это и есть DI-контейнер.
        var services = new ServiceCollection();

        // Регистрируем ICache с реализацией MemoryCache.
        // Выбран Singleton, потому что кэш должен жить всё время работы приложения
        // и быть общим для всех, кто его запрашивает.
        services.AddSingleton<ICache, MemoryCache>();

        // Регистрируем CacheService.
        // Выбран Singleton, так как сервис не хранит состояния и может быть общим.
        services.AddSingleton<CacheService>();

        // Строим провайдер сервисов.
        var serviceProvider = services.BuildServiceProvider();

        // Запрашиваем CacheService из контейнера.
        var cacheService = serviceProvider.GetRequiredService<CacheService>();

        // Вызываем метод сервиса.
        cacheService.CacheAndRetrieve("user:1", "Иван");
    }
}
