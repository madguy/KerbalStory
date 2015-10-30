namespace KerbalStory {
	using Contracts;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal sealed class Chapter {
		public static Chapter GetInstance(String chapterId) {
			var configNode = GameDatabase.Instance.GetConfigNodes("CHAPTER").FirstOrDefault(n => n.GetValue("id") == chapterId);
			if (configNode == null) {
				return null;
			}

			return new Chapter(configNode);
		}

		/// <summary>
		/// チャプターID
		/// </summary>
		public String Id { get; private set; }

		/// <summary>
		/// コントラクトタイトル
		/// </summary>
		public String Title { get; private set; }

		/// <summary>
		/// インストラクター名
		/// </summary>
		public String InstructorName { get; private set; }

		/// <summary>
		/// インストラクタータイプ
		/// </summary>
		public String InstractorType { get; private set; }

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

		/// <summary>
		/// 失敗報酬
		/// </summary>
		public Int32 FailureFunds { get; private set; }

		/// <summary>
		/// 難易度
		/// </summary>
		public Contract.ContractPrestige Difficulty { get; private set; }

		public IList<ContractParameter> ContractParameters { get; private set; }

		public Chapter(ConfigNode configNode) {
			this.Id = configNode.GetValue("id");

			var instructor = configNode.GetNode("INSTRACTOR");
			this.InstructorName = instructor.GetValue("name");
			this.InstractorType = instructor.GetValue("type");

			this.Title = configNode.GetValue("title");
			this.Story = configNode.GetValue("story").Replace("\\n", "\n");

			this.Difficulty = configNode.GetValue("difficulty").ToEnum<Contract.ContractPrestige>();
			this.Science = Int32.Parse(configNode.GetValue("science"));
			this.Reputation = Int32.Parse(configNode.GetValue("reputation"));
			var funds = configNode.GetValue("funds").Split(',');
			this.AdvanceFunds = Int32.Parse(funds[0]);
			this.CompletionFunds = Int32.Parse(funds[1]);
			this.FailureFunds = Int32.Parse(funds[2]);

			this.ContractParameters = configNode.GetNodes("PARAM").Select(node => {
				var paramName = node.GetValue("name");
				var parameterType = ContractSystem.GetParameterType(paramName);
				var contractParameter = (ContractParameter)Activator.CreateInstance(parameterType);
				contractParameter.Load(node);
				return contractParameter;
			}).ToList();
		}

		public override Boolean Equals(Object obj) {
			var other = (Chapter)obj;
			return this.Id == other.Id;
		}

		public override Int32 GetHashCode() {
			return this.Id.GetHashCode();
		}
	}
}
