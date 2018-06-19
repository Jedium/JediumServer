using System;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    /// <summary>
    /// Client-to-server connection client-side.
    /// </summary>
    public interface IClientConnection : IInterfacedActor
    {
        /// <summary>
        /// Register new client on server
        /// </summary>
        /// <param name="clientID">Client GUID.</param>
        /// <param name="scene">The name of the scene client wants to log in.</param>
        /// <returns></returns>
        Task RegisterConnection(Guid clientID, string scene);

        /// <summary>
        /// Internal. Disconnects the client.
        /// </summary>
        /// <returns></returns>
        Task KillConnection();
    }
}