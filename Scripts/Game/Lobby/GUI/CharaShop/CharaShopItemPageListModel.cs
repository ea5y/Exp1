/// <summary>
/// キャラクターショップアイテムページリストデータ
/// 
/// 2016/06/16
/// </summary>

using System;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace XUI.CharaShopItemPageList {

	#region ==== スクロールビュー ====

	[Serializable]
	public class GUICharaShopItemScrollView : PageScrollView<GUICharaShopItem> {

		/// <summary>
		/// 作成
		/// </summary>
		/// <param name="prefab">ベース</param>
		/// <param name="parent">登録先</param>
		/// <param name="itemIndex">インデックス</param>
		/// <returns></returns>
		protected override GUICharaShopItem Create( GameObject prefab, Transform parent, int itemIndex ) {

			return GUICharaShopItem.Create( prefab, parent, itemIndex );
		}

		/// <summary>
		/// クリア
		/// </summary>
		/// <param name="item"></param>
		protected override void ClearValue( GUICharaShopItem item ) {

			item.SetShopInfo( null );
		}

		public void SetPageButtonEnable( bool enable ) {
			float alpha = enable ? 1.0f : 0.5f;
			if( Attach.BackButtonGroup != null ) {
				Attach.BackButtonGroup.alpha = alpha;
			}
			if( Attach.NextButtonGroup != null ) {
				Attach.NextButtonGroup.alpha = alpha;
			}
		}
	}

	#endregion ==== スクロールビュー ====

	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// スクロールビュー
		/// </summary>
		GUICharaShopItemScrollView ShopItemScrollView { get; }

		/// <summary>
		/// 表示するリスト
		/// </summary>
		ReadOnlyCollection<CharaShopItemInfo> CurrentShopInfoList { get; }

		/// <summary>
		/// ページボタンの有効
		/// </summary>
		bool PageButtonEnabele { get; set; }

		/// <summary>
		/// 今のページ
		/// </summary>
		int CurrentPage { get; }

		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		event EventHandler OnPageChange;

		/// <summary>
		/// 表示するリスト変更イベント
		/// </summary>
		event EventHandler OnCurrentShopInfoListChange;

		#endregion ==== イベント ====

		#region ==== 初期化 ====

		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== アクション ====

		/// <summary>
		/// ショップパラメータリストを更新
		/// </summary>
		/// <param name="infos"></param>
		void SetCurrentShopInfoList( IList<CharaShopItemInfo> infos );

		/// <summary>
		/// ページを変更する
		/// </summary>
		void SetPage( int page );

		/// <summary>
		/// ページ戻る
		/// </summary>
		void BackPage();

		/// <summary>
		/// 一番後ろに戻る
		/// </summary>
		void BackEndPage();

		/// <summary>
		/// ページを進める
		/// </summary>
		void NextPage();

		/// <summary>
		/// 一番最後のページに進める
		/// </summary>
		void NextEndPage();

		/// <summary>
		/// トータルのアイテム数をセットする
		/// </summary>
		/// <param name="count"></param>
		void SetTotalItemCount( int count );

		/// <summary>
		/// リストの位置を修正する
		/// </summary>
		void Reposition();

		/// <summary>
		/// アイテムリストを取得
		/// </summary>
		/// <returns></returns>
		List<GUICharaShopItem> GetShopItemList();

		#endregion ==== アクション ====
	}

	public class Model : IModel {

		#region ==== フィールド ====

		private GUICharaShopItemScrollView shopItemScrollView = null;

		private readonly List<CharaShopItemInfo> currentViewShopInfo = new List<CharaShopItemInfo>();

		private bool pageButtonEnabele = true;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// メールスクロールビュー
		/// </summary>
		public GUICharaShopItemScrollView ShopItemScrollView { get { return shopItemScrollView; } }

		/// <summary>
		/// 表示するリスト
		/// </summary>
		public ReadOnlyCollection<CharaShopItemInfo> CurrentShopInfoList {
			get { return currentViewShopInfo.AsReadOnly(); }
		}

		/// <summary>
		/// ページボタンの有効
		/// </summary>
		public bool PageButtonEnabele {
			get { return pageButtonEnabele; }
			set { pageButtonEnabele = value; }
		}

		/// <summary>
		/// 今のページ
		/// </summary>
		public int CurrentPage { get; private set; }

		#endregion ==== プロパティ ====

		#region ==== イベント ====

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		public event EventHandler OnPageChange = (sender, e) => { };

		/// <summary>
		/// 表示するリスト変更イベント
		/// </summary>
		public event EventHandler OnCurrentShopInfoListChange = (sender, e) => { };

		#endregion ==== イベント ====

		#region ==== コンストラクタ ====

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="itemScrollView"></param>
		/// <param name="viewAttach"></param>
		public Model( GUICharaShopItemScrollView itemScrollView, PageScrollViewAttach viewAttach ) {

			shopItemScrollView = itemScrollView;
			ShopItemScrollView.Create( viewAttach, null );
		}

		#endregion ==== コンストラクタ ====

		#region ==== 初期化 ====

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup() {

			CurrentPage = -1;
			currentViewShopInfo.Clear();
			ShopItemScrollView.Clear();
		}

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose() {

			currentViewShopInfo.Clear();

			OnPageChange = null;
			OnCurrentShopInfoListChange = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクション ====

		/// <summary>
		/// リストを更新
		/// </summary>
		/// <param name="infos"></param>
		public void SetCurrentShopInfoList( IList<CharaShopItemInfo> infos ) {

			currentViewShopInfo.Clear();
			currentViewShopInfo.AddRange( infos );

			OnCurrentShopInfoListChange( this, EventArgs.Empty );
		}

		/// <summary>
		/// ページを変更
		/// </summary>
		public void SetPage( int page ) {

			ShopItemScrollView.ScrollReset();
			if( ShopItemScrollView.SetPage( page, 0 ) ) {
				CurrentPage = page;
			}
			OnPageChange( this, EventArgs.Empty );
		}

		/// <summary>
		/// ページ戻る
		/// </summary>
		public void BackPage() {

			ShopItemScrollView.ScrollReset();
			if( ShopItemScrollView.SetNextPage( -1 ) ) {
				CurrentPage = ShopItemScrollView.PageIndex;
				OnPageChange( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// 一番後ろに戻る
		/// </summary>
		public void BackEndPage() {

			ShopItemScrollView.ScrollReset();
			if( ShopItemScrollView.SetPage( 0, 0 ) ) {
				CurrentPage = ShopItemScrollView.PageIndex;
				OnPageChange( this, EventArgs.Empty );
			}
		}


		/// <summary>
		/// ページを進める
		/// </summary>
		public void NextPage() {

			ShopItemScrollView.ScrollReset();
			if( ShopItemScrollView.SetNextPage( 1 ) ) {
				CurrentPage = ShopItemScrollView.PageIndex;
				OnPageChange( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// 一番最後のページに進める
		/// </summary>
		public void NextEndPage() {

			ShopItemScrollView.ScrollReset();
			if( ShopItemScrollView.SetPage( ShopItemScrollView.PageMax - 1, 0 ) ) {
				CurrentPage = ShopItemScrollView.PageIndex;
				OnPageChange( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// トータルのアイテム数をセットする
		/// </summary>
		/// <param name="count"></param>
		public void SetTotalItemCount( int count ) {

			ShopItemScrollView.ScrollReset();
			ShopItemScrollView.Setup( count, 0 );
		}

		/// <summary>
		/// リストの位置を修正する
		/// </summary>
		public void Reposition() {

			ShopItemScrollView.ScrollReset();
			ShopItemScrollView.Reposition();
		}

		/// <summary>
		/// アイテムリストを取得
		/// </summary>
		/// <returns></returns>
		public List<GUICharaShopItem> GetShopItemList() {

			List<GUICharaShopItem> itemList = new List<GUICharaShopItem>();

			for( int i = 0 ; i < ShopItemScrollView.ItemMax ; i++ ) {
				itemList.Add( ShopItemScrollView.GetItem( i ) );
			}
			return itemList;
		}

		#endregion ==== アクション =====
	}
}
