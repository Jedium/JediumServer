using System;
using System.Linq;
using ColoredConsole;
using ConsoleShell;
using DomainInternal;

namespace ServerConsole.Commands
{
    internal class HelpShellCommand : ShellCommandBase
    {
        public HelpShellCommand() : base("help", "Prints this help")
        {
        }

        public override void Invoke(Shell shell, string[] args)
        {
            var items = shell.GetCommandsDescriptions(string.Join(" ", args));

            var padSize = items.Max(x => x.Key.Length) + 4;

            ColorConsole.WriteLine("Commands:".Cyan());
            foreach (var item in items) ColorConsole.WriteLine("- ", item.Key.PadRight(padSize).White(), item.Value);
        }
    }


    internal class GetUsersCommand : ShellCommandBase
    {
        private readonly ITerminalConnection _term;

        public GetUsersCommand(ITerminalConnection term) : base("get-users", "Gets the list of all users")
        {
            _term = term;
        }

        public override void Invoke(Shell shell, string[] args)
        {
            var ret = _term.GetUsers().Result;
            foreach (var user in ret) ColorConsole.WriteLine($"{user.UserId}  {user.Username}  {user.Password}");
        }
    }

    internal class CreateUserCommand : ShellCommandBase
    {
        private readonly ITerminalConnection _term;

        public CreateUserCommand(ITerminalConnection term) : base("create-user",
            "Creates a new user. Params: login password")
        {
            _term = term;
        }

        public override void Invoke(Shell shell, string[] args)
        {
            if (args.Length < 2)
            {
                ColorConsole.WriteLine($"Missing required params".DarkCyan());
                return;
            }

            DatabaseUser user = new DatabaseUser
            {
                AvatarId = Guid.Parse("5ba75901-f2bb-4238-82cd-e917837ecf58"),
                AvatarProps = "",
                Password = args[1],
                UserId = Guid.NewGuid(),
                Username = args[0]
            };

            var result = _term.CreateUser(user).Result;

            if (result.Item1)
                ColorConsole.WriteLine($"User {args[0]} created successfully".Green());
            else
                ColorConsole.WriteLine($"Error creating user {result.Item2}".Red());
        }
    }
}