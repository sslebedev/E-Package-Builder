﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPB2Messanger;

namespace EPB2Server
{
   class Program
   {
      private enum IncomingMessage
      {
         
      }
      private static void OnClientConnected(Server.RemoteClient client, ReceivedMessage message)
      {
         Console.WriteLine(string.Format("Client connected with (nameMsg '{0}', msg '{1}')",
            message.ReadName(), message.Read()));

         WritableMessage newMsg = client.NewMessage();
         newMsg.WriteName("Server response");
         newMsg.Write("Connection success");
         newMsg.Send();
      }

      private static void OnClientMsgReceived(Server.RemoteClient client, ReceivedMessage message)
      {
         string msgName = message.ReadName();
         string msg = message.Read();

         Console.WriteLine(string.Format("Received message (nameMsg '{0}', msg '{1}')",
            msgName, msg));

         if (msgName == "RequestBuild")
         {
            // buildQueue.Push(...);
         }
         else if (msgName == "UpdateQueueTable")
         {
            // WritableMessage newMsg = client.NewMessage();
            // write build queue state
         }
         else if (msgName == "CheckoutConfigFile")
         {
            // ...
         }
      }

      static void Main(string[] args)
      {
         Server server = new Server();

         server.OnCleintConnected += OnClientConnected;
         server.OnClientMsgReceived += OnClientMsgReceived;

         server.Start();
      }
   }
}
