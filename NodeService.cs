using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace GossipNode
{
    public class NodeService : INodeService
    {
        private readonly INodeRegistryService _nodeRegistryService;
        private readonly IHostApplicationLifetime _lifetime;

        public NodeService(INodeRegistryService nodeRegistryService, IHostApplicationLifetime lifetime)
        {
            _nodeRegistryService = nodeRegistryService;
            _lifetime = lifetime;
        }

        public void Start(string bootstrapNode, string port)
        {
            var localAddress = $"http://localhost:{port}";

            _nodeRegistryService.AddNode(new NodeInfo { Address = localAddress });

            if (!string.IsNullOrEmpty(bootstrapNode))
            {
                _nodeRegistryService.AddNode(new NodeInfo { Address = bootstrapNode });
                _nodeRegistryService.ShareNodes(bootstrapNode);
            }

            // Start the HTTP server
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(localAddress);
                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPost("/nodes", async context =>
                            {
                                var nodes = await context.Request.ReadFromJsonAsync<IEnumerable<NodeInfo>>();
                                _nodeRegistryService.ReceiveNodes(nodes);
                            });

                            endpoints.MapGet("/nodes", async context =>
                            {
                                await context.Response.WriteAsJsonAsync(_nodeRegistryService.GetAllNodes());
                            });
                        });
                    });
                });

            var host = hostBuilder.Build();

            host.Start();

            // Register for application stopping event to gracefully shut down the server
            _lifetime.ApplicationStopping.Register(() => host.StopAsync().Wait());
        }
    }
}
