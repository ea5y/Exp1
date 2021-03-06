/// <summary>
/// ミニマップ
/// 
/// 2014/10/22
/// </summary>
using UnityEngine;
using System;
using System.Collections;

using Scm.Common.GameParameter;

public class GUIMinimap : Singleton<GUIMinimap>
{
	#region 宣言
	/// <summary>
	/// アイコンの種類
	/// </summary>
	[System.Serializable]
	public enum IconType
	{
		None,
		Player,
		Person,
		MainTower,
		SubTower,
		Guardian,
		Npc,
		Mob,
		RespawnPoint,
		Shield,
		ShieldGenerator,
		Transporter,
		HealPod,
		Catapult,
		ExpPod,
        Capsule
	}
	#endregion

	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

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
		public Transform minimapParent;
		public Transform minimapRoot;
		public UICenterOnChild uiCenterOnChild;
		public UIScrollView uiScrollView;
		public TweenAlpha minimapBGTween;

		// プレハブ
		public Prefab prefab;
		[System.Serializable]
		public class Prefab
		{
			public UISprite minimap;	// Minimap本体
			public GUIMinimapTargetMarker targetMarker;

			// アイコンプレハブ
			public Icon icon;
			[System.Serializable]
			public class Icon
			{
				public IconSet playerSet;
				public IconSet personSet;
				public IconSet mobSet;
				public IconSet npcSet;
				public IconSet guardianSet;
				public IconSet mainTowerSet;
				public IconSet subTowerSet;
				public IconSet respawnPointSet;
				public IconSet shieldSet;
				public IconSet shieldGeneratorSet;
				public IconSet transporterSet;
				public IconSet healPodSet;
				public IconSet catapultSet;
				public IconSet expPodSet;

                public IconSet capsuleSet;


