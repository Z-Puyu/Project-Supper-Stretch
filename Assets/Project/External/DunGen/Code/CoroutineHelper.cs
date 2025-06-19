using System.Collections;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	public sealed class CoroutineHelper : MonoBehaviour
	{
		private static CoroutineHelper instance;
		private static CoroutineHelper Instance
		{
			get
			{
				if (CoroutineHelper.instance == null)
				{
					var obj = new GameObject("DunGen Coroutine Helper");
					obj.hideFlags = HideFlags.HideInHierarchy;

					CoroutineHelper.instance = obj.AddComponent<CoroutineHelper>();
				}

				return CoroutineHelper.instance;
			}
		}


		public static Coroutine Start(IEnumerator routine)
		{
			return CoroutineHelper.Instance.StartCoroutine(routine);
		}

		public static void StopAll()
		{
			CoroutineHelper.Instance.StopAllCoroutines();
		}
	}
}
