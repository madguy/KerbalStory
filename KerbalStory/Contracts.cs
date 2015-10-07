namespace KerbalStory {
	using Contracts;
	using FinePrint.Contracts.Parameters;
	using FinePrint.Utilities;
	using System;
	using System.Linq;
	using UnityEngine;

	public class StoryContract : Contract {
		public static readonly Int32 StoryMissionId = Int32.MaxValue;

		private Chapter chapter;

		public static Contract Generate(Chapter chapter) {
			var contract = (StoryContract)Contract.Generate(typeof(StoryContract), Contract.ContractPrestige.Trivial, StoryContract.StoryMissionId, Contract.State.Generated);
			contract.chapter = chapter;
			contract.SetScience(chapter.Science);
			contract.SetReputation(chapter.Reputation);
			contract.SetFunds(chapter.AdvanceFunds, chapter.CompletionFunds);

			foreach (ContractParameter param in chapter.ContractParameters) {
				contract.AddParameter(param);
			}
			contract.Offer();
			return contract;
		}

		protected override Boolean Generate() {
			this.SetExpiry();

			this.expiryType = DeadlineType.None;
			this.deadlineType = DeadlineType.None;

			this.agent = new Contracts.Agents.Agent("Kerbin government", "Squad/Agencies/KerbinWorldFirstRecordKeepingSociety", "Squad/Agencies/KerbinWorldFirstRecordKeepingSociety_scaled");
			return true;
		}

		/// <summary>
		/// キャンセルの可否
		/// </summary>
		/// <returns></returns>
		public override Boolean CanBeCancelled() {
			return false;
		}

		/// <summary>
		/// 辞退の可否
		/// </summary>
		/// <returns></returns>
		public override Boolean CanBeDeclined() {
			return false;
		}

		/// <summary>
		/// コントラクト独自のHashCode
		/// </summary>
		/// <returns></returns>
		protected override String GetHashString() {
			return "StoryContract";
		}

		/// <summary>
		/// タイトル
		/// </summary>
		/// <returns></returns>
		protected override String GetTitle() {
			return this.chapter.Title;
		}

		/// <summary>
		/// 詳細
		/// </summary>
		/// <returns></returns>
		protected override String GetDescription() {
			return this.chapter.Description;
		}

		/// <summary>
		/// あらすじ
		/// </summary>
		/// <returns></returns>
		protected override String GetSynopsys() {
			return this.chapter.Synopsis;
		}

		/// <summary>
		/// コントラクト完了時のミニウィンドウに表示する文字
		/// </summary>
		/// <returns></returns>
		protected override String MessageCompleted() {
			return String.Format("{0} is finished", this.chapter.Id);
		}

		protected override void OnLoad(ConfigNode node) {
			Debug.Log("CONTRACT==LOAD");

			var chapterId = node.GetValue("chapterId");
			this.chapter = GameDatabase.Instance.GetConfigNodes("CHAPTER").Select(n => new Chapter(n)).First(n => n.Id == chapterId);
			Debug.Log(this.chapter != null);
			base.OnLoad(node);
		}

		protected override void OnSave(ConfigNode node) {
			Debug.Log("CONTRACT==SAVE");
			node.AddValue("chapterId", this.chapter.Id);
			base.OnSave(node);
		}

		/// <summary>
		/// コントラクトを受けられる状態か
		/// </summary>
		/// <returns></returns>
		public override Boolean MeetRequirements() {
			return this.MissionSeed == StoryContract.StoryMissionId;
		}

		/// <summary>
		/// オファー時のイベント
		/// </summary>
		protected override void OnOffered() {
			var dialog = new StoryDialog();
			dialog.Message = this.chapter.Story.Trim();
			dialog.OnClick += () => {
				this.Accept();
			};
			dialog.Show();
		}

		/// <summary>
		/// 完了時のイベント
		/// </summary>
		protected override void OnCompleted() {

		}

		protected override void OnParameterStateChange(ContractParameter p) {
			Debug.Log("OnParameterStateChange");
			base.OnParameterStateChange(p);
		}
	}
}
