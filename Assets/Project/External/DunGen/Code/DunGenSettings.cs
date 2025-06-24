
using System.IO;
using System.Linq;
using DunGen.Project.External.DunGen.Code.Collision;
using DunGen.Project.External.DunGen.Code.Tags;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEditor;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	public sealed class DunGenSettings : ScriptableObject
	{
		#region Singleton

		private static DunGenSettings instance;
		public static DunGenSettings Instance
		{
			get
			{
				if (DunGenSettings.instance != null)
					return DunGenSettings.instance;
				else
				{
					DunGenSettings.instance = DunGenSettings.FindOrCreateInstanceAsset();
					return DunGenSettings.instance;
				}
			}
		}


		public static DunGenSettings FindOrCreateInstanceAsset()
		{
			// Try to find an existing instance in a resource folder
			DunGenSettings.instance = Resources.Load<DunGenSettings>("DunGen Settings");

			// Create a new instance if one is not found
			if (DunGenSettings.instance == null)
			{
#if UNITY_EDITOR
				DunGenSettings.instance = ScriptableObject.CreateInstance<DunGenSettings>();

				if (!Directory.Exists(Application.dataPath + "/Resources"))
					AssetDatabase.CreateFolder("Assets", "Resources");

				AssetDatabase.CreateAsset(DunGenSettings.instance, "Assets/Resources/DunGen Settings.asset");
				DunGenSettings.instance.defaultSocket = DunGenSettings.instance.GetOrAddSocketByName("Default");
#else
				throw new System.Exception("No instance of DunGen settings was found.");
#endif
			}

			return DunGenSettings.instance;
		}

		#endregion

		public DoorwaySocket DefaultSocket { get { return this.defaultSocket; } }
		public TagManager TagManager { get { return this.tagManager; } }

		/// <summary>
		/// Optional broadphase settings for speeding up collision tests
		/// </summary>
		[SubclassSelector]
		[SerializeReference]
		public BroadphaseSettings BroadphaseSettings = new SpatialHashBroadphaseSettings();

		/// <summary>
		/// If true, sprite components will be ignored when automatically calculating bounding volumes around tiles
		/// </summary>
		public bool BoundsCalculationsIgnoreSprites = false;
		/// <summary>
		/// If true, tile bounds will be automatically recalculated whenever a tile is saved. Otherwise, bounds must be recalculated manually using the button in the Tile inspector
		/// </summary>
		public bool RecalculateTileBoundsOnSave = true;
		/// <summary>
		/// If true, tile instances will be re-used instead of destroyed and re-created, improving generation performance at the cost of increased memory consumption
		/// </summary>
		public bool EnableTilePooling = false;
		/// <summary>
		/// If true, a window will be displayed when a generation failure occurs, allowing you to inspect the failure report
		/// </summary>
		public bool DisplayFailureReportWindow = true;
		/// <summary>
		/// Should the DunGen folder be checked for files that are no longer in use? If true, when loading DunGen will check if any old files are still present in the DunGen directory from previous DunGen version and will present the user with a list of files to potentially delete
		/// </summary>
		public bool CheckForUnusedFiles = true;

		[SerializeField]
		private DoorwaySocket defaultSocket = null;

		[SerializeField]
		private TagManager tagManager = new TagManager();


		private void OnValidate()
		{
			if(this.defaultSocket == null)
				this.defaultSocket = this.GetOrAddSocketByName("Default");
		}

#if UNITY_EDITOR
		private DoorwaySocket GetOrAddSocketByName(string name)
		{
			string path = AssetDatabase.GetAssetPath(this);

			var socket = AssetDatabase.LoadAllAssetsAtPath(path)
				.OfType<DoorwaySocket>()
				.FirstOrDefault(x => x.name == name);

			if (socket != null)
				return socket;

			socket = ScriptableObject.CreateInstance<DoorwaySocket>();
			socket.name = name;

			AssetDatabase.AddObjectToAsset(socket, this);

#if UNITY_2021_1_OR_NEWER
			AssetDatabase.SaveAssetIfDirty(socket);
#else
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(socket));
#endif

			return socket;
		}
#endif
	}
}
