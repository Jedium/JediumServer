using System;
using System.Configuration;
using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using Akka.Interfaced;
using ColoredConsole;
using ConsoleShell;
using DomainInternal;
using ServerConsole.Commands;

namespace ServerConsole
{
    class Program
    {
        private static ActorSystem _system;

        private static ITerminalConnection _terminal;

        private static readonly string _localUrl = "akka.tcp://VirtualFramework@localhost:18095/user/Terminal";

        static void Main(string[] args)
        {
            AkkaConfigurationSection section = (AkkaConfigurationSection) ConfigurationManager.GetSection("akka");
            Config aconfig = section.AkkaConfig;


            try
            {
                _system = ActorSystem.Create("VirtualFrameworkConsole", aconfig);
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine($"Error initializig Akka:{ex.Message}".Red());
                //  throw;
            }

            try
            {
                IActorRef asel =
                    _system.ActorSelection(_localUrl)
                        .ResolveOne(TimeSpan.Zero).Result;

                _terminal = asel.Cast<TerminalConnectionRef>();
            }
            catch (Exception e)
            {
                ColorConsole.WriteLine($"Error connecting to server:{e.Message}".Red());
            }


            if (_terminal == null) ColorConsole.WriteLine("Error connecting to server".Red());


            var shell = new Shell();
            RegisterCommands(shell, true);

            shell.WritePrompt += ShellOnWritePrompt;
            shell.ShellCommandNotFound += ShellOnShellCommandNotFound;
            shell.PrintAlternatives += ShellOnPrintAlternatives;

            try
            {
                shell.RunShell();
            }
            catch (ApplicationExitException)
            {
                _system.Terminate().Wait();
            }
        }


        private static void RegisterCommands(Shell shell, bool interactive)
        {
            if (interactive)
            {
                shell.AddCommand("exit", "Exit from program", Exit);
                shell.AddCommand("quit", "Exit from program", Exit);
            }

            shell.AddCommand(new HelpShellCommand());
            shell.AddCommand(new GetUsersCommand(_terminal));
            shell.AddCommand(new CreateUserCommand(_terminal));
        }

        private static void Exit(Shell shell, IShellCommand shellCommand, string[] strings)
        {
            throw new ApplicationExitException();
        }


        #region ShellEventHandlers

        private static void ShellOnPrintAlternatives(object sender, PrintAlternativesEventArgs e)
        {
            ColorConsole.WriteLine("Possible commands: ".Cyan());
            foreach (var alternative in e.Alternatives) ColorConsole.WriteLine("- ", alternative.White());
        }

        private static void ShellOnShellCommandNotFound(object sender, CommandNotFoundEventArgs e)
        {
            ColorConsole.WriteLine($"Command not found: ".Red(), e.Input.White());
        }

        private static void ShellOnWritePrompt(object sender, EventArgs eventArgs)
        {
            ColorConsole.Write("[ ".Green(), DateTime.Now.ToLongTimeString(), " ]-> ".Green());
        }

        #endregion
    }
}