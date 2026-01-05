using SaintsField.ComponentHeader;
using SaintsField.Playa;

namespace SaintsField.Samples.Scripts.HeaderComponentExample.Issues
{
    public class Issue258ColorOriginalName : SaintsMonoBehaviour
    {
        [ShowInInspector][HeaderLabel("<color=red><field/>")]
        private static int _num = 10;

        [Button]
        private void Incr()
        {
            _num = (_num + 1) % 5;
        }
    }
}
