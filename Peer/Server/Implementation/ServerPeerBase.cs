using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MicroLightServerRuntime.Logging.Implementation;
using MicroLightServerRuntime.Logging.Interfaces;
using MicroLightServerRuntime.Peer.Server.Interfaces;
using MicroLightServerRuntime.Peer.Utils.Implementation;
 

namespace MicroLightServerRuntime.Peer.Server.Implementation
{
    public abstract class ServerPeerBase : IDisposable
    {
        /// <summary>
        ///   The loger.
        /// </summary>
        public static readonly ILogger log = new LogManager();

        public static readonly ILogger operationDataLogger = new LogManager();
        
        private const int MessageHeaderSize = 4;
        private const int maxConnectionCount = 1000;
        private int _receivedMessageCount = 0;  //for testing
        private Stopwatch _watch;  //for testing

        private BlockingCollection<MessageData> sendingQueue;
        private Thread sendMessageWorker;

        private static Mutex _mutex = new Mutex();
        private Socket _listenSocket;
        private int _bufferSize=4096;
        private int _connectedSocketCount;
        private int _maxConnectionCount;
        private SocketAsyncEventArgsPool _socketAsyncReceiveEventArgsPool;
        private SocketAsyncEventArgsPool _socketAsyncSendEventArgsPool;
        private Semaphore _acceptedClientsSemaphore;
        private AutoResetEvent waitSendEvent;

        private string Ip;
        private int Port;

        public bool StartServer = false;

        public Dictionary<string, AsyncUserToken> ClientDictionary = new Dictionary<string, AsyncUserToken>();

        public delegate void OnEventCompletedHanlder(AsyncUserToken token);
        public delegate void OnEventReceiveCompletedHanlder(AsyncUserToken token ,byte[] msg);
        public event OnEventReceiveCompletedHanlder OnReceiveCompletedEvent;
        public event OnEventCompletedHanlder OnSendCompletedEvent;
        public event OnEventCompletedHanlder OnConnectedEvent;
        public event OnEventCompletedHanlder OnDisconnectEvent;
   


        public ServerPeerBase(string ip, int port)
        {
            Ip = ip;
            Port = port;
               _socketAsyncReceiveEventArgsPool = new SocketAsyncEventArgsPool(maxConnectionCount);
            _socketAsyncSendEventArgsPool = new SocketAsyncEventArgsPool(maxConnectionCount);
            _acceptedClientsSemaphore = new Semaphore(maxConnectionCount, maxConnectionCount);

            sendingQueue = new BlockingCollection<MessageData>();
            sendMessageWorker = new Thread(new ThreadStart(SendQueueMessage));

            for (int i = 0; i < maxConnectionCount; i++)
            {
                SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
                socketAsyncEventArgs.Completed += OnIOCompleted;
                socketAsyncEventArgs.SetBuffer(new Byte[_bufferSize], 0, _bufferSize);
                _socketAsyncReceiveEventArgsPool.Push(socketAsyncEventArgs);
            }

            for (int i = 0; i < maxConnectionCount; i++)
            {
                SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
                socketAsyncEventArgs.Completed += OnIOCompleted;
                socketAsyncEventArgs.SetBuffer(new Byte[_bufferSize], 0, _bufferSize);
                _socketAsyncSendEventArgsPool.Push(socketAsyncEventArgs);
            }

            waitSendEvent = new AutoResetEvent(false);



            Start();
        }

        public bool  Start( )
        {
            ClientDictionary = new Dictionary<string, AsyncUserToken>();
            if (StartServer) return StartServer;
                IPEndPoint LoadEndpoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
               _listenSocket = new Socket(LoadEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.ReceiveBufferSize = _bufferSize;
            _listenSocket.SendBufferSize = _bufferSize;
            _listenSocket.Bind(LoadEndpoint);
            _listenSocket.Listen(_maxConnectionCount);
            sendMessageWorker.Start();
            StartAccept(null);
            _mutex.WaitOne();
            StartServer = true;
            return StartServer;
        }
        public bool Stop()
        {
            if (OnDisconnectEvent != null)
                OnDisconnectEvent(null);
            foreach (KeyValuePair<string, AsyncUserToken> Key in ClientDictionary)
            {
                Key.Value.Dispose();
                _acceptedClientsSemaphore.Release();
                Interlocked.Decrement(ref _connectedSocketCount);
                Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", _connectedSocketCount);
                 
            }
            ClientDictionary.Clear();
            try
            {
                _listenSocket.Close();
            }
            catch { }
            _mutex.ReleaseMutex();
            _acceptedClientsSemaphore.Release();

            StartServer = false;
            return StartServer;
        }
        public bool Send(AsyncUserToken token, byte[] msg)
        {
            sendingQueue.Add(new MessageData { Message = msg, Token = token });
            return true;
        }
        public bool SendAll(byte[] msg)
        {
            lock (ClientDictionary)
            {
                foreach (KeyValuePair<string, AsyncUserToken> Key in ClientDictionary)
                {
                    sendingQueue.Add(new MessageData { Message = msg, Token = Key.Value });
                }
            }
            return true;
        }
        public bool SendOthers(AsyncUserToken token, byte[] msg)
        {
            lock (ClientDictionary)
            {
                foreach (KeyValuePair<string, AsyncUserToken> Key in ClientDictionary)
                {
                    if (token.Socket.RemoteEndPoint.ToString() != Key.Key)
                    {
                        sendingQueue.Add(new MessageData { Message = msg, Token = Key.Value });
                    }
                }
            }
            return true;
        }
        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += (sender, e) => ProcessAccept(e);
            }
            else
            {
                acceptEventArg.AcceptSocket = null;
            }

