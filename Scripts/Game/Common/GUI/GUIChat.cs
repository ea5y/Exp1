/// <summary>
/// チャットシステム
/// 
/// 2014/06/14
/// </summary>

#if !UNITY_EDITOR && !UNITY_STANDALONE
#define CHAT_MOBILE
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.GameParameter;
using Scm.Common.NGWord;

public class GUIChat : Singleton<GUIChat>
{
	#region 定義
	// この回数以上連続で同じ文言を発言させない
	static readonly int ConinuityForbidNum = 3;
	// 最後の発言からこの時間過ぎるとキャッシュクリア
	static readonly float ChatWordChacheClearTime = 5f;
	#endregion

	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のチャットタイプアクティブフラグ
	/// </summary>
	[SerializeField]
	bool _isStartChatTypeActive = false;
	bool StartChatTypeActive { get { return _isStartChatTypeActive; } }

	/// <summary>
	/// 初期化時のマクロアクティブフラグ
	/// </summary>
	[SerializeField]
	bool _isStartMacroActive = false;
	bool StartMacroActive { get { return _isStartMacroActive; } }

	/// <summary>
	/// マクロボタン設定
	/// </summary>
	[SerializeField]
	MacroButtonConfig _macroButton;
	MacroButtonConfig MacroButton { get { return _macroButton; } }
	[System.Serializable]
	public class MacroButtonConfig
	{
		public int maxLength = 8;	// 最大文字数
		public string overReplaceString = "…";	// 文字数をオーバーした時に置き換える文字
	};

	/// <summary>
	/// チャットカラー
	/// </summary>
	[SerializeField]
	ChatColorCode _chatColor;
	ChatColorCode ChatColor { get { return _chatColor; } }
	[System.Serializable]
	public class ChatColorCode
	{
		public Color say = NGUIText.ParseColor("FFFFFF", 0);
		public Color guild = NGUIText.ParseColor("00FFFF", 0);
		public Color team = NGUIText.ParseColor("0000FF", 0);
		public Color whisper = NGUIText.ParseColor("FFC0CB", 0);
		public Color shout = NGUIText.ParseColor("FF0000", 0);
		public Color adminYell = NGUIText.ParseColor("FFFF00", 0);
		public Color adminGuild = NGUIText.ParseColor("FFFF00", 0);
		public Color adminTeam = NGUIText.ParseColor("FFFF00", 0);
		public Color adminWhisper = NGUIText.ParseColor("FFFF00", 0);
		public Color adminShout = NGUIText.ParseColor("FFFF00", 0);
		public Color system = NGUIText.ParseColor("FF7800", 0);
		public Color error = NGUIText.ParseColor("FF0000", 0);
	}

