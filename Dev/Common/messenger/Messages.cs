using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace EPBMessanger
{
    public interface ISender
    {
        WritableMessage NewMessage();
        void Disconnect();
    }

    public class ReceivedMessage
    {
        private readonly string _msgName;
        private readonly string _msg;

        internal static ReceivedMessage New(Socket s)
        {
            string msgName = Messages.ReceiveMessage(s);
            if (msgName == null) {
                return null;
            }
            string msg = Messages.ReceiveMessage(s);
            if (msg == null) {
                return null;
            }

            return new ReceivedMessage(msgName, msg);
        }

        private ReceivedMessage(string msgName, string msg)
        {
            _msgName = msgName;
            _msg = msg;
        }

        public string ReadName()
        {
            return _msgName;
        }

        public string Read()
        {
            return _msg;
        }
    }

    public class WritableMessage
    {
        private readonly Socket _socket;
        private string _msgName;
        private string _msg;

        internal WritableMessage(Socket s)
        {
            _socket = s;
        }

        public void WriteName(string msgName)
        {
            _msgName = msgName;
        }

        public void Write(string msg)
        {
            _msg = msg;
        }

        public bool Send()
        {
            return Messages.SendMessage(_socket, _msgName) &&
                   Messages.SendMessage(_socket, _msg);
        }
    }

    internal class AsyncMessageReceiver
    {
        public delegate bool MsgHandler(object param, ReceivedMessage msg);
        public delegate void DisconnectedAction(object param);

        private readonly object _param;
        private readonly Socket _socket;
        private readonly MsgHandler _onMsg;
        private readonly DisconnectedAction _onDisconnected;

        public static void ReceiveAsync(Socket s, object param, MsgHandler onMsg, DisconnectedAction onDisconnected)
        {
            new AsyncMessageReceiver(s, param, onMsg, onDisconnected).RunAsync();
        }

        private AsyncMessageReceiver(Socket s, object param, MsgHandler onMsg, DisconnectedAction onDisconnected)
        {
            _param = param;
            _socket = s;
            _onMsg = onMsg;
            _onDisconnected = onDisconnected;
        }

        private void RunAsync()
        {
            new Thread(new ThreadStart(() => {
                while (true) {
                    ReceivedMessage msg = ReceivedMessage.New(_socket);
                    if (msg == null) {
                        if (_onDisconnected != null) {
                            _onDisconnected(_param);
                        }
                        break;
                    }
                    if (_onMsg != null) {
                        if (!_onMsg(_param, msg)) {
                            if (_onDisconnected != null) {
                                _onDisconnected(_param);
                            }
                            break;
                        }
                    }
                }
            })).Start();
        }
    }

    internal static class Messages
    {
        public static bool SendMessage(Socket s, string msg)
        {
            try {
                byte[] buffer = Encoding.Default.GetBytes(msg);
                byte[] sizeBuffer = BitConverter.GetBytes(buffer.Length);
                s.Send(sizeBuffer, sizeBuffer.Length, SocketFlags.None);
                s.Send(buffer, buffer.Length, SocketFlags.None);
                return true;
            } catch (System.Exception ex) {
                ex.ToString();
                return false;
            }
        }

        public static string ReceiveMessage(Socket s)
        {
            try {
                byte[] sizeBuffer = new byte[sizeof(Int32)];
                if (s.Receive(sizeBuffer) == 0) {
                    return null;
                }
                int msgSize = BitConverter.ToInt32(sizeBuffer, 0);

                byte[] buffer = new byte[msgSize];
                int bytsRead = 0;
                while (bytsRead != msgSize) {
                    int read = s.Receive(buffer, bytsRead, msgSize - bytsRead, SocketFlags.None);
                    if (read == 0) {
                        return null;
                    }
                    bytsRead += read;
                }

                return Encoding.Default.GetString(buffer);
            } catch (System.Exception ex) {
                ex.ToString();
                return null;
            }
        }
    }
}
