using System.Web.Http;
using LogAnalyzer.Web.App_Start;
using WebActivatorEx;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace LogAnalyzer.Web.App_Start
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configuration
                .EnableSwagger(c => c.SingleApiVersion("v1", "LogAnalizer.Web"))
                .EnableSwaggerUi(c => { });
        }
    }
}