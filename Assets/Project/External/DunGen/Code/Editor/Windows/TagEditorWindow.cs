using DunGen.Project.External.DunGen.Code;
using DunGen.Project.External.DunGen.Code.Tags;
using UnityEditor;
using UnityEngine;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Windows
{
	public class TagEditorWindow : EditorWindow
	{
		#region Styles

		private sealed class Styles
		{
			public GUIStyle Header { get; private set; }
			public GUIStyle DeleteButton { get; private set; }


			public Styles()
			{
				this.Header = new GUIStyle(EditorStyles.boldLabel)
				{
					alignment = TextAnchor.LowerCenter
				};

				this.DeleteButton = new GUIStyle("IconButton")
				{
					alignment = TextAnchor.MiddleCenter
				};
			}
		}

		#endregion

		private const string ShowTagIDsPrefKey = "DunGen.Tags.ShowIDs";

		public bool ShowTagIDs
		{
			get { return EditorPrefs.GetBool(TagEditorWindow.ShowTagIDsPrefKey, false); }
			set { EditorPrefs.SetBool(TagEditorWindow.ShowTagIDsPrefKey, value); }
		}

		private TagManager tagManager;
		private int editTagID;
		private int tagToDelete;
		private bool hasTagIDChanged;
		private string newName;
		private Styles styles;
		private Vector2 scrollPosition;


		private void OnEnable()
		{
			this.tagManager = DunGenSettings.Instance.TagManager;
			this.minSize = new Vector2(250, 250);
			this.maxSize = new Vector2(600, 3000);

			this.editTagID = -1;
			this.tagToDelete = -1;
			this.hasTagIDChanged = false;
		}

		private void OnGUI()
		{
			if (this.styles == null)
				this.styles = new Styles();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(string.Format("-- Tags ({0}) --", this.tagManager.TagCount), this.styles.Header);
			this.ShowTagIDs = EditorGUILayout.Toggle(new GUIContent("Show IDs"), this.ShowTagIDs);
			EditorGUILayout.EndHorizontal();

			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

			var tags = this.tagManager.GetTagIDs();

			for (int i = 0; i < tags.Length; i++)
				this.DrawTag(tags[i], i);

			EditorGUILayout.EndScrollView();

			if(this.tagToDelete >= 0)
			{
				this.tagManager.RemoveTag(this.tagToDelete);
				this.tagToDelete = -1;

				this.ProcessChanges();
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if (GUILayout.Button(new GUIContent("+ Add Tag")))
			{
				int newTagID = this.tagManager.AddTag("New Tag");

				if (newTagID >= 0)
				{
					this.SetEditTagID(newTagID);
					this.ProcessChanges();
				}
			}
		}

		private void DrawTag(int tagId, int index)
		{
			var evt = Event.current;
			string tagName = this.tagManager.TryGetNameFromID(tagId);

			Color previousBackgroundColour = GUI.backgroundColor;

			GUI.backgroundColor = (index % 2 == 0) ? Color.white : Color.grey;
			EditorGUILayout.BeginHorizontal("box");
			GUI.backgroundColor = previousBackgroundColour;

			if (this.editTagID == tagId)
			{
				string inputName = "Tag " + tagId.ToString();
				bool wasEnterPressed = evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter);

				GUI.SetNextControlName(inputName);
				this.newName = EditorGUILayout.TextField(this.newName);

				var rect = GUILayoutUtility.GetLastRect();
				bool hasClickedOut = evt.type == EventType.MouseDown && !rect.Contains(evt.mousePosition);


				if (this.hasTagIDChanged)
				{
					EditorGUI.FocusTextInControl(inputName);
					this.hasTagIDChanged = false;
				}

				if (wasEnterPressed || hasClickedOut)
				{
					this.tagManager.TryRenameTag(tagId, this.newName);

					this.SetEditTagID(-1);
					this.ProcessChanges();
				}
			}
			else
			{
				string labelText;

				if (this.ShowTagIDs)
					labelText = string.Format("[{0}] {1}", tagId, tagName);
				else
					labelText = tagName;

				EditorGUILayout.LabelField(labelText);

				var rect = GUILayoutUtility.GetLastRect();

				if (evt.type == EventType.MouseDown)
				{
					if (rect.Contains(evt.mousePosition))
					{
						this.SetEditTagID(tagId);
						this.Repaint();
					}
				}
			}

			// Delete
			const float deleteButtonSize = 20;
			if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), this.styles.DeleteButton, GUILayout.Width(deleteButtonSize), GUILayout.Height(deleteButtonSize)))
			{
				if (EditorUtility.DisplayDialog("Delete Tag?", "Are you sure you want to delete this tag?", "Delete", "Cancel"))
				{
					this.tagToDelete = tagId;
					this.SetEditTagID(-1);
				}
			}

			EditorGUILayout.EndHorizontal();
		}

		private void SetEditTagID(int id)
		{
			this.editTagID = id;
			this.hasTagIDChanged = true;
			this.newName = this.tagManager.TryGetNameFromID(id);
		}

		private void ProcessChanges()
		{
			EditorUtility.SetDirty(DunGenSettings.Instance);
			this.Repaint();
		}

		#region Static Methods

		[MenuItem("Window/DunGen/Tags")]
		public static TagEditorWindow Open()
		{
			var window = EditorWindow.GetWindow<TagEditorWindow>(true, "DunGen Tags", true);
			window.Show();

			return window;
		}

		#endregion
	}
}
