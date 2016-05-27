using System;
using System.Windows;
using System.Windows.Media;
using EPBClient;
using EPBMessanger;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Input;

namespace EPBClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class MainLogger
        {
            private readonly MainWindow _mainWindow;
            private readonly TextRange _logs;

            public MainLogger(MainWindow w)
            {
                _mainWindow = w;
                _logs = new TextRange(w.TextLog.Document.ContentStart,
                   w.TextLog.Document.ContentEnd);
            }

            public void WriteLog(string msg)
            {
                string time = System.DateTime.Now.ToLongTimeString();
                string date = System.DateTime.Now.ToLongDateString();
                string logMsg = String.Format("{0}, {1} : {2}\n", time, date, msg);

                Delegate append = new Action(() => _logs.Text += logMsg);
                _mainWindow.Dispatcher.Invoke(append, null);
            }

            public void Clear()
            {
                Delegate append = new Action(() => _logs.Text = "");
                _mainWindow.Dispatcher.Invoke(append, null);
            }
        }

        private static MainLogger _logger;
        public static MainLogger Logger
        {
            get { return _logger; }
        }

        public MainWindow()
        {
            InitializeComponent();
            ProjectsView.ItemsSource = _projects;
            _logger = new MainLogger(this);

            ClientStorage.Init();
            ClientStorage.Client.OnServerConnected += OnServerConnected;
            ClientStorage.Client.OnServerDisconnected += OnServerDisconnected;
            ClientStorage.Client.OnServerMsgReceived += OnServerMsgReceived;

            Logger.WriteLog("Attention! Server not connected!");
        }

        private void OnServerMsgReceived(ReceivedMessage msg)
        {
            string msgName = msg.ReadName();

            if (msgName == "Projects")
            {
                string projects = msg.Read();
                SetProjects(projects.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else if (msgName == "BuildLog")
            {
                Logger.WriteLog(msg.Read());
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
            Logger.WriteLog("Server disconnected!");
            SetProjects(new string[0]);
        }

        private void MainWndAction(Action a)
        {
            Dispatcher.Invoke(a, null);
        }

        private void SetProjects(string[] projects)
        {
            MainWndAction(() =>
            {
                _projects.Clear();

                foreach (var proj in projects)
                    _projects.Add(new ProjectsEntity(proj));
            });
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

        private readonly ObservableCollection<ProjectsEntity> _projects = new ObservableCollection<ProjectsEntity>();

        public ObservableCollection<ProjectsEntity> Projects
        {
            get { return _projects; }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _projects[0].SrcStatus = new Status(BuildStatus.Ready);
            _projects[0].IsSelected = true;
        }

        private void Row_MouseLeave(object sender, MouseEventArgs args)
        {

        }


        private void ProjectsView_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
                return;

            if (e.AddedItems.Count == 1)
            {
                if (dataGrid.SelectedItems.Count != 1)
                    return;

                SelectedProjText.Content = (e.AddedItems[0] as ProjectsEntity).Name;
            }
            else if (e.RemovedItems.Count == 1)
            {
                if (dataGrid.SelectedItems.Count != 1)
                    return;

                SelectedProjText.Content = "";
            }
        }

        private void ButtonSources_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjText.Content as string == "")
                return;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("RequestBuild");
            msg.Write(String.Format("{0}\n{1}", "Sources", SelectedProjText.Content));
            msg.Send();
        }

        private void ButtonPC_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjText.Content as string == "")
                return;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("RequestBuild");
            msg.Write(String.Format("{0}\n{1}", "PC", SelectedProjText.Content));
            msg.Send();
        }

        private void ButtonRelease_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjText.Content as string == "")
                return;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("RequestBuild");
            msg.Write(String.Format("{0}\n{1}", "Release", SelectedProjText.Content));
            msg.Send();
        }

        private void ButtonFull_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjText.Content as string == "")
                return;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("RequestBuild");
            msg.Write(String.Format("{0}\n{1}", "Full", SelectedProjText.Content));
            msg.Send();
        }
    }

    public enum BuildStatus
    {
        Ready,
        Building,
        Failed
    }

    public struct Status
    {
        public string queuePos;
        public BuildStatus status;

        public Status(BuildStatus status = BuildStatus.Failed, string pos = "")
        {
            this.status = status;
            this.queuePos = pos;
        }
    }

    public class ProjectsEntity : INotifyPropertyChanged
    {
        public ProjectsEntity(string name)
        {
            _name = name;
            SrcStatus = new Status(BuildStatus.Building, "1");
            SrcToolTip = "Version #Latest\nYour Task\nIn Progress";
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private Status _src;
        public Status SrcStatus
        {
            get { return _src; }
            set
            {
                _src = value;
                RaisePropertyChanged("SrcStatus");
            }
        }

        private string _srcToolTip;
        public string SrcToolTip
        {
            get { return _srcToolTip; }
            set
            {
                _srcToolTip = value;
                RaisePropertyChanged("SrcToolTip");
            }
        }


        private void RaisePropertyChanged(string name)
        {
            var h = PropertyChanged;
            if (h != null)
                h(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class BuildStatusEntity : FrameworkElement
    {
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawEllipse(Brush, new Pen(Brushes.Gray, 1), new Point(13, 10), 10, 10);
            drawingContext.DrawText(NumText, new Point(9, 3));
        }

        public Brush Brush;
        public FormattedText NumText;

        private static readonly DependencyProperty StatusProp =
            DependencyProperty.Register("Status", typeof(Status), typeof(BuildStatusEntity),
            new FrameworkPropertyMetadata(default(Status), FrameworkPropertyMetadataOptions.AffectsRender, StatusValueChanged));

        public Status Status
        {
            get { return (Status)base.GetValue(StatusProp); }
            set { base.SetValue(StatusProp, value); }
        }

        private static void StatusValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var s = (Status)eventArgs.NewValue;
            var e = dependencyObject as BuildStatusEntity;

            e.Brush = GetBrush(s);
            e.NumText = FormatText(s.queuePos);
        }

        private static Brush GetBrush(Status s)
        {
            switch (s.status)
            {
                case BuildStatus.Building:
                    return Brushes.Yellow;
                case BuildStatus.Ready:
                    return Brushes.LawnGreen;
                case BuildStatus.Failed:
                    return Brushes.Red;
            }

            return Brushes.Transparent;
        }

        private static FormattedText FormatText(string text)
        {
            return new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Courier New"),
                14,
                Brushes.Black);
        }
    }
}
