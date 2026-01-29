using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonFrameworks.Utilities;
using SaintsField;
using SaintsField.Playa;
using SaveAndLoadSystem.Runtime.Momentos;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveAndLoadSystem.Runtime {
    public sealed class SaveGameSystem : Singleton<SaveGameSystem> {
        [field: SerializeField] private bool HasUnlimitedSlots { get; set; }
        
        [field: SerializeField, MinValue(1), HideIf(nameof(this.HasUnlimitedSlots))] 
        private int SlotCount { get; set; } = 1;
        
        [field: SerializeField] private string CustomSaveGamePath { get; set; } = string.Empty;
        [field: SerializeField] private string DefaultSaveGameName { get; set; } = "autosave";
        [field: SerializeField] private string SaveFileExtension { get; set; } = "json";
        
        [field: SerializeReference, ReferencePicker]
        private ISerialiser Serialiser { get; set; } = new JsonSerialiser();
        
        private List<SaveSlot> SaveSlots { get; } = new List<SaveSlot>();
        private List<List<SaveGame>> Saves { get; } = new List<List<SaveGame>>();
        private GameSessionInfo CurrentGameSession { get; set; }
        
        public int MaxSlotIndex => this.HasUnlimitedSlots ? int.MaxValue : this.SlotCount - 1;
        public int MaxPresentSlotIndex => this.SaveSlots.Count - 1;
        public int OccupiedSlotCount => this.SaveSlots.Count(slot => !string.IsNullOrWhiteSpace(slot.Name));
        public SaveSlot CurrentSaveSlot => this.CurrentGameSession.SaveSlot;
        public bool HasAnySaveGame => this.Saves.Any(slot => slot.Count > 0);

        private string Extension => this.SaveFileExtension.StartsWith(".")
                ? this.SaveFileExtension
                : $".{this.SaveFileExtension}";

        private string SaveGameDirectory => string.IsNullOrWhiteSpace(this.CustomSaveGamePath)
                ? Application.persistentDataPath
                : this.CustomSaveGamePath;
        
        protected override void Awake() {
            base.Awake();
            Directory.CreateDirectory(this.SaveGameDirectory);
            foreach (string path in Directory.EnumerateFiles(this.SaveGameDirectory, $"*{this.Extension}")) {
                try {
                    using FileStream stream = File.OpenRead(path);
                    using StreamReader reader = new StreamReader(stream);
                    string data = reader.ReadToEnd();
                    SaveGame save = this.Serialiser.Deserialise<SaveGame>(data);
                    SaveSlot slot = save.metadata.Slot;
                    if (this.FindSlot(slot.Index) != slot) {
                        this.SaveSlots[slot.Index] = slot;
                    }

                    this.Saves[slot.Index].Add(save);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
            
            this.Saves.ForEach(slot => slot.Sort());
        }

        public IReadOnlyList<SaveSlot> EnumerateSaveSlots() {
            return this.SaveSlots.ToList().AsReadOnly();
        }
        
        public IReadOnlyList<SaveGame> EnumerateSaves(int slot) {
            if (slot < 0 || slot >= this.SaveSlots.Count) {
                return Array.Empty<SaveGame>();
            }
            
            return this.Saves[slot].ToList().AsReadOnly();
        }

        private SaveSlot FindSlot(int i) {
            i = Math.Clamp(i, 0, this.MaxSlotIndex);
            while (this.SaveSlots.Count <= i) {
                this.SaveSlots.Add(new SaveSlot(this.SaveSlots.Count));
                this.Saves.Add(new List<SaveGame>());
            }
            
            return this.SaveSlots[i];
        }

        private string FindPathToSaveFile(string filename) {
            if (!filename.EndsWith(this.SaveFileExtension)) {
                filename += this.Extension;
            }

            while (File.Exists(Path.Combine(this.SaveGameDirectory, filename))) {
                int index = 1;
                do {
                    filename = $"{Path.GetFileNameWithoutExtension(filename)} ({index}){this.Extension}";
                    index += 1;
                } while (File.Exists(Path.Combine(this.SaveGameDirectory, filename)));
            }
            
            return Path.Combine(this.SaveGameDirectory, filename);
        }

        /// <summary>
        /// Deletes the save at the given index.
        /// </summary>
        /// <param name="slot">The 0-based index of the save slot.</param>
        /// <param name="index">The 0-based index of the save within the slot.</param>
        public void Delete(int slot, int index) {
            if (slot < 0 || slot > this.MaxPresentSlotIndex || index < 0 || index >= this.Saves[slot].Count) {
                return;
            }
            
            File.Delete(this.Saves[slot][index].metadata.SaveFilePath);
            this.Saves[slot].RemoveAt(index);
        }

        /// <summary>
        /// Deletes all saves in the given slot.
        /// </summary>
        /// <param name="slot">The 0-based index of the save slot.</param>
        public void Clear(int slot) {
            if (slot < 0 || slot > this.MaxPresentSlotIndex) {
                return;
            }

            for (int i = 0; i < this.Saves[slot].Count; i += 1) {
                this.Delete(slot, i);
            }
            
            this.Saves[slot].Clear();
            this.SaveSlots[slot] = new SaveSlot(slot);
        }
        
        /// <summary>
        /// Creates a new game session with the given slot and display name.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="displayName"></param>
        /// <returns><c>true</c> if the game session was successfully created, <c>false</c> otherwise.</returns>
        public bool NewGame(int slot, string displayName) {
            if (slot > this.MaxSlotIndex) {
#if DEBUG
                Debug.LogError($"Cannot create new game with out-of-range save slot {slot}", this);       
#endif
                return false;
            }
            
            this.Clear(slot);
            slot = Math.Clamp(slot, 0, this.MaxSlotIndex);
            if (string.IsNullOrWhiteSpace(displayName)) {
#if DEBUG
                Debug.LogError("Cannot create new game with an empty display name.", this);       
#endif
                return false;
            }
            
            SaveGame save = SaveGame.Create(this.FindSlot(slot));
            this.CurrentGameSession = new GameSessionInfo(save, save, this.FindSlot(slot));
            this.RenameCurrentSaveSlot(displayName);
            return true;
        }
        
        [Button]
        public bool LoadLatestSave() {
            try {
                SaveGame save = this.Saves.Where(slot => slot.Count > 0).Select(slot => slot.Max()).Max();
                this.CurrentGameSession = new GameSessionInfo(
                    save, SaveGame.Create(save.metadata.Slot), save.metadata.Slot
                );
                
                return true;
            } catch (Exception e) {
                Debug.LogException(e);
                return false;
            }
        }
        
        /// <summary>
        /// Loads the save game at the given slot and index.
        /// </summary>
        /// <param name="slot">The 0-based index of the save slot to load from.</param>
        /// <param name="index">The 0-based index of the save game within the slot to load.</param>
        [Button]
        public bool Load(int slot = 0, int index = 0) {
            if (slot < 0 || slot >= this.SaveSlots.Count || string.IsNullOrWhiteSpace(this.SaveSlots[slot].Name)) {
#if DEBUG
                Debug.LogError($"Invalid game session at save slot: {slot}", this);
#endif
                return false;
            }
            
            if (index < 0 || index >= this.Saves.Count) {
#if DEBUG
                Debug.LogError($"Invalid save game index: {index}", this);
#endif
                return false;
            }
            
            this.CurrentGameSession = new GameSessionInfo(
                this.Saves[slot][index], SaveGame.Create(this.FindSlot(slot)), this.FindSlot(slot)
            );
            
            return true;
        }
        
        /// <summary>
        /// Saves the current game.
        /// </summary>
        /// <param name="index">The 0-based index within the current save slot to save to.</param>
        /// <param name="displayName">The display name for the save slot, or a default name if not provided.</param>
        /// <returns><c>true</c> if the game was saved successfully, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// If <paramref name="index"/> is greater than the maximum index of the save game list,
        /// a new save will be created.
        /// </remarks>
        [Button]
        public bool Save(int index = int.MaxValue, string displayName = "") {
            if (string.IsNullOrWhiteSpace(this.CurrentGameSession.SaveSlot.Name)) {
#if DEBUG
                Debug.LogError("Cannot save game as no session is active.", this);
#endif
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(displayName)) {
                displayName = this.DefaultSaveGameName;
            }
            
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filename = $"{this.CurrentGameSession.SaveSlot.Name}_{timestamp}{this.SaveFileExtension}";
            string path = this.FindPathToSaveFile(filename);
            this.CurrentGameSession.GameState.metadata = new SaveGame.Metadata(
                this.CurrentGameSession.SaveSlot, path, displayName, timestamp
            );
            
            int slot = this.CurrentGameSession.SaveSlot.Index;
            this.Delete(slot, index);   
            IEnumerable<ISaveable> objects = Object.FindObjectsByType<Component>(
                FindObjectsInactive.Include, FindObjectsSortMode.None
            ).OfType<ISaveable>();
            foreach (ISaveable obj in objects) {
                obj.Save();
            }
            
            this.Saves[slot].Insert(Math.Clamp(index, 0, this.Saves[slot].Count), this.CurrentGameSession.GameState);
            string data = this.Serialiser.Serialise(this.CurrentGameSession.GameState);
            File.WriteAllText(this.CurrentGameSession.GameState.metadata.SaveFilePath, data);
            return true;
        }
        
        /// <summary>
        /// Renames the current save slot for the current game session.
        /// </summary>
        /// <param name="newName">The new name for the save slot.</param>
        /// <remarks>Existing saves in the slot will be retained.</remarks>
        public void RenameCurrentSaveSlot(string newName) {
            SaveSlot newSlot = new SaveSlot(this.CurrentGameSession.SaveSlot.Index, newName);
            this.CurrentGameSession = this.CurrentGameSession with { SaveSlot = newSlot };
            this.SaveSlots[newSlot.Index] = newSlot;
            foreach (SaveGame save in this.Saves[newSlot.Index]) {
                save.MoveToSlot(newSlot);
            }
        }

        /// <summary>
        /// Creates a new save slot with the given display name for the current game session.
        /// </summary>
        /// <param name="displayName">The display name for the new save slot.</param>
        /// <returns><c>true</c> if the slot was created successfully, <c>false</c> otherwise.</returns>
        /// <remarks>Existing saves in the old slot used for the current game session
        /// will not be moved to the new slot.</remarks>
        public bool CreateNewSaveSlot(string displayName) {
            if (this.MaxPresentSlotIndex >= this.MaxSlotIndex) {
#if DEBUG
                Debug.LogError("Maximum allowed slot index is reached.", this);
#endif
                return false;
            }

            SaveSlot slot = new SaveSlot(this.SaveSlots.Count, displayName);
            this.SaveSlots.Add(slot);
            this.Saves.Add(new List<SaveGame>());
            this.CurrentGameSession = this.CurrentGameSession with { SaveSlot = slot };
            return true;
        }

        /// <summary>
        /// Deletes all save slots and saves.
        /// </summary>
        public void Clear() {
            for (int slot = 0; slot < this.SaveSlots.Count; slot += 1) {
                this.Clear(slot);
            }
            
            this.SaveSlots.Clear();
            this.Saves.Clear();
        }

        internal S ReadSaveData<S>(string id) where S : IMomento, new() {
            return this.CurrentGameSession.LoadedSave.ReadSaveData<S>(id);
        } 
        
        internal void WriteSaveData<S>(string id, S data) where S : IMomento {
            this.CurrentGameSession.GameState[id] = data;
        }
    }
}
