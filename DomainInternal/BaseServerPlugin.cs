using Akka.Interfaced;

namespace DomainInternal
{
    public abstract class BaseServerPlugin : InterfacedActor
    {
        protected IDatabaseAgent _database;
        protected IObjectsManager _manager;

        public BaseServerPlugin(IDatabaseAgent database, IObjectsManager manager)
        {
            _database = database;
            _manager = manager;
        }
    }
}