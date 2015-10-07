namespace KerbalStory {
	using Contracts;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class KerbalStory : MonoBehaviour {
		private ApplicationLauncherButton luncherButton;
		private StoryDialog dialog = new StoryDialog();

		private IList<String> chapterIds;

		/// <summary>
		/// シーン切り替え時の生成イベント
		/// </summary>
		private void Awake() {
			if (Util.IsModEnabled == false) {
				return;
			}

			var texture = GameDatabase.Instance.GetTexture("KerbalStory/book", false);
			this.luncherButton = ApplicationLauncher.Instance.AddModApplication(LuncherButtonOn, LuncherButtonOff, null, null, null, null,
				ApplicationLauncher.AppScenes.ALWAYS, texture);

			var nodes = GameDatabase.Instance.GetConfigNodes("CHAPTER");
			this.chapterIds = nodes.Select(node => node.GetValue("id")).ToList();

			GameEvents.Contract.onContractsLoaded.Add(OnContractsLoaded);
		}

		/// <summary>
		/// シーン切り替え時の破棄イベント
		/// </summary>
		internal void OnDestroy() {
			if (Util.IsModEnabled == false) {
				return;
			}

			if (luncherButton != null) {
				ApplicationLauncher.Instance.RemoveModApplication(luncherButton);
				luncherButton = null;
			}

			GameEvents.Contract.onContractsLoaded.Remove(OnContractsLoaded);
		}

		private void LuncherButtonOn() {
			var contract = ContractSystem.Instance.Contracts.FirstOrDefault(c => c is StoryContract) as StoryContract;
			if (contract == null) {
				luncherButton.SetFalse();
				return;
			}

			this.dialog.Message = contract.Story;
			this.dialog.OnClick += () => {
				luncherButton.SetFalse();
			};
			this.dialog.Show();
		}

		private void LuncherButtonOff() {
			this.dialog.Hide();
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
			if (Util.IsModEnabled == false) {
				return;
			}

			GameEvents.Contract.onCompleted.Add(OnContractComplete);
		}

		internal void OnDestroy() {
			if (Util.IsModEnabled == false) {
				return;
			}

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

	internal static class Util {
		public static Boolean IsModEnabled {
			get {
				var isEnabled = HighLogic.CurrentGame.scenarios.Any(psm => psm.moduleName == typeof(KerbalStoryScenario).Name);
				Debug.Log(String.Format("== Mod is {0} ==", isEnabled));
				return isEnabled;
			}
		}
	}
}
