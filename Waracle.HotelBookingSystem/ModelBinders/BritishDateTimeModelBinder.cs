using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Waracle.HotelBookingSystem.Web.Api.ModelBinders
{
    public class BritishDateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            try
            {
                var value = valueProviderResult.FirstValue;
                if (string.IsNullOrWhiteSpace(value))
                {
                    return Task.CompletedTask;
                }

                var culture = new CultureInfo("en-GB");
                var date = DateTime.Parse(value, culture, DateTimeStyles.None);
                bindingContext.Result = ModelBindingResult.Success(date);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Invalid date format. Expected British format (e.g., dd/MM/yyyy or dd/MM/yyyy HH:mm:ss). {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
