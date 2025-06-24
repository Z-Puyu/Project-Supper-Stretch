using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Player;

public class ExperienceSystem : MonoBehaviour {
    public static event UnityAction<ExperienceSystem> OnLevelUp;
    
    [field: SerializeField, Tooltip("This curve maps current level to experience points required to level up")] 
    private AnimationCurve LevellingCurve { get; set; } = new AnimationCurve();
    
    public int CurrentLevel { get; set; }
    public int CurrentXp { get; set; }
    public int XpToNextLevel { get; set; }

    private void Start() {
        this.XpToNextLevel = Mathf.FloorToInt(this.LevellingCurve.Evaluate(this.CurrentLevel));
    }

    public void AddExperience(int experience) {
        this.CurrentXp += experience;
        Debug.Log($"Gained {experience} xp. Current: {this.CurrentXp} / {this.XpToNextLevel}");
        if (this.CurrentXp >= this.XpToNextLevel) {
            this.LevelUp();
        }
    }

    public void LevelUp() {
        this.CurrentXp -= this.XpToNextLevel;
        this.CurrentLevel += 1;
        this.XpToNextLevel = Mathf.FloorToInt(this.LevellingCurve.Evaluate(this.CurrentLevel));
        ExperienceSystem.OnLevelUp.Invoke(this);
    }
}
