using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace PerfectCode.FileSystemIO
{
    [DataContract(IsReference = true)]
    public class FileSystem : IFileSystem
    {
        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string GetLastSubDirectoryName(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return directoryInfo.Name;
        }

        public void EnsureDirectory(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void TryClearDirectory(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                    // Ignore all exceptions, deleting files is fire and forget
                }
            }

            var directories = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories);
            foreach (var directory in directories)
            {
                try
                {
                    Directory.Delete(directory);
                }
                catch (Exception)
                {
                    // Ignore all exceptions, deleting directories is fire and forget
                }
            }
        }

        public IEnumerable<string> EnumerateDirectories(string path)
        {
            return Directory.EnumerateDirectories(path);
        }

        public string CreateDirectory(string path, string subDirectory)
        {
            var fullPath = Path.Combine(path, subDirectory);
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }

        public void CopyFile(string sourceFileName, string targetFileName)
        {
            if (File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }
            File.Move(sourceFileName, targetFileName);
        }

        public string CombinePaths(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public string GetExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public string ChangeExtension(string fileName, string extension)
        {
            return Path.ChangeExtension(fileName, extension);
        }

        public string GetFileNameWithoutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        public string ReplaceExtension(string fileName, string newExtension)
        {
            return Path.ChangeExtension(fileName, newExtension);
        }

        public void OpenInExplorer(string fileName)
        {
            var argument = @"/select, " + fileName;

            Process.Start("explorer.exe", argument);
        }

        public IEnumerable<string> EnumerateFiles(string path, bool recursive = false, List<string> patterns = null)
        {
            if (Directory.Exists(path))
            {
                var files =
                    Directory.EnumerateFiles(path, "*.*",
                        recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();

                if (patterns != null && patterns.Any())
                {
                    var filteredFiles = new List<string>();
                    foreach (var pattern in patterns)
                    {
                        var regexFriendlyPattern = pattern.Replace("*", ".*");
                        regexFriendlyPattern = regexFriendlyPattern.Replace("(", @"\(");
                        regexFriendlyPattern = regexFriendlyPattern.Replace(")", @"\)");
                        var regex = new Regex(regexFriendlyPattern);
                        filteredFiles.AddRange(files.Where(file => regex.IsMatch(file)));
                    }
                    return filteredFiles;
                }

                return files;
            }
            return new List<string>();
        }

        public void CreateFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }
        }

        public void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        public string GetFileName(string fullName)
        {
            return Path.GetFileName(fullName);
        }

        public string RemoveIllegalCharactersFromFileName(string fileName)
        {
            foreach (var invalidPathChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidPathChar.ToString(), "");
            }
           
            return fileName;
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public Stream ReadFile(string filePath)
        {
            return File.OpenRead(filePath);
        }
    }
}
