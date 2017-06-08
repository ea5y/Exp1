/// <summary>
/// キャラクターショップアイテムデータ
/// 
/// 2016/06/15
/// </summary>

using System;

namespace XUI.CharaShopItem {

	/// <summary>
	/// インターフェイス
	/// </summary>
	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// キャラクターショップパラメータ
		/// </summary>
		CharaShopItemInfo Info { get; set; }

		#endregion ==== プロパティ ====

		#region ==== 初期化 ====

		/// <summary>
		/// セットアップ
		/// </summary>
		void Setup();

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== イベント ====

		/// <summary>
		/// ショップ情報が更新された時
		/// </summary>
		event EventHandler<EventArgs> OnCharaShopInfoChange;

		#endregion ==== イベント ====
	}

	/// <summary>
	/// データ
	/// </summary>
	public class Model : IModel {

		#region ==== フィールド ====

		private CharaShopItemInfo info = null;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// ショップ情報
		/// </summary>
		public CharaShopItemInfo Info {
			get {
				return info;
			}
			set {
				if( info != value ) {
					info = value;

					OnCharaShopInfoChange( this, EventArgs.Empty );
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

			OnCharaShopInfoChange = null;
		}

		#endregion ==== 破棄 ====

		#region ==== イベント ====

		/// <summary>
		/// ショップパラメータ更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnCharaShopInfoChange = ( sender, e ) => { };

		#endregion ==== イベント ====
	}
}