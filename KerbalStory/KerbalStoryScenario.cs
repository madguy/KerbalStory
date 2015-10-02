namespace KerbalAdventure {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.ComponentModel;

	public class KerbalStoryScenario : ScenarioModule {
		private static readonly String NODE_NAME = "KERBAL_STORY_SETTINGS";

		private ConfigNode settingsNode = null;

		public KerbalStoryScenario() {
		}

		public override void OnLoad(ConfigNode node) {
			base.OnLoad(node);

			Debug.Log("OnLoad");

			if (node.HasNode(NODE_NAME)) {
				settingsNode = node.GetNode(NODE_NAME);
			}
		}

		public override void OnSave(ConfigNode node) {
			base.OnSave(node);

			Debug.Log("OnSave");

			if (node.HasNode(NODE_NAME)) {
				settingsNode = node.GetNode(NODE_NAME);
			} else {
				settingsNode = node.AddNode(NODE_NAME);
			}
		}
	}

	internal static class ConfigNodeExtend {
		public static T TryGetValue<T>(this ConfigNode config, String key, T defaultValue) where T : struct {
			var converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter == null) {
				return defaultValue;
			}

			if (String.IsNullOrEmpty(key)) {
				return defaultValue;
			}

			return (T)converter.ConvertFromString(config.GetValue(key));
		}
	}
}
