/// <summary>
/// メールアイテム
/// 
/// 2016/05/16
/// </summary>

using UnityEngine;
using XUI.AchievementItem;

[DisallowMultipleComponent]
[RequireComponent( typeof( AchievementItemView ) )]
public class GUIAchievementItem : MonoBehaviour {

	#region ==== フィールド ====

	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private AchievementItemView _viewAttach = null;

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// ビュー
	/// </summary>
	private AchievementItemView ViewAttach { get { return _viewAttach; }}

	/// <summary>
	/// コントローラ
	/// </summary>
	private IController Controller { get; set; }

	#endregion ==== プロパティ ====

	#region ==== 作成 ====

	/// <summary>
	/// アイテム作成
	/// </summary>
	/// <param name="prefab">ベース</param>
	/// <param name="parent">登録先</param>
	/// <param name="index">インデックス</param>
	/// <returns></returns>
	public static GUIAchievementItem Create( GameObject prefab, Transform parent, int index ) {

		// プレハブの確認
		UnityEngine.Assertions.Assert.IsNotNull( prefab, "GUIAchievementItem:'prefab' Not Found!!" );

		// インスタンス作成
		var go = SafeObject.Instantiate( prefab ) as GameObject;
		UnityEngine.Assertions.Assert.IsNotNull( prefab, "GUIAchievementItem:'SafeObject.Instantiate(prefab) as GameObject' Not Found!!" );

		// 名前
		go.name = string.Format( "{0}{1}", prefab.name, index );

		// 関連付け
		go.transform.SetParent( parent, false );

		// 有効化
		if( !go.activeSelf ) {
			go.SetActive( true );
		}

		// コンポーネント取得
		var item = go.GetComponent( typeof( GUIAchievementItem )) as GUIAchievementItem;
		UnityEngine.Assertions.Assert.IsNotNull( item, "GUIAchievementItem:'go.GetComponent(typeof(GUIAchievementItem)) as GUIAchievementItem' Not Found!!" );
		item.Initialize();

		return item;
	}

	#endregion ==== 作成 ====

	#region ==== 初期化 ====

	/// <summary>
	/// メンバーの初期化
	/// </summary>
	private void MemberInit() {

		this.Controller = null;
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize() {

		this.MemberInit();
		this.Construct();
		this.Setup();
	}

	/// <summary>
	/// 組み立て
	/// </summary>
	private void Construct() {

		// モデル
		var model = new Model();

		// ビュー
		IView view = null;
		if( this.ViewAttach != null ) {
			view = this.ViewAttach.GetComponent( typeof( IView ) ) as IView;
		}

		// コントローラ作成
		var controller = new Controller( model, view );
		Controller = controller;
		Controller.OnGetButtonClick += handleGetButtonClick;
	}

	#endregion ==== 初期化 ====

	#region ==== 各種情報更新 ====

	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup() {

		if( this.Controller != null ) {
			this.Controller.Setup();
		}
	}

	/// <summary>
	/// アチーブメント情報をセットする
	/// </summary>
	/// <param name="info"></param>
	public void SetAchievementInfo( AchievementInfo info ) {

		if( Controller == null ) return;

		Controller.SetAchievementInfo( info );
	}

	/// <summary>
	/// アチーブメント情報を更新する
	/// </summary>
	public void UpdateAchievementInfo() {

		if( Controller == null ) return;

		Controller.UpdateAchievementInfo();
	}

	#endregion ==== 各種情報更新 ====

	#region ==== 破棄 ====

	/// <summary>
	/// 破棄
	/// </summary>
	private void OnDestroy() {

		// コントローラの破棄
		if( Controller != null ) {
			Controller.Dispose();
			Controller = null;
		}
	}

	#endregion ==== 破棄 ====

	#region ==== アクティブ ====

	/// <summary>
	/// アクティブ設定
	/// </summary>
	/// <param name="isActive"></param>
	private void SetActive( bool isActive ) {

		if( this.Controller != null ) {
			this.Controller.SetActive( isActive );
		}
	}

	#endregion ==== アクティブ ====

	#region ==== イベント ====

	/// <summary>
	/// 取得ボタンを押下した時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void handleGetButtonClick( object sender, GetRewardEventArgs e ) {

		GUIAchievement.GetReward( e.AchieveMasterID );
	}

	#endregion ==== イベント ====
}
