using System;
using EPBMessanger;
using System.Threading;
using System.Text;
using EPackageBuilder;

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

        private class ClientLogger : BuilderFunctions.ILogger
        {
            private readonly ISender _client;

            public ClientLogger(ISender s)
            {
                _client = s;
            }

            public void WriteLogLine(String txt = "")
            {
                var msg = _client.NewMessage();
                msg.WriteName("BuildLog");
                msg.Write(txt);
                msg.Send();
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
                var args = message.Split();

                switch (args[0])
                {
                    case "Sources":
                        _builder.AddBuildRequest(clientParam.UID, new ClientLogger(client), args[1], Builder.BuildType.MakeSources);
                        break;
                    case "PC":
                        _builder.AddBuildRequest(clientParam.UID, new ClientLogger(client), args[1], Builder.BuildType.BuildPC);
                        break;
                    case "Release":
                        _builder.AddBuildRequest(clientParam.UID, new ClientLogger(client), args[1], Builder.BuildType.BuildRelease);
                        break;
                    case "Full":
                        _builder.AddBuildRequest(clientParam.UID, new ClientLogger(client), args[1], Builder.BuildType.BuildFull);
                        break;
                }
            } else if (msgName == "UpdateQueueTable") {
                // WritableMessage newMsg = client.NewMessage();
                // write build queue state
            } else if (msgName == "GetProjects") {
                var clientMsg = client.NewMessage();
                clientMsg.WriteName("Projects");

                StringBuilder sb = new StringBuilder();
                foreach (var proj in _builder.Projects)
                {
                    sb.Append(proj);
                    sb.Append("\n");
                }

                clientMsg.Write(sb.ToString());
                clientMsg.Send();
            }
            else if (msgName == "CheckOutConfigFile") // TODO SSL: include it in switch/case when merged
            {
                var clientMsg = client.NewMessage();
                clientMsg.WriteName("CheckOutConfigFile");
                clientMsg.Write(_builder.CheckoutConfig(message));
                clientMsg.Send();
            }
        }

        static private Builder _builder = new Builder();

        static void Main(string[] args)
        {
            if (!BuilderInit())
                return;

            Server server = new Server();
            server.OnCleintConnected += OnClientConnected;
            server.OnClientMsgReceived += OnClientMsgReceived;
            server.OnClientDisconnected += OnClientDisconnected;


            Thread t = new Thread(server.Start);
            t.Start();

            _builder.Start();
            t.Interrupt();
        }

        static private bool BuilderInit()
        {
            if (!_builder.Init())
            {
                Console.WriteLine("Builder initialization failed!");
                return false;
            }

            StringBuilder sb = new StringBuilder("Builder initialization success!\nAvailable projects:\n");
            for (int i = 0; i < _builder.Projects.Count; ++i)
            {
                sb.Append("    ");
                sb.Append(i + 1);
                sb.Append(". ");
                sb.Append(_builder.Projects[i]);
                sb.Append("\n");
            }

            Console.WriteLine(sb.ToString());
            return true;
        }
    }
}
