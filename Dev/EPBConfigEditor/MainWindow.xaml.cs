using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EPBMessanger;
using System.Diagnostics;


namespace epb
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

    public partial class MainWindow : Window
    {
        class ConfigContent
        {
            public string[] Paths;

            public static ConfigContent ParseFromString(string str)
            {
                string[] paths = str.Split(new char[] { '\n' });
                return new ConfigContent(paths);
            }

            public override string ToString()
            {
                return String.Format("{0}\n{1}\n{2}\n{3}\n{4}",
                    Paths[0],
                    Paths[1],
                    Paths[2],
                    Paths[3],
                    Paths[4]);
            }

            public ConfigContent()
            {
                Paths = new string[] { "", "", "", "", "" };
            }

            private ConfigContent(string[] paths)
            {
                Debug.Assert(paths.Length == 5);
                Paths = paths;
            }
        }

        ConfigContent _checkoutedConfig;

        public MainWindow()
        {
            InitializeComponent();

            ClientStorage.Init();
            ClientStorage.Client.OnServerConnected += OnServerConnected;
            ClientStorage.Client.OnServerDisconnected += OnServerDisconnected;
            ClientStorage.Client.OnServerMsgReceived += OnServerMsgReceived;
        }

        private void OnServerMsgReceived(ReceivedMessage msg)
        {
            string msgNameFromServer = msg.ReadName();
            if (msgNameFromServer == "Projects")
            {
                string[] projectsName = msg.Read().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                MainWndAction(new Action(() => { listBox1.ItemsSource = projectsName; }));
            }
            else if (msgNameFromServer == "CheckOutConfigFile")
            {
                _checkoutedConfig = ConfigContent.ParseFromString(msg.Read());
                MainWndAction(() => SetActiveConfig(_checkoutedConfig));
            }
        }

        private void OnServerConnected()
        {
            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("GetProjects");
            msg.Write("");
            msg.Send();
        }

        private void OnServerDisconnected()
        {
            MainWndAction(() => {
                label2.Content = "";
                listBox1.SelectedItem = null;
                listBox1.ItemsSource = null;
                SetActiveConfig(new ConfigContent());
            });
        }

        private void MainWndAction(Action d)
        {
            Dispatcher.Invoke(d, null);
        }

        private void SetActiveConfig(ConfigContent cfg)
        {
            Debug.Assert(cfg.Paths.Length == 5);

            textBox1.Text = cfg.Paths[0];
            textBox2.Text = cfg.Paths[1];
            textBox3.Text = cfg.Paths[2];
            textBox4.Text = cfg.Paths[3];
            textBox5.Text = cfg.Paths[4];
        }

        private ConfigContent SaveActiveConfig()
        {
            ConfigContent res = new ConfigContent();
            res.Paths[0] = textBox1.Text;
            res.Paths[1] = textBox2.Text;
            res.Paths[2] = textBox3.Text;
            res.Paths[3] = textBox4.Text;
            res.Paths[4] = textBox5.Text;
            return res;
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;

            string selectedProject = listBox1.SelectedItem.ToString();

            if (((string)label2.Content).Length != 0)
            {
                CheckInConfig();
            }

            label2.Content = selectedProject;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("CheckOutConfigFile");
            msg.Write(selectedProject);
            msg.Send();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("CheckInConfigCancel");
            msg.Write("");
            msg.Send();

            label2.Content = "";
            listBox1.SelectedItem = null;
            SetActiveConfig(new ConfigContent());
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            CheckInConfig();

            label2.Content = "";
            listBox1.SelectedItem = null;
            SetActiveConfig(new ConfigContent());
        }

        private void CheckInConfig()
        {
            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("CheckInConfigFile");
            msg.Write(SaveActiveConfig().ToString());
            msg.Send();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            ConfigContent current = SaveActiveConfig();
            current.Paths[0] = _checkoutedConfig.Paths[0];
            SetActiveConfig(current);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ConfigContent current = SaveActiveConfig();
            current.Paths[1] = _checkoutedConfig.Paths[1];
            SetActiveConfig(current);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            ConfigContent current = SaveActiveConfig();
            current.Paths[2] = _checkoutedConfig.Paths[2];
            SetActiveConfig(current);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            ConfigContent current = SaveActiveConfig();
            current.Paths[3] = _checkoutedConfig.Paths[3];
            SetActiveConfig(current);
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            ConfigContent current = SaveActiveConfig();
            current.Paths[4] = _checkoutedConfig.Paths[4];
            SetActiveConfig(current);
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            OpenConnectionWindow w = new OpenConnectionWindow();
            w.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            ClientStorage.Client.Disconnect();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            string text = System.IO.File.ReadAllText("..\\..\\about_program.txt", System.Text.Encoding.GetEncoding(1251));
            MessageBox.Show(text);
        }
    }
}
