namespace KerbalStory {
	using System;
	using UnityEngine;

	internal class ConfirmDialog {
		public static readonly String LOCK_ID = "4901858818";
		private static readonly Int32 WINDOW_ID = 714199;

		private String title = "確認";
		private String message;

		private Action onSuccess;
		private Action onCancel;

		private Rect windowPosition = new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 50, 400, 100);
		private GUIStyle windowStyle;
		private GUIStyle labelStyle;

		public static void ShowDialog(String message, Action onSuccess, Action onCancel) {
			new ConfirmDialog() {
				message = message,
				onSuccess = onSuccess,
				onCancel = onCancel,
			};
		}

		private ConfirmDialog() {
			this.windowStyle = new GUIStyle(HighLogic.Skin.window);
			this.labelStyle = new GUIStyle() {
				normal = new GUIStyleState() {
					textColor = Color.white,
				},
				alignment = TextAnchor.MiddleCenter,
			};

			InputLockManager.SetControlLock(LOCK_ID);
			RenderingManager.AddToPostDrawQueue(144, OnDraw);
		}

		private void Dispose() {
			InputLockManager.RemoveControlLock(LOCK_ID);
			RenderingManager.RemoveFromPostDrawQueue(144, OnDraw);
		}

		private void OnDraw() {
			this.windowPosition = KSPUtil.ClampRectToScreen(GUILayout.Window(WINDOW_ID, this.windowPosition, this.OnWindowDraw, this.title));
		}

		private void OnWindowDraw(Int32 windowId) {
			GUILayout.BeginVertical();
			{
				GUILayout.Space(10);
				GUILayout.Label(this.message, this.labelStyle, GUILayout.ExpandWidth(true));
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("OK", GUILayout.ExpandWidth(true))) {
						if (this.onSuccess != null) {
							this.onSuccess();
						}
						this.Dispose();
					}
					if (GUILayout.Button("NO", GUILayout.ExpandWidth(true))) {
						if (this.onCancel != null) {
							this.onCancel();
						}
						this.Dispose();
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}
	}
}
