using EPackageBuilder;
using EPBMessanger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

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

        private struct BuildRequest
        {
            public int OwnerUID;
            public string ProjectName;
            public BuilderFunctions.ILogger Logger;
            public BuildType Type;
        };

        private readonly List<BuildRequest> _buildQueue = new List<BuildRequest>();

        private readonly object _builderLock = new object();

        private readonly List<string> _projects = new List<string>();
        public List<string> Projects
        {
            get { return _projects; }
        }

        private string _configDir;

        public Builder()
        {
        }

        public bool Init()
        {
            _configDir = Environment.CurrentDirectory + @"\..\..\Configs";
            DirectoryInfo dir = new DirectoryInfo(_configDir);

            if (!dir.Exists)
                return false;

            FileInfo[] fileInfos = dir.GetFiles("*.cfg", SearchOption.TopDirectoryOnly);
            foreach (FileInfo fi in fileInfos)
            {
                string projectName = fi.Name.Remove(fi.Name.Length - 4);
                _projects.Add(projectName);
            }

            return true;
        }

        public void AddBuildRequest(int clientUID, BuilderFunctions.ILogger logger, string projectName, BuildType type)
        {
            BuildRequest request = new BuildRequest() {
                OwnerUID = clientUID,
                ProjectName = projectName,
                Logger = logger,
                Type = type
            };

            EnqueueRequest(request);
        }

        private void EnqueueRequest(BuildRequest request)
        {
            lock (_builderLock) {
                _buildQueue.Add(request);
            }
        }

        private bool DequeueRequest(out BuildRequest request)
        {
            request = default(BuildRequest);

            lock (_builderLock) {
                if (_buildQueue.Count == 0)
                    return false;

                request = _buildQueue[0];
                _buildQueue.RemoveAt(0);
                return true;
            }
        }

        public void Start()
        {
            while (true)
            {
                BuildRequest r;
                while (true)
                {
                    bool requestExists = DequeueRequest(out r);
                    if (!requestExists)
                    {
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }
                    if (r.ProjectName == _checkoutConfig)
                    {
                        EnqueueRequest(r);
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }
                    break;
                }

                Build(r);
            }
        }

        private void Build(BuildRequest r)
        {
            BuilderFunctions.Context ctx =
                new BuilderFunctions.Context(String.Format("{0}\\{1}.cfg", _configDir, r.ProjectName));

            switch (r.Type)
            {
                case BuildType.MakeSources:
                    BuilderFunctions.MakeSources(ctx, r.Logger);
                    break;
                case BuildType.BuildPC:
                    BuilderFunctions.ProcessBuild(ctx.UnityPath, BuilderFunctions.BuildTypes.Standalone, ctx.PathSources, ctx.PathBuilds, r.Logger);
                    break;
                case BuildType.BuildRelease:
                    BuilderFunctions.ProcessBuild(ctx.UnityPath, BuilderFunctions.BuildTypes.Release, ctx.PathSources, ctx.PathBuilds, r.Logger);
                    break;
                case BuildType.BuildFull:
                    BuilderFunctions.MakeSources(ctx, r.Logger);
                    BuilderFunctions.ProcessBuild(ctx.UnityPath, BuilderFunctions.BuildTypes.Standalone, ctx.PathSources, ctx.PathBuilds, r.Logger);
                    BuilderFunctions.ProcessBuild(ctx.UnityPath, BuilderFunctions.BuildTypes.Release, ctx.PathSources, ctx.PathBuilds, r.Logger);
                    break;
                case BuildType.BuildOther:
                    break;
            }
        }

        private string _checkoutConfig = "";

        public string CheckoutConfig(string name)
        {
            _checkoutConfig = name;
            return File.ReadAllText(_configDir + "/" + name);
        }

        public void CheckinConfig(string file)
        {
            File.WriteAllText(_checkoutConfig, file);
            _checkoutConfig = "";
        }
    }
}
