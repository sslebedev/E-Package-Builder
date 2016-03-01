using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using EGMPackageBuilder.Properties;

namespace EGMPackageBuilder
{
    public partial class MainWindow : Form, BuilderFunctions.ILogger
    {
        private class ExternalPaths
        {
            public string GameCheckPath;
            public string UnityPath;
        };

        private readonly ExternalPaths externalPaths = new ExternalPaths();

        // Main window
        #region Main window

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Deseialize();
            SetWaiting(false);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Serialize();
        }

        private void SetWaiting(bool state)
        {
            buttonBrowseProjectFolder.Enabled = !state;
            buttonBrowseSourcesFolder.Enabled = !state;
            buttonBrowseBuildsFolder.Enabled = !state;
            buttonBrowseZipFolder.Enabled = !state;

            buttonMakeSources.Enabled = !state;
            buttonIncrVersion.Enabled = !state;
            buttonRenewVersion.Enabled = !state;
            buttonShowVersion.Enabled = !state;
            buttonGameCheck.Enabled = !state;
            buttonBuildPc.Enabled = !state;
            buttonBuildEgm.Enabled = !state;
            buttonDoAll.Enabled = !state;
            buttonZip.Enabled = !state;

            buttonLogClear.Enabled = !state;
            buttonSeparator.Enabled = !state;

            tbPathProject.Enabled = !state;
            tbPathSources.Enabled = !state;
            tbPathBuilds.Enabled = !state;
            tbPathZip.Enabled = !state;

            tbProjectDescription.Enabled = !state;

            fileToolStripMenuItem.Enabled = !state;
            toolsToolStripMenuItem.Enabled = !state;

            buttonCancel.Enabled = state;
        }

        #endregion

        // Context mangement
        #region Context management

        private void Serialize(string fileName = null)
        {
            try {
                bool isAppDataFile = fileName == null;
                if (isAppDataFile) {
                    fileName = Path.Combine(Application.LocalUserAppDataPath, "fields.epbcontext");
                }

                var file = new StreamWriter(fileName);
                file.WriteLine(tbProjectDescription.Text);
                file.WriteLine(tbPathProject.Text);
                file.WriteLine(tbPathSources.Text);
                file.WriteLine(tbPathBuilds.Text);
                file.WriteLine(tbPathZip.Text);
                file.WriteLine(externalPaths.GameCheckPath);
                file.WriteLine(externalPaths.UnityPath);
                file.Close();

                if (!isAppDataFile) {
                    WriteLogLine("Context saved: " + fileName);
                }
            } catch { }
        }

        private void Deseialize(string fileName = null)
        {
            try {
                bool isAppDataFile = fileName == null;
                if (isAppDataFile) {
                    fileName = Path.Combine(Application.LocalUserAppDataPath, "fields.epbcontext");
                }

                var file = new StreamReader(fileName);

                tbProjectDescription.Text = file.ReadLine();
                tbPathProject.Text = file.ReadLine();
                tbPathSources.Text = file.ReadLine();
                tbPathBuilds.Text = file.ReadLine();
                tbPathZip.Text = file.ReadLine();
                externalPaths.GameCheckPath = file.ReadLine();
                if (externalPaths.GameCheckPath != null && externalPaths.GameCheckPath.Length == 0) {
                    WriteLogLine("WARNING! GameCheck path is not set in current context!");
                }
                externalPaths.UnityPath = file.ReadLine();
                if (externalPaths.UnityPath != null && externalPaths.UnityPath.Length == 0) {
                    WriteLogLine("WARNING! IGT Unity path is not set in current context!");
                }
                file.Close();

                if (!isAppDataFile) {
                    WriteLogLine("Context loaded: " + fileName);
                }
            } catch (Exception exception) {
                WriteLogLine("FAIL to load context: " + exception.Message);
            }
        }