            _acceptedClientsSemaphore.WaitOne();
            if (!_listenSocket.AcceptAsync(acceptEventArg))
            {
                ProcessAccept(acceptEventArg);
            }
        }
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                SocketAsyncEventArgs readEventArgs = _socketAsyncReceiveEventArgsPool.Pop();
                if (readEventArgs != null)
                {
                    var token = new  AsyncUserToken(e.AcceptSocket);
                    readEventArgs.UserToken = token;
                    e.UserToken = token;
                    Interlocked.Increment(ref _connectedSocketCount);
                    ClientDictionary.Add(e.AcceptSocket.RemoteEndPoint.ToString(), token);
                    if (OnConnectedEvent != null)
                        OnConnectedEvent(token);

                    Console.WriteLine("Client connection accepted. There are {0} clients connected to the server", _connectedSocketCount);
                    if (!e.AcceptSocket.ReceiveAsync(readEventArgs))
                    {
                        ProcessReceive(readEventArgs);
                    }
                }
                else
                {
                    Console.WriteLine("There are no more available sockets to allocate.");
                }
            }
            catch (SocketException ex)
            {
                AsyncUserToken token = e.UserToken as AsyncUserToken;
                Console.WriteLine("Error when processing data received from {0}:\r\n{1}", token.Socket.RemoteEndPoint, ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // Accept the next connection request.
            StartAccept(e);
        }
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = e.UserToken as AsyncUserToken;
                int lengthBuffer = e.BytesTransferred;      //获取接收的字节长度
                //处理接收到的数据
                byte[] receiveBuffer = e.Buffer;
                byte[] buffer = new byte[lengthBuffer];
                Buffer.BlockCopy(receiveBuffer, 0, buffer, 0, lengthBuffer);
                if (OnReceiveCompletedEvent != null)
                    OnReceiveCompletedEvent(token, buffer);        
                //接收后续的数据
                if (!token.Socket.ReceiveAsync(e))
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }
       
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            _socketAsyncSendEventArgsPool.Push(e);
            waitSendEvent.Set();
            if (OnSendCompletedEvent != null)
                OnSendCompletedEvent((AsyncUserToken)e.UserToken);
            
        }
        private void SendQueueMessage()
        {
            while (true)
            {
                var messageData = sendingQueue.Take();
                if (messageData != null)
                {
                    SendMessage(messageData, messageData.Message);
                }
            }
        }
        private void SendMessage(MessageData messageData, byte[] message)
        {
            var sendEventArgs = _socketAsyncSendEventArgsPool.Pop();
            if (sendEventArgs != null)
            {
                if (messageData.Token != null)
                {
                    if (messageData.Token.Socket != null)
                    {
                        if (messageData.Token.Socket.Connected )
                        {
                            
                                try
                                {
                                    sendEventArgs.SetBuffer(message, 0, message.Length);
                                    sendEventArgs.UserToken = messageData.Token;
                                    messageData.Token.Socket.SendAsync(sendEventArgs);
                                }
                                catch
                                { }
                            }
                    }
                }
            }
            else
            {
                waitSendEvent.WaitOne();
                SendMessage(messageData, message);
            }
        }
       
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            var token = e.UserToken as AsyncUserToken;
            if(ClientDictionary.ContainsKey(token.Socket.RemoteEndPoint.ToString()))
            ClientDictionary.Remove(token.Socket.RemoteEndPoint.ToString());
           
            
            token.Dispose();
          
            Interlocked.Decrement(ref _connectedSocketCount);
            Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", _connectedSocketCount);
            _socketAsyncReceiveEventArgsPool.Push(e);
        }

        #region IDisposable Members  

        // Disposes the instance of SocketClient.  
        public void Dispose()
        {
            Stop();
        }

        #endregion
    }




}
