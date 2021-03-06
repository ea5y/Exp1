/// <summary>
/// キャラスロットリスト
/// 
/// 2016/05/24
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

public class GUICharaSlotList : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// キャラリストビューアタッチ
	/// </summary>
	[SerializeField]
	private XUI.CharaList.CharaListView _charaListViewAttach = null;
	private XUI.CharaList.CharaListView CharaListViewAttach { get { return _charaListViewAttach; } }

	/// <summary>
	/// キャラアイテムページ付スクロールビュー
	/// </summary>
	[SerializeField]
	private XUI.CharaList.GUIItemScrollView _itemScrollView = null;
	public XUI.CharaList.GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }

	/// <summary>
	/// モデル
	/// </summary>
	private XUI.CharaList.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	private XUI.CharaList.IView View { get; set; }

	/// <summary>
	/// スロットキャラリストコントローラ
	/// </summary>
	private XUI.CharaList.ICharaSlotListController Controller { get; set; }

	/// <summary>
	/// 登録されているアイテムが押された時の通知用
	/// </summary>
	public Action<GUICharaItem> OnItemClickEvent = (item) => { };

	/// <summary>
	/// 登録されているアイテムが長押しされた時の通知用
	/// </summary>
	public Action<GUICharaItem> OnItemLongPressEvent = (item) => { };

	/// <summary>
	/// 登録されているアイテムに変更があった時の通知用
	/// </summary>
	public Action<GUICharaItem> OnItemChangeEvent = (item) => { };

	/// <summary>
	/// 全てのアイテムが更新された時のイベント通知
	/// </summary>
	public event Action OnUpdateItemsEvent = () => { };

	/// <summary>
	/// アイテムの総数
	/// </summary>
	public int Capacity { get { return (this.Controller != null) ? this.Controller.Capacity : 0; } }

	/// <summary>
	/// スロット解放数
	/// </summary>
	public int SlotCount { get { return (this.Controller != null) ? this.Controller.SlotCount : 0; } }
	#endregion

	#region 初期化
	void Awake()
	{
		Construct();
	}

	private void Construct()
	{
		MemberInit();

		// モデル生成
		this.Model = new XUI.CharaList.Model(this.ItemScrollView);

		// ビュー生成
		XUI.CharaList.IView view = null;
		if (this.CharaListViewAttach != null)
		{
			view = this.CharaListViewAttach.GetComponent(typeof(XUI.CharaList.IView)) as XUI.CharaList.IView;
		}
		this.View = view;

		// コントローラ
		var controller = new XUI.CharaList.CharaSlotListController(this.Model, this.View);
		this.Controller = controller;
		this.Controller.OnItemClickEvent += OnItemClick;
		this.Controller.OnItemLongPressEvent += OnItemLongPress;
		this.Controller.OnItemChangeEvent += OnItemChange;
		this.Controller.OnUpdateItemsEvent += OnUpdateItems;
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
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

		this.OnItemClickEvent = null;
		this.OnItemLongPressEvent = null;
		this.OnItemChangeEvent = null;
		this.OnUpdateItemsEvent = null;
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// アクティブを設定する
	/// </summary>
	public void SetActive(bool isActive)
	{
		this.View.SetActive(isActive);
	}
	#endregion

	#region アイテム総数設定
	/// <summary>
	/// アイテム総数を設定
	/// </summary>
	/// <param name="capacity"> アイテム総数 </param>
	/// <param name="slotCount"> スロット解放数 </param>
	public void SetupCapacity(int capacity, int slotCount)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupCapacity(capacity, slotCount);
	}
	#endregion

	#region キャラアイテム設定
	/// <summary>
	/// スロットキャラアイテム設定
	/// </summary>
	/// <param name="charaInfoList"> スロットに刺さっているキャラ情報リスト </param>
	public void SetupItems(List<CharaInfo> charaInfoList)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetupItems(charaInfoList);
	}
	#endregion

	#region キャラ追加
	/// <summary>
	/// スロット空枠にキャラを追加する
	/// </summary>
	public bool AddChara(CharaInfo charaInfo)
	{
		if (this.Controller == null) { return false; }
		return this.Controller.AddChara(charaInfo);
	}
	#endregion

	#region キャラ削除
	/// <summary>
	/// スロットに刺さっているキャラを外す
	/// </summary>
	public bool RemoveChara(CharaInfo charaInfo)
	{
		if (this.Controller == null) { return false; }
		return this.Controller.RemoveChara(charaInfo);
	}
	#endregion

	#region クリア
	/// <summary>
	/// スロットキャラ情報のみクリアする
	/// </summary>
	public bool ClearChara()
	{
		if (this.Controller == null) { return false; }
		return this.Controller.ClearChara();
	}

	/// <summary>
	/// スロット情報クリア
	/// </summary>
	public void ClearSlot()
	{
		if (this.Controller == null) { return; }
		this.Controller.ClearSlot();
	}
	#endregion

	#region キャラ情報取得
	/// <summary>
	/// 現ページ内のキャラアイテムリストを返す
	/// </summary>
	public List<GUICharaItem> GetNowPageItemList()
	{
		if(this.Controller == null)
		{
			return new List<GUICharaItem>();
		}
		return this.Controller.GetNowPageItemList();
	}

	/// <summary>
	/// スロットに刺さっているキャラ情報リストを取得する
	/// </summary>
	public List<CharaInfo> GetCharaInfoList()
	{
		if(this.Controller == null)
		{
			return new List<CharaInfo>();
		}
		return this.Controller.GetCharaInfoList();
	}
	#endregion

	#region アイテムボタン有効設定
	/// <summary>
	/// ページ内の全アイテムのボタン有効設定
	/// </summary>
	public void SetItemsButtonEnable(bool isEnable)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetItemsButtonEnable(isEnable);
	}
	#endregion

	#region テーブル整形
	/// <summary>
	/// テーブル整形
	/// </summary>
	public void Reposition()
	{
		if (this.Controller == null) { return; }
		this.Controller.Reposition();
	}
	#endregion

	#region アイテムイベント
	/// <summary>
	/// アイテムが押された時に呼び出される
	/// </summary>
	private void OnItemClick(object sender, XUI.CharaList.ItemClickEventArgs e)
	{
		// 通知
		this.OnItemClickEvent(e.Item);
	}

	/// <summary>
	/// アイテムが長押しされた時に呼び出される
	/// </summary>
	private void OnItemLongPress(object sender, XUI.CharaList.ItemClickEventArgs e)
	{
		// 通知
		this.OnItemLongPressEvent(e.Item);
	}

	/// <summary>
	/// アイテムに変更があった時に呼び出される
	/// </summary>
	private void OnItemChange(object sender, XUI.CharaList.ItemChangeEventArgs e)
	{
		// 通知
		this.OnItemChangeEvent(e.Item);
	}

	/// <summary>
	/// 全てのアイテムが更新された時に呼び出される
	/// </summary>
	private void OnUpdateItems(object sender, EventArgs e)
	{
		// 通知
		this.OnUpdateItemsEvent();
	}
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
			this.AddEvent(this.AddChara);
			this.AddEvent(this.TouchRemoveChara);
			this.AddEvent(this.ClearChara);
			this.AddEvent(this.ClearSlot);
		}

		[SerializeField]
		CharaListEvent _charaList = new CharaListEvent();
		public CharaListEvent CharaList { get { return _charaList; } }
		[System.Serializable]
		public class CharaListEvent : IDebugParamEvent
		{
			public event System.Action<int, int> ExecuteCapacity = delegate { };
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

					var slotCount = UnityEngine.Random.Range(0, this.capacity + 1);
					var count = UnityEngine.Random.Range(0, slotCount + 1);
					this.list.Clear();
					for (int i = 0; i < count; i++)
					{
						var info = new CharaInfo();
						var uuid = (ulong)(i + 1);
						info.DebugRandomSetup();
						info.DebugSetUUID(uuid);
						this.list.Add(info);
					}
					this.ExecuteCapacity(this.capacity, slotCount);
					this.ExecuteList(new List<CharaInfo>(this.list.ToArray()));
				}
				if (this.executeList)
				{
					this.executeList = false;
					this.ExecuteList(new List<CharaInfo>(this.list.ToArray()));
				}
			}
		}

		[SerializeField]
		AddCharaEvent _addChara = new AddCharaEvent();
		public AddCharaEvent AddChara { get { return _addChara; } set { _addChara = value; } }
		[System.Serializable]
		public class AddCharaEvent : IDebugParamEvent
		{
			public event System.Action<CharaInfo> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			AvatarType avatarType = AvatarType.Begin;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					var info = new CharaInfo();
					info.DebugRandomSetup();
					info.DebugSetAvatarType((int)avatarType);
					Execute(info);

				}
			}
		}

		[SerializeField]
		TouchRemoveCharaEvent _touchRemoveChara = new TouchRemoveCharaEvent();
		public TouchRemoveCharaEvent TouchRemoveChara { get { return _touchRemoveChara; } set { _touchRemoveChara = value; } }
		[System.Serializable]
		public class TouchRemoveCharaEvent : IDebugParamEvent
		{
			public event System.Action<bool> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			bool isTouchRemove = false;
			public Func<CharaInfo, bool> RemoveFunc = (info) => { return false; };

			public void Update()
			{
				if (this.execute)
				{
					execute = false;
					Execute(isTouchRemove);
				}
			}

			public void OnItemClick(GUICharaItem item)
			{
				RemoveFunc(item.GetCharaInfo());
			}
		}

		[SerializeField]
		ClearCharaEvent _clearChara = new ClearCharaEvent();
		public ClearCharaEvent ClearChara { get { return _clearChara; } set { _clearChara = value; } }
		[System.Serializable]
		public class ClearCharaEvent : IDebugParamEvent
		{
			public event System.Action Execute = delegate { };
			[SerializeField]
			bool execute = false;

			public void Update()
			{
				if (this.execute)
				{
					Execute();
				}
			}
		}

		[SerializeField]
		ClearSlotEvent _clearSlot = new ClearSlotEvent();
		public ClearSlotEvent ClearSlot { get { return _clearSlot; } set { _clearSlot = value; } }
		[System.Serializable]
		public class ClearSlotEvent : IDebugParamEvent
		{
			public event System.Action Execute = delegate { };
			[SerializeField]
			bool execute = false;

			public void Update()
			{
				if (this.execute)
				{
					Execute();
				}
			}
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += () => { SetActive(false); };
		d.ExecuteActive += () =>
		{
			d.ReadMasterData();
			SetActive(true);
		};

		d.CharaList.ExecuteCapacity += (capacity, slotCount) => { this.SetupCapacity(capacity, slotCount); };
		d.CharaList.ExecuteList += (list) => { this.SetupItems(list); };
		d.AddChara.Execute += (charaInfo) => { this.AddChara(charaInfo); };
		d.TouchRemoveChara.Execute += (isTouchRemove) =>
		{
			if (isTouchRemove)
			{
				OnItemClickEvent = d.TouchRemoveChara.OnItemClick;
				d.TouchRemoveChara.RemoveFunc = RemoveChara;
			}
			else
			{
				OnItemClickEvent = (info) => { };
				d.TouchRemoveChara.RemoveFunc = (info) => { return false; };
			}
		};
		d.ClearChara.Execute += () => { this.ClearChara(); };
		d.ClearSlot.Execute += () => { this.ClearSlot(); };
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
