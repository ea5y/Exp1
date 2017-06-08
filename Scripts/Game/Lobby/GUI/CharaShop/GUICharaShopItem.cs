/// <summary>
/// キャラクターショップアイテム
/// 
/// 2016/06/15
/// </summary>

using UnityEngine;
using XUI.CharaShopItem;
using Asobimo.WebAPI;

[DisallowMultipleComponent]
[RequireComponent( typeof( CharaShopItemView ) )]
public class GUICharaShopItem : MonoBehaviour {

	#region ==== 文字列 ====

	private static string networkErrorMessage				= MasterData.GetText( TextType.TX529_CharaTicket_NetworkErrorMessage );
	private static string ticketExchangeMessage				= MasterData.GetText( TextType.TX530_CharaShop_TicketExchangeMessage );
	private static string purchaseError_NotBeRetrieved		= MasterData.GetText( TextType.TX531_CharaShop_Error_NotBeRetrieved );
	private static string purchaseError_AcquisitionLimit	= MasterData.GetText( TextType.TX532_CharaShop_Error_AcquisitionLimit );
	private static string purchaseError_PointShortage		= MasterData.GetText( TextType.TX533_CharaShop_Error_PointShortage );
	private static string purchaseAcquisitionCompletion		= MasterData.GetText( TextType.TX534_CharaShop_AcquisitionCompletion );

	#endregion ==== 文字列 ====

	#region ==== フィールド ====

	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private CharaShopItemView _viewAttach = null;

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// ビュー
	/// </summary>
	private CharaShopItemView ViewAttach { get { return _viewAttach; } }

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
	public static GUICharaShopItem Create( GameObject prefab, Transform parent, int index ) {

		// プレハブの確認
		UnityEngine.Assertions.Assert.IsNotNull( prefab, "GUICharaShopItem:'prefab' Not Found!!" );

		// インスタンス作成
		var go = SafeObject.Instantiate( prefab ) as GameObject;
		UnityEngine.Assertions.Assert.IsNotNull( prefab, "GUICharaShopItem:'SafeObject.Instantiate(prefab) as GameObject' Not Found!!" );

		// 名前
		go.name = string.Format( "{0}{1}", prefab.name, index );

		// 関連付け
		go.transform.SetParent( parent, false );

		// 有効化
		if( !go.activeSelf ) {
			go.SetActive( true );
		}

		// コンポーネント取得
		var item = go.GetComponent( typeof( GUICharaShopItem )) as GUICharaShopItem;
		UnityEngine.Assertions.Assert.IsNotNull( item, "GUICharaShopItem:'go.GetComponent(typeof(GUICharaShopItem)) as GUICharaShopItem' Not Found!!" );
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

		//gameObject.SetActive( true );

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
		Controller.OnTicketExchangeButtonEvent += handleTicketExchangeButtonEvent;
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
	/// ショップ情報をセットする
	/// </summary>
	/// <param name="info"></param>
	public void SetShopInfo( CharaShopItemInfo info ) {

		if( Controller == null ) return;

		Controller.SetShopInfo( info );
	}

	/// <summary>
	/// ショップ情報を更新する
	/// </summary>
	public void UpdateShopInfo() {

		if( Controller == null ) return;

		Controller.UpdateShopInfo();
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
	/// チケット交換ボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void handleTicketExchangeButtonEvent( object sender, TicketExchangeEventArgs e ) {

		// 購入確認
		GUIMessageWindow.SetModeYesNo( string.Format( ticketExchangeMessage, e.Name ), () => { ticketExchange( e.ProductID ); }, null );
	}

	#endregion ==== イベント ====

	#region ==== アクション ====

	/// <summary>
	/// キャラクターショップを開きなおす
	/// </summary>
	private void charaShopReOpen() {

		// リスト更新
		GUICharaShop.Instance.RefreshList();
	}

	#endregion ==== アクション ====

	#region ==== 通信 ====

	#region ---- チケット交換 ----

	/// <summary>
	/// チケット交換
	/// </summary>
	/// <param name="id"></param>
	private void ticketExchange( int id ) {

		// 通信中
		GUISystemMessage.SetModeConnect( MasterData.GetText( TextType.TX305_Network_Communication ) );

		// キャラクター購入
		Asobimo.WebAPI.AsobimoWebAPI.Instance.PurchaseProduct<Asobimo.WebAPI.PurchaseProduct>( id, 1, null, null, ticketExchangeResponse );
	}

	/// <summary>
	/// チケット交換結果
	/// </summary>
	private void ticketExchangeResponse( Asobimo.WebAPI.AsobimoWebAPI.WebAPIRequest<Asobimo.WebAPI.PurchaseProduct> req ) {

		// 通信中メッセージを閉じる
		GUISystemMessage.Close();

		string msg = "";

		// 通信エラーチェック
		if( req.HasError ) {
			GUIMessageWindow.SetModeOK( string.Format( networkErrorMessage, req.Error, req.HttpStatus ), null );
			return;
		}

		// 購入結果
		switch( req.Result.ProductObtainCode ) {
			case ObtainCode.Completion:
				// 正常取得完了
				Scm.Common.XwMaster.CharaMasterData charaData;
				Scm.Common.XwMaster.ShopItemMasterData shopData;

				if( MasterData.TryGetShop( req.Result.Product.GameID, out shopData ) ) {
					// キャラクターデータ取得
					if( MasterData.TryGetChara( shopData.CharacterId, out charaData ) ) {
						// 取得ダイアログ表示
						msg = string.Format( purchaseAcquisitionCompletion, charaData.Name );

					} else {
						// キャラクターデータ取得失敗
						msg = string.Format( networkErrorMessage, "TryGetCharaError", shopData.CharacterId );
					}
				} else {
					// ショップデータ取得失敗
					msg = string.Format( networkErrorMessage, "TryGetShopError", req.Result.Product.GameID );
				}
				// WebStore取得要求
				RequestWebStore();

				break;

			case ObtainCode.NotBeRetrieved:
				// 取得不可能
				msg = purchaseError_NotBeRetrieved;
				break;

			case ObtainCode.AcquisitionLimit:
				// 取得上限
				msg = purchaseError_AcquisitionLimit;
				break;

			case ObtainCode.PointShortage:
				// ポイント不足
				msg = purchaseError_PointShortage;
				break;
		}
		GUIMessageWindow.SetModeOK( msg, charaShopReOpen );
	}

	#endregion ---- チケット交換 ----

	#region ---- WebStore パケット ----

	/// <summary>
	/// WebStoreの取得要求
	/// </summary>
	void RequestWebStore() {

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
		GUICharaTicket.Instance.UpdateWebStoreCheckTime();
	}

	#endregion ---- WebStore パケット ----

	#endregion ==== 通信 ====
}
