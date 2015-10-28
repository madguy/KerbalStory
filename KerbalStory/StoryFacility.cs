namespace KerbalStory {
	using System;
	using UnityEngine;

	internal class StoryFacility : SpaceCenterBuilding {
		public static readonly String LOCK_ID = "9382013920";

		protected override void OnClicked() {
			InputLockManager.SetControlLock(ControlTypes.KSC_ALL, LOCK_ID);
			PopupDialog.SpawnPopupDialog(new MultiOptionDialog("MESSAGE", "WINDOW_TITLE", HighLogic.Skin), false, HighLogic.Skin);
		}

		protected override void OnOnDestroy() {

		}

		private void onDialogClose() {
			InputLockManager.RemoveControlLock(LOCK_ID);
		}
	}
}
