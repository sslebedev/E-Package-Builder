using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using EPackageBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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
            public BuilderFunctions.ILogger Logger;
            public BuildType Type;
        };

        private readonly BlockingCollection<BuildRequest> buildQueue
            = new BlockingCollection<BuildRequest>(); // ConcurentQueue by default


        private readonly List<string> projects = new List<string>();
        public List<string> Projects
        {
            get { return projects; }
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
                projects.Add(projectName);
            }
        }

        public void AddBuildRequest(int clientUID, BuilderFunctions.ILogger logger, string projectName, BuildType type)
        {
            var request = new BuildRequest {
                OwnerUID = clientUID,
                ProjectName = projectName,
                Logger = logger,
                Type = type
            };

            buildQueue.Add(request);
        }

        public void Start()
        {
            while (!buildQueue.IsCompleted) {
                BuildRequest request = null;
                try {
                    request = buildQueue.Take(); // Blocks
                } catch (InvalidOperationException) {
                    Debug.Assert(false, "Unreachible code");
                }

                if (request != null) {
                    ProcessRequest(request);
                }
            }
        }

        private void ProcessRequest(BuildRequest request)
        {
            if (checkoutedConfig != null && request.ProjectName == checkoutedConfig) {
                Task.Delay(10000).ContinueWith(_ => buildQueue.Add(request));
                return;
            }

            Build(request);
        }

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
