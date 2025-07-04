using System.Linq;
using SaintsField;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Project.Scripts.Characters.Enemies;

[DisallowMultipleComponent]
public class EnemySpawnPoint : MonoBehaviour {
    [field: SerializeField, SaintsDictionary("Enemy", "Weight")]
    private SaintsDictionary<EnemyCharacter, int> Enemies { get; set; } = [];
    
    private int TotalWeight { get; set; }

    private void Awake() {
        this.TotalWeight = this.Enemies.Values.Sum();
    }


    public void Spawn() {
        int select = Random.Range(0, this.TotalWeight);
        int current = 0;
        foreach ((EnemyCharacter enemy, int weight) in this.Enemies) {
            current += weight;
            if (select >= current) {
                continue;
            }
            
            EnemyCharacter spawned = Object.Instantiate(enemy, this.transform.position, Quaternion.identity);
            spawned.transform.SetParent(null);
            break;
        }
    }
}
