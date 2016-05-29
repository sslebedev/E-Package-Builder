using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using EPackageBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;

namespace EPBServer
{
    class Builder
    {
        public enum BuildType
        {
            MakeSources,
            BuildPC,
            BuildRelease,
            BuildFull,
            BuildOther
        };

        private class BuildRequest
        {
            public int OwnerUID; // TODO resharper warns it is unused, is it right?
            public string ProjectName;
            public string Version;
            public BuilderFunctions.ILogger Logger;
            public BuildType Type;
        };

        private enum ProjectState
        {
            NotBuild,
            Build,
            Ready,
        };

        private class ProjectEntity
        {
            public readonly string      Name;

            public struct Info
            {
                public static Info CreateDefault()
                {
                    var info = new Info()
                    {
                        State       = ProjectState.NotBuild,
                        Version     = "",
                        LastError   = "",

                        Requested   = false,
                        QueuePos    = -1
                    };

                    return info;
                }

                public override string ToString()
                {
                    StringBuilder sb = new StringBuilder();

                    switch (State)
                    {
                        case ProjectState.NotBuild:
                            sb.Append("NotBuild\n");
                            sb.Append(Requested.ToString());
                            sb.Append('\n');
                            if (Requested)
                            {
                                sb.Append(QueuePos.ToString());
                                sb.Append('\n');
                            }
                            sb.Append(LastError);
                            if (LastError.Length != 0)
                            {
                                sb.Append('\n');
                                sb.Append(Version);
                            }
                            break;
                        case ProjectState.Build:
                            sb.Append("Build\n");
                            sb.Append(Version);
                            break;
                        case ProjectState.Ready:
                            sb.Append("Ready\n");
                            sb.Append(Version);
                            break;
                    }

                    return sb.ToString();
                }

                public ProjectState     State;
                public string           Version;
                public string           LastError;

                public bool             Requested;
                public int              QueuePos;
            }

            public readonly Dictionary<BuildType, Info> BuildInfo = new Dictionary<BuildType, Info>();

            public ProjectEntity(string projectName)
            {
                Name = projectName;

                BuildInfo.Add(BuildType.MakeSources,    Info.CreateDefault());
                BuildInfo.Add(BuildType.BuildPC,        Info.CreateDefault());
                BuildInfo.Add(BuildType.BuildRelease,   Info.CreateDefault());
                BuildInfo.Add(BuildType.BuildFull,      Info.CreateDefault());
                BuildInfo.Add(BuildType.BuildOther,     Info.CreateDefault());
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Name);

                foreach (var p in BuildInfo)
                {
                    sb.Append('\n');
                    sb.Append(Enum.GetName(p.Key.GetType(), p.Key));
                    sb.Append('\n');
                    sb.Append(p.Value.ToString());
                }

                return sb.ToString();
            }
        }

        private readonly BlockingCollection<BuildRequest> buildQueue
            = new BlockingCollection<BuildRequest>(); // ConcurentQueue by default
        private int buildQueuePosition = 0;

