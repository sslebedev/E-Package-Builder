using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPB2Messanger;

namespace EPB2Client
{
   class ClientStorage
   {
      private static Client _client;
      public static Client Client
      {
         private set { _client = value; }
         get { return _client;  }
      }

      public static void Init()
      {
         Client = new Client();
      }
   }
}
