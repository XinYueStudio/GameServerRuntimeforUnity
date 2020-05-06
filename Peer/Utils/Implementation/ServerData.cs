 
using MicroLightServerRuntime.Peer.Utils.Interfaces;

namespace MicroLightServerRuntime.Peer.Utils.Implementation
{
    public class ServerData : IServerData
    {
        public ServerType Type { get ; set ; }
        public string Ip { get ; set ; }
        public int Port { get; set ; }

   
    }
}
