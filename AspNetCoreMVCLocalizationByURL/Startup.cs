using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCoreMVCLocalizationByURL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
               // Add support for finding localized views, based on file name suffix, e.g. Index.fr.cshtml
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
               // Add support for localizing strings in data annotations (e.g. validation messages) via the
               // IStringLocalizer abstractions.
               .AddDataAnnotationsLocalization();


            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("tr-TR"),
                };

                // State what the default culture for your application is. This will be used if no specific culture
                // can be determined for a given request.
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;

                // You can change which providers are configured to determine the culture for requests, or even add a custom
                // provider with your own logic. The providers will be asked in order to provide a culture for each request,
                // and the first to provide a non-null result that is in the configured supported cultures list will be used.
                // By default, the following built-in providers are configured:
                // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
                // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
                // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header

                options.RequestCultureProviders = new[]{ new Override.RouteDataRequestCultureProvider{
                    IndexOfCulture=1,
                    IndexofUICulture=1
                }};
                //options.RequestCultureProviders = new[]{ new QueryStringRequestCultureProvider
                //{
                //    QueryStringKey = "culture",
                //    UIQueryStringKey = "culture"
                //} };
            });


            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("culture", typeof(LanguageRouteConstraint));
            });

            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                        name: "LocalizedDefault",
                        template: "{culture:culture}/{controller=Home}/{action=Index}/{id?}"
                ); routes.MapRoute(
                      name: "default",
                      template: "{*catchall}",
                      defaults: new { controller = "Home", action = "RedirectToDefaultCulture", culture = "en" });
            });

            //var options = new RewriteOptions()
            //    .AddRedirectToHttps();

            //app.UseRewriter(options);
        }

        public class LanguageRouteConstraint : IRouteConstraint
        {
            public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
            {

                if (!values.ContainsKey("culture"))
                    return false;

                var culture = values["culture"].ToString();
                return culture == "en" || culture == "tr";
            }
        }
    }
}
