#if UNITY_TMPRO

using Flexalon.Runtime.Cloner;
using TMPro;
using UnityEngine;

namespace Flexalon.Samples.Runtime
{
    // Implements DataBinding by binding a string to a TMP_Text.
    [DisallowMultipleComponent, AddComponentMenu("Flexalon Samples/Text Data Binding")]
    public class TextDataBinding : MonoBehaviour, DataBinding
    {
        private TMP_Text _text;

        void OnEnable()
        {
            this._text = this.GetComponentInChildren<TMP_Text>();
        }

        public void SetData(object data)
        {
            this._text.text = (string) data;
        }
    }
}

#endif