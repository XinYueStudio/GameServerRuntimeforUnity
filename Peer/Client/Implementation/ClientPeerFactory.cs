 
using MicroLightServerRuntime.Peer.Client.Interfaces;
using MicroLightServerRuntime.Peer.Utils.Interfaces;

namespace MicroLightServerRuntime.Peer.Client.Implementation
{
    public class ClientPeerFactory : IClientPeerFactory
    {
        public ClientPeerFactory()
        {

        }

        public T CreateClientPeer<T>(IServerData iserverdata) where T : ClientPeerBase
        {
            return new ClientPeer(iserverdata.Ip, iserverdata.Port) as T;
        }
    }
}
