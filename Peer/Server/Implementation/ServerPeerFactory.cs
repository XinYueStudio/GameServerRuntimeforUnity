

using System;
using MicroLightServerRuntime.Peer.Server.Interfaces;
using MicroLightServerRuntime.Peer.Utils.Interfaces;

namespace MicroLightServerRuntime.Peer.Server.Implementation
{
    public class ServerPeerFactory : IServerPeerFactory
    {
        public T CreateServerPeer<T>(IServerData iserverdata) where T : ServerPeerBase
        {
            throw new NotImplementedException();
        }
    }
}
