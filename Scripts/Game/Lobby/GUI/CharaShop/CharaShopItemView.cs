/// <summary>
/// キャラクターショップアイテム画面
/// 
/// 2016/06/15
/// </summary>

using System;
using UnityEngine;

namespace XUI.CharaShopItem {

	/// <summary>
	/// インターフェイス
	/// </summary>
	public interface IView {

		#region ==== イベント ====

		/// <summary>
		/// チケット交換ボタンイベント
		/// </summary>
		event EventHandler<EventArgs> OnTicketExchangeButtonEvent;

		#endregion ==== イベント ====

		#region ==== 初期化 ====

		/// <summary>
		/// セットアップ
		/// </summary>
		void Setup();

		#endregion ==== 初期化 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態の取得
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="isActive"></param>
		/// <param name="isTweemSkip"></param>
		void SetActive( bool isActive, bool isTweemSkip );

		/// <summary>
		/// アクティブ状態の取得
		/// </summary>
		/// <returns></returns>
		GUIViewBase.ActiveState GetActiveState();

		#endregion ==== アクティブ ====

		#region ==== アクション ====

		/// <summary>
		/// キャラクターショップアイテム設定
		/// </summary>
		/// <param name="info"></param>
		void SetCharaShopInfo( CharaShopItemInfo info );

		#endregion ==== アクション ====
	}

	/// <summary>
	/// ビュー
	/// </summary>
	public class CharaShopItemView : GUIViewBase, IView {

		#region ==== フィールド ====

		/// <summary>
		/// キャラクターアイテム
		/// </summary>
		[SerializeField]
		private GUICharaItem charaItem = null;

		/// <summary>
		/// 名前
		/// </summary>
		[SerializeField]
		private UILabel nameLabel = null;

		/// <summary>
		/// 説明
		/// </summary>
		[SerializeField]
		private UILabel infoLabel = null;

		/// <summary>
		/// クールタイム
		/// </summary>
		[SerializeField]
		private UILabel coolTimeLabel = null;

		/// <summary>
		/// 交換ボタン
		/// </summary>
		[SerializeField]
		private XUIButton ticketExchangeButton = null;

		/// <summary>
		/// 購入済みの際表示するやつ
		/// </summary>
		[SerializeField]
		private GameObject purchasedSprite = null;

		#endregion ==== フィールド ====

		#region ==== イベント ====

		/// <summary>
		/// 交換ボタンを押下した時
		/// </summary>
		public event EventHandler<EventArgs> OnTicketExchangeButtonEvent = ( sender, e ) => { };

		#endregion ==== イベント ====

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
		private void OnDestroy() {

			OnTicketExchangeButtonEvent = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="isActive"></param>
		/// <param name="isTweenSkip"></param>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			this.SetRootActive( isActive, isTweenSkip );
		}

		/// <summary>
		/// アクティブ状態の取得
		/// </summary>
		/// <returns></returns>
		public ActiveState GetActiveState() {

			return GetRootActiveState();
		}

		#endregion ==== アクティブ ====

		#region ==== アクション ====

		/// <summary>
		/// アイテムの項目設定
		/// </summary>
		/// <param name="info"></param>
		public void SetCharaShopInfo( CharaShopItemInfo info ) {

			if( info == null ) return;

			// アイコン設定
			charaItem.SetState(CharaItem.Controller.ItemStateType.Icon, new CharaInfo( ( AvatarType )info.CharaID ) );

			// 名前設定
			nameLabel.text = info.CharaName;

			// 説明設定
			infoLabel.text = info.Explanatory;

			// クールタイム設定
			coolTimeLabel.text = string.Format( "{0}", info.CoolTime );

			// 購入済みならばtrueにする
			purchasedSprite.SetActive( info.ProductStatus == CharaShopItemInfo.Status.Limit );

			// 購入ボタン有効設定
			ticketExchangeButton.isEnabled = ( info.ProductStatus == CharaShopItemInfo.Status.Normal );

			// 表示
			SetActive( true, true );
		}

		#endregion ==== アクション ====

		#region ==== NGUIリフレクション ====

		/// <summary>
		/// チケット交換ボタンクリック
		/// </summary>
		public void OnTicketExchangeButtonClick() {

			OnTicketExchangeButtonEvent( this, EventArgs.Empty );
		}

		#endregion ==== NGUIリフレクション ====
	}
}

