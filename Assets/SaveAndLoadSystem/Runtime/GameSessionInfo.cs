namespace SaveAndLoadSystem.Runtime {
    public readonly record struct GameSessionInfo(
        SaveGame LoadedSave,
        SaveGame GameState,
        SaveSlot SaveSlot
    );
}
