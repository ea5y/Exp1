/// <summary>
/// アイテムBOX
/// 
/// 2016/03/30
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;
using Scm.Common;
using Scm.Common.Packet;
using XUI.ItemBox;

public class GUIItemBox : Singleton<GUIItemBox>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	private ItemBoxView _viewAttach = null;
	private ItemBoxView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// アイテムページリスト
	/// </summary>
	[SerializeField]
	private GUIItemPageList _itemPageList = null;
	private GUIItemPageList ItemPageList { get { return _itemPageList; } }

	/// <summary>
	/// アイテム売却リスト
	/// </summary>
	[SerializeField]
	private GUISelectItemList _sellItemList = null;
	private GUISelectItemList SellItemList { get { return _sellItemList; } }

	/// <summary>
	/// ベースアイテムのリソース
	/// </summary>
	[SerializeField]
	private GameObject _baseItemResource = null;
	private GameObject BaseItemResource { get { return _baseItemResource; } }

	/// <summary>
	/// インスタンス化したベースアイテムをアタッチする場所
	/// </summary>
	[SerializeField]
	private GameObject _attachBaseItem = null;
	private GameObject AttachBaseItem { get { return _attachBaseItem; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;
	private bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// まとめて売却できるアイテム最大数
	/// </summary>
	[SerializeField]
	private int _multiSellItemMax = 100;
	public int MultiSellItemMax { get { return _multiSellItemMax; } }

	/// <summary>
	/// 所持金表示フォーマット
	/// </summary>
	[SerializeField]
	private string _haveMoneyFormat = "{0:#,0}";
	private string HaveMoneyFormat { get { return _haveMoneyFormat; } }

	/// <summary>
	/// アイテム売却額表示フォーマット
	/// </summary>
	[SerializeField]
	private string _soldPriceFormat = "{0:#,0}";
	private string SoldPriceFormat { get { return _soldPriceFormat; } }

	/// <summary>
	/// アイテム総売却額表示フォーマット
	/// </summary>
	[SerializeField]
	private string _totalSoldPriceFormat = "{0:#,0}";
	private string TotalSoldPriceFormat { get { return _totalSoldPriceFormat; } }

	// プレイヤーステータス情報
	private PlayerStatusInfo PlayerStatusInfo { get { return NetworkController.ServerValue != null ? NetworkController.ServerValue.PlayerStatusInfo : new PlayerStatusInfo(); } }

	/// <summary>
	/// ベースアイテム
	/// </summary>
	private GUIItem BaseItem { get; set; }

	/// <summary>
	/// コントローラ
	/// </summary>
	private IController Controller { get; set; }
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
		this.CreateBaseItem();
	}
	void Start()
	{
		this.Constrcut();
		// 売却リスト設定
		if(this.SellItemList != null)
		{
			this.SellItemList.SetupCapacity(this.MultiSellItemMax);
		}
		// 初期化アクティブ設定
		this.SetActive(this.IsStartActive, true);
		if (this.IsStartActive)
		{
			this.Setup();
		}
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.BaseItem = null;
		this.Controller = null;
	}
	/// <summary>
	/// ベースアイテム生成
	/// </summary>
	private void CreateBaseItem()
	{
		// 子を消す
		this.AttachBaseItem.DestroyChild();

		GUIItem item = GUIItem.Create(this.BaseItemResource, this.AttachBaseItem.transform, 0);
		if(item == null)
		{
			Debug.LogWarning("GameItem is null!!");
		}
		this.BaseItem = item;
	}
	private void Constrcut()
	{
		var model = new Model();
		model.HaveMoneyFormat = this.HaveMoneyFormat;
		model.SoldPriceFormat = this.SoldPriceFormat;
		model.TotalSoldPriceFormat = this.TotalSoldPriceFormat;

		// ビュー生成
		IView view = null;
		if(this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラ生成
		var controller = new Controller(model, view, this.ItemPageList, this.SellItemList, this.BaseItem);
		this.Controller = controller;
		this.Controller.OnSellMulti += this.HandleSellMulti;
		this.Controller.OnItemLock += this.HandleItemLock;
	}

	/// <summary>
	/// 破棄
	/// </summary>
	void OnDestroy()
	{
		if(this.Controller != null)
		{
			this.Controller.Dispose();
		}
	}
	#endregion

	#region 有効無効
	void OnEnable()
	{
		GUIItemSellSimple.AddSellItemEvent(this.HandleSellItemEvent);
		GUIItemSellSimple.AddSellItemResponseEvent(this.HandleSellItemResponseEvent);
	}
	void OnDisable()
	{
		GUIItemSellSimple.RemoveSellItemEvent(this.HandleSellItemEvent);
		GUIItemSellSimple.RemoveSellItemResponseEvent(this.HandleSellItemResponseEvent);
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

	#region 各情報更新
	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup()
	{
		if(this.Controller != null)
		{
			this.Controller.Setup();
		}

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバからアイテムBOX情報を取得
		LobbyPacket.SendPlayerItemBox(this.Response);
	}
	/// <summary>
	/// 再初期化
	/// </summary>
	private void ReSetup()
	{
		if (this.Controller != null)
		{
			this.Controller.ReSetup();
		}

		// ステータス情報を更新する
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// サーバからアイテムBOX情報を取得
		LobbyPacket.SendPlayerItemBox(this.Response);
	}
	/// <summary>
	/// ステータス情報を更新する
	/// </summary>
	/// <param name="haveMoney"></param>
	private void SetupStatusInfo(int haveMoney)
	{
		if(this.Controller != null)
		{
			this.Controller.SetupStatusInfo(haveMoney);
		}
	}
	#endregion

	#region 単体売却イベント
	/// <summary>
	/// アイテム単体売却時のイベントハンドラー
	/// </summary>
	private void HandleSellItemEvent(){}

	/// <summary>
	/// アイテム単位売却パケット受信時のイベントハンドラー
	/// </summary>
	private void HandleSellItemResponseEvent()
	{
		this.ReSetup();
	}
	#endregion

	#region 通信系
	#region PlayerItemBox パケット
	/// <summary>
	/// PlayerItemBoxReq パケットのレスポンス
	/// </summary>
	private void Response(LobbyPacket.PlayerItemBoxResArgs args)
	{
		this.SetupCapacity(args.Capacity, args.Count);

		// 所有アイテム情報を取得する
		LobbyPacket.SendPlayerItemAll(this.Response);
	}
	/// <summary>
	/// 総数を設定する
	/// </summary>
	private void SetupCapacity(int capacity, int itemCount)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupCapacity(capacity, itemCount);
	}
	#endregion

	#region PlayerItemAll パケット
	/// <summary>
	/// PlayerItemAllReq パケットのレスポンス
	/// </summary>
	private void Response(LobbyPacket.PlayerItemAllResArgs args)
	{
		// 通信中メッセージを閉じる
		GUISystemMessage.Close();

		this.SetupItem(args.List);
	}
	/// <summary>
	/// 個々のアイテムの設定をする
	/// </summary>
	private void SetupItem(List<ItemInfo> list)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupItemInfoList(list);
	}
	#endregion

	#region SellPlayerMultiItem パケット
	/// <summary>
	/// まとめて売却イベントハンドラー
	/// </summary>
	private void HandleSellMulti(object sender, SellMultiEventArgs e)
	{
		LobbyPacket.SendSellMultiPlayerItem(e.IndexList.ToArray(), this.Response);

		// 通信中表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// SellPlayerMultiItemReq パケットのレスポンス
	/// </summary>
	private void Response(LobbyPacket.SellMultiPlayerItemResArgs args)
	{
		// アイテムBOX情報を取得し直す
		LobbyPacket.SendPlayerItemBox(this.Response);

		// ステータス情報を更新
		var info = this.PlayerStatusInfo;
		this.SetupStatusInfo(info.GameMoney);

		// まとめて売却結果
		this.SellMultiResult(args.Result, args.IndexList, args.Money, args.SoldPrice);
	}
	/// <summary>
	/// まとめて売却結果
	/// </summary>
	private void SellMultiResult(bool result, List<int> indexList, int money, int soldPrice)
	{
		if(!result)
		{
			// 売却失敗時は画面を閉じる
			GUIController.Clear();

			// 警告ログ
			string msg = string.Format("SellMultiPlayerItemResponse Result={0} Index={1} money={2} soldPrice={3}",
				result, indexList.ToArray().ToStringArray(), money, soldPrice);
			Debug.LogWarning(msg);
			BugReportController.SaveLogFileWithOutStackTrace(msg);
			return;
		}

		if(this.Controller != null)
		{
			// 結果処理
			this.Controller.SellMultiResult(result, indexList, money, soldPrice);
		}
	}
	#endregion

	#region SetLockPlayerItem パケット
	/// <summary>
	/// アイテムロックイベントハンドラー
	/// </summary>
	private void HandleItemLock(object sender, ItemLockEventArgs e)
	{
		// ロックパケット送信
		LobbyPacket.SendSetLockPlayerItem(e.ItemMasterId, e.IsLock, this.Response);

		// 「通信中」表示
		GUISystemMessage.SetModeConnect(MasterData.GetText(TextType.TX305_Network_Communication));
	}
	/// <summary>
	/// SetLockPlayerItemReq パケットのレスポンス
	/// </summary>
	private void Response(LobbyPacket.SetLockPlayerItemResArgs args)
	{
		// 所有アイテム情報を取得し直す
		LobbyPacket.SendPlayerItemAll(this.Response);
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
			this.AddEvent(this.ItemList);
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
		ItemListEvent _itemList = new ItemListEvent();
		public ItemListEvent ItemList { get { return _itemList; } }
		[System.Serializable]
		public class ItemListEvent : IDebugParamEvent
		{
			public event System.Action<int, int> ExecuteCapacity = delegate { };
			public event System.Action<List<ItemInfo>> ExecuteList = delegate { };
			[SerializeField]
			bool executeDummy = false;
			[SerializeField]
			int capacity = 0;
			[SerializeField]
			int itemCount = 0;

			[SerializeField]
			bool executeList = false;
			[SerializeField]
			List<ItemInfo> list = new List<ItemInfo>();

			public void Update()
			{
				if(this.executeDummy)
				{
					this.executeDummy = false;

					var count = this.itemCount;
					this.list.Clear();
					for(int i = 0; i < count; ++i)
					{
						var info = new ItemInfo();
						var index = i + 1;
						info.DebugRandomSetup();
						info.DebugSetIndex(index);
						this.list.Add(info);
					}
					this.ExecuteCapacity(this.capacity, this.itemCount);
					this.ExecuteList(new List<ItemInfo>(this.list.ToArray()));
				}
				if(this.executeList)
				{
					this.executeList = false;
					this.ExecuteList(new List<ItemInfo>(this.list.ToArray()));

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
		d.ItemList.ExecuteCapacity += (capacity, itemCount) => { this.SetupCapacity(capacity, itemCount); };
		d.ItemList.ExecuteList += (list) => { this.SetupItem(list); };
	}
	bool _isDebugInit = false;
	void DebugUpdate()
	{
		if(!this._isDebugInit)
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
