/// <summary>
/// 戦略ゲージ
/// 
/// 2014/07/17
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;
using Scm.Common.GameParameter;

public class GUITacticalGauge : Singleton<GUITacticalGauge>
{
	#region 定義
	#endregion

	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のバトルタイプ
	/// </summary>
	[SerializeField]
	BattleType _startBattleType = BattleType.Tower;
	BattleType StartBattleType { get { return _startBattleType; } }

	/// <summary>
	/// 共通部分
	/// </summary>
	[SerializeField]
	TacticalGauge.TGUICommon.Manager _common;
	TacticalGauge.TGUICommon.Manager Common { get { return _common; } }

	/// <summary>
	/// タワー戦
	/// </summary>
	[SerializeField]
	TacticalGauge.TGUITowerBattle.Manager _towerBattle;
	TacticalGauge.TGUITowerBattle.Manager TowerBattle { get { return _towerBattle; } }

	/// <summary>
	/// 殲滅戦
	/// </summary>
	[SerializeField]
	TacticalGauge.TGUIExterminate.Manager _exterminate;
	TacticalGauge.TGUIExterminate.Manager Exterminate { get { return _exterminate; } }

	/// <summary>
	/// 協力戦
	/// </summary>
	[SerializeField]
	TacticalGauge.TGUICooperative.Manager _cooperative;
	TacticalGauge.TGUICooperative.Manager Cooperative { get { return _cooperative; } }

	/// <summary>
	/// ソロ戦
	/// </summary>
	[SerializeField]
	TacticalGauge.TGUISolo.Manager _solo;
	TacticalGauge.TGUISolo.Manager Solo { get { return _solo; } }

	/// <summary>
	/// バグスレイヤー
	/// </summary>
	[SerializeField]
	TacticalGauge.TGUIBugSlayer.Manager _bugSlayer;
	TacticalGauge.TGUIBugSlayer.Manager BugSlayer { get { return _bugSlayer; } }

    /// <summary>
    /// Escort
    /// </summary>
    [SerializeField]
    TacticalGauge.TGUIEscort.Manager _escort;
    TacticalGauge.TGUIEscort.Manager Escort { get { return _escort; } }

    /// <summary>
    /// Resident Area
    /// </summary>
    [SerializeField]
    TacticalGauge.TGUIResidentArea.Manager _residentArea;
    TacticalGauge.TGUIResidentArea.Manager ResidentArea { get { return _residentArea; } }

    /// <summary>
    /// アタッチオブジェクト
    /// </summary>
    //[SerializeField]
    //AttachObject _attach;
    //public AttachObject Attach { get { return _attach; } }
    //[System.Serializable]
    //public class AttachObject
    //{
    //    public UILabel timeLabel;
    //}

    // バトルタイプ
    BattleType BattleType { get; set; }

	/// <summary>
	/// チームスキル発動までのカウンタを表示するか
	/// </summary>
	public static bool IsDispTeamSkillInfo
	{
		get
		{
			if (Instance != null && Instance.BattleType == BattleType.Tower)
			{
				return true;
			}

			return false;
		}
	}

