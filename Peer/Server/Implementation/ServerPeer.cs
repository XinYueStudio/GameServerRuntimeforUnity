
using System;
using System.Net;
using System.Net.Sockets;
using MicroLightServerRuntime.Peer.Server.Interfaces;

namespace MicroLightServerRuntime.Peer.Server.Implementation
{
    public class ServerPeer : ServerPeerBase, IServerPeer
    {
      

        public ServerPeer(string Ip, int Port) : base(Ip, Port)
        {

        }
        public bool StartUpServer()
        {
            return base.StartServer;
        }
        public bool ShutdownServer()
        {
            base.Stop();
            return !base.StartServer;
        }

    
       
    }
}
