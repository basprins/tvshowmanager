using System.Collections.Generic;
using System.IO;

namespace PerfectCode.FileSystemIO
{
    public interface IFileSystem
    {
        string GetCurrentDirectory();
        bool DirectoryExists(string path);
        string GetLastSubDirectoryName(string path);
        string CreateDirectory(string path, string subDirectory);
        void EnsureDirectory(string path);
        void TryClearDirectory(string path);

        IEnumerable<string> EnumerateDirectories(string path);
        IEnumerable<string> EnumerateFiles(string path, bool recursive = false, List<string> patterns = null);

        void CreateFile(string fileName);
        void DeleteFile(string fileName);
        string GetFileName(string fullName);
        string RemoveIllegalCharactersFromFileName(string fileName);
        bool FileExists(string fileName);
        Stream ReadFile(string filePath);
        void CopyFile(string sourceFileName, string targetFileName);

        string CombinePaths(params string[] paths);
        string GetExtension(string fileName);
        string ChangeExtension(string fileName, string extension);
        string GetFileNameWithoutExtension(string fileName);
        string ReplaceExtension(string fileName, string newExtension);
        void OpenInExplorer(string fileName);
    }
}