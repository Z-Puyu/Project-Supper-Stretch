//Example comes from https://bitbucket.org/catlikecodingunitytutorials/basics-02-building-a-graph/src/master/

using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Project.External.FastScriptReload.Samples.Basic.Scripts
{
	public class Graph : MonoBehaviour {

		[SerializeField]
		Transform pointPrefab;

		[SerializeField, Range(10, 100)]
		int resolution = 10;

		[SerializeField]
		FunctionLibrary.FunctionName function;
		
		Transform[] points;
		
		/*  EXPERIMENTAL: Add new fields at runtime (editor rendered)
		 *  To enable feature please go to Fast Script Reload -> Start Screen -> New Fields -> Enable experimental added field support
		 *  Then:
		 *  1) hit play
		 *  2) uncomment '_testUMove' variable
		 *  3) uncomment usage of experimental variable in Update() method
		 *
		 *  Variable will only show in editor when first read.
		 *
		 *	PLEASE READ LIMITATIONS AROUND NEW FIELDS FEATURE - this feature is bit more involved and at this stage please expect it to break more often
		 *  If you could report any issues via Discord that'd be great and will help make the tool better! Thanks!
		 */
		
		// [SerializeField] [Range(-3, 3)] private float _testUMove = 0f; //EXPERIMENTAL: Add new field example

		void Awake ()
		{
			var pointsHolderGo = new GameObject("PointsHolder");
		
			float step = 2f / this.resolution;
			var scale = Vector3.one * step;
			this.points = new Transform[this.resolution * this.resolution];
			for (int i = 0; i < this.points.Length; i++) {
				Transform point = this.points[i] = Object.Instantiate(this.pointPrefab);
				point.localScale = scale;
				point.SetParent(pointsHolderGo.transform, false);
			}
		}
		

		void Update() 
		{
			var f = FunctionLibrary.GetFunction(this.function);
			var time = Time.time;
			var step = 2f / this.resolution;
			var v = 0.5f * step - 1f;
			for (int i = 0, x = 0, z = 0; i < this.points.Length; i++, x++)
			{
				if (x == this.resolution)
				{
					x = 0;
					z += 1;
					v = (z + 0.5f) * step - 1f;
				}

				var u = (x + 0.5f) * step - 1f;
				
				this.points[i].localPosition = f(u, v, time);
				
				// EXPERIMENTAL: add new fields flow - uncomment below line to see changes
				// points[i].localPosition = f(u + _testUMove, v, time); 
			}
		}

		void OnScriptHotReload()
		{
			Debug.Log($"Script 'Graph.cs' was changed and hot reloaded, you have access to instance via 'this', eg: {nameof(this.resolution)} value is: {this.resolution}"); 
		}
		
		static void OnScriptHotReloadNoInstance()
		{
			Debug.Log("Script 'Graph.cs' was changed - this method is executed even without any instance in the scene. There's no access to 'this'. " +
			          "Useful if you just added a type / need to perform some one-off init"); 
		}
	}
}

// Dynamically adding new types, OnScriptHotReloadNewTypeAdded will trigger and allow setup, re-add for test
 // public class NewMonoBehaviourTest : MonoBehaviour
 // {
 // 	static void OnScriptHotReloadNewTypeAdded()
 // 	{
 // 		var go = new GameObject("TestDynamic");
 // 		go.AddComponent<NewMonoBehaviourTest>();
 // 	}
 //
 // 	void Start()
 // 	{
 // 		Debug.Log("Start - NewMonoBehaviourTest");
 // 		GameObject.CreatePrimitive(PrimitiveType.Cube);
 // 	}
 //
 // 	private void Update()
 // 	{
 // 		Debug.Log("test");
 // 	}
 //
 // 	void OnScriptHotReload()
 // 	{
 // 		Debug.Log("Script hot reloaded -NewMonoBehaviourTest"); 
 // 	}
 // }