/// <summary>
/// 進化合成表示
/// 
/// 2016/02/02
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUIEvolution : Singleton<GUIEvolution>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	XUI.Evolution.EvolutionView _viewAttach = null;
	XUI.Evolution.EvolutionView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// キャラページリスト
	/// </summary>
	[SerializeField]
	GUICharaPageList _charaPageList = null;
	GUICharaPageList CharaPageList { get { return _charaPageList; } }

	/// <summary>
	/// 進化素材リスト
	/// </summary>
	[SerializeField]
	GUIEvolutionMaterialList _materialList = null;
	GUIEvolutionMaterialList MaterialList { get { return _materialList; } }

	/// <summary>
	/// キャラアイテムリソース
	/// </summary>
	[SerializeField]
	GameObject _charaItemResource = null;
	GameObject CharaItemResource { get { return _charaItemResource; } }

	/// <summary>
	/// インスタンス化したベースキャラアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	GameObject _attachBaseChara = null;
	GameObject AttachBaseChara { get { return _attachBaseChara; } }

	/// <summary>
	/// インスタンス化した進化キャラアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	GameObject _attachEvolutionChara = null;
	GameObject AttachEvolutionChara { get { return _attachEvolutionChara; } }

	/// <summary>
	/// 進化素材最大数
	/// </summary>
	[SerializeField]
	int _materialMaxCount = 5;
	int MaterialMaxCount { get { return _materialMaxCount; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// 所持金表示フォーマット
	/// </summary>
	[SerializeField]
	string _haveMoneyFormat = "{0:#,0}";
	string HaveMoneyFormat { get { return _haveMoneyFormat; } }

	/// <summary>
	/// 費用表示フォーマット
	/// </summary>
	[SerializeField]
	string _needMoneyFormat = "{0:#,0}";
	string NeedMoneyFormat { get { return _needMoneyFormat; } }

	/// <summary>
	/// 追加料金表示フォーマット
	/// </summary>
	[SerializeField]
	string _addOnChargeFormat = "{0:#,0}";
	string AddOnChargeFormat { get { return _addOnChargeFormat; } }

	// プレイヤーステータス情報
	PlayerStatusInfo PlayerStatusInfo { get { return NetworkController.ServerValue != null ? NetworkController.ServerValue.PlayerStatusInfo : new PlayerStatusInfo(); } }

	/// <summary>
	/// ベースキャラ
	/// </summary>
	GUICharaItem BaseChara { get; set; }

	/// <summary>
	/// 進化キャラ
	/// </summary>
	GUICharaItem EvolutionChara { get; set; }

	/// <summary>
	/// モデル
	/// </summary>
	XUI.Evolution.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	XUI.Evolution.IView View { get; set; }

	/// <summary>
	/// コントローラ
	/// </summary>
	XUI.Evolution.Controller Controller { get; set; }
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();
		this.BaseChara = this.CreateCharaItem(this.AttachBaseChara);
		this.EvolutionChara = this.CreateCharaItem(this.AttachEvolutionChara);
	}

	void Start()
	{
		this.Construct();

		// 素材リスト設定
		if(this.MaterialList != null)
		{
			this.MaterialList.SetCapacity(this.MaterialMaxCount);
		}

		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true);
	}

	/// <summary>
	/// キャラアイテム生成
	/// </summary>
	private GUICharaItem CreateCharaItem(GameObject attachObj)
	{
		// 子供を消す
		attachObj.DestroyChild();

		GUICharaItem item = GUICharaItem.Create(this.CharaItemResource, attachObj.transform, 0);
		if (item == null)
		{
			Debug.LogWarning("CharaItem is null!!");
			return null;
		}

		return item;
	}

	void Construct()
	{
		// モデル生成
		var model = new XUI.Evolution.Model();
		this.Model = model;
		this.Model.HaveMoneyFormat = this.HaveMoneyFormat;
		this.Model.NeedMoneyFormat = this.NeedMoneyFormat;
		this.Model.AddOnChargeFormat = this.AddOnChargeFormat;

		// ビュー生成
		XUI.Evolution.IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.Evolution.IView)) as XUI.Evolution.IView;
		}
		this.View = view;

		// コントローラー生成
		var controller = new XUI.Evolution.Controller(model, view, this.CharaPageList, this.BaseChara, this.MaterialList, this.EvolutionChara);
		this.Controller = controller;
		this.Controller.OnFusion += this.HandleFusion;
		this.Controller.OnFusionCalc += this.HandleFusionCalc;
		this.Controller.OnPlayerCharacter += this.HandlePlayerCharacter;
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	void MemberInit()
	{
		this.BaseChara = null;

		this.Model = null;
		this.View = null;
		this.Controller = null;
	}

	/// <summary>
	/// 破棄
	/// </summary>
	void OnDestroy()
	{
		if (this.Controller != null)
		{
			this.Controller.Dispose();
		}
	}
	#endregion

	#region 有効無効
	void OnEnable()
	{
		GUICharaSimpleInfo.AddLockClickEvent(this.HandleLockClickEvent);
		GUICharaSimpleInfo.AddLockResponseEvent(this.HandleLockResponse);
	}
	void OnDisable()
	{
		GUICharaSimpleInfo.RemoveLockClickEvent(this.HandleLockClickEvent);
		GUICharaSimpleInfo.RemoveLockResponseEvent(this.HandleLockResponse);
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetActive(false, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open()
	{
		if (Instance != null)
		{
			Instance.Setup();
			Instance.SetActive(true, false);
		}
	}
	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen()
	{
		if (Instance != null)
		{
			Instance.ReSetup();
			Instance.SetActive(true, false);
		}
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive(bool isActive, bool isTweenSkip)
	{
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
	private void Setup()
	{
		if(this.Controller != null)
		{
			// コントローラ側初期化
			this.Controller.Setup();
		}

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバからキャラボックス情報取得
		LobbyPacket.SendPlayerCharacterBox(this.Response);
	}

	/// <summary>
	/// 再初期化
	/// </summary>
	private void ReSetup()
	{
		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバからキャラボックス情報取得
		LobbyPacket.SendPlayerCharacterBox(this.Response);
	}

	/// <summary>
	/// ステータス情報を更新する
	/// </summary>
	private void SetupStatusInfo(int haveMoney)
	{
		if(this.Controller != null)
		{
			this.Controller.SetupStatusInfo(haveMoney);
		}
	}
	#endregion

	#region 通信系
	#region PlayerCharacterBox パケット
	/// <summary>
	/// PlayerCharacterBoxReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PlayerCharacterBoxResArgs args)
	{
		this.SetupCapacity(args.Capacity, args.Count);

		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);
	}
	/// <summary>
	/// 総数を設定する
	/// </summary>
	void SetupCapacity(int capacity, int count)
	{
		if (this.Controller != null)
		{
			this.Controller.SetupCapacity(capacity, count);
		}
	}
	#endregion

	#region PlayerCharacterAll パケット
	/// <summary>
	/// PlayerCharacterAllReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PlayerCharacterAllResArgs args)
	{
		// 通信中メッセージを閉じる
		GUISystemMessage.Close();

		this.SetupItem(args.List);
	}
	/// <summary>
	/// 個々のアイテムの設定をする
	/// </summary>
	void SetupItem(List<CharaInfo> list)
	{
		if (this.Controller != null)
		{
			this.Controller.SetupCharaInfoList(list);
		}
	}
	#endregion

	#region Evolution パケット
	/// <summary>
	/// 合成イベントハンドラー
	/// </summary>
	void HandleFusion(object sender, XUI.Evolution.FusionEventArgs e)
	{
		LobbyPacket.SendEvolution(e.BaseCharaUUID, e.BaitCharaUUIDList.ToArray(), this.Response);

		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// EvolutionReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.EvolutionResArgs args)
	{
		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// 合成結果
		this.FusionResult(args.Result, args.Money, args.Price, args.AddOnCharge, args.SynchroBonus, args.CharaInfo);
	}
	/// <summary>
	/// 合成結果
	/// </summary>
	void FusionResult(bool result, int money, int price, int addOnCharge, int synchroBonus, CharaInfo charaInfo)
	{
		if(!result)
		{
			// 失敗しているときは閉じる
			GUIController.Clear();
			return;
		}

		if (this.Controller != null)
		{
			this.Controller.FusionResult(result, money, price, addOnCharge, synchroBonus, charaInfo);
		}
	}
	#endregion

	#region EvolutionCalc パケット
	private void HandleFusionCalc(object sender, XUI.Evolution.FusionCalcEventArgs e)
	{
		LobbyPacket.SendEvolutionCalc(e.BaseCharaUUID, e.BaitCharaUUIDList.ToArray(), this.Response);
	}
	/// <summary>
	/// EvolutionCalcReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.EvolutionCalcResArgs args)
	{
		this.EvolutionCalc(args.Result, args.Money, args.Price, args.AddOnCharge);
	}
	/// <summary>
	/// 合成試算結果
	/// </summary>
	void EvolutionCalc(bool result, int money, int price, int addOnCharge)
	{
		if (this.Controller != null)
		{
			this.Controller.FusionCalcResult(result, money, price, addOnCharge);
		}
	}
	#endregion

	#region SetLockPlayerCharacter パケット
	/// <summary>
	/// ロック設定イベントハンドラー
	/// </summary>
	private void HandleLockClickEvent(CharaInfo obj)
	{
		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}

	/// <summary>
	/// SetLockPlayerCharacterReq パケットのレスポンス
	/// </summary>
	private void HandleLockResponse(LobbyPacket.SetLockPlayerCharacterResArgs args)
	{
		// 所有キャラクター情報を取得する
		LobbyPacket.SendPlayerCharacterAll(this.Response);
	}
	#endregion

	#region PlayerCharacter パケット
	private void HandlePlayerCharacter(object sender, XUI.Evolution.PlayerCharacterEventArgs e)
	{
		LobbyPacket.SendPlayerCharacter(e.UUID, this.Response);
		// 通信中の表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// PlayerCharacterReq パケットのレスポンス
	/// </summary>
	void Response(LobbyPacket.PlayerCharacterResArgs args)
	{
		// 通信中を閉じる
		GUISystemMessage.Close();
		this.SetupPlayerCharacterInfo(args.CharaInfo, args.SlotBonusHitPoint, args.SlotBonusAttack, args.SlotBonusDefense, args.SlotBonusExtra, args.SlotList);
	}
	/// <summary>
	/// プレイヤーキャラクター情報を設定
	/// </summary>
	void SetupPlayerCharacterInfo(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList)
	{
		if(this.Controller != null)
		{
			this.Controller.SetupPlayerCharacterInfo(info, slotHitPoint, slotAttack, slotDefense, slotExtra, slotList);
		}
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
			this.AddEvent(this.StatusInfo);
			this.AddEvent(this.CharaList);
		}

		[SerializeField]
		StatusInfoEvent _statusInfo = new StatusInfoEvent();
		public StatusInfoEvent StatusInfo { get { return _statusInfo; } }
		[System.Serializable]
		public class StatusInfoEvent : IDebugParamEvent
		{
			public event System.Action<int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int haveMoney = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.haveMoney);
				}
			}
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
		d.StatusInfo.Execute += (haveMoney) => { this.SetupStatusInfo(haveMoney); };
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
