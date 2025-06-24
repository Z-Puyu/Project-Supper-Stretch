namespace Project.Scripts.SaveGames;

public class EncryptedSaveGame : SaveGame {
    public string CodeWord { get; private init; }
    
    public EncryptedSaveGame(string codeWord) {
        this.CodeWord = codeWord;
    }
}
