using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.SaveGames;

/*[Serializable]
internal class EncryptedJsonSaveGameParser : JsonSaveGameParser, IEncryptor {
    private const string CodeWordKeys =
            "QWERTYUIOPASDFGHJKLZXVBNMqwertyuiopasdfghjklzxcvbnm0123456789!@#$%^&*()_-+={}[]|;:'\",.<>/?";
    private const int CodeWordLength = 10;
    
    private string CodeWord { get; set; } = string.Empty;
    
    protected override string Encode(SaveGame saveGame) {
        string unencrypted = base.Encode(saveGame);
        return JsonUtility.ToJson(saveGame);
    }
    
    protected override SaveGame Decode(string texts) {
        return JsonUtility.FromJson<SaveGame>(texts);
    }

    private void UpdateCodeWord() {
        IEnumerable<char> chars =
                Enumerable.Repeat(EncryptedJsonSaveGameParser.CodeWordKeys, EncryptedJsonSaveGameParser.CodeWordLength)
                          .Select(str => str[Random.Range(0, str.Length)]);
        this.CodeWord = new string(chars.ToArray());
    }

    public string Encrypt(string texts) {
        throw new NotImplementedException()
    }
}*/
