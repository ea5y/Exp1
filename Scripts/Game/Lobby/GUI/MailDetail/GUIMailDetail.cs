using UnityEngine;
using System;
using System.Collections;

using XUI.MailDetail;


[DisallowMultipleComponent]
[RequireComponent(typeof(MailDetailView))]
public class GUIMailDetail : Singleton<GUIMailDetail>
{
	#region === Field ===
	
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private MailDetailView viewAttach = null;
	
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool isStartActive = false;
	
	/// <summary>
	/// アイテムのスタック数表示フォーマット
	/// </summary>
	[SerializeField]
	private string itemCountFormat = "×{0}";
	
	/// <summary>
	/// ベースゲームアイテム
	/// </summary>
	[SerializeField]
	private GameObject baseGameItem = null;
	
	/// <summary>
	/// インスタンス化したベースゲームアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	private GameObject attachBaseItem = null;

	/// <summary>
	/// ベースアイテム
	/// </summary>
	private GUIItem presentItem = null;
	
	#endregion === Field ===

	#region === Property ===

	private bool IsStartActive { get { return isStartActive; } }
	
	private MailDetailView ViewAttach { get { return viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }

	#endregion === Property ===


	#region === 初期化 ===

	/// <summary>
	/// メンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Controller = null;
	}

	protected override void Awake()
	{
		base.Awake();
		MemberInit();
		CreateBaseItem();
	}

