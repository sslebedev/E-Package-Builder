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

            public class Info
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
                            sb.Append(Requested.ToString());
                            sb.Append('\n');
                            if (Requested)
                            {
                                sb.Append(QueuePos.ToString());
                                sb.Append('\n');
                            }
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

        private BuildRequest Take()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (_storage.Count > 0)
                    {
                        BuildRequest request = _storage.First.Value;

                        // update position
                        for (var cur = _storage.First; cur != null; cur = cur.Next)
                            GetInfo(cur.Value).QueuePos -= 1;

                        Debug.Assert(GetInfo(request).QueuePos == 0);
                        _storage.RemoveFirst();
                        Debug.Assert(_storage.Count == 0 ||
                            GetInfo(_storage.First.Value).QueuePos == 1);

                        return request;
                    }
                }

                Thread.Sleep(100);
            }
        }

        private void Push(BuildRequest request)
        {
            lock (_lock)
            {
                GetInfo(request).QueuePos = _storage.Count + 1;
                _storage.AddLast(request);
            }
        }

        private BuildRequest Remove(Predicate<BuildRequest> trigger)
        {
            lock (_lock)
            {
                LinkedListNode<BuildRequest> deleted = null;
                for (var cur = _storage.First; cur != null; cur = cur.Next)
                {
                    if (trigger.Invoke(cur.Value))
                    {
                        deleted = cur;
                        break;
                    }
                }

                // project build already started
                if (deleted == null)
                    return null;

                for (var cur = deleted.Next; cur != null; cur = cur.Next)
                    GetInfo(cur.Value).QueuePos -= 1;

                _storage.Remove(deleted);
                return deleted.Value;
            }
        }

        private readonly LinkedList<BuildRequest> _storage = new LinkedList<BuildRequest>();
        private readonly Object _lock = new Object();

        private readonly BlockingCollection<BuildRequest> buildQueue
            = new BlockingCollection<BuildRequest>(); // ConcurentQueue by default

        private static readonly Dictionary<string, ProjectEntity> projects = new Dictionary<string, ProjectEntity>();
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

        private ProjectEntity.Info GetInfo(BuildRequest request)
        {
            return projects[request.ProjectName].BuildInfo[request.Type];
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

            lock (projectsLock)
            {
                // project build already request
                if (GetInfo(request).State == ProjectState.Build || GetInfo(request).Requested)
                    return;

                GetInfo(request).Requested = true;
                Push(request);
            }

            var h = OnProjectsStateUpdate;
            if (h != null)
                h();
        }

        public void CancelBuildRequest(int clientUID, string projectName)
        {
            lock (projectsLock)
            {
                bool update = false;
                while (true)
                {
                    BuildRequest canceled = Remove(request => request.OwnerUID == clientUID && request.ProjectName == projectName);
                    if (canceled == null)
                    {
                        if (update)
                            break;
                        else
                            return;
                    }

                    GetInfo(canceled).State = ProjectState.NotBuild;
                    GetInfo(canceled).LastError = "build was canceled";
                    GetInfo(canceled).Requested = false;
                    update = true;
                }
            }

            var h = OnProjectsStateUpdate;
            if (h != null)
                h();
        }

        public void Start()
        {
            while (true) {
                BuildRequest request = Take();

                Debug.Assert(request != null);
                ProcessRequest(request);

                Thread.Sleep(200);
            }
        }

        private void ProcessRequest(BuildRequest request)
        {
            if (checkoutedProj != null && request.ProjectName == checkoutedProj)
            {
                Thread.Sleep(1000);

                lock (projectsLock)
                    Push(request);

                var h = OnProjectsStateUpdate;
                if (h != null)
                    h();

                return;
            }


            OnBuildStart(request);
            bool success = false;
            string errorMsg = null;
            try
            {
                //Build(request);
                Thread.Sleep(3000); // HACK : testing
                success = true;
            }
            catch (BuilderFunctions.FunctionFailedExeption e)
            {
                errorMsg = e.Message;
            }
            finally
            {
                OnBuildEnd(request, success, errorMsg);
            }
        }

        private void OnBuildStart(BuildRequest request)
        {
            lock (projectsLock)
            {
                Debug.Assert(GetInfo(request).Requested == true);
                Debug.Assert(GetInfo(request).QueuePos == 0);

                GetInfo(request).State = ProjectState.Build;
                GetInfo(request).Version = request.Version;
            }

            var h = OnProjectsStateUpdate;
            if (h != null)
                h();
        }

        private void OnBuildEnd(BuildRequest request, bool success, string errorMsg)
        {
            lock (projectsLock)
            {
                var info = GetInfo(request);
                Debug.Assert(info.Requested);
                Debug.Assert(info.State == ProjectState.Build);

                info.Requested = false;

                if (success)
                    info.State = ProjectState.Ready;
                else
                {
                    info.State = ProjectState.NotBuild;


                    if (errorMsg != null)
                        info.LastError = errorMsg;
                    else
                        info.LastError = "unknown error";
                }
            }

            var h = OnProjectsStateUpdate;
            if (h != null)
                h();
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
        private string checkoutedProj;

        public string CheckoutConfig(string name)
        {
            checkoutedProj = name;
            checkoutedConfig = configDir + "/" + name + ".cfg";
            return File.ReadAllText(checkoutedConfig);
        }

        public void CheckinConfig(string file)
        {
            File.WriteAllText(checkoutedConfig, file);
            checkoutedConfig = null;
            checkoutedProj = null;
        }

        public void CheckinCancel()
        {
            checkoutedConfig = null;
            checkoutedProj = null;
        }
    }
}
