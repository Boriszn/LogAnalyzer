using System.Web.Http;

namespace LogAnalyzer.Web.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "GetErrorsCount",
                routeTemplate: "api/getErrorsCount/{collectionName}",
                defaults: new
                {
                    controller = "LogApi",
                    action = "GetErrorsCount",
                }
            );

            config.Routes.MapHttpRoute(
                name: "GetEnvironmentNameApi",
                routeTemplate: "api/environmentInfo",
                defaults: new
                {
                    controller = "LogApi",
                    action = "GetEnvironmentName",
                }
            );

            config.Routes.MapHttpRoute(
                name: "GetCollectionsApi",
                routeTemplate: "api/collections",
                defaults: new
                {
                    controller = "LogApi",
                    action = "GetAllCollections"
                }
            );


            config.Routes.MapHttpRoute(
               name: "GetNumberOfNewItemsApi",
               routeTemplate: "api/{collectionName}/new",
               defaults: new
               {
                   controller = "LogApi",
                   action = "GetNumberOfNewItems",
               }
           );

            config.Routes.MapHttpRoute(
               name: "GetNewItemsApi",
               routeTemplate: "api/getNewItems/{collectionName}",
               defaults: new
               {
                   controller = "LogApi",
                   action = "GetNewItems",
               }
           );

            config.Routes.MapHttpRoute(
                name: "GetLogById",
                routeTemplate: "api/{collectionName}/{logid}",
                defaults: new
                {
                    controller = "LogApi",
                    action = "GetLogById",
                }
            );

            config.Routes.MapHttpRoute(
                name: "GetLogsByQueryApi",
                routeTemplate: "api/{collectionName}",
                defaults: new
                {
                    controller = "LogApi",
                    action = "GetLogsByQuery",
                }
            );
            
        }
    }
}
