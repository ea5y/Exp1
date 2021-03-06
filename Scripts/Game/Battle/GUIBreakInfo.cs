/// <summary>
/// 倒した相手の情報
/// 
/// 2014/07/21
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIBreakInfo : Singleton<GUIBreakInfo>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// カラー情報
	/// </summary>
	[SerializeField]
	ColorInfo _colorInfomation;
	ColorInfo ColorInfomation { get { return _colorInfomation; } }
	[System.Serializable]
	public class ColorInfo
	{
		public Color myteamLabel = Color.blue;
		public Color myteamFrame = Color.blue;
		public Color enemyLabel = Color.red;
		public Color enemyFrame = Color.red;
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
		public GUIPopup popup;
	}

	// アクティブ設定
	bool IsActive { get; set; }
	// ポップアップシステム
	BreakInfoPopupQueue PopupQueue { get; set; }
	// キャラアイコン
	CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }

	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.IsActive = false;
		this.PopupQueue = new BreakInfoPopupQueue();
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// ポップアップシステム
		this.Attach.popup.Setup(this.PopupQueue);

		// 表示設定
		this._SetActive(this.IsStartActive);
	}
	#endregion

	#region アクティブ設定
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;

		// アニメーション開始
		this.Attach.rootTween.Play(this.IsActive);
	}
	#endregion

	#region カラー情報取得
	public static ColorInfo GetColorInfo()
	{
		return (Instance != null ? Instance.ColorInfomation : null);
	}
	#endregion

	#region 追加
	public static void Add(ObjectBase win, ObjectBase lose)
	{
		if (Instance == null)
			return;

		bool isWinMyteam = false;
		{
			var winTeam = win.TeamType.GetClientTeam();
			isWinMyteam = (winTeam == TeamTypeClient.Friend);
		}
		AvatarType winAvatarType = AvatarType.None;
        int winSkinId = 0;
		{
			var c = win as Character;
			if (c != null) {
                winAvatarType = c.AvatarType;
                winSkinId = c.SkinId;
            }

		}
		AvatarType loseAvatarType = AvatarType.None;
        int loseSkinId = 0;
		{
			var c = lose as Character;
			if (c != null) {
                loseAvatarType = c.AvatarType;
                loseSkinId = c.SkinId;
            }
		}
		Instance._Add(isWinMyteam, winAvatarType, winSkinId, win.UserName, loseAvatarType, loseSkinId, lose.UserName);
	}
	void _Add(bool isWinMyteam, AvatarType winAvatarType, int winSkinId, string winName, AvatarType loseAvatarType, int loseSkinId, string loseName)
	{
		this.PopupQueue.Enqueue(
			new GUIBreakInfoItem.Info(isWinMyteam, winAvatarType, winSkinId, winName, loseAvatarType, loseSkinId, loseName),
			(GUIBreakInfoItem item, GUIBreakInfoItem.Info info) =>
			{
				item.Setup(info, this.CharaIcon);
			});
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParam _debug = new DebugParam();
	DebugParam Debug { get { return _debug; } }
	[System.Serializable]
	public class DebugParam
	{
		public bool execute;
		public bool isWinMyteam;
		public AvatarType winAvatarType;
        public int winSkinId;
		public string winName;
		public AvatarType loseAvatarType;
        public int loseSkinId;
		public string loseName;

		public bool IsReadMasterData { get; set; }
	}

	void DebugUpdate()
	{
		var t = this.Debug;
		if (t.execute)
		{
			t.execute = false;
			{
				string err = "";
				if (Object.FindObjectOfType(typeof(FiberController)) == null)
					err += "FiberController.prefab を入れて下さい\r\n";
				if (MasterData.Instance == null)
					err += "MasterData.prefab を入れて下さい\r\n";
				if (!string.IsNullOrEmpty(err))
					UnityEngine.Debug.LogWarning(err);
			}
			if (!t.IsReadMasterData)
			{
				MasterData.Read();
				t.IsReadMasterData = true;
			}
			this._Add(t.isWinMyteam, t.winAvatarType, t.loseSkinId, t.winName, t.loseAvatarType, t.loseSkinId, t.loseName);
		}
	}
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}


/// <summary>
/// 倒した相手のポップアップキュー
/// </summary>
public class BreakInfoPopupQueue : IPopupQueue
{
	#region フィールド＆プロパティ
	Queue<Item> Queue { get; set; }
	class Item
	{
		public GUIBreakInfoItem.Info info;	// アイテム情報
		public System.Action<GUIBreakInfoItem, GUIBreakInfoItem.Info> setupFunc;	// アイテム生成後に呼び出すデリゲート
	}
	#endregion

	#region 設定
	/// <summary>
	/// クローン
	/// </summary>
	public BreakInfoPopupQueue Clone()
	{
		var t = (BreakInfoPopupQueue)MemberwiseClone();
		if (this.Queue != null)
			t.Queue = new Queue<Item>(this.Queue);
		return t;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BreakInfoPopupQueue()
	{
		this.Queue = new Queue<Item>();
	}

	/// <summary>
	/// キュー登録
	/// </summary>
	public void Enqueue(GUIBreakInfoItem.Info info, System.Action<GUIBreakInfoItem, GUIBreakInfoItem.Info> setupFunc)
	{
		this.Queue.Enqueue(new Item()
			{
				info = info,
				setupFunc = setupFunc
			});
	}
	#endregion

	#region IPopupQueue
	/// <summary>
	/// キューが存在するかどうか
	/// </summary>
	public bool IsQueue { get { return this.Queue.Count > 0; } }

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
			var item = this.Queue.Dequeue();
			var com = GUIBreakInfoItem.Create(prefab, parent, 0);
			// アイテムセットアップ
			if (item.setupFunc != null)
				item.setupFunc(com, item.info);
			// 作成した GameObject から PopupItem を取得する
			var popupItem = com.GetComponent(typeof(GUIPopupItem)) as GUIPopupItem;
			return popupItem;
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("BreakInfoPopupQueue.Create:\r\n" + e);
			return null;
		}
	}
	#endregion IPopupQueue
}

