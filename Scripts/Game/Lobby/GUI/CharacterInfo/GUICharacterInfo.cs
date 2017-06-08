/// <summary>
/// キャラクター詳細
/// 
/// 2016/03/25
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XUI.CharacterInfo;
using System;

public class GUICharacterInfo : Singleton<GUICharacterInfo> {

	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	CharacterInfoView _viewAttach = null;
	CharacterInfoView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// キャラアイテム
	/// </summary>
	[SerializeField]
	GameObject _charaItem = null;
	GameObject CharaItem { get { return _charaItem; } }
	// 表示キャラ
	GUICharaItem SelectChara { get; set; }

	/// <summary>
	/// インスタンス化したキャラアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	GameObject _attachCharaItem = null;
	GameObject AttachCharaItem { get { return _attachCharaItem; } }

	/// <summary>
	/// キャラクターボード
	/// </summary>
	private CharaBoard CharaBoard { get { return ScmParam.Lobby.CharaBoard; } }

	/// <summary>
	/// スロットリスト
	/// </summary>
	[SerializeField]
	GUICharaSlotList _slotList = null;
	GUICharaSlotList SlotList { get { return _slotList; } }

	#region フォーマット

	#region 名前・リビルド・コスト
	/// <summary>
	/// 名前
	/// </summary>
	[SerializeField]
	string _charaNameFormat = "{0}";
	string CharaNameFormat { get { return _charaNameFormat; } }
	/// <summary>
	/// リビルドタイム
	/// </summary>
	[SerializeField]
	string _rebuildTimeFormat = "{0}";
	string RebuildTimeFormat { get { return _rebuildTimeFormat; } }
	/// <summary>
	/// コスト
	/// </summary>
	[SerializeField]
	string _charaCostFormat = "{0}";
	string CharaCostFormat { get { return _charaCostFormat; } }

	#endregion

	#region ステータス情報

	/// <summary>
	/// 合計表示のフォーマット
	/// </summary>
	[SerializeField]
	string _totalFormat = "{0}";
	string TotalFormat { get { return _totalFormat; } }

	/// <summary>
	/// 基礎表示のフォーマット
	/// </summary>
	[SerializeField]
	string _baseFormat = "{0}";
	string BaseFormat { get { return _baseFormat; } }

	/// <summary>
	/// スロット補正表示のフォーマット
	/// </summary>
	[SerializeField]
	string _slotFormat = "{0}";
	string SlotFormat { get { return _slotFormat; } }

	/// <summary>
	/// シンクロ補正表示のフォーマット
	/// </summary>
	[SerializeField]
	string _syncFormat = "{0:+#;-#;+0}";
	string SyncFormat { get { return _syncFormat; } }

	#endregion

	#region 強化関連
	/// <summary>
	/// ランク
	/// </summary>
	[SerializeField]
	string _rankFormat = "{0}";
	string RankFormat { get { return _rankFormat; } }

	/// <summary>
	/// レベル
	/// </summary>
	[SerializeField]
	string _levelFormat = "{0}";
	string levelFormat { get { return _levelFormat; } }

	/// <summary>
	/// EXP
	/// </summary>
	[SerializeField]
	string _expFormat = "{0:#,0}";
	string ExpFormat { get { return _expFormat; } }

	/// <summary>
	/// 残りシンクロ回数
	/// </summary>
	[SerializeField]
	string _synchroRemainfoFormat = "{0}";
	string SynhroRemainFormat { get { return _synchroRemainfoFormat; } }

	/// <summary>
	/// 強化スロット数
	/// </summary>
	[SerializeField]
	string _powerupSloNumtFormat = "{0}/{1}";
	string PowerupSloNumtFormat { get { return _powerupSloNumtFormat; } }

	#endregion

	#endregion