	private void Start()
	{
		this.Construct();

		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true, this.IsStartActive);
	}

	/// <summary>
	/// アイテムアイコン作成
	/// </summary>
	private void CreateBaseItem()
	{
		UnityEngine.Assertions.Assert.IsFalse(baseGameItem == null, "GUIMailDetail CreateBaseItem: baseGameItem is Null");
		UnityEngine.Assertions.Assert.IsFalse(attachBaseItem == null, "GUIMailDetail CreateBaseItem: attachBaseItem is Null");

		var go = SafeObject.Instantiate(baseGameItem);
		UnityEngine.Assertions.Assert.IsFalse(go == null, "GUIMailDetail CreateBaseItem: SafeObject.Instantiate Failed");

		attachBaseItem.DestroyChild();

		// 親子付け
		go.SetParentWithLayer(attachBaseItem, false);
		// アクティブ化
		if(!go.activeSelf) {
			go.SetActive(true);
		}

		// コンポーネント取得
		presentItem = go.GetComponentInChildren(typeof(GUIItem)) as GUIItem;
		UnityEngine.Assertions.Assert.IsFalse(presentItem == null, "GUIMailDetail CreateBaseItem: GUIItem Not Found");
	}


	private void Construct()
	{
		// モデル生成
		var model = new Model();
		model.ItemCountFormat = itemCountFormat;

		// ビュー生成
		IView view = null;
		if(this.ViewAttach != null) {
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}
		
		// コントローラー生成
		var controller = new Controller(model, view, presentItem);
		Controller = controller;
		Controller.OnMailDelete += HandleMailDelete;
		Controller.OnMailLock += HandleMailLock;
		Controller.OnMailUnlock += HandleMailUnlock;
		Controller.OnItemReceive += HandleItemReceive;
	}



	#endregion === 初期化 ===

	#region === 破棄 ===

	/// <summary>
	/// オブジェクトが破棄されたとき
	/// </summary>
	private void OnDestroy()
	{
		// コントローラーの破棄をする
		if(Controller != null){
			Controller.Dispose();
			Controller = null;
		}
	}

	#endregion === 破棄 ===


	#region === アクティブ設定 ===
	
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null){
			Instance.SetActive(false, false, false);
		}
	}

	/// <summary>
	/// 開く
	/// </summary>
	public static void Open(MailInfo mail, int lockCount)
	{
		if (Instance != null && mail != null){
			Instance.SetActive(true, false, true);

			if(Instance.Controller != null) {
				Instance.Controller.SetMailInfo(mail);
				Instance.Controller.SetLockCount(lockCount);
			}
		}
	}
	
	/// <summary>
	/// アクティブ設定
	/// </summary>
	private void SetActive(bool isActive, bool isTweenSkip, bool isSetup)
	{
		if (isSetup){
			this.Setup();
		}

		if (this.Controller != null){
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}

	#endregion === アクティブ設定 ===

	#region === 各種情報更新 ===

	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup()
	{
		if (this.Controller != null){
			this.Controller.Setup();
		}
	}

	#endregion === 各種情報更新 ===


	#region === Lock ===

	/// <summary>
	/// メールアンロック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleMailUnlock(object sender, MailUnlockEventArgs e)
	{
		if(e.Type == MailInfo.MailType.Admin) {
			MailUnlock(e.Index, e.OverKeepDays);
		} else if(e.Type == MailInfo.MailType.Present) {
			ItemMailUnlock(e.Index, e.OverKeepDays);
		}
	}

	/// <summary>
	/// メールロック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleMailLock(object sender, MailEventArgs e)
	{
		if(e.Type == MailInfo.MailType.Admin) {
			MailLock(e.Index);
		} else if(e.Type == MailInfo.MailType.Present) {
			ItemMailLock(e.Index);
		}
	}

	/// <summary>
	/// 運営メールをアンロックする
	/// </summary>
	/// <param name="index"></param>
	/// <param name="over"></param>
	private void MailUnlock(int index, bool over)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		if(over) {
			// 保持期限が切れている
			LobbyPacket.SendSetLockAdminMail(index, false, ResponseSetLockAdminMailWithDelete);
		} else {
			// ロックの通信
			LobbyPacket.SendSetLockAdminMail(index, false, ResponseSetLockAdminMail);
		}
	}

	/// <summary>
	/// アイテムメールをアンロックする
	/// </summary>
	/// <param name="index"></param>
	/// <param name="over"></param>
	private void ItemMailUnlock(int index, bool over)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		if(over) {
			// 保持期限が切れている
			LobbyPacket.SendSetLockPresentMail(index, false, ResponseSetLockPresentMailWithDelete);
		} else {
			// ロックの通信
			LobbyPacket.SendSetLockPresentMail(index, false, ResponseSetLockPresentMail);
		}
	}

	/// <summary>
	/// 運営メールをロックする
	/// </summary>
	/// <param name="index"></param>
	private void MailLock(int index)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		// ロックの通信
		LobbyPacket.SendSetLockAdminMail(index, true, ResponseSetLockAdminMail);
	}

	/// <summary>
	/// アイテムメールをロックする
	/// </summary>
	/// <param name="index"></param>
	private void ItemMailLock(int index)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		// ロックの通信
		LobbyPacket.SendSetLockPresentMail(index, true, ResponseSetLockPresentMail);
	}



	/// <summary>
	/// ロック変更
	/// </summary>
	/// <param name="response"></param>
	private void ResponseSetLockAdminMail(LobbyPacket.SetLockAdminMailResArgs response)
	{
		GUISystemMessage.Close();

		// 解除成功
		if(response.Result == Scm.Common.GameParameter.MailLockResult.Success) {
			
			// ロック変更
			GUIMail.ChangeMailLock(response.Index, response.Locked);

			// メールのロック変更
			Controller.UpdateLockFlag();
		} else {
			// なんか出す？
		}
	}

	/// <summary>
	/// ロック解除＋削除
	/// </summary>
	/// <param name="response"></param>
	private void ResponseSetLockAdminMailWithDelete(LobbyPacket.SetLockAdminMailResArgs response)
	{
		GUISystemMessage.Close();

		// 解除成功
		if(response.Result == Scm.Common.GameParameter.MailLockResult.Success) {
			// ロック変更
			GUIMail.ChangeMailLock(response.Index, response.Locked);

			// 指定メール削除
			GUIMail.DeleteMail(response.Index);
			
			// 確認表示
			if(Controller != null) {
				Controller.DeleteMail();
			}
		} else {
			// なんか出す？
		}
	}

	/// <summary>
	/// ロック変更
	/// </summary>
	/// <param name="response"></param>
	private void ResponseSetLockPresentMail(LobbyPacket.SetLockPresentMailResArgs response)
	{
		GUISystemMessage.Close();

		// 解除成功
		if(response.Result == Scm.Common.GameParameter.MailLockResult.Success) {
			// メールのロック変更
			GUIMail.ChangeItemMailLock(response.Index, response.Locked);

			// メールのロック変更
			Controller.UpdateLockFlag();
		} else {
			// なんか出す？
		}
	}

	/// <summary>
	/// ロック解除＋削除
	/// </summary>
	/// <param name="response"></param>
	private void ResponseSetLockPresentMailWithDelete(LobbyPacket.SetLockPresentMailResArgs response)
	{
		GUISystemMessage.Close();

		// 解除成功
		if(response.Result == Scm.Common.GameParameter.MailLockResult.Success) {
			// ロック変更
			GUIMail.ChangeItemMailLock(response.Index, response.Locked);

			// 指定メール削除
			GUIMail.DeleteItemMail(response.Index);

			// 確認表示
			if(Controller != null) {
				Controller.DeleteMail();
			}
		} else {
			// なんか出す？
		}
	}

	#endregion === Lock ===

	#region === Item Receive ===

	/// <summary>
	/// アイテム受け取り時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleItemReceive(object sender, MailEventArgs e)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		LobbyPacket.SendReceivePresentMailItem(e.Index, ResponseSendReceivePresentMailItem);
	}

	/// <summary>
	/// アイテム受け取り通信コールバック
	/// </summary>
	/// <param name="response"></param>
	private void ResponseSendReceivePresentMailItem(LobbyPacket.ReceivePresentMailItemResArgs response)
	{
		GUISystemMessage.Close();

		if(Controller == null) return;
		
		// アイテム受け取り
		Controller.ItemReceived(response.Result);


	}

	#endregion === Item Receive ===

	#region === Mail Delete ===

	/// <summary>
	/// メール削除イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleMailDelete(object sender, MailEventArgs e)
	{
		// 指定メール削除
		if(e.Type == MailInfo.MailType.Admin) {
			MailDelete(e.Index);
		} else if(e.Type == MailInfo.MailType.Present) {
			ItemMailDelete(e.Index);
		}
	}

	/// <summary>
	/// 運営メール削除
	/// </summary>
	/// <param name="index"></param>
	private void MailDelete(int index)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		LobbyPacket.SendDeleteAdminMail(index, ResponseMailDelete);
	}
	
	/// <summary>
	/// 運営メール削除通信レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseMailDelete(LobbyPacket.DeleteAdminMailResArgs response)
	{
		GUISystemMessage.Close();

		if(response.Result) {
			GUIMail.DeleteMail(response.Index);
			
			if(Controller != null) {
				Controller.DeleteMail();
			}
		}
	}

	/// <summary>
	/// アイテムメール削除
	/// </summary>
	/// <param name="index"></param>
	private void ItemMailDelete(int index)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		LobbyPacket.SendDeletePresentMail(index, ResponseItemMailDelete);
	}

	/// <summary>
	/// アイテムメール削除通信レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseItemMailDelete(LobbyPacket.DeletePresentMailResArgs response)
	{
		GUISystemMessage.Close();

		Debug.Log(response.Result);

		if(response.Result) {
			GUIMail.DeleteItemMail(response.Index);

			if(Controller != null) {
				Controller.DeleteMail();
			}
		}
	}

	#endregion === Mail Delete ===



	#region === デバッグ ===

