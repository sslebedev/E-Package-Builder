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

      public delegate void RemoteClientAction(RemoteClient client, ReceivedMessage msg);
      public event RemoteClientAction OnCleintConnected;
      public event RemoteClientAction OnClientMsgReceived;

      private readonly Socket _serverSocket;

      public Server(int port = 11000, int backlog = 100)
      {
         IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
         _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

         _serverSocket.Bind(ipEndPoint);
         _serverSocket.Listen(backlog);
      }

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

            //new ClientMessageReceiver(client, this).RunAsync();
            AsyncMessageReceiver.ReceiveAsync(clientSocket, client, OnMsgReceived);
         }
      }

      private readonly object _msgReceivedLock = new object();
      internal void OnMsgReceived(object param, ReceivedMessage msg)
      {
         lock (_msgReceivedLock)
         {
            if (OnClientMsgReceived != null)
            {
               OnClientMsgReceived((RemoteClient)param, msg);
            }
         }
      }

      private class ClientMessageReceiver
      {
         private readonly RemoteClient _client;
         private readonly Server _server;

         public ClientMessageReceiver(RemoteClient client, Server server)
         {
            _client = client;
            _server = server;
         }

         public void RunAsync()
         {
            Thread t = new Thread(new ThreadStart(this.Run));
            t.Start();
         }

         private void Run()
         {
            while (true)
            {
               ReceivedMessage msg = _client.ReceiveMessage();
               if (msg == null)
               {
                  // client disconnected
                  break;
               }
               _server.OnMsgReceived(_client, msg);
            }
         }
      }
   }
}
