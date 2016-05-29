using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EPBMessanger;
using System.Text;
using EPackageBuilder;
using System.Collections.Generic;

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

            private ClientParamBase(int uid)
            {
                UID = uid;
            }

            private static int nextClientUID;

            private static int CreateClientUID()
            {
                return nextClientUID++;
            }
        }

        private class ClientLogger : BuilderFunctions.ILogger
        {
            private readonly ISender client;

            public ClientLogger(ISender s)
            {
                client = s;
            }

            public void WriteLogLine(String txt = "")
            {
                var msg = client.NewMessage();
                msg.WriteName("BuildLog");
                msg.Write(txt);
                msg.Send();
            }
        }

        private static readonly Dictionary<int, ISender> clients = new Dictionary<int, ISender>();

        private static void OnClientConnected(ReceivedMessage msg, ISender client, out object param)
        {
            param = null;

            var msgName = msg.ReadName();
            var message = msg.Read();
            if (msgName != "ConnectionMessage") {
                client.Disconnect();
                return;
            }

            var clientParam = ClientParamBase.Make();
            param = clientParam;

            clients.Add(clientParam.UID, client);

            Console.WriteLine("New client connected\n" +
                "Connection message: {0}\n" +
                "Granted UID: {1}\n\n", message, clientParam.UID);

            WritableMessage newMsg = client.NewMessage();
            newMsg.WriteName("ServerResponse");
            newMsg.Write("Connection success");
            newMsg.Send();

            if (message == "EPB2Client")
            {
                WritableMessage projectsMsg = client.NewMessage();
                projectsMsg.WriteName("ProjectsInfo");
                projectsMsg.Write(Builder.GenerateProjectsInfo());
                projectsMsg.Send();
            }
        }

        private static void OnProjectStateUpdate()
        {
            foreach (var client in clients)
            {
                WritableMessage projectsMsg = client.Value.NewMessage();
                projectsMsg.WriteName("ProjectsInfo");
                projectsMsg.Write(Builder.GenerateProjectsInfo());
                projectsMsg.Send();
            }
        }

        private static void OnClientDisconnected(object param)
        {
            if (param == null)
                return;


            var clientParam = (ClientParamBase)param;

            clients.Remove(clientParam.UID);

            Console.WriteLine("Client disconnected\n" +
                "Client UID: {0}\n\n", clientParam.UID);
        }

        private static void OnClientMsgReceived(ReceivedMessage msg, ISender client, object param)
        {
            var clientParam = (ClientParamBase)param;
            string msgName = msg.ReadName();
            string message = msg.Read();

            Console.WriteLine("Client message received\n" +
                "Client UID: {0}\n" +
                "Message name: {1}\n" +
                "Message : {2}\n\n", clientParam.UID, msgName, message);

            if (message == "disconnect") {
                client.Disconnect();
                return;
            }

            switch (msgName) {
                case "RequestBuild": {
                    var args = message.Split();

                    Action<Builder.BuildType> addBuildRequest = buildType => 
                         Builder.AddBuildRequest(clientParam.UID,
                                new ClientLogger(client),
                                args[1],
                                args[2],
                                buildType);
                    switch (args[0])
                    {
                        case "Sources":
                            Console.WriteLine("addBuildRequest(Builder.BuildType.MakeSources);");
                            addBuildRequest(Builder.BuildType.MakeSources);
                            break;
                        case "PC":
                            Console.WriteLine("addBuildRequest(Builder.BuildType.BuildPC);");
                            addBuildRequest(Builder.BuildType.BuildPC);
                            break;
                        case "Release":
                            Console.WriteLine("addBuildRequest(Builder.BuildType.BuildRelease);");
                            addBuildRequest(Builder.BuildType.BuildRelease);
                            break;
                        case "Full":
                            Console.WriteLine("addBuildRequest(Builder.BuildType.BuildFull);");
                            addBuildRequest(Builder.BuildType.BuildFull);
                            break;
                    }
                }
                    break;
                case "UpdateQueueTable":
                    Debug.Assert(false, "NotImplemented"); // TODO implement it
                    break;
                case "GetProjects": {
                    var clientMsg = client.NewMessage();
                    clientMsg.WriteName("Projects");

                    var s = GenerateResponceGetProjects();
                    clientMsg.Write(s);
                    clientMsg.Send();
                    }
                    break;
                case "CheckOutConfigFile": {
                    var clientMsg = client.NewMessage();
                    clientMsg.WriteName("CheckOutConfigFile");
                    clientMsg.Write(Builder.CheckoutConfig(message));
                    clientMsg.Send();
                    }
                    break;
                default:
                    throw new Exception("Unknown request"); // TODO Handle it
            }
        }

        private static string GenerateResponceGetProjects()
        {
            var sb = new StringBuilder();
            foreach (var proj in Builder.Projects) {
                sb.Append(proj);
                sb.Append("\n");
            }

            var s = sb.ToString();
            return s;
        }

        private static readonly Builder Builder = new Builder();
        private static readonly Server Server = new Server();

        public static void Main()
        {
            try {
                // Init server
                Server.OnCleintConnected += OnClientConnected;
                Server.OnClientMsgReceived += OnClientMsgReceived;
                Server.OnClientDisconnected += OnClientDisconnected;

                // Init builder
                Builder.Init();
                Console.WriteLine(GenerateBuilderInfo());
                Builder.OnProjectsStateUpdate += OnProjectStateUpdate;

                // Start tasks
                Task.Run(() => Server.Start());
                Task.Run(() => Builder.Start());
                Thread.Sleep(Timeout.Infinite);
            } catch(FileNotFoundException exception) {
                Console.WriteLine(exception.Message);
            }
        }

        private static string GenerateBuilderInfo()
        {
            var sb = new StringBuilder("Available projects:\n");
            for (var i = 0; i < Builder.Projects.Length; ++i) {
                sb.Append("    ");
                sb.Append(i + 1);
                sb.Append(". ");
                sb.Append(Builder.Projects[i]);
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
