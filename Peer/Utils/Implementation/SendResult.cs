 

namespace MicroLightServerRuntime.Peer.Utils.Implementation
{
    /// <summary>
    ///   Return value  
    /// </summary>
    public enum SendResult
    {
        /// <summary>
        ///   Successfully enqueued for sending.
        /// </summary>
        Ok,
        /// <summary>
        ///   Peer is disconnected; data sending was refused.
        /// </summary>
        Disconnected = 2,
        /// <summary>
        ///   The peer's send buffer is full; data sending was refused.
        /// </summary>
        SendBufferFull = 1,
        /// <summary>
        /// Sending failed because the message size exceeded the MaxMessageSize that was configured for the receiver. 
        /// </summary>
        MessageToBig = 3,
        /// <summary>
        /// Send failed because the specified channel is not supported by the peer.
        /// </summary>
        InvalidChannel = 5,
        /// <summary>
        /// Send Failed due an unexpected error.
        /// </summary>
        Failed = 4,
        /// <summary>
        /// Send failed because the specified content type is not supported by the peer.
        /// </summary>
        InvalidContentType = 6,
        /// <summary>
        ///   Encrypted sending failed; peer does not support encryption.
        /// </summary>
        EncryptionNotSupported = -1
    }
}
