/// <summary>
/// アチーブメントアイテムページリストデータ
/// 
/// 2016/05/16
/// </summary>

using System;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace XUI.AchievementItemPageList {

	[Serializable]
	public class GUIAchievementItemScrollView : PageScrollView<GUIAchievementItem> {

		/// <summary>
		/// 作成
		/// </summary>
		/// <param name="prefab">ベース</param>
		/// <param name="parent">登録先</param>
		/// <param name="itemIndex">インデックス</param>
		/// <returns></returns>
		protected override GUIAchievementItem Create( GameObject prefab, Transform parent, int itemIndex ) {

			return GUIAchievementItem.Create( prefab, parent, itemIndex );
		}

		/// <summary>
		/// クリア
		/// </summary>
		/// <param name="item"></param>
		protected override void ClearValue( GUIAchievementItem item ) {

			item.SetAchievementInfo( null );
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

	public interface IModel {

		#region ==== プロパティ ====

		/// <summary>
		/// スクロールビュー
		/// </summary>
		GUIAchievementItemScrollView AchievementItemScrollView { get; }

		/// <summary>
		/// 表示するリスト
		/// </summary>
		ReadOnlyCollection<AchievementInfo> CurrentAchievementInfoList { get; }

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
		event EventHandler OnCurrentAchievementInfoListChange;

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
		/// アチーブメントパラメータリストを更新
		/// </summary>
		/// <param name="infos"></param>
		void SetCurrentAchievementInfoList( IList<AchievementInfo> infos );

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
		List<GUIAchievementItem> GetAchievementItemList();

		#endregion ==== アクション ====
	}

	public class Model : IModel {

		#region ==== フィールド ====

		private GUIAchievementItemScrollView achievementItemScrollView = null;

		private readonly List<AchievementInfo> currentViewAchievementInfo = new List<AchievementInfo>();

		private bool pageButtonEnabele = true;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		/// <summary>
		/// メールスクロールビュー
		/// </summary>
		public GUIAchievementItemScrollView AchievementItemScrollView { get { return achievementItemScrollView; } }

		/// <summary>
		/// 表示するリスト
		/// </summary>
		public ReadOnlyCollection<AchievementInfo> CurrentAchievementInfoList {
			get { return currentViewAchievementInfo.AsReadOnly(); }
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
		public event EventHandler OnCurrentAchievementInfoListChange = (sender, e) => { };

		#endregion ==== イベント ====

		#region ==== コンストラクタ ====

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="itemScrollView"></param>
		/// <param name="viewAttach"></param>
		public Model( GUIAchievementItemScrollView itemScrollView, PageScrollViewAttach viewAttach ) {

			achievementItemScrollView = itemScrollView;
			AchievementItemScrollView.Create( viewAttach, null );
		}

		#endregion ==== コンストラクタ ====

		#region ==== 初期化 ====

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup() {

			CurrentPage = -1;
			currentViewAchievementInfo.Clear();
			AchievementItemScrollView.Clear();
		}

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose() {

			currentViewAchievementInfo.Clear();

			OnPageChange = null;
			OnCurrentAchievementInfoListChange = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクション ====

		/// <summary>
		/// リストを更新
		/// </summary>
		/// <param name="infos"></param>
		public void SetCurrentAchievementInfoList( IList<AchievementInfo> infos ) {

			currentViewAchievementInfo.Clear();
			currentViewAchievementInfo.AddRange( infos );

			OnCurrentAchievementInfoListChange( this, EventArgs.Empty );
		}

		/// <summary>
		/// ページを変更
		/// </summary>
		public void SetPage( int page ) {

			AchievementItemScrollView.ScrollReset();
			if( AchievementItemScrollView.SetPage( page, 0 ) ) {
				CurrentPage = page;
			}
			OnPageChange( this, EventArgs.Empty );
		}

		/// <summary>
		/// ページ戻る
		/// </summary>
		public void BackPage() {

			AchievementItemScrollView.ScrollReset();
			if( AchievementItemScrollView.SetNextPage( -1 ) ) {
				CurrentPage = AchievementItemScrollView.PageIndex;
				OnPageChange( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// 一番後ろに戻る
		/// </summary>
		public void BackEndPage() {

			AchievementItemScrollView.ScrollReset();
			if( AchievementItemScrollView.SetPage( 0, 0 ) ) {
				CurrentPage = AchievementItemScrollView.PageIndex;
				OnPageChange( this, EventArgs.Empty );
			}
		}


		/// <summary>
		/// ページを進める
		/// </summary>
		public void NextPage() {

			AchievementItemScrollView.ScrollReset();
			if( AchievementItemScrollView.SetNextPage( 1 ) ) {
				CurrentPage = AchievementItemScrollView.PageIndex;
				OnPageChange( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// 一番最後のページに進める
		/// </summary>
		public void NextEndPage() {

			AchievementItemScrollView.ScrollReset();
			if( AchievementItemScrollView.SetPage( AchievementItemScrollView.PageMax - 1, 0 ) ) {
				CurrentPage = AchievementItemScrollView.PageIndex;
				OnPageChange( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// トータルのアイテム数をセットする
		/// </summary>
		/// <param name="count"></param>
		public void SetTotalItemCount( int count ) {

			AchievementItemScrollView.ScrollReset();
			AchievementItemScrollView.Setup( count, 0 );
		}

		/// <summary>
		/// リストの位置を修正する
		/// </summary>
		public void Reposition() {

			AchievementItemScrollView.ScrollReset();
			AchievementItemScrollView.Reposition();
		}

		/// <summary>
		/// アイテムリストを取得
		/// </summary>
		/// <returns></returns>
		public List<GUIAchievementItem> GetAchievementItemList() {

			List<GUIAchievementItem> itemList = new List<GUIAchievementItem>();

			for( int i = 0 ; i < AchievementItemScrollView.ItemMax ; i++ ) {
				itemList.Add( AchievementItemScrollView.GetItem( i ) );
			}
			return itemList;
		}

		#endregion ==== アクション =====
	}
}
