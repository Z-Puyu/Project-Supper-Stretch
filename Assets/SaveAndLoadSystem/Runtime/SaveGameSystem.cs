using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonFrameworks.Utilities;
using SaintsField;
using SaintsField.Playa;
using SaveAndLoadSystem.Runtime.Momentos;
using UnityEngine;

namespace SaveAndLoadSystem.Runtime {
    public sealed class SaveGameSystem : Singleton<SaveGameSystem> {
        [field: SerializeField] private string CustomSaveGamePath { get; set; } = string.Empty;
        [field: SerializeField] private string DefaultSaveGameName { get; set; } = "autosave";
        [field: SerializeField] private string SaveFileExtension { get; set; } = "json";
        
        [field: SerializeReference, ReferencePicker]
        private ISerialiser Serialiser { get; set; } = new JsonSerialiser();
        
        private SaveGame? CurrentSession { get; set; }

        private string FindPathToSaveFile(string filename) {
            if (string.IsNullOrWhiteSpace(filename)) {
                filename = this.DefaultSaveGameName;
            }

            if (!filename.EndsWith(this.SaveFileExtension)) {
                filename += this.SaveFileExtension.StartsWith(".")
                        ? this.SaveFileExtension
                        : $".{this.SaveFileExtension}";
            }

            string folder = string.IsNullOrWhiteSpace(this.CustomSaveGamePath)
                    ? Application.persistentDataPath
                    : this.CustomSaveGamePath;
            return Path.Combine(folder, filename);
        }
        
        private void Save(SaveGame save, bool overwrite = false) {
            string path = this.FindPathToSaveFile(save.Filename);
            if (!overwrite && File.Exists(path)) {
                int index = 1;
                do {
                    path = this.FindPathToSaveFile($"{save.Filename} ({index})");
                    index += 1;
                } while (File.Exists(path));
            }

#if DEBUG
            Debug.Log($"Saving game to: {path}", this);      
#endif
            IEnumerable<ISaveable> objects = Object.FindObjectsByType<Component>(
                FindObjectsInactive.Include, FindObjectsSortMode.None
            ).OfType<ISaveable>();
            foreach (ISaveable obj in objects) {
                obj.Save();    
            }
            
            string data = this.Serialiser.Serialise(save);
            File.WriteAllText(path, data);
        }

        public SaveGame NewGame() {
            return this.CurrentSession = new SaveGame { Filename = this.DefaultSaveGameName };
        }

        [Button]
        public bool Save(string filename = "") {
            if (this.CurrentSession is not null) {
                if (!string.IsNullOrWhiteSpace(filename)) {
                    this.CurrentSession.Filename = filename;
                }
                
                this.Save(this.CurrentSession);
                return true;
            }
#if DEBUG
            Debug.LogError("Cannot save game as no session is active.", this);
#endif
            return false;
        }

        [Button]
        public void Load(string filename) {
            string path = this.FindPathToSaveFile(filename);
            if (!File.Exists(path)) {
#if DEBUG
                Debug.LogError($"Save file does not exist at path: {path}. Game cannot load.", this);
#endif
                return;
            }
            
#if DEBUG
            Debug.Log($"Loading game from: {path}", this);
#endif
            this.CurrentSession = this.Serialiser.Deserialise<SaveGame>(File.ReadAllText(path));
            IEnumerable<ISaveable> objects = Object.FindObjectsByType<Component>(
                FindObjectsInactive.Include, FindObjectsSortMode.None
            ).OfType<ISaveable>();
            foreach (ISaveable obj in objects) {
                obj.Load();  
            }
        }

        internal S ReadSaveData<S>(string id) where S : IMomento, new() {
            return (this.CurrentSession ?? this.NewGame()).ReadSaveData<S>(id);
        } 
        
        internal void UpdateSaveData<S>(string id, S data) where S : IMomento {
            (this.CurrentSession ?? this.NewGame())[id] = data;
        }
    }
}
