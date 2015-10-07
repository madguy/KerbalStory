namespace KerbalStory {
	using System;
	using UnityEngine;

	internal class StoryDialog {
		public static readonly String LOCK_ID = "9382013920";

		public String Message { get; set; }
		public Int32 Width { get; set; }
		public Int32 Height { get; set; }

		public event Action OnClick;

		private KerbalInstructor instructor;
		private Int32 instructorPortraitSize = 128;
		private Int32 textureBorderRadius = 124;
		private Rect instructorRect;
		private RenderTexture instructorTexture;

		private Rect windowPosition = new Rect(300, 60, 600, 400);
		private GUIStyle windowStyle;
		private GUIStyle labelStyle = new GUIStyle(HighLogic.Skin.label);
		private GUIStyle buttonStyle = new GUIStyle(HighLogic.Skin.button);
		private GUIStyle scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);
		private Vector2 scrollPos = Vector2.zero;
		private Boolean isVisible = false;

		public StoryDialog() {
			this.windowStyle = new GUIStyle(HighLogic.Skin.window);
			this.windowStyle.fixedWidth = 600f;
			this.windowStyle.fixedHeight = 400f;
			this.Width = 580;
			this.Height = 350;

			this.instructor = Instructors.Wernher;
			this.instructorTexture = new RenderTexture(this.instructorPortraitSize, this.instructorPortraitSize, 8);
			this.instructor.instructorCamera.targetTexture = this.instructorTexture;
		}

		public void Show() {
			if (this.isVisible == true) {
				return;
			}

			InputLockManager.SetControlLock(LOCK_ID);
			RenderingManager.AddToPostDrawQueue(144, OnDraw);
			this.isVisible = true;
		}

		public void Hide() {
			if (this.isVisible == false) {
				return;
			}

			InputLockManager.RemoveControlLock(LOCK_ID);
			RenderingManager.RemoveFromPostDrawQueue(144, OnDraw);
			this.isVisible = false;
		}

		private void OnDraw() {
			this.windowPosition = GUILayout.Window(11, windowPosition, OnWindow, "Kerbal Story", windowStyle);
		}

		private void OnWindow(Int32 windowId) {
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

					GUILayout.Label(this.instructor.CharacterName, this.labelStyle, GUILayout.Width(charaRectSize));
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, this.scrollStyle);
					{
						GUILayout.Label(this.Message);
					}
					GUILayout.EndScrollView();
					if (GUILayout.Button("OK", GUILayout.ExpandWidth(true))) {
						if (this.OnClick != null) {
							this.OnClick();
						}
						this.Hide();
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		private class Instructors {
			public static KerbalInstructor Gene { get { return GetInstructor("Instructor_Gene"); } }
			public static KerbalInstructor Wernher { get { return GetInstructor("Instructor_Wernher"); } }

			private static KerbalInstructor GetInstructor(String name) {
				var gameObject = (GameObject)UnityEngine.Object.Instantiate(AssetBase.GetPrefab(name));
				return gameObject.GetComponent<KerbalInstructor>();
			}
		}
	}
}
