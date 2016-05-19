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
        public MainWindow()
        {
            InitializeComponent();

            ClientStorage.Init();
            ClientStorage.Client.OnServerConnected += OnServerConnected;
            ClientStorage.Client.OnServerDisconnected += OnServerDisconnected;
            ClientStorage.Client.OnServerMsgReceived += OnServerMsgReceived;

            /*ClientStorage.Client.Connect("127.0.0.1", 11000, "EPBConfigEditor");

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("GetProjects");
            msg.Write("");
            msg.Send();*/
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
                string[] FilePaths = msg.Read().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                MainWndAction(new Action(() =>
                {

                    textBox1.Text = FilePaths[0];
                    textBox2.Text = FilePaths[1];
                    textBox3.Text = FilePaths[2];
                    textBox4.Text = FilePaths[3];
                    textBox5.Text = FilePaths[4];
                }));
            }
        }

        private void OnServerConnected()
        {
            //MessageBox.Show("Connected");

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("GetProjects");
            msg.Write("");
            msg.Send();
        }

        private void OnServerDisconnected()
        {
            Delegate onServerDisconnected = new Action(() => { });

            Dispatcher.Invoke(onServerDisconnected, null);
        }

        private void MainWndAction(Delegate d)
        {
            Dispatcher.Invoke(d, null);
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedProject = listBox1.SelectedItem.ToString();
            //MessageBox.Show(selectedProject + ".txt");

            label2.Content = selectedProject;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("CheckOutConfigFile");
            msg.Write(selectedProject);
            msg.Send();

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            label2.Content = "Config is not selected";
            textBox1.Text = "Nil";
            textBox2.Text = "Nil";
            textBox3.Text = "Nil";
            textBox4.Text = "Nil";
            textBox5.Text = "Nil";
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(textBox1.Text);
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


    }
}
