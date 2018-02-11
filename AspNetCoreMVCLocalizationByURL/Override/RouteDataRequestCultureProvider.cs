using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace AspNetCoreMVCLocalizationByURL.Override
{
    public class RouteDataRequestCultureProvider : RequestCultureProvider
    {
        public int IndexOfCulture;
        public int IndexofUICulture;

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            string culture = null;
            string uiCulture = null;

            var twoLetterCultureName = httpContext.Request.Path.Value.Split('/')[IndexOfCulture]?.ToString();
            var twoLetterUICultureName = httpContext.Request.Path.Value.Split('/')[IndexofUICulture]?.ToString();

            if (twoLetterCultureName == "tr")
                culture = "tr-TR";
            else if (twoLetterCultureName == "en")
                culture = uiCulture = "en-US";

            if (twoLetterUICultureName == "tr")
                culture = "tr-TR";
            else if (twoLetterUICultureName == "en")
                culture = uiCulture = "en-US";

            if (culture == null && uiCulture == null)
                return NullProviderCultureResult;
       
            if (culture != null && uiCulture == null)
                uiCulture = culture;

            if (culture == null && uiCulture != null)
                culture = uiCulture;

            var providerResultCulture = new ProviderCultureResult(culture, uiCulture);

            return Task.FromResult(providerResultCulture);
        }
    }
}
