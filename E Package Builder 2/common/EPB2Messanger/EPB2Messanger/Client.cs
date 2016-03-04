using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace EPB2Messanger
{
   public class Client
   {
      private Socket _socket;
      private bool _valid;

      public Client()
      {
         _valid = true;
      }

      public WritableMessage NewMessage()
      {
         if (!_valid)
         {
            return null;
         }
         return new WritableMessage(_socket);
      }

      //internal ReceivedMessage ReceiveMessage()
      //{
      //   string msgName = Messages.ReceiveMessage(_socket);
      //   if (msgName == null) {
      //      _valid = false;
      //      return null;
      //   }
      //   string msg = Messages.ReceiveMessage(_socket);
      //   if (msg == null) {
      //      _valid = false;
      //      return null;
      //   }
      //
      //   return new ReceivedMessage(msgName, msg);
      //}

      public bool Connect(string serverIP, int port, string connectionMsg)
      {
         try
         {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ipEndPoint);
         }
         catch (System.Exception e)
         {
            e.ToString();
            return false;
         }

         WritableMessage newMsg = NewMessage();
         newMsg.WriteName("ConnectionMessage");
         newMsg.Write(connectionMsg);
         newMsg.Send();
         return true;
      }

      public void Disconnect()
      {
         _socket.Shutdown(SocketShutdown.Both);
         _socket.Close();
      }
   }
}
