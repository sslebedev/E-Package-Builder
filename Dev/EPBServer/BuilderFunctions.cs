using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EPackageBuilder
{
    public class BuilderFunctions
    {
        public class FunctionFailedExeption : Exception
        {
            public FunctionFailedExeption(string message) : base(message) { }
        }

        private class FileSamples
        {
            public readonly string BuildCfg;
            public readonly string GameFullBat;
            public readonly string GameShortBat;
            public readonly string CopyListTxt;
            public readonly string GameVersionTxt;

            public FileSamples(string pathTemplates)
            {
                BuildCfg = File.ReadAllText(Path.Combine(pathTemplates, "build.cfg"));
                GameFullBat = File.ReadAllText(Path.Combine(pathTemplates, "GameFULL.bat"));
                GameShortBat = File.ReadAllText(Path.Combine(pathTemplates, "GameSHORT.bat"));
                CopyListTxt = File.ReadAllText(Path.Combine(pathTemplates, "copylist.txt"));
                GameVersionTxt = File.ReadAllText(Path.Combine(pathTemplates, "GameVersion.txt"));
            }
        }

        public interface ILogger
        {
            void WriteLogLine(String txt = "");
        }

        public static void MakeSources(
            string pathTemplates,
            string pathProject,
            string pathSources,
            string description,
            string gameCheckToolPath,
            ILogger logger)
        {
            var fileSamples = new FileSamples(pathTemplates);

            var pathRootPrj = pathProject;
            var pathRootSrc = pathSources;

            logger.WriteLogLine(DateTime.Now + " | Making sources");
            logger.WriteLogLine("Source directory: " + pathRootPrj);
            logger.WriteLogLine("Target directory: " + pathRootSrc);

            // Create directory if not exist
            if (!Directory.Exists(pathRootSrc)) {
                logger.WriteLogLine("Target directory not exist. Creating...");
                Directory.CreateDirectory(pathRootSrc);
            }

            // Cleaning up target
            logger.WriteLogLine("Cleaning " + pathRootSrc + "...");
            Helpers.ClearDirectory(pathRootSrc);

            // Reading game info
            logger.WriteLogLine("Reading game info...");
            var gameInfo = ParseGameInfo(pathRootPrj, description);

            // Making folders
            logger.WriteLogLine("Creating structure...");
            var folderGame = "Game-" + gameInfo.ShortName;
            var pathProjects = Path.Combine(pathRootSrc, "projects");
            Directory.CreateDirectory(pathProjects);
            var pathGame = Path.Combine(pathProjects, folderGame);
            Directory.CreateDirectory(pathGame);

            // Make system files
            logger.WriteLogLine("Creating system files...");

            const string nameBuildCfg = "build.cfg";
            logger.WriteLogLine(nameBuildCfg);
            var textBuildCfg = fileSamples.BuildCfg;
            textBuildCfg = textBuildCfg.Replace("###SHORTNAME###", gameInfo.ShortName);
            textBuildCfg = textBuildCfg.Replace("###REVISION###", gameInfo.Revision);
            textBuildCfg = textBuildCfg.Replace("###FAMILY###", gameInfo.Family);
            textBuildCfg = textBuildCfg.Replace("###DESCRIPTION###", gameInfo.Description);
            File.WriteAllText(Path.Combine(pathRootSrc, nameBuildCfg), textBuildCfg);

            var nameGameFullBat = "Game" + gameInfo.FullName + ".bat";
            logger.WriteLogLine(nameGameFullBat);
            var textGameFullBat = fileSamples.GameFullBat;
            File.WriteAllText(Path.Combine(pathRootSrc, nameGameFullBat), textGameFullBat);

            var nameGameShortBat = "Game" + gameInfo.ShortName + ".bat";
            logger.WriteLogLine(nameGameShortBat);
            var textGameShortBat = fileSamples.GameShortBat;
            textGameShortBat = textGameShortBat.Replace("###FULLNAME###", gameInfo.FullName);
            textGameShortBat = textGameShortBat.Replace("###SHORTNAME###", gameInfo.ShortName);
            File.WriteAllText(Path.Combine(pathGame, nameGameShortBat), textGameShortBat);

            // Copy files and folders from source to dst using copylist
            logger.WriteLogLine("Copy sources...");
            var copylist = ParceCopyList(fileSamples.CopyListTxt);
            foreach (var s in copylist) {
                var src = Path.Combine(pathRootPrj, s);
                var dst = Path.Combine(pathGame, s);
                if (File.Exists(src)) {
                    logger.WriteLogLine(src + ";");
                    File.Copy(src, dst);
                } else if (Directory.Exists(src)) {
                    logger.WriteLogLine(src + ";");
                    Helpers.DirectoryCopy(src, dst);
                } else {
                    throw new FileNotFoundException("File or directory '" + pathRootSrc + "' not found");
                }
            }

            // Copy PRV
            {
                var src = Path.Combine(pathRootPrj, "PRV");
                var dst = Path.Combine(pathRootSrc, "PRV");
                if (Directory.Exists(src)) {
                    logger.WriteLogLine("Copy PRV...");
                    Helpers.DirectoryCopy(src, dst);
                } else {
                    logger.WriteLogLine("Warning: '" + src + "' not found;");
                }
            }

            // Running AscendCleanup
            logger.WriteLogLine("Running AscendCleanup...");
            Helpers.AscendCleanup(pathGame);

            // Writing game version
            WriteGameVersion(pathProject, logger);

            // Game check
            try {
                var gameCheckResult = ProcessGameCheck(gameCheckToolPath, pathSources);
                logger.WriteLogLine(gameCheckResult);
                logger.WriteLogLine();
            } catch (FunctionFailedExeption exeption) {
                logger.WriteLogLine("CAPITAL WARNING: Cannot perform game check: " + exeption.Message);
            }
            // Message completed
            logger.WriteLogLine("Completed.");
        }

        private static IEnumerable<String> ParceCopyList(string copyList)
        {
            var res = copyList.Split(Environment.NewLine.ToCharArray()).Select(l => l.Trim()).Where(l => l.Length > 0 && !l.StartsWith(@"#"));
            return res;
        }

        public static void WriteGameVersion(string pathProject, ILogger logger)
        {
            const string nameGameVersion = "GameVersion.txt";
            var pathGameVersion = Path.Combine(pathProject, nameGameVersion);
            logger.WriteLogLine("Game Version:");
            var textNewGameVersion = File.ReadAllText(pathGameVersion);
            logger.WriteLogLine(textNewGameVersion);
            logger.WriteLogLine("-----------");
        }

        private class GameInfo
        {
            public string Description;
            public string Family;
            public string Platform;
            public string SAP;
            public string Revision;
            public string ShortName { get { return Platform + SAP; } }
            public string FullName { get { return Family + Platform + SAP + Revision; } }
        }

        private static GameInfo ParseGameInfo(string rootPath, string description)
        {
            var regs = Path.Combine(rootPath, "Registries");
            var binregName = Directory.GetFiles(regs, "*.xbinreg").First();
            var text = File.ReadAllText(Path.Combine(regs, binregName));

            var gameClientIndex0 = text.IndexOf("GameClient>");
            var gameClientIndex1 = gameClientIndex0 + "GameClient>".Length;
            var gameClientIndex2 = text.IndexOf("<", gameClientIndex1, StringComparison.Ordinal);

            var fullName = text.Substring(gameClientIndex1, gameClientIndex2 - gameClientIndex1);
            // Game020001SI9M01
            var gameInfo = new GameInfo {
                Description = description,
                Family = fullName.Substring(4, 3),
                Platform = fullName.Substring(7, 3),
                SAP = fullName.Substring(10, 3),
                Revision = fullName.Substring(13, 3)
            };

            return gameInfo;
        }

        public static void MakeGameVersion(string pathProject, string templateGameVersion, ILogger logger, bool increase)
        {
            const string nameGameVersion = "GameVersion.txt";
            var pathGameVersion = Path.Combine(pathProject, nameGameVersion);

            var linesOldGameVersion = File.ReadAllLines(pathGameVersion);
            var textNewGameVersion = templateGameVersion;

            var oldBuild = int.Parse(linesOldGameVersion[0].Substring(linesOldGameVersion[0].IndexOf(':') + 1));
            var oldSdk = linesOldGameVersion[2].Substring(linesOldGameVersion[2].IndexOf(':') + 1).Trim();
            var oldUnity = linesOldGameVersion[3].Substring(linesOldGameVersion[3].IndexOf(':') + 1).Trim();

            // 11/27/2015 - 18:04 (+03:00)
            var date = DateTime.Now.ToString("MM'/'dd'/'yyyy - HH:mm (zzz)");

            textNewGameVersion = textNewGameVersion.Replace("###BUILD###", (oldBuild + (increase ? 1 : 0)).ToString());
            textNewGameVersion = textNewGameVersion.Replace("###DATE###", date);
            textNewGameVersion = textNewGameVersion.Replace("###SDK###", oldSdk);
            textNewGameVersion = textNewGameVersion.Replace("###UNITY###", oldUnity);

            File.WriteAllText(pathGameVersion, textNewGameVersion);
            WriteGameVersion(pathProject, logger);
        }

        public static string ProcessGameCheck(string gameCheckTool, string pathRelease)
        {
            const string gameCheckArgs = "-grp \"{0}\" -gsp \"{1}\" -rf \"{2}\"";

            if (gameCheckTool.Length == 0) {
                throw new FunctionFailedExeption("Error: GameCheck path is not set, cannot be performed");
            }

            //var checkResult = Application.LocalUserAppDataPath + "\\gameCheckResult.xml";
            var checkResult = pathRelease + "\\gameCheckResult.xml";

            var pathProjects = Path.Combine(pathRelease, "projects");
            var srcInfo = new DirectoryInfo(pathProjects);
            var directoryInfo = srcInfo.GetDirectories().FirstOrDefault(d => d.Name.StartsWith("Game"));
            Debug.Assert(directoryInfo != null, "directoryInfo != null");
            var gsp = directoryInfo.FullName;
            var param = string.Format(gameCheckArgs, pathRelease, gsp, checkResult);
            var pInfo = new ProcessStartInfo(gameCheckTool, param) { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };
            var p = Process.Start(pInfo);

            p.BeginOutputReadLine();
            p.OutputDataReceived += OnProcessOutputDataReceived;
            p.WaitForExit();

            return ParseGameCheckResult(checkResult);
        }

        private static string ParseGameCheckResult(string result)
        {
            var xDoc = XDocument.Load(result, LoadOptions.SetLineInfo);

            Debug.Assert(xDoc.Root != null, "xDoc.Root != null");
            return "GameCheck results:\r\n"
                + xDoc.Root.Elements()
                .SelectMany(x => x.Elements().SelectMany(y => y.Elements()))
                .Select(x => x.Value).Aggregate((x, y) => x + "\r\n" + y);
        }

        private static void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == "Done.") {
                ((Process)sender).Kill();
            }
        }

        public static class BuildTypes
        {
            public const string Standalone = "Standalone";
            public const string Release = "Release";
        }

        public static void ProcessBuild(string unity, string buildType, string pathSources, string pathBuild, ILogger logger)
        {
            const string formatUnityArgs = "-batchmode -nographics -quit -projectPath \"{0}\" -executeMethod {1} -logFile \"{2}\"";
            const string formatCommand = "S3dCommandLineBuild.{0}Game";

            //var logFile = Application.LocalUserAppDataPath + "\\BuildLog.txt";
            var logFile = pathBuild + "\\BuildLog.txt";

            logger.WriteLogLine("Making " + buildType + " build...");

            if (unity.Length == 0) {
                throw new FunctionFailedExeption("Error: Unity path is not set, cannot be performed");
            }

            logger.WriteLogLine("Allocating directories...");
            // Allocate build target directory
            if (!Directory.Exists(pathBuild)) {
                logger.WriteLogLine("Target directory not exist. Creating...");
                Directory.CreateDirectory(pathBuild);
            }
            Helpers.ClearDirectory(pathBuild);

            // Calculate output directory
            var command = String.Format(formatCommand, buildType);
            var unityArgs = string.Format(formatUnityArgs, pathSources, command, logFile);
            var pathUnityOutput = Path.Combine(Path.Combine(pathSources, "Builds"), buildType);

            // Reallocate output directory
            if (!Directory.Exists(pathUnityOutput)) {
                Directory.CreateDirectory(pathUnityOutput);
            }
            Helpers.ClearDirectory(pathUnityOutput);

            // Build
            logger.WriteLogLine("Building...");
            var pInfo = new ProcessStartInfo(unity, unityArgs) { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };
            var p = Process.Start(pInfo);
            p.WaitForExit();

            // Result
            logger.WriteLogLine("Unity output:");
            logger.WriteLogLine(logFile);
            logger.WriteLogLine("Checking result...");
            if (Directory.GetFileSystemEntries(pathUnityOutput).Length == 0) {
                throw new FunctionFailedExeption("Error: No unity output result found");
            }

            // Copy result and delete temp
            logger.WriteLogLine("Copying to taget directory...");
            Helpers.DirectoryCopy(pathUnityOutput, pathBuild);
            logger.WriteLogLine("Cleaning temporaries...");
            Helpers.DeleteDirectory(Path.Combine(pathSources, "Builds"));

            logger.WriteLogLine("Completed.");
        }
    }
}