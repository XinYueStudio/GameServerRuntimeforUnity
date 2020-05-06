using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicroLightServerRuntime.Peer.Server.Implementation;

namespace MicroLightServerRuntime.Peer.Server.Interfaces
{
    public delegate void OnEventReceiveCompletedHanlder(string ip, byte[] msg);

    public delegate void OnEventCompletedHanlder(string ip);
    public interface IServerPeerBase
    {
         event OnEventReceiveCompletedHanlder OnReceiveCompletedEvent;
         event OnEventCompletedHanlder OnSendCompletedEvent;
         event OnEventCompletedHanlder OnConnectedEvent;
         event OnEventCompletedHanlder OnDisconnectEvent;
         event OnEventCompletedHanlder OnNotConnectEvent;

        bool Init();
        bool Start();
        bool Stop();
        bool Send(string ip, byte[] buffer);
        bool SendAll(byte[] buffer);
        bool SendOthers(string ip, byte[] buffer);
        bool StopOne(string ip);
    }


 
}
