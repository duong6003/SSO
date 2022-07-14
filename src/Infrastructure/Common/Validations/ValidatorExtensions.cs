using System.Net.Mail;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Common.Validations;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, IFormFile?> IsValidContentType<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, ICollection<string> contentTypes)
    {
        return ruleBuilder.Must(file =>
        {
            if (file is null) 
                return true;
            return contentTypes.Any(x => file.ContentType.StartsWith(x));
        });
    }
    public static IRuleBuilderOptions<T, string?> IsMatchMediaType<T>(this IRuleBuilder<T, string?> ruleBuilder, ICollection<string> types)
    {
        return ruleBuilder.Must(type =>
        {
            if (type is null) 
                return true;
            return types.Any(x => x.Trim().ToLower().Equals(type.Trim().ToLower()));
        });
    }
    
    public static IRuleBuilderOptions<T, string?> IsValidPhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        /*
            - Numbers can start with +84 or 84 (eg +84981234567, 84981234567)
            - The prefixes 03, 05, 07, 08, 09 (eg 0981234567)
        */
        return ruleBuilder.Matches(@"^((((\+?)84)(0{0,1})|0)(3|5|9|8)\d{8})$");
    }
    public static IRuleBuilderOptions<T, string?> IsValidPassword<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        /*
            - At least one number
            - At least 8 characters length
        */
        return ruleBuilder.Matches(@"^.*(?=.{8,128})(?=.*\d)(?=.*[a-z]).*$");
    }
    public static IRuleBuilderOptions<T, string?> IsValidUserName<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        /*
            - allowed characters : ._~!@#$%^&*()_+<>?|]
        */
        return ruleBuilder.Matches("^[a-zA-Z0-9._~!@#$%^&*()_+<>?|]+$");
    }
    public static bool IsValidEmail(this string emailAddress)
    {
         try
        {
            MailAddress m = new MailAddress(emailAddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
    public static bool IsValidPhoneNumber(this string phoneNumber)
    {
        /*
            - Numbers can start with +84 or 84 (eg +84981234567, 84981234567)
            - The prefixes 03, 05, 07, 08, 09 (eg 0981234567)
        */
        return Regex.IsMatch(phoneNumber, @"(((\+|)84)|0)(3|5|7|8|9)+([0-9]{8})\b", RegexOptions.CultureInvariant);
    }
}
