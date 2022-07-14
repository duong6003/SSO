using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class DateTimeModelBinder : IModelBinder
{
    public static readonly Type[] SUPPORTED_TYPES = new Type[] { typeof(DateTime), typeof(DateTime?) };

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        if (!SUPPORTED_TYPES.Contains(bindingContext.ModelType))
        {
            return Task.CompletedTask;
        }

        var modelName = GetModelName(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var dateToParse = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(dateToParse))
        {
            return Task.CompletedTask;
        }

        var dateTime = Helper.ParseDateTime(dateToParse);

        bindingContext.Result = ModelBindingResult.Success(dateTime);

        return Task.CompletedTask;
    }

    private string GetModelName(ModelBindingContext bindingContext)
    {
        if (!string.IsNullOrEmpty(bindingContext.BinderModelName))
        {
            return bindingContext.BinderModelName;
        }

        return bindingContext.ModelName;
    }
}


public class Helper
{
    public static DateTime? ParseDateTime(
        string dateToParse,
        string[]? formats = null,
        IFormatProvider? provider = null,
        DateTimeStyles styles = DateTimeStyles.None)
    {
        var CUSTOM_DATE_FORMATS = new string[]
            {    
            "dd-MM-yyyy",
            "dd/MM/yyyy"
            };

        if (formats == null || !formats.Any())
        {
            formats = CUSTOM_DATE_FORMATS;
        }

        DateTime validDate;

        foreach (var format in formats)
        {
            if (format.EndsWith("Z"))
            {
                if (DateTime.TryParseExact(dateToParse, format,
                         provider,
                         DateTimeStyles.AssumeUniversal,
                         out validDate))
                {
                    return validDate;
                }
            }

            if (DateTime.TryParseExact(dateToParse, format,
                     provider, styles, out validDate))
            {
                return validDate;
            }
        }
        return null;
    }
}