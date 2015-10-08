namespace KerbalStory {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	internal sealed class Util {
		public static Boolean IsModEnabled {
			get {
				return HighLogic.CurrentGame.scenarios.Any(psm => psm.moduleName == typeof(KerbalStoryScenario).Name);
			}
		}
	}
}
