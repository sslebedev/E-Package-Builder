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

         BuilderClient.Init();
         BuilderClient.OnServerConnected += OnServerConnected;
         BuilderClient.OnServerDisconnected += OnServerDisconnected;
      }

      private void OnServerConnected()
      {
         mainGrid.Background = (Brush)this.Resources["ConnectedBrush"];
      }

      private void OnServerDisconnected()
      {
         mainGrid.Background = (Brush)this.Resources["NonConnectedBrush"];
      }

      private void MenuItem_Click_1(object sender, RoutedEventArgs e)
      {
         OpenConnectionWindow w = new OpenConnectionWindow();
         w.Show();
      }

      private void MenuItem_Click_2(object sender, RoutedEventArgs e)
      {
         BuilderClient.Discinnect();
      }

      private void Button_Click_1(object sender, RoutedEventArgs e)
      {
         BuilderClient.SendMessage(string.Format("Message {0}: {1}",
            messageCount++, textMessage.Text));
      }
   }
}
