using System;
using System.Net;
using System.Net.Sockets;

namespace EPBMessanger
{
    public class Client
    {
        private Socket _socket;
        private bool _valid;

        public event Action OnServerConnected;
        public event Action OnServerDisconnected;

        public delegate void ServerMsgAction(ReceivedMessage msg);
        public event ServerMsgAction OnServerMsgReceived;

        public Client()
        {
            _valid = true;
        }

        public WritableMessage NewMessage()
        {
            if (!_valid) {
                return null;
            }
            return new WritableMessage(_socket);
        }

        public bool Connect(string serverIP, int port, string connectionMsg)
        {
            try {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(ipEndPoint);

                _valid = true;
            } catch (System.Exception e) {
                e.ToString();
                return false;
            }

            WritableMessage newMsg = NewMessage();
            newMsg.WriteName("ConnectionMessage");
            newMsg.Write(connectionMsg);
            newMsg.Send();

            if (OnServerConnected != null) {
                OnServerConnected();
            }

            AsyncMessageReceiver.ReceiveAsync(_socket, null,
               (param, msg) => {
                   if (OnServerMsgReceived != null)
                       OnServerMsgReceived(msg);
                   return _valid;
               },
               (param) => {
                   if (OnServerDisconnected != null)
                       OnServerDisconnected();
                   _valid = false;
               });
            return true;
        }

        public void Disconnect()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

            if (OnServerDisconnected != null) {
                OnServerDisconnected();
            }
        }
    }
}
