using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace EPBMessanger
{
    public class Server
    {
        private readonly Socket serverSocket;

        public Server(int port = 11000, int backlog = 100)
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(backlog);
        }

        #region Remote Client

        private class RemoteClient : ISender
        {
            private readonly Socket socket;
            
            public bool IsValid { private set; get; }

            public WritableMessage NewMessage()
            {
                return IsValid ? new WritableMessage(socket) : null;
            }

            public void Disconnect()
            {
                IsValid = false;

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            internal RemoteClient(Socket s)
            {
                socket = s;
                IsValid = true;
            }

            internal ReceivedMessage ReceiveMessage()
            {
                var msg = ReceivedMessage.New(socket);
                if (msg == null) {
                    IsValid = false;
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

        private readonly object msgReceivedLock = new object();
        private readonly object disconnectedLock = new object();

        public void Start()
        {
            while (true) {
                var clientSocket = serverSocket.Accept();
                var client = new RemoteClient(clientSocket);
                var connectionMsg = client.ReceiveMessage();

                if (connectionMsg == null) {
                    // client disconnected
                    continue;
                }

                object clientParam = null;
                var evt = OnCleintConnected;  // Sic!
                if (evt != null) {
                    evt(connectionMsg, client, out clientParam);
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
            lock (msgReceivedLock) {
                var evt = OnClientMsgReceived;  // Sic!
                if (evt != null) {
                    var p = param as System.Tuple<RemoteClient, object>;
                    Debug.Assert(p != null, "p != null");
                    evt(msg, p.Item1, p.Item2);
                    return p.Item1.IsValid;
                }
            }

            return false;
        }

        private void OnClientDiconnected(object param)
        {
            lock (disconnectedLock) {
                var evt = OnClientDisconnected; // Sic!
                if (evt == null) {
                    return;
                }

                var p = param as System.Tuple<RemoteClient, object>;
                Debug.Assert(p != null, "p != null");
                evt(p.Item2);
            }
        }

        #endregion
    }
}
