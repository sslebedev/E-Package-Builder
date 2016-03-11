using EPBMessanger;

namespace EPBClient
{
    class ClientStorage
    {
        private static Client _client;
        public static Client Client
        {
            private set { _client = value; }
            get { return _client; }
        }

        public static void Init()
        {
            Client = new Client();
        }
    }
}
