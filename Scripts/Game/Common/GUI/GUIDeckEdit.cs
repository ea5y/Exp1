/// <summary>
/// デッキ編集
/// 
/// 2014/11/10
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

public class GUIDeckEdit : Singleton<GUIDeckEdit>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のモード
	/// </summary>
	[SerializeField]
	DeckMode _startMode = DeckMode.None;
	DeckMode StartMode { get { return _startMode; } }
	public enum DeckMode
	{
		None,
		Edit,
		CharaSelect,
	}

	/// <summary>
	/// ページの表示フォーマット
	/// </summary>
	[SerializeField]
	string _pageFormat = "{0} / {1}";
	string PageFormat { get { return _pageFormat; } }

	/// <summary>
	/// デッキコストの表示フォーマット
	/// </summary>
	[SerializeField]
	string _deckCostFormat = "{0}";
	string DeckCostFormat { get { return _deckCostFormat; } }

	/// <summary>
	/// デッキキャパシティの表示フォーマット
	/// </summary>
	[SerializeField]
	string _deckCapacityFormat = " / {0}";
	string DeckCapacityFormat { get { return _deckCapacityFormat; } }

	/// <summary>
	/// 保存してあるデッキ情報
	/// </summary>
	[SerializeField]
	DeckInfo _deckInfo;
	DeckInfo DeckInfo { get { return _deckInfo; } set { _deckInfo = value; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		// 基本セット
		public UIPlayTween rootTween;
		public GameObject homeButtonGroup;
		public GameObject closeButtonGroup;

		// デッキ情報
		public UILabel pageLabel;
		public UILabel deckLabel;
		public UILabel deckCostLabel;
		[SerializeField]
		UIPlayTween _costOverTween;
		public UIPlayTween CostOverTween { get { return _costOverTween; } }

		public UILabel deckCapacityLabel;
		public UIButton okButton;

		// アイテム系
		public GUIDeckEditItem itemPrefab;
		public UIGrid itemTable;

		// モードごとによってアクティブにするグループ
		public ModeActiveGroup modeActive;
		[System.Serializable]
		public class ModeActiveGroup
		{
			public UIPlayTween editPlayTween;
			public UIPlayTween charaSelectPlayTween;
		}
	}

	// 現在のモード
	DeckMode Mode { get; set; }
	// ホームボタンを押した時のデリゲート
	System.Action OnHomeFunction { get; set; }
	// 閉じるボタンを押した時のデリゲート
	System.Action OnCloseFunction { get; set; }
	// 選択した時のデリゲート
	System.Action<int, DeckInfo, CharaInfo> OnSelectFunction { get; set; }
	// ロード中
	bool IsLoading { get; set; }

	// デッキコスト
	int DeckTotalCost { get; set; }
	// デッキキャパシティ
	int DeckCapacity { get; set; }
	// 現在選択されているスロットインデックス
	int CurrentSlotIndex { get; set; }
	// アイテムリスト
	List<GUIDeckEditItem> ItemList { get; set; }
    public bool IsSetDeck { get; set; }
    private ulong LeaderUUID = 0;

	// シリアライズされていないメンバーの初期化
	void MemberInit()
	{
		this.Mode = DeckMode.None;
		this.OnHomeFunction = delegate { };
		this.OnCloseFunction = delegate { };
		this.OnSelectFunction = delegate { };

		this.DeckTotalCost = 0;
		this.DeckCapacity = 0;
		this.CurrentSlotIndex = -1;
		this.ItemList = new List<GUIDeckEditItem>();
        this.IsSetDeck = false;
	}

	// キャラアイコン
	CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }
	// スキルアイコン
	SkillIcon SkillIcon { get { return ScmParam.Battle.SkillIcon; } }

	/// <summary>
	/// 現在のモード
	/// </summary>
	public static DeckMode NowMode { get { return (Instance != null ? Instance.Mode : DeckMode.None); } }
	/// <summary>
	/// 現在選択されているスロットインデックス
	/// </summary>
	public static int NowSlotIndex { get { return (Instance != null ? Instance.CurrentSlotIndex : 0); } }

	// OKボタンを押せるかどうか
	bool CanSelectOK
	{
		get
		{
			//// コストオーバー
			//if (this.DeckTotalCost > this.DeckCapacity)
			//	return false;
			// キャラリストが不正
			var list = this.DeckInfo.CharaInfoList;
			if (list == null && list.Count < 1)
				return false;
			// リーダーがセットされていない
			if (list[0].IsDeckSlotEmpty)
				return false;

			return true;
		}
	}
	#endregion

	#region 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// アイテム削除
