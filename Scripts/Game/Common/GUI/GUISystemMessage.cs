/// <summary>
/// システムメッセージ
/// 
/// 2014/08/22
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUISystemMessage : Singleton<GUISystemMessage>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のモード
	/// </summary>
	[SerializeField]
	Mode _startActiveMode = Mode.None;
	Mode StartActiveMode { get { return _startActiveMode; } }
	public enum Mode
	{
		None,
		Message,	// メッセージ
		OK,			// OKボタン
		YesNo,		// YesNoボタン
		Connect,	// 通信中メッセージ
	}

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
		public UILabel titleLabel;
		public UILabel messageLabel;
		public UILabel connectMessageLabel;
		public UILabel okLabel;
		public UILabel yesLabel;
		public UILabel noLabel;

		// メッセージ
		public Message message;
		[System.Serializable]
		public class Message
		{
			public GameObject messageGroup;
			public GameObject connectGroup;
		}
		// ボタン
		public Button button;
		[System.Serializable]
		public class Button
		{
			public GameObject okGroup;
			public GameObject yesnoGroup;
		}
	}

	// 現在のモード
	Mode NowMode { get; set; }
	// NGUIに対するデリゲート
	System.Action onOK { get; set; }
	System.Action onYes { get; set; }
	System.Action onNo { get; set; }
	System.Action onCloseFinish { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.NowMode = (Mode)(-1);
		this.ClearDelegate();
	}

	// 表示しているウィンドウタイトル
	string TitleText { set { if (this.Attach.titleLabel != null) this.Attach.titleLabel.text = value; } }
	// 表示しているメッセージ
	string MessageText
	{
		set
		{
			if (this.Attach.messageLabel != null) this.Attach.messageLabel.text = value;
			if (this.Attach.connectMessageLabel != null) this.Attach.connectMessageLabel.text = value;
		}
	}
	// 表示しているOKボタン名
	string OKButtonText { set { if (this.Attach.okLabel != null) this.Attach.okLabel.text = value; } }
	// 表示しているYesボタン名
	string YesButtonText { set { if (this.Attach.yesLabel != null) this.Attach.yesLabel.text = value; } }
	// 表示しているNoボタン名
	string NoButtonText { set { if (this.Attach.noLabel != null) this.Attach.noLabel.text = value; } }
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// 表示設定
		this.SetActive(this.StartActiveMode);
	}
	void ClearDelegate()
	{
		this.onOK = delegate { };
		this.onYes = delegate { };
		this.onNo = delegate { };
		this.onCloseFinish = delegate { };
	}
	#endregion

	#region モード設定
	#region SetModeMessage
	/// <summary>
	/// メッセージのみのウィンドウ
	/// ウィンドウを閉じるには別で呼び出す必要がある
	/// </summary>
	public static bool SetModeMessage(string title, string text)
	{
		if (Instance == null)
			return false;

		Instance.SetupModeMessage(title, text);
		Instance.SetActive(Mode.Message);
		return true;
	}
	void SetupModeMessage(string title, string text)
	{
		// デリゲートクリア
		this.ClearDelegate();
		// テキスト設定
		this.TitleText = title;
		this.MessageText = text;
	}
	#endregion

	#region SetModeOK
	/// <summary>
	/// 確認ボタン付きメッセージウィンドウ
	/// </summary>
	public static bool SetModeOK(string title, string text, System.Action onOK)
	{
		return SetModeOK(title, text, MasterData.GetText(TextType.TX057_Common_YesButton), true, onOK);
	}
	public static bool SetModeOK(string title, string text, bool isClose, System.Action onOK)
	{
		return SetModeOK(title, text, MasterData.GetText(TextType.TX057_Common_YesButton), isClose, onOK);
	}
	public static bool SetModeOK(string title, string text, string okButton, System.Action onOK)
	{
		return SetModeOK(title, text, okButton, true, onOK);
	}
	public static bool SetModeOK(string title, string text, string okButton, bool isClose, System.Action onOK)
	{
		if (Instance == null)
			return false;

		Instance.SetupModeOK(title, text, okButton, isClose, onOK);
		Instance.SetActive(Mode.OK);
		return true;
	}
	void SetupModeOK(string title, string text, string okButton, bool isClose, System.Action onOK)
	{
		// デリゲートクリア
		this.ClearDelegate();
		// テキスト設定
		this.TitleText = title;
		this.MessageText = text;
		// OKボタン設定
		this.OKButtonText = okButton;
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onOK != null ? onOK : delegate { });
			if (isClose)
				this.onOK = () => { Close(action); };
			else
				this.onOK = action;
		}
	}
	#endregion

	#region SetModeYesNo
	/// <summary>
	/// YesNoメッセージウィンドウ
	/// </summary>
	public static bool SetModeYesNo(string title, string text, System.Action onYes, System.Action onNo)
	{
		return SetModeYesNo(title, text, MasterData.GetText(TextType.TX057_Common_YesButton), MasterData.GetText(TextType.TX058_Common_NoButton), true, onYes, onNo);
	}
	public static bool SetModeYesNo(string title, string text, bool isClose, System.Action onYes, System.Action onNo)
	{
		return SetModeYesNo(title, text, MasterData.GetText(TextType.TX057_Common_YesButton), MasterData.GetText(TextType.TX058_Common_NoButton), isClose, onYes, onNo);
	}
	public static bool SetModeYesNo(string title, string text, string yesButton, string noButton, System.Action onYes, System.Action onNo)
	{
		return SetModeYesNo(title, text, yesButton, noButton, true, onYes, onNo);
	}
	public static bool SetModeYesNo(string title, string text, string yesButton, string noButton, bool isClose, System.Action onYes, System.Action onNo)
	{
		if (Instance == null)
			return false;

		Instance.SetupModeYesNo(title, text, yesButton, noButton, isClose, onYes, onNo);
		Instance.SetActive(Mode.YesNo);
		return true;
	}
	void SetupModeYesNo(string title, string text, string yesButton, string noButton, bool isClose, System.Action onYes, System.Action onNo)
	{
		// デリゲートクリア
		this.ClearDelegate();
		// テキスト設定
		this.TitleText = title;
		this.MessageText = text;
		// Yesボタン設定
		this.YesButtonText = yesButton;
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onYes != null ? onYes : delegate { });
			if (isClose)
				this.onYes = () => { Close(action); };
			else
				this.onYes = action;
		}

		// Noボタン設定
		this.NoButtonText = noButton;
		{
			// ボタンを押してウィンドウが閉じてからアクションを開始する、または
			// ボタンを押した瞬間にアクションを開始する(ウィンドウは閉じない)
			var action = (onNo != null ? onNo : delegate { });
			if (isClose)
				this.onNo = () => { Close(action); };
			else
				this.onNo = action;
		}
	}
	#endregion

	#region SetModeConnect
	/// <summary>
	/// 通信中のウィンドウ
	/// ウィンドウを閉じるには別で呼び出す必要がある
	/// </summary>
	public static bool SetModeConnect(string text)
	{
		if (Instance == null)
			return false;

		Instance.SetupModeConnect(text);
		Instance.SetActive(Mode.Connect);
		return true;
	}
	void SetupModeConnect(string text)
	{
		// デリゲートクリア
		this.ClearDelegate();
		// テキスト設定
		this.MessageText = text;
	}
	#endregion

	#region Close
	/// <summary>
	/// ウィンドウを閉じる
	/// </summary>
	public static bool Close()
	{
		return Close(null);
	}
	public static bool Close(System.Action onFinish)
	{
		if (Instance == null)
			return false;

		Instance.SetupClose(onFinish);
		Instance.SetActive(Mode.None);
		return true;
	}
	void SetupClose(System.Action onFinish)
	{
		// デリゲートクリア
		this.ClearDelegate();
		// 閉じた後の処理設定
		this.onCloseFinish = (onFinish != null ? onFinish : delegate { });
	}
	#endregion

	#region SetActive
	/// <summary>
	/// モードごとのアクティブ設定
	/// </summary>
	void SetActive(Mode mode)
	{
		if (this.NowMode == mode)
			return;
		this.NowMode = mode;

		var m = this.Attach.message;
		var b = this.Attach.button;
		switch (mode)
		{
		case Mode.None: this.SetActiveGroup(false, null, null); break;
		case Mode.Message: this.SetActiveGroup(true, m.messageGroup, null); break;
		case Mode.OK: this.SetActiveGroup(true, m.messageGroup, b.okGroup); break;
		case Mode.YesNo: this.SetActiveGroup(true, m.messageGroup, b.yesnoGroup); break;
		case Mode.Connect: this.SetActiveGroup(true, m.connectGroup, null); break;
		}
	}
	/// <summary>
	/// アクティブグループ設定
	/// </summary>
	void SetActiveGroup(bool isRootActive, GameObject activeMessage, GameObject activeButton)
	{
		// ルート表示
		if (this.Attach.rootTween != null)
			this.Attach.rootTween.Play(isRootActive);
		else
			this.gameObject.SetActive(isRootActive);
		if (!isRootActive)
			return;

		// メッセージアクティブ化
		{
			var m = this.Attach.message;
			var list = new List<GameObject>();
			list.Add(m.messageGroup);
			list.Add(m.connectGroup);
			foreach (var go in list)
			{
				if (go == null)
					continue;
				bool isActive = (activeMessage == go);
				go.SetActive(isActive);
			}
		}

		// ボタンアクティブ化
		{
			var b = this.Attach.button;
			var list = new List<GameObject>();
			list.Add(b.okGroup);
			list.Add(b.yesnoGroup);
			foreach (var go in list)
			{
				if (go == null)
					continue;
				bool isActive = (activeButton == go);
				go.SetActive(isActive);
			}
		}
	}
	#endregion
	#endregion

	#region NGUIリフレクション
	public void OnOK()
	{
		this.onOK();
	}
	public void OnYes()
	{
		this.onYes();
	}
	public void OnNo()
	{
		this.onNo();
	}
	public void OnCloseFinish()
	{
		this.onCloseFinish();
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParameter _debugParam = new DebugParameter();
	DebugParameter DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class DebugParameter
	{
		public bool executeClose;
		public bool executeMode;
		public Mode mode = Mode.None;
		public bool isClose = true;
		public string title = "Title";
		public string text = "Text";
		public string okButton = "OK";
		public string yesButton = "Yes";
		public string noButton = "No";
	}

	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.executeClose)
		{
			t.executeClose = false;
			Close();
		}
		if (t.executeMode)
		{
			t.executeMode = false;
			switch (t.mode)
			{
			case Mode.None:
				Close();
				break;
			case Mode.Message:
				SetModeMessage(t.title, t.text);
				break;
			case Mode.OK:
				SetModeOK(t.title, t.text, t.okButton, t.isClose, () => { Debug.Log("OK"); });
				break;
			case Mode.YesNo:
				SetModeYesNo(t.title, t.text, t.yesButton, t.noButton, t.isClose, () => { Debug.Log("Yes"); }, () => { Debug.Log("No"); });
				break;
			case Mode.Connect:
				SetModeConnect(t.text);
				break;
			}
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
