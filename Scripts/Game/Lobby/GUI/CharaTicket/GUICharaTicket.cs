/// <summary>
/// キャラクターチケット
/// 
/// 2016/06/14
/// </summary>

using System;
using UnityEngine;
using XUI.CharaTicket;
using Asobimo.WebAPI;

[DisallowMultipleComponent]
[RequireComponent( typeof( CharaTicketView ) )]
public class GUICharaTicket : Singleton<GUICharaTicket> {

	#region ==== 文字列 ====

	private static string shopMaintenanceMessage	= MasterData.GetText( TextType.TX536_CharaTicket_ShopMaintenanceMessage );
	private static string networkErrorMessage		= MasterData.GetText( TextType.TX529_CharaTicket_NetworkErrorMessage );
	private static string tokenRefreshError			= "Token Refresh Error";

	#endregion ==== 文字列 ====

	#region ==== フィールド ====

	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private CharaTicketView _viewAttach = null;

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;

	/// <summary>
	/// 更新間隔
	/// </summary>
	private float updateInterval = 180;

	/// <summary>
	/// 前回のWebStore更新時間(Unity実行時間)
	/// </summary>
	private float webStoreCheckTime = 0;

	/// <summary>
	/// メニューカウント
	/// </summary>
	private int menuCount = 0;

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// ビュー
	/// </summary>
	private CharaTicketView ViewAttach { get { return _viewAttach; } }

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

		// 初回はすぐに更新できるよう
		webStoreCheckTime = -updateInterval;
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
		Controller = new Controller( model, view );
		Controller.OnCharaExchange			+= handleExchangeChara;
		Controller.OnCharaTicketPurchase	+= handleTicketPurchase;
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

