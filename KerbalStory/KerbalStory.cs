namespace KerbalStory {
	using Contracts;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public sealed class KerbalStory : MonoBehaviour {
		private ApplicationLauncherButton luncherButton;

		private IList<String> chapterIds;

		/// <summary>
		/// Startのオーバーライド
		/// </summary>
		private void Start() {
			GameEvents.Contract.onCompleted.Add(this.OnContractComplete);
			StartCoroutine(this.SceneInitialize());
		}

		/// <summary>
		/// シーンを初期化します。
		/// </summary>
		/// <returns></returns>
		private IEnumerator SceneInitialize() {
			yield return StartCoroutine(this.WaitScenarioInitialize());

			var scenario = KerbalStoryScenario.Instance;
			if (scenario == null) {
				yield break;
			}

			var nodes = GameDatabase.Instance.GetConfigNodes("CHAPTER");
			this.chapterIds = nodes.Select(node => node.GetValue("id")).ToList();

			yield return StartCoroutine(this.ModInitialize(scenario));
			if (scenario.Enabled == false) {
				yield break;
			}

			var texture = GameDatabase.Instance.GetTexture("KerbalStory/book", false);
			this.luncherButton = ApplicationLauncher.Instance.AddModApplication(LuncherButtonOn, LuncherButtonOff, null, null, null, null,
				ApplicationLauncher.AppScenes.ALWAYS, texture);

			this.MoveToNextChapter(scenario);
			this.ActivateChapter(scenario);
		}

		/// <summary>
		/// シナリオ初期化を待機します。
		/// </summary>
		/// <returns></returns>
		private IEnumerator WaitScenarioInitialize() {
			while (true) {
				if (KerbalStoryScenario.Instance != null) {
					break;
				}
				yield return 0;
			}
		}

		/// <summary>
		/// Modを初期化します。
		/// </summary>
		/// <returns></returns>
		private IEnumerator ModInitialize(KerbalStoryScenario scenario) {
			if (scenario.Initialized == true) {
				yield break;
			}

			// ダイアログを表示
			ConfirmDialog.ShowDialog("KerbalStoryを有効にしますか？", () => {
				scenario.Initialized = true;
				scenario.Enabled = true;
				scenario.Chapter = this.chapterIds[0];
				scenario.State = StoryState.Introduction;

				var funding = Funding.Instance;
				funding.AddFunds((-1 * funding.Funds), TransactionReasons.None);
			}, () => {
				scenario.Initialized = true;
				scenario.Enabled = false;
			});

			while (true) {
				if (scenario.Initialized == true) {
					break;
				}
				yield return 0;
			}
		}

		/// <summary>
		/// チャプタークリア済みの場合に次のチャプターに移動させます。
		/// </summary>
		private void MoveToNextChapter(KerbalStoryScenario scenario) {
			if (scenario.State != StoryState.Completed) {
				return;
			}

			var nextChapterId = this.chapterIds.SkipWhile(id => id != scenario.Chapter).Skip(1).FirstOrDefault();
			if (nextChapterId == null) {
				return;
			}

			scenario.Chapter = nextChapterId;
			scenario.State = StoryState.Introduction;
		}

		/// <summary>
		/// チャプターが開始前の場合に、状態をアクティブに変更します。
		/// </summary>
		private void ActivateChapter(KerbalStoryScenario scenario) {
			if (scenario.State != StoryState.Introduction) {
				return;
			}

			var currentChapterId = this.chapterIds.FirstOrDefault(id => id == scenario.Chapter);
			if (String.IsNullOrEmpty(currentChapterId)) {
				return;
			}

			var contract = StoryContract.Generate(currentChapterId);
			ContractSystem.Instance.Contracts.Add(contract);
			scenario.State = StoryState.Active;
		}

		/// <summary>
		/// シーン切り替え時の破棄イベント
		/// </summary>
		internal void OnDestroy() {
			if (KerbalStoryScenario.Instance.Enabled == false) {
				return;
			}

			if (luncherButton != null) {
				ApplicationLauncher.Instance.RemoveModApplication(this.luncherButton);
				luncherButton = null;
			}

			GameEvents.Contract.onCompleted.Remove(this.OnContractComplete);
		}

		/// <summary>
		/// ランチャーボタンON時のイベント
		/// </summary>
		private void LuncherButtonOn() {
			var currentContract = ContractSystem.Instance.Contracts.FirstOrDefault(c => c is StoryContract) as StoryContract;
			if (currentContract == null) {
				luncherButton.SetFalse();
				return;
			}

			var chapter = Chapter.GetInstance(currentContract.Chapter.Id);
			StoryDialog.ShowDialog(chapter.Instructor, chapter.Story, () => {
				luncherButton.SetFalse();
			});
		}

		/// <summary>
		/// ランチャーボタンOFF時のイベント
		/// </summary>
		private void LuncherButtonOff() {

		}

		/// <summary>
		/// ストーリーコントラクト完了時のイベント
		/// </summary>
		/// <param name="contract"></param>
		private void OnContractComplete(Contract contract) {
			if ((contract is StoryContract) == false) {
				return;
			}
			var scenario = KerbalStoryScenario.Instance;
			if (scenario == null) {
				return;
			}

			scenario.State = StoryState.Completed;
		}
	}

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class KerbalStoryFlight : MonoBehaviour {
		private void Awake() {
			GameEvents.Contract.onCompleted.Add(this.OnContractComplete);
		}

		internal void OnDestroy() {
			GameEvents.Contract.onCompleted.Remove(this.OnContractComplete);
		}

		private void OnContractComplete(Contract contract) {
			if ((contract is StoryContract) == false) {
				return;
			}
			var scenario = KerbalStoryScenario.Instance;
			if (scenario == null) {
				return;
			}

			scenario.State = StoryState.Completed;
		}
	}
}
