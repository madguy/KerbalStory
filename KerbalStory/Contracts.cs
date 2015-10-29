namespace KerbalStory {
	using Contracts;
	using Contracts.Agents;
	using System;
	using System.Linq;

	public sealed class StoryContract : Contract {
		public static readonly Int32 StoryMissionId = Int32.MaxValue;

		internal Chapter Chapter { get; private set; }

		public static Contract Generate(String chapterId) {
			var contract = Contract.Generate(typeof(StoryContract), Contract.ContractPrestige.Trivial, StoryContract.StoryMissionId, Contract.State.Generated) as StoryContract;
			if (contract == null) {
				return null;
			}

			var chapter = Chapter.GetInstance(chapterId);
			contract.Chapter = chapter;
			contract.prestige = chapter.Difficulty;
			contract.AutoAccept = true;
			contract.SetScience(chapter.Science);
			contract.SetReputation(chapter.Reputation);
			contract.SetFunds(chapter.AdvanceFunds, chapter.CompletionFunds, chapter.FailureFunds);

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

			this.agent = AgentList.Instance.GetAgent("Kerbin government");
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
		/// コントラクト独自のHashString
		/// </summary>
		/// <returns></returns>
		protected override String GetHashString() {
			return this.MissionSeed.ToString() + this.DateAccepted.ToString();
		}

		/// <summary>
		/// タイトル
		/// </summary>
		/// <returns></returns>
		protected override String GetTitle() {
			return this.Chapter.Title;
		}

		/// <summary>
		/// コントラクト完了時のミニウィンドウに表示する文字
		/// </summary>
		/// <returns></returns>
		protected override String MessageCompleted() {
			return String.Format("{0} is finished", this.Chapter.Id);
		}

		protected override void OnLoad(ConfigNode node) {
			var chapterId = node.GetValue("chapterId");
			this.Chapter = Chapter.GetInstance(chapterId);
			base.OnLoad(node);
		}

		protected override void OnSave(ConfigNode node) {
			node.AddValue("chapterId", this.Chapter.Id);
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
			StoryDialog.ShowDialog(this.Chapter.Instructor, this.Chapter.Story, () => {
				this.Accept();
			});
		}

		/// <summary>
		/// 完了時のイベント
		/// </summary>
		protected override void OnCompleted() {

		}
	}
}
