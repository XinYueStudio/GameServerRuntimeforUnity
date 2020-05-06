using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
 
using MicroLightServerRuntime.Logging.Implementation;
using MicroLightServerRuntime.Logging.Interfaces;
using MicroLightServerRuntime.Peer.Server.Implementation;
using MicroLightServerRuntime.Peer.Utils.Implementation;

namespace MicroLightServerRuntime.Peer.Client.Implementation
{

    /// <summary>
    ///   Base class for Tcp client implementations.
    /// </summary>
    public abstract class ClientPeerBase : IDisposable
        {
        /// <summary>
        ///   The loger.
        /// </summary>
        public static readonly ILogger log=new LogManager();

        public static readonly ILogger operationDataLogger = new LogManager();
 
      
        /// <summary>
        ///   Gets a value indicating whether this instance is connected to a remote host.
        /// </summary>
        public bool Connected;



        public delegate void OnEventCompletedHanlder(string ip);
        public delegate void OnEventReceiveCompletedHanlder(string ip, byte[] msg);
        public event OnEventCompletedHanlder OnConnectedEvent;
        public event OnEventCompletedHanlder OnDisConnectedEvent;
        public event OnEventReceiveCompletedHanlder OnReceiveCompletedEvent;       
        public event OnEventCompletedHanlder OnSendCompletedEvent;        

        public ClientPeerBase(string  ip,int port)
        {
            log.IsDebugEnabled = true;
            operationDataLogger.IsDebugEnabled = true;

            this.hostEndPoint = new IPEndPoint(IPAddress.Parse(ip),port);
            this.autoConnectEvent = new AutoResetEvent(false);
            this.autoSendEvent = new AutoResetEvent(false);
            this.sendingQueue = new BlockingCollection<byte[]>();
         
            this.clientSocket = new Socket(this.hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.sendMessageWorker = new Thread(new ThreadStart(SendQueueMessage));
       
            this.sendEventArgs = new SocketAsyncEventArgs();
            this.sendEventArgs.UserToken = this.clientSocket;
            this.sendEventArgs.RemoteEndPoint = this.hostEndPoint;
            this.sendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSend);

            this.receiveEventArgs = new SocketAsyncEventArgs();
            this.receiveEventArgs.UserToken = new AsyncUserToken(clientSocket);
            this.receiveEventArgs.RemoteEndPoint = this.hostEndPoint;
            this.receiveEventArgs.SetBuffer(new Byte[bufferSize], 0, bufferSize);
            this.receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceive);
            this.Connect(ip,port);
        }


 
        /// <summary>
        ///   Closes the connection to the remote host.
        /// </summary>
        public virtual void Disconnect()
        {
            if (clientSocket != null)
            {
                if (OnDisConnectedEvent != null)
                    OnDisConnectedEvent(clientSocket.RemoteEndPoint.ToString());
                if (this.sendMessageWorker != null)
                {
                    this.sendMessageWorker.Abort();
                    this.sendMessageWorker = null;
                }

                try
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }
                catch 
                {
                }
                finally
                {
                    clientSocket.Close();
                   
                }
            }
            this.Connected = false;
        }



        private int bufferSize = 60000;
        private const int MessageHeaderSize = 4;

        private Socket clientSocket;
        private bool connected = false;
        private IPEndPoint hostEndPoint;
        private AutoResetEvent autoConnectEvent;
        private AutoResetEvent autoSendEvent;
        private SocketAsyncEventArgs sendEventArgs;
        private SocketAsyncEventArgs receiveEventArgs;
        private BlockingCollection<byte[]> sendingQueue;
 
        private Thread sendMessageWorker;


 

        public virtual void Connect(string ip, int port)
        {
            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
            connectArgs.UserToken = this.clientSocket;
            connectArgs.RemoteEndPoint = this.hostEndPoint;
            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);

            clientSocket.ConnectAsync(connectArgs);
            autoConnectEvent.WaitOne();

            SocketError errorCode = connectArgs.SocketError;
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((Int32)errorCode);
            }
            sendMessageWorker.Start();
      

            if (!clientSocket.ReceiveAsync(receiveEventArgs))
            {
                ProcessReceive(receiveEventArgs);
            }
        }
 
        public void Send(byte[] message)
        {
            sendingQueue.Add(message);
        }

        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            autoConnectEvent.Set();
            connected = (e.SocketError == SocketError.Success);
            if (OnConnectedEvent != null)
                OnConnectedEvent(e.RemoteEndPoint.ToString());
            Connected = true;

        }
        private void OnSend(object sender, SocketAsyncEventArgs e)
        {
            autoSendEvent.Set();
            if (OnSendCompletedEvent != null)
                OnSendCompletedEvent(e.RemoteEndPoint.ToString());
        }
        private void SendQueueMessage()
        {
            while (true)
            {
                var message = sendingQueue.Take();
                if (message != null)
                {
                    sendEventArgs.SetBuffer(message, 0, message.Length);
                    clientSocket.SendAsync(sendEventArgs);
                    autoSendEvent.WaitOne();
                }
            }
        }

        private void OnReceive(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = e.UserToken as AsyncUserToken;

                //处理接收到的数据
                int lengthBuffer = e.BytesTransferred;
                byte[] receiveBuffer = e.Buffer;
                byte[] buffer = new byte[lengthBuffer];
                Buffer.BlockCopy(receiveBuffer, 0, buffer, 0, lengthBuffer);

                if (OnReceiveCompletedEvent != null)
                    OnReceiveCompletedEvent(token.Socket.RemoteEndPoint.ToString(), buffer);
                //接收后续的数据
                if (!clientSocket.ReceiveAsync(e))
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                ProcessError(e);
            }
        }

      
   
        
        private void ProcessError(SocketAsyncEventArgs e)
        {
          

            // Throw the SocketException
            throw new SocketException((Int32)e.SocketError);
        }

        #region IDisposable Members

 

        #endregion
        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; 
        ///   <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Disconnect();

                }
            }

            /// <summary>
            ///   Releases all resources used this instance.
            /// </summary>
        public void Dispose()
        {
            autoConnectEvent.Close();
            if (this.clientSocket.Connected)
            {
                this.clientSocket.Close();
            }
            this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

    }
