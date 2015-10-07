namespace KerbalStory {
	using System;
	using UnityEngine;

	[KSPScenario(ScenarioCreationOptions.AddToExistingCareerGames, GameScenes.SPACECENTER, GameScenes.FLIGHT)]
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
	}
}
