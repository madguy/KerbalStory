namespace KerbalStory {
	using System;
	using UnityEngine;

	internal class ConfirmDialog {
		public static readonly String LOCK_ID = "4901858818";
		private static readonly Int32 WINDOW_ID = 714199;

		public String Message { get; private set; }

		public event Action OnConfirm;
		public event Action OnCancel;

		private String title = "確認";

		private Rect windowPosition = new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 50, 400, 100);
		private GUIStyle windowStyle;
		private GUIStyle labelStyle;

		private Boolean isVisible = false;

		public ConfirmDialog(String message) {
			this.Message = message;
			this.windowStyle = new GUIStyle(HighLogic.Skin.window);
			this.labelStyle = new GUIStyle() {
				normal = new GUIStyleState() {
					textColor = Color.white,
				},
				alignment = TextAnchor.MiddleCenter,
			};
		}

		~ConfirmDialog() {
			this.windowStyle = null;
			this.labelStyle = null;

			if (this.OnConfirm != null) {
				foreach (var action in this.OnConfirm.GetInvocationList()) {
					this.OnConfirm -= (Action)action;
				}
			}

			if (this.OnCancel != null) {
				foreach (var action in this.OnCancel.GetInvocationList()) {
					this.OnCancel -= (Action)action;
				}
			}
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
			this.windowPosition = KSPUtil.ClampRectToScreen(GUILayout.Window(WINDOW_ID, this.windowPosition, this.OnWindowDraw, this.title));
		}

		private void OnWindowDraw(Int32 windowId) {
			GUILayout.BeginVertical();
			{
				GUILayout.Space(10);
				GUILayout.Label(this.Message, this.labelStyle, GUILayout.ExpandWidth(true));
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("YES", GUILayout.ExpandWidth(true))) {
						if (this.OnConfirm != null) {
							this.OnConfirm();
						}
						this.Hide();
					}
					if (GUILayout.Button("NO", GUILayout.ExpandWidth(true))) {
						if (this.OnCancel != null) {
							this.OnCancel();
						}
						this.Hide();
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}
	}
}
