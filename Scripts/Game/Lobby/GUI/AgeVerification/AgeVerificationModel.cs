/// <summary>
/// 年齢認証データ
/// 
/// 2016/06/27
/// </summary>

using UnityEngine;
using System.Collections;

namespace XUI.AgeVerification {

	#region ==== 定数 ====

	/// <summary>
	/// 年齢認証画面モード
	/// </summary>
	public enum ViewMode {
		None,
		AgeVerification,
		FullAgeVerification,
		NonAgeVerification
	}

	#endregion ==== 定数 ====

	/// <summary>
	/// 年齢認証データインターフェイス
	/// </summary>
	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// 年齢認証画面モード
		/// </summary>
		ViewMode Mode { get; set; }

		/// <summary>
		/// 年齢認証チェック
		/// </summary>
		bool SkipFlag { get; set; }

		#endregion ==== プロパティ ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====
	}

	/// <summary>
	/// 年齢認証データ
	/// </summary>
	public class Model : IModel {

		#region ==== フィールド ====

		private ViewMode mode = ViewMode.None;
		private bool skipFlag = false;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// 年齢認証画面モード
		/// </summary>
		public ViewMode Mode { get { return mode; } set { mode = value; } }

		/// <summary>
		/// 年齢認証チェック
		/// </summary>
		public bool SkipFlag { get { return skipFlag; } set { skipFlag = value; } }

		#endregion ==== プロパティ ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
		}

		#endregion ==== 破棄 ====
	}
}