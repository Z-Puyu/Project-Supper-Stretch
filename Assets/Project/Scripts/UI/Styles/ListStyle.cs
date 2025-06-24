using Flexalon.Runtime.Core;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Styles;

[CreateAssetMenu(fileName = "List Container Style", menuName = "UI Framework/Styles/Containers/List Container Style")]
public class ListStyle : FlexStyle {
    [field: SerializeField] public bool DistributeEntries { get; private set; }
    
    [field: SerializeField, HideIf(nameof(this.DistributeEntries))] 
    public float Spacing { get; private set; }
    
    [field: SerializeField] public bool StretchEntries { get; private set; } = true;
    [field: SerializeField] public Align EntryAlignment { get; private set; } = Align.Center;
}