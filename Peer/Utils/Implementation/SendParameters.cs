 

namespace MicroLightServerRuntime.Peer.Utils.Implementation
{

    /// <summary>
    ///   The struct contains the parameters for 
    ///   and contains the info about incoming data at  
    /// </summary>
    public struct SendParameters
    {

        /// <summary>
        ///   Gets or sets the channel id for the  protocol.
        /// </summary>
        public int ChannelId
        {
            get;
            set;
        }
        /// <summary>
        ///   Gets or sets a value indicating whether the data is sent EncryptedCode.
        /// </summary>
        public byte EncryptedType
        {
            get;
            set;
        }
       
    }
}
