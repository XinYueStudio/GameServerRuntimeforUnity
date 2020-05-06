using System;
 

namespace MicroLightServerRuntime.Peer.Utils.Implementation
{
     
    public enum ServerType : int
    {
        MasterServer,
        PlayerServer,
        LevelServer,
        ChatServer ,
        PropServer,
        FriendsServer,    
        ShoppingServer
    }
}