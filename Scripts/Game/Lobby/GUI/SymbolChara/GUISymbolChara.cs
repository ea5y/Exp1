/// <summary>
///シンボルキャラクター表示
/// 
/// 2016/03/28
/// </summary>

using UnityEngine;
using System;
using System.Collections.Generic;

using XUI.SymbolChara;

public class GUISymbolChara : Singleton<GUISymbolChara> {

	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	SymbolCharaView _viewAttach = null;
	SymbolCharaView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// キャラページリスト
	/// </summary>
	[SerializeField]
	GUICharaPageList _charaPageList = null;
	GUICharaPageList CharaPageList { get { return _charaPageList; } }

	/// <summary>
	/// ベースキャラアイテム
	/// </summary>
	[SerializeField]
	GameObject _baseCharaItem = null;
	GameObject BaseCharaItem { get { return _baseCharaItem; } }

	/// <summary>
	/// インスタンス化した現在のキャラアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	GameObject _attachNowChara = null;
	GameObject AttachNowChara { get { return _attachNowChara; } }

	/// <summary>
	/// インスタンス化した選択キャラアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	GameObject _attachSelectChara = null;
	GameObject AttachSelectChara { get { return _attachSelectChara; } }

	// 名前表示フォーマット
	[SerializeField]
	string _nameFormat = "";
	string NameFormat { get { return _nameFormat; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	// キャラ
	GUICharaItem SymbolChara { get; set; }
	GUICharaItem SelectChara { get; set; }

	// コントローラー
	IController Controller { get; set; }

	// シリアライズされていないメンバー初期化
	void MemberInit() {

		this.SymbolChara = null;
		this.SelectChara = null;
		this.Controller = null;
	}
	#endregion

	#region 初期化
	override protected void Awake() {

		base.Awake();
		this.MemberInit();
		this.CreateSymbolChara();
	}

	void Start() {

		this.Construct();

		// 初期アクティブ設定
		this.SetActive( this.IsStartActive, true, this.IsStartActive, false );
	}

	void CreateSymbolChara() {

		if( this.AttachNowChara == null ) {
			Debug.LogWarning( "AttachNowChara is Null!!" );
			return;
		}
		if( this.AttachSelectChara == null ) {
			Debug.LogWarning( "AttachSelectChara is Null!!" );
			return;
		}
		if( this.BaseCharaItem == null ) {
			Debug.LogWarning( "BaseCharaItem is Null!!" );
			return;
		}

		// インスタンス化
		this.SymbolChara = SymbolInstantite( this.BaseCharaItem, this.AttachNowChara );
		this.SelectChara = SymbolInstantite( this.BaseCharaItem, this.AttachSelectChara );
	}

	GUICharaItem SymbolInstantite( GameObject item, GameObject attach ) {

		// キャラをインスタンス化
		var obj = SafeObject.Instantiate( item );
		if( obj == null ) {
			GameObject.Destroy( obj );
			return null;
		}
		var go = obj as GameObject;
		if( go == null ) {
			Debug.LogWarning( obj.name + " is Mismatch type!! [GameObject]" );
			return null;
		}

		// 子供を消す
		attach.DestroyChild();

		// 親子付け
		go.SetParentWithLayer( attach, false );

		// アクティブ化
		if( !go.activeSelf ) {
			go.SetActive( true );
		}

		// コンポーネント取得
		GUICharaItem charaItem = go.GetComponentInChildren( typeof( GUICharaItem ) ) as GUICharaItem;
		if( charaItem == null ) {
			Debug.LogWarning( "BaseChara is null!! go.GetComponentInChildren( typeof( GUICharaItem ))" );
			return null;
		}
		return charaItem;
	}

	void Construct() {

		// モデル生成
		var model = new Model();

		// ビュー生成
		IView view = null;
		if( this.ViewAttach != null ) {
			view = this.ViewAttach.GetComponent( typeof( IView )) as IView;
		}

		// コントローラー生成
		var controller = new Controller( model, view, this.CharaPageList, this.SymbolChara, this.SelectChara );
		this.Controller = controller;
		this.Controller.OnCharaChange += this.HandleOnCharaChenge;

		// フォーマット設定
		model.SymbolNameFormat = this.NameFormat;
		model.SelectNameFormat = this.NameFormat;
	}

	/// <summary>
	/// 破棄
	/// </summary>
	void OnDestroy() {

		if( this.Controller != null ) {
			this.Controller.Dispose();
		}
	}
	#endregion

	#region 有効無効
	void OnEnable() {
		GUICharaSimpleInfo.AddLockClickEvent( this.HandleLockClickEvent );
		GUICharaSimpleInfo.AddLockResponseEvent( this.HandleLockResponse );
	}

	void OnDisable() {
		GUICharaSimpleInfo.RemoveLockClickEvent( this.HandleLockClickEvent );
		GUICharaSimpleInfo.RemoveLockResponseEvent( this.HandleLockResponse );
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close() {

		if( Instance != null ) Instance.SetActive( false, false, false, false );
	}

	/// <summary>
	/// 開く
	/// </summary>
	public static void Open() {

		if( Instance != null ) Instance.SetActive( true, false, true, false );
	}

	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen() {

		if( Instance != null ) Instance.SetActive( true, false, false, true );
	}

	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive( bool isActive, bool isTweenSkip, bool isSetup, bool reset ) {

		if( isSetup ) {
			this.Setup();
		}

		if( this.Controller != null ) {
			this.Controller.SetActive( isActive, isTweenSkip, reset );
		}
	}
	#endregion

	#region 各種情報更新
	/// <summary>
	/// 初期設定
	/// </summary>
	void Setup() {

		if( this.Controller != null ) {
			this.Controller.Setup();
		}

		// サーバーに問い合わせる
		LobbyPacket.SendPlayerCharacterBox( this.Response );
	}

	#endregion

	#region 通信系

	#region PlayerCharacterBox パケット
	/// <summary>
	/// PlayerCharacterBoxReq パケットのレスポンス
	/// </summary>
	void Response( LobbyPacket.PlayerCharacterBoxResArgs args ) {

		GUIDebugLog.AddMessage( string.Format( "PlayerCharacterBox:Capacity={0}", args.Capacity ));

		this.SetupCapacity( args.Capacity, args.Count );

		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll( this.Response );
	}

	/// <summary>
	/// 総数を設定する
	/// </summary>
	void SetupCapacity( int capacity, int count ) {

		if( this.Controller != null ) {
			this.Controller.SetupCapacity( capacity, count );
		}
	}
	#endregion

	#region PlayerCharacterAll パケット
	/// <summary>
	/// PlayerCharacterAllReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PlayerCharacterAllResArgs args) {

		GUIDebugLog.AddMessage( string.Format( "PlayerCharacterAll:List.Count={0}", args.List.Count ));

		// 「通信中」閉じる
		GUISystemMessage.Close();

		this.SetupItem( args.List );
	}

	/// <summary>
	/// 個々のアイテムの設定をする
	/// </summary>
	void SetupItem( List<CharaInfo> list ) {

		if( this.Controller != null ) {
			this.Controller.SetupItem( list );
		}
	}
	#endregion

	#region SetLockPlayerCharacter パケット
	/// <summary>
	/// ロック設定イベントハンドラー
	/// </summary>
	void HandleLockClickEvent( CharaInfo obj ) {
	
		// 「通信中」表示
		GUISystemMessage.SetModeConnect( MasterData.GetText( TextType.TX305_Network_Communication ));
	}

	/// <summary>
	/// SetLockPlayerCharacterReq パケットのレスポンス
	/// </summary>
	void HandleLockResponse( LobbyPacket.SetLockPlayerCharacterResArgs args ) {
	
		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll( this.Response );
	}
	#endregion

	#region SetSymbolPlayerCharacterReq パケット
	void HandleOnCharaChenge( object sender, CharaChangeEventArgs args ) {

		CommonPacket.SendSetSymbolPlayerCharacterReq( args.SelectCharaUUID, HandleOnCharaChengeResponse );
	}

	/// <summary>
	/// SendSetSymbolPlayerCharacterReqのレスポンス
	/// </summary>
	/// <param name="args"></param>
	void HandleOnCharaChengeResponse( CommonPacket.SetSymbolPlayerCharacterResArgs args ) {
	}

	#endregion

	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();
	GUIDebugParam DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public GUIDebugParam()
		{
			this.AddEvent(this.CharaList);
		}

		[SerializeField]
		CharaListEvent _charaList = new CharaListEvent();
		public CharaListEvent CharaList { get { return _charaList; } }
		[System.Serializable]
		public class CharaListEvent : IDebugParamEvent
		{
			public event System.Action<int> ExecuteCapacity = delegate { };
			public event System.Action<List<CharaInfo>> ExecuteList = delegate { };
			[SerializeField]
			bool executeDummy = false;
			[SerializeField]
			int capacity = 0;

			[SerializeField]
			bool executeList = false;
			[SerializeField]
			List<CharaInfo> list = new List<CharaInfo>();

			public void Update()
			{
				if (this.executeDummy)
				{
					this.executeDummy = false;

					var count = UnityEngine.Random.Range(0, this.capacity + 1);
					this.list.Clear();
					for (int i = 0; i < count; i++)
					{
						var info = new CharaInfo();
						var uuid = (ulong)(i + 1);
						info.DebugRandomSetup();
						info.DebugSetUUID(uuid);
						this.list.Add(info);
					}
					this.ExecuteCapacity(this.capacity);
					this.ExecuteList(new List<CharaInfo>(this.list.ToArray()));
				}
				if (this.executeList)
				{
					this.executeList = false;
					this.ExecuteList(new List<CharaInfo>(this.list.ToArray()));
				}
			}
		}
	}
	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () =>
			{
				d.ReadMasterData();
				Open();
			};
		d.CharaList.ExecuteCapacity += (capacity) => { this.SetupCapacity(capacity, capacity); };
		d.CharaList.ExecuteList += (list) => { this.SetupItem(list); };
	}
	bool _isDebugInit = false;
	void DebugUpdate()
	{
		if (!this._isDebugInit)
		{
			this._isDebugInit = true;
			this.DebugInit();
		}

		this.DebugParam.Update();
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.DebugUpdate();
		}
	}
#endif
	#endregion
}
