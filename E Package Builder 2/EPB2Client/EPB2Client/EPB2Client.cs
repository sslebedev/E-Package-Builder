using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPB2Messanger;

namespace EPB2Client
{
   class BuilderClient
   {
      public static event Action OnServerConnected;
      public static event Action OnServerDisconnected;

      public static void Init()
      {
         _instance = new BuilderClient();
      }

      public static bool Connect(string serverIP, int potr)
      {
         if (_instance._client.Connect(serverIP, potr, "EPB2Client"))
         {
            if (OnServerConnected != null)
            {
               OnServerConnected();
            }
            return true;
         }

         return false;
      }

      public static void SendMessage(string msg)
      {
         WritableMessage newMsg = _instance._client.NewMessage();
         newMsg.WriteName("");
         newMsg.Write(msg);
         newMsg.Send();
      }

      public static void Discinnect()
      {
         _instance._client.Disconnect();

         if (OnServerDisconnected != null)
         {
            OnServerDisconnected();
         }
      }

      private BuilderClient()
      {
         _client = new Client();
      }

      private static BuilderClient _instance;
      private Client _client;
   }
}
