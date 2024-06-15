namespace GossipNode
{
    public interface INodeRegistryService
    {
        void AddNode(NodeInfo node);
        IEnumerable<NodeInfo> GetAllNodes();
        void ShareNodes(string address);
        void ReceiveNodes(IEnumerable<NodeInfo> nodes);
    }
}
