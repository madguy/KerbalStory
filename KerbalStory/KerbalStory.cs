namespace KerbalAdventure {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class KerbalStory : MonoBehaviour {
		private ApplicationLauncherButton button;

		private IList<Chapter> chapters;

		private void Awake() {
			var texture = GameDatabase.Instance.GetTexture("KerbalAdventure/book", false);
			this.button = ApplicationLauncher.Instance.AddModApplication(ButtonClick, ButtonClick, null, null, null, null,
				ApplicationLauncher.AppScenes.ALWAYS, texture);
			this.chapters = new List<Chapter>();
		}

		private void Start() {

		}

		private void ButtonClick() {

		}

		internal void OnDestroy() {
			if (button != null) {
				ApplicationLauncher.Instance.RemoveModApplication(button);
				button = null;
			}
		}
	}
}