    /// <summary>
    /// Round index, only contained in occupy & resident maps
    /// </summary>
    public static int RoundIndex {
        get {
            if (Instance != null && Instance.ResidentArea != null) {
                return Instance.ResidentArea.RoundIndex;
            }
            return -1;
        }
    }

	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.BattleType = BattleType.Tower;
	}
	#endregion

	#region 初期化
	protected new void Awake()
	{
		base.Awake();
		this.MemberInit();

		// 初期化
		this.Common.Clear();
		this.TowerBattle.Clear();
		this.Exterminate.Clear();
		this.Cooperative.Clear();
		this.Solo.Clear();
		this.BugSlayer.Clear();

		// 表示設定
		this._SetBattleType(this.StartBattleType);

	}
	#endregion

	#region 更新
	void Update()
	{
		this.Common.Update();

        if (this.BattleType == BattleType.Escort)
        {
            this.SetHostageGuide();
        }
        
	}
	#endregion

	#region バトルタイプ
	public static void SetBattleType(BattleType battleType)
	{
		if (Instance != null) Instance._SetBattleType(battleType);
	}

    public static BattleType GetBattleType()
    {
        if (Instance != null)
        {
            return Instance.BattleType;
        }
        Debug.Log("Error");
        return BattleType.BugSlayer;
    }

	void _SetBattleType(BattleType battleType)
	{
		this.BattleType = battleType;

		// 共通部分設定
		this.Common.SetBattleType(battleType);

		// バトルタイプごとの表示切り替え
		this.SetBattleTypeActive(battleType);
	}
	void SetBattleTypeActive(BattleType battleType)
	{
		// 一旦すべての表示をオフにする
		// 表示切り替え登録
		var list = new List<GameObject>();
		{
			list.Add(this.TowerBattle.Attach.root);
			list.Add(this.TowerBattle.Attach.panel3DRoot);
			list.Add(this.Exterminate.Attach.root);
			list.Add(this.Cooperative.Attach.root);
			list.Add(this.Solo.Attach.root);
			list.Add(this.BugSlayer.Attach.root);
            list.Add(this.Escort.Attach.root);
            list.Add(this.ResidentArea.Attach.root);
		}
		foreach (var t in list)
		{
			if (t != null)
				t.SetActive(false);
		}

		// バトルタイプごとの表示切り替え
		switch (battleType)
		{
		case BattleType.Tower:
			this.Common.SetActive(true);
			if (this.TowerBattle.Attach.root != null)
				this.TowerBattle.Attach.root.SetActive(true);
			if (this.TowerBattle.Attach.panel3DRoot != null)
				this.TowerBattle.Attach.panel3DRoot.SetActive(true);
			break;
		case BattleType.Exterminate:
			this.Common.SetActive(true);
			if (this.Exterminate.Attach.root != null)
				this.Exterminate.Attach.root.SetActive(true);
			break;
		case BattleType.Cooperative:
			this.Common.SetActive(true);
			if (this.Cooperative.Attach.root != null)
				this.Cooperative.Attach.root.SetActive(true);
			break;
		case BattleType.Solo:
			this.Common.SetActive(false);
			if (this.Solo.Attach.root != null)
				this.Solo.Attach.root.SetActive(true);
			break;
		case BattleType.BugSlayer:
			this.Common.SetActive(true);
			if (this.BugSlayer.Attach.root != null)
				this.BugSlayer.Attach.root.SetActive(true);
			break;
        case BattleType.Escort:
            this.Common.SetActive(true);
            if (this.Escort.Attach.root != null)
                this.Escort.Attach.root.SetActive(true);

            //init hostage
            this.InitHostageState();
            break;
        case BattleType.Resident:
            this.Common.SetActive(true);
            if (this.ResidentArea.Attach.root != null)
                this.ResidentArea.Attach.root.SetActive(true);
            break;
		default:
			this.Common.SetActive(true);
			break;
		}

	}
	#endregion

	#region タワーゲージ
	public static GUITowerGauge GetTowerGauge(ObjectBase tower)
	{
		return (Instance != null ? Instance._GetTowerGauge(tower) : null);
	}
	GUITowerGauge _GetTowerGauge(ObjectBase tower)
	{
		TeamTypeClient teamTypeClient = tower.TeamType.GetClientTeam();

		if (teamTypeClient == TeamTypeClient.Friend)
			return this.TowerBattle.GetTowerGauge(true, tower.EntrantType, tower.TacticalId);
		else if (teamTypeClient == TeamTypeClient.Enemy)
			return this.TowerBattle.GetTowerGauge(false, tower.EntrantType, tower.TacticalId);
		return null;
	}
	#endregion

	#region 残り時間パケット
	public static void RemainingTime(float second, float roundSecond, FieldStateType fieldStateType)
	{
		if (Instance != null) Instance._RemainingTime(second, roundSecond, fieldStateType);
	}
	void _RemainingTime(float second, float roundSecond, FieldStateType fieldStateType)
	{
		bool isStopTimer = false;
		switch (fieldStateType)
		{
		case FieldStateType.Free:		// 戦闘開始前の時間は EffectMessage で処理するのでここでは 0 にする
		case FieldStateType.Waiting:	// 戦闘開始前の時間は EffectMessage で処理するのでここでは 0 にする
			isStopTimer = true;
			second = 0f;
			break;
		case FieldStateType.Infinite:	// 時間無制限バトルはタイマーを止める
		case FieldStateType.Extra:		// 戦闘終了後はタイマーを止める
			isStopTimer = true;
			break;
		}
		this.Common.SetRemainingTime(second, roundSecond, isStopTimer);
	}
	#endregion

	#region チームスキルポイントパケット
	public static void TeamSkillPoint(TeamType teamType, byte point)
	{
		if (Instance != null) Instance._TeamSkillPoint(teamType, point);
	}
	void _TeamSkillPoint(TeamType teamType, byte point)
	{
		TeamTypeClient teamTypeClient = teamType.GetClientTeam();

		// 正規化する
		int normalizedPoint = this._GetNormalizeSideGaugePoint(teamType, point);

		if (teamTypeClient == TeamTypeClient.Friend)
		{
			this.TowerBattle.SetTeamSkillPoint(true, normalizedPoint);
			this.TowerBattle.SetRawTeamSkillPoint(true, point);
		}
		else if (teamTypeClient == TeamTypeClient.Enemy)
		{
			this.TowerBattle.SetTeamSkillPoint(false, normalizedPoint);
			this.TowerBattle.SetRawTeamSkillPoint(false, point);
		}
	}
	#endregion

	#region チームスキル発動カウント取得
	public static int GetSideGaugeCount(TeamTypeClient team)
	{
		if (Instance != null) return Instance._GetSideGaugeCount(team);

		return 0;
	}
	int _GetSideGaugeCount(TeamTypeClient team)
	{
		return this.TowerBattle.GetSideGaugeCount(team);
	}

	#endregion

	#region 残りポイントパケット
	public static void SideGauge(TeamType teamType, int remain, int total, int roundIndex)
	{
		if (Instance != null) Instance._SideGauge(teamType, remain, total, roundIndex);
	}
	void _SideGauge(TeamType teamType, int remain, int total, int roundIndex)
	{
		TeamTypeClient teamTypeClient = teamType.GetClientTeam();
		if (teamTypeClient == TeamTypeClient.Friend)
		{
			// タワー戦
			this.TowerBattle.SetRemainingPoint(true, remain, total);
			// 殲滅戦
			this.Exterminate.SetRemainingPoint(true, remain, total);
			// 協力戦
			this.Cooperative.SetRemainingPoint(remain, total);
            // バグスレイヤー
            this.BugSlayer.SetRemainingPoint(true, remain, total);
            // Resident Area
            this.ResidentArea.SetRemainingPoint(true, remain, total, roundIndex);

			// BGM判定.
			if (remain < (total * 0.3f))
			{
				if (BattleMain.Sound != null)
					BattleMain.Sound.PlayLimitBgm(SoundController.BgmID.Stage01_Limit);
			}
		}
		else if (teamTypeClient == TeamTypeClient.Enemy)
		{
			// タワー戦
			this.TowerBattle.SetRemainingPoint(false, remain, total);
			// 殲滅戦
			this.Exterminate.SetRemainingPoint(false, remain, total);
			// バグスレイヤー
			this.BugSlayer.SetRemainingPoint(false, remain, total);
            // Resident Area
            this.ResidentArea.SetRemainingPoint(false, remain, total, roundIndex);
		}

        if (teamType == TeamType.Red) {
            // Always set the score of the scroter side
            this.Escort.SetRemainingPoint(remain, total);
        }
        
	}
	public static int GetNormalizeSideGaugePoint(TeamType teamType, int point)
	{
		return (Instance != null ? Instance._GetNormalizeSideGaugePoint(teamType, point) : 0);
	}
	int _GetNormalizeSideGaugePoint(TeamType teamType, int point)
	{
		int maxSideGauge = 0;
		int sideGauge = 0;

		TeamTypeClient teamTypeClient = teamType.GetClientTeam();
		if (teamTypeClient == TeamTypeClient.Friend)
		{
			maxSideGauge = this.TowerBattle.MaxMyteamSideGauge;
			sideGauge = this.TowerBattle.NowMyteamSideGauge;
		}
		else if (teamTypeClient == TeamTypeClient.Enemy)
		{
			maxSideGauge = this.TowerBattle.MaxEnemySideGauge;
			sideGauge = this.TowerBattle.NowEnemySideGauge;
		}
		else
		{
			return 0;
		}

		// 正規化する
		int normalizePoint = Scm.Common.Utility.TeamSkill.GetNormalizePoint(maxSideGauge, sideGauge, point);
		return normalizePoint;
	}
	#endregion

	#region NGUIリフレクション
	public void OnExit()
	{
		// リザルトへ
		if (BattleMain.Instance != null)
		{
			// メッセージウィンドウ設定
			GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX130_ExitTraining),
				() =>
				{
					BattleMain.Instance.OnResult();
				},
				() => { }
			);
		}
	}
	#endregion

	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	DebugParameter _debugParam = new DebugParameter();
	DebugParameter DebugParam { get { return _debugParam; } set { _debugParam = value; } }
	[System.Serializable]
	public class DebugParameter
	{
		// 共通部分
		public Common common = new Common();
		[System.Serializable]
		public class Common
		{
			public bool executeBattleType;
			public BattleType battleType = BattleType.Tower;
			public bool executeStateType;
			public FieldStateType fieldStateType = FieldStateType.Waiting;
			public float remainingTime = 900f;
            public float roundRemainingTime = 900f;
		}

		// タワー戦
		public TowerBattle towerBattle = new TowerBattle();
		[System.Serializable]
		public class TowerBattle
		{
			public bool updateTeamSkillPoint;
			public float myteamSkillPoint = 0f;
			public float enemySkillPoint = 0f;
			public bool updateTowerGauge;
			public float myteamMainTower = 1f;
			public List<float> myteamSubTowerList = new List<float>(new float[] { 0.2f, 0.4f, 0.6f, 0.8f, 1f, });
			public float enemyMainTower = 1f;
			public List<float> enemySubTowerList = new List<float>(new float[] { 0.2f, 0.4f, 0.6f, 0.8f, 1f, });
		}

		// 殲滅戦
		public Exterminate exterminate = new Exterminate();
		[System.Serializable]
		public class Exterminate
		{
			public bool update;
			public int myteamRemain = 99;
			public int myteamTotal = 99;
			public int enemyRemain = 99;
			public int enemyTotal = 99;
		}

		// 協力戦
		public Cooperative cooperative = new Cooperative();
		[System.Serializable]
		public class Cooperative
		{
			public bool update;
            public int remain = 99;
            public int total = 99;
        }

		// ソロ戦
		public Solo solo = new Solo();
		[System.Serializable]
		public class Solo
		{
		}

		// バグスレイヤー
		public BugSlayer bugSlayer = new BugSlayer();
		[System.Serializable]
		public class BugSlayer
		{
			public bool update;
            public int myteamRemain = 99;
            public int myteamTotal = 99;
            public int enemyRemain = 99;
            public int enemyTotal = 99;
        }
	}
	void DebugUpdate()
	{
		// 共通部分
		{
			var t = this.DebugParam.common;
			if (t.executeBattleType)
			{
				t.executeBattleType = false;
				this._SetBattleType(t.battleType);
			}
			if (t.executeStateType)
			{
				t.executeStateType = false;
				this._SetBattleType(this.BattleType);
				this._RemainingTime(t.remainingTime, t.roundRemainingTime, t.fieldStateType);
			}
		}

		// タワー戦
		{
			var t = this.DebugParam.towerBattle;
			if (t.updateTeamSkillPoint)
			{
				this.TowerBattle.SetTeamSkillPoint(true, t.myteamSkillPoint);
				this.TowerBattle.SetTeamSkillPoint(false, t.enemySkillPoint);
			}
			if (t.updateTowerGauge)
			{
				this.TowerBattle.DebugUpdateTowerGauge(true, t.myteamMainTower, t.myteamSubTowerList);
				this.TowerBattle.DebugUpdateTowerGauge(false, t.enemyMainTower, t.enemySubTowerList);
			}
		}

		// 殲滅戦
		{
			var t = this.DebugParam.exterminate;
			if (t.update)
			{
				this.Exterminate.SetRemainingPoint(true, t.myteamRemain, t.myteamTotal);
				this.Exterminate.SetRemainingPoint(false, t.enemyRemain, t.enemyTotal);
			}
		}

		// 協力戦
		{
			var t = this.DebugParam.cooperative;
			if (t.update)
			{
				this.Cooperative.SetRemainingPoint(t.remain, t.total);
			}
		}

		// ソロ戦
		{
		}

		// バグスレイヤー
		{
			var t = this.DebugParam.bugSlayer;
			if (t.update)
			{
				this.BugSlayer.SetRemainingPoint(true, t.myteamRemain, t.myteamTotal);
                this.BugSlayer.SetRemainingPoint(false, t.enemyRemain, t.enemyTotal);
            }
		}
	}
	void OnValidate()
	{
		if (Application.isPlaying)
			this.DebugUpdate();
	}
