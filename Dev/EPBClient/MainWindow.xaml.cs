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

            if (msgName == "ProjectsInfo")
            {
                string s = msg.Read();
                UpdateProjects(s.Split(new char[] { '\n' }));
            }
            else if (msgName == "BuildLog")
            {
                Logger.WriteLog(msg.Read());
            }
        }

        private void OnServerConnected()
        {
            //var msg = ClientStorage.Client.NewMessage();
            //msg.WriteName("GetProjects");
            //msg.Write("");
            //msg.Send();
        }

        private void OnServerDisconnected()
        {
            Logger.WriteLog("Server disconnected!");
            UpdateProjects(new string[0]);
        }

        private void MainWndAction(Action a)
        {
            Dispatcher.Invoke(a, null);
        }

        private void UpdateProjects(string[] projects)
        {
            MainWndAction(() =>
            {
                _projects.Clear();

                for (int i = 0; i < projects.Length;)
                {
                    string projName = projects[i++];
                    var projEntity = new ProjectsEntity(projName);
                    _projects.Add(projEntity);

                    for (int j = 0; j < 5; ++j)
                    {
                        switch (projects[i++])
                        {
                            case "MakeSources":
                                {
                                    switch (projects[i++])
                                    {
                                        case "NotBuild":
                                            if (projects[i++] == "True")
                                                projEntity.SrcStatus = new Status(BuildStatus.Failed, projects[i++]);
                                            else
                                                projEntity.SrcStatus = new Status(BuildStatus.Failed, "");
                                            string lastError = projects[i++];
                                            if (lastError.Length != 0)
                                            {
                                                string version = projects[i++];
                                                projEntity.SrcToolTip = String.Format("#version {0}\nlast error: {1}", version, lastError);
                                            }
                                            else
                                                projEntity.SrcToolTip = "not built";
                                            break;
                                        case "Build":
                                            projEntity.SrcStatus = new Status(BuildStatus.Building, "0");
                                            projEntity.SrcToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                        case "Ready":
                                            projEntity.SrcStatus = new Status(BuildStatus.Ready, "");
                                            projEntity.SrcToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                    }
                                }
                                break;
                            case "BuildPC":
                                {
                                    switch (projects[i++])
                                    {
                                        case "NotBuild":
                                            if (projects[i++] == "True")
                                                projEntity.PCStatus = new Status(BuildStatus.Failed, projects[i++]);
                                            else
                                                projEntity.PCStatus = new Status(BuildStatus.Failed, "");
                                            string lastError = projects[i++];
                                            if (lastError.Length != 0)
                                            {
                                                string version = projects[i++];
                                                projEntity.PCToolTip = String.Format("#version {0}\nlast error: {1}", version, lastError);
                                            }
                                            else
                                                projEntity.PCToolTip = "not built";
                                            break;
                                        case "Build":
                                            projEntity.PCStatus = new Status(BuildStatus.Building, "0");
                                            projEntity.PCToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                        case "Ready":
                                            projEntity.PCStatus = new Status(BuildStatus.Ready, "");
                                            projEntity.PCToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                    }
                                }
                                break;
                            case "BuildRelease":
                                {
                                    switch (projects[i++])
                                    {
                                        case "NotBuild":
                                            if (projects[i++] == "True")
                                                projEntity.ReleaseStatus = new Status(BuildStatus.Failed, projects[i++]);
                                            else
                                                projEntity.ReleaseStatus = new Status(BuildStatus.Failed, "");
                                            string lastError = projects[i++];
                                            if (lastError.Length != 0)
                                            {
                                                string version = projects[i++];
                                                projEntity.ReleaseToolTip = String.Format("#version {0}\nlast error: {1}", version, lastError);
                                            }
                                            else
                                                projEntity.ReleaseToolTip = "not built";
                                            break;
                                        case "Build":
                                            projEntity.ReleaseStatus = new Status(BuildStatus.Building, "0");
                                            projEntity.ReleaseToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                        case "Ready":
                                            projEntity.ReleaseStatus = new Status(BuildStatus.Ready, "");
                                            projEntity.ReleaseToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                    }
                                }
                                break;
                            case "BuildFull":
                                {
                                    switch (projects[i++])
                                    {
                                        case "NotBuild":
                                            if (projects[i++] == "True")
                                                projEntity.FullStatus = new Status(BuildStatus.Failed, projects[i++]);
                                            else
                                                projEntity.FullStatus = new Status(BuildStatus.Failed, "");
                                            string lastError = projects[i++];
                                            if (lastError.Length != 0)
                                            {
                                                string version = projects[i++];
                                                projEntity.FullToolTip = String.Format("#version {0}\nlast error: {1}", version, lastError);
                                            }
                                            else
                                                projEntity.FullToolTip = "not built";
                                            break;
                                        case "Build":
                                            projEntity.FullStatus = new Status(BuildStatus.Building, "0");
                                            projEntity.FullToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                        case "Ready":
                                            projEntity.FullStatus = new Status(BuildStatus.Ready, "");
                                            projEntity.FullToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                    }
                                }
                                break;
                            case "BuildOther":
                                {
                                    switch (projects[i++])
                                    {
                                        case "NotBuild":
                                            if (projects[i++] == "True")
                                                projEntity.OtherStatus = new Status(BuildStatus.Failed, projects[i++]);
                                            else
                                                projEntity.OtherStatus = new Status(BuildStatus.Failed, "");
                                            string lastError = projects[i++];
                                            if (lastError.Length != 0)
                                            {
                                                string version = projects[i++];
                                                projEntity.OtherToolTip = String.Format("#version {0}\nlast error: {1}", version, lastError);
                                            }
                                            else
                                                projEntity.OtherToolTip = "not built";
                                            break;
                                        case "Build":
                                            projEntity.OtherStatus = new Status(BuildStatus.Building, "0");
                                            projEntity.OtherToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                        case "Ready":
                                            projEntity.OtherStatus = new Status(BuildStatus.Ready, "");
                                            projEntity.OtherToolTip = String.Format("#version {0}", projects[i++]);
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
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

        private string Version()
        {
            if (versionRadioButton.IsChecked.Value)
                return "Latest";
            return versionNum.Text;
        }

        private void ButtonSources_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjText.Content as string == "")
                return;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("RequestBuild");
            msg.Write(String.Format("{0}\n{1}\n{2}", "Sources", SelectedProjText.Content, Version()));
            msg.Send();
        }

        private void ButtonPC_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjText.Content as string == "")
                return;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("RequestBuild");
            msg.Write(String.Format("{0}\n{1}\n{2}", "PC", SelectedProjText.Content, Version()));
            msg.Send();
        }

        private void ButtonRelease_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjText.Content as string == "")
                return;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("RequestBuild");
            msg.Write(String.Format("{0}\n{1}\n{2}", "Release", SelectedProjText.Content, Version()));
            msg.Send();
        }

        private void ButtonFull_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProjText.Content as string == "")
                return;

            var msg = ClientStorage.Client.NewMessage();
            msg.WriteName("RequestBuild");
            msg.Write(String.Format("{0}\n{1}\n{2}", "Full", SelectedProjText.Content, Version()));
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

        private Status _pc;
        public Status PCStatus
        {
            get { return _pc; }
            set
            {
                _pc = value;
                RaisePropertyChanged("PCStatus");
            }
        }

        private string _pcToolTip;
        public string PCToolTip
        {
            get { return _pcToolTip; }
            set
            {
                _pcToolTip = value;
                RaisePropertyChanged("PCToolTip");
            }
        }

        private Status _release;
        public Status ReleaseStatus
        {
            get { return _release; }
            set
            {
                _release = value;
                RaisePropertyChanged("ReleaseStatus");
            }
        }

        private string _releaseToolTip;
        public string ReleaseToolTip
        {
            get { return _releaseToolTip; }
            set
            {
                _releaseToolTip = value;
                RaisePropertyChanged("ReleaseToolTip");
            }
        }

        private Status _full;
        public Status FullStatus
        {
            get { return _full; }
            set
            {
                _full = value;
                RaisePropertyChanged("FullStatus");
            }
        }

        private string _fullToolTip;
        public string FullToolTip
        {
            get { return _fullToolTip; }
            set
            {
                _fullToolTip = value;
                RaisePropertyChanged("FullToolTip");
            }
        }

        private Status _other;
        public Status OtherStatus
        {
            get { return _other; }
            set
            {
                _other = value;
                RaisePropertyChanged("OtherStatus");
            }
        }

        private string _otherToolTip;
        public string OtherToolTip
        {
            get { return _otherToolTip; }
            set
            {
                _otherToolTip = value;
                RaisePropertyChanged("OtherToolTip");
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
