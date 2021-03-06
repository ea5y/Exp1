/// <summary>
/// オプション
/// 
/// 2014/06/04
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.NGWord;

public class GUIOption : Singleton<GUIOption>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// アカウントコードフォーマット
	/// </summary>
	[SerializeField]
	string _accountCodeFormat = "{0}";
	string AccountCodeFormat { get { return _accountCodeFormat; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach = new AttachObject();
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween = null;
		public UIButton homeButton = null;
		public UIButton closeButton = null;
		public UILabel accountCodeLabel = null;

		public UITable itemTable = null;
		public Prefab itemPrefab = new Prefab();
		[System.Serializable]
		public class Prefab
		{
			public GUIOptionItemCheckBox prefabCheckBox = null;
			public GUIOptionItemColor prefabColor = null;
			public GUIOptionItemInput prefabInput = null;
			public GUIOptionItemPopupList prefabPopupList = null;
			public GUIOptionItemSlider prefabSlider = null;
		}
	}

	// アクティブ設定
	bool IsActive { get; set; }
	// 閉じるボタンを押した時のデリゲート
	System.Action OnCloseFunction { get; set; }
	// 開いた時のコンフィグデータを保存しておく
	ConfigFile.Config ConfigData { get; set; }
	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.IsActive = false;
		this.ConfigData = new ConfigFile.Config();
		this.OnCloseFunction = delegate { };
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();

		// オプションアイテムの作成
		this.StartCoroutine(this.ItemSetupCoroutine());

		// 表示設定
		this._SetActive(this.IsStartActive, true, true, null);
	}
	#endregion

	#region アクティブ設定
	public static void Close()
	{
		SetActive(false);
	}
	public static void SetActive(bool isActive)
	{
		SetActive(isActive, true, true, null);
	}
	public static void SetActive(bool isActive, bool isUseHome, bool isUseClose, System.Action onClose)
	{
		if (Instance != null) Instance._SetActive(isActive, isUseHome, isUseClose, onClose);
	}
	void _SetActive(bool isActive, bool isUseHome, bool isUseClose, System.Action onClose)
	{
		this.IsActive = isActive;
		this.OnCloseFunction = (onClose != null ? onClose : delegate { });

		if (isActive)
		{
			this.ConfigData = ConfigFile.Instance.Data.Clone();
			this.RepositionItem();
		}

		// UI設定
		{
			var t = this.Attach;
			if (t.homeButton != null)
				t.homeButton.gameObject.SetActive(isUseHome);
			if (t.closeButton != null)
				t.closeButton.gameObject.SetActive(isUseClose);
			// アクティブ化
			if (t.rootTween != null)
				t.rootTween.Play(isActive);
			else
				this.gameObject.SetActive(isActive);
		}

		// その他UIの表示設定
		GUILobbyResident.SetActive(!isActive);
		GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX139_Option_ScreenTitle));
		GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX140_Option_HelpMessage));
	}
	#endregion

	#region アカウントコード設定
	/// <summary>
	/// アカウントコード設定
	/// </summary>
	public static void SetAccountCode(string accountCode)
	{
		if (Instance != null) Instance._SetAccountCode(accountCode);
	}
	void _SetAccountCode(string accountCode)
	{
		var t = this.Attach;
		if (t.accountCodeLabel != null)
		{
			t.accountCodeLabel.text = string.Format(this.AccountCodeFormat, accountCode);
		}
	}
	#endregion

	#region アイテム操作
	IEnumerator ItemSetupCoroutine()
	{
		// アイテム削除
		this.DestroyItem();

		// アイテム追加
		this.AddItem();

		// Unityの仕様で1フレーム置かないとちゃんと削除が完了しない
		yield return null;
		// 再配置
		this.RepositionItem();
	}
	void AddItem()
	{
		int index = 0;
		//// チャットポップアップ数
		//{
		//	var desc = ObsolateSrc.Option.ChatPopupNumDesc;
		//	var dict = ObsolateSrc.Option.ChatPopupNumDict;
		//	this.AddItemPopupList(index++,
		//		(item) =>
		//		{
		//			item.Setup(desc, ConfigFile.Option.ChatPopupNumStr, dict,
		//				(value) =>
		//				{
		//					ConfigFile.Option.ChatPopupNumDB = value;
		//					GUIChat.SetPopupNum(ConfigFile.Option.ChatPopupNum);
		//				});
		//		});
		//}
		//// チャットポップの時間
		//{
		//	var desc = ObsolateSrc.Option.ChatPopupTimerDesc;
		//	var steps = ObsolateSrc.Option.ChatPopupTimerSteps;
		//	this.AddItemSlider(index++,
		//		(item) =>
		//		{
		//			item.Setup(desc, ConfigFile.Option.ChatPopupTimer, ConfigFile.Option.ChatPopupTimerMin, ConfigFile.Option.ChatPopupTimerMax, steps,
		//				(value) =>
		//				{
		//					ConfigFile.Option.ChatPopupTimer = value;
		//					GUIChat.SetPopupTimer(ConfigFile.Option.ChatPopupTimer);
		//				},
		//				(label, value) =>
		//				{
		//					label.text = string.Format(ObsolateSrc.Option.ChatPopupTimerFormat, value);
		//				});
		//		});
		//}
		// BGM
		{
			var desc = MasterData.GetText(TextType.TX501_Option_BGMDesc);
			var steps = ObsolateSrc.Option.BgmSteps;
			this.AddItemSlider(index++,
				(item) =>
				{
					item.Setup(desc, ConfigFile.Option.Bgm, ConfigFile.Option.BgmMin, ConfigFile.Option.BgmMax, steps,
						(value) =>
						{
							ConfigFile.Option.Bgm = value;
							SoundController.Bgm_Volume = ConfigFile.Option.Bgm;
						},
						(label, value) =>
						{
							label.text = string.Format(ObsolateSrc.Option.BgmFormat, value);
						});
				});
		}
		// SE
		{
			var desc = MasterData.GetText(TextType.TX502_Option_SEDesc);
			var steps = ObsolateSrc.Option.SeSteps;
			this.AddItemSlider(index++,
				(item) =>
				{
					item.Setup(desc, ConfigFile.Option.Se, ConfigFile.Option.SeMin, ConfigFile.Option.SeMax, steps,
						(value) =>
						{
							ConfigFile.Option.Se = value;
							SoundController.Se_Volume = ConfigFile.Option.Se;
						},
						(label, value) =>
						{
							label.text = string.Format(ObsolateSrc.Option.SeFormat, value);
						});
				});
		}
		// Voice
		//if (Scm.Common.Utility.Language == Scm.Common.GameParameter.Language.Japanese)
		//{
		//	var desc = MasterData.GetText(TextType.TX503_Option_VoiceDesc);
		//	var steps = ObsolateSrc.Option.VoiceSteps;
		//	this.AddItemSlider(index++,
		//		(item) =>
		//		{
		//			item.Setup(desc, ConfigFile.Option.Voice, ConfigFile.Option.VoiceMin, ConfigFile.Option.VoiceMax, steps,
		//				(value) =>
		//				{
		//					ConfigFile.Option.Voice = value;
		//					SoundController.Voice_Volume = ConfigFile.Option.Voice;
		//				},
		//				(label, value) =>
		//				{
		//					label.text = string.Format(ObsolateSrc.Option.VoiceFormat, value);
		//				});
		//		});
		//}
		//// マクロボタン閉じる
		//{
		//	var desc = ObsolateSrc.Option.MacroCloseDesc;
		//	this.AddItemCheckBox(index++,
		//		(item) =>
		//		{
		//			item.Setup(desc, ConfigFile.Option.IsMacroClose,
		//				(value) =>
		//				{
		//					ConfigFile.Option.IsMacroClose = value;
		//				});
		//		});
		//}
		//// ロックオン距離
		//{
		//	var desc = ObsolateSrc.Option.LockonRangeDesc;
		//	var dict = ObsolateSrc.Option.LockonRangeDict;
		//	this.AddItemPopupList(index++,
		//		(item) =>
		//		{
		//			item.Setup(desc, ConfigFile.Option.LockonRangeStr, dict,
		//				(value) =>
		//				{
		//					ConfigFile.Option.LockonRangeDB = value;
		//					GUIObjectUI.SetLockonRange(ConfigFile.Option.LockonRange);
		//				});
		//		});
		//}
		//// マクロボタンの1行の表示数
		//{
		//    var desc = ObsolateSrc.Option.MacroButtonColumnDesc;
		//    var dict = ObsolateSrc.Option.MacroButtonColumnDict;
		//    this.AddItemPopupList(index++,
		//        (item) =>
		//        {
		//            item.Setup(desc, ConfigFile.Option.MacroButtonColumnStr, dict,
		//                (value) =>
		//                {
		//                    ConfigFile.Option.MacroButtonColumnDB = value;
		//                    GUIChat.MacroColumnSetup(ConfigFile.Option.MacroButtonColumn);
		//                });
		//        });
		//}
		// マクロ編集
		{
			for (int i = 0, max = ConfigFile.Option.ChatMacroList.Count; i < max; i++)
			{
				string desc, empty;
				var info = ConfigFile.Option.ChatMacroList[i];

				// マクロボタン名
				//desc = string.Format(ObsolateSrc.Option.MacroButtonDescFormat, i + 1);
				//empty = ObsolateSrc.Option.MacroButtonEmpty;
				//this.AddItemInput(index++,
				//	(item) =>
				//	{
				//		item.Setup(desc, info.ButtonName, empty, ObsolateSrc.Option.MacroButtonType, ObsolateSrc.Option.MacroButtonLength,
				//			(input, value) =>
				//			{
				//				var text = NGWord.DeleteNGWord(value);
				//				input.value = text;
				//				info.Setup(text, info.Macro);
				//				ConfigFile.Option.ChatMacroList[info.Index] = info;
				//				GUIChat.MacroItemSetup(info.Index, info);
				//			});
				//	});

				// マクロ内容
				desc = string.Format(MasterData.GetText(TextType.TX504_Option_MacroDescFormat), i + 1);
				empty = ObsolateSrc.Option.MacroEmpty;
				this.AddItemInput(index++,
					(item) =>
					{
						item.Setup(desc, info.Macro, empty, ObsolateSrc.Option.MacroType, ObsolateSrc.Option.MacroLength,
							(input, value) =>
							{
								var text = NGWord.DeleteNGWord(value);
								input.value = text;
								info.Setup(info.ButtonName, text);
								ConfigFile.Option.ChatMacroList[info.Index] = info;
								GUIChat.MacroItemSetup(info.Index, info);
							});
					});
			}
		}
		//{
		//    bool data = true;
		//    this.AddItemCheckBox(index++, (item) => { item.Setup("----------ここから下はダミーデータ----------", data, (value) => { data = value; }); });
		//}
		//{
		//    Dictionary<int, string> dict = new Dictionary<int, string>()
		//    {
		//        { 0, "非表示" },
		//        { 1, "1" },
		//        { 2, "2" },
		//        { 3, "3" },
		//        { 4, "最大表示" },
		//    };
		//    string data = "最大表示";
		//    this.AddItemPopupList(index++, (item) => { item.Setup("ObjectUIの表示レベル", data, dict, null); });
		//}
		//{
		//    bool data = true;
		//    this.AddItemCheckBox(index++, (item) => { item.Setup("上下カメラ反転", data, null); });
		//}
		//{
		//    bool data = true;
		//    this.AddItemCheckBox(index++, (item) => { item.Setup("左右カメラ反転", data, null); });
		//}
		//{
		//    float data = 800f;
		//    this.AddItemSlider(index++,
		//        (item) =>
		//        {
		//            item.Setup("BGM(step=0)", data, 0f, 1000f, 0,
		//                null,
		//                (label, value) =>
		//                {
		//                    label.text = string.Format("{0:0} dB", value);
		//                });
		//        });
		//}
		//{
		//    float data = 1000f;
		//    this.AddItemSlider(index++,
		//        (item) =>
		//        {
		//            item.Setup("SE(step=21)", data, 0f, 1000f, 21,
		//                null,
		//                (label, value) =>
		//                {
		//                    label.text = string.Format("{0:0}dB", value);
		//                });
		//        });
		//}
		//{
		//    float data = 8f;
		//    this.AddItemSlider(index++,
		//        (item) =>
		//        {
		//            item.Setup("時間(step=0)", data, 0.5f, 16f, 0,
		//                null,
		//                (label, value) =>
		//                {
		//                    label.text = string.Format("{0:0.0} 秒", value);
		//                });
		//        });
		//}
		//{
		//    this.AddItemInput(index++, (item) => { item.Setup("UIInput.KeyboardType.Default", "初期文字列", "空の場合の文字列", UIInput.KeyboardType.Default, 0, null); });
		//    this.AddItemInput(index++, (item) => { item.Setup("UIInput.KeyboardType.ASCIICapable", "初期文字列", "空の場合の文字列", UIInput.KeyboardType.ASCIICapable, 0, null); });
		//    this.AddItemInput(index++, (item) => { item.Setup("UIInput.KeyboardType.NumbersAndPunctuation", "初期文字列", "空の場合の文字列", UIInput.KeyboardType.NumbersAndPunctuation, 0, null); });
		//    this.AddItemInput(index++, (item) => { item.Setup("UIInput.KeyboardType.URL", "初期文字列", "空の場合の文字列", UIInput.KeyboardType.URL, 0, null); });
		//    this.AddItemInput(index++, (item) => { item.Setup("UIInput.KeyboardType.NumberPad", "初期文字列", "空の場合の文字列", UIInput.KeyboardType.NumberPad, 0, null); });
		//    this.AddItemInput(index++, (item) => { item.Setup("UIInput.KeyboardType.PhonePad", "初期文字列", "空の場合の文字列", UIInput.KeyboardType.PhonePad, 0, null); });
		//    this.AddItemInput(index++, (item) => { item.Setup("UIInput.KeyboardType.NamePhonePad", "初期文字列", "空の場合の文字列", UIInput.KeyboardType.NamePhonePad, 0, null); });
		//    this.AddItemInput(index++, (item) => { item.Setup("UIInput.KeyboardType.EmailAddress", "初期文字列", "空の場合の文字列", UIInput.KeyboardType.EmailAddress, 0, null); });
		//}
	}
	/// <summary>
	/// アイテム追加(チェックボックス)
	/// </summary>
	void AddItemCheckBox(int index, System.Action<GUIOptionItemCheckBox> setupFunc)
	{
		var prefab = this.Attach.itemPrefab.prefabCheckBox;
		if (prefab == null)
			return;
		var table = this.Attach.itemTable;
		if (table == null)
			return;

		// アイテム作成
		var item = GUIOptionItemCheckBox.Create(prefab.gameObject, table.transform, index);
		if (item == null)
			return;
		if (setupFunc != null)
			setupFunc(item);
	}
	/// <summary>
	/// アイテム追加(カラー)
	/// </summary>
	void AddItemColor(int index, System.Action<GUIOptionItemColor> setupFunc)
	{
		var prefab = this.Attach.itemPrefab.prefabColor;
		if (prefab == null)
			return;
		var table = this.Attach.itemTable;
		if (table == null)
			return;

		// アイテム作成
		var item = GUIOptionItemColor.Create(prefab.gameObject, table.transform, index);
		if (item == null)
			return;
		if (setupFunc != null)
			setupFunc(item);
	}
	/// <summary>
	/// アイテム追加(入力)
	/// </summary>
	void AddItemInput(int index, System.Action<GUIOptionItemInput> setupFunc)
	{
		var prefab = this.Attach.itemPrefab.prefabInput;
		if (prefab == null)
			return;
		var table = this.Attach.itemTable;
		if (table == null)
			return;

		// アイテム作成
		var item = GUIOptionItemInput.Create(prefab.gameObject, table.transform, index);
		if (item == null)
			return;
		if (setupFunc != null)
			setupFunc(item);
	}
	/// <summary>
	/// アイテム追加(ポップアップリスト)
	/// </summary>
	void AddItemPopupList(int index, System.Action<GUIOptionItemPopupList> setupFunc)
	{
		var prefab = this.Attach.itemPrefab.prefabPopupList;
		if (prefab == null)
			return;
		var table = this.Attach.itemTable;
		if (table == null)
			return;

		// アイテム作成
		var item = GUIOptionItemPopupList.Create(prefab.gameObject, table.transform, index);
		if (item == null)
			return;
		if (setupFunc != null)
			setupFunc(item);
	}
	/// <summary>
	/// アイテム追加(スライダー)
	/// </summary>
	void AddItemSlider(int index, System.Action<GUIOptionItemSlider> setupFunc)
	{
		var prefab = this.Attach.itemPrefab.prefabSlider;
		if (prefab == null)
			return;
		var table = this.Attach.itemTable;
		if (table == null)
			return;

		// アイテム作成
		var item = GUIOptionItemSlider.Create(prefab.gameObject, table.transform, index);
		if (item == null)
			return;
		if (setupFunc != null)
			setupFunc(item);
	}
	/// <summary>
	/// 再配置
	/// </summary>
	void RepositionItem()
	{
		var table = this.Attach.itemTable;
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
		var table = this.Attach.itemTable;
		if (table != null)
		{
			var t = table.transform;
			for (int i = 0, max = t.childCount; i < max; i++)
			{
				var child = t.GetChild(i);
				Object.Destroy(child.gameObject);
			}
		}
	}
	#endregion

	#region ファイル書き込み
	/// <summary>
	/// アクティブ化した時とコンフィグデータが違う場合は
	/// 更新されているのでファイルに書き込む
	/// </summary>
	void ConfigWrite()
	{
		// 開いた時とデータが違う場合はファイルに書き込み
		if (!this.ConfigData.Equals(ConfigFile.Instance.Data))
		{
			ConfigFile.Instance.Write();
			this.ConfigData = ConfigFile.Instance.Data.Clone();
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnHome()
	{
		this.ConfigWrite();
		Close();
	}
	public void OnClose()
	{
		this.ConfigWrite();
		this.OnCloseFunction();
		Close();
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
		public bool execute;
		public bool isActive;
		public bool isUseHome;
		public bool isUseClose;
	}

	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.execute)
		{
			t.execute = false;
			this._SetActive(t.isActive, t.isUseHome, t.isUseClose, null);
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
