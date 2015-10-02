namespace KerbalAdventure {
	using Contracts;
	using FinePrint.Contracts.Parameters;
	using FinePrint.Utilities;
	using System;
	using UnityEngine;

	public class StoryContract : Contract {
		public static readonly Int32 StoryMissionId = Int32.MaxValue;

		private Chapter chapter;

		internal static Contract Generate(Chapter chapter) {
			var contract = (StoryContract)Contract.Generate(typeof(StoryContract), Contract.ContractPrestige.Trivial, StoryContract.StoryMissionId, Contract.State.Generated);
			contract.SetChapter(chapter);
			contract.Offer();
			ContractSystem.Instance.Contracts.Add(contract);
			return contract;
		}

		protected override Boolean Generate() {
			this.SetExpiry();

			this.expiryType = DeadlineType.None;
			this.deadlineType = DeadlineType.None;

			this.agent = new Contracts.Agents.Agent("Kerbin government", "Squad/Agencies/KerbinWorldFirstRecordKeepingSociety", "Squad/Agencies/KerbinWorldFirstRecordKeepingSociety_scaled");

			return true;
		}

		private void SetChapter(Chapter chapter) {
			this.chapter = chapter;

			this.SetScience(this.chapter.Science);
			this.SetReputation(this.chapter.Reputation);
			this.SetFunds(this.chapter.AdvanceFunds, this.chapter.CompletionFunds);

			foreach (ContractParameter param in this.chapter.ContractParameters) {
				this.AddParameter(param);
			}
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
			return String.Format("StoryContract {0}", this.chapter.Id);
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
			base.OnLoad(node);
		}

		protected override void OnSave(ConfigNode node) {
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
			dialog.Message = "test";
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
