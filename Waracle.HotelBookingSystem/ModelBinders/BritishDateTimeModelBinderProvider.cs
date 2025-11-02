using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Waracle.HotelBookingSystem.Web.Api.ModelBinders
{
    public class BritishDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(DateTime) ||
                context.Metadata.ModelType == typeof(DateTime?))
            {
                return new BritishDateTimeModelBinder();
            }

            return null;
        }
    }
}
