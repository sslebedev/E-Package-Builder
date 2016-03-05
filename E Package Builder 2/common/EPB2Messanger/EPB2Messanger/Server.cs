using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace EPB2Messanger
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

      public class RemoteClient
      {
         private readonly Socket _socket;
         private bool _valid;

         public WritableMessage NewMessage()
         {
            if (!_valid)
            {
               return null;
            }
            return new WritableMessage(_socket);
         }

         internal RemoteClient(Socket s)
         {
            _socket = s;
            _valid = true;
         }

         internal ReceivedMessage ReceiveMessage()
         {
            ReceivedMessage msg = ReceivedMessage.New(_socket);
            if (msg == null)
            {
               _valid = false;
               return null;
            }
            return msg;
         }
      }

      public delegate void RemoteClientMsgAction(RemoteClient client, ReceivedMessage msg);
      public event RemoteClientMsgAction OnCleintConnected;
      public event RemoteClientMsgAction OnClientMsgReceived;

      public delegate void RemoteClientAction(RemoteClient client);
      public event RemoteClientAction OnClientDisconnected;

      #endregion

      #region Start Server

      private readonly object _msgReceivedLock = new object();
      private readonly object _disconnectedLock = new object();

      public void Start()
      {
         while (true)
         {
            Socket clientSocket = _serverSocket.Accept();
            RemoteClient client = new RemoteClient(clientSocket);
            ReceivedMessage connectionMsg = client.ReceiveMessage();

            if (connectionMsg == null)
            {
               // client disconnected
               continue;
            }

            if (OnCleintConnected != null)
            {
               OnCleintConnected(client, connectionMsg);
            }

            AsyncMessageReceiver.ReceiveAsync(clientSocket, client, OnMsgReceived, OnClientDiconnected);
         }
      }

      private void OnMsgReceived(object param, ReceivedMessage msg)
      {
         lock (_msgReceivedLock)
         {
            if (OnClientMsgReceived != null)
            {
               OnClientMsgReceived((RemoteClient)param, msg);
            }
         }
      }

      private void OnClientDiconnected(object param)
      {
         lock (_disconnectedLock)
         {
            if (OnClientDisconnected != null)
            {
               OnClientDisconnected((RemoteClient)param);
            }
         }
      }

      #endregion
   }
}
