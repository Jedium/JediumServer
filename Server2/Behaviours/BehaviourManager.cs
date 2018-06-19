using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Akka.Serialization;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using NLog;

namespace Server2.Behaviours
{
    public static class BehaviourManager
    {
        private static readonly ILogger _log = LogManager.GetLogger("BehaviourManager");
        public static Dictionary<string, Type> RegisteredSnapshotTypes = new Dictionary<string, Type>();

        public static List<Type> RegisteredMessageTypes = new List<Type>();

        public static int LoadBehaviours(string path)
        {
            int ret = 0;
            _log.Info("-----------Start loading behaviour plugins---------------");
            _log.Info($"Path:{path}");

            //enumerate manifests
            List<BehaviourPluginManifest> manifests = new List<BehaviourPluginManifest>();
            foreach (string mnfst in Directory.GetFiles(path, "*.jpl"))
            {
                BehaviourPluginManifest man = BehaviourPluginManifest.LoadFromFile(mnfst);
                if (man != null)
                    manifests.Add(man);
            }
            //

            foreach (var mnf in manifests) LoadPluginFromManifest(mnf, path);
            
            _log.Info("-----------Finished loading behaviour plugins---------------");
            return ret;
        }

         static void LoadPluginFromManifest(BehaviourPluginManifest man, string path)
        {
            if (!File.Exists(Path.Combine(path, man.ServerDLL)) || !File.Exists(Path.Combine(path, man.SharedDLL)))
            {
                _log.Warn($"Can't find DLLs for plugin {man.Name}");
                return;
            }

            AssemblyName san = AssemblyName.GetAssemblyName(Path.Combine(path, man.ServerDLL));

            Assembly sas = Assembly.Load(san);

            Type behaviourType = typeof(JediumBehaviour);

            Type dbType = typeof(JediumBehaviourDBSnapshot);

            Type[] types = sas.GetTypes();

            foreach (Type t in types)
                if (t.IsAbstract || t.IsInterface)
                {
                }
                else
                {
                    if (t.BaseType == behaviourType)
                    {
                        JediumBehaviour jb = (JediumBehaviour) Activator.CreateInstance(t, new object[] {null});
                        string btype = jb.GetBehaviourType();
                        BehaviourTypeRegistry.BehaviourTypes.Add(btype, t);
                        //we also need to add type to TYPEBEHAVOUR
                        TYPEBEHAVIOUR.AddRegisteredType(btype);
                        _log.Info($"Added behaviour:{btype},{t}");
                    }

                    if (t.BaseType == dbType)
                    {
                        BehaviourTypeRegistry.DBTypes.Add(t);
                        _log.Info($"Registered DB snapshot type:{t}");
                    }
                }

            AssemblyName shan = AssemblyName.GetAssemblyName(Path.Combine(path, man.SharedDLL));

            Assembly shas = Assembly.Load(shan);

            Type snapshotType = typeof(JediumBehaviourSnapshot);

            types = shas.GetTypes();

            Type messageType = typeof(JediumBehaviourMessage);

            foreach (Type t in types)
                if (t.IsAbstract || t.IsInterface)
                {
                }
                else
                {
                    if (t.BaseType == snapshotType)
                    {
                        JediumBehaviourSnapshot jb = (JediumBehaviourSnapshot) Activator.CreateInstance(t);
                        string btype = jb.GetBehaviourType();
                        RegisteredSnapshotTypes.Add(btype, t);

                        _log.Info($"Added snapshot:{btype},{t}");
                    }

                    if (t.GetInterface(messageType.FullName) != null)
                    {
                        RegisteredMessageTypes.Add(t);


                        _log.Info($"Added message:{t}");
                    }
                }

            _log.Info("Finished loading behaviour plugin " + man.Name + " ,v. " + man.Version);
        }
    }

    public class LoadedTypeProvider : IKnownTypesProvider
    {
        public IEnumerable<Type> GetKnownTypes()
        {
            return BehaviourManager.RegisteredMessageTypes;
        }
    }
}