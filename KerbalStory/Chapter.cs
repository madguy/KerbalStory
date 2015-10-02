namespace KerbalAdventure {
	using Contracts;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Chapter {
		public String Id { get; private set; }

		public String Title { get; set; }

		public String Synopsis { get; set; }

		public String Description { get; set; }

		/// <summary>
		/// サイエンスポイント
		/// </summary>
		public Int32 Science { get; set; }

		/// <summary>
		/// 評判
		/// </summary>
		public Int32 Reputation { get; set; }

		/// <summary>
		/// 前報酬
		/// </summary>
		public Int32 AdvanceFunds { get; set; }

		/// <summary>
		/// 完了報酬
		/// </summary>
		public Int32 CompletionFunds { get; set; }

		public IList<ContractParameter> ContractParameters { get; private set; }

		public Chapter(String id) {
			this.Id = id;
			this.ContractParameters = new List<ContractParameter>();
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
