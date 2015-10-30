namespace KerbalStory {
	using System;
	using UnityEngine;

	internal class StoryDialog {
		private static readonly String LOCK_ID = "9382013920";
		private static readonly Int32 WINDOW_ID = 280391;

		private String message;

		private KerbalInstructor instructor;
		private String instructorName;
		private Int32 instructorPortraitSize = 128;
		private Int32 textureBorderRadius = 124;
		private Rect instructorRect;
		private RenderTexture instructorTexture;

		private Rect windowPosition;
		private GUIStyle labelStyle;
		private GUIStyle scrollStyle;

		private Vector2 scrollPos = Vector2.zero;

		private Action callback;

		public static void ShowDialog(String instructorTypeText, String instructorName, String message, Action callback) {
			var instructorType = instructorTypeText.ToEnum<InstructorType>();
			new StoryDialog(instructorType, instructorName, message, callback);
		}

		private StoryDialog(InstructorType instractorType, String instructorName, String message, Action callback) {
			this.instructorName = instructorName;
			this.message = message;
			this.callback = callback;

			var windowWidth = Math.Max(600, Screen.width / 2);
			var windowHeight = Math.Max(500, Screen.height / 2);
			this.windowPosition = new Rect((Screen.width / 2) - (windowWidth / 2), (Screen.height / 2) - (windowHeight / 2), windowWidth, windowHeight);

			this.labelStyle = new GUIStyle(HighLogic.Skin.label) {
				alignment = TextAnchor.MiddleCenter,
			};
			this.scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);
			this.instructor = this.GetInstructor(instractorType);
			this.instructorTexture = new RenderTexture(this.instructorPortraitSize, this.instructorPortraitSize, 8);
			this.instructor.instructorCamera.targetTexture = this.instructorTexture;
			this.instructor.instructorCamera.ResetAspect();

			InputLockManager.SetControlLock(LOCK_ID);
			RenderingManager.AddToPostDrawQueue(144, this.OnDraw);
		}

		private void Dispose() {
			InputLockManager.RemoveControlLock(LOCK_ID);
			RenderingManager.RemoveFromPostDrawQueue(144, this.OnDraw);
			UnityEngine.Object.Destroy(this.instructor.gameObject);
		}

		private void OnDraw() {
			this.windowPosition = KSPUtil.ClampRectToScreen(GUILayout.Window(WINDOW_ID, this.windowPosition, this.OnWindowDraw, String.Empty));
		}

		private void OnWindowDraw(Int32 windowId) {
			var charaRectSize = this.instructorPortraitSize;
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical(GUILayout.Width(charaRectSize));
				{
					GUILayout.Box(String.Empty, GUILayout.Width(charaRectSize), GUILayout.Height(charaRectSize));
					if (Event.current.type == EventType.Repaint) {
						var rect = GUILayoutUtility.GetLastRect();
						this.instructorRect = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);
						Graphics.DrawTexture(this.instructorRect, this.instructorTexture, new Rect(0f, 0f, 1f, 1f), this.textureBorderRadius, this.textureBorderRadius, this.textureBorderRadius, this.textureBorderRadius, Color.white, this.instructor.PortraitRenderMaterial);
					}

					var instructorName = this.instructorName ?? this.instructor.CharacterName;
					GUILayout.Label(instructorName, this.labelStyle, GUILayout.Width(charaRectSize));
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, this.scrollStyle);
					{
						GUILayout.Label(this.message);
					}
					GUILayout.EndScrollView();
					if (GUILayout.Button("OK", GUILayout.ExpandWidth(true))) {
						if (this.callback != null) {
							this.callback();
						}
						this.Dispose();
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		public KerbalInstructor GetInstructor(InstructorType type) {
			var name = String.Format("Instructor_{0}", type.ToString());
			var gameObject = (GameObject)UnityEngine.Object.Instantiate(AssetBase.GetPrefab(name));
			return gameObject.GetComponent<KerbalInstructor>();
		}

		public enum InstructorType {
			Gene, Wernher,
		}
	}
}
