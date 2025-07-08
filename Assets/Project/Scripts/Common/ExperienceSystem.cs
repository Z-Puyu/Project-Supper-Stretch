using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Common;

public class ExperienceSystem : MonoBehaviour {
    public static event UnityAction<ExperienceSystem> OnExperienceChanged = delegate { };
    
    [field: SerializeField, Tooltip("This curve maps current level to experience points required to level up")] 
    private AnimationCurve LevellingCurve { get; set; } = new AnimationCurve();
    
    [NotNull]
    [field: SerializeField, Required] 
    private GameObject? Owner { get; set; }
    
    public int CurrentLevel { get; private set; }
    public int CurrentXp { get; private set; }
    public int XpToNextLevel { get; private set; }

    private void Start() {
        this.XpToNextLevel = Mathf.FloorToInt(this.LevellingCurve.Evaluate(this.CurrentLevel));
    }

    public void AddExperience(int experience) {
        this.CurrentXp += experience;
        ExperienceSystem.OnExperienceChanged.Invoke(this);
        while (this.CurrentXp >= this.XpToNextLevel) {
            this.LevelUp();
        }
    }

    private void LevelUp() {
        this.CurrentXp -= this.XpToNextLevel;
        this.CurrentLevel += 1;
        this.XpToNextLevel = Mathf.FloorToInt(this.LevellingCurve.Evaluate(this.CurrentLevel));
        ExperienceSystem.OnExperienceChanged.Invoke(this);
    }
}
