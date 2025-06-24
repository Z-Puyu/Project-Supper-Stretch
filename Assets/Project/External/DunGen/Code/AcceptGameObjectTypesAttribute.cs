using System;
using UnityEngine;

namespace DunGen.Project.External.DunGen.Code
{
	public enum GameObjectFilter
	{
		Scene = 1,
		Asset = 2,

		All = GameObjectFilter.Scene | GameObjectFilter.Asset,
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class AcceptGameObjectTypesAttribute : PropertyAttribute
	{
		public GameObjectFilter Filter { get; private set; }


		public AcceptGameObjectTypesAttribute(GameObjectFilter filter)
		{
			this.Filter = filter;
		}
	}
}