	/// <summary>
	/// アクティブ
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// コントローラー
	/// </summary>
	IController Controller { get; set; }
	
	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.Controller = null;
		this.SelectChara = null;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();
		this.CreateCharaItem();
#if XW_DEBUG
		if (this.ViewAttach != null)
		{
			this.ViewAttach.DebugCopyLabel();
		}
#endif

	}
	void Start()
	{
		this.Construct();
		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true, this.IsStartActive,0);

	}
	void Construct()
	{
		// モデル生成
		var model = new Model();
		// 基礎データ・ステータス
		model.CharaNameFormat = this.CharaNameFormat;
		model.CharaCsotFormat = this.CharaCostFormat;
		model.RebuildTimeFormat = this.RebuildTimeFormat;
		model.TotalHitPointFormat = this.TotalFormat;
		model.BaseHitPointFormat = this.BaseFormat;
		model.SlotHitPointFormat = this.SlotFormat;
		model.SyncHitPointFormat = this.SyncFormat;
		model.TotalAttackFormat = this.TotalFormat;
		model.BaseAttackFormat = this.BaseFormat;
		model.SlotAttackFormat = this.SlotFormat;
		model.SyncAttackFormat= this.SyncFormat;
		model.TotalDefenseFormat = this.TotalFormat;
		model.BaseDefenseFormat = this.BaseFormat;
		model.SlotDefenseFormat = this.SlotFormat;
		model.SyncDefenseFormat = this.SyncFormat;
		model.TotalExtraFormat = this.TotalFormat;
		model.BaseExtraFormat = this.BaseFormat;
		model.SlotExtraFormat = this.SlotFormat;
		model.SyncExtraFormat = this.SyncFormat;
		// 強化関連
		model.RankFormat = this.RankFormat;
		model.PowerupLevelFormat = this.levelFormat;
		model.PowerupExpFormat = this.ExpFormat;
		model.SynchroRemainFormat = this.SynhroRemainFormat;
		model.PowerupSlotFormat = this.PowerupSloNumtFormat;


		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラー生成
		var controller = new Controller(model, view, this.SelectChara,this.SlotList, this.CharaBoard);
		this.Controller = controller;
		this.Controller.OnLock += this.HandleSetLockPlayerCharacter;

	}
	/// <summary>
	/// キャラアイテムの作成
	/// </summary>
	void CreateCharaItem()
	{
		if (this.AttachCharaItem == null)
		{
			Debug.LogWarning("AttachCharaItem is Null!!");
			return;
		}
		if( this.CharaItem == null)
		{
			Debug.LogWarning("CharaIrem is Null!!");
			return;
		}
		var obj = SafeObject.Instantiate(this.CharaItem);
		if(obj == null)
		{
			GameObject.Destroy(obj);
			return;
		}
		var go = obj as GameObject;
		if(go == null)
		{
			Debug.LogWarning(obj.name + "is Mismatch type!! [GameObject]");
			return;
		}

		// 子供を消す
		this.AttachCharaItem.DestroyChild();

		// 親子付け
		go.SetParentWithLayer(this.AttachCharaItem, false);
		// アクティブ化
		if (!go.activeSelf)
		{
			go.SetActive(true);
		}

		// コンポーネント取得
		this.SelectChara = go.GetComponentInChildren(typeof(GUICharaItem)) as GUICharaItem;
		if(this.SelectChara == null)
		{
			Debug.LogWarning("Chara is null!! go.GetComponentInChildren(typeof(GUICharaItem))");
			return;
		}
	}
	/// <summary>
	/// 破棄
	/// </summary>
	void onDestroy()
	{
		if(this.Controller != null)
		{
			this.Controller.Dispose();
		}
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetActive(false, false, false,0);
	}
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open(ulong uuid)
	{
		if (Instance != null)Instance.SetActive(true, false, true,uuid);
	}
	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen(ulong uuid)
	{
		if (Instance != null) Instance.SetActive(true, false, false, uuid);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive(bool isActive, bool isTweenSkip, bool isSetup,ulong uuid)
	{
		if (isSetup)
		{
			this.Setup(uuid);
		}

		if (this.Controller != null)
		{
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}
	#endregion

	#region 各種情報更新
	/// <summary>
	/// 初期設定
	/// </summary>
	void Setup(ulong uuid)
	{
		if (this.Controller != null)
		{
			this.Controller.Setup();
		}

		// 仕様確認
		if (uuid != 0)
		{
			// キャラクターの情報取得を希望する
			LobbyPacket.SendPlayerCharacter(uuid, this.Response);
		}
	}

	#endregion

	#region 通信系

	#region PlayerCharacter パケット
	void SendPlayerCharacterPacket(ulong uuid)
	{
		// キャラクターの情報取得を希望する
		LobbyPacket.SendPlayerCharacter(uuid, this.Response);
		// 通信中の表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}

	/// <summary>
	/// PlayerCharacter パケットレスポンス
	/// </summary>
	/// <param name="args"></param>
	void Response(LobbyPacket.PlayerCharacterResArgs args)
	{
		// 通信中を閉じる
		GUISystemMessage.Close();

		// データをセットする
		this.SetupPlayerCharacterData(args.CharaInfo, args.SlotBonusHitPoint, args.SlotBonusAttack, args.SlotBonusDefense, args.SlotBonusExtra, args.SlotList);
	}
	/// <summary>
	/// キャラクター詳細をセットする
	/// </summary>
	void SetupPlayerCharacterData(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList)
	{
		if (this.Controller != null)
		{
			this.Controller.SetCharaInfo(info,slotHitPoint, slotAttack, slotDefense, slotExtra, slotList);
		}
#if XW_DEBUG
		if (this.ViewAttach != null)
		{
			var text = string.Format(
				"UUID:{0:#,0}\r\n" +
				"UseCount:{1:#,0}"
				, info.UUID, info.UseCount);
			this.ViewAttach.DebugSetLabel(text);
		}
#endif
	}
	#endregion

	#region　SetLockPlayerCharacter パケット
	public void HandleSetLockPlayerCharacter(object sender,SetLockPlayerCharacterEventArgs e)
	{
		// ロック状態の変更を希望する
		LobbyPacket.SendSetLockPlayerCharacter(e.UUID, e.IsLock, this.Response);
		// 通信中の表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));

	}

	/// <summary>
	/// SetLockPlayerCharacter パケットレスポンス
	/// </summary>
	/// <param name="args"></param>
	void Response(LobbyPacket.SetLockPlayerCharacterResArgs args)
	{
		LockResult(args.Result, args.UUID, args.IsLock);
	}
	void LockResult(bool result,ulong uuid,bool isLock)
	{
		// リザルト確認
		if (!result)
		{
			// ロック失敗メッセージ表示
			this.Controller.CharacterLockError();

			// 失敗していたら閉じる
			GUIController.Clear();
			return;
		}

		// キャラクター情報の再取得を行う
		this.SendPlayerCharacterPacket(uuid);
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
		/// <summary>
		/// 初期化
		/// 機能追加時はここでAddEventすること
		/// </summary>
		public GUIDebugParam()
		{
			this.AddEvent(this.DebugRebuildTime);
		}

		[SerializeField]
		RebuildTimeEvent _debugRebuildTime = new RebuildTimeEvent();
		public RebuildTimeEvent DebugRebuildTime { get { return _debugRebuildTime; } }
		[System.Serializable]
		public class RebuildTimeEvent : IDebugParamEvent
		{
			public event System.Action<int> UpdateRebuild = delegate { };
			[SerializeField]
			bool update = false;
			[SerializeField]
			int rebuildTime = 0;

			public void Update()
			{
				if (this.update)
				{
					this.UpdateRebuild(this.rebuildTime);
				}
			}
		}

		[SerializeField]
		RebuildTimeEvent _characterUUID = new RebuildTimeEvent();
		public RebuildTimeEvent CharacterUUID { get { return _characterUUID; } }
		[System.Serializable]
		public class CharacterUUIDEvent : IDebugParamEvent
		{
			public event System.Action<ulong> UpdateUUID = delegate { };
			[SerializeField]
			bool update = false;
			[SerializeField]
			ulong UUID = 0;

			public void Update()
			{
				if (this.update)
				{
					this.update = false;
					this.UpdateUUID(this.UUID);
				}
			}
		}


		[SerializeField]
		int _debugCharaCost = 0;
		int DebugCharaCost { get { return _debugCharaCost; } }


	}
	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () => { SetActive(true,true,true,0); };

		d.DebugRebuildTime.UpdateRebuild += (time) =>
		{
//			Debug.Log("UpdateRebuild;" + time);
//			this.SetRebuildTime(time);
		}; 

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
