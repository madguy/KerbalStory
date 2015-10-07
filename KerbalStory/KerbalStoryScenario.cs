namespace KerbalStory {
	using System;
	using UnityEngine;

	[KSPScenario(ScenarioCreationOptions.AddToExistingCareerGames | ScenarioCreationOptions.AddToNewCareerGames, GameScenes.SPACECENTER)]
	public class KerbalStoryScenario : ScenarioModule {
		[KSPField(isPersistant = true)]
		public String chapter;

		[KSPField(isPersistant = true)]
		private String state;

		public StoryState State {
			get {
				return this.state.ToEnum<StoryState>();
			}
			set {
				this.state = value.ToString();
			}
		}

		public override void OnLoad(ConfigNode node) {
			this.chapter = this.chapter ?? "Chapter1";
			this.state = this.state ?? StoryState.Introduction.ToString();
		}
	}
}
