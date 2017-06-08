/// <summary>
/// メールアイテムデータ
/// 
/// 2016/05/16
/// </summary>

using System;

namespace XUI.AchievementItem {

	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// アチーブメントパラメータ
		/// </summary>
		AchievementInfo Info { get; set; }

		#endregion ==== プロパティ ====

		#region ==== 初期化 ====

		void Setup();

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== イベント ====

		/// <summary>
		/// アチーブメント情報が更新された時
		/// </summary>
		event EventHandler OnAchievementInfoChange;

		#endregion ==== イベント ====
	}

	public class Model : IModel {

		#region ==== フィールド ====

		private AchievementInfo info = null;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// アチーブメント情報
		/// </summary>
		public AchievementInfo Info {
			get {
				return info;
			}
			set {
				if( info != value ) {
					info = value;

					OnAchievementInfoChange( this, EventArgs.Empty );
				}
			}
		}

		#endregion ==== プロパティ ====

		#region ==== 初期化 ====

		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup() {
		}

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
		}

		#endregion ==== 破棄 ====

		#region ==== イベント ====

		/// <summary>
		/// アチーブメントパラメータ更新イベント
		/// </summary>
		public event EventHandler OnAchievementInfoChange = ( sender, e ) => { };

		#endregion ==== イベント ====
	}
}