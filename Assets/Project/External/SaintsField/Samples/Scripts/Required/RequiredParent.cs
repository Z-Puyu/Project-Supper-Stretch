using UnityEngine;

namespace SaintsField.Samples.Scripts.Required
{
    public class RequiredParent : MonoBehaviour
    {
        [SerializeField, Required] protected GameObject _gameObject;
    }
}
