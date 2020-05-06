using System;
using System.Collections.Generic;
using MicroLightServerRuntime.Peer.Server.Implementation;
using MicroLightServerRuntime.Peer.Server.Interfaces;
using MicroLightServerRuntime.Peer.Utils.Implementation;

namespace MicroLightServerRuntime.Peer.Client.Interfaces
{
    public interface IClientPeerList
    {
        Dictionary<ServerType, IClientPeer> PeerList { get; set; }
        bool AddClientPeer(ServerType type, IClientPeer clientPeer);
        IClientPeer GetClientPeer(ServerType type);
    }
}