namespace Project.Scripts.SaveGames;

internal interface IEncryptor {
    public abstract string Encrypt(string texts);
    
    public abstract string Decrypt(string gibberish, string codeWord);
}
