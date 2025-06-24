#if UNITY_TMPRO

using System;
using System.Collections.Generic;
using Flexalon.Runtime.Cloner;
using TMPro;
using UnityEngine;

namespace Flexalon.Samples.Runtime
{
    // Provides the text of an TMP_InputField as a data source which can be assigned to a FlexalonCloner.
    [AddComponentMenu("Flexalon Samples/Input Field Data Source")]
    public class InputFieldDataSource : MonoBehaviour, DataSource
    {
        [SerializeField]
        private TMP_InputField _inputField;
        public TMP_InputField InputField
        {
            get => this._inputField;
            set
            {
                this._inputField = value;
                this.UpdateData(this._inputField.text);
            }
        }

        public event Action DataChanged;

        private List<string> _data = new List<string>();
        public IReadOnlyList<object> Data => this._data;

        void OnEnable()
        {
            this._inputField.onValueChanged.AddListener(this.UpdateData);
            this.UpdateData(this._inputField.text);
        }

        void OnDisable()
        {
            this._inputField.onValueChanged.RemoveListener(this.UpdateData);
        }

        private void UpdateData(string text)
        {
            this._data.Clear();
            foreach (char c in text)
            {
                this._data.Add(c.ToString());
            }

            this.DataChanged?.Invoke();
        }
    }
}

#endif