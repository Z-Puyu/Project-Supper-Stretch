using DunGen.Editor.Project.External.DunGen.Code.Editor.Utility;
using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Windows
{
	public sealed class DungeonGeneratorWindow : EditorWindow
	{
		private class SerializedDungeonGeneratorContainer : ScriptableObject
		{
			public DungeonGenerator generator;
		}

		private SerializedDungeonGeneratorContainer container;
		private SerializedObject serializedObject;
		private SerializedProperty generatorProperty;
		private GameObject lastDungeon;
		private bool overwriteExisting = true;

		[MenuItem("Window/DunGen/Generate Dungeon")]
		private static void OpenWindow()
		{
			EditorWindow.GetWindow<DungeonGeneratorWindow>(false, "New Dungeon", true);
		}

		private void OnGUI()
		{
			if (this.serializedObject == null)
				return;

			this.serializedObject.Update();
			DungeonGeneratorDrawUtil.DrawDungeonGenerator(this.generatorProperty, false);
			this.serializedObject.ApplyModifiedProperties();

			EditorGUILayout.Space();

			this.overwriteExisting = EditorGUILayout.Toggle("Overwrite Existing?", this.overwriteExisting);

			if (GUILayout.Button("Generate"))
				this.GenerateDungeon();
		}

		private void OnEnable()
		{
			// Create a container object to hold our generator
			this.container = ScriptableObject.CreateInstance<SerializedDungeonGeneratorContainer>();
			this.container.generator = new DungeonGenerator
			{
				AllowTilePooling = false
			};

			// Setup serialization
			this.serializedObject = new SerializedObject(this.container);
			this.generatorProperty = this.serializedObject.FindProperty("generator");

			this.container.generator.OnGenerationStatusChanged += this.HandleGenerationStatusChanged;
		}

		private void OnDisable()
		{
			if (this.container != null && this.container.generator != null)
				this.container.generator.OnGenerationStatusChanged -= this.HandleGenerationStatusChanged;

			if (this.container != null)
				Object.DestroyImmediate(this.container);

			this.container = null;
			this.serializedObject = null;
			this.generatorProperty = null;
		}

		private void GenerateDungeon()
		{
			if (this.lastDungeon != null)
			{
				if (this.overwriteExisting)
					UnityUtil.Destroy(this.lastDungeon);
				else
					this.container.generator.DetachDungeon();
			}

			this.lastDungeon = new GameObject("Dungeon Layout");
			this.container.generator.Root = this.lastDungeon;

			Undo.RegisterCreatedObjectUndo(this.lastDungeon, "Create Procedural Dungeon");
			this.container.generator.GenerateAsynchronously = false;
			this.container.generator.Generate();
		}

		private void HandleGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			if (status == GenerationStatus.Failed)
			{
				UnityUtil.Destroy(this.lastDungeon);
				this.lastDungeon = this.container.generator.Root = null;
			}
		}
	}
}