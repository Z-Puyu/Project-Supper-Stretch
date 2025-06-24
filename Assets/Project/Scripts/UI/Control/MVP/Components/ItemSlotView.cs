using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Editor;
using Project.Scripts.Common;
using Project.Scripts.Items;
using Project.Scripts.Items.Definitions;
using Project.Scripts.UI.Control.MVP.Interfaces;
using SaintsField;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control.MVP.Components;

public class ItemSlotView : UIView {
    [NotNull] [field: SerializeField] private TextMeshProUGUI? Nameplate { get; set; }
    public string ItemName { private get; set; } = string.Empty;
    
    public bool IsEmpty => this.ItemName == string.Empty;
    
    public override void Refresh() {
        this.Nameplate.text = this.ItemName;
    }
    
    public override void Clear() {
        this.ItemName = string.Empty;
    }
}
