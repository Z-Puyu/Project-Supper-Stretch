using System.Collections.Generic;
using DunGen.Project.External.DunGen.Code.Utility;
using UnityEngine;

namespace Project.External.DunGen.Samples.Basic.Scripts
{
	public class ScreenText : MonoBehaviour
	{
		#region Helper Class

		private sealed class ScreenTextData
		{
			public string Text;
			public float Timer;
		}

		#endregion

		public GUIStyle Style = new GUIStyle();
		public float MessageFadeTime = 5;

		private readonly List<ScreenTextData> messages = new List<ScreenTextData>();


		public void AddMessage(string message)
		{
			this.messages.Add(new ScreenTextData() { Text = message, Timer = this.MessageFadeTime });
		}

		private void Update()
		{
			for (int i = this.messages.Count - 1; i >= 0; i--)
			{
				var message = this.messages[i];

				if (message.Timer > 0)
				{
					message.Timer -= Time.deltaTime;

					if (message.Timer <= 0)
						this.messages.RemoveAt(i);
				}
			}
		}

		private void OnGUI()
		{
			Vector2 bottomRight = GUIUtility.ScreenToGUIPoint(new Vector2(Screen.width, Screen.height));
			float bufferSize = 5;

			bottomRight -= new Vector2(bufferSize, bufferSize);

			for (int i = this.messages.Count - 1; i >= 0; i--)
			{
				var msg = this.messages[i];

				GUIContent content = new GUIContent(msg.Text);
				Vector2 stringSize = GUI.skin.label.CalcSize(content);

				GUI.Label(new Rect(bottomRight.x - stringSize.x, bottomRight.y - stringSize.y, stringSize.x, stringSize.y), content, this.Style);
				bottomRight -= new Vector2(0, stringSize.y + bufferSize);
			}
		}


		#region Static Methods

		public static void Log(object obj)
		{
			ScreenText.Log(obj.ToString());
		}

		public static void Log(string format, params object[] args)
		{
			var component = UnityUtil.FindObjectByType<ScreenText>();

			if (component == null)
				return;

			component.AddMessage(string.Format(format, args));
		}

		#endregion
	}
}