		if( Instance != null ) {
			Instance.SetActive( false, false, false );
			Instance.checkMenu( false );
		}
	}

	/// <summary>
	/// 開く
	/// </summary>
	public static void Open() {
		if( Instance != null ) {
			Instance.SetActive( true, false, true );
			Instance.checkMenu( true );
		}
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
			// トークンチェック
			checkAsobimoTokenRequest();
		}
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
	/// WebStore確認時間更新
	/// </summary>
	public void UpdateWebStoreCheckTime() {

		// 現在の実行時間をセット
		webStoreCheckTime = Time.realtimeSinceStartup;
	}

	#endregion ==== 各種情報更新 ====

	#region ==== アクション ====

	/// <summary>
	/// メニュー確認
	/// </summary>
	/// <param name="isOpen">開いた時TRUE、閉じた時FALSE</param>
	private void checkMenu( bool isOpen ) {

		// メニューカウント
		menuCount += ( isOpen ) ? 1 : -1;

		// 完全に閉じられた場合、WebStoreを確認する
		if( menuCount <= 0 ) {
			menuCount = 0;

			// WebStore確認
			RequestWebStore();
		}
	}

	/// <summary>
	/// キャラクター交換ボタンが押された時
	/// </summary>
	void handleExchangeChara( object sender, EventArgs args ) {

		// メニュー確認
		checkMenu( true );
		// キャラクター交換メニューを開く
		GUIController.Open( new GUIScreen( GUICharaShop.Open, GUICharaShop.Close, GUICharaShop.ReOpen ) );
	}

	/// <summary>
	/// チケット購入ボタンが押された時
	/// </summary>
	void handleTicketPurchase( object sender, EventArgs args ) {

		// メニュー確認
		checkMenu( true );
		// キャラクターチケット購入メニューを開く
		GUIController.Open( new GUIScreen( GUITicketShop.Open, GUITicketShop.Close, GUITicketShop.ReOpen ) );
	}

	/// <summary>
	/// 通信エラーで全て閉じる
	/// </summary>
	private void errorClose() {

		// 全て閉じる
		GUIController.Clear();
	}

	#endregion ==== アクション ====

	#region ==== 通信系 ====

	#region ---- トークンチェック ----

	/// <summary>
	/// トークン確認要求
	/// </summary>
	void checkAsobimoTokenRequest() {

		// トークンが有効かどうか確認
        if (AuthEntry.Instance.AuthMethod != null) {
            AuthEntry.Instance.AuthMethod.CheckToken(checkAsobimoTokenResponse);
        } else {
#if XW_DEBUG
            checkAsobimoTokenResponse(true);
#endif
        }
        
	}

	/// <summary>
	/// トークン確認要求結果
	/// </summary>
	void checkAsobimoTokenResponse( bool enable ) {

		if( enable ) {
			// トークンが有効なので、ショップメンテナンス確認へ
			checkShopMeintenanceRequest();

		} else {
			// トークンが無効なので更新要求
			updateAsobimoTokenRequest();
		}
	}

	#endregion ---- トークンチェック ----

	#region ---- トークン更新 ----

	/// <summary>
	/// トークン更新要求
	/// </summary>
	void updateAsobimoTokenRequest() {

		// トークン更新
        AuthEntry.Instance.AuthMethod.RefreshToken(updateAsobimoTokenResponse);
	}

	/// <summary>
	/// トークン更新要求結果
	/// </summary>
	void updateAsobimoTokenResponse( bool enable ) {

		if( enable ) {
			// 更新成功なので、ショップメンテナンス確認へ
			checkShopMeintenanceRequest();

		} else {
			// 更新失敗
			GUIMessageWindow.SetModeOK( string.Format( networkErrorMessage, tokenRefreshError, 0 ), errorClose );
		}
	}

	#endregion ---- トークン更新 ----

	#region ---- ショップメンテナンス確認 ----

	/// <summary>
	/// ショップメンテナンス状態の要求
	/// </summary>
	private void checkShopMeintenanceRequest() {

#if UNITY_ANDROID || UNITY_IOS
		// ショップメンテナンス状態の要求
		if( !string.IsNullOrEmpty( AsobimoWebAPI.Instance.AsobimoId ) ) {
			AsobimoWebAPI.Instance.CheckShopMaintenance( checkShopMeintenanceResponse );
		}
#endif
	}

	/// <summary>
	/// ショップメンテナンス状態の要求結果
	/// </summary>
	/// <param name="req"></param>
	private void checkShopMeintenanceResponse( AsobimoWebAPI.WebAPIRequest<AsobimoWebAPI.CheckShopMaintenanceResultJson> req ) {

		// エラーチェック
		if( req.HasError ) {
			GUIMessageWindow.SetModeOK( string.Format( networkErrorMessage, req.Error, req.HttpStatus ), errorClose );
			return;
		}

		// メンテナンス中ならば終了
		if( req.Result.IsShopMaintenance ) {
			// メンテナンス中ダイアログの表示
			GUIMessageWindow.SetModeOK( shopMaintenanceMessage, errorClose );
			return;
		}

		// 商品ステータスリスト要求
		charaShopListRequest();
	}

	#endregion ---- ショップメンテナンス確認 ----

	#region ---- 商品ステータスリスト ----

	/// <summary>
	/// 商品ステータスリストの要求
	/// </summary>
	private void charaShopListRequest() {
        Debug.Log("========>AsobimoId: " + AsobimoWebAPI.Instance.AsobimoId);
#if UNITY_ANDROID || UNITY_IOS
		// 商品ステータスリストの要求
		if( !string.IsNullOrEmpty( AsobimoWebAPI.Instance.AsobimoId )) {
			AsobimoWebAPI.Instance.GetProductStatusList<ProductStatusList>( "", "", "", 0, charaShopListResponse );
		}
#endif
	}

	/// <summary>
	/// 商品ステータスリストの要求結果
	/// </summary>
	/// <param name="req"></param>
	private void charaShopListResponse( AsobimoWebAPI.WebAPIRequest<ProductStatusList> req ) {

		int loop = 0;
		int max = 0;
		int limit = 0;
		int num = 0;
		CharaShopItemInfo info = null;

		// エラーチェック
		if( req.HasError ) {
			GUIMessageWindow.SetModeOK( string.Format( networkErrorMessage, req.Error, req.HttpStatus ), errorClose );
			return;
		}

		// 商品数の取得
		max = limit = req.Result.ProductNum;

		// 上限カウント
		num = req.Result.TicketNum;

		// 購入確認
		for( loop = 0 ; loop < max ; loop++ ) {
			info = new CharaShopItemInfo( req.Result.ProductArray[loop] );
			if (info.ProductStatus != CharaShopItemInfo.Status.Limit) continue;

			// 購入済ならば減算
			limit--;
		}

		// 現在の所持チケット枚数と上限を設定
		Controller.SetTicketPurchaseLimit( limit - num );
		Controller.SetTicketNum( num );
	}

	#endregion ---- 商品ステータスリスト ----

	#region ---- WebStore パケット ----

	/// <summary>
	/// WebStoreの取得要求
	/// </summary>
	void RequestWebStore() {

		// 前回から指定秒数経っていなければ無視
		if( ( Time.realtimeSinceStartup - webStoreCheckTime ) < updateInterval ) {
			return;
		}

		// WebStoreの取得要求
		LobbyPacket.SendReceiveWebStore( this.Response );
	}

	/// <summary>
	/// WebStore パケットのレスポンス
	/// </summary>
	void Response( LobbyPacket.ReceiveWebStoreResArgs args ) {

		// 「通信中」閉じる
		GUISystemMessage.Close();

		// WebStore確認時間更新
		UpdateWebStoreCheckTime();
	}

	#endregion ---- WebStore パケット ----

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
		TemprateEvent _list = new TemprateEvent();

		public TemprateEvent List { get { return _list; } }

		[System.Serializable]
		public class TemprateEvent : IDebugParamEvent {
			public event System.Action Execute = delegate { };

			[SerializeField]
			private int num = 0;
			public int Num { get { return num; } }

			[SerializeField]
			private int limit = 0;
			public int Limit { get { return limit; } }

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
			ViewAttach.ChangeTicket( d.List.Limit, d.List.Num );
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
