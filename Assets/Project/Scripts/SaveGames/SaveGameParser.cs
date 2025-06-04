using System;
using System.IO;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Project.Scripts.SaveGames;

[Serializable]
public class SaveGameParser {
    private const string PassCode = "PeAdSoScCsOsDaEp";
    /*private static Random Random { get; } = new Random();
    
    public static string GenerateRandomPassword(int length = 16) {
        // ReSharper disable once StringLiteralTypo
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < length; i += 1) {
            result.Append(chars[SaveGameParser.Random.Next(chars.Length)]);
        }
        
        return result.ToString();
    }*/

    protected string Encode(SaveGame saveGame) {
        string json = JsonUtility.ToJson(saveGame);
        StringBuilder sb = new StringBuilder(json.Length);
        for (int i = 0; i < json.Length; i += 1) {
            sb.Append(json[i] ^ SaveGameParser.PassCode[i % SaveGameParser.PassCode.Length]);
        }
        
        return sb.ToString();
    }

    protected SaveGame Decode(string texts) {
        StringBuilder sb = new StringBuilder(texts.Length);
        for (int i = 0; i < texts.Length; i += 1) {
            sb.Append(texts[i] ^ SaveGameParser.PassCode[i % SaveGameParser.PassCode.Length]);
        }
        
        return JsonUtility.FromJson<SaveGame>(sb.ToString());
    }
    
    public bool TryReadFrom(string directoryPath, string fileName, out SaveGame? saveGame) {
        string path = Path.Combine(directoryPath, fileName);
        if (!File.Exists(path)) {
            saveGame = null;
            return false;
        }

        try {
            using FileStream stream = new FileStream(path, FileMode.Open);
            using StreamReader reader = new StreamReader(stream);
            string contents = reader.ReadToEnd();
            saveGame = this.Decode(contents);
            return true;
        } catch (Exception e) {
            Debug.LogError($"Error when loading game from {path}:\n {e}");
            saveGame = null;
            return false;
        }
    }
    
    public void WriteToFile(SaveGame saveGame, string directoryPath, string fileName) {
        string path = Path.Combine(directoryPath, fileName);
        try {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            string contents = this.Encode(saveGame);
            using FileStream stream = new FileStream(path, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(contents);
        } catch (Exception e) {
            Debug.LogError($"Error when saving game to {path}:\n {e}");
        }
    }
}
