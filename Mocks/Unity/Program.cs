using System;
using System.Diagnostics;
using System.IO;

namespace Unity
{
    class Program
    {
        // Keys
        #region Keys

        struct KeyValues
        {
            public bool batchmode;
            public bool nographics;
            public bool quit;
            public string projectPath;
            public string executeMethod;
            public string logFile;

            public bool IsValid
            {
                get
                {
                    return batchmode && nographics && quit &&
                        !string.IsNullOrEmpty(projectPath) &&
                        !string.IsNullOrEmpty(executeMethod) &&
                        !string.IsNullOrEmpty(logFile);
                }
            }
        }

        private class UnknownKeyException : Exception
        {
            public UnknownKeyException(string message)
                : base(message)
            { }
        }

        #endregion

        // File system helpers
        #region Helpers

        public static void DeleteDirectory(string path)
        {
            var dir = new DirectoryInfo(path);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Directory does not exist or could not be found: "
                    + path);
            }

            foreach (var directory in Directory.GetDirectories(path)) {
                DeleteDirectory(directory);
            }

            try {
                Directory.Delete(path, true);
            } catch (IOException) {
                Directory.Delete(path, true);
            } catch (UnauthorizedAccessException) {
                Directory.Delete(path, true);
            }
        }
        
        public static void ClearDirectory(string dirName)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(dirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Directory does not exist or could not be found: "
                    + dirName);
            }

            foreach (var f in dir.GetFiles()) {
                f.Delete();
            }
            foreach (var d in dir.GetDirectories()) {
                DeleteDirectory(d.FullName);
            }
        }

        #endregion

        // Valid parameter set
        // "-batchmode -nographics -quit -projectPath \"{0}\" -executeMethod {1} -logFile \"{2}\""
        // Where:
        // {0} -- pathSources
        // {1} -- Build type: S3dCommandLineBuild.StandaloneGame/S3dCommandLineBuild.ReleaseGame
        // {2} -- logFile = Application.LocalUserAppDataPath + "\\BuildLog.txt"
        static void Main(string[] args)
        {
            var values = new KeyValues();
            try {
                for (var i = 0; i < 9; ++i) {
                    var key = ((string)args.GetValue(i)).Trim();
                    switch (key) {
                        case "-batchmode":
                            values.batchmode = true;
                            break;
                        case "-nographics":
                            values.nographics = true;
                            break;
                        case "-quit":
                            values.quit = true;
                            break;
                        case "-projectPath":
                            values.projectPath = args.GetValue(++i) as string;
                            break;
                        case "-executeMethod":
                            values.executeMethod = args.GetValue(++i) as string;
                            break;
                        case "-logFile":
                            values.logFile = args.GetValue(++i) as string;
                            break;
                        default:
                            throw new UnknownKeyException("Unknown key '" + key + "'");
                    }
                }
            } catch (IndexOutOfRangeException e) {
                Console.WriteLine("Error: " + e.Message);
                return;
            } catch (UnknownKeyException e) {
                Console.WriteLine("Error: " + e.Message);
                return;
            }

            if (!values.IsValid) {
                Console.WriteLine("Error: " + "Not enough keys");
                return;
            }

            Debug.Assert(values.executeMethod != null, "values.executeMethod != null");
            Debug.Assert(values.projectPath != null, "values.projectPath != null");
            Debug.Assert(values.logFile != null, "values.logFile != null");

            var buildType = values.executeMethod.Replace("S3dCommandLineBuild.", "").Replace("Game", "").Trim(); // S3dCommandLineBuild.ReleaseGame
            if (buildType != "Standalone" && buildType != "Release") {
                Console.WriteLine("Error: " + "unknown build type '" + buildType + "'");
                return;
            }

            var pathUnityOutput = Path.Combine(Path.Combine(values.projectPath, "Builds"), buildType);
            if (!Directory.Exists(pathUnityOutput)) {
                Directory.CreateDirectory(pathUnityOutput);
            }
            ClearDirectory(pathUnityOutput);

            File.WriteAllText(Path.Combine(pathUnityOutput, "ReadMe.txt"), "HEY, I AM VALID BUILD!");
            File.WriteAllText(values.logFile, "HEY, I AM UNITY LOG!");
        }
    }
}
