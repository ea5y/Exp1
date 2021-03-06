/// <summary>
/// キャラ選択リスト
/// 
/// 2016/01/14
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUISelectCharaList : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// 選択キャラリストビューアタッチ
	/// </summary>
	[SerializeField]
	XUI.CharaList.SelectCharaListView _selectListViewAttach = null;
	XUI.CharaList.SelectCharaListView SelectListViewAttach { get { return _selectListViewAttach; } }

	/// <summary>
	/// キャラアイテムページ付スクロールビュー
	/// </summary>
	[SerializeField]
	private XUI.CharaList.GUIItemScrollView _itemScrollView = null;
	public XUI.CharaList.GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }

	/// <summary>
	/// モデル
	/// </summary>
	XUI.CharaList.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	XUI.CharaList.IView View { get; set; }

	/// <summary>
	/// 選択キャラリストビュー
	/// </summary>
	XUI.CharaList.ISelectCharaListView SelectListView { get; set; }

	/// <summary>
	/// 選択キャラリストコントローラ
	/// </summary>
	XUI.CharaList.ISelectCharaListController Controller {get; set;}

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
	/// 全選択を外すボタンが押された時の通知用	
	/// </summary>
	public Action OnAllClearClickEvent = () => { };
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
		XUI.CharaList.ISelectCharaListView selectListView = null;
		XUI.CharaList.IView view = null;
		if(this.SelectListViewAttach != null)
		{
			selectListView = this.SelectListViewAttach.GetComponent(typeof(XUI.CharaList.ISelectCharaListView)) as XUI.CharaList.ISelectCharaListView;
			view = this.SelectListViewAttach.GetComponent(typeof(XUI.CharaList.IView)) as XUI.CharaList.IView;
		}
		this.SelectListView = selectListView;
		this.View = view;
		this.SelectListView.OnAllClearClickEvent += OnAllClearClick;

		// コントローラ
		var controller = new XUI.CharaList.SelectCharaListController(this.Model, this.View, this.SelectListView);
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
		this.SelectListView = null;
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
		this.OnAllClearClickEvent = null;
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// アクティブを設定する
	/// </summary>
	/// <param name="isActive"></param>
	public void SetActive(bool isActive)
	{
		this.View.SetActive(isActive);
	}
	#endregion

	#region アイテム総数設定
	/// <summary>
	/// アイテムの総数を設定する
	/// </summary>
	public void SetupCapacity(int capacity)
	{
		if (this.Model == null) { return; }

		this.Model.Capacity = capacity;
		ClearChara();
	}
	#endregion

	#region キャラ情報追加
	/// <summary>
	/// 空いている枠にキャラを追加する
	/// </summary>
	/// <returns>
	/// 追加 = true, false = 追加失敗
	/// </returns>
	public bool AddChara(CharaInfo charaInfo)
	{
		if (this.Controller == null || this.Model == null || charaInfo == null) { return false; }

		// 空枠を検索
		int emptyIndexInTotal = -1;
		if(this.Controller.TryGetEmptyIndexInTotal(out emptyIndexInTotal))
		{
			// 追加
			if(this.Model.CharaInfoList.Contains(charaInfo))
			{
				// すでに追加しているキャラ情報なら追加しない
				return false;
			}
			// データ追加
			this.Model.AddCharaInfo(charaInfo);
			return true;
		}
		else
		{
			// 空き枠がない
			return false;
		}
	}
	#endregion

	#region キャラ情報削除
	/// <summary>
	/// キャラアイコンを外し空アイテム状態にする
	/// </summary>
	/// <returns>
	/// true = 追加 false = 追加失敗
	/// </returns>
	public bool RemoveChara(CharaInfo charaInfo)
	{
		if (this.Model == null || charaInfo == null) { return false; }

		// 削除するキャラ情報を検索
		CharaInfo removeInfo = null;
		foreach(var info in this.Model.CharaInfoList)
		{
			if (info == null) continue;
			if(charaInfo.UUID == info.UUID)
			{
				removeInfo = info;
				break;
			}
		}

		// 削除
		return this.Model.RemoveCharaInfo(removeInfo);
	}
	#endregion

	#region キャラ情報クリア
	/// <summary>
	/// キャラ情報クリアする
	/// </summary>
	/// <returns>
	/// true = クリアされた, false = クリアしなかった
	/// </returns>
	public bool ClearChara()
	{
		if (this.Model == null) { return false; }

		// キャラ情報がセットされているか検索
		bool isExecute = false;
		foreach(var info in this.Model.CharaInfoList)
		{
			if(info != null)
			{
				// ひとつでもキャラ情報が存在するならクリア処理を行う
				isExecute = true;
				break;
			}
		}
		
		if(isExecute)
		{
			this.Model.ClearCharaInfo();
		}

		return isExecute;
	}
	#endregion

	#region キャラ情報取得
	/// <summary>
	/// 現ページ内のキャラアイテムリストを返す
	/// </summary>
	public List<GUICharaItem> GetNowPageItemList()
	{
		if(this.Model == null)
		{
			return new List<GUICharaItem>();
		}
		return this.Model.GetNowPageItemList();
	}

	/// <summary>
	/// 追加されているキャラ情報リストを取得する
	/// </summary>
	public List<CharaInfo> GetCharaInfoList()
	{
		if (this.Model == null)
		{
			return new List<CharaInfo>();
		}

		List<CharaInfo> infoList = new List<CharaInfo>();
		foreach(var info in this.Model.CharaInfoList)
		{
			if (info == null) { continue; }
			infoList.Add(info);
		}

		return infoList;
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
	/// <param name="sender"></param>
	/// <param name="e"></param>
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

	#region スクロール
	/// <summary>
	/// スクロール更新
	/// </summary>
	public void UpdateScroll()
	{
		if (this.Controller == null) { return; }
		this.Controller.UpdateScroll();
	}

	/// <summary>
	/// テーブル整形
	/// </summary>
	public void Reposition()
	{
		if (this.Controller == null) { return; }
		this.Controller.Reposition();

	}
	#endregion

	#region 全選択を外すボタン
	/// <summary>
	/// 全選択を外すボタンが押された時に呼び出される
	/// </summary>
	private void OnAllClearClick(object sender, EventArgs e)
	{
		// キャラ情報クリア
		ClearChara();

		// 通知
		this.OnAllClearClickEvent();
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
			AddEvent(this.SetCapacity);
			AddEvent(this.AddChara);
			AddEvent(this.TouchRemoveChara);
		}

		[SerializeField]
		SetCapacityEvent _setCapacity = new SetCapacityEvent();
		public SetCapacityEvent SetCapacity { get { return _setCapacity; } set { _setCapacity = value; } }
		[System.Serializable]
		public class SetCapacityEvent : IDebugParamEvent
		{
			public event System.Action<int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int capacity = 1;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					Execute(this.capacity);
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
				if(this.execute)
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
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteActive += () => { SetActive(true); };
		d.ExecuteClose += () => { SetActive(false); };

		d.SetCapacity.Execute += (capacity) =>
		{
			SetupCapacity(capacity);
		};

		d.AddChara.Execute += (charaInfo) =>
		{
			d.ReadMasterData();
			AddChara(charaInfo);
		};

		d.TouchRemoveChara.Execute += (isTouchRemove) =>
		{
			if(isTouchRemove)
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
