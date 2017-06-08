/// <summary>
/// キャラクターショップ
/// 
/// 2016/06/16
/// </summary>

using UnityEngine;
using XUI.CharaShop;
using System.Collections.Generic;
using Asobimo.WebAPI;

[DisallowMultipleComponent]
[RequireComponent( typeof( CharaShopView ) )]
public class GUICharaShop : Singleton<GUICharaShop> {

	#region ==== 文字列 ====

	private static string networkErrorMessage = MasterData.GetText( TextType.TX529_CharaTicket_NetworkErrorMessage );

	#endregion ==== 文字列 ====

	#region ==== フィールド ====

	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private CharaShopView _viewAttach = null;

	/// <summary>
	/// ページリスト
	/// </summary>
	[SerializeField]
	private GUICharaShopItemPageList charaShopItemPageList = null;

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// ビュー
	/// </summary>
	private CharaShopView ViewAttach { get { return _viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	private bool IsStartActive { get { return _isStartActive; } }

	#endregion ==== プロパティ ====

	#region ==== 初期化 ====

	/// <summary>
	/// 起動
	/// </summary>
	protected override void Awake() {

		base.Awake();
		MemberInit();
	}

	/// <summary>
	/// 開始
	/// </summary>
	private void Start() {

		Construct();

		// 初期アクティブ設定
		SetActive( IsStartActive, true, IsStartActive );
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit() {

		Controller = null;
	}

	/// <summary>
	/// 作成
	/// </summary>
	private void Construct() {

		// モデル生成
		var model = new Model();

		// ビュー生成
		IView view = null;
		if( ViewAttach != null ) {
			view = ViewAttach.GetComponent( typeof( IView ) ) as IView;
		}

		// コントローラー生成
		Controller = new Controller( model, view, charaShopItemPageList );
	}

	#endregion ==== 初期化 ====

	#region ==== 破棄 ====

	/// <summary>
	/// 破棄
	/// </summary>
	private void OnDestroy() {

		if( Controller != null ) {
			Controller.Dispose();
		}
	}

	#endregion ==== 破棄 ====

	#region ==== 更新 ====

	/// <summary>
	/// 更新
	/// </summary>
	private void Update() {

		// マッチング中であればクローズ
		if( GUIMatchingState.IsMatching ) {
			GUIController.Clear();
		}
	}

	#endregion ==== 更新 ====

	#region ==== アクティブ設定 ====

	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close() {

		if( Instance != null ) Instance.SetActive( false, false, false );
	}

	/// <summary>
	/// 開く
	/// </summary>
	public static void Open() {

		if( Instance != null ) Instance.SetActive( true, false, true );
	}

	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen() {

		if( Instance != null ) Instance.SetActive( true, false, false );
	}

	/// <summary>
	/// アクティブ設定
	/// </summary>
	private void SetActive( bool isActive, bool isTweenSkip, bool isSetup ) {

		// セットアップ
		if( isSetup ) {
			Setup();
		}

		// コントローラのアクティブ設定
		if( Controller != null ) {
			Controller.SetActive( isActive, isTweenSkip );
		}

		if( isActive ) {
			// 商品ステータスリストの要求
			RefreshList();
		}

		// マルチタップ設定(アクティブな場合OFF)
		Input.multiTouchEnabled = !isActive;
	}

	#endregion ==== アクティブ設定 ====

	#region ==== 各種情報更新 ====

	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup() {

		// コントローラセットアップ
		if( Controller != null ) {
			Controller.Setup();
		}
	}

	/// <summary>
	/// リスト更新
	/// </summary>
	public void RefreshList() {

		// 商品ステータスリストの要求
		charaShopListRequest();
	}

	#endregion ==== 各種情報更新 ====

	#region ==== 通信系 ====

	#region ---- 商品ステータスリスト ----

	/// <summary>
	/// 商品ステータスリストの要求
	/// </summary>
	private void charaShopListRequest() {

#if UNITY_ANDROID || UNITY_IOS
		// 商品ステータスリストの要求
		if( !string.IsNullOrEmpty( AsobimoWebAPI.Instance.AsobimoId ) ) {
			AsobimoWebAPI.Instance.GetProductStatusList<ProductStatusList>( "", "", "", 0, charaShopListResponse );
		}
#endif
	}

	/// <summary>
	///  商品ステータスリストの要求結果
	/// </summary>
	private void charaShopListResponse( AsobimoWebAPI.WebAPIRequest<ProductStatusList> req ) {

		List<CharaShopItemInfo> list = new List<CharaShopItemInfo>();

		// エラーチェック
		if( !req.HasError ) {
			// 商品個数の取得
			int length = req.Result.ProductNum;

			// 変換してリストへ追加
			for( int i = 0 ; i < length ; i++ ) {
                Scm.Common.XwMaster.ShopItemMasterData shopData;
                // Plate number review special logic: disable some characters
                if (!MasterData.TryGetShop(req.Result.ProductArray[i].GameID, out shopData)) {
                    continue;
                }
                if (!Scm.Common.Master.CharaMaster.Instance.IsValidCharacter(shopData.CharacterId)) {
                    continue;
                }
                list.Add( new CharaShopItemInfo( req.Result.ProductArray[i] ) );
			}

			// チケット枚数設定
			ViewAttach.SetTicketNum( req.Result.TicketNum );

		} else {
			// 通信エラー
			GUIMessageWindow.SetModeOK( string.Format( networkErrorMessage, req.Error, req.HttpStatus ), errorClose );
		}
		// リスト一覧更新
		Controller.UpdateList( list );
	}

	/// <summary>
	/// 通信エラーでメニューを閉じる
	/// </summary>
	private void errorClose() {

		// 全て閉じる
		GUIController.Clear();
	}

	#endregion ---- 商品ステータスリスト ----

	#endregion ==== 通信系 ====



	#region ==== デバッグ ====

#if UNITY_EDITOR && XW_DEBUG

	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();

	GUIDebugParam DebugParam { get { return _debugParam; } }

	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase {
		public GUIDebugParam() {
		}

		[SerializeField]
		int max = 10;
		public int Max { get { return max; } }

		[SerializeField]
		TemprateEvent _list = new TemprateEvent();

		public TemprateEvent List { get { return _list; } }

		[System.Serializable]
		public class TemprateEvent : IDebugParamEvent {
			public event System.Action Execute = delegate { };

			[SerializeField]
			bool execute = false;

			public void Update() {
				if( this.execute ) {
					this.execute = false;
					this.Execute();
				}
			}
		}
	}

	private void DebugInit() {

		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () => {
			d.ReadMasterData();
			Open();
		};

		d.List.Execute += () => {
			// デバッグ用適当リスト作成
			List<CharaShopItemInfo> info = new List<CharaShopItemInfo>();
			for( int i = 0 ; i < d.Max ; i++ ) {
				info.Add( new CharaShopItemInfo( i, 0, 0, i, 100, "Name:" + i, "Info:" + i, CharaShopItemInfo.Status.Normal ) );
			}

			// 登録
			Controller.UpdateList( info );
		};
	}

	bool _isDebugInit = false;

	private void DebugUpdate() {

		if( !this._isDebugInit ) {
			this._isDebugInit = true;
			this.DebugInit();
		}

		this.DebugParam.Update();
		this.DebugParam.List.Update();
	}

	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate() {

		if( Application.isPlaying ) {
			this.DebugUpdate();
		}
	}

#endif

	#endregion ==== デバッグ ====
}