#if UNITY_EDITOR && XW_DEBUG

	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();

	GUIDebugParam DebugParam { get { return _debugParam; } }

	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public GUIDebugParam()
		{
			this.AddEvent(this.MailOpen);
		}

		[SerializeField]
		MailOpenEvent _mailOpen = new MailOpenEvent();
		
		public MailOpenEvent MailOpen { get { return _mailOpen; } }
		
		[System.Serializable]
		public class MailOpenEvent : IDebugParamEvent
		{
			public event System.Action Execute = delegate { };

			[SerializeField]
			bool execute = false;

			public string name = "Name";
			public string title = "Title";

			[Multiline(5)]
			public string body = "本文";
			public int icon = 0;
			public string received;
			public int itemId = 1;
			public int itemCount = 1;
			public string deadline;
			public string itemReceived;

			public MailInfo mail;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;

					DateTime rec;
					if(!DateTime.TryParse(received, out rec)) {
						rec = DateTime.Now;
					}

					DateTime timeTmp;
					DateTime? dead = null;
					if(DateTime.TryParse(deadline, out timeTmp)) {
						dead = timeTmp;
					}


					DateTime? itemRec = null;
					if(DateTime.TryParse(itemReceived, out timeTmp)) {
						itemRec = timeTmp;
					}
					
					mail = new MailInfo(name, title, body, icon, rec, itemId, itemCount, dead, itemRec);
					
					this.Execute();
				}
			}
		}
	}

	private void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () =>
		{
		};

		d.MailOpen.Execute += () => {
			GUIMailDetail.Open(d.MailOpen.mail, 10);
		};
	}

	bool _isDebugInit = false;

	private void DebugUpdate()
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

	#endregion === デバッグ ===

}
