namespace KerbalStory {
	using Contracts;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class KerbalStory : MonoBehaviour {
		private ApplicationLauncherButton button;

		private IList<String> chapterIds;

		private void Awake() {
			var texture = GameDatabase.Instance.GetTexture("KerbalStory/book", false);
			this.button = ApplicationLauncher.Instance.AddModApplication(ButtonClick, ButtonClick, null, null, null, null,
				ApplicationLauncher.AppScenes.ALWAYS, texture);

			var nodes = GameDatabase.Instance.GetConfigNodes("CHAPTER");
			this.chapterIds = nodes.Select(node => node.GetValue("id")).ToList();

			GameEvents.Contract.onContractsLoaded.Add(OnContractsLoaded);
		}

		internal void OnDestroy() {
			if (button != null) {
				ApplicationLauncher.Instance.RemoveModApplication(button);
				button = null;
			}

			GameEvents.Contract.onContractsLoaded.Remove(OnContractsLoaded);
		}

		private void ButtonClick() {

		}

		private void OnContractsLoaded() {
			this.ChapterChange();
			this.ActivateContract();
		}

		private void ChapterChange() {
			var scenario = this.GetScenario(HighLogic.CurrentGame);
			if (scenario == null || scenario.State != StoryState.Completed) {
				return;
			}

			var nextChapterId = this.chapterIds.SkipWhile(id => id != scenario.chapter).Skip(1).FirstOrDefault();
			if (nextChapterId == null) {
				return;
			}

			scenario.chapter = nextChapterId;
			scenario.State = StoryState.Introduction;
		}

		private void ActivateContract() {
			var scenario = this.GetScenario(HighLogic.CurrentGame);
			if (scenario == null || scenario.State != StoryState.Introduction) {
				return;
			}

			var currentChapterId = this.chapterIds.FirstOrDefault(id => id == scenario.chapter);
			if (String.IsNullOrEmpty(currentChapterId)) {
				return;
			}

			var contract = StoryContract.Generate(currentChapterId);
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

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class KerbalStoryFlight : MonoBehaviour {
		private void Awake() {
			GameEvents.Contract.onCompleted.Add(OnContractComplete);
		}

		internal void OnDestroy() {
			GameEvents.Contract.onCompleted.Remove(OnContractComplete);
		}

		private void OnContractComplete(Contract contract) {
			if ((contract is StoryContract) == false) {
				return;
			}
			var scenario = this.GetScenario(HighLogic.CurrentGame);
			scenario.State = StoryState.Completed;
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