        private readonly Dictionary<string, ProjectEntity> projects = new Dictionary<string, ProjectEntity>();
        private readonly Object projectsLock = new Object();
        public string[] Projects
        {
            get
            {
                var keys = new string[projects.Keys.Count];
                projects.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        private string configDir;

        public void Init()
        {
            configDir = Environment.CurrentDirectory + @"\..\..\Configs"; // TODO WTF, refactor this. use working dir maybe, as I've done, see cda5db861dcc81dda2b37fbcd138d2dc699f4c11
            var dir = new DirectoryInfo(configDir);

            if (!dir.Exists) {
                throw new FileNotFoundException(configDir);
            }

            var fileInfos = dir.GetFiles("*.cfg", SearchOption.TopDirectoryOnly);
            foreach (var fi in fileInfos) {
                // get file name without extension
                var projectName = fi.Name.Remove(fi.Name.Length - 4); // TODO refactor this. http://lmgtfy.com/?q=c%23+get+filename+with+extension
                projects.Add(projectName, new ProjectEntity(projectName));
            }
        }

        public string GenerateProjectsInfo()
        {
            lock (projectsLock)
            {
                StringBuilder sb = new StringBuilder();

                bool r = false;
                foreach (var p in projects.Values)
                {
                    if (r)
                        sb.Append('\n');
                    r = true;
                    sb.Append(p.ToString());
                }

                return sb.ToString();
            }
        }

        public void AddBuildRequest(int clientUID, BuilderFunctions.ILogger logger, string projectName, string version, BuildType type)
        {
            var request = new BuildRequest
            {
                OwnerUID = clientUID,
                ProjectName = projectName,
                Logger = logger,
                Type = type,
                Version = version
            };

            OnBuildAdd(request.ProjectName, request.Type);
            buildQueue.Add(request);
        }

        public void Start()
        {
            while (!buildQueue.IsCompleted) {
                BuildRequest request = null;
                try {
                    request = buildQueue.Take(); // Blocks
                } catch (InvalidOperationException) {
                    Debug.Assert(false, "Unreachable code");
                }

                if (request != null) {
                    ProcessRequest(request);
                }
            }
        }

        private void ProcessRequest(BuildRequest request)
        {
            if (checkoutedConfig != null && request.ProjectName == checkoutedConfig)
            {
                Task.Delay(10000).ContinueWith(_ =>
                {
                    OnBuildAdd(request.ProjectName, request.Type);
                    buildQueue.Add(request);
                });
                return;
            }


            OnBuildStart(request.ProjectName, request.Type, request.Version);
            bool success = false;
            string errorMsg = null;
            try
            {
                //Build(request);
                Thread.Sleep(7000); // HACK : testing
                success = true;
            }
            catch (BuilderFunctions.FunctionFailedExeption e)
            {
                errorMsg = e.Message;
            }
            finally
            {
                OnBuildEnd(request.ProjectName, request.Type, success, errorMsg);
            }
        }

        private void OnBuildStart(string projName, BuildType type, string version)
        {
            lock (projectsLock)
            {
                var r = projects[projName].BuildInfo[type];
                Debug.Assert(r.Requested == true);
                Debug.Assert(r.QueuePos == 0);

                r.State = ProjectState.Build;
                r.Version = version;
                projects[projName].BuildInfo[type] = r;

                var h = OnProjectsStateUpdate;
                if (h != null)
                    h();
            }
        }

        private void OnBuildEnd(string projName, BuildType type, bool success, string errorMsg)
        {
            lock (projectsLock)
            {
                buildQueuePosition--;
                var r = projects[projName].BuildInfo[type];
                Debug.Assert(r.State == ProjectState.Build);
                Debug.Assert(r.Requested == true);

                r.Requested = false;
                if (success)
                    r.State = ProjectState.Ready;
                else
                {
                    r.State = ProjectState.NotBuild;


                    if (errorMsg != null)
                        r.LastError = errorMsg;
                    else
                        r.LastError = "unknown error";
                }
                projects[projName].BuildInfo[type] = r;

                foreach (var p in buildQueue)
                {
                    var r_ = projects[p.ProjectName].BuildInfo[p.Type];
                    r_.QueuePos -= 1;

                    projects[p.ProjectName].BuildInfo[p.Type] = r_;
                }

                var h = OnProjectsStateUpdate;
                if (h != null)
                    h();
            }
        }

        private void OnBuildAdd(string projName, BuildType type)
        {
            lock (projectsLock)
            {
                var r = projects[projName].BuildInfo[type];
                if (r.State == ProjectState.Build || r.Requested == true)
                    return;

                r.QueuePos = buildQueuePosition++;
                r.Requested = true;
                projects[projName].BuildInfo[type] = r;

                var h = OnProjectsStateUpdate;
                if (h != null)
                    h();
            }
        }

        public event Action OnProjectsStateUpdate;

        private class Config
        {
            public string PathProject { get; private set; }
            public string PathSources { get; private set; }
            public string PathBuilds { get; private set; }
            public string Description { get; private set; }
            public string PathTemplates { get; private set; } // TODO add this path into your config. now it is placed in project root
            public string GameCheckToolPath { get; private set; }
            public string UnityPath { get; private set; }

            public static Config Parse(string cfgFileName)
            {
                var cfgFile = new StreamReader(cfgFileName);

                var instance = new Config {
                    PathProject = cfgFile.ReadLine(),
                    PathSources = cfgFile.ReadLine(),
                    PathBuilds = cfgFile.ReadLine(),
                    Description = cfgFile.ReadLine(),
                    PathTemplates = cfgFile.ReadLine(),
                    GameCheckToolPath = cfgFile.ReadLine(),
                    UnityPath = cfgFile.ReadLine()
                };

                return instance;
            }
        }

        private void Build(BuildRequest r)
        {
            var config = Config.Parse(
                String.Format("{0}\\{1}.cfg",
                configDir,
                r.ProjectName));

            Action makeSources =
                () => BuilderFunctions.MakeSources(config.PathTemplates,
                                                   config.PathProject,
                                                   config.PathSources,
                                                   config.Description,
                                                   config.GameCheckToolPath,
                                                   r.Logger);
            Action<string> processBuild =
                buildType => BuilderFunctions.ProcessBuild(config.UnityPath,
                                                           buildType,
                                                           config.PathSources,
                                                           config.PathBuilds,
                                                           r.Logger);
            switch (r.Type) {
                case BuildType.MakeSources:
                    makeSources();
                    break;
                case BuildType.BuildPC:
                    processBuild(BuilderFunctions.BuildTypes.Standalone);
                    break;
                case BuildType.BuildRelease:
                    processBuild(BuilderFunctions.BuildTypes.Release);
                    break;
                case BuildType.BuildFull:
                    makeSources();
                    processBuild(BuilderFunctions.BuildTypes.Standalone);
                    processBuild(BuilderFunctions.BuildTypes.Release);
                    break;
                case BuildType.BuildOther:
                    break;
            }
        }

        private string checkoutedConfig;

        public string CheckoutConfig(string name)
        {
            checkoutedConfig = configDir + "/" + name + ".cfg";
            return File.ReadAllText(checkoutedConfig);
        }

        public void CheckinConfig(string file)
        {
            File.WriteAllText(checkoutedConfig, file);
            checkoutedConfig = null;
        }
    }
}
