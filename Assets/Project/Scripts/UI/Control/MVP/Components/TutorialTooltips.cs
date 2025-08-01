using System;
using System.Collections.Generic;
using Project.Scripts.Common;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Components;

public class TutorialTooltips : MonoBehaviour {
    private GameObject? CurrentTooltip { get; set; }
    private Queue<GameObject> Tooltips { get; set; } = [];
    [field: SerializeField] private float DisplayDuration { get; set; } = 5;
    private float ExpireTime { get; set; }
    
    private void Awake() {
        foreach (Transform child in this.transform) {
            this.Tooltips.Enqueue(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    private void Start() {
        GameEvents.UI.OnNextTutorial += this.NextTutorial;
    }

    private void OnDestroy() {
        GameEvents.UI.OnNextTutorial -= this.NextTutorial;   
    }

    public void NextTutorial() {
        if (this.Tooltips.Count == 0) {
            return;
        }
        
        this.CloseTooltip();
        this.CurrentTooltip = this.Tooltips.Dequeue();
        this.CurrentTooltip.gameObject.SetActive(true);
        this.ExpireTime = Time.time + this.DisplayDuration;
    }

    private void Update() {
        if (!this.CurrentTooltip || !(Time.time > this.ExpireTime)) {
            return;
        }

        this.CloseTooltip();
        this.CurrentTooltip = null;
    }
    
    private void CloseTooltip() {
        GameObject? curr = this.CurrentTooltip;
        if (curr && curr.activeInHierarchy) {
            LeanTween.alphaCanvas(curr.GetComponent<CanvasGroup>(), 0, 0.5f)
                     .setOnComplete(() => {
                         curr.SetActive(false);
                     });
        } 
    }
}
