 
using System.Collections.Generic;
 
using MicroLightServerRuntime.Peer.Client.Interfaces;
 
using MicroLightServerRuntime.Peer.Utils.Implementation;

namespace MicroLightServerRuntime.Peer.Client.Implementation
{
    public class ClientPeerList : IClientPeerList
    {
        public Dictionary<ServerType, IClientPeer> PeerList { get; set; }

        public bool AddClientPeer(ServerType type, IClientPeer clientPeer)
        {
            if(PeerList.ContainsKey(type))
            {
                return false;
            }
            else
            {
                PeerList.Add(type, clientPeer);

                return true; ;
            }

          
        }

        public IClientPeer GetClientPeer(ServerType type)
        {
            if (PeerList.ContainsKey(type))
            {
                IClientPeer cientpeer;
                PeerList.TryGetValue(type, out cientpeer);
                return cientpeer;
            }
            return null;
        }
    }
}
