using System.Collections.Generic;
using System.Collections.ObjectModel;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	public delegate void DungenCharacterDelegate(DungenCharacter character);
	public delegate void CharacterTileChangedEvent(DungenCharacter character, Tile previousTile, Tile newTile);

	/// <summary>
	/// Contains information about the dungeon the character is in
	/// </summary>
	[AddComponentMenu("DunGen/Character")]
	public class DungenCharacter : MonoBehaviour
	{
		#region Statics

		public static event DungenCharacterDelegate CharacterAdded;
		public static event DungenCharacterDelegate CharacterRemoved;

		public static ReadOnlyCollection<DungenCharacter> AllCharacters { get; private set; }
		private static readonly List<DungenCharacter> allCharacters = new List<DungenCharacter>();

		static DungenCharacter()
		{
			DungenCharacter.AllCharacters = new ReadOnlyCollection<DungenCharacter>(DungenCharacter.allCharacters);
		}

		#endregion

		public Tile CurrentTile
		{
			get
			{
				if (this.overlappingTiles == null || this.overlappingTiles.Count == 0)
					return null;
				else
					return this.overlappingTiles[this.overlappingTiles.Count - 1];
			}
		}
		public event CharacterTileChangedEvent OnTileChanged;

		private List<Tile> overlappingTiles;


		protected virtual void OnEnable()
		{
			if (this.overlappingTiles == null)
				this.overlappingTiles = new List<Tile>();

			DungenCharacter.allCharacters.Add(this);

			if (DungenCharacter.CharacterAdded != null)
				DungenCharacter.CharacterAdded(this);
		}

		protected virtual void OnDisable()
		{
			DungenCharacter.allCharacters.Remove(this);

			if (DungenCharacter.CharacterRemoved != null)
				DungenCharacter.CharacterRemoved(this);
		}

		internal void ForceRecheckTile()
		{
			this.overlappingTiles.Clear();

			foreach (var tile in UnityUtil.FindObjectsByType<Tile>())
				if (tile.Placement.Bounds.Contains(this.transform.position))
				{
					this.OnTileEntered(tile);
					break;
				}
		}

		protected virtual void OnTileChangedEvent(Tile previousTile, Tile newTile) { }

		internal void OnTileEntered(Tile tile)
		{
			if (this.overlappingTiles.Contains(tile))
				return;

			var previousTile = this.CurrentTile;
			this.overlappingTiles.Add(tile);

			if (this.CurrentTile != previousTile)
			{
				this.OnTileChanged?.Invoke(this, previousTile, this.CurrentTile);
				this.OnTileChangedEvent(previousTile, this.CurrentTile);
			}
		}

		internal void OnTileExited(Tile tile)
		{
			if (!this.overlappingTiles.Contains(tile))
				return;

			var previousTile = this.CurrentTile;
			this.overlappingTiles.Remove(tile);

			if (this.CurrentTile != previousTile)
			{
				this.OnTileChanged?.Invoke(this, previousTile, this.CurrentTile);
				this.OnTileChangedEvent(previousTile, this.CurrentTile);
			}
		}
	}
}
