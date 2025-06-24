using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.InventorySystem.LootContainers;

public class CoinDropConfig : ScriptableObject {
    [field: SerializeField, MinValue(0)]
    public int BaseWeight { get; private set; }
    
    [field: SerializeField, MinMaxSlider(0, 1000)]
    public Vector2Int AmountRange { get; private set; }

    [field: SerializeField, MinValue(0)]
    private float EnemyStrengthWeightMultiplier { get; set; } = 1;
    
    [field: SerializeField, MinValue(0)]
    private float GameDifficultyWeightMultiplier { get; set; } = 1;
    
    [field: SerializeField, MinValue(0)]
    private float EnemyStrengthDropAmountMultiplier { get; set; } = 1;
    
    [field: SerializeField, MinValue(0)]
    private float GameDifficultyDropAmountMultiplier { get; set; } = 1;
}