//		this.DestroyItem();

		// 最初は閉じておく
		Close();
	}
	#endregion

	#region モード設定
	/// <summary>
	/// ウィンドウを閉じる
	/// </summary>
	public static void Close(bool pSave = false)
	{
	    if (Instance != null)
	    {
	        Instance.SetWindowActive(DeckMode.None, false);
	        Instance.IsSetDeck = false;

            if (pSave && null != Instance.ItemList && Instance.ItemList.Count > 0 && Instance.LeaderUUID != Instance.ItemList[0].CharaInfo.UUID)
	        {
	            CommonPacket.SendSetSymbolPlayerCharacterReq(Instance.ItemList[0].CharaInfo.UUID,
	                HandleOnCharaChengeResponse);
	        }

	        Instance.LeaderUUID = 0;
	    }
	}

    static void HandleOnCharaChengeResponse(CommonPacket.SetSymbolPlayerCharacterResArgs args)
    {
    }
	/// <summary>
	/// デッキエディットモードで開く
	/// </summary>
	public static void SetModeEdit(bool isServerRequest, System.Action onHome, System.Action onClose)
	{
		if (Instance != null) Instance._SetMode(DeckMode.Edit, isServerRequest, true, true, onHome, onClose, null);
	}
	/// <summary>
	/// キャラ選択モードで開く
	/// </summary>
	public static void SetModeCharaSelect(bool isServerRequest, bool isUseHome, bool isUseClose, System.Action onHome, System.Action onClose, System.Action<int, DeckInfo, CharaInfo> onSelect)
	{
		if (Instance != null) Instance._SetMode(DeckMode.CharaSelect, isServerRequest, isUseHome, isUseClose, onHome, onClose, onSelect);
	}
	/// <summary>
	/// モード設定(大元)
	/// </summary>
	void _SetMode(DeckMode mode, bool isServerRequest, bool isUseHome, bool isUseClose, System.Action onHome, System.Action onClose, System.Action<int, DeckInfo, CharaInfo> onSelect)
	{
		this.OnHomeFunction = (onHome != null ? onHome : delegate { });
		this.OnCloseFunction = (onClose != null ? onClose : delegate { });
		this.OnSelectFunction = (onSelect != null ? onSelect : delegate { });

		// UI設定
		{
			var t = this.Attach;
			if (t.homeButtonGroup != null)
				t.homeButtonGroup.SetActive(isUseHome);
			if (t.closeButtonGroup != null)
				t.closeButtonGroup.SetActive(isUseClose);
		}

		// ウィンドウアクティブ設定
		this.SetWindowActive(mode, mode != DeckMode.None);

		// モードごとのアクティブ設定
		var m = this.Attach.modeActive;
		switch (mode)
		{
		case DeckMode.None: this.SetModeActive(null); break;
		case DeckMode.Edit: this.SetModeActive(m.editPlayTween); break;
		case DeckMode.CharaSelect: this.SetModeActive(m.charaSelectPlayTween); break;
		}

		// サーバーからデッキの情報を取得する
		if (isServerRequest)
			this._SendDeckInfo();
		else
			this._SetDeck(this.DeckInfo, this.CurrentSlotIndex);
	}
	/// <summary>
	/// ウィンドウアクティブ設定
	/// </summary>
	void SetWindowActive(DeckMode mode, bool isActive)
	{
		this.Mode = mode;

		// アクティブ化
		//if (this.Attach.rootTween != null)
		//	this.Attach.rootTween.Play(isActive);
		//else
			this.gameObject.SetActive(isActive);

		// その他UIの表示設定
		GUILobbyResident.SetActive(!isActive);
		string title = "", help = "";
		switch (mode)
		{
		case DeckMode.None:
			title = "";
			help = "";
			break;
		case DeckMode.Edit:
			title = MasterData.GetText(TextType.TX134_DeckEdit_ScreenTitle);
			help = MasterData.GetText(TextType.TX135_DeckEdit_HelpMessage);
			break;
		case DeckMode.CharaSelect:
			title = string.Empty;
			help = MasterData.GetText(TextType.TX136_DeckCharaSelect_HelpMessage);
			break;
		}
		GUIScreenTitle.Play(isActive, title);
		GUIHelpMessage.Play(isActive, help);
	}
	/// <summary>
	/// モードごとのアクティブ設定
	/// </summary>
	void SetModeActive(UIPlayTween activePlayTween)
	{
		// グループの表示設定
		// NGUI3.7.4 で SetActive のオンオフでやっていたらフレームレート(FPS10制限で出る)によっては
		// 閉じる開くのスケールがこのグループで設定している物が最後まで掛からないバグあり
		// 解決策は SetActive ではなく UIWidget のアルファを使用した
		// 柔軟性を持たせるため UIPlayTween を使用している
		var m = this.Attach.modeActive;
		var list = new List<UIPlayTween>();
		list.Add(m.editPlayTween);
		list.Add(m.charaSelectPlayTween);
		foreach (var t in list)
		{
			if (t == null)
				continue;
			bool active = (activePlayTween == t);
			t.Play(active);
		}
	}
	#endregion

	#region デッキ情報をサーバーから取得する
	/// <summary>
	/// デッキ情報をサーバーから取得する
	/// デッキの総数を取得=>ページ更新=>現在のデッキ取得=>デッキ設定
	/// </summary>
	public static void SendDeckInfo()
	{
		if (Instance != null) Instance._SendDeckInfo();
	}
	/// <summary>
	/// デッキ情報をサーバーから取得する
	/// デッキの総数を取得=>ページ更新=>現在のデッキ取得=>デッキ設定
	/// </summary>
	void _SendDeckInfo()
	{
		// ロード中フラグ
		this.IsLoading = true;
		// デッキ情報初期化
		this._SetDeck(null, -1);
		// デッキの総数を問い合わせる
		CommonPacket.SendCharacterDeckNum();
	}
	#endregion

	#region ページ設定
	/// <summary>
	/// ページ設定
	/// </summary>
	public static void SetPage(int currentDeckID, int num)
	{
		if (Instance != null) Instance._SetPage(currentDeckID, num);
	}
	/// <summary>
	/// ページ設定
	/// </summary>
	void _SetPage(int currentDeckID, int num)
	{
		// UI設定
		var t = this.Attach;
		if (t.pageLabel != null)
			t.pageLabel.text = string.Format(this.PageFormat, currentDeckID, num);
	}
	#endregion

	#region デッキ設定
	/// <summary>
	/// 現在のデッキ情報を設定する
	/// </summary>
	public static void SetDeck(DeckInfo info, int currentSlotIndex)
	{
		if (Instance != null) Instance._SetDeck(info, currentSlotIndex);
	}

	/// <summary>
	/// 現在のデッキ情報を設定する
	/// </summary>
	void _SetDeck(DeckInfo info, int currentSlotIndex)
	{
        //Reget Item in Show Order, Because the grid sort by horizon
        //Bug Find
        //不能将根部的UIPlayTween打开，打开之后grid可能没有初始化GetChildList()无法获取
	    ReGetItemList();
		// ロード中フラグを下げる
		if (info != null)
			this.IsLoading = false;

		this.DeckInfo = (info == null ? new DeckInfo() : info);
		this.DeckCapacity = this.DeckInfo.DeckCapacity;
		this.DeckTotalCost = this.DeckInfo.DeckTotalCost;
		this.CurrentSlotIndex = (this.Mode == DeckMode.CharaSelect ? currentSlotIndex : -1);	// キャラセレ以外は選択枠は出さない
		// 念の為インデックスを設定しなおす
		for (int i = 0, max = this.DeckInfo.CharaInfoList.Count; i < max; i++)
		{
			var t = this.DeckInfo.CharaInfoList[i];
			t.DeckSlotIndex = i;
		}

		// UI設定
		{
			var t = this.Attach;
			if (t.deckLabel != null)
				t.deckLabel.text = this.DeckInfo.DeckName;
			// コストUI更新
			this.CostUIUpdate();
		}

		// アイテムセットアップ
		this.ItemSetup(this.DeckInfo.CharaInfoList, this.CurrentSlotIndex);
        //Select the First One
        if (this.Mode == DeckMode.Edit && info != null && !IsSetDeck)
	    {
            SetSelectItem(this.ItemList[0]);
	        IsSetDeck = true;
	        LeaderUUID = this.ItemList[0].CharaInfo.UUID;
	    }
	}

	/// <summary>
	/// コスト表示UIの更新
	/// </summary>
	void CostUIUpdate()
	{
		// UI設定
		var t = this.Attach;
		if (t.deckCostLabel != null)
			t.deckCostLabel.text = string.Format(this.DeckCostFormat, this.DeckTotalCost);
		if (t.deckCapacityLabel != null)
			t.deckCapacityLabel.text = string.Format(this.DeckCapacityFormat, this.DeckCapacity);

		// キャパシティを超えている場合の演出
		if (this.DeckTotalCost > this.DeckCapacity)
		{
			// キャパシティ超えている
			if (t.CostOverTween != null)
				t.CostOverTween.Play(true);
		}
		else
		{
			// 正常
			if (t.CostOverTween != null)
			{
				t.CostOverTween.SetTweener(
					(tw) =>
					{
						// ループをさせない元の状態(from設定)に戻す
						tw.enabled = false;
						tw.Sample(0f, false);
					});
			}
		}
	}
	#endregion

	#region アイテム操作
	/// <summary>
	/// アイテムセットアップ
	/// </summary>
	void ItemSetup(List<CharaInfo> charaList, int currentSlotIndex)
	{
		// キャラクタスロット設定
		if (charaList != null && this.ItemList != null)
		{
			// キャラリスト > アイテム数ならアイテムの数を増やす
			// アイテム数 > キャラリストなら空のキャラリストを設定する
			int max = Mathf.Max(charaList.Count, this.ItemList.Count);
			for (int i = 0; i < max; i++)
			{
				// キャラ情報取得
				CharaInfo info = null;
				if (i < charaList.Count)
					info = charaList[i];
				else
					info = new CharaInfo();
				if (info == null)
					continue;

				// アイテム取得
				GUIDeckEditItem item = null;
				if (i < this.ItemList.Count)
					item = this.ItemList[i];
				else
					item = this.AddItem(i);
				if (item == null)
					continue;

				// アイテム設定
				item.Setup(info, this.CharaIcon, this.SkillIcon);
				// 選択されているスロットかどうか
				var isSelectActive = (i == currentSlotIndex);
				item.SetSelectSpriteActive(isSelectActive);
			}
		}

		// OKボタンが押せるかどうか
		if (this.Attach.okButton != null)
			this.Attach.okButton.isEnabled = this.CanSelectOK;

		// 再配置
		this.RepositionItem();
	}
	/// <summary>
	/// アイテム追加
	/// </summary>
	GUIDeckEditItem AddItem(int index)
	{
		var prefab = this.Attach.itemPrefab;
		if (prefab == null)
			return null;
		var table = this.Attach.itemTable;
		if (table == null)
			return null;

		// アイテム作成
		var item = GUIDeckEditItem.Create(prefab.gameObject, table.transform, index);
		if (item == null)
			return null;
		// リストに追加
		this.ItemList.Add(item);

		return item;
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
		// リスト削除
		this.ItemList.Clear();
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

	#region スロット変更
    public void ResetDeckOrder()
    {
        ReGetItemList();
        List<CharaInfo> tCharaInfos = new List<CharaInfo>();
        for (int i = 0; i < ItemList.Count; ++i)
        {
            GUIDeckEditItem t = ItemList[i];
            t.CharaInfo.DeckSlotIndex = i;
            t.Refresh();
            tCharaInfos.Add(t.CharaInfo);
        }
        this.DeckInfo.ReOrderCharaInfoList(tCharaInfos);
    }

    public void ReGetItemList()
    {
       ItemList.Clear();
       foreach (var item in Attach.itemTable.GetChildList())
        {
            GUIDeckEditItem t = item.GetComponent<GUIDeckEditItem>();
            ItemList.Add(t);
            if (DeckMode.Edit == this.Mode)
            {
                t.GetComponent<DeckDragItem>().interactable = true;
            }
            else
            {
                t.GetComponent<DeckDragItem>().interactable = false;
            }
        }
    }

	/// <summary>
	/// スロット変更
	/// </summary>
	public static void ChangeSlot(CharaInfo info, int index)
	{
		if (Instance != null) Instance._ChangeSlot(info, index);
	}
	/// <summary>
	/// スロット変更
	/// </summary>
	void _ChangeSlot(CharaInfo info, int index)
	{
		if (info == null)
		{
			Debug.LogWarning("CharaInfo is Null!!");
			return;
		}
		var deckInfo = this.DeckInfo;
		if (deckInfo == null)
		{
			Debug.LogWarning("DeckInfo is Null!!");
			return;
		}
		if (deckInfo.CharaInfoList == null)
		{
			Debug.LogWarning("DeckInfo.CharaInfoList is Null!!");
			return;
		}
		if (index < 0 || index >= deckInfo.CharaInfoList.Count)
		{
			Debug.LogWarning("DeckInfo.charaInfoList Out of Range!! Index=" + index + " ListCount=" + deckInfo.CharaInfoList.Count);
			return;
		}
		try
		{
			// スロット変更
			info.DeckSlotIndex = index;
			deckInfo.CharaInfoList[index] = info;
			// アイテム設定
			this.ItemList[index].Setup(info, this.CharaIcon, this.SkillIcon);

			this.DeckTotalCost = deckInfo.DeckTotalCost;
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("index=" + index + " List.Count=" + deckInfo.CharaInfoList.Count + "\r\n" + e);
		}
		// コストUI更新
		this.CostUIUpdate();
		// OKボタンが押せるかどうか
		if (this.Attach.okButton != null)
			this.Attach.okButton.isEnabled = this.CanSelectOK;
	}
	#endregion

	#region 選択したアイテム設定
	/// <summary>
	/// 選択したアイテムを設定する
	/// </summary>
	public static void SetSelectItem(GUIDeckEditItem item)
	{
		if (Instance != null) Instance._SetSelectItem(item);
	}
	/// <summary>
	/// 選択したアイテムを設定する
	/// </summary>
	void _SetSelectItem(GUIDeckEditItem item)
	{
		switch (GUIDeckEdit.NowMode)
		{
		case GUIDeckEdit.DeckMode.Edit:
			this.SelectEditMode(item);
			break;
		case GUIDeckEdit.DeckMode.CharaSelect:
			this.SelectCharaMode(item, GUIMapWindow.SelectRespawnObj);
			break;
		}
	}
	/// <summary>
	/// エディットモード時のキャラスと選択処理
	/// </summary>
	void SelectEditMode(GUIDeckEditItem item)
	{
		// ロード中
		if (this.IsLoading)
			return;

		// インデックスを設定
		this.CurrentSlotIndex = item.CharaInfo.DeckSlotIndex;

		// デッキ画面非表示
//		GUIDeckEdit.Close();
		// デッキ使用中キャラを設定する
		List<ulong> usedOwnCharaIDList = new List<ulong>();
		if (this.DeckInfo != null)
		{
			var list = this.DeckInfo.CharaInfoList;
			if (list != null)
			{
				foreach (var t in list)
				{
					usedOwnCharaIDList.Add(t.UUID);
				}
			}
		}
		// キャラクターストレージを開く
//		GUICharacterStorage.SetModeDeckEdit(item.CharaInfo.UUID, usedOwnCharaIDList,
		GUIDeckEditCharacterStorage.SetModeDeckEdit(item.CharaInfo.UUID, usedOwnCharaIDList,
			null,
			() =>
			{
				// デッキ編集に戻る
				GUIDeckEdit.SetModeEdit(false, this.OnHomeFunction, this.OnCloseFunction);
			},
			(changeTargetOwnCharaID, charaInfo) =>
			{
				if (charaInfo == null)
					return;
				// デッキ編集に戻る
				GUIDeckEdit.SetModeEdit(false, this.OnHomeFunction, this.OnCloseFunction);
				if (changeTargetOwnCharaID == charaInfo.UUID)
					return;
				// スロット変更
				// デッキモードを変えてからスロットを変更しないと
				// モードごとのグループ表示がされない
				GUIDeckEdit.ChangeSlot(charaInfo.Clone(), item.CharaInfo.DeckSlotIndex);
			}
			);
	}
	/// <summary>
	/// キャラ選択時のスロット選択処理
	/// </summary>
	void SelectCharaMode(GUIDeckEditItem item, ObjectBase respawn)
	{
		if (GUIMapWindow.LastWindowMode == GUIMapWindow.MapMode.Respawn && respawn == null)
		{
			Debug.LogWarning("Respawn Point Not Found!!");
			GUIMapWindow.LostRespawnObj();
			return;
		}
		// クールタイム中もしくは空のデッキアイテムが選択された時は処理しない
		else if (0f < item.RemainingRebuildTime || item.CharaInfo.IsDeckSlotEmpty )
		{
			return;
		}

		// インデックスを設定
		this.CurrentSlotIndex = item.CharaInfo.DeckSlotIndex;

		// デッキ画面非表示
		GUIDeckEdit.Close();
		this.OnSelectFunction(item.CharaInfo.DeckSlotIndex, this.DeckInfo, item.CharaInfo);

		if (GUIMapWindow.LastWindowMode == GUIMapWindow.MapMode.Respawn)
		{
			// 選択したキャラクターインデックスを送信する
			BattlePacket.SendSelectCharacter(item.CharaInfo.DeckSlotIndex, respawn.InFieldId);
		}
		else if (GUIMapWindow.LastWindowMode == GUIMapWindow.MapMode.Transport)
		{
			// 前回モードでマップ画面を開く
			GUIMapWindow.OpenLastMode();
		}
	}
	#endregion

	#region 削除するアイテム設定
	/// <summary>
	/// 削除するアイテム設定
	/// </summary>
	public static void SetDeleteItem(GUIDeckEditItem item)
	{
		if (Instance != null) Instance._SetDeleteItem(item);
	}
	/// <summary>
	/// 削除するアイテム設定
	/// </summary>
	void _SetDeleteItem(GUIDeckEditItem item)
	{
		switch (GUIDeckEdit.NowMode)
		{
		case GUIDeckEdit.DeckMode.Edit:
			this.DeleteEditMode(item);
			break;
		}
	}
	/// <summary>
	/// エディットモード時の削除処理
	/// </summary>
	void DeleteEditMode(GUIDeckEditItem item)
	{
		// スロット変更
		GUIDeckEdit.ChangeSlot(new CharaInfo(), item.CharaInfo.DeckSlotIndex); 
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// ホームボタンを押した時
	/// </summary>
	public void OnHome()
	{
		Close();
		this.OnHomeFunction();
	}
	/// <summary>
	/// 閉じるボタンを押した時
	/// </summary>
	public void OnClose()
	{
		Close();
		this.OnCloseFunction();
	}
	/// <summary>
	/// PlayTween再生終了時の処理
	/// </summary>
	public void OnActivePlayTweenFinish()
	{
		if (this.Mode != DeckMode.None)
		{
			// 低FPSだと何故かスケールしきらない（頂点位置の更新がされない?）時がある
			// Activeの切り替えで表示されたのでとりあえずこれで対処
			gameObject.SetActive(false);
			gameObject.SetActive(true);
		}
	}
	/// <summary>
	/// OKボタンを押した時
	/// </summary>
	public void OnOK()
	{
		switch (this.Mode)
		{
		case DeckMode.Edit:
			this.OKModeEdit();
			break;
		}
	}
	/// <summary>
	/// エディットモード時のOKボタンを押した処理
	/// </summary>
	void OKModeEdit()
	{
		// 選択できるかどうか
		if (!this.CanSelectOK)
			return;

		// UNDONE:本来であれば変更されているかチェックしてサーバーに送るか決める
		// キャラクターデッキを送信する
		var deckInfo = this.DeckInfo;
		if (deckInfo != null && deckInfo.CharaInfoList != null)
		{
			int max = deckInfo.CharaInfoList.Count;
			ulong[] charaIDs = new ulong[max];
			for (int i = 0; i < max; i++)
			{
				var info = deckInfo.CharaInfoList[i];
				if (info.IsDeckSlotEmpty)
					charaIDs[i] = 0;	// スロットが空なら0
				else
					charaIDs[i] = info.UUID;
			}
			CommonPacket.SendSetCharacterDeck(deckInfo.DeckID, deckInfo.DeckName, charaIDs);
		}

		// 閉じる
		Close(true);
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
		public DeckMode mode;
		public bool isModeHomeButton;
		public bool isModeCloseButton;
		public bool updatePage;
		public int nowPage;
		public int maxPage;
		public bool executeDeck;
		public DeckInfo deckInfo;
		public bool executeCreateDeck;
		public int createListMaxCharaID;

		public bool IsReadMasterData { get; set; }
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
			this._SetMode(t.mode, false, t.isModeHomeButton, t.isModeCloseButton,
				() => { Debug.Log("OnHome"); }, () => { Debug.Log("OnClose"); }, (a, b, c) => { Debug.Log("OnOK"); });
		}
		if (t.updatePage)
		{
			this._SetPage(t.nowPage, t.maxPage);
		}
		if (t.executeDeck)
		{
			t.executeDeck = false;

			this.DebugPrefabUpdate();
			this._SetDeck(t.deckInfo, t.deckInfo.CurrentSlotIndex);
		}
		if (t.executeCreateDeck)
		{
			t.executeCreateDeck = false;

			if (this.DebugPrefabUpdate())
			{
				List<Scm.Common.Master.CharaMasterData> dataList = new List<Scm.Common.Master.CharaMasterData>();
				for (int i = 0; i <= t.createListMaxCharaID; i++)
				{
					Scm.Common.Master.CharaMasterData data;
					if (MasterData.TryGetChara(i, out data))
					{
						dataList.Add(data);
					}
				}

				for (int i = 0, max = t.deckInfo.CharaInfoList.Count; i < max; i++)
				{
					var dataIndex = Random.Range(0, dataList.Count - 1);
					var data = dataList[dataIndex];

					var info = new CharaInfo();
					var uuid = (ulong)(i + 1);
					info.DebugRandomSetup();
					info.DebugSetUUID(uuid);
					info.DebugSetAvatarType(data.ID);
					info.DebugSetName(data.Name);
					t.deckInfo.CharaInfoList[i] = info;
				}
			}
		}
	}
	bool DebugPrefabUpdate()
	{
		string err = null;
		if (Object.FindObjectOfType(typeof(FiberController)) == null)
			err += "FiberController.prefab を入れて下さい\r\n";
		if (MasterData.Instance == null)
			err += "MasterData.prefab を入れて下さい\r\n";
		if (!string.IsNullOrEmpty(err))
		{
			Debug.LogWarning(err);
			return false;
		}

		var t = this.DebugParam;
		if (!t.IsReadMasterData)
		{
			MasterData.Read();
			t.IsReadMasterData = true;
		}
		return t.IsReadMasterData;
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		if (Application.isPlaying)
			this.DebugUpdate();
	}
#endif
	#endregion
}
