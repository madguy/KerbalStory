namespace KerbalStory {
	using System;
	using System.Linq;

	internal sealed class Util {
		public static Boolean IsModEnabled {
			get {
				return HighLogic.CurrentGame.scenarios.Any(psm => psm.moduleName == typeof(KerbalStoryScenario).Name);
			}
		}

		public static KerbalStoryScenario GetScenario() {
			var protoScenario = HighLogic.CurrentGame.scenarios.FirstOrDefault(s => s.moduleRef is KerbalStoryScenario);
			if (protoScenario == null || protoScenario.moduleRef == null) {
				return null;
			}

			return protoScenario.moduleRef as KerbalStoryScenario;
		}
	}
}
