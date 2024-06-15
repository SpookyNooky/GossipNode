using System.Collections.Concurrent;

namespace GossipNode
{
    public class NodeRegistryService : INodeRegistryService
    {
        private readonly ConcurrentDictionary<string, NodeInfo> _nodes = new();

        public void AddNode(NodeInfo node)
        {
            _nodes[node.Address] = node;
        }

        public IEnumerable<NodeInfo> GetAllNodes()
        {
            return _nodes.Values;
        }

        public void ShareNodes(string address)
        {
            var httpClient = new HttpClient();
            var allNodes = GetAllNodes();

            httpClient.PostAsJsonAsync($"{address}/nodes", allNodes);
        }

        public void ReceiveNodes(IEnumerable<NodeInfo> nodes)
        {
            foreach (var node in nodes)
            {
                AddNode(node);
            }
        }
    }
}
