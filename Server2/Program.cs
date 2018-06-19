using Topshelf;

namespace Server2
{
    class Program
    {
        private static int Main(string[] args)
        {

            //boilerplate Topshelfа для запуска сервиса в консольном приложении
            return (int) HostFactory.Run(x =>
            {
                x.SetServiceName("Jedium");
                x.SetDisplayName("Jedium main server");
                x.SetDescription("Main server for Jedium project");

                x.UseAssemblyInfoForServiceInfo();
                x.RunAsLocalSystem();
                x.StartAutomatically();

                x.Service<HostService>(s =>
                {
                    HostService mainSrv = null;
                    s.ConstructUsing(() =>
                    {
                        mainSrv = new HostService();
                        return mainSrv;
                    });
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}