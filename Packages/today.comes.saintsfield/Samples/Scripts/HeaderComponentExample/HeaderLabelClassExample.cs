using SaintsField.ComponentHeader;

namespace SaintsField.Samples.Scripts.HeaderComponentExample
{
    [HeaderLabel("$" + nameof(value))]
    [HeaderLeftLabel("dynamic:")]
    public class HeaderLabelClassExample : SaintsMonoBehaviour
    {
        public string value;
    }
}
