using Flexalon.Runtime.Core;
using UnityEngine;

namespace Flexalon.Samples.Runtime
{
    // Changes the material or text color of each child to create a gradient.
    [ExecuteAlways, AddComponentMenu("Flexalon Samples/Flexalon Color Gradient")]
    public class FlexalonColorGradient : MonoBehaviour
    {
        // First color of the gradient.
        [SerializeField]
        private Color _color1;
        public Color Color1
        {
            get => this._color1;
            set
            {
                this._color1 = value;
                this.UpdateColors(this._node);
            }
        }

        // Last color of the gradient.
        [SerializeField]
        private Color _color2;
        public Color Color2
        {
            get => this._color2;
            set
            {
                this._color2 = value;
                this.UpdateColors(this._node);
            }
        }

        // Should update colors when layout changes?
        [SerializeField]
        private bool _runOnLayoutChange;
        public bool RunOnLayoutChange
        {
            get => this._runOnLayoutChange;
            set
            {
                this._runOnLayoutChange = value;
                this.UpdateRunOnLayoutChange();
            }
        }

        private FlexalonNode _node;

        void OnEnable()
        {
            this._node = Flexalon.Runtime.Core.Flexalon.GetOrCreateNode(this.gameObject);
            this.UpdateRunOnLayoutChange();
            this.UpdateColors(this._node);
        }

        void UpdateRunOnLayoutChange()
        {
            this._node.ResultChanged -= this.UpdateColors;
            if (this._runOnLayoutChange)
            {
                this._node.ResultChanged += this.UpdateColors;
            }
        }

        void OnDisable()
        {
            this._node.ResultChanged -= this.UpdateColors;
        }

        private void UpdateColors(FlexalonNode node)
        {
            foreach (Transform child in this.transform)
            {
                var color = Color.Lerp(this._color1, this._color2, (float)(child.GetSiblingIndex()) / this.transform.childCount);
#if UNITY_TMPRO
                if (child.TryGetComponent<TMPro.TMP_Text>(out var text))
                {
                    text.color = color;
                } else
#endif
#if UNITY_UI
                if (child.TryGetComponent<UnityEngine.UI.Graphic>(out var graphic))
                {
                    graphic.color = color;
                } else
#endif
                if (child.TryGetComponent<FlexalonDynamicMaterial>(out var tdm))
                {
                    tdm.SetColor(color);
                }
            }
        }
    }
}