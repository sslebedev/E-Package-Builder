using System;
using System.Windows;
using EPBClient;

namespace EPBClient
{
    /// <summary>
    /// Логика взаимодействия для OpenConnectionWindow.xaml
    /// </summary>
    public partial class OpenConnectionWindow : Window
    {
        public OpenConnectionWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Int32 port;
            if (!Int32.TryParse(textPort.Text, out port) ||
               !ClientStorage.Client.Connect(textServerIP.Text, port, "EPB2Client"))
            {
                MainWindow.Logger.WriteLog("Connection failed!");
            }
            else
            {
                MainWindow.Logger.WriteLog("Connection success!");
            }

            Hide();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
