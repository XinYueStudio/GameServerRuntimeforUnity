using System.Collections.Generic;
 
using MicroLightServerRuntime.Peer.Utils.Implementation;

namespace MicroLightServerRuntime.Peer.Server.Interfaces
{
    public interface IServerPeerList
    {
        Dictionary<ServerType, IServerPeer> PeerList { get; set; }
        bool AddServer(ServerType type, IServerPeer serverPeer);
        IServerPeer GetServerForType(ServerType type);
    }
}