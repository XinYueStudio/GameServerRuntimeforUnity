

 
using MicroLightServerRuntime.Peer.Server.Implementation;
using MicroLightServerRuntime.Peer.Utils.Interfaces;

namespace MicroLightServerRuntime.Peer.Server.Interfaces
{
    public interface IServerPeerFactory
    {
        T CreateServerPeer<T>(IServerData iserverdata) where T : ServerPeerBase;
    }
}