#endif
	#endregion

    #region Hostage State
        //get percent
        //set hostage pos

        //waypoint
        //group
        //target
    float startPosX;
    float distance;

    List<List<BattleFieldWaypointMasterData>> waypointGroupList = new List<List<BattleFieldWaypointMasterData>>();

    //Init hostage state
    void InitHostageState()
    {
        this.startPosX = this.Escort.HostageSprite.transform.localPosition.x;
        this.distance = this.Escort.HostageStateSlider.GetComponentInChildren<UISprite>().width;

        List<BattleFieldWaypointMasterData> waypointList = MasterData.TryGetBattleFieldWaypoint((int)ScmParam.Battle.BattleFieldType);
        this.InitWaypointGroupListNew(waypointList);
        this.InstantiateWayPointNew(this.waypointGroupList, waypointList);
    }

    void InitWaypointGroupListNew(List<BattleFieldWaypointMasterData> waypointList)
    {
        List<BattleFieldWaypointMasterData> waypointInSameGroupList = new List<BattleFieldWaypointMasterData>();
        foreach (var point in waypointList)
        {
            waypointInSameGroupList.Add(point);
            if (point.StopFlag == true)
            {
                List<BattleFieldWaypointMasterData> temp = new List<BattleFieldWaypointMasterData>(waypointInSameGroupList);
                this.waypointGroupList.Add(temp);
            }
        }
    }

    float GetDistanceNew(List<BattleFieldWaypointMasterData> waypointList)
    {
        float distance = 0f;
        var prePoint = waypointList[0];
        foreach (var point in waypointList)
        {
            if (prePoint.Index == point.Index)
                continue;
            distance += Vector3.Distance(new Vector3(prePoint.PositionX, prePoint.PositionY, prePoint.PositionZ), new Vector3(point.PositionX, point.PositionY, point.PositionZ));

            prePoint = point;
            
        }
        return distance;
    }

    void InstantiateWayPointNew(List<List<BattleFieldWaypointMasterData>> waypointGroupList, List<BattleFieldWaypointMasterData> waypointList)
    {
        float distance = 0f;
        float distanceSum = this.GetDistanceNew(waypointList);
        float sliderWidth = this.Escort.HostageStateSlider.gameObject.GetComponentInChildren<UISprite>().width;
        float startPosX = -sliderWidth / 2;
        float posX = 0f;
        float posY = this.Escort.HostageStateSlider.transform.localPosition.y;
        foreach (var group in waypointGroupList)
        {
            distance = this.GetDistanceNew(group);
            
            posX = distance / distanceSum * sliderWidth + startPosX;
            Vector3 position = new Vector3(posX, posY, 0);

            GameObject go = NGUITools.AddChild(this.Escort.HostageStateSlider.gameObject, this.Escort.Waypoint.gameObject);
            if (go)
            {
                go.transform.parent = this.Escort.HostageStateSlider.gameObject.transform;
                go.transform.localPosition = position;
            }
        }
    }


    public void SetHostageState(float percent)
    {
        float posX = this.startPosX + (percent / 100) * this.distance;
        this.SetHostagePosX(posX);
    }

    void SetHostagePosX(float posX)
    {
        float posY = this.Escort.HostageSprite.transform.localPosition.y;
        this.Escort.HostageSprite.transform.localPosition = new Vector3(posX, posY, 0);
    }

    public void ShowEscortTime(int time)
    {
        StartCoroutine(this.ShowTimeTips(time));
    }

    IEnumerator ShowTimeTips(int time)
    {
        this.Escort.timeTips.GetComponent<UILabel>().color = this.IsTeamAttack() ? Color.green : Color.red;
        this.Escort.timeTips.GetComponent<UILabel>().text = "护送时间增加 " + time + " 秒";
        this.Escort.timeTips.SetActive(true);
        this.Escort.timeTips.GetComponent<TweenAlpha>().PlayForward();
        yield return new WaitForSeconds(3f);
        this.Escort.timeTips.GetComponent<TweenAlpha>().PlayReverse();
        yield return new WaitForSeconds(3f);
        this.Escort.timeTips.SetActive(false);
    }
    #endregion

    #region hostage guide
    Hostage hostage;
    float Ymin = 0.0f, Ymax = 1.0f, Xmin = 0.0f, Xmax = 1.0f;
    Vector2 centerViewPos = new Vector2(0.5f, 0.5f);
    Vector3 destinationViewPos = Vector3.zero;
    bool isDivide = false;
    void SetHostageGuide()
    {
        //hostage pos
        this.hostage = NpcManager.Instance.transform.GetComponentInChildren<Hostage>();
        if (this.hostage)
        {
            var hostageViewPos = Camera.main.WorldToViewportPoint(this.hostage.gameObject.transform.position);
            this.CalculateGuidePos(hostageViewPos, this.Escort.guideHostage);
            //set marker
            this.SetMarker(this.hostage.gameObject.transform.position, this.Escort.hostageMarker);
        }
       
        //destination pos
        this.destinationViewPos = this.GetDestinationViewPos();
        this.CalculateGuidePos(this.destinationViewPos, this.Escort.guideDestination);
        //set marker
        this.SetMarker(this.GetDestinationPos(), this.Escort.destinationMarker);

        //set color
        if (this.isDivide == false && this.hostage)
        {
            this.SetGuideColor(this.Escort.guideHostage, this.IsTeamAttack() ? Color.blue : Color.red);
            this.SetGuideColor(this.Escort.guideDestination, this.IsTeamAttack() ? Color.blue : Color.red);
        }
    }

    bool IsTeamAttack()
    {
        var teamType = GameController.GetPlayer().TeamType;
        return teamType == this.hostage.TeamType;
    }

    void SetGuideColor(GameObject guide, Color color)
    {
        var sprite = guide.GetComponentInChildren<UISprite>();
        sprite.color = color;
    }

    Vector3 GetDestinationViewPos()
    {
        var result = Camera.main.WorldToViewportPoint(this.GetDestinationPos());
        return result;
    }

    Vector3 GetDestinationPos()
    {
        List<BattleFieldWaypointMasterData> waypointList = MasterData.TryGetBattleFieldWaypoint((int)ScmParam.Battle.BattleFieldType);

        var destinationPos = new Vector3(waypointList[waypointList.Count - 1].PositionX, waypointList[waypointList.Count - 1].PositionY, waypointList[waypointList.Count - 1].PositionZ);
        return destinationPos;
    }

    void CalculateGuidePos(Vector3 targetViewPos, GameObject guide)
    {
        float topX;
        if (targetViewPos.z > 0 && this.CalculateGuidePosTop(targetViewPos, this.centerViewPos, out topX))
        {
            //at top
            this.SetGuidePos(new Vector2(topX, this.Ymax), guide);
            return;
        }

        float bottomX;
        if (targetViewPos.z < 0 && this.CalculateGuidePosBottom(targetViewPos, this.centerViewPos, out bottomX))
        {
            //at bottom
            this.SetGuidePos(new Vector2(bottomX, this.Ymin), guide);
            return;
        }

        float leftY;
        if (this.CalculateGuidePosLeft(targetViewPos, this.centerViewPos, out leftY))
        {

            //at left
            this.SetGuidePos(new Vector2(this.Xmin, leftY), guide);
            return;
        }

        float rightY;
        if (this.CalculateGuidePosRight(targetViewPos, this.centerViewPos, out rightY))
        {
            //at right
            this.SetGuidePos(new Vector2(this.Xmax, rightY), guide);
            return;
        }

        if (guide.activeSelf == true)
        {
            guide.SetActive(false);
        }
    }

    void SetGuideRotation(GameObject guide)
    {
        //rotate the arrow
        guide.transform.FindChild("Arrow").transform.LookAt(guide.transform.localPosition, Vector3.up);
        //get the root of arrow
        var point = guide.transform.FindChild("Arrow").transform.FindChild("Point");
        //change parent of point
        point.transform.parent = guide.transform;
        //set the label's localposition with the point localposition that has changed
        guide.transform.FindChild("Label").transform.localPosition = point.transform.localPosition;
        //reset the parent of point
        point.transform.parent = guide.transform.FindChild("Arrow").transform;
    }

    void SetGuidePos(Vector2 viewPos, GameObject guide)
    {
        if (guide.activeSelf == false)
        {
            guide.SetActive(true);
        }
        var screenPos = Camera.main.ViewportToScreenPoint(viewPos);
       
        var realPos = NGUIMath.ScreenToParentPixels(screenPos, guide.transform.parent);

        guide.transform.localPosition = realPos;
        this.SetGuideRotation(guide);
    }

    
    bool CalculateGuidePosTop(Vector3 hostageViewPos, Vector2 centerViewPos, out float x)
    {
        x = (this.Ymax - centerViewPos.y) / (hostageViewPos.y - centerViewPos.y) * (hostageViewPos.x - centerViewPos.x) + centerViewPos.x;
        if (x > this.Xmin && x < this.Xmax && hostageViewPos.y > 1.0)
            return true;
        else
            return false;
    }

    bool CalculateGuidePosBottom(Vector3 hostageViewPos, Vector2 centerViewPos, out float x)
    {
        x = (this.Ymin - centerViewPos.y) / (hostageViewPos.y - centerViewPos.y) * (hostageViewPos.x - centerViewPos.x) + centerViewPos.x;
        if (x > this.Xmin && x < this.Xmax)
            return true;
        else
            return false;
    }

    bool CalculateGuidePosLeft(Vector3 hostageViewPos, Vector2 centerViewPos, out float y)
    {
        if (hostageViewPos.z > 0)
        {
            y = (this.Xmin - centerViewPos.x) / (hostageViewPos.x - centerViewPos.x) * (hostageViewPos.y - centerViewPos.y) + centerViewPos.y;
            if (y > this.Ymin && y < this.Ymax && hostageViewPos.x < -0.1)
                return true;
            else
                return false;
        }
        else
        {
            y = this.Ymax - ((this.Xmax - centerViewPos.x) / (hostageViewPos.x - centerViewPos.x) * (hostageViewPos.y - centerViewPos.y) + centerViewPos.y);
            if (y > this.Ymin && y < this.Ymax && hostageViewPos.x > 0.5)
                return true;
            else
                return false;
        }
        
    }

    bool CalculateGuidePosRight(Vector3 hostageViewPos, Vector2 centerViewPos, out float y)
    {
        if (hostageViewPos.z > 0)
        {
            y = (this.Xmax - centerViewPos.x) / (hostageViewPos.x - centerViewPos.x) * (hostageViewPos.y - centerViewPos.y) + centerViewPos.y;
            if (y > this.Ymin && y < this.Ymax && hostageViewPos.x > 1.1)
                return true;
            else
                return false;
        }
        else
        {
            y = this.Ymax - ((this.Xmin - centerViewPos.x) / (hostageViewPos.x - centerViewPos.x) * (hostageViewPos.y - centerViewPos.y) + centerViewPos.y);
            
            if (y > this.Ymin && y < this.Ymax && hostageViewPos.x < 0.5)
                return true;
            else
                return false;
        }
    }

    #endregion

    #region hostageMarker
    void SetMarker(Vector3 pos, GameObject marker)
    {
        var screenPos = Camera.main.WorldToScreenPoint(pos);
        
        if (screenPos.z > 0)
        {
            if (marker.activeSelf == false)
            {
                marker.gameObject.SetActive(true);
            }
            var realPos = NGUIMath.ScreenToParentPixels(screenPos, marker.transform.parent);
            marker.transform.localPosition = realPos;
        }
        else
        {
            if (marker.activeSelf == true)
            {
                marker.gameObject.SetActive(false);
            }
        }
        
    }

    #endregion
}