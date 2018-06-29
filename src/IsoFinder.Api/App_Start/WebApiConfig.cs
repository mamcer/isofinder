using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IsoFinder.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            GlobalConfiguration.Configuration.Filters.Add(new NotImplementedExceptionFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            
            json.SerializerSettings.Formatting = Formatting.Indented;
            
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            
            json.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
