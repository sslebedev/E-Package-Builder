using System.Net;
using System.Net.Sockets;

namespace EPBMessanger
{
    public class Server
    {
        private readonly Socket _serverSocket;

        public Server(int port = 11000, int backlog = 100)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _serverSocket.Bind(ipEndPoint);
            _serverSocket.Listen(backlog);
        }

        #region Remote Client

        private class RemoteClient : ISender
        {
            private readonly Socket _socket;
            private bool _valid;

            public bool IsValid
            {
                private set { _valid = value; }
                get { return _valid; }
            }

            public WritableMessage NewMessage()
            {
                if (!_valid) {
                    return null;
                }
                return new WritableMessage(_socket);
            }

            public void Disconnect()
            {
                _valid = false;

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }

            internal RemoteClient(Socket s)
            {
                _socket = s;
                _valid = true;
            }

            internal ReceivedMessage ReceiveMessage()
            {
                ReceivedMessage msg = ReceivedMessage.New(_socket);
                if (msg == null) {
                    _valid = false;
                    return null;
                }
                return msg;
            }
        }

        public delegate void ClientConnectionHandler(ReceivedMessage msg, ISender client, out object param);
        public delegate void ClientDisconnectionHandler(object param);
        public event ClientConnectionHandler OnCleintConnected;
        public event ClientDisconnectionHandler OnClientDisconnected;

        public delegate void ReceiveClientMsgAction(ReceivedMessage msg, ISender client, object param);
        public event ReceiveClientMsgAction OnClientMsgReceived;

        #endregion

        #region Start Server

        private readonly object _msgReceivedLock = new object();
        private readonly object _disconnectedLock = new object();

        public void Start()
        {
            while (true) {
                Socket clientSocket = _serverSocket.Accept();
                RemoteClient client = new RemoteClient(clientSocket);
                ReceivedMessage connectionMsg = client.ReceiveMessage();

                if (connectionMsg == null) {
                    // client disconnected
                    continue;
                }

                object clientParam = null;
                if (OnCleintConnected != null) {
                    OnCleintConnected(connectionMsg, client, out clientParam);
                }

                AsyncMessageReceiver.ReceiveAsync(
                    clientSocket,
                    System.Tuple.Create(client, clientParam),
                    OnMsgReceived,
                    OnClientDiconnected);
            }
        }

        private bool OnMsgReceived(object param, ReceivedMessage msg)
        {
            lock (_msgReceivedLock) {
                if (OnClientMsgReceived != null) {
                    var p = param as System.Tuple<RemoteClient, object>;
                    OnClientMsgReceived(msg, p.Item1, p.Item2);
                    return p.Item1.IsValid;
                }
            }

            return false;
        }

        private void OnClientDiconnected(object param)
        {
            lock (_disconnectedLock) {
                if (OnClientDisconnected != null) {
                    var p = param as System.Tuple<RemoteClient, object>;
                    OnClientDisconnected(p.Item2);
                }
            }
        }

        #endregion
    }
}
