using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class Tooltip : MonoBehaviour {
    [NotNull] [field: SerializeField] private TextMeshProUGUI? Header { get; set; }
    [NotNull] [field: SerializeField] private TextMeshProUGUI? Text { get; set; } 
    [NotNull] [field: SerializeField] private LayoutElement? LayoutElement { get; set; }
    [field: SerializeField] private int WrapLimit { get; set; } = 80;
    [NotNull] private RectTransform? RectTransform { get; set; }

    private void Awake() {
        this.RectTransform = this.GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = "") {
        if (string.IsNullOrWhiteSpace(header) || string.IsNullOrEmpty(content)) {
            this.Header.gameObject.SetActive(false);
        } else {
            this.Header.gameObject.SetActive(true);
            this.Header.text = header;
        }
        
        this.Text.text = content;
        int length = Mathf.Max(this.Header.text.Length, this.Text.text.Length);
        this.LayoutElement.enabled = length > this.WrapLimit;
    }
    
    private void Update() {
        Vector2 mousePosition = Input.mousePosition;
        float pivotX = mousePosition.x / Screen.width;
        float pivotY = mousePosition.y / Screen.height;
        this.RectTransform.pivot = new Vector2(pivotX, pivotY);
        this.transform.position = Input.mousePosition;
    }
}
