namespace KerbalStory {
	using System;
	using UnityEngine;

	internal class StoryDialog {
		public static readonly String LOCK_ID = "9382013920";
		private static readonly Int32 WINDOW_ID = 280391;

		public String Title { get; set; }

		public String Message { get; set; }

		private KerbalInstructor instructor;

		public KerbalInstructor Instructor {
			get {
				return this.instructor;
			}
			set {
				this.instructor = value;
				if (value == null) {
					return;
				}

				this.instructorTexture = new RenderTexture(this.instructorPortraitSize, this.instructorPortraitSize, 8);
				this.instructor.instructorCamera.targetTexture = this.instructorTexture;
				this.instructor.instructorCamera.ResetAspect();
			}
		}

		public String InstructorName { get; set; }

		public event Action OnClick;

		private Int32 instructorPortraitSize = 128;
		private Int32 textureBorderRadius = 124;
		private Rect instructorRect;
		private RenderTexture instructorTexture;

		private Rect windowPosition = new Rect(300f, 150f, 600f, 500f);
		private GUIStyle windowStyle;
		private GUIStyle labelStyle;
		private GUIStyle scrollStyle;

		private Vector2 scrollPos = Vector2.zero;
		private Boolean isVisible = false;

		public StoryDialog() {
			this.windowStyle = new GUIStyle() {
				fixedWidth = 400f,
				fixedHeight = 600f,
			};
			this.labelStyle = new GUIStyle(HighLogic.Skin.label) {
				alignment = TextAnchor.MiddleCenter,
			};
			this.scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);
			this.Instructor = Instructors.Wernher;
		}

		~StoryDialog() {
			this.windowStyle = null;
			this.labelStyle = null;
			this.scrollStyle = null;
			this.instructor = null;
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
			this.windowPosition = KSPUtil.ClampRectToScreen(GUILayout.Window(WINDOW_ID, this.windowPosition, this.OnWindowDraw, this.Title));
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
						Graphics.DrawTexture(this.instructorRect, this.instructorTexture, new Rect(0f, 0f, 1f, 1f), this.textureBorderRadius, this.textureBorderRadius, this.textureBorderRadius, this.textureBorderRadius, Color.white, this.Instructor.PortraitRenderMaterial);
					}

					var instructorName = this.InstructorName ?? this.Instructor.CharacterName;
					GUILayout.Label(instructorName, this.labelStyle, GUILayout.Width(charaRectSize));
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

		public class Instructors {
			public static KerbalInstructor Gene { get { return GetInstructor("Instructor_Gene"); } }
			public static KerbalInstructor Wernher { get { return GetInstructor("Instructor_Wernher"); } }

			private static KerbalInstructor GetInstructor(String name) {
				var gameObject = (GameObject)UnityEngine.Object.Instantiate(AssetBase.GetPrefab(name));
				return gameObject.GetComponent<KerbalInstructor>();
			}
		}
	}
}