	#region チャット色
	static Dictionary<ChatType, string> ColorCodeDict { get; set; }
	static string ErrorColorCode { get; set; }
	static bool CreateColorCode()
	{
		if (Instance == null)
		{
			Debug.LogWarning("Not Found! GUIChat.Instance");
			return false;
		}

		var cc = Instance.ChatColor;

		// マスターデータからカラーデータを読み込む
		cc.say = MasterData.GetColor(ColorType.C014_ChatSay);
		cc.guild = MasterData.GetColor(ColorType.C015_ChatGuild);
		cc.team = MasterData.GetColor(ColorType.C016_ChatTeam);
		cc.whisper = MasterData.GetColor(ColorType.C017_ChatWhisper);
		cc.shout = MasterData.GetColor(ColorType.C018_ChatShout);
		cc.adminYell = MasterData.GetColor(ColorType.C019_ChatAdminYell);
		cc.adminGuild = MasterData.GetColor(ColorType.C020_ChatAdminGuild);
		cc.adminTeam = MasterData.GetColor(ColorType.C021_ChatAdminTeam);
		cc.adminWhisper = MasterData.GetColor(ColorType.C022_ChatAdminWhisper);
		cc.adminShout = MasterData.GetColor(ColorType.C023_ChatAdminShout);
		cc.system = MasterData.GetColor(ColorType.C024_ChatSystem);
		cc.error = MasterData.GetColor(ColorType.C025_ChatError);

		// カラーコードを追加する
		ColorCodeDict = new Dictionary<ChatType, string>();
		ColorCodeDict.Add(ChatType.Say, NGUIText.EncodeColor(cc.say));
		ColorCodeDict.Add(ChatType.Guild, NGUIText.EncodeColor(cc.guild));
		ColorCodeDict.Add(ChatType.Team, NGUIText.EncodeColor(cc.team));
		ColorCodeDict.Add(ChatType.Whisper, NGUIText.EncodeColor(cc.whisper));
		ColorCodeDict.Add(ChatType.Shout, NGUIText.EncodeColor(cc.shout));
		ColorCodeDict.Add(ChatType.AdminYell, NGUIText.EncodeColor(cc.adminYell));
		ColorCodeDict.Add(ChatType.AdminGuild, NGUIText.EncodeColor(cc.adminGuild));
		ColorCodeDict.Add(ChatType.AdminTeam, NGUIText.EncodeColor(cc.adminTeam));
		ColorCodeDict.Add(ChatType.AdminWhisper, NGUIText.EncodeColor(cc.adminWhisper));
		ColorCodeDict.Add(ChatType.AdminShout, NGUIText.EncodeColor(cc.adminShout));
		ColorCodeDict.Add(ChatType.System, NGUIText.EncodeColor(cc.system));
		ErrorColorCode = NGUIText.EncodeColor(cc.error);

		return true;
	}
	public static string AddColorCode(string text, ChatType chatType)
	{
		if (ColorCodeDict == null)
		{
			if (!CreateColorCode())
				return text;
		}

		string colorCode;
		if (!ColorCodeDict.TryGetValue(chatType, out colorCode))
		{
			Debug.LogWarning("Not Found! ChatType");
			return text;
		}

		return string.Format("[{0}]{1}[-]", colorCode, text);
	}
	public static string AddErrorColorCode(string text)
	{
		return string.Format("[{0}]{1}[-]", ErrorColorCode, text);
	}
	#endregion

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
	    public GameObject ChatWindow;
	    public UITextList ChatTextList;
		public UIInput input;
		public UIPlayTween chatTypeTween;
		public UIPlayTween macroTween;
		public GUIPopup popup;
		public List<GameObject> battleDeactiveGroupList;

        public UILabel chatType;

		// ボタン
		public ActiveButton activeButton;
		[System.Serializable]
		public class ActiveButton
		{
			public UIButton say;
			public UIButton guild;
			public UIButton team;
			public UIButton whisper;
			public UIButton shout;
		}

