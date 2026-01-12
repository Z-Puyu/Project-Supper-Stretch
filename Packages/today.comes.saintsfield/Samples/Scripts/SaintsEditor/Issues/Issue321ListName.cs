using System;

namespace SaintsField.Samples.Scripts.SaintsEditor.Issues
{
    public class Issue321ListName : SaintsMonoBehaviour
    {
        [Serializable]
        public struct TestStruct
        {
            public string name;
            public int value;
        }

        public TestStruct[] fixedInNewVersion;

        [FieldLabelText("<field.name/>")]
        public TestStruct[] workaroundForOldVersion;
    }
}
