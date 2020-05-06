 
using System.Net;
using System.Net.Sockets;
 
using MicroLightServerRuntime.Peer.Client.Interfaces;

namespace MicroLightServerRuntime.Peer.Client.Implementation
{
    public class ClientPeer : ClientPeerBase, IClientPeer
    {
 
        public ClientPeer(string Ip, int Port) : base(Ip, Port)
        {

        }
        public bool Connect()
        {
            return base.Connected;
        }

      
        public bool DisConnect()
        {
            base.Dispose();
            return !base.Connected;
        }
    }
}
