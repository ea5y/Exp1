/// <summary>
/// キャラアイテム
/// 
/// 2016/01/08
/// </summary>
using UnityEngine;
using System;
using System.Collections;

public class GUICharaItem : MonoBehaviour
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	XUI.CharaItem.CharaItemView _viewAttach = null;
	XUI.CharaItem.CharaItemView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// ドラッグ用
	/// </summary>
	[SerializeField]
	private UIDragScrollView _dragScrollView = null;
	private UIDragScrollView DragScrollView { get { return _dragScrollView; } }

	/// <summary>
	/// パラメータ表示フォーマット
	/// </summary>
	[SerializeField]
	string _parameterFormat = "Lv.{0}";
	string ParameterFormat { get { return _parameterFormat; } }

	/// <summary>
	/// レベル最大時フォーマット
	/// </summary>
	[SerializeField]
	string _lvMaxFormat = "Lv.Max";
	string LvMaxFormat { get { return _lvMaxFormat; } }

	/// <summary>
	/// レベル最大時カラー
	/// </summary>
	[SerializeField]
	Color _lvMaxColor = Color.red;
	Color LvMaxColor { get { return _lvMaxColor; } }

	/// <summary>
	/// ランク色(通常時)
	/// </summary>
	[SerializeField]
	XUI.CharaItem.RankColor _rankColor = null;
	XUI.CharaItem.RankColor RankColor { get { return _rankColor; } }

	/// <summary>
	/// 高ランク色
	/// </summary>
	[SerializeField]
	XUI.CharaItem.RankColor _heightRankColor = null;
	XUI.CharaItem.RankColor HeightRankColor { get { return _heightRankColor; } }

	/// <summary>
	/// 材料ランク色(通常時)
	/// </summary>
	[SerializeField]
	XUI.CharaItem.RankColor _matRankColor = null;
	XUI.CharaItem.RankColor MatRankColor { get { return _matRankColor; } }

	/// <summary>
	/// 材料高ランク色
	/// </summary>
	[SerializeField]
	XUI.CharaItem.RankColor _matHeightRankColor = null;
	XUI.CharaItem.RankColor MatHeightRankColor { get { return _matHeightRankColor; } }
	
	/// <summary>
	/// モデル
	/// </summary>
	XUI.CharaItem.IModel Model { get; set; }

	/// <summary>
	/// ビュー
	/// </summary>
	XUI.CharaItem.IView View { get; set; }

	/// <summary>
	/// コントローラ
	/// </summary>
	XUI.CharaItem.IController Controller { get; set; }

	// キャラアイコン
	private CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

	/// <summary>
	/// アイテムが押された時の通知用
	/// </summary>
	public event Action<GUICharaItem> OnItemClickEvent = (item) => { };

	/// <summary>
	/// アイテムが長押しされた時の通知用
	/// </summary>
	public event Action<GUICharaItem> OnItemLongPressEvent = (item) => { };

	/// <summary>
	/// アイテムに変化があった時の通知用
	/// アイテム変化 = キャラ情報のユニークIDに変化あった時
	/// </summary>
	public event Action<GUICharaItem> OnItemChangeEvent = (item) => { };
	#endregion

	#region 初期化
	/// <summary>
	/// 生成
	/// </summary>
	public static GUICharaItem Create(GameObject prefab, Transform parent, int itemIndex)
	{
		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		if (go == null)
			return null;

		//// 親子付け
		go.SetParentWithLayer(parent.gameObject, false);
		// 名前
		go.name = string.Format("{0}{1}", prefab.name, itemIndex);
		// 可視化
		if (!go.activeSelf)
		{
			go.SetActive(true);
		}

		// コンポーネント取得
		var item = go.GetComponentInChildren(typeof(GUICharaItem)) as GUICharaItem;
		if (item == null)
			return null;

		// ドラッグによるスクロール機能をOFFに設定
		if (item.DragScrollView != null)
		{
			item.DragScrollView.enabled = false;
		}

		return item;
	}

	void Awake()
	{
		Construct();
	}

	private void Construct()
	{
		this.MemberInit();

		// モデル生成
		var model = new XUI.CharaItem.Model();
		this.Model = model;
		this.Model.OnSetCharaInfoChange += OnItemChange;
		this.Model.ParameterFormat = this.ParameterFormat;
		this.Model.LvMaxFormat = this.LvMaxFormat;
		this.Model.LvMaxColor = this.LvMaxColor;
		this.Model.RankColor = this.RankColor;
		this.Model.HeightRankColor = this.HeightRankColor;
		this.Model.MaterialRankColor = this.MatRankColor;
		this.Model.MaterialHeightRankColor = this.MatHeightRankColor;

		// ビュー生成
		XUI.CharaItem.IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.CharaItem.IView)) as XUI.CharaItem.IView;
		}
		this.View = view;
		this.View.OnItemClickEvent += OnItemClick;
		// 売り切り版には無いため登録しない
		//this.View.OnItemLongPressEvent += OnItemLongPress;

		// コントローラー生成
		var controller = new XUI.CharaItem.Controller(model, view, this.CharaIcon);
		this.Controller = controller;

		// 初期時は空アイテム
		SetState(XUI.CharaItem.Controller.ItemStateType.Empty);
		// 初期時はキャラ情報に空をセット
		SetCharaInfo(null);
		// 初期時はアイテム有効
		SetDisableState(XUI.CharaItem.Controller.DisableType.None);
		// 初期時はボタン有効
		SetButtonEnable(true);
		// 初期時は所有状態設定なし
		SetPossessionState(XUI.CharaItem.Controller.PossessionStateType.None);
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

	#region アイテム状態
	/// <summary>
	/// アイテムの状態をセット
	/// </summary>
	/// <param name="stateType"></param>
	public void SetState(XUI.CharaItem.Controller.ItemStateType stateType)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetItemState(stateType);
	}

	/// <summary>
	/// アイテムの状態をセット
	/// </summary>
	/// <param name="stateType"></param>
	public void SetState(XUI.CharaItem.Controller.ItemStateType stateType, CharaInfo charaInfo)
	{
		if (this.Controller == null) { return; }
		SetCharaInfo(charaInfo);
		this.Controller.SetItemState(stateType);
	}

	/// <summary>
	/// アイテムの状態を取得する
	/// </summary>
	/// <returns></returns>
	public XUI.CharaItem.Controller.ItemStateType GetState()
	{
		return this.Controller.GetItemState();
	}
	#endregion

	#region キャラ情報
	/// <summary>
	/// キャラ情報セット
	/// </summary>
	/// <param name="info"></param>
	public void SetCharaInfo(CharaInfo info)
	{
		this.Model.CharaInfo = info;
	}

	/// <summary>
	/// キャラ情報取得
	/// </summary>
	/// <returns></returns>
	public CharaInfo GetCharaInfo()
	{
		if (this.Model == null) { return null; }

		return this.Model.CharaInfo;
	}
	#endregion

	#region アイテム無効状態
	/// <summary>
	/// アイテムの無効状態をセットする
	/// </summary>
	/// <param name="itemState"></param>
	public void SetDisableState(XUI.CharaItem.Controller.DisableType state)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetDisableState(state);
	}

	/// <summary>
	/// アイテムの無効状態を取得する
	/// </summary>
	public XUI.CharaItem.Controller.DisableType GetDisableState()
	{
		if (this.Controller == null) { return XUI.CharaItem.Controller.DisableType.None; }
		return this.Controller.DisableState;
	}

	/// <summary>
	/// 素材状態をセットする
	/// </summary>
	/// <param name="baitIndex"></param>
	public void SetBaitState(int baitIndex)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetBaitState(baitIndex);
	}
	#endregion

	#region ランク
	/// <summary>
	/// ランク色をセットする
	/// </summary>
	public void SetRankColor(int materialRank)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetRankColor(materialRank);
	}
	#endregion

	#region 材料ランク
	/// <summary>
	/// 材料ランクセット
	/// </summary>
	public void SetMaterialRank(int materialRank)
	{
		if (Model == null) { return; }
		this.Model.MaterialRank = materialRank;
	}

	/// <summary>
	/// 材料ランク取得
	/// </summary>
	public int GetMaterialRank()
	{
		if (this.Model == null) { return 0; }
		return this.Model.MaterialRank;
	}

	/// <summary>
	/// 材料ランク色をセットする
	/// </summary>
	public void SetMaterialRankColor(int rank)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetMaterialRankColor(rank);
	}
	#endregion

	#region 所有状態
	/// <summary>
	/// 所有状態設定
	/// </summary>
	public void SetPossessionState(XUI.CharaItem.Controller.PossessionStateType state)
	{
		if (this.Controller == null) { return; }
		this.Controller.SetPossessionState(state);
	}

	/// <summary>
	/// 所有状態取得
	/// </summary>
	/// <returns></returns>
	public XUI.CharaItem.Controller.PossessionStateType GetPossessionState()
	{
		if (this.Controller == null) { return XUI.CharaItem.Controller.PossessionStateType.None; }
		return this.Controller.GetPossessionState();
	}
	#endregion

	#region ボタン有効
	/// <summary>
	/// ボタン有効設定
	/// </summary>
	/// <param name="isEnable"></param>
	public void SetButtonEnable(bool isEnable)
	{
		if (this.Model == null) { return; }
		this.Model.IsButtonEnable = isEnable;
	}

	/// <summary>
	/// ボタン有効フラグ取得
	/// </summary>
	/// <returns></returns>
	public bool GetButtonEnable()
	{
		if (this.Model == null) { return false; }
		return this.Model.IsButtonEnable;
	}
	#endregion

	#region 選択
	/// <summary>
	/// 選択設定
	/// </summary>
	/// <param name="isSelect"></param>
	public void SetSelect(bool isSelect)
	{
		if (this.Model == null) { return; }
		this.Model.IsSelect = isSelect;
	}

	/// <summary>
	/// 選択フラグ取得
	/// </summary>
	/// <returns></returns>
	public bool GetSelect()
	{
		if (this.Model == null) { return false; }
		return this.Model.IsSelect;
	}
	#endregion

	#region インデックス値
	/// <summary>
	/// アイテムのインデックス値をセット
	/// </summary>
	/// <param name="index"></param>
	public void SetIndex(int index)
	{
		if (this.Model == null) { return; }
		this.Model.Index = index;
	}

	/// <summary>
	/// アイテムのインデックス値を取得
	/// </summary>
	/// <returns></returns>
	public int GetIndex()
	{
		if (this.Model == null) { return -1; }
		return this.Model.Index;
	}
	#endregion

	#region スクロール
	/// <summary>
	/// ドラッグによるスクロールの有効設定
	/// </summary>
	public void SetDragScrollEnable(bool isEnable)
	{
		if (this.DragScrollView == null) { return; }
		this.DragScrollView.enabled = isEnable;
	}
	#endregion

	#region イベント
	/// <summary>
	/// アイテムが押された時に呼び出される
	/// </summary>
	private void OnItemClick(object sender, EventArgs e)
	{
		// イベント通知
		this.OnItemClickEvent(this);
	}

	/// <summary>
	/// アイテムが長押しされた時に呼び出される
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnItemLongPress(object sender, EventArgs e)
	{
		// イベント通知
		this.OnItemLongPressEvent(this);

		// キャラ簡易情報ウィンドウ表示
		CharaSimpleInfoOpen();
	}

	/// <summary>
	/// アイテムに変化があった時に呼び出される
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void OnItemChange(object sender, EventArgs e)
	{
		// イベント通知
		this.OnItemChangeEvent(this);
	}
	#endregion

	#region キャラ簡易情報ウィンドウ表示
	/// <summary>
	/// キャラ簡易情報ウィンドウを表示
	/// </summary>
	private void CharaSimpleInfoOpen()
	{
		// キャラアイコン状態の時のみ表示
		if(GetState() != XUI.CharaItem.Controller.ItemStateType.Icon) { return; }

		// キャラアイテムのビューポート座標取得
		Camera screenCamera = NGUITools.FindCameraForLayer(this.gameObject.layer);
		if (screenCamera == null) { return; }
		var viewportPoint = screenCamera.WorldToViewportPoint(this.transform.position);

		// キャラアイテムが画面中心値より右側の位置に存在するならキャラ簡易情報ウィンドウを左側に表示
		// キャラアイテムが画面中心値より左側の位置に存在するならキャラ簡易情報ウィンドウを右側に表示
		if (viewportPoint.x > 0.5f)
		{
			// 左側に表示
			Vector3 position = GetCharaSimpleInfoPosition(screenCamera, false);
			var single = new GUISingle(() => { GUICharaSimpleInfo.Open(position, this.Model.CharaInfo); }, GUICharaSimpleInfo.Close);
			GUIController.SingleOpen(single);
		}
		else
		{
			// 右側に表示
			Vector3 position = GetCharaSimpleInfoPosition(screenCamera, true);
			var single = new GUISingle(() => { GUICharaSimpleInfo.Open(position, this.Model.CharaInfo); }, GUICharaSimpleInfo.Close);
			GUIController.SingleOpen(single);
		}
	}

	/// <summary>
	/// キャラ簡易情報ウィンドウが画面内に収まるようにした表示位置を取得する
	/// </summary>
	/// <param name="screenCamera"></param>
	/// <param name="isRight"></param>
	/// <returns></returns>
	private Vector2 GetCharaSimpleInfoPosition(Camera screenCamera, bool isRight)
	{
		// キャラアイテムの四隅のワールド座標取得
		UIWidget charaItemWidget = this.gameObject.GetComponent<UIWidget>();
		if (charaItemWidget == null) { return new Vector2(); }
		Vector3[] charaItemCorners = charaItemWidget.worldCorners;

		// キャラ簡易情報ウィンドウの四隅のワールド座標取得
		Vector3[] simpleInfoCorners = GUICharaSimpleInfo.GetWorldCorners();

		// キャラ簡易情報ウィンドウの幅と高さを取得
		float simpleInfoHalfHeight = (Math.Abs(simpleInfoCorners[1].y - simpleInfoCorners[0].y) * 0.5f);
		float simpleInfoHalfWidth = Mathf.Abs(simpleInfoCorners[3].x - simpleInfoCorners[0].x) * 0.5f;

		// キャラアイテムの隣りに表示するために位置を取得
		float y = this.transform.position.y;
		float x = 0;
		if(isRight)
		{
			// キャラアイテムの右側の位置取得
			x = charaItemCorners[3].x + simpleInfoHalfWidth;
		}
		else
		{
			// キャラアイテムの左側の位置取得
			x = charaItemCorners[0].x - simpleInfoHalfWidth;
		}

		//　画面左下、右上のワールド座標をビューポートから取得
		Vector2 min = screenCamera.ViewportToWorldPoint(new Vector2(0, 0));
		Vector2 max = screenCamera.ViewportToWorldPoint(new Vector2(1, 1));
		
		// キャラ簡易情報ウィンドウが画面外にはみ出ていないかチェック(左右)
		if(x + simpleInfoHalfWidth >= max.x)
		{
			// 右側にはみ出ているのでキャラアイテムの左側に表示
			x = charaItemCorners[0].x - simpleInfoHalfWidth;
		}
		else if(x - simpleInfoHalfWidth <= min.x)
		{
			// 左側にはみ出ているのでキャラアイテムの右側に表示
			x = charaItemCorners[3].x + simpleInfoHalfWidth;
		}

		// キャラ簡易情報ウィンドウが画面外にはみ出ていないかチェック(上下)
		if(y + simpleInfoHalfHeight >= max.y)
		{
			// 上側にはみ出ているのではみ出ないように補正
			y = max.y - simpleInfoHalfHeight;
		}
		else if(y - simpleInfoHalfHeight <= min.y)
		{
			// 下側にはみ出ているのではみ出ないように補正
			y = min.y + simpleInfoHalfHeight;
		}

		return new Vector2(x, y);
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
			this.AddEvent(this.State);
			this.AddEvent(this.Disable);
			this.AddEvent(this.Enable);
			this.AddEvent(this.Select);
			this.AddEvent(this.Possession);
			this.AddEvent(this.MaterialRank);
		}

		[SerializeField]
		StateEvent _state = new StateEvent();
		public StateEvent State { get { return _state; } set { _state = value; } }
		[System.Serializable]
		public class StateEvent : IDebugParamEvent
		{
			public event System.Action<XUI.CharaItem.Controller.ItemStateType, CharaInfo> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			XUI.CharaItem.Controller.ItemStateType stateType = XUI.CharaItem.Controller.ItemStateType.Empty;
			[SerializeField]
			CharaInfo charaInfo = null;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					CharaInfo info = null;
					if(stateType != XUI.CharaItem.Controller.ItemStateType.Empty)
					{
						info = charaInfo.Clone();
					}
					this.Execute(stateType, info);
				}
			}
		}

		[SerializeField]
		DisableEvent _disable = new DisableEvent();
		public DisableEvent Disable { get { return _disable; } set { _disable = value; } }
		[System.Serializable]
		public class DisableEvent : IDebugParamEvent
		{
			public event System.Action<XUI.CharaItem.Controller.DisableType, int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			XUI.CharaItem.Controller.DisableType state = XUI.CharaItem.Controller.DisableType.None;
			[SerializeField]
			int selectNumber = 1;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.state, this.selectNumber);
				}
			}
		}

		[SerializeField]
		ButtonEnableEvent _enable = new ButtonEnableEvent();
		public ButtonEnableEvent Enable { get { return _enable; } set { _enable = value; } }
		[System.Serializable]
		public class ButtonEnableEvent : IDebugParamEvent
		{
			public event System.Action<bool> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			bool isEnable = true;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.isEnable);
				}
			}
		}

		[SerializeField]
		SelectEvent _select = new SelectEvent();
		public SelectEvent Select { get { return _select; } set { _select = value; } }
		[System.Serializable]
		public class SelectEvent : IDebugParamEvent
		{
			public event System.Action<bool> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			bool isSelect = true;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.isSelect);
				}
			}
		}

		[SerializeField]
		PossessionEvent _possession = new PossessionEvent();
		public PossessionEvent Possession { get { return _possession; } set { _possession = value; } }
		[System.Serializable]
		public class PossessionEvent : IDebugParamEvent
		{
			public event System.Action<XUI.CharaItem.Controller.PossessionStateType> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			XUI.CharaItem.Controller.PossessionStateType stateType = XUI.CharaItem.Controller.PossessionStateType.None;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.stateType);
				}
			}
		}

		[SerializeField]
		MaterialRankEvent _materialRank = new MaterialRankEvent();
		public MaterialRankEvent MaterialRank { get { return _materialRank; } set { _materialRank = value; } }
		[System.Serializable]
		public class MaterialRankEvent : IDebugParamEvent
		{
			public event System.Action<int> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			int rank = 0;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute(this.rank);
				}
			}
		}
	}

	void DebugInit()
	{
		//MaterialRank
		var d = this.DebugParam;

		d.ExecuteClose += () => { SetActive(false); };
		d.ExecuteActive += () => { SetActive(true); };

		d.State.Execute += (stateType, info) =>
		{
			d.ReadMasterData();
			SetState(stateType, info);
		};

		d.Disable.Execute += (state, selectNumber) =>
		{
			if (state == XUI.CharaItem.Controller.DisableType.Bait)
			{
				SetBaitState(selectNumber);
			}
			else
			{
				SetDisableState(state);
			}
		};

		d.Enable.Execute += (isEnable) =>
		{
			SetButtonEnable(isEnable);
		};

		d.Select.Execute += (isSelect) =>
		{
			SetSelect(isSelect);
		};

		d.Possession.Execute += (stateType) =>
		{
			SetPossessionState(stateType);
		};

		d.MaterialRank.Execute += (rank) =>
		{
			SetMaterialRank(rank);
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
