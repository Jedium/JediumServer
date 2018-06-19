using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace DomainInternal
{
    public interface ITerminalConnection : IInterfacedActor
    {
        Task<string> ExecuteCommand(string command);

        Task<List<DatabaseUser>> GetUsers();
        Task<Tuple<bool, string>> CreateUser(DatabaseUser user);
    }
}