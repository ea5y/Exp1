/// <summary>
/// マップウィンドウ
/// 
/// 2014/12/05
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.GameParameter;
public class GUIMapWindow : Singleton<GUIMapWindow>
{
	#region 定義

	// スケール処理を開始する指の数
	const int ScaleStartFingerNum = 2;

	// スケール処理を開始するしきい値
	const float ScaleStartThreshold = 0.01f;

	// ウィンドウの状態
	public enum MapMode{ Off, Briefing , Battle , Respawn , Transport }
	#endregion 

	#region フィールド＆プロパティ

	// スクロール率
	[SerializeField,Tooltip("スクロール率")]
	float _scrollMag = 0.005f;
	float ScrollMag{ get{ return _scrollMag; } }

	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField,Tooltip("初期モード")]
	MapMode _startMode = MapMode.Off;
	MapMode StartMode{ get{ return this._startMode; } }
 
	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField] AttachObject _attach;
	public AttachObject Attach { get { return _attach; } }
	// アタッチするオブジェクト
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween			rootTween;
		public UIPlayTween			respawnTween;
		public UITable				myMemberIconRoot;
		public UITable				enemyIconRoot;
		public GameObject			myTeamIconPrefab;
		public GameObject			enemyTeamIconPrefab;
		public GUIScrollMessage		scrollMsg;
		public Transform			mapOffset;
		public UIScrollView			mapScrollView;
		public UICenterOnChild		centerOnChild;
		public UIDragScrollView		dragScrollView;
		public GameObject			charaChangeButton;
		public GameObject			groupSelectRespawn;
		public BoxCollider			bgCollider;
		[Header("チームスキル関連")]
		public UILabel				myTeamSkillCountLabel;
		public UILabel				enemyTeamSkillCountLabel;
		public GameObject			myTeamInfoRoot;
		public GameObject			enemyTeamInfoRoot;
	}

	// アクティブ設定
	bool _isActive;
	public static bool IsActive { get { return ( Instance != null ) ? Instance._isActive : false ; }}
	// バトルメンバー情報
	List<MemberInfo> MemberList{ get;set; }
	// 味方アイコンリスト
	List<GUIMapWindowMemberIcon> FriendMemberIconList{ get;set; }
	// 敵アイコンリスト
	List<GUIMapWindowMemberIcon> EnemyMemberIconList{ get;set; }

	/// <summary>
	/// マップパネルの四隅
	/// </summary>
	public static Vector3[] MapPanelWorldCorners
	{
		get
		{
			if(Instance!= null && Instance.Attach.mapScrollView != null)
			{
				if( Instance.MapPanelWorldCornerCache == null )
				{
					Instance.MapPanelWorldCornerCache = Instance.Attach.mapScrollView.panel.worldCorners;
				}
				return Instance.MapPanelWorldCornerCache;
			}

			return new Vector3[4];
		}
	}
	Vector3[] MapPanelWorldCornerCache{get;set;}

	// キャラアイコンクラスインスタンス
	public CharaIcon CharaIcon { get{ return ScmParam.Battle.CharaIcon; } } 

	// ウィンドウモード
	MapMode _mode;
	public static MapMode Mode { get { return ( Instance != null ) ? Instance._mode : GUIMapWindow.MapMode.Off ; }}

	// 前回開いた時の画面モード
	MapMode _lastMode;
	MapMode LastMode { get{ return Instance._lastMode; }}
	public static MapMode LastWindowMode
	{ 
		get{ return Instance != null ? Instance.LastMode : MapMode.Off;}
		set{ if(Instance != null)Instance._lastMode = value; }
	}

	/// <summary>
	/// 選択されたリスポーンオブジェクト
	/// </summary>
	ObjectBase _selectRespawnObject;
	public static ObjectBase SelectRespawnObj { get { return (Instance != null) ? Instance._selectRespawnObject : null; }}

	// スクロールパネルのクリップ初期値
	Vector2 StartClipOffset{get;set;}
	// スクロールパネルの初期位置
	Vector3 StartScrollPanelPos{get;set;}

	// モード変更時のイベント
	public delegate void ChengeModeEventHandler( GUIMapWindow.MapMode mode );
	event ChengeModeEventHandler _changeModeEvent;
	public static event ChengeModeEventHandler ChangeModeEvent
	{ 
		add { if( Instance != null ) Instance._changeModeEvent += value;}
		remove { if(Instance!= null) Instance._changeModeEvent -= value;}
	}

	// マップ枠のサイズ
	Vector2 MapFrameSize
	{
		get
		{
			if( this.Attach.mapScrollView != null )	
				return new Vector2(this.Attach.mapScrollView.panel.baseClipRegion.z,this.Attach.mapScrollView.panel.baseClipRegion.w);

			return Vector2.one;
		}
	}

	#endregion
	
	#region 初期化
	protected override void Awake()
	{
		base.Awake();

		this.MemberList = new List<MemberInfo>();
		this.FriendMemberIconList = new List<GUIMapWindowMemberIcon>();
		this.EnemyMemberIconList = new List<GUIMapWindowMemberIcon>();
		this.AllIconDestroy();
		this.Attach.centerOnChild.enabled = false;
		//this.Attach.centerOnChild.Recenter();
		this._lastMode = this._mode = MapMode.Off;

		if(this.Attach.scrollMsg != null)
			this.Attach.scrollMsg.SetMessage("");

		this._SetMode(this.StartMode);
	}

	void Start()
	{
		// 通信情報を元に勝利条件をセット
		if( NetworkController.Instance != null )
		{
			var serverValue = NetworkController.ServerValue;

			BattleFieldMasterData bfData;
			if(MasterData.TryGetBattleField(serverValue.BattleFieldId, out bfData))
			{
				if( bfData.BattleRule != null && this.Attach.scrollMsg != null) {
                    this.Attach.scrollMsg.SetMessage(BattleMain.GetRuleInfo(BattleMain.GetPlayerTeamType(), bfData.BattleRule));
                }
			}
		}

		// スクロール初期値セット
		if( this.Attach.mapScrollView != null )
		{
			this.StartClipOffset = this.Attach.mapScrollView.panel.clipOffset;
			this.StartScrollPanelPos = this.Attach.mapScrollView.panel.transform.position;
		}
	}

	void SetActive( bool isActive )
	{
		this._isActive = isActive;

		if( GUIMapWindow.IsActive )
		{
			this.Attach.dragScrollView.enabled = true;

			// マップルートのスケールを変える
			Vector3 scale = Vector3.one;
			scale.x = this.MapFrameSize.x/GUIMinimap.MapFrameSize.x;
			scale.y = this.MapFrameSize.y/GUIMinimap.MapFrameSize.y;

			this.Attach.mapOffset.transform.localScale = scale;
		}

		// スクロールメッセージの位置をリセット
		if(this.Attach.scrollMsg != null)
			this.Attach.scrollMsg.Reset();

		if( this.Attach.rootTween != null )
			this.Attach.rootTween.Play(isActive);
	}

	void AllIconDestroy()
	{
		// 各Rootオブジェクト以下のキャラアイコンを全て破棄する
		foreach (Transform icon in this.Attach.myMemberIconRoot.transform)
			Destroy(icon.gameObject);

		foreach (Transform icon in this.Attach.enemyIconRoot.transform)
			Destroy(icon.gameObject);

		this.FriendMemberIconList.Clear();
		this.EnemyMemberIconList.Clear();	
	}
	#endregion

	#region 各モードのセット

	public static void SetMode( MapMode mode )
	{
		if(Instance != null)Instance._SetMode(mode);
	}

	void _SetMode( MapMode mode )
	{
		this._lastMode = this._mode;
		this._mode = mode;

		this.SetActive((mode != MapMode.Off));

		switch( this._mode )
		{
		case MapMode.Briefing:
			{
				SetMinimap();
				if(this.Attach.charaChangeButton != null)
					this.Attach.charaChangeButton.gameObject.SetActive(false);
				if(this.Attach.groupSelectRespawn != null)
					this.Attach.groupSelectRespawn.SetActive(false);
				EnableControlButtons(false);
			}
			break;

		case MapMode.Battle:
			{
				SetMinimap();
				if(this.Attach.charaChangeButton != null)
					this.Attach.charaChangeButton.gameObject.SetActive(false);
				if(this.Attach.groupSelectRespawn != null)
					this.Attach.groupSelectRespawn.SetActive(false);
				EnableControlButtons(true);
			}
			break;

		case MapMode.Transport:
			{
				SetMinimap();
				if(this.Attach.charaChangeButton != null)
					this.Attach.charaChangeButton.gameObject.SetActive(true);
				if(this.Attach.groupSelectRespawn != null)
					this.Attach.groupSelectRespawn.SetActive(true);
				this.ScrollReset();
				RespawnAnimationStart();
				EnableControlButtons(false);
			}
			break;

		case MapMode.Respawn:
			{
				SetMinimap();
				if(this.Attach.charaChangeButton != null)
					this.Attach.charaChangeButton.gameObject.SetActive(false);
				if(this.Attach.groupSelectRespawn != null)
					this.Attach.groupSelectRespawn.SetActive(true);
				this.ScrollReset();
				RespawnAnimationStart();
				EnableControlButtons(false);
			}
			break;
		case MapMode.Off:
			{
				GUIMinimap.ResetParent();
				EnableControlButtons(true);
			}				
			break;
		}

		// チームスキル情報のActiveセット
		{
			this.Attach.enemyTeamInfoRoot.SetActive(GUITacticalGauge.IsDispTeamSkillInfo);
			this.Attach.myTeamInfoRoot.SetActive(GUITacticalGauge.IsDispTeamSkillInfo);
		}

		// モード変更イベント発生
		if( this._changeModeEvent != null )
			this._changeModeEvent(this._mode);
	}
	void RespawnAnimationStart()
	{
		if( this.Attach.respawnTween != null )
		{
			var pt = this.Attach.respawnTween;
			pt.SetTweener((obj)=>
				{
					obj.tweenFactor = 0f;
					obj.enabled = true;
				});
			pt.Play(true);
		}
	}
	/// <summary>
	/// 前回のモードでウィンドウを開く
	/// </summary>
	public static void OpenLastMode()
	{
		if(Instance != null)
			SetMode(Instance.LastMode);
	}

	/// <summary>
	/// 操作ボタンを有効にするかどうか
	/// </summary>
	/// <param name="enabled"></param>
	void EnableControlButtons( bool enabled )
	{
		GUIMoveStick.IsStickEnable = enabled;

		if( !enabled )
			GUISkill.ForceReleaseButtons();

		this.Attach.bgCollider.enabled = !enabled;
	}
	#endregion

	#region ミニマップ表示
	public static void SetMinimap()
	{
		if (Instance != null) Instance._SetMinimap();
	}
	void _SetMinimap()
	{
		GUIMinimap.ChangeParent(this.Attach.mapOffset);

		// プレイヤーを中心に合わせる
		//if( GUIMapWindow.Mode != MapMode.Respawn && GUIMapWindow.Mode != MapMode.Transport )
		//{
		//	var player = GameController.GetPlayer();
		//	if( player != null && player.MinimapIconItem != null )
		//		MapScrollTarget(player.MinimapIconItem.transform);
		//}
	}
	#endregion

	#region Update
	void Update()
	{
		// たまに指を離しても閉じない時があるので保険
		if (this._mode == MapMode.Battle )
		{
			if( (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) &&
				Input.touchCount == 0 )
			{
				this._SetMode(MapMode.Off);
			}
			else if( !Input.GetMouseButton(0) )
			{
				this._SetMode(MapMode.Off);
			}
		}
	}

	void LateUpdate()
	{
		// キャッシュ初期化
		this.MapPanelWorldCornerCache = null;
	}
	#endregion

	#region スクロール関連
	/// <summary>
	/// 目標が中心に来るようにスクロールさせる（マップのスクロール上限は越えない）
	/// </summary>
	/// <param name="targetTrans"></param>
	/// <returns></returns>
	//void MapScrollTarget(Transform targetTrans)
	//{		
	//	this.Attach.centerOnChild.CenterOn(targetTrans);
	//	var springPanel = this.Attach.mapScrollView.GetComponent<SpringPanel>();

	//	// １フレームで移動できるぐらい無茶苦茶大きい値
	//	springPanel.strength =  10000000;
	//	springPanel.onFinished = ()=>{ this.Attach.mapScrollView.RestrictWithinBounds(true); };
	//}
	/// <summary>
	/// マップスクロールのリセット
	/// </summary>
	void ScrollReset()
	{
		if( this.Attach.mapScrollView != null )
		{
			this.Attach.mapScrollView.panel.clipOffset =  this.StartClipOffset;
			this.Attach.mapScrollView.panel.transform.position = this.StartScrollPanelPos;
		}
	}
	#endregion

	#region メンバーのセット

	/// <summary>
	/// メンバー情報のセット
	/// </summary>
	/// <param name="list"></param>
	public static void SetMemberList(List<MemberInfo> memberList )
	{
		if( Instance != null )Instance._SetMemberList(memberList);
	}

	void _SetMemberList( List<MemberInfo> memberList )
	{
        Dictionary<TeamTypeClient, List<MemberInfo>> memberDic = new Dictionary<TeamTypeClient, List<MemberInfo>>();
        

        // チーム毎に振り分ける
        foreach( var info in memberList )
        {
            List<MemberInfo> list = null;

            if( ! memberDic.TryGetValue(info.teamType, out list) )
            {
                memberDic.Add(info.teamType,new List<MemberInfo>());
				list = memberDic[info.teamType];
            }
            list.Add(info);
        }

		List<MemberInfo> infoList = null;

		// 味方
		if( memberDic.TryGetValue(TeamTypeClient.Friend,out infoList))
		{
			UpdateMemberIcon(this.Attach.myTeamIconPrefab,infoList,this.FriendMemberIconList,this.Attach.myMemberIconRoot.transform);	
		}
		else
		{
			foreach( var icon in this.FriendMemberIconList )
				icon.gameObject.SetActive(false);
		}

		// 敵
		if( memberDic.TryGetValue(TeamTypeClient.Enemy,out infoList))
		{
			UpdateMemberIcon(this.Attach.enemyTeamIconPrefab,infoList,this.EnemyMemberIconList,this.Attach.enemyIconRoot.transform);	
		}
		else
		{
			foreach( var icon in this.EnemyMemberIconList )
				icon.gameObject.SetActive(false);
		}

		this.RepositionMemberIcon();
	}

	/// <summary>
	/// メンバー情報の更新
	/// </summary>
	/// <param name="memList"></param>
	/// <param name="iconList"></param>
	/// <param name="iconRoot"></param>
	void UpdateMemberIcon( GameObject iconPrefab , List<MemberInfo> memList , List<GUIMapWindowMemberIcon> iconList , Transform iconRoot )
	{
		// 昇順に並び替え
		memList.Sort((a,b)=>{ return GameGlobal.AscendSort(a.tacticalId,b.tacticalId); });

		// NULLチェック
		if( memList == null || iconList == null || iconRoot == null )
			return;

		// アイコンの設定
		int idx = 0;
		foreach( var info in memList )
		{
			GUIMapWindowMemberIcon icon = null;
				
			if( iconList.Count <= idx )
			{
				if ( iconPrefab != null)	
				{
					icon = GUIMapWindowMemberIcon.Create(iconPrefab, iconRoot.transform, idx);
					iconList.Add(icon);
				}
			}
			else
			{
				icon = iconList[idx];
			}
			// 情報をセット
			if(icon != null)
			{
				icon.Setup(info,this.CharaIcon);
			}
			idx++;
		}
		// 余計なアイコンはオフに
		int sIdx = iconList.Count-(iconList.Count-memList.Count);

		for( int i = sIdx ; i < iconList.Count ; i++ )
		{
			var offIcon = iconList[i];
			offIcon.gameObject.SetActive(false);
		}
	}

	void RepositionMemberIcon()
	{
		if( this.Attach.myMemberIconRoot != null ) 
			this.Attach.myMemberIconRoot.Reposition();

		if( this.Attach.enemyIconRoot != null )
			this.Attach.enemyIconRoot.Reposition();
	}
	#endregion

	#region マップパネルのアイコンが表示されてるか判定
	public static bool IsMapIconVisible( Vector3 worldPos )
	{
		return ( Instance != null && Instance.Attach.mapScrollView != null ) ? Instance.Attach.mapScrollView.panel.IsVisible(worldPos) : false;
	}
	#endregion 

	#region リスポーンポイントの選択
	public static void SelectRespawnPoint( ObjectBase respawnPoint )
	{
		if(Instance != null)Instance._SelectRespawnPoint(respawnPoint);
	}
	void _SelectRespawnPoint( ObjectBase respawnPoint )
	{
		if( respawnPoint == null )
			return;

		this._selectRespawnObject = respawnPoint;

		if( this._mode == MapMode.Respawn )
		{
			// デッキ画面開く
			GUIDeckEdit.SetModeCharaSelect(true,false,true,null,GUIMapWindow.OpenLastMode,null);
		}
		else
		{
			BattlePacket.SendSelectCharacter(GUIDeckEdit.NowSlotIndex,respawnPoint.InFieldId);
		}
		this._SetMode(MapMode.Off);
	}
	#endregion

	#region 再出撃失敗した時の挙動
	/// <summary>
	/// SendCharacterが失敗
	/// </summary>
	public static void FailSendCharacter()
	{
		if(Instance != null)Instance._FailSendCharacter();
	}
	public void _FailSendCharacter()
	{
		string msg = MasterData.GetText(TextType.TX075_MW_SelCharaFailMsg);

		// ブリーフィング中は専用のメッセージに変える
		if( NetworkController.ServerValue.FieldStateType == FieldStateType.Waiting )
		{
			msg = MasterData.GetText(TextType.TX076_MW_SelCharaFailMsgBriefing);
		}

		GUIMessageWindow.SetModeOK(msg,MasterData.GetText(TextType.TX057_Common_YesButton),true,GUIMessageWindow.GuideMode.None,()=>{});

		this._SetMode(this.LastMode);
	}
	/// <summary>
	/// リスポーン地点が既に破壊されてる
	/// </summary>
	public static void LostRespawnObj()
	{
		if(Instance != null)Instance._LostRespawnObj();
	}
	public void _LostRespawnObj()
	{
		// デッキ画面閉じる
		GUIDeckEdit.Close();

		GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX077_MW_DestroyRespawnObjMsg),MasterData.GetText(TextType.TX057_Common_YesButton),true,GUIMessageWindow.GuideMode.None,()=>{});
		this._SetMode(this.LastMode);
	}
	#endregion 

	#region チームスキルカウントセット
	public static void SetTeamSkillBreakCount( TeamTypeClient teamType , int count )
	{
		if( Instance != null )Instance._SetTeamSkillCount(teamType,count);
	}
	void _SetTeamSkillCount(TeamTypeClient teamType, int count)
	{
		switch (teamType)
		{
		case TeamTypeClient.Enemy:
			if (this.Attach.enemyTeamSkillCountLabel != null)
				this.Attach.enemyTeamSkillCountLabel.text = string.Format(MasterData.GetText( TextType.TX069_MW_TeamSkillBreakCount , count.ToString()));
			break;

		case TeamTypeClient.Friend:
			if (this.Attach.myTeamSkillCountLabel != null)
				this.Attach.myTeamSkillCountLabel.text = string.Format(MasterData.GetText( TextType.TX069_MW_TeamSkillBreakCount , count.ToString()));
			break;
		}
	}

	#endregion

	#region NGUIリフレクション
	public void OnReposition()
	{
		if( IsActive )
		{
			this.Attach.scrollMsg.ReStart();
			this.RepositionMemberIcon();
		}
	}
	public void OnCharacterChange()
	{
		if( IsActive )
		{
			GUIDeckEdit.SetModeCharaSelect(true,false,true,null,GUIMapWindow.OpenLastMode,null);
			this._SetMode(MapMode.Off);
		}
	}
	#endregion 

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG


	/// <summary>
	/// デバッグパラメータ
	/// </summary>
	[SerializeField,Tooltip("テスト用の設定")]
	DebugParameter _debugParam = new DebugParameter();
	public DebugParameter DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class DebugParameter
	{
		[Tooltip("パラメータ反映フラグ")]
		public bool execute;
		[Tooltip("マップモード")]
		public MapMode mode;
		[Tooltip("勝利条件メッセージタイプ")]
		public BattleType battleRule = BattleType.Tower;
		[Tooltip("味方テームスキル発動まで数値")]
		public int myTeamSkillCount;
		[Tooltip("味方テームスキル発動まで数値")]
		public int enemyTeamSkillCount;
		[Tooltip("アイコン設定")]
		public DebugIcon debugIcon;
		public bool IsReadMasterData { get; set; }
	}

	/// <summary>
	/// アイコン関連
	/// </summary>
	[System.Serializable]
	public class DebugIcon
	{
		[Tooltip("アイコン生成")]
		public bool executeCreateIcon;
		[Tooltip("ランダム生成フラグ")]
		public bool isRandom;
		[Tooltip("自サイドランダム生成数")]
		public int randomFriendNum;
		[Tooltip("敵サイドランダム生成数")]
		public int randomEnemyNum;
		[Tooltip("自サイドの生成しているアイコンのパラメータ")]
		public List<DebugIconParam> friendIconParams = new List<DebugIconParam>();
		[Tooltip("敵サイドの生成しているアイコンのパラメータ")]
		public List<DebugIconParam> enemyIconParams = new List<DebugIconParam>();
	}

	/// <summary>
	/// アイコン１つの情報
	/// </summary>
	[System.Serializable]
	public class DebugIconParam
	{
		public int ID;
		public AvatarType avatarType;
		[HideInInspector] public TeamTypeClient teamType;
		public int lv;
		public string name;
        public int breakCount;
		public int rank;
	}

	void DebugUpdate()
	{
		var t = this.DebugParam;

		// 起動
		if (t.execute)
		{
			t.execute = false;
			{
				DebugPrefabUpdate();
				this._SetMode(t.mode);

				// バトルルール取得
				if( BattleRuleMaster.Instance != null )
				{
					BattleRuleMasterData data = null;
					BattleRuleMaster.Instance.TryGetMasterData((int)this.DebugParam.battleRule, out data );

					if( Instance != null && Instance.Attach.scrollMsg != null && data != null ) {
                        Instance.Attach.scrollMsg.SetMessage(BattleMain.GetRuleInfo(BattleMain.GetPlayerTeamType(), data));
                    }
				}

				// チームスキル数値反映
				this._SetTeamSkillCount(TeamTypeClient.Friend , t.myTeamSkillCount);
				this._SetTeamSkillCount(TeamTypeClient.Enemy , t.enemyTeamSkillCount);
			}
		}

		// アイコンデバッグ
		if( t.debugIcon != null && t.debugIcon.executeCreateIcon )
		{
			t.debugIcon.executeCreateIcon = false;
			DebugCharaIconCreate();
		}
	}

	void DebugCharaIconCreate()
	{
	 	List< MemberInfo > memInfoList = new List<MemberInfo>();

		if( !this.DebugParam.debugIcon.isRandom )
		{
			List<DebugIconParam> iconParams = new List<DebugIconParam>();
			iconParams.AddRange(this.DebugParam.debugIcon.friendIconParams.ToArray());
			iconParams.AddRange(this.DebugParam.debugIcon.enemyIconParams.ToArray());
			foreach( var icon in iconParams )
			{
				MemberInfo info = new MemberInfo();
				info.tacticalId = icon.ID;
				info.avatarType = icon.avatarType;
				info.teamType = icon.teamType;
				info.name = icon.name;
				memInfoList.Add(info);
			}
		}
		else
		{
			this.DebugParam.debugIcon.friendIconParams.Clear();
			for( int i = 0 ; i < this.DebugParam.debugIcon.randomFriendNum ; i++ ) 
			{
				DebugIconParam icon = new DebugIconParam();
				MemberInfo info = new MemberInfo();
				icon.name = info.name = string.Format("友だち{0}号",i+1);
				icon.ID = info.tacticalId = i+1;
				icon.teamType = info.teamType = TeamTypeClient.Friend;
				icon.avatarType = info.avatarType = (AvatarType)Random.Range((int)AvatarType.Begin,(int)AvatarType.End);
				icon.lv = Random.Range(1,100);
                icon.breakCount = Random.Range(1,100);
				icon.rank = Random.Range(1,10);
				memInfoList.Add(info);
				this.DebugParam.debugIcon.friendIconParams.Add(icon);
			}

			this.DebugParam.debugIcon.enemyIconParams.Clear();
			for( int i = 0 ; i < this.DebugParam.debugIcon.randomEnemyNum ; i++ ) 
			{
				DebugIconParam icon = new DebugIconParam();
				MemberInfo info = new MemberInfo();
				icon.name = info.name = string.Format("敵{0}号",i+1);
				icon.ID = info.tacticalId = i+1;
				icon.teamType = info.teamType = TeamTypeClient.Enemy;
				icon.avatarType = info.avatarType = (AvatarType)Random.Range((int)AvatarType.Begin,(int)AvatarType.End);
				icon.lv = Random.Range(1,100);
                icon.breakCount = Random.Range(1, 100);
				icon.rank = Random.Range(1, 10);
				memInfoList.Add(info);
				this.DebugParam.debugIcon.enemyIconParams.Add(icon);
			}
		}

		this._SetMemberList(memInfoList);
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
		this.DebugUpdate();
	}
#endif
	#endregion
}
