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

namespace epb
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            string[] Projects = { "Project1", "Project2", "Project3", "Project4", "Project5" };
            listBox1.ItemsSource = Projects;
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedProject = listBox1.SelectedItem.ToString();
            //MessageBox.Show(selectedProject + ".txt");

            label2.Content = selectedProject;

            if (File.Exists(selectedProject + ".txt"))
            {
                string[] ProjectContent = File.ReadAllLines(selectedProject + ".txt");

                textBox1.Text = ProjectContent[1];
                textBox2.Text = ProjectContent[2];
                textBox3.Text = ProjectContent[3];
                textBox4.Text = ProjectContent[4];
                textBox5.Text = ProjectContent[5];
            }
            else
            {
                textBox1.Text = "Nan";
                textBox2.Text = "Nan";
                textBox3.Text = "Nan";
                textBox4.Text = "Nan";
                textBox5.Text = "Nan";
                MessageBox.Show("Config is empty");
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            label2.Content = "Config is not selected";
            textBox1.Text = "Nan";
            textBox2.Text = "Nan";
            textBox3.Text = "Nan";
            textBox4.Text = "Nan";
            textBox5.Text = "Nan";
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


    }
}
