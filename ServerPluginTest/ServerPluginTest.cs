using System;
using System.Threading.Tasks;
using DomainInternal;

namespace ServerPluginTest
{
    public class ServerPluginTest : BaseServerPlugin, IBasicServerPlugin
    {
        public ServerPluginTest(IDatabaseAgent database, IObjectsManager manager) : base(database, manager)
        {
        }

        async Task IBasicServerPlugin.PluginMessage(string msg)
        {
            //message to plugin sample
            Console.WriteLine("___MESSAGE:" + msg);
        }

        async Task<PluginInfo> IBasicServerPlugin.GetInfo()
        {
            return new PluginInfo
            {
                Name = "Test plugin",
                Version = 1
            };
        }

        protected override void PreStart()
        {
            Console.WriteLine("--------------------------PLUGIN STARTED---------------");

            //DB access sample
            var bundles = _database.GetAllBundles().Result;

            Console.WriteLine("__COUNT: " + bundles.Count);
            foreach (var bundle in bundles) Console.WriteLine("___BUNDLE:" + bundle.BundleId + ";" + bundle.BundleFile);
        }
    }
}