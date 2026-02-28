using System.Collections.Generic;
using UnityEngine;

namespace SaintsField.Samples.Scripts.IssueAndTesting.Issue.Issue102
{
    public class Issue102 : SaintsMonoBehaviour
    {
        [SerializeField, GetComponentInChildren(false, typeof(MudCurvePoint), true)]
        private List<Transform> mPoints = new List<Transform>();
    }
}
