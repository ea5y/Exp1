using UnityEngine;
using System;
using System.Collections;

using XUI.Mail;

/// <summary>
/// メールのトップ操作GUI
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MailView))]
public class GUIMail : Singleton<GUIMail>
{
	#region === Field ===
	
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private MailView viewAttach = null;
	
	/// <summary>
	/// メールページリスト
	/// </summary>
	[SerializeField]
	private GUIMailItemPageList mailItemPageList = null;
	
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool isStartActive = false;

	/// <summary>
	/// 未読数値フォーマット
	/// </summary>
	[SerializeField]
	private string unreadNumFormat = "({0})";
	
	/// <summary>
	/// 開くタブ設定
	/// </summary>
	private MailTabType openTabType = MailTabType.Mail;

	#endregion === Field ===

	#region === Property ===
	
	private bool IsStartActive { get { return isStartActive; } }
	
	private MailView ViewAttach { get { return viewAttach; } }

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
		this.MemberInit();
	}

	private void Start()
	{
		this.Construct();

		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true, this.IsStartActive);
	}

	private void Construct()
	{
		// モデル生成
		var model = new Model();
		model.UnreadNumFormat = unreadNumFormat;

		// ビュー生成
		IView view = null;
		if(this.ViewAttach != null) {
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}
		
		// コントローラー生成
		var controller = new Controller(model, view, mailItemPageList);
		Controller = controller;
		Controller.OnPageChange += HandlePageChange;
		Controller.OnAllMailRead += HandleAllMailRead;
		Controller.OnAllMailDelete += HandleAllMailDelete;
		Controller.OnAllMailItemReceive += HandleAllMailItemReceive;
		Controller.OnTabChange += HandleTabChange;
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
	public static void Open()
	{
		if (Instance != null){
			Instance._Open();
		}
	}

	private void _Open()
	{
		SetActive(true, false, true);

		// 指定タブ開く
		if(this.Controller != null) {
			Controller.ForceOpenTab(openTabType);
		}
	}

	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen()
	{
		if (Instance != null){
			Instance._ReOpen();
		}
	}


	private void _ReOpen()
	{
		SetActive(true, false, false);

		// 指定タブ開く
		if(this.Controller != null) {
			Controller.ForceOpenTab(openTabType);
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

			// 最初に開くのをメールに戻しておく
			if(!isActive) {
				openTabType = MailTabType.Mail;
			}
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

	
	private void HandleTabChange(object sender, TabChangeEventArgs e)
	{
		openTabType = e.TabType;
		MailCountCheck();
	}


	private void MailCountCheck()
	{
		// メールボックスの件数取得
		LobbyPacket.SendMailBox(ResponseMailBox);
	}
	

	/// <summary>
	/// 件数取得
	/// </summary>
	/// <param name="response"></param>
	private void ResponseMailBox(LobbyPacket.MailBoxResArgs response)
	{
		foreach(var box in response.List) {
			switch(box.Type) {
				case MailTabType.Mail:
					SetMailCount(box.Total, box.Unread, box.Locked);
					break;
				case MailTabType.Item:
					SetItemMailCount(box.Total, box.Unread, box.Locked);
					break;
			}
		}

		// 初期タブ開く
		OpenTab(openTabType);
	}

	
	/// <summary>
	/// メール件数をセットする
	/// </summary>
	/// <param name="total"></param>
	/// <param name="unread"></param>
	private void SetMailCount(int total, int unread, int locked)
	{
		if(Controller == null) return;
		
		Controller.SetMailCount(total, unread, locked);
	}

	/// <summary>
	/// アイテムメール件数をセットする
	/// </summary>
	/// <param name="total"></param>
	/// <param name="unread"></param>
	private void SetItemMailCount(int total, int unread, int locked)
	{
		if(Controller == null) return;
		
		Controller.SetItemMailCount(total, unread, locked);
	}


	/// <summary>
	/// 指定のタブを開く
	/// </summary>
	/// <param name="type"></param>
	private void OpenTab(MailTabType type)
	{
		if(Controller == null) return;

		Controller.SetupTab();
	}

	/// <summary>
	/// 現在開いてるリストを更新する
	/// </summary>
	public static void UpdateCurrentList()
	{
		if(Instance != null) {
			Instance.internalUpdateCurrentList();
		}
	}

	/// <summary>
	/// 内部用 現在開いてるリストを更新する
	/// </summary>
	private void internalUpdateCurrentList()
	{
		if(Controller == null) return;

		Controller.UpdateCurrentList();
	}
	

	#region === Page Change ===

	/// <summary>
	/// ページが変更されたとき
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandlePageChange(object sender, TabPageChangeEventArgs e)
	{
		if(e.TabType == MailTabType.Mail) {
			SendMailPageChange(e.ItemIndex, e.ItemCount, false);
		} else if(e.TabType == MailTabType.Item) {
			SendItemMailPageChange(e.ItemIndex, e.ItemCount, false);
		}
	}
	
	/// <summary>
	/// メールのページ変更を送る
	/// </summary>
	private void SendMailPageChange(int start, int count, bool deleted)
	{
		LobbyPacket.SendAdminMail(start, count, deleted, ResponseAdminMail);
	}

	/// <summary>
	/// メールページ変更レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseAdminMail(LobbyPacket.AdminMailResArgs response)
	{
		if(Controller == null) return;

		Controller.UpdateMailList(response.List, response.Start, response.Count);
	}

	/// <summary>
	/// アイテムメールのページ変更を送る
	/// </summary>
	private void SendItemMailPageChange(int start, int count, bool deleted)
	{
		LobbyPacket.SendPresentMail(start, count, deleted, ResponsePresentMail);
	}

	/// <summary>
	/// アイテムメールページ変更レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponsePresentMail(LobbyPacket.PresentMailResArgs response)
	{
		if(Controller == null) return;

		Controller.UpdateItemMailList(response.List, response.Start, response.Count);
	}

	#endregion === Page Change ===

	#region === MailDetail ===

	/// <summary>
	/// 指定メールの詳細を開く
	/// </summary>
	/// <param name="mail"></param>
	public static void OpenMailDetail(MailInfo mail)
	{
		if(Instance != null) {
			Instance.internalOpenMailDetail(mail);
		}
	}


	/// <summary>
	/// メール詳細を開く
	/// </summary>
	/// <param name="index"></param>
	private void internalOpenMailDetail(MailInfo mail)
	{
		if(mail == null) return;

		if(mail.IsRead) {
			// 既読のときはすぐに開く
			if(mail.Type == MailInfo.MailType.Admin) {
				Controller.OpenMailDetail(mail.Index);
			} else if(mail.Type == MailInfo.MailType.Present) {
				Controller.OpenItemMailDetail(mail.Index);
			}
		} else {
			// 既読にしてから開く
			if(mail.Type == MailInfo.MailType.Admin) {
				LobbyPacket.SendSetAdminMailReadFlag(mail.Index, ResponseSetAdminMailReadFlag);
			} else if(mail.Type == MailInfo.MailType.Present) {
				LobbyPacket.SendSetPresentMailReadFlag(mail.Index, ResponseSetPresentMailReadFlag);
			}
		}
	}

	/// <summary>
	/// 運営メール既読フラグ通信レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseSetAdminMailReadFlag(LobbyPacket.SetAdminMailReadFlagResArgs response)
	{
		if(Controller == null) return;

		// 指定メールを既読にして、開く
		Controller.OpenMailDetail(response.Index);
	}

	/// <summary>
	/// アイテムメール既読フラグ通信レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseSetPresentMailReadFlag(LobbyPacket.SetPresentMailReadFlagResArgs response)
	{
		if(Controller == null) return;

		// 指定メールを既読にして、開く
		Controller.OpenItemMailDetail(response.Index);
	}

	#endregion === MailDetail ===

	#region === Mail Lock ===

	/// <summary>
	/// メールのロックを変更
	/// </summary>
	/// <param name="index"></param>
	/// <param name="locked"></param>
	public static void ChangeMailLock(int index, bool locked)
	{
		if(Instance != null) {
			Instance.internalChangeMailLock(index, locked);
		}
	}

	/// <summary>
	/// 内部用 メールのロック変更
	/// </summary>
	/// <param name="index"></param>
	/// <param name="locked"></param>
	private void internalChangeMailLock(int index, bool locked)
	{
		if(Controller == null) return;
		
		Controller.ChangeMailLock(index, locked);
	}


	/// <summary>
	/// アイテムメールのロックを変更
	/// </summary>
	/// <param name="index"></param>
	/// <param name="locked"></param>
	public static void ChangeItemMailLock(int index, bool locked)
	{
		if(Instance != null) {
			Instance.internalChangeItemMailLock(index, locked);
		}
	}

	/// <summary>
	/// 内部用 アイテムメールのロックを変更
	/// </summary>
	/// <param name="index"></param>
	/// <param name="locked"></param>
	private void internalChangeItemMailLock(int index, bool locked)
	{
		if(Controller == null) return;

		Controller.ChangeItemMailLock(index, locked);
	}

	#endregion === Mail Lock ===

	#region === Mail Delete ===

	/// <summary>
	/// 指定運営メールを削除
	/// </summary>
	/// <param name="index"></param>
	public static void DeleteMail(int index)
	{
		if(Instance != null) {
			Instance.internalDeleteMail(index);
		}
	}

	/// <summary>
	/// 内部用 指定運営メールを削除
	/// </summary>
	/// <param name="index"></param>
	private void internalDeleteMail(int index)
	{
		if(Controller == null) return;

		Controller.DeleteMail(index);

		// 件数取得
		MailCountUpdate();
	}

	/// <summary>
	/// 指定のアイテムメールを削除
	/// </summary>
	/// <param name="index"></param>
	public static void DeleteItemMail(int index)
	{
		if(Instance != null) {
			Instance.internalDeleteItemMail(index);
		}
	}

	/// <summary>
	/// 内部用 指定のアイテムメールを削除
	/// </summary>
	/// <param name="index"></param>
	private void internalDeleteItemMail(int index)
	{
		if(Controller == null) return;

		Controller.DeleteItemMail(index);

		MailCountUpdate();
	}


	private void MailCountUpdate()
	{
		// メールボックスの件数取得
		LobbyPacket.SendMailBox(ResponseMailBoxUpdate);
	}

	/// <summary>
	/// 件数取得
	/// </summary>
	/// <param name="response"></param>
	private void ResponseMailBoxUpdate(LobbyPacket.MailBoxResArgs response)
	{
		foreach(var box in response.List) {
			switch(box.Type) {
				case MailTabType.Mail:
					SetMailCount(box.Total, box.Unread, box.Locked);
					break;
				case MailTabType.Item:
					SetItemMailCount(box.Total, box.Unread, box.Locked);
					break;
			}
		}
		
		// 初期タブ開く
		Controller.ReopenCurrentPage();
	}


	#endregion === Mail Delete ===



	#region === All Read ===

	/// <summary>
	/// 全メール既読
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleAllMailRead(object sender, EventArgs e)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		LobbyPacket.SendSetAdminMailReadFlagAll(ResponseSetAdminMailReadFlagAll);
	}
	
	/// <summary>
	/// 全メール既読通信レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseSetAdminMailReadFlagAll(LobbyPacket.SetAdminMailReadFlagAllResArgs response)
	{
		GUISystemMessage.Close();

		if(Controller == null) return;

		openTabType = MailTabType.Mail;

		Controller.AllMailRead(response.Result, response.Count);
	}

	#endregion === All Read ===

	#region === All ItemReceive ===
	
	/// <summary>
	/// 全アイテムを受け取る
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleAllMailItemReceive(object sender, EventArgs e)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		LobbyPacket.SendReceivePresentMailItemAll(ResponseReceivePresentMailItemAll);
	}

	/// <summary>
	/// 全アイテム受け取り通信レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseReceivePresentMailItemAll(LobbyPacket.ReceivePresentMailItemAllResArgs response)
	{
		GUISystemMessage.Close();

		if(Controller == null) return;
		
		openTabType = MailTabType.Item;
	
		Controller.AllItemReceive(response.Count, response.ExpirationCount);
	}

	#endregion === All ItemReceive ===

	#region === All Delete ===

	/// <summary>
	/// 全削除する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleAllMailDelete(object sender, AllMailDeleteEventArgs e)
	{
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

		if(e.TabType == MailTabType.Mail) {
			LobbyPacket.SendDeleteAdminMailAll(ResponseDeleteAdminMailAll);
		} else if(e.TabType == MailTabType.Item) {
			LobbyPacket.SendDeletePresentMailAll(ResponseDeletePresentMailAll);
		}
	}

	/// <summary>
	/// 全運営メール削除通信レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseDeleteAdminMailAll(LobbyPacket.DeleteAdminMailAllResArgs response)
	{
		GUISystemMessage.Close();

		if(Controller == null) return;
		
		if(response.Result) {
			openTabType = MailTabType.Mail;
		}

		Controller.AllMailDelete(response.Result, response.Count);

	}
	
	/// <summary>
	/// 全アイテムメール削除通信レスポンス
	/// </summary>
	/// <param name="response"></param>
	private void ResponseDeletePresentMailAll(LobbyPacket.DeletePresentMailAllResArgs response)
	{
		GUISystemMessage.Close();

		if(Controller == null) return;

		if(response.Result) {
			openTabType = MailTabType.Item;
		}

		Controller.AllItemMailDelete(response.Result, response.Count);


	}

	#endregion === All Delete ===


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
		}

		[SerializeField]
		TemprateEvent _sample = new TemprateEvent();
		
		public TemprateEvent Sample { get { return _sample; } }
		
		[System.Serializable]
		public class TemprateEvent : IDebugParamEvent
		{
			public event System.Action Execute = delegate { };

			[SerializeField]
			bool execute = false;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
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
			d.ReadMasterData();
			Open();
		};

		d.Sample.Execute += () => { Debug.Log("Sample"); };
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
