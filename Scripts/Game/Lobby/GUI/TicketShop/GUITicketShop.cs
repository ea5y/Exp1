/// <summary>
/// チケットショップ
/// 
/// 2016/06/15
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUI.TicketShop;
using Asobimo.WebAPI;
using Asobimo.Purchase;

// 未使用ワーニング抑制
#pragma warning disable 414

[DisallowMultipleComponent]
[RequireComponent( typeof( TicketShopView ) )]
public class GUITicketShop : Singleton<GUITicketShop> {

	#region ==== 文字列 ====

	private static string networkErrorMessage = MasterData.GetText( TextType.TX529_CharaTicket_NetworkErrorMessage );

	#endregion ==== 文字列 ====

	#region ==== フィールド ====

	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private TicketShopView _viewAttach = null;

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
	private TicketShopView ViewAttach { get { return _viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	private bool IsStartActive { get { return _isStartActive; } }

    /// <summary>
    /// 当前角色券总数
    /// </summary>
    private int curTicketNum = 0;
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
		Controller = new Controller( model, view );
		Controller.OnTicketPurchaseButtonEvent += handleTicketPurchaseButtonEvent;
		Controller.OnTermsButtonEvent += handleTermsButtonEvent;
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

	#region ==== ポーズ状態検知 ====

	/// <summary>
	/// ポーズ状態検知
	/// </summary>
	/// <param name="pause"></param>
	public void OnApplicationPause( bool pause ) {

		if( !pause ) {
			// 強制更新終了
			NetworkController.IsForceService = false;
		}
	}

	#endregion ==== ポーズ状態検知 ====

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
			// リスト更新
			updateList();

		} else {
			// 念の為、Photon強制更新をOFFにする
			NetworkController.IsForceService = false;
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

	#endregion ==== 各種情報更新 ====

	#region ==== アクション ====

	/// <summary>
	/// リスト更新
	/// </summary>
	public void updateList() {

		// マーケット商品リストの設定
#if PLATFORM_XIAOYOU || ANDROID_XY
        getXiaoYouProductList();
#else
        getProductList();
#endif

        // 商品ステータスリストの要求
		charaShopListRequest();
	}

	/// <summary>
	/// マーケット側の商品一覧を取得する
	/// </summary>
	private void getProductList() {

		int count = 0;
		ProductData[] productList = null;

		// 課金可能かどうかチェック
		if( PurchaseManager.Instance.PlatformPurchase.EnablePurchase ) {
			// 辞書の取得
			var list = PurchaseManager.Instance.PlatformPurchase.InappProductDataList;

			// 配列作成
			productList = new ProductData[list.Count];

			// 商品リストを使いやすいように配列化
			foreach( KeyValuePair<string, ProductData> data in list ) {
				productList[count++] = ( !data.Equals( null ) ) ? data.Value : null;
			}
		}

		// リストを登録
		Controller.SetProductList( productList );
	}

    void getXiaoYouProductList()
    {
#if UNITY_ANDROID
        if (!string.IsNullOrEmpty(AsobimoWebAPI.Instance.AsobimoId))
        {
            AsobimoWebAPI.Instance.GetTicketList<TicketList>((response) =>
            {
                if (response.Result.ProductArray != null && response.Result.ProductArray.Length > 0)
                {
                    int count = 0;
                    ProductData[] productList = null;
                    productList = new ProductData[response.Result.ProductArray.Length];

                    foreach (var item in response.Result.ProductArray)
                    {
                        productList[count] = new ProductData(item.onetime_payment_type_code, 
                            PurchaseItemType.Inapp, 
                            item.onetime_payment_type_name, 
                            item.product_id_key, item.price_total, 
                            item.onetime_payment_way_code, 
                            item.onetime_payment_name, 
                            item.onetime_payment_name,
                            item.today_paid_count,
                            item.daily_limit);
                        count++;
                    }

                    Controller.SetProductList(productList);
                }
            });
        }
#endif   
    }

	/// <summary>
	/// 購入完了
	/// </summary>
	private void purchaseCompletion( PurchaseState state, AsobimoPurchaseState asobimoState ) {

		if( state == PurchaseState.Complete ) {
			// マーケット側正常終了
			if( asobimoState == AsobimoPurchaseState.Registered ) {
				// 登録まで完了

				// リスト更新
				updateList();

			} else {
				// アソビモ側エラー
			}
		} else {
			// マーケット側エラー
		}
	}

	/// <summary>
	/// チケット購入ボタンが押された時
	/// </summary>
	void handleTicketPurchaseButtonEvent( object sender, TicketPurchaseEventArgs args ) {

		// 強制的にPhotonServiceを更新するフラグ
		NetworkController.IsForceService = true;

        // 購入
#if PLATFORM_XIAOYOU || ANDROID_XY
        if (AuthEntry.Instance.AuthMethod != null)
        {
            AuthEntry.Instance.AuthMethod.Purchase(args.ProductID);
            StopCoroutine("RefreshPurchaseCompletion");
            StartCoroutine("RefreshPurchaseCompletion");
        }
#else
        PurchaseManager.Instance.Purchase( args.ProductID, purchaseCompletion );
#endif   
	}

    int refreshPurchaseCount = 10;
    IEnumerator RefreshPurchaseCompletion()
    {
        curTicketNum = Controller.GetTicketNum();
        refreshPurchaseCount = 5;
        while (refreshPurchaseCount > 0)
        {
            //Debug.Log("refreshPurchaseCount:" + refreshPurchaseCount);
            yield return new WaitForSeconds(3);
            refreshPurchaseCount--;
            updateList();
        }
    }

	/// <summary>
	/// 規約ボタンが押された時
	/// </summary>
	void handleTermsButtonEvent( object sender, EventArgs args ) {

		// 規約画面をWebViewで開く
		GUIWebView.Open( AsobimoWebAPI.Instance.GetOutlineRuleTicketURL(), true );
	}

	#endregion ==== アクション ====

	#region ==== 通信系 ====

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
	/// 商品ステータスリストの要求結果
	/// </summary>
	private void charaShopListResponse( AsobimoWebAPI.WebAPIRequest<ProductStatusList> req ) {

		int loop = 0;
		int max = 0;
		int count = 0;
		CharaShopItemInfo info = null;

		// エラーチェック
		if( req.HasError ) {
			GUIMessageWindow.SetModeOK( string.Format( networkErrorMessage, req.Error, req.HttpStatus ), errorClose );
			return;
		}

		// 商品数の取得
		max = req.Result.ProductNum;

		// 上限カウント
		count = req.Result.TicketNum;

		// 購入確認
		for( loop = 0 ; loop < max ; loop++ ) {
			info = new CharaShopItemInfo( req.Result.ProductArray[loop] );
			if( info.ProductStatus != CharaShopItemInfo.Status.Limit ) continue;

			// 購入済ならばカウント
			count++;
		}

		// 現在の所持チケット枚数と上限を設定
		Controller.SetTicketPurchaseLimit( max - count );
		Controller.SetTicketNum( req.Result.TicketNum );
        if (curTicketNum != Controller.GetTicketNum())
        {
            refreshPurchaseCount = -1;
        }
	}

	/// <summary>
	/// エラーで全てのUIを閉じる
	/// </summary>
	private void errorClose() {

		// 全て閉じる
		GUIController.Clear();
	}

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
