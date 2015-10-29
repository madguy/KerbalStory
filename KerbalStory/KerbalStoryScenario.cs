namespace KerbalStory {
	using System;
	using System.Linq;

	[KSPScenario(ScenarioCreationOptions.AddToNewCareerGames, GameScenes.SPACECENTER, GameScenes.FLIGHT)]
	public sealed class KerbalStoryScenario : ScenarioModule {

		public static KerbalStoryScenario Instance;

		[KSPField(isPersistant = true)]
		private Boolean initialized;

		public Boolean Initialized {
			get { return this.initialized; }
			set { this.initialized = value; }
		}

		[KSPField(isPersistant = true)]
		private new Boolean enabled;

		public Boolean Enabled {
			get { return this.enabled; }
			set { this.enabled = value; }
		}

		[KSPField(isPersistant = true)]
		private String chapter;

		public String Chapter {
			get { return this.chapter; }
			set { this.chapter = value; }
		}

		[KSPField(isPersistant = true)]
		private String state;

		public StoryState State {
			get { return this.state.ToEnum<StoryState>(); }
			set { this.state = value.ToString(); }
		}

		public override void OnAwake() {
			Instance = this;
		}
	}

	public enum StoryState {
		Introduction, Active, Completed,
	}
}
