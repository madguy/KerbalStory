namespace KerbalStory {
	using Contracts;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class KerbalStory : MonoBehaviour {
		private ApplicationLauncherButton button;

		private IList<Chapter> chapters;

		private void Awake() {
			var texture = GameDatabase.Instance.GetTexture("KerbalStory/book", false);
			this.button = ApplicationLauncher.Instance.AddModApplication(ButtonClick, ButtonClick, null, null, null, null,
				ApplicationLauncher.AppScenes.ALWAYS, texture);

			var nodes = GameDatabase.Instance.GetConfigNodes("CHAPTER");
			this.chapters = nodes.Select(node => new Chapter(node)).ToList();

			GameEvents.Contract.onContractsLoaded.Add(ChapterChange);
			GameEvents.Contract.onContractsLoaded.Add(ActivateContract);
		}

		internal void OnDestroy() {
			if (button != null) {
				ApplicationLauncher.Instance.RemoveModApplication(button);
				button = null;
			}

			GameEvents.Contract.onContractsLoaded.Remove(ChapterChange);
			GameEvents.Contract.onContractsLoaded.Remove(ActivateContract);
		}

		private void ButtonClick() {
			Debug.Log(ContractSystem.Instance.Contracts.Count);
		}

		private void ChapterChange() {
			var scenario = this.GetScenario(HighLogic.CurrentGame);
			if (scenario.State != StoryState.Completed) {
				return;
			}

			var nextChapter = this.chapters.SkipWhile(c => c.Id != scenario.chapter).Skip(1).FirstOrDefault();
			if (nextChapter == null) {
				return;
			}

			scenario.chapter = nextChapter.Id;
			scenario.State = StoryState.Introduction;
		}

		private void ActivateContract() {
			var scenario = this.GetScenario(HighLogic.CurrentGame);
			if (scenario.State != StoryState.Introduction) {
				return;
			}

			var currentChapter = this.chapters.FirstOrDefault(c => c.Id == scenario.chapter);
			if (currentChapter == null) {
				return;
			}

			var contract = StoryContract.Generate(currentChapter);
			ContractSystem.Instance.Contracts.Add(contract);
			scenario.State = StoryState.Active;
		}

		private KerbalStoryScenario GetScenario(Game game) {
			var protoScenario = game.scenarios.FirstOrDefault(s => s.moduleRef is KerbalStoryScenario);
			if (protoScenario == null) {
				return null;
			}

			return protoScenario.moduleRef as KerbalStoryScenario;
		}
	}

	public enum StoryState {
		Introduction, Active, Completed,
	}
}