				[System.Serializable]
				public class IconSet
				{
					public GUIMinimapIconItem iconItem;	// アイコンアイテムのプリセット
					public UISprite iconSprite;			// アイコン本体
				}
			}
		}
	}

	// アクティブ設定
	bool IsActive { get; set; }
	// ミニマップのスケール値
	Vector2 MinimapScale { get; set; }
	public static Vector2 Scale { get { return Instance != null ? Instance.MinimapScale : Vector2.one; } }
	// ミニマップの回転角度
	float MapRotateZ{ get;set; }
	public static float MapRotation
	{
		get
		{
			if(Instance != null)
				return Instance.MapRotateZ;
			return 0;
		}
	}
	// プレイヤーアイコンTransform
	public Transform PlayerIconTrans{get;set;}

	// スクロールパネルの枠のサイズ
	public static Vector2 MapFrameSize
	{
		get
		{
			if( Instance != null )
				return new Vector2( Instance.Attach.uiScrollView.panel.baseClipRegion.z , Instance.Attach.uiScrollView.panel.baseClipRegion.w);

			return Vector2.one;
		}
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();

		this.MinimapScale = Vector2.one;

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

	#region Minimap の親を変更する
	public static bool ChangeParent(Transform parent)
	{
		return (Instance == null ? false : Instance._ChangeParent(parent));
	}
	public static bool ResetParent()
	{
		return (Instance == null ? false : Instance._ChangeParent(Instance.Attach.minimapParent));
	}
	bool _ChangeParent(Transform parent)
	{
		var t = this.Attach.minimapRoot;
		if (t == null)
			return false;
		// マップウィンドウで拡縮した拡大率を維持する
		bool b = this.SetParent(t, parent, Vector3.zero, t.rotation, t.localScale);
		if (parent != null)
		{
			// ミニマップの親子関係を変えた時に
			// NGUIの更新タイミングがおかしいのかこうしないと上手く表示されない
			//parent.gameObject.SetActive(false);
			//parent.gameObject.SetActive(true);

			// 親パネルの変更をする場合はMarkParentAsChangedで変更を伝える
			//http://www.tasharen.com/forum/index.php?topic=11741.0
			NGUITools.MarkParentAsChanged(parent.gameObject);
		}

		// ミニマップの背景画像の表示切り替え
		if( this.Attach.minimapBGTween != null )
		{
			// ミニマップ親についたらオン
			if( this.Attach.minimapParent == parent )
				this.Attach.minimapBGTween.PlayForward();
			else
				this.Attach.minimapBGTween.PlayReverse();
		}

		return b;
	}
	/// <summary>
	/// 親を設定する
	/// </summary>
	bool SetParent(Transform t, Transform parent)
	{
		return this.SetParent(t, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}
	bool SetParent(Transform t, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		if (t == null)
			return false;

		if (parent != null)
		{
			NGUITools.SetLayer(t.gameObject, parent.gameObject.layer);
		}
		t.parent = parent;
		t.localPosition = position;
		t.localRotation = rotation;
		t.localScale = scale;

		return true;
	}
	#endregion

	#region スプライト作成
	/// <summary>
	/// Minimap 本体の作成
	/// GUIMinimapSettings にある UIAtlas を使用する
	/// </summary>
	public static UISprite CreateMinimap(UIAtlas atlas, Vector2 minimapScale)
	{
		return (Instance == null ? null : Instance._CreateMinimap(atlas, minimapScale));
	}
	UISprite _CreateMinimap(UIAtlas atlas, Vector2 minimapScale)
	{
		// 作成
		var prefab = this.Attach.prefab.minimap;
		var parent = this.Attach.minimapRoot;
		var sp = this.CreateComponent(prefab, parent);
		if (sp == null)
			return null;

		// 設定
		sp.atlas = atlas;
		this.MinimapScale = minimapScale;

		// ミニマップの回転
		var teamType = NetworkController.ServerValue.TeamType;	
		this.RotateMap(teamType);
		return sp;
	}
	/// <summary>
	/// アイコン作成
	/// </summary>
	public static GUIMinimapIconItem CreateIcon(IconType iconType, ObjectBase o)
	{
		return (Instance != null ? Instance._CreateIcon(iconType, o) : null);
	}
	GUIMinimapIconItem _CreateIcon(IconType iconType, ObjectBase o)
	{
		// プレハブ取得
		AttachObject.Prefab.Icon.IconSet iconSet = null;
		{
			var iconPrefabs = this.Attach.prefab.icon;
			switch (iconType)
			{
			case IconType.Player: iconSet = iconPrefabs.playerSet; break;
			case IconType.Person: iconSet = iconPrefabs.personSet; break;
			case IconType.MainTower: iconSet = iconPrefabs.mainTowerSet; break;
			case IconType.SubTower: iconSet = iconPrefabs.subTowerSet; break;
			case IconType.Guardian: iconSet = iconPrefabs.guardianSet; break;
			case IconType.Npc: iconSet = iconPrefabs.npcSet; break;
			case IconType.Mob: iconSet = iconPrefabs.mobSet; break;
			case IconType.RespawnPoint: iconSet = iconPrefabs.respawnPointSet; break;
			case IconType.Shield: iconSet = iconPrefabs.shieldSet; break;
			case IconType.ShieldGenerator: iconSet = iconPrefabs.shieldGeneratorSet; break;
			case IconType.Transporter: iconSet = iconPrefabs.transporterSet; break;
			case IconType.HealPod: iconSet = iconPrefabs.healPodSet; break;
			case IconType.Catapult: iconSet = iconPrefabs.catapultSet; break;
			case IconType.ExpPod: iconSet = iconPrefabs.expPodSet; break;
            case IconType.Capsule: iconSet = iconPrefabs.capsuleSet; break;
			}
		}
		if (iconSet == null)
			return null;

		// アイコンアイテム作成
		GUIMinimapIconItem item = null;
		if (iconSet.iconItem != null)
		{
			var t = iconSet.iconItem.transform;
			item = this.CreateComponent(iconSet.iconItem, this.Attach.minimapRoot, t.localPosition, t.localRotation, t.localScale);
		}
		if (item == null)
			return null;

		// アイコン本体作成
		UISprite icon = null;
		if (iconSet.iconSprite != null)
		{
			var t = iconSet.iconSprite.transform;
			icon = this.CreateComponent(iconSet.iconSprite, item.IconRoot, t.localPosition, t.localRotation, t.localScale);
		}

		// 設定
		item.Setup(o, icon);

		// 自プレイヤーの場合の処理
		if( PlayerManager.Instance != null && PlayerManager.Instance.Player != null && o != null &&
			o.InFieldId == PlayerManager.Instance.Player.InFieldId )
		{
			this.PlayerIconTrans = item.transform;
			this.Attach.uiCenterOnChild.enabled = false;
			//this.Attach.uiCenterOnChild.onFinished = ()=>{this.Attach.uiScrollView.RestrictWithinBounds(true);};
		}

		return item;
	}

	/// <summary>
	/// ミニマップ回転
	/// </summary>
	void RotateMap(TeamType teamType)
	{
		// 赤チームならマップを180度反転させて自チームが下に来るようにする
		if( teamType == TeamType.Red )
		{
			this.MapRotateZ = 180f;

			if( this.Attach.minimapRoot.eulerAngles.z != this.MapRotateZ )
			{
				//Debug.Log(string.Format("回転:{0}",teamType));
				this.Attach.minimapRoot.rotation = Quaternion.Euler(0f,0f,this.MapRotateZ);

		 		var ary = this.Attach.minimapRoot.GetComponentsInChildren<GUIMinimapIconItem>();
				// 既にくっついているマップアイコンのRotationを計算しなおし
				foreach( var icon in ary )
				{
					icon.transform.rotation = Quaternion.identity;
					icon.RotateUpdate();
				}
			}
		}
	}

	/// <summary>
	/// コンポーネント作成
	/// </summary>
	T CreateComponent<T>(T prefab, Transform parent) where T : Component
	{
		if (prefab == null)
			return null;
		var t = prefab.transform;
		return this.CreateComponent(prefab, parent, t.localPosition, t.localRotation, t.localScale);
	}
	T CreateComponent<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale) where T : Component
	{
		// プレハブが空
		if (prefab == null)
			return null;
		// インスタンス化できない
		var inst = SafeObject.Instantiate(prefab) as Component;
		if (inst == null)
			return null;
		// コンポーネントが取得できない
		var com = inst.GetComponent<T>();
		if (com == null)
		{
			UnityEngine.Object.Destroy(inst);
			return null;
		}

		// 親子付け
		this.SetParent(com.transform, parent, position, rotation, scale);

		return com;
	}
	#endregion

	#region Update
	void Update()
	{
		if( this.PlayerIconTrans != null )
		{
			this.Attach.uiCenterOnChild.CenterOnRestictWithinPanel(this.PlayerIconTrans);
		}
	}
	#endregion

	#region NGUIリフレクション
	public void OnClose()
	{
		this._SetActive(false);
	}
	public void OnMinimap()
	{
		GUIMapWindow.SetMode(GUIMapWindow.MapMode.Battle);
	}
	public void OnReleaseMinimap()
	{
		if( GUIMapWindow.Mode == GUIMapWindow.MapMode.Battle )
			GUIMapWindow.SetMode(GUIMapWindow.MapMode.Off);
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
	}
	void DebugUpdate()
	{
		var t = this.DebugParam;
		if (t.execute)
		{
			t.execute = false;
			{
				this._SetActive(t.isActive);
			}
		}
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		this.DebugUpdate();
	}
#endif
	#endregion
}
