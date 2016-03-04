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
using System.Windows.Shapes;

namespace EPB2Client
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
            !BuilderClient.Connect(textServerIP.Text, port))
         {
            MessageBox.Show("Connection failed!");
         }

         Hide();
      }

      private void Button_Click_2(object sender, RoutedEventArgs e)
      {
         Hide();
      }
   }
}
