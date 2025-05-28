using System;
using System.Collections.Generic;
using LendTech.SharedKernel.Enums;
namespace LendTech.SharedKernel.Models;
/// <summary>
/// مدل استاندارد پاسخ API
/// </summary>
public class ApiResponse<T>
{
/// <summary>
/// وضعیت موفقیت
/// </summary>
public bool IsSuccess { get; set; }
/// <summary>
/// کد وضعیت
/// </summary>
public ResponseStatus Status { get; set; }

/// <summary>
/// پیام
/// </summary>
public string? Message { get; set; }

/// <summary>
/// داده
/// </summary>
public T? Data { get; set; }

/// <summary>
/// لیست خطاها
/// </summary>
public Dictionary<string, string[]>? Errors { get; set; }

/// <summary>
/// زمان پاسخ
/// </summary>
public DateTime Timestamp { get; set; } = DateTime.UtcNow;

/// <summary>
/// شناسه ردیابی
/// </summary>
public string? TraceId { get; set; }

/// <summary>
/// ایجاد پاسخ موفق
/// </summary>
public static ApiResponse<T> SuccessResult(T? data = default, string? message = null)
{
    return new ApiResponse<T>
    {
        IsSuccess = true,
        Status = ResponseStatus.Success,
        Data = data,
        Message = message ?? "عملیات با موفقیت انجام شد"
    };
}

/// <summary>
/// ایجاد پاسخ خطا
/// </summary>
public static ApiResponse<T> ErrorResult(ResponseStatus status, string message, Dictionary<string, string[]>? errors = null)
{
    return new ApiResponse<T>
    {
        IsSuccess = false,
        Status = status,
        Message = message,
        Errors = errors
    };
}

/// <summary>
/// ایجاد پاسخ خطای اعتبارسنجی
/// </summary>
public static ApiResponse<T> ValidationError(Dictionary<string, string[]> errors)
{
    return new ApiResponse<T>
    {
        IsSuccess = false,
        Status = ResponseStatus.ValidationError,
        Message = "خطای اعتبارسنجی رخ داده است",
        Errors = errors
    };
}

/// <summary>
/// ایجاد پاسخ NotFound
/// </summary>
public static ApiResponse<T> NotFound(string message = "موردی یافت نشد")
{
    return new ApiResponse<T>
    {
        IsSuccess = false,
        Status = ResponseStatus.NotFound,
        Message = message
    };
}

/// <summary>
/// ایجاد پاسخ Unauthorized
/// </summary>
public static ApiResponse<T> Unauthorized(string message = "احراز هویت الزامی است")
{
    return new ApiResponse<T>
    {
        IsSuccess = false,
        Status = ResponseStatus.Unauthorized,
        Message = message
    };
}

/// <summary>
/// ایجاد پاسخ Forbidden
/// </summary>
public static ApiResponse<T> Forbidden(string message = "شما به این بخش دسترسی ندارید")
{
    return new ApiResponse<T>
    {
        IsSuccess = false,
        Status = ResponseStatus.Forbidden,
        Message = message
    };
}
}
/// <summary>
/// مدل پاسخ API بدون داده
/// </summary>
public class ApiResponse : ApiResponse<object>
{
/// <summary>
/// ایجاد پاسخ موفق بدون داده
/// </summary>
public static ApiResponse Success(string? message = null)
{
return new ApiResponse
{
IsSuccess = true,
Status = ResponseStatus.Success,
Message = message ?? "عملیات با موفقیت انجام شد"
};
}
/// <summary>
/// ایجاد پاسخ خطا
/// </summary>
public static ApiResponse Error(ResponseStatus status, string message, Dictionary<string, string[]>? errors = null)
{
    return new ApiResponse
    {
        IsSuccess = false,
        Status = status,
        Message = message,
        Errors = errors
    };
}

/// <summary>
/// ایجاد پاسخ خطای اعتبارسنجی
/// </summary>
public new static ApiResponse ValidationError(Dictionary<string, string[]> errors)
{
    return new ApiResponse
    {
        IsSuccess = false,
        Status = ResponseStatus.ValidationError,
        Message = "خطای اعتبارسنجی رخ داده است",
        Errors = errors
    };
}

/// <summary>
/// ایجاد پاسخ NotFound
/// </summary>
public new static ApiResponse NotFound(string message = "موردی یافت نشد")
{
    return new ApiResponse
    {
        IsSuccess = false,
        Status = ResponseStatus.NotFound,
        Message = message
    };
}

/// <summary>
/// ایجاد پاسخ Unauthorized
/// </summary>
public new static ApiResponse Unauthorized(string message = "احراز هویت الزامی است")
{
    return new ApiResponse
    {
        IsSuccess = false,
        Status = ResponseStatus.Unauthorized,
        Message = message
    };
}

/// <summary>
/// ایجاد پاسخ Forbidden
/// </summary>
public new static ApiResponse Forbidden(string message = "شما به این بخش دسترسی ندارید")
{
    return new ApiResponse
    {
        IsSuccess = false,
        Status = ResponseStatus.Forbidden,
        Message = message
    };
}
}
