using EPackageBuilder;
using System;
using System.Collections.Generic;
using System.IO;

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

        private readonly List<BuildRequest> buildQueue = new List<BuildRequest>();

        private readonly object builderLock = new object();

        private readonly List<string> projects = new List<string>();
        public List<string> Projects
        {
            get { return projects; }
        }

        private string configDir;

        public Builder()
        {
        }

        public bool Init()
        {
            configDir = Environment.CurrentDirectory + @"\..\..\Configs";
            DirectoryInfo dir = new DirectoryInfo(configDir);

            if (!dir.Exists)
                return false;

            FileInfo[] fileInfos = dir.GetFiles("*.cfg", SearchOption.TopDirectoryOnly);
            foreach (FileInfo fi in fileInfos) {
                string projectName = fi.Name.Remove(fi.Name.Length - 4);
                projects.Add(projectName);
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
            lock (builderLock) {
                buildQueue.Add(request);
            }
        }

        private bool DequeueRequest(out BuildRequest request)
        {
            request = default(BuildRequest);

            lock (builderLock) {
                if (buildQueue.Count == 0)
                    return false;

                request = buildQueue[0];
                buildQueue.RemoveAt(0);
                return true;
            }
        }

        public void Start()
        {
            while (true) {
                BuildRequest r;
                while (true) {
                    bool requestExists = DequeueRequest(out r);
                    if (!requestExists) {
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }
                    if (r.ProjectName == _checkoutConfig) {
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
            var ctx = new BuilderFunctions.Context(
                String.Format("{0}\\{1}.cfg",
                configDir,
                r.ProjectName));

            switch (r.Type) {
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
            return File.ReadAllText(configDir + "/" + name);
        }

        public void CheckinConfig(string file)
        {
            File.WriteAllText(_checkoutConfig, file);
            _checkoutConfig = "";
        }
    }
}
