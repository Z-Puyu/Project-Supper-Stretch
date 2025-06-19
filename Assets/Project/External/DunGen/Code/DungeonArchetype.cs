using System;
using System.Collections.Generic;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	/// <summary>
	/// A description of the layout of a dungeon
	/// </summary>
	[Serializable]
	[CreateAssetMenu(fileName = "New Archetype", menuName = "DunGen/Dungeon Archetype", order = 700)]
	public sealed class DungeonArchetype : ScriptableObject, ISerializationCallbackReceiver
	{
		#region Legacy Properties

		[Obsolete("StraightenChance is deprecated. Use StraighteningSettings instead")]
		public float StraightenChance = 0.0f;

		#endregion

		public static int CurrentFileVersion = 1;

		/// <summary>
		/// A collection of tile sets from which rooms will be selected to fill the dungeon
		/// </summary>
		public List<TileSet> TileSets = new List<TileSet>();
		/// <summary>
		/// A collection of tile sets that can be used at the start of branch paths
		/// </summary>
		public List<TileSet> BranchStartTileSets = new List<TileSet>();
		/// <summary>
		/// Defines how the TileSets and BranchStartTileSets are used when placing rooms at the beginning of a branch
		/// </summary>
		public BranchCapType BranchStartType = BranchCapType.InsteadOf;
		/// <summary>
		/// A collection of tile sets that can be used at the end of branch paths
		/// </summary>
		public List<TileSet> BranchCapTileSets = new List<TileSet>();
		/// <summary>
		/// Defines how the TileSets and BranchEndTileSets are used when placing rooms at the end of a branch
		/// </summary>
		public BranchCapType BranchCapType = BranchCapType.AsWellAs;
		/// <summary>
		/// The maximum depth (in tiles) that any branch in the dungeon can be
		/// </summary>
		public IntRange BranchingDepth = new IntRange(2, 4);
		/// <summary>
		/// The maximum number of branches each room can have
		/// </summary>
		public IntRange BranchCount = new IntRange(0, 2);
		/// <summary>
		/// The chance that this archetype will produce a straight section for the main path
		/// </summary>
		public PathStraighteningSettings StraighteningSettings = new PathStraighteningSettings();
		/// <summary>
		/// Should DunGen attempt to prevent this archetype from appearing more than once throughout the dungeon layout?
		/// </summary>
		public bool Unique = false;

		[SerializeField]
		private int fileVersion = 0;


		public bool GetHasValidBranchStartTiles()
		{
			if (this.BranchStartTileSets.Count == 0)
				return false;

			foreach (var tileSet in this.BranchStartTileSets)
				if (tileSet.TileWeights.Weights.Count > 0)
					return true;

			return false;
		}

		public bool GetHasValidBranchCapTiles()
		{
			if (this.BranchCapTileSets.Count == 0)
				return false;

			foreach (var tileSet in this.BranchCapTileSets)
				if (tileSet.TileWeights.Weights.Count > 0)
					return true;

			return false;
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this == null)
				return;

			bool isDirty = false;

			// Upgrade to StraighteningSettings
			if (this.fileVersion < 1)
			{
				if (this.StraighteningSettings == null)
					this.StraighteningSettings = new PathStraighteningSettings();

#pragma warning disable 0618
				if (this.StraightenChance > 0.0f)
				{
					this.StraighteningSettings.StraightenChance = Mathf.Clamp01(this.StraightenChance);
					this.StraighteningSettings.OverrideStraightenChance = true;
				}
#pragma warning restore 0618

				this.fileVersion = 1;
				isDirty = true;
			}

#if UNITY_EDITOR
			// Schedule to mark dirty & save
			if (isDirty)
			{
				UnityEditor.EditorApplication.delayCall += () =>
				{
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
				};
			}
#endif
		}

		#endregion
	}

	public enum BranchCapType : byte
	{
		InsteadOf,
		AsWellAs,
	}
}
