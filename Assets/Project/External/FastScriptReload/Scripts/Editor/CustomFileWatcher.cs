using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Project.External.FastScriptReload.Scripts.Editor {
    [InitializeOnLoad]
    public class CustomFileWatcher : EditorWindow
    {
        public class HashEntry
        {
            private Dictionary<string, string> _hashes = new Dictionary<string, string>();
            // Some metadata for the update function to use
            // WARN: Note this data isn't exactly synced up or anything. It just reads it in when the filewatcher is initialized.
            private string _searchPattern;
            private bool _includeSubdirectories;
        
            public Dictionary<string, string> Hashes => this._hashes;
            public string SearchPattern => this._searchPattern;
            public bool IncludeSubdirectories => this._includeSubdirectories;

            public HashEntry(Dictionary<string, string> hashes, string searchPattern, bool includeSubdirectories)
            {
                this._hashes = hashes;
                this._searchPattern = searchPattern;
                this._includeSubdirectories = includeSubdirectories;
            }
        }

        private static Dictionary<string, HashEntry> FileHashes;
        private static object StateLock = new object();

        private static object ListLock; // Shared lock object
        private static Thread LivewatcherThread;

        public static bool InitSignaled = false;
        private static readonly int WatcherThreadRunEveryNSeconds = 500; //TODO: expose in settings

        static CustomFileWatcher()
        {
            CustomFileWatcher.FileHashes = new Dictionary<string, HashEntry>();
            CustomFileWatcher.ListLock = new object();
            CustomFileWatcher.LivewatcherThread = null;
        }
    
        private static void UpdateFileWatcher()
        {
            if (CustomFileWatcher.FileHashes.Count > 0)
            {
                foreach (var kvp in CustomFileWatcher.FileHashes)
                {
                    CustomFileWatcher.CheckForChanges(kvp.Key, kvp.Value.SearchPattern, kvp.Value.IncludeSubdirectories);
                }
            }
            else
            {
                Debug.LogError("File watcher has not been initialized yet. Please initialize first.");
            }
        }
    
        public static void TryEnableLivewatching()
        {
            if (CustomFileWatcher.LivewatcherThread != null)
            {
                Debug.LogWarning("Livewatcher is already running.");
                return;
            }

            // Run on a separate thread every 1 second
            CustomFileWatcher.LivewatcherThread = new Thread(() =>
            {
                var timer = new Timer((state) =>
                {
                    // Go at it if we've initialized
                    if (CustomFileWatcher.FileHashes.Count > 0)
                        CustomFileWatcher.UpdateFileWatcher();
                }, null, 0, CustomFileWatcher.WatcherThreadRunEveryNSeconds);
            });

            CustomFileWatcher.LivewatcherThread.Start();
        }
    
        public static void InitializeSingularFilewatcher(string directoryPath, string searchPattern, bool includeSubdirectories)
        {
#if ImmersiveVrTools_DebugEnabled
        Debug.Log("Initializing hashes for directory: " + directoryPath);
#endif

            var thread = new Thread(() =>
            {
                lock (CustomFileWatcher.StateLock)
                {
                    var hashes = new Dictionary<string, string>();
                    var files = Directory.GetFiles(directoryPath, searchPattern, includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                    foreach (var filePath in files)
                    {
                        var hash = CustomFileWatcher.GetFileHash(filePath);
                        hashes[filePath] = hash;
                    }

                    CustomFileWatcher.FileHashes[directoryPath] = new HashEntry(hashes, searchPattern, includeSubdirectories);
                }
            });
            thread.Start();
        }

        private static void CheckForChanges(string directoryPath, string searchPattern, bool includeSubdirectories)
        {
            // Not really sure if this nuclear locking is needed
            lock (CustomFileWatcher.StateLock)
            {
                var hashes = CustomFileWatcher.FileHashes[directoryPath].Hashes;

                // Time profiling: Start the stopwatch for Directory.GetFiles
#if ImmersiveVrTools_DebugEnabled

            System.Diagnostics.Stopwatch getFilesStopwatch = new System.Diagnostics.Stopwatch();
            getFilesStopwatch.Start();
#endif

                string[] files = Directory.GetFiles(directoryPath, searchPattern, includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

#if ImmersiveVrTools_DebugEnabled

            // Time profiling: Stop the stopwatch for Directory.GetFiles and log the elapsed time
            getFilesStopwatch.Stop();
            Debug.Log("Directory.GetFiles elapsed time: " + getFilesStopwatch.ElapsedMilliseconds + " ms");
#endif

                // Check if files were created or modified
                // Time profiling: Start the stopwatch for file creation/modification
                var fileChangeStopwatch = new System.Diagnostics.Stopwatch();
                fileChangeStopwatch.Start();

                foreach (var file in files)
                {
                    if (!hashes.ContainsKey(file))
                    {
                        // New file
#if ImmersiveVrTools_DebugEnabled
                    Debug.Log("New file: " + file);
#endif
                        continue;
                    }

                    else if (hashes[file] != CustomFileWatcher.GetFileHash(file))
                    {
                        // File changed
#if ImmersiveVrTools_DebugEnabled
                    Debug.Log("File changed: " + file);
#endif
                        CustomFileWatcher.RecordChange(file);
                    }
                }

#if ImmersiveVrTools_DebugEnabled
            // Time profiling: Stop the stopwatch for file creation/modification and log the elapsed time
            fileChangeStopwatch.Stop();
            Debug.Log("File creation/modification elapsed time: " + fileChangeStopwatch.ElapsedMilliseconds + " ms");
#endif

                // Check if any files were deleted
                // Time profiling: Start the stopwatch for file deletion
                var fileDeletionStopwatch = new System.Diagnostics.Stopwatch();
                fileDeletionStopwatch.Start();

                foreach (var kvp in hashes)
                {
                    if (!File.Exists(kvp.Key))
                    {
#if ImmersiveVrTools_DebugEnabled
                    Debug.Log("File deleted: " + kvp.Key);
#endif
                    }
                }

                // Time profiling: Stop the stopwatch for file deletion and log the elapsed time
#if ImmersiveVrTools_DebugEnabled
            fileDeletionStopwatch.Stop();
            Debug.Log("File deletion elapsed time: " + fileDeletionStopwatch.ElapsedMilliseconds + " ms");
#endif

                // Update hashes
                hashes.Clear();
                foreach (var file in files)
                {
                    var hash = CustomFileWatcher.GetFileHash(file);
                    hashes[file] = hash;
                }
            }
        }


        private static string GetFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = md5.ComputeHash(stream);
                    var sb = new StringBuilder();
                    for (var i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }
                    return sb.ToString();
                }
        }

        private static void RecordChange(string path)
        {
            if (FastScriptReloadManager.Instance.ShouldIgnoreFileChange()) return;

            lock (CustomFileWatcher.ListLock)
            {
                FastScriptReloadManager.Instance.AddFileChangeToProcess(path);
            }
        }
    }
}
