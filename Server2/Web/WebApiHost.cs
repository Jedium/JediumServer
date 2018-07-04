using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Akka.Interfaced;
using Common.Logging;
using DomainInternal;
using Unity;

namespace Server2.Web
{
    public class WebApiHost : InterfacedActor, IWebApiHost
    {
        private readonly ILog _logger;

        private HttpSelfHostServer _assetsServer;

        private readonly IDatabaseAgent _database;

        private readonly string _mainUrl;

        private readonly IObjectsManager _manager;

        public WebApiHost(string url, IDatabaseAgent database, IObjectsManager manager)
        {
            _logger = LogManager.GetLogger("[Assets Host]");

            _mainUrl = url;
            _database = database;
            _manager = manager;
        }

        protected override void PreStart()
        {
            var config = new HttpSelfHostConfiguration(_mainUrl);
            config.MaxReceivedMessageSize = 2147483647; // use config for this value
            config.Routes.MapHttpRoute("Assets", "api/{controller}/{action}/{id}",
                new {id = RouteParameter.Optional});
            //json by default
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            var container = new UnityContainer();
            container.RegisterInstance(_database);
            container.RegisterInstance(_manager);
            config.DependencyResolver = new UnityResolver(container);


            _assetsServer = new HttpSelfHostServer(config);
            _assetsServer.OpenAsync().Wait();

            _logger.Info($"Started at {_mainUrl}");
        }

        protected override void PostStop()
        {
            _assetsServer.CloseAsync().Wait();
            _logger.Info("Stopped");
        }
    }
}