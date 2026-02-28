using SaintsField.ComponentHeader;

namespace SaintsField.Samples.Scripts.HeaderComponentExample
{
    public class HeaderLabelExample : SaintsMonoBehaviour
    {
        [HeaderLeftLabel("Fixed Text")]
        [HeaderLabel]  // dynamic text
        public string label;
    }
}
