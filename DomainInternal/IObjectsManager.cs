using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace DomainInternal
{
    public interface IObjectsManager : IInterfacedActor
    {
        Task LoadObjects();
        Task LoadAllScenes();

        Task<List<Tuple<Guid, string,string>>> GetLoadedScenesList();

        Task<List<Tuple<Guid, string>>> GetSceneObjects(Guid scene);

        Task SaveObjectsToDB();
    }
}