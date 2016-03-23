using System;
using EPBMessanger;
using System.Threading;

namespace EPBServer
{
    class Program
    {
        public struct ClientParamBase
        {
            public readonly int UID;

            public static ClientParamBase Make()
            {
                return new ClientParamBase(CreateClientUID());
            }

            private ClientParamBase(int UID)
            {
                this.UID = UID;
            }

            private static int _nextClientUID = 0;

            private static int CreateClientUID()
            {
                return _nextClientUID++;
            }
        }

        private static void OnClientConnected(ReceivedMessage msg, ISender client, out object param)
        {
            param = null;

            string msgName = msg.ReadName();
            string message = msg.Read();
            if (msgName != "ConnectionMessage") {
                client.Disconnect();
                return;
            }

            ClientParamBase clientParam = ClientParamBase.Make();
            param = clientParam;

            Console.WriteLine(String.Format(
                "New client connected\n" +
                "Connection message: {0}\n" +
                "Granted UID: {1}\n\n", message, clientParam.UID));

            WritableMessage newMsg = client.NewMessage();
            newMsg.WriteName("ServerResponse");
            newMsg.Write("Connection success");
            newMsg.Send();
        }

        private static void OnClientDisconnected(object param)
        {
            if (param == null)
                return;

            ClientParamBase clientParam = (ClientParamBase)param;

            Console.WriteLine(String.Format(
                "Client disconnected\n" +
                "Client UID: {0}\n\n", clientParam.UID));
        }

        private static void OnClientMsgReceived(ReceivedMessage msg, ISender client, object param)
        {
            ClientParamBase clientParam = (ClientParamBase)param;
            string msgName = msg.ReadName();
            string message = msg.Read();

            Console.WriteLine(String.Format(
                "Client message received\n" +
                "Client UID: {0}\n" +
                "Message name: {1}\n" +
                "Message : {2}\n\n", clientParam.UID, msgName, message));

            if (message == "disconnect") {
                client.Disconnect();
                return;
            }

            if (msgName == "RequestBuild") {
                // buildQueue.Push(...);
            } else if (msgName == "UpdateQueueTable") {
                // WritableMessage newMsg = client.NewMessage();
                // write build queue state
            } else if (msgName == "CheckoutConfigFile") {
                // ...
            }
        }

        static void Main(string[] args)
        {
            Server server = new Server();
            server.OnCleintConnected += OnClientConnected;
            server.OnClientMsgReceived += OnClientMsgReceived;
            server.OnClientDisconnected += OnClientDisconnected;


            Thread t = new Thread(server.Start);
            t.Start();

            while (Console.ReadLine() != "exit")
                ;

            t.Interrupt();
        }
    }
}
