using System;
using System.Collections.Generic;
using MicroLightServerRuntime.Peer.Server.Interfaces;
using MicroLightServerRuntime.Peer.Utils.Implementation;

namespace MicroLightServerRuntime.Peer.Server.Implementation
{
    public class ServerPeerList : IServerPeerList
    {
        public Dictionary<ServerType, IServerPeer> PeerList { get ; set ; }

        public bool AddServer(ServerType type, IServerPeer serverPeer)
        {
            if (PeerList.ContainsKey(type))
            {
                return false;
            }
            else
            {
                PeerList.Add(type, serverPeer);

                return true; ;
            }
        }

        public IServerPeer GetServerForType(ServerType type)
        {
            throw new NotImplementedException();
        }
    }
}
