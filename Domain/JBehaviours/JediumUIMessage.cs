using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BehaviourMessages
{
    public class JediumUIMessage:JediumBehaviourMessage
    {
        public readonly string dllName;
        public readonly Guid bundleId;
        public readonly string xamlName;
        public readonly string archiveName;

        private readonly int _behType;

        public JediumUIMessage(string dllName, Guid bundleId, string xamlName, string archiveName)
        {
            this.dllName = dllName;
            this.bundleId = bundleId;
            this.xamlName = xamlName;
            this.archiveName = archiveName;
            _behType = TYPEBEHAVIOUR.GetTypeIndex("UI");
        }

        public int GetBehaviourType()
        {
            return _behType;
        }
    }

    public class JediumUISnapshot : JediumBehaviourSnapshot
    {

        public  string dllName;
        public  Guid bundleId;
        public string xamlName;
        public string archiveName;

        public JediumUISnapshot(Guid localId, string dllName,Guid bundleId, string xamlName, string archiveName) : base("UI", localId)
        {
            this.dllName = dllName;
            this.bundleId = bundleId;
            this.xamlName = xamlName;
            this.archiveName = archiveName;
        }

    }
}
