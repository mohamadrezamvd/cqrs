using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LendTech.SharedKernel.Extensions;

/// <summary>
/// Extension متدهای JSON
/// </summary>
public static class JsonExtensions
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private static readonly JsonSerializerOptions IndentedOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// سریالایز به JSON
    /// </summary>
    public static string ToJson<T>(this T obj, bool indented = false)
    {
        return JsonSerializer.Serialize(obj, indented ? IndentedOptions : DefaultOptions);
    }

    /// <summary>
    /// دیسریالایز از JSON
    /// </summary>
    public static T? FromJson<T>(this string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        
        try
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// تلاش برای دیسریالایز از JSON
    /// </summary>
    public static bool TryFromJson<T>(this string json, out T? result)
    {
        result = default;
        
        if (string.IsNullOrWhiteSpace(json)) return false;
        
        try
        {
            result = JsonSerializer.Deserialize<T>(json, DefaultOptions);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// کلون عمیق با استفاده از JSON
    /// </summary>
    public static T? DeepClone<T>(this T obj)
    {
        if (obj == null) return default;
        
        var json = obj.ToJson();
        return json.FromJson<T>();
    }

    /// <summary>
    /// تبدیل به JsonDocument
    /// </summary>
    public static JsonDocument? ToJsonDocument(this string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        
        try
        {
            return JsonDocument.Parse(json);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// دریافت مقدار از JsonElement
    /// </summary>
    public static T? GetValue<T>(this JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            try
            {
                return JsonSerializer.Deserialize<T>(property.GetRawText(), DefaultOptions);
            }
            catch
            {
                return default;
            }
        }
        
        return default;
    }

    /// <summary>
    /// بررسی معتبر بودن JSON
    /// </summary>
    public static bool IsValidJson(this string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        
        try
        {
            using var doc = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
