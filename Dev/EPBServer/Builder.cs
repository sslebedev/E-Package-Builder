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
            BuildPCFullscreen,
            BuildDevelopment
        };

        private struct BuildRequest
        {
            public int OwnerUID;
            public string ProjectName;
            public ILogger Client;
            public BuildType type;
        };

        private readonly Queue<BuildRequest> _buildQueue = new Queue<BuildRequest>();

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

        public void AddBuildRequest(int clientUID, ISender client, string projectName)
        {

        }
    }
}
