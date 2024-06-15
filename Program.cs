using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GossipNode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var nodeService = host.Services.GetRequiredService<INodeService>();

            string bootstrapNode = args.Length > 0 ? args[0] : null;
            string port = args.Length > 1 ? args[1] : "5001"; // Default to port 5001 if not specified
            nodeService.Start(bootstrapNode, port);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<INodeRegistryService, NodeRegistryService>();
                    services.AddSingleton<INodeService, NodeService>();
                });
    }
}
