namespace KerbalStory {
	using System;
	using System.ComponentModel;

	internal static class Extends {
		public static T ToEnum<T>(this String value) {
			if (String.IsNullOrEmpty(value)) {
				return default(T);
			}

			return (T)Enum.Parse(typeof(T), value, true);
		}

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
