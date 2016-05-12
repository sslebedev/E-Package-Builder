using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace EPackageBuilder
{
    class Helpers
    {
        public static void DirectoryCopy(string dirNameSrc, string dirNameDst, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(dirNameSrc);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Directory does not exist or could not be found: "
                    + dirNameSrc);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(dirNameDst)) {
                Directory.CreateDirectory(dirNameDst);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files) {
                var temppath = Path.Combine(dirNameDst, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (var subdir in dirs) {
                    string temppath = Path.Combine(dirNameDst, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, true);
                }
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


        public static void AscendCleanup(string dirName)
        {
            var dirs = Directory.GetDirectories(dirName, "*", SearchOption.AllDirectories);

            foreach (var dir in dirs.Where(d => d.EndsWith("bin") || d.EndsWith("obj"))) {
                Directory.Delete(dir, true);
            }
        }

        public static void ZipDirectory(string archiveName, string contentDirectory)
        {
            if (File.Exists(archiveName)) {
                File.Delete(archiveName);
            }
            ZipFile.CreateFromDirectory(contentDirectory, archiveName, CompressionLevel.NoCompression, false);
        }
    }
}
