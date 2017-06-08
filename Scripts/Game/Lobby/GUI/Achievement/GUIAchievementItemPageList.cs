/// <summary>
/// アチーブメントアイテムページリスト
/// 
/// 2016/05/13
/// </summary>

using UnityEngine;
using System;
using System.Collections.Generic;
using XUI.AchievementItemPageList;
using Scm.Common.Packet;

/// <summary>
/// アチーブメントのスクロールビュー操作GUI
/// </summary>
[DisallowMultipleComponent]
[RequireComponent( typeof( AchievementItemPageListView ) )]
public class GUIAchievementItemPageList : MonoBehaviour {

	#region ==== フィールド ====

	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private AchievementItemPageListView viewAttach = null;

	/// <summary>
	/// スクロールビュー設定
	/// </summary>
	[SerializeField]
	private GUIAchievementItemScrollView itemScrollView = null;

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	private AchievementItemPageListView ViewAttach { get { return viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }

	#endregion ==== プロパティ ====

	#region ==== イベント ====

	/// <summary>
	/// ページ変更イベント
	/// </summary>
	public event EventHandler<PageChangeEventArgs> OnPageChange = (sender, e) => { };

	#endregion ==== イベント ====

	#region ==== 初期化 ====

	/// <summary>
	/// メンバー初期化
	/// </summary>
	private void MemberInit() {

		Controller = null;
	}

	/// <summary>
	/// 起動
	/// </summary>
	private void Awake() {

		MemberInit();
		Construct();
	}

	/// <summary>
	/// 組み立て
	/// </summary>
	private void Construct() {

		// モデル生成
		var model = new Model( itemScrollView, viewAttach.PageScrollViewAttach );

		// ビュー生成
		IView view = null;
		if( ViewAttach != null ) {
			view = ViewAttach.GetComponent( typeof( IView ) ) as IView;
		}

		// コントローラー生成
		Controller = new Controller( model, view );
		Controller.OnPageChange += HandlePageChange;
	}


	#endregion ==== 初期化 ====

	#region ==== 破棄 ====

	/// <summary>
	/// オブジェクトが破棄されたとき
	/// </summary>
	private void OnDestroy() {

		if( Controller != null ) {
			Controller.Dispose();
			Controller = null;
		}
	}

	#endregion ==== 破棄 ====

	#region ==== アクティブ設定 ====

	/// <summary>
	/// アクティブ設定
	/// </summary>
	public void SetActive( bool isActive ) {

		if( Controller != null ) {
			Controller.SetActive( isActive, false );
		}
	}

	#endregion ==== アクティブ設定 ====

	#region ==== 各種情報更新 ====

	/// <summary>
	/// 初期設定
	/// </summary>
	public void Setup() {

		if( Controller != null ) {
			Controller.Setup();
		}
	}

	#endregion ==== 各種情報更新 ====

	#region ==== アクション ====

	/// <summary>
	/// アイテム数をセットする
	/// </summary>
	/// <param name="count"></param>
	public void SetItemCount( int count ) {

		if( Controller == null ) return;

		Controller.SetTotalItemCount( count );
	}

	/// <summary>
	/// 指定ページに変更する
	/// </summary>
	/// <param name="page"></param>
	public void SetPage( int page ) {

		if( Controller == null ) return;

		Controller.SetPage( page );
	}

	/// <summary>
	/// 再度ページを開く
	/// </summary>
	public void ReopenPage() {

		if( Controller == null ) return;

		Controller.ReopenPage();
	}



	/// <summary>
	/// リスト更新
	/// </summary>
	/// <param name="infos"></param>
	public void SetViewAchievementInfoList( List<AchievementInfo> infos ) {

		if( Controller == null ) return;

		Controller.SetViewItem( infos );
	}

	/// <summary>
	/// 今のリストを更新する
	/// </summary>
	public void UpdateCurrentList() {

		if( Controller == null ) return;

		Controller.UpdateCurrentList();
	}


	/// <summary>
	/// ページ変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandlePageChange( object sender, PageChangeEventArgs e ) {

		OnPageChange( this, e );
	}

	#endregion ==== アクション ====
}