        private BuilderFunctions.Context AccuireContext()
        {
            return new BuilderFunctions.Context(tbPathSources.Text,
                                                tbPathProject.Text,
                                                tbPathBuilds.Text,
                                                tbProjectDescription.Text,
                                                cbReqOpenExplorer.Checked,
                                                externalPaths.GameCheckPath);
        }

        #endregion

        // Menu
        #region Menu

        private void loadContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialogContext.InitialDirectory = tbPathProject.Text;
            openFileDialogContext.Filter = Resources.ContextFilesFilter;
            openFileDialogContext.FilterIndex = 1;
            if (openFileDialogContext.ShowDialog() == DialogResult.OK) {
                var fileName = openFileDialogContext.FileName;
                Deseialize(fileName);
            }
        }

        private void saveContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialogContext.InitialDirectory = tbPathProject.Text;
            saveFileDialogContext.Filter = Resources.ContextFilesFilter;
            saveFileDialogContext.FilterIndex = 1;
            if (saveFileDialogContext.ShowDialog() == DialogResult.OK) {
                var fileName = saveFileDialogContext.FileName;
                Serialize(fileName);
            }
        }

        private string BrowseGameCheck()
        {
            const string gamecheckToolName = "GameCheck|IGT.GameCheck.CLI.exe";

            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".exe";
            dialog.Filter = gamecheckToolName;
            dialog.Title = "Select GameCheckTool executable";
            dialog.ShowDialog();

            return dialog.FileName;
        }

        private void gameCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            externalPaths.GameCheckPath = BrowseGameCheck();
            Serialize();
        }

        private string BrowseUnity()
        {
            const string gamecheckToolName = "Unity|Unity.exe";

            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".exe";
            dialog.Filter = gamecheckToolName;
            dialog.Title = "Select IGT Unity executable";
            dialog.ShowDialog();

            return dialog.FileName;
        }

        private void unityPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            externalPaths.UnityPath = BrowseUnity();
            Serialize();
        }

        #endregion

        // Path selection
        #region Path selection

        private void buttonBrowseProjectFolder_Cick(object sender, EventArgs e)
        {
            ChooseFolder(folderBrowserProject, tbPathProject);
            Serialize();
        }

        private void buttonBrowseSourcesFolder_Click(object sender, EventArgs e)
        {
            ChooseFolder(folderBrowserSources, tbPathSources);
            Serialize();
        }

        private void buttonBrowseBuildsFolder_Click(object sender, EventArgs e)
        {
            ChooseFolder(folderBrowserBuilds, tbPathBuilds);
            Serialize();
        }

        private void buttonBrowseZipFolder_Click(object sender, EventArgs e)
        {
            ChooseFolder(folderBrowserZip, tbPathZip);
            Serialize();
        }

        private void ChooseFolder(FolderBrowserDialog folderBrowserDialog, TextBox textField)
        {
            try {
                var dir = new DirectoryInfo(textField.Text);
                if (dir.Exists) {
                    folderBrowserDialog.SelectedPath = textField.Text;
                }
            } catch { }

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                var selectedPath = folderBrowserDialog.SelectedPath;
                textField.Text = selectedPath;
            }
        }

        #endregion

        // Log
        #region Log

        private void buttonLogClear_Click(object sender, EventArgs e)
        {
            textLog.Clear();
        }

        private void buttonSeparator_Click(object sender, EventArgs e)
        {
            WriteLogLine("---------------------------------------------------------------------");
            WriteLogLine();
        }

        public void WriteLogLine(String txt = "")
        {
            if (InvokeRequired) {
                Invoke(new Action<string>(WriteLogLine), new object[] { txt });
                return;
            }
            textLog.AppendText(txt + Environment.NewLine);
        }

        #endregion

        // Commands
        #region Commands

        private void buttonMakeSources_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => BuilderFunctions.MakeSources(AccuireContext(), this));
        }

        private void buttonIncrVersion_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => BuilderFunctions.MakeGameVersion(tbPathProject.Text, this, true));
        }

        private void buttonRenewVersion_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => BuilderFunctions.MakeGameVersion(tbPathProject.Text, this, false));
        }

        private void buttonShowVersion_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => BuilderFunctions.WriteGameVersion(tbPathProject.Text, this));
        }

        private void buttonGameCheck_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => {
                WriteLogLine("Performing game check...");
                WriteLogLine(BuilderFunctions.ProcessGameCheck(externalPaths.GameCheckPath, tbPathSources.Text));
                WriteLogLine();
            });
        }

        private void buttonBuildPc_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => BuilderFunctions.ProcessBuild(externalPaths.UnityPath, BuilderFunctions.BuildTypes.Standalone, tbPathProject.Text, Path.Combine(tbPathBuilds.Text, "Standalone"), this));
        }

        private void buttonBuildEgm_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => BuilderFunctions.ProcessBuild(externalPaths.UnityPath, BuilderFunctions.BuildTypes.Release, tbPathProject.Text, Path.Combine(tbPathBuilds.Text, "Release"), this));
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (currentActivity != null && currentActivity.IsAlive) {
                currentActivity.Abort();
            }
        }

        private void buttonDoAll_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => {
                var context = AccuireContext();
                context.NeedOpenExplorer = false;
                BuilderFunctions.MakeSources(context, this);
                BuilderFunctions.ProcessBuild(externalPaths.UnityPath, BuilderFunctions.BuildTypes.Standalone, context.PathProject,
                                              Path.Combine(context.PathBuilds, "Standalone"), this);
                BuilderFunctions.ProcessBuild(externalPaths.UnityPath, BuilderFunctions.BuildTypes.Release, context.PathProject,
                                              Path.Combine(context.PathBuilds, "Release"), this);

                WriteLogLine("All finished.");
            });
        }

        private void buttonZip_Click(object sender, EventArgs e)
        {
            StartBlockingActivity(() => {
                const string formatZip = "{0} {1} {2}.zip";
                var date = DateTime.Now.ToString("yyyy'_'MM'_'dd");

                var context = AccuireContext();

                if (!Directory.Exists(tbPathZip.Text)) {
                    Directory.CreateDirectory(tbPathZip.Text);
                }
                Helpers.ClearDirectory(tbPathZip.Text);

                var zipSources = String.Format(formatZip, context.Description, date, "sources");
                WriteLogLine("Making " + zipSources + "...");
                try {
                    Helpers.ZipDirectory(Path.Combine(tbPathZip.Text, zipSources), context.PathSources);
                } catch (Exception exception) {
                    WriteLogLine("Exception occured: " + exception.Message);
                }

                var zipBuilds = String.Format(formatZip, context.Description, date, "builds");
                WriteLogLine("Making " + zipBuilds + "...");
                try {
                    Helpers.ZipDirectory(Path.Combine(tbPathZip.Text, zipBuilds), context.PathBuilds);
                } catch (Exception exception) {
                    WriteLogLine("Exception occured: " + exception.Message);
                }

                WriteLogLine("Finished.");
            });
        }

        private Thread currentActivity;

        private void StartBlockingActivity(Action action)
        {
            ParameterizedThreadStart processBlockingActivity = activity => {
                try {
                    Invoke(new Action(() => SetWaiting(true)));

                    try {
                        Debug.Assert(activity != null && activity as Action != null, "activity != null");
                        (activity as Action)();
                    } catch (ThreadAbortException) {
                        throw; // is raised automatically after catch, rethrown for explicity
                    } catch (Exception exception) {
                        WriteLogLine("Exception occured: " + exception.Message);
                    } finally {
                        Invoke(new Action(() => SetWaiting(false)));
                    }
                } catch (ThreadAbortException) {
                    WriteLogLine("User canceled.");
                }
            };

            currentActivity = new Thread(processBlockingActivity) { IsBackground = true };
            currentActivity.Start(action);
        }

        #endregion
    }
}