		// マクロ関連
		public Macro macro;
		[System.Serializable]
		public class Macro
		{
			public GUIChatMacroItem itemPrefab;
			public UITable itemTable;
		}
	}

	// チャットタイプアクティブフラグ
	bool IsChatTypeActive { get; set; }
	// マクロアクティブフラグ
	bool IsMacroActive { get; set; }
	// マクロボタンリスト
	List<GUIChatMacroItem> MacroButtonList { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.IsChatTypeActive = false;
		this.IsMacroActive = false;
		this.MacroButtonList = new List<GUIChatMacroItem>();
		this.ChatWordChache = new List<string>();
	}

	// チャットマクロリスト
	List<ChatMacroInfo> ChatMacroList { get { return ConfigFile.Option.ChatMacroList; } }
	// チャットタイプ
	public ChatType ChatType { get { return ScmParam.Common.ChatType; } set { ScmParam.Common.ChatType = value; } }

	/// <summary>
	/// チャットが入力中かどうか
	/// </summary>
	public static bool IsInput
	{
		get
		{
			if (Instance == null)
				return false;
			if (Instance.Attach.input == null)
				return false;
			// チャット中かどうか
			if (Instance.Attach.input.isSelected)
				return true;
			// マクロボタン編集中かどうか
			foreach (var item in Instance.MacroButtonList)
			{
				if (item.IsInput)
					return true;
			}
			return false;
		}
	}
	/// <summary>
	/// マクロボタン名の最大文字数
	/// </summary>
	public static int MacroButtonNameMaxLength { get { return (Instance != null ? Instance.MacroButton.maxLength : 0); } }
	/// <summary>
	/// マクロボタン名が最大文字数を超えた時に置き換える文字
	/// </summary>
	public static string MacroButtonNameOverReplaceString { get { return (Instance != null ? Instance.MacroButton.overReplaceString : ""); } }

	// 発言した文字列のキャッシュ（前後の空白は取り除く）
	List<string> ChatWordChache{get;set;}
	// チャットクリアFiber
	Fiber ChatWordChacheClearFiber{get;set;}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// マクロアイテムセットアップ
		StartCoroutine(this.ItemSetupCoroutine());

		// UI設定
		{
			var t = this.Attach;
			// ポップアップシステム
			if (t.popup != null)
			{
				this._SetPopupNum(ConfigFile.Option.ChatPopupNum);
				this._SetPopupTimer(ConfigFile.Option.ChatPopupTimer);
				t.popup.Setup(ScmParam.Common.ChatPopupQueue);
			}

			// チャットタイプ
			if (t.chatTypeTween != null)
				t.chatTypeTween.Play(false);
#if CHAT_MOBILE
			// モバイル版では入力文字を見えないように画面外に配置する
			if (t.input != null && t.input.label != null)
				t.input.label.transform.localPosition = new Vector3(0f, Screen.height*1.5f, 0f);
#endif
			// バトル中表示をオフにする
			if (ScmParam.Common.AreaType == AreaType.Field)
			{
				if (t.battleDeactiveGroupList != null)
				{
					foreach (var go in t.battleDeactiveGroupList)
					{
						go.SetActive(false);
					}
				}
			}
		}

		// チャットタイプ設定
		this._SetChatTypeActive(this.StartChatTypeActive);
		this._SetChatType(this.ChatType);
		// マクロアクティブ設定
		this._SetMacroActive(this.StartMacroActive);
	}
	#endregion

	#region ポップアップアイテム設定
	public static void SetPopupNum(int num)
	{
		if (Instance != null) Instance._SetPopupNum(num);
	}
	void _SetPopupNum(int num)
	{
		if (this.Attach.popup != null)
			this.Attach.popup.ItemMax = num;
	}
	public static void SetPopupTimer(float timer)
	{
		if (Instance != null) Instance._SetPopupTimer(timer);
	}
	void _SetPopupTimer(float timer)
	{
		if (this.Attach.popup != null)
			this.Attach.popup.Timer = timer;
	}
	#endregion

	#region チャットタイプ設定
	public static void SetChatTypeActive(bool isActive)
	{
		if (Instance != null) Instance._SetChatTypeActive(isActive);
	}
	void _SetChatTypeActive(bool isActive)
	{
		this.IsChatTypeActive = isActive;

		// チャットタイプウィンドウ再生
		var t = this.Attach;
		if (t.chatTypeTween != null)
			t.chatTypeTween.Play(isActive);
	}
	public static void SetChatType(ChatType chatType)
	{
		if (Instance != null) Instance._SetChatType(chatType);
	}
	void _SetChatType(ChatType chatType)
	{
		this.ChatType = chatType;
		var x = this.Attach.activeButton;
		switch (chatType)
		{
		case ChatType.Say: this.SetChatTypeGroupActive(x.say); break;
		case ChatType.Guild: this.SetChatTypeGroupActive(x.guild); break;
		case ChatType.Team: this.SetChatTypeGroupActive(x.team); break;
		case ChatType.Whisper: this.SetChatTypeGroupActive(x.whisper); break;
		case ChatType.Shout: this.SetChatTypeGroupActive(x.shout); break;
		default: this.SetChatTypeGroupActive(x.say); break;
		}
	}
	void SetChatTypeGroupActive(UIButton activeButton)
	{
		// 売り切り版では非表示
		//// アクティブ化
		//var list = new List<UIButton>();
		//{
		//	var x = this.Attach.activeButton;
		//	list.Add(x.say);
		//	list.Add(x.guild);
		//	list.Add(x.team);
		//	list.Add(x.whisper);
		//	list.Add(x.shout);
		//}
		//foreach (var t in list)
		//{
		//	if (t == null)
		//		continue;
		//	bool isActive = (activeButton == t);
		//	t.gameObject.SetActive(isActive);
		//}
	}
	#endregion

	#region マクロアクティブ
	public static void SetMacroActive(bool isActive)
	{
		if (Instance != null) Instance._SetMacroActive(isActive);
	}
	void _SetMacroActive(bool isActive)
	{
		this.IsMacroActive = isActive;

		// マクロウィンドウ再生
		var t = this.Attach;
		if (t.macroTween != null)
			t.macroTween.Play(isActive);
	}
	#endregion

	#region メッセージ
	/// <summary>
	/// メッセージを表示する
	/// </summary>
	public static void AddMessage(ChatInfo chatInfo)
	{
		switch (chatInfo.chatType)
		{
		case ChatType.AdminYell:
		case ChatType.AdminGuild:
		case ChatType.AdminTeam:
		case ChatType.AdminWhisper:
		case ChatType.AdminShout:
			AddAdminMessage(chatInfo);
			break;
		default:
			AddChatMessage(chatInfo);
			break;
		}
	}
	/// <summary>
	/// システムメッセージを表示する
	/// </summary>
	public static void AddSystemMessage(bool isError, string text)
	{
		var chatInfo = new ChatInfo { playerID = 0, chatType = ChatType.System, name = "", text = text };
		// 色変換
		SetColorCode(ref chatInfo, isError);

		// メッセージ表示
		GUIChatLog.AddMessage(chatInfo);
		if (Instance != null) Instance._AddMessage(chatInfo);
	}
	static void AddChatMessage(ChatInfo chatInfo)
	{
		// NGUIのBBコード削除
		chatInfo.text = NGUIText.StripSymbols(chatInfo.text);
		// 色変換
		SetColorCode(ref chatInfo, false);

		// メッセージ表示
		GUIChatLog.AddMessage(chatInfo);
		if (Instance != null) Instance._AddMessage(chatInfo);
	}
	static void AddAdminMessage(ChatInfo chatInfo)
	{
		// GMメッセージ表示
		GUIGMWindow.AddMessage(chatInfo);

		// 色変換
		SetColorCode(ref chatInfo, false);

		// メッセージ表示
		GUIChatLog.AddMessage(chatInfo);
		if (Instance != null) Instance._AddMessage(chatInfo);
	}
	static void SetColorCode(ref ChatInfo chatInfo, bool isError)
	{
		// 色変換
		if (!string.IsNullOrEmpty(chatInfo.text))
		{
			if (isError)
				chatInfo.text = AddErrorColorCode(chatInfo.text);
			else
				chatInfo.text = AddColorCode(chatInfo.text, chatInfo.chatType);
		}
		if (!string.IsNullOrEmpty(chatInfo.name))
		{
			if (isError)
				chatInfo.name = AddErrorColorCode(chatInfo.name + ":");
			else
				chatInfo.name = AddColorCode(chatInfo.name + ":", chatInfo.chatType);
		}

        if (!string.IsNullOrEmpty(chatInfo.type))
        {
            if (isError)
                chatInfo.type = AddErrorColorCode(chatInfo.type);
            else
                chatInfo.type = AddColorCode(chatInfo.type, chatInfo.chatType);
        }
	}
	void _AddMessage(ChatInfo chatInfo)
	{
		// チャット登録
		ScmParam.Common.ChatPopupQueue.Enqueue(chatInfo);
	}
	#endregion

	#region チャット送信
	/// <summary>
	/// チャットをサーバーに送信する
	/// </summary>
	public static void SendChat(string text)
	{
		if (Instance != null) Instance._SendChat(text);
	}
	void _SendChat(string text)
	{
		this._SendChat(text, text);
	}

    /// <summary>
    /// Whisper id and name
    /// </summary>
    long whisperPlayerId;
    long WhisperPlayerId { get { return this.whisperPlayerId; } set { this.whisperPlayerId = value; } }

    string whisperPlayerName = "";
    string WhisperPlayerName { get { return this.whisperPlayerName; } set { this.whisperPlayerName = value; } }

	void _SendChat(string text, string rawText)
	{
		// 入力文字がないなら何もしない
		if (string.IsNullOrEmpty(text))
			return;

		// 文字制限以上なら削除
		if( this.Attach.input != null ) 
		{
			int textMax = this.Attach.input.characterLimit;
			if( textMax < text.Length )
			{
				int sIdx = textMax-1;
				int cnt = text.Length-textMax;
				text = text.Remove(sIdx,cnt);
			}
		}

		// 連続して同じ発言をしているかチェック
		if(IsContinuityChat(text))
		{
			GUIChat.AddMessage(new ChatInfo { playerID = 0, chatType = Scm.Common.GameParameter.ChatType.System, name = "System", text = MasterData.GetText(TextType.TX138_Chat_ContinuityNoticeStr) });
			return;
		}

#if UNITY_EDITOR || XW_DEBUG
		if (!Scm.Client.GameListener.ConnectFlg)
		{
			GUIChat.AddMessage(new ChatInfo { playerID = 0, chatType = this.ChatType, name = "Name", text = text });
			return;
		}
#endif
#if XW_DEBUG || ENABLE_GM_COMMAND
		if (GMCommand.CommandSelf(rawText))
		{
			GUIDebugLog.AddMessage(rawText);
			Debug.Log(rawText);
			return;
		}
		if (GMCommand.IsGMCommand(rawText))
		{
			// GMコマンドならNGワード検出せずそのままサーバーに送る
			// @bf_a 系対策
			text = rawText;
		}
#endif
#if GOONE
		if (GMCommand.IsGMCommand(text))
		{
			CommonPacket.SendGmCommand(text.Substring(1));
			return;
		}
#endif
        if(this.ChatType == ChatType.Say)
            this.WhisperPlayerId = 0;

        //this.WhisperPlayerId
        //luwanzhong: add new idear show whisper in local
        if (this.ChatType == ChatType.Whisper)
        {
            var str = "[私聊] 对[url=" + (int)ChatType.Whisper + "," + this.WhisperPlayerId + "," + this.WhisperPlayerName + "]" + "[" + this.WhisperPlayerName + "]" + "悄悄地说: " + text;
            str = AddColorCode(str, ChatType.Whisper);
            this.Attach.ChatTextList.Add(str);
        }

		CommonPacket.SendChat(this.ChatType, text, this.WhisperPlayerId);
	}

	/// <summary>
	/// 連続して同じ発言をしているかチェック
	/// </summary>
	/// <returns></returns>
	bool IsContinuityChat(string text)
	{
		// キャッシュ
		this.ChatWordChache.Add(text.Trim());

		int sameCnt = 0;

		if( GUIChat.ConinuityForbidNum <= this.ChatWordChache.Count )
		{
			string checkText = this.ChatWordChache[this.ChatWordChache.Count-1];
			int sIdx = this.ChatWordChache.Count-2;
			int eIdx = this.ChatWordChache.Count-GUIChat.ConinuityForbidNum;

			for( int i = sIdx  ; eIdx <= i ; i-- )
			{
				string txt = this.ChatWordChache[i];
				if( txt == checkText )
					sameCnt++;
			}
		}

		// チャット発言キャッシュクリアFiber開始
		this.ChatWordChacheClearFiber = new Fiber(this.ChatStrChacheClearCoroutine());

		return (GUIChat.ConinuityForbidNum-1 <= sameCnt);
	}
	/// <summary>
	/// キャッシュクリアコルーチン
	/// </summary>
	/// <returns></returns>
	IEnumerator ChatStrChacheClearCoroutine()
	{
		var waitFiber = new WaitSeconds(GUIChat.ChatWordChacheClearTime);

		while(waitFiber.IsWait)
			yield return null;

		this.ChatWordChache.Clear();
	}
	#endregion

	#region 更新
	void Update()
	{
		// チャット発言キャッシュクリアFiber
		if( this.ChatWordChacheClearFiber != null )
		{
 			if(!this.ChatWordChacheClearFiber.Update())
			{
				this.ChatWordChacheClearFiber = null;
			}
		}
	}
	#endregion 

	#region アイテム操作
	public static void MacroItemSetup(int index, ChatMacroInfo info)
	{
		if (Instance != null) Instance._MacroItemSetup(index, info);
	}
	void _MacroItemSetup(int index, ChatMacroInfo info)
	{
		if (this.MacroButtonList != null)
		{
			if (index < this.MacroButtonList.Count)
			{
				var item = this.MacroButtonList[index];
				item.Setup(info);
			}
		}
	}
	public static void MacroColumnSetup(int column)
	{
		if (Instance != null) Instance._MacroColumnSetup(column);
	}
	void _MacroColumnSetup(int column)
	{
		var table = this.Attach.macro.itemTable;
		if (table != null)
		{
			table.columns = column;
			// 再配置
			this.RepositionItem();
		}
	}
	IEnumerator ItemSetupCoroutine()
	{
		// マクロアイテム削除
		this.DestroyItem();
		// マクロアイテム追加
		if (this.ChatMacroList != null)
		{
			foreach (var t in this.ChatMacroList)
			{
				this.AddItem(t);
			}
		}
		// Unityの仕様で1フレーム置かないとちゃんと削除が完了しない
		yield return null;
		// 再配置
		this.RepositionItem();
	}
	/// <summary>
	/// アイテム追加
	/// </summary>
	void AddItem(ChatMacroInfo info)
	{
		var prefab = this.Attach.macro.itemPrefab;
		if (prefab == null)
			return;
		var table = this.Attach.macro.itemTable;
		if (table == null)
			return;

		// アイテム作成
		var item = GUIChatMacroItem.Create(prefab.gameObject, table.transform, info.Index);
		if (item == null)
			return;
		// 設定
		item.Setup(info);
		// リストに追加
		this.MacroButtonList.Add(item);
	}
	/// <summary>
	/// 再配置
	/// </summary>
	void RepositionItem()
	{
		var table = this.Attach.macro.itemTable;
		if (table != null)
		{
			table.Reposition();
		}
	}
	/// <summary>
	/// 全てのアイテムを削除
	/// </summary>
	void DestroyItem()
	{
		// テーブル以下の余計なオブジェクトを削除する
		var table = this.Attach.macro.itemTable;
		if (table != null)
		{
			var t = table.transform;
			for (int i = 0, max = t.childCount; i < max; i++)
			{
				var child = t.GetChild(i);
				Object.Destroy(child.gameObject);
			}
		}
		// リストクリア
		this.MacroButtonList.Clear();
	}
	#endregion

	#region NGUIリフレクション
	public void OnLog()
	{
		GUIChatLog.Toggle();
	}

	public void OnInputSelect()
	{
		var t = this.Attach;

        t.ChatWindow.SetActive(!t.ChatWindow.activeSelf);

		if (t.input != null)
			t.input.isSelected = true;
		if (t.chatTypeTween != null)
			t.chatTypeTween.Play(false);
	}
	public void OnInputSubmit()
	{
		string rawText = UIInput.current.value;
		// NGワードチェック
        string text = ApplicationController.Language == Language.Japanese ? NGWord.DeleteNGWord(rawText) : FilterWordController.Instance.GetFilteredWord(rawText);

		// 文字を消してフォーカスを外す
		UIInput.current.value = "";
		UIInput.current.RemoveFocus();

		// チャット送信
		this._SendChat(text, rawText);
	}

    public void OnClickSend()
    {
        string rawText = Attach.input.value;
        // NGワードチェック
        string text = ApplicationController.Language == Language.Japanese ? NGWord.DeleteNGWord(rawText) : FilterWordController.Instance.GetFilteredWord(rawText);

        // 文字を消してフォーカスを外す
        Attach.input.value = "";
        // チャット送信
        this._SendChat(text, rawText);
    }
	public void OnInputChange()
	{
	}

	public void OnChatType()
	{
		this._SetChatTypeActive(!this.IsChatTypeActive);
	}
	public void OnChatTypeSay()
	{
        //Old
		//this._SetChatType(ChatType.Say);
		//this._SetChatTypeActive(false);
	}
	public void OnChatTypeGuild()
	{
        //Old
		//this._SetChatType(ChatType.Guild);
		//this._SetChatTypeActive(false);
	}
	public void OnChatTypeParty()
	{
        //Old
		//this._SetChatType(ChatType.Team);
		//this._SetChatTypeActive(false);
	}
	public void OnChatTypeWis(long playerId, string name)
	{
        //Old
		//this._SetChatType(ChatType.Whisper);
		//this._SetChatTypeActive(false);

        //New
        this.SetChatTypeNew(ChatType.Whisper);
        this.WhisperPlayerId = playerId;
        this.WhisperPlayerName = name;
	}
	public void OnChatTypeShout()
	{
        //Old
		//this._SetChatType(ChatType.Shout);
		//this._SetChatTypeActive(false);
	}
	public void OnMacro()
	{
        //Old
		//this._SetMacroActive(!this.IsMacroActive);
	}

    public void OnCloseClick()
    {
        var t = this.Attach;

        t.ChatWindow.SetActive(!t.ChatWindow.activeSelf);

        if (t.input != null)
            t.input.isSelected = true;
        if (t.chatTypeTween != null)
            t.chatTypeTween.Play(false);
    }

    void SetChatTypeNew(ChatType type)
    {
        switch (type)
        {
            case ChatType.Say:
                this.Attach.chatType.text = "[综合]";
                break;
            case ChatType.Whisper:
                this.Attach.chatType.text = "[私聊]";
                break;
        }
        this.ChatType = type;
        this.Attach.input.isSelected = true;
    }
	#endregion

    #region 收到信息处理

    
    public void OnReceiveMessage(ChatInfo chatInfo)
    {
        string text = "";
        var name = chatInfo.name.Split(':');
        var relName = name[0].Substring(9, name[0].Length - 10);
        switch (chatInfo.chatType)
        {
            case ChatType.Whisper:
                text = chatInfo.type + "[url=" + (int)ChatType.Whisper + "." + chatInfo.playerID + "." + relName + "." + "]" + " " + name[0] + "[/url] " + "" + "悄悄地对你说:" + " " + chatInfo.text;
                break;
            case ChatType.Say:
                text = "[url=" + (int)ChatType.Say + "." + "]" + chatInfo.type + "[/url] " + name[0] + ":" + " " + chatInfo.text;
                break;
            case ChatType.Team:
                break;
            case ChatType.Shout:
                break;
            case ChatType.System:
                text = chatInfo.name + chatInfo.text;
                break;
            default:
                text = chatInfo.name + chatInfo.text;
                break;
        }

        if (!string.IsNullOrEmpty(text))
        {
            this.Attach.ChatTextList.Add(text);
        }
    }

    #endregion
}


/// <summary>
/// チャットポップアップキュー
/// </summary>
public class ChatPopupQueue : IPopupQueue
{
	#region フィールド＆プロパティ
	Queue<ChatInfo> Queue { get; set; }
	#endregion

	#region 設定
	/// <summary>
	/// クローン
	/// </summary>
	public ChatPopupQueue Clone()
	{
		var t = (ChatPopupQueue)MemberwiseClone();
		if (this.Queue != null)
			t.Queue = new Queue<ChatInfo>(this.Queue);
		return t;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ChatPopupQueue()
	{
		this.Queue = new Queue<ChatInfo>();
	}

	/// <summary>
	/// キュー登録
	/// </summary>
	public void Enqueue(ChatInfo item)
	{
		this.Queue.Enqueue(item);
	}
	#endregion

	#region IPopupQueue
	/// <summary>
	/// キューが存在するかどうか
	/// </summary>
	public bool IsQueue
	{
		get
		{
			return this.Queue.Count > 0;
		}
	}

	/// <summary>
	/// 全てのキューをクリアする
	/// </summary>
	public void Clear()
	{
		this.Queue.Clear();
	}

	/// <summary>
	/// キューから取り出してアイテムを生成する
	/// </summary>
	public GUIPopupItem Create(GameObject prefab, Transform parent)
	{
		try
		{
			// キューから取り出してアイテムを作成する
			var chatInfo = this.Queue.Dequeue();
                        //New Message Handle
            
		    if (chatInfo != null)
		    {
                GUIChat.Instance.OnReceiveMessage(chatInfo);
		    }
		    return null;
            //End
			var com = GUIChatItem.Create(prefab, parent, 0);
			// アイテムセットアップ
			com.Setup(chatInfo);
			// 作成した GameObject から PopupItem を取得する
			var item = com.GetComponent(typeof(GUIPopupItem)) as GUIPopupItem;
			return item;
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("ChatPopupItemQueue.Create:" + e);
			return null;
		}
	}
	#endregion IPopupQueue
}

