
using MicroLightServerRuntime.Peer.Client.Implementation;
using MicroLightServerRuntime.Peer.Server.Implementation;
using MicroLightServerRuntime.Peer.Server.Interfaces;
using MicroLightServerRuntime.Peer.Utils.Interfaces;

namespace MicroLightServerRuntime.Peer.Client.Interfaces
{
    public interface IClientPeerFactory
    {
        T CreateClientPeer<T>(IServerData iserverdata) where T : ClientPeerBase;
    }
}