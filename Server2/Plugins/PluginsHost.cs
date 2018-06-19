using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Interfaced;
using Common.Logging;
using DomainInternal;

namespace Server2.Plugins
{
    public class PluginsHost : InterfacedActor, IPluginsHost
    {
        private readonly ILog _logger;

        private readonly IDatabaseAgent _database;

        private Dictionary<PluginInfo, IBasicServerPlugin> _loadedPlugins;
        private readonly IObjectsManager _manager;

        public PluginsHost(IDatabaseAgent database, IObjectsManager manager)
        {
            _logger = LogManager.GetLogger("[Plugins Host]");
            _database = database;
            _manager = manager;
        }

        async Task IPluginsHost.LoadPlugins()
        {
            _loadedPlugins = new Dictionary<PluginInfo, IBasicServerPlugin>();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "plugins");

            _logger.Info("Loading plugins from: " + path);

            if (Directory.Exists(path))
            {
                string[] dllFileNames = null;
                dllFileNames = Directory.GetFiles(path, "*.dll");
                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);

                foreach (string dllFile in dllFileNames)
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                    _logger.Info("Checking assembly:" + an);
                    Assembly assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }

                Type pluginType = typeof(IBasicServerPlugin);

                ICollection<Type> pluginTypes = new List<Type>();

                foreach (Assembly assembly in assemblies)
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types)
                            if (type.IsInterface || type.IsAbstract)
                            {
                            }
                            else
                            {
                                if (type.GetInterface(pluginType.FullName) != null)
                                {
                                    _logger.Info("Found plugin:" + type);
                                    pluginTypes.Add(type);
                                }
                            }
                    }

                
                foreach (Type type in pluginTypes)
                {
                    _logger.Info("Creating actor for plugin: " + type);
                    IBasicServerPlugin pluginAgent = Context.ActorOf(Props.Create(() =>
                            (BaseServerPlugin) Activator.CreateInstance(type, _database, _manager)))
                        .Cast<BasicServerPluginRef>();

                    // pluginAgent.PluginMessage("TEST PLUGIN").Wait();

                    PluginInfo info = pluginAgent.GetInfo().Result;
                    _logger.Info($"Successfully loaded plugin: {info.Name}, version: {info.Version}");
                    _loadedPlugins.Add(info, pluginAgent);
                }
            }
            else
            {
                _logger.Warn($"Path {path} not found, skipping plugins loading");
            }
        }
    }
}