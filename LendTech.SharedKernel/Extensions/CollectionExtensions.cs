using System;
using System.Collections.Generic;
using System.Linq;

namespace LendTech.SharedKernel.Extensions;

/// <summary>
/// Extension متدهای مجموعه‌ها
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// بررسی خالی بودن مجموعه
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }

    /// <summary>
    /// بررسی خالی نبودن مجموعه
    /// </summary>
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return !collection.IsNullOrEmpty();
    }

    /// <summary>
    /// اجرای عملیات روی هر عضو مجموعه
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
        {
            action(item);
        }
    }

    /// <summary>
    /// اجرای عملیات روی هر عضو مجموعه با اندیس
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
    {
        var index = 0;
        foreach (var item in collection)
        {
            action(item, index++);
        }
    }

    /// <summary>
    /// تقسیم مجموعه به دسته‌های کوچکتر
    /// </summary>
    public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
    {
        var batch = new List<T>(batchSize);
        
        foreach (var item in collection)
        {
            batch.Add(item);
            
            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }

    /// <summary>
    /// حذف تکراری‌ها بر اساس یک ویژگی
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        
        foreach (var item in collection)
        {
            if (seenKeys.Add(keySelector(item)))
                yield return item;
        }
    }

    /// <summary>
    /// تبدیل به HashSet
    /// </summary>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
    {
        return new HashSet<T>(collection);
    }

    /// <summary>
    /// تبدیل به HashSet با کلید مشخص
    /// </summary>
    public static HashSet<TKey> ToHashSet<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector)
    {
        return new HashSet<TKey>(collection.Select(keySelector));
    }

    /// <summary>
    /// اضافه کردن آیتم در صورت عدم وجود
    /// </summary>
    public static bool AddIfNotExists<T>(this ICollection<T> collection, T item)
    {
        if (!collection.Contains(item))
        {
            collection.Add(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// اضافه کردن محدوده آیتم‌ها
    /// </summary>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        items.ForEach(collection.Add);
    }

    /// <summary>
    /// حذف محدوده آیتم‌ها
    /// </summary>
    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        items.ForEach(item => collection.Remove(item));
    }

    /// <summary>
    /// دریافت صفحه از مجموعه
    /// </summary>
    public static IEnumerable<T> GetPage<T>(this IEnumerable<T> collection, int pageNumber, int pageSize)
    {
        return collection.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// مرتب‌سازی بر اساس چند معیار
    /// </summary>
    public static IOrderedEnumerable<T> OrderByMultiple<T>(this IEnumerable<T> collection, params Func<T, object>[] keySelectors)
    {
        if (keySelectors.Length == 0)
            return collection.OrderBy(x => x);

        var orderedQuery = collection.OrderBy(keySelectors[0]);
        
        for (int i = 1; i < keySelectors.Length; i++)
        {
            orderedQuery = orderedQuery.ThenBy(keySelectors[i]);
        }

        return orderedQuery;
    }

    /// <summary>
    /// انتخاب تصادفی از مجموعه
    /// </summary>
    public static T? PickRandom<T>(this IList<T> collection)
    {
        if (collection.IsNullOrEmpty()) return default;
        
        var random = new Random();
        return collection[random.Next(collection.Count)];
    }

    /// <summary>
    /// انتخاب چند آیتم تصادفی از مجموعه
    /// </summary>
    public static IEnumerable<T> PickRandom<T>(this IList<T> collection, int count)
    {
        if (collection.IsNullOrEmpty() || count <= 0) yield break;
        
        var random = new Random();
        var indexes = new HashSet<int>();
        
        while (indexes.Count < Math.Min(count, collection.Count))
        {
            indexes.Add(random.Next(collection.Count));
        }

        foreach (var index in indexes)
        {
            yield return collection[index];
        }
    }
}
