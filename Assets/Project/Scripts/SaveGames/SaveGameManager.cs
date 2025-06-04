using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Util.DataPersistence;
using Project.Scripts.Util.Linq;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.SaveGames;

public class SaveGameManager : Singleton<SaveGameManager> {
    [field: SerializeField]
    private string SaveFileName { get; set; } = "autosave";
    
    [field: SerializeField]
    private string SaveGameDirectory { get; set; } = string.Empty;
    
    private SaveGameParser Parser { get; set; } = new SaveGameParser();
    
    private SaveGame? SavedGame { get; set; }

    protected override void Awake() {
        base.Awake();
        this.SaveGameDirectory = Application.persistentDataPath;
        this.SaveFileName = "autosave";
    }
    
    private void Start() {
        this.LoadGame();
    }

    public void NewGame() {
        this.SavedGame = new SaveGame();
    }

    public void LoadGame() {
        if (this.Parser.TryReadFrom(this.SaveGameDirectory, this.SaveFileName, out SaveGame? save)) {
            save?.Load();
        } else {
            Debug.LogWarning("Trying to load a save when there is no saved game. Will start a new game instead.");
            this.NewGame();
        }
    }

    public void SaveGame(string sessionId) {
        SaveGame save = new SaveGame();
        Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
              .OfType<IPersistent>()
              .Select(obj => obj.Save())
              .ForEach(data => save.Register(data));
        this.Parser.WriteToFile(save, this.SaveGameDirectory, this.SaveFileName);
    }
}
