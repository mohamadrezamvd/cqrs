using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace LendTech.SharedKernel.Helpers;

/// <summary>
/// کلاس کمکی برای اعتبارسنجی
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// اعتبارسنجی یک شیء با استفاده از Data Annotations
    /// </summary>
    public static (bool IsValid, List<ValidationResult> Errors) ValidateObject(object obj)
    {
        var context = new ValidationContext(obj, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        
        bool isValid = Validator.TryValidateObject(obj, context, results, true);
        
        return (isValid, results);
    }

    /// <summary>
    /// اعتبارسنجی کد ملی
    /// </summary>
    public static bool IsValidNationalCode(string nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
            return false;

        nationalCode = nationalCode.Trim();

        // بررسی طول
        if (nationalCode.Length != 10)
            return false;

        // بررسی عددی بودن
        if (!nationalCode.All(char.IsDigit))
            return false;

        // بررسی یکسان نبودن تمام ارقام
        if (nationalCode.Distinct().Count() == 1)
            return false;

        // محاسبه و بررسی رقم کنترل
        var check = Convert.ToInt32(nationalCode[9].ToString());
        var sum = 0;
        
        for (var i = 0; i < 9; i++)
        {
            sum += Convert.ToInt32(nationalCode[i].ToString()) * (10 - i);
        }
        
        var remainder = sum % 11;
        
        return (remainder < 2 && check == remainder) || (remainder >= 2 && check == 11 - remainder);
    }

    /// <summary>
    /// اعتبارسنجی شماره شبا
    /// </summary>
    public static bool IsValidIban(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return false;

        // حذف فاصله‌ها
        iban = iban.Replace(" ", "").ToUpper();

        // بررسی طول و فرمت برای ایران
        if (!Regex.IsMatch(iban, @"^IR\d{24}$"))
            return false;

        // الگوریتم IBAN
        var ibanDigits = iban.Substring(4) + "1827" + iban.Substring(2, 2);
        var numericIban = "";
        
        foreach (var c in ibanDigits)
        {
            if (char.IsDigit(c))
                numericIban += c;
            else
                numericIban += (c - 'A' + 10).ToString();
        }

        // محاسبه mod 97
        var remainder = 0;
        foreach (var digit in numericIban)
        {
            remainder = (remainder * 10 + int.Parse(digit.ToString())) % 97;
        }

        return remainder == 1;
    }

    /// <summary>
    /// اعتبارسنجی کد پستی ایران
    /// </summary>
    public static bool IsValidPostalCode(string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return false;

        postalCode = postalCode.Replace("-", "").Trim();
        
        return Regex.IsMatch(postalCode, @"^\d{10}$");
    }

    /// <summary>
    /// اعتبارسنجی URL
    /// </summary>
    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// اعتبارسنجی شماره کارت بانکی
    /// </summary>
    public static bool IsValidBankCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        if (cardNumber.Length != 16 || !cardNumber.All(char.IsDigit))
            return false;

        // الگوریتم Luhn
        var sum = 0;
        var isEven = false;
        
        for (var i = cardNumber.Length - 1; i >= 0; i--)
        {
            var digit = int.Parse(cardNumber[i].ToString());
            
            if (isEven)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }
            
            sum += digit;
            isEven = !isEven;
        }

        return sum % 10 == 0;
    }
}
