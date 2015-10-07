namespace KerbalStory {
	using Contracts;
	using FinePrint.Contracts.Parameters;
	using FinePrint.Utilities;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Chapter {

		public String Id { get; private set; }

		/// <summary>
		/// コントラクトタイトル
		/// </summary>
		public String Title { get; private set; }

		/// <summary>
		/// コントラクトあらすじ
		/// </summary>
		public String Synopsis { get; private set; }

		/// <summary>
		/// コントラクト詳細
		/// </summary>
		public String Description { get; private set; }

		/// <summary>
		/// ストーリーメッセージ
		/// </summary>
		public String Story { get; private set; }

		/// <summary>
		/// サイエンスポイント
		/// </summary>
		public Int32 Science { get; private set; }

		/// <summary>
		/// 評判
		/// </summary>
		public Int32 Reputation { get; private set; }

		/// <summary>
		/// 前報酬
		/// </summary>
		public Int32 AdvanceFunds { get; private set; }

		/// <summary>
		/// 完了報酬
		/// </summary>
		public Int32 CompletionFunds { get; private set; }

		public IList<ContractParameter> ContractParameters { get; private set; }

		public Chapter(ConfigNode configNode) {
			this.Id = configNode.GetValue("id");
			this.Title = configNode.GetValue("title");
			this.Description = configNode.GetValue("description");
			this.Synopsis = configNode.GetValue("synopsis");
			this.Story = configNode.GetValue("story").Replace("\\n", "\n");

			this.Science = Int32.Parse(configNode.GetValue("science"));
			this.Reputation = Int32.Parse(configNode.GetValue("reputation"));
			var funds = configNode.GetValue("funds").Split(',');
			this.AdvanceFunds = Int32.Parse(funds[0]);
			this.CompletionFunds = Int32.Parse(funds[1]);

			this.ContractParameters = configNode.GetNodes("PARAM").Select(o => this.ParseObjective(o)).ToList();
		}

		public override Boolean Equals(Object obj) {
			var other = (Chapter)obj;
			return this.Id == other.Id;
		}

		public override Int32 GetHashCode() {
			return this.Id.GetHashCode();
		}

		private ContractParameter ParseObjective(ConfigNode node) {
			switch (node.GetValue("name")) {
				case "RecordTrackingParameter":
					return new RecordTrackingParameter(node.GetValue("trackType").ToEnum<RecordTrackType>(), Double.Parse(node.GetValue("trackValue")));
				case "ProbeSystemsParameter":
					return new ProbeSystemsParameter();
				case "satellite":
					var seed = Environment.TickCount;
					var random = new Random(seed);
					var targetBody = FlightGlobals.Bodies.First(body => body.name == node.GetValue("target"));
					var altitudeFactor = 0.5;
					var inclinationFactor = 0.5;
					var eccentricity = random.NextDouble() * (altitudeFactor / 2.0);
					var deviationWindow = 10.0;
					var orbit = CelestialUtilities.GenerateOrbit(OrbitType.STATIONARY, seed, targetBody, altitudeFactor, inclinationFactor, eccentricity);
					return new SpecificOrbitParameter(OrbitType.STATIONARY, orbit.inclination, orbit.eccentricity, orbit.semiMinorAxis, orbit.LAN, orbit.argumentOfPeriapsis, orbit.meanAnomaly, orbit.epoch, targetBody, deviationWindow);
				default:
					return null;
			}
		}
	}
}
