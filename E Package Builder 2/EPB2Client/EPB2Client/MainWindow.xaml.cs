using System;
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
using EPB2Messanger;

namespace EPB2Client
{
   /// <summary>
   /// Логика взаимодействия для MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private int messageCount = 0;
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

      }

      private void OnServerConnected()
      {
         mainGrid.Background = (Brush)this.Resources["ConnectedBrush"];
      }

      private void OnServerDisconnected()
      {
         Delegate onServerDisconnected = new Action(() =>
            { mainGrid.Background = (Brush)this.Resources["NonConnectedBrush"]; });

         Dispatcher.Invoke(onServerDisconnected, null);
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

      private void Button_Click_1(object sender, RoutedEventArgs e)
      {
         WritableMessage msg = ClientStorage.Client.NewMessage();
         msg.WriteName("Text");
         msg.Write(string.Format("Message {0}: {1}",
            messageCount++, textMessage.Text));
         msg.Send();
      }
   }
}
