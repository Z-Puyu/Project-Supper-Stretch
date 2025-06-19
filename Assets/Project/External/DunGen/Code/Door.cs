using System;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	[Serializable]
	public class Door : MonoBehaviour
	{
		public delegate void DoorStateChangedDelegate(Door door, bool isOpen);

		[HideInInspector]
		public Dungeon Dungeon;
		[HideInInspector]
		public Doorway DoorwayA;
		[HideInInspector]
		public Doorway DoorwayB;
		[HideInInspector]
		public Tile TileA;
		[HideInInspector]
		public Tile TileB;

		public bool DontCullBehind
		{
			get { return this.dontCullBehind; }
			set
			{
				if (this.dontCullBehind == value)
					return;

				this.dontCullBehind = value;
				this.SetDoorState(this.isOpen);
			}
		}

		public bool ShouldCullBehind
		{
			get
			{
				if (this.DontCullBehind)
					return false;

				return !this.isOpen;
			}
		}
		public virtual bool IsOpen
		{
			get { return this.isOpen; }
			set
			{
				if (this.isOpen == value)
					return;

				this.SetDoorState(value);
			}
		}

		public event DoorStateChangedDelegate OnDoorStateChanged;


		[SerializeField]
		private bool dontCullBehind;
		[SerializeField]
		private bool isOpen = true;


		private void OnDestroy()
		{
			this.OnDoorStateChanged = null;
		}

		public void SetDoorState(bool isOpen)
		{
			this.isOpen = isOpen;

			if (this.OnDoorStateChanged != null)
				this.OnDoorStateChanged(this, isOpen);
		}
	}
}
