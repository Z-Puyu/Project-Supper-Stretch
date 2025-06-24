using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation.CodeRewriting.Partials {
    public static class PartialClassFinder
    {
        //TODO: optimize lookup, currently this loads all files in directory and checks if they have 'partial' string in them. For directories with many files this will not be sufficient
        public static IEnumerable<string> FindPartialClassFilesInDirectory(string filePath, int maxDepth = int.MaxValue)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return Enumerable.Empty<string>();

            string directory = Path.GetDirectoryName(filePath);
            string fileContent = File.ReadAllText(filePath);

            string partialClassName = PartialClassFinder.ExtractPartialClassName(fileContent);
            if (string.IsNullOrEmpty(partialClassName))
            {
                Debug.LogWarning($"No partial class/struct found in file: {filePath}");
                return Enumerable.Empty<string>();
            }

            return PartialClassFinder.FindPartialClassFilesRecursively(directory, partialClassName, maxDepth, currentDepth: 0);
        }

        private static IEnumerable<string> FindPartialClassFilesRecursively(string directory, string partialClassName, int maxDepth, int currentDepth)
        {
            var files = new List<string>();

            try
            {
                // Search in current directory
                files.AddRange(Directory.GetFiles(directory, "*.cs")
                                        .Where(file => PartialClassFinder.IsPartialClassFile(file, partialClassName)));

                // If we haven't reached max depth, search in subdirectories
                if (currentDepth < maxDepth)
                {
                    foreach (var subDir in Directory.GetDirectories(directory))
                    {
                        files.AddRange(PartialClassFinder.FindPartialClassFilesRecursively(subDir, partialClassName, maxDepth, currentDepth + 1));
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogWarning($"Access denied to directory: {directory}. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error searching directory: {directory}. Error: {ex.Message}");
            }

            return files;
        }

        private static bool IsPartialClassFile(string filePath, string className)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                return content.Contains($"partial class {className}")
                    || content.Contains($"partial struct {className}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading file: {filePath}. Error: {ex.Message}");
                return false;
            }
        }

        private static string ExtractPartialClassName(string fileContent)
        {
            // This is a simple implementation and might need to be more robust
            // depending on your specific needs
            var partialClassDeclaration = fileContent.Split(new[] { "partial class", "partial struct" }, StringSplitOptions.RemoveEmptyEntries)
                                                     .Skip(1)
                                                     .FirstOrDefault();

            if (partialClassDeclaration == null)
                return string.Empty;

            return partialClassDeclaration.TrimStart().Split(new[] { ' ', '{', ':' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault().Trim();
        }
    }
}
