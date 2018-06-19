using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Interfaced;
using DomainInternal;

namespace Server2.Connection
{
    class TerminalConnection : InterfacedActor, ITerminalConnection
    {
        private readonly IDatabaseAgent _database;

        public TerminalConnection(IDatabaseAgent db)
        {
            _database = db;
        }

        async Task<Tuple<bool, string>> ITerminalConnection.CreateUser(DatabaseUser user)
        {
            var tus = _database.GetUserByName(user.Username).Result;

            if (tus != null) return Tuple.Create(false, $"User {user.Username} already exists");

            _database.CreateUser(user).Wait();

            return new Tuple<bool, string>(true, $"User {user.Username} created");
        }

        async Task<List<DatabaseUser>> ITerminalConnection.GetUsers()
        {
            return _database.GetUsers().Result;
        }

        async Task<string> ITerminalConnection.ExecuteCommand(string command)
        {
            return $"Command {command} executed";
        }
    }
}