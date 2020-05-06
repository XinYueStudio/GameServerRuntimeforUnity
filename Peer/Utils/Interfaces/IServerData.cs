 
using MicroLightServerRuntime.Peer.Utils.Implementation;

namespace MicroLightServerRuntime.Peer.Utils.Interfaces
{
    public interface IServerData
    {
        ServerType Type { get; set; }
        string Ip { get; set; }
        int Port { get; set; }
    }
}
