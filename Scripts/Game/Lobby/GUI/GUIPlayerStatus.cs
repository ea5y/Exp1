/// <summary>
/// プレイヤーステータス
/// 
/// 2014/05/27
/// </summary>
//#define DEBUG_OFFLINE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;
using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.PacketCode;

public class GUIPlayerStatus : Singleton<GUIPlayerStatus>
{
    #region Enumes
    
    [System.Serializable]
    public enum Mode
    {
        None,
        Player,
        Person
    }

	private enum LoadState
	{
		None,
		Loading,
		Finished,
		Error
	}

    #endregion

    #region Classes

    // アタッチオブジェクト
    [System.Serializable]
    public class AttachObjects
    {
        public UIPlayTween RootTween;
        public GameObject CommonStatusGroup;
        public GameObject PlayerStatusGroup;
        public GameObject PersonStatusGroup;
        public GameObject CharaBoard;
        public GameObject CharaPlate;
        public UILabel PlayerName;
        public UILabel Synonym;
        public UILabel Backbone;
        public UILabel GuildName;
        public UILabel Grade;
        public UILabel GradeTitle;
        public UISlider GradePoint;
        public UILabel GradePointMin;
        public UILabel GradePointMax;
        public UILabel BattleCount;
        public UILabel WinCount;
        public UILabel LoseCount;
        public UILabel DrawCount;
        public UILabel KillCount;
        public UILabel TowerCount;

        public UIButton AchieveButton;
        public UIButton ProfileButton;
        public UIButton SynonymButton;
        public UIButton BackboneButton;
        public UILabel Money;
        public UILabel Crystal;
        public UILabel Medal;

        public UIButton ChatButton;
        public UIButton PartyButton;
        public UIButton GuildButton;
        public UIButton FriendButton;
        public UIButton TradeButton;
        public UIButton BlackListButton;
        public UIButton ReportButton;
        public UILabel Profile;
    }
    
    #endregion

    #region Fields & Properties
    
    const string TitleMsg = "プレイヤーステータス";
    const string HelpMsg = "プレイヤーステータスです";

#if DEBUG_OFFLINE
    [SerializeField]
    private Mode defaultMode = Mode.None;
    [SerializeField]
    private AvatarType defaultAvatar = AvatarType.None;
    [SerializeField]
    private int defaultGrade = 1;
#endif

    [SerializeField]
    private AttachObjects attach;

	private CharaBoard charaBoard;
    private GameObject currentCharaBoard;
    private GameObject currentCharaPlate;

    private PlayerGradeMasterData playerGradeMasterData;
    private PlayerGradeTitleMasterData playerGradeTitleMasterData;
    private AffiliationForceMasterData affiliationForceMasterData;
    private AliasWordMasterData aliasWord1stMasterData;
    private AliasWordMasterData aliasWord2ndMasterData;
    private AliasParticleMasterData aliasParticleMastarData;

    private PlayerStatusInfo status;
    private string gradeTitle;
    private string synonym;
    private string backbone;

	private LoadState loadState;

    #endregion
    
    #region Static Methods

    public static void Setup(List<PlayerStatusInfo> statuses)
    {
        if (Instance != null)
            Instance.setup(statuses);
    }
    
    public static void SetMode(Mode mode)
    {
        if (Instance != null)
            Instance.setMode(mode);
    }

    #endregion

    #region MonoBehaviour

    protected override void Awake()
    {
        base.Awake();

		charaBoard = new CharaBoard();

		loadState = LoadState.None;
    }
    
    void Start()
    {
#if DEBUG_OFFLINE
        MasterData.Read();
        setMode(defaultMode);
#else
        init();
#endif
    }
    
    void OnDestroy()
    {
        if (currentCharaBoard != null)
        {
            GameObject.Destroy(currentCharaBoard);
            currentCharaBoard = null;
        }

        if (currentCharaPlate != null)
        {
            GameObject.Destroy(currentCharaPlate);
            currentCharaPlate = null;
        }
    }
    
    #endregion

    #region NGUI
    
    public void OnHome()
    {
        this.setMode(Mode.None);
    }

    public void OnClose()
    {
        this.setMode(Mode.None);
        GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.Top);
    }
 
    #endregion

    #region Private Methods

    private void setMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.None:
                attach.RootTween.Play(false);
                GUILobbyResident.SetActive(true);
                GUIScreenTitle.Play(false);
                GUIHelpMessage.Play(false);
                break;
            case Mode.Player:
                attach.PlayerStatusGroup.SetActive(true);
                attach.PersonStatusGroup.SetActive(false);
                initAttachObjects();
                requestPlayerStatus();
                attach.RootTween.Play(true);
                break;
            case Mode.Person:
                attach.PlayerStatusGroup.SetActive(false);
                attach.PersonStatusGroup.SetActive(true);
                initAttachObjects();
                requestPlayerStatus();
                attach.RootTween.Play(true);
                break;
        }
    }
    
    private void init()
    {
        initAttachObjects();
        setMode(Mode.None);
    }
    
    private void setup(List<PlayerStatusInfo> statuses)
    {
        if (statuses.Count > 0)
        {
            status = statuses[0];
			loadState = LoadState.Finished;
        }
		else
		{
			loadState = LoadState.Error;
			return;
		}

        setupMasterData();
        setupCharaBoard();
        setupCharaPlate();
        setupAttachObjects();

        GUILobbyResident.SetActive(false);
        GUIScreenTitle.Play(true, TitleMsg);
        GUIHelpMessage.Play(true, HelpMsg);
    }

    private void setupMasterData()
    {
        MasterData.TryGetPlayerGrade(status.PlayerGradeID, out playerGradeMasterData);
        MasterData.TryGetAffiliationForce(status.AffiliationForceId, out affiliationForceMasterData);
        playerGradeTitleMasterData = playerGradeMasterData.PlayerGradeTitle;
        MasterData.TryGetAliasWord(status.AliasWord1stId, out aliasWord1stMasterData);
        MasterData.TryGetAliasWord(status.AliasWord2ndId, out aliasWord2ndMasterData);
        MasterData.TryGetAliasParticle(status.AliasParticleId, out aliasParticleMastarData);
        gradeTitle = playerGradeTitleMasterData.Name;
        synonym = aliasWord1stMasterData.Word + aliasParticleMastarData.Word + aliasWord2ndMasterData.Word;
        backbone = affiliationForceMasterData.Name;
    }

    private void setupCharaBoard()
    {
		charaBoard.GetBoard(status.AvatarType, status.SkinId, false, prefab => { setupCharaBoardPrefab(prefab); });
	}
    
    private void setupCharaBoardPrefab(GameObject prefab)
    {
        if (currentCharaBoard != null)
		{
            GameObject.Destroy(currentCharaBoard);
			currentCharaBoard = null;
		}

		if (prefab == null)
			return;
        
		GameObject go = Object.Instantiate(prefab) as GameObject;
        go.transform.parent = attach.CharaBoard.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        
        currentCharaBoard = go;
    }

	private void setupCharaPlate()
	{
		string boardPath = CharacterPlate.GetFilePath(status.AvatarType);
		if (!string.IsNullOrEmpty(boardPath))
		{
			string bundlePath = string.Empty;
			string assetPath = boardPath;
			//GameConstant.EffectPath.ConvertAssetBundlePath(ref bundlePath, ref assetPath);
			ResourceLoad.Instantiate(bundlePath, assetPath, null, obj => { this.setupCharaPlatePrefab(obj); });
		}
	}

    private void setupCharaPlatePrefab(GameObject go)
    {
        if (currentCharaPlate != null)
            GameObject.Destroy(currentCharaPlate);
        
        go.transform.parent = attach.CharaPlate.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        
        currentCharaPlate = go;
    }
    
    private void initAttachObjects()
    {
        if (attach.PlayerName != null)
            attach.PlayerName.text = "";
        if (attach.Synonym != null)
            attach.Synonym.text = "";
        if (attach.Backbone != null)
            attach.Backbone.text = "";
        if (attach.GuildName != null)
            attach.GuildName.text = "";
        if (attach.Grade != null)
            attach.Grade.text = "グレード";
        if (attach.GradeTitle != null)
            attach.GradeTitle.text = "";
        if (attach.GradePoint != null)
            attach.GradePoint.value = 0.0f;
        if (attach.GradePointMin != null)
            attach.GradePointMin.text = "";
        if (attach.GradePointMax != null)
            attach.GradePointMax.text = "";
        if (attach.BattleCount != null)
            attach.BattleCount.text = "";
        if (attach.WinCount != null)
            attach.WinCount.text = "";
        if (attach.LoseCount != null)
            attach.LoseCount.text = "";
        if (attach.DrawCount != null)
            attach.DrawCount.text = "";
        if (attach.KillCount != null)
            attach.KillCount.text = "";
        if (attach.TowerCount != null)
            attach.TowerCount.text = "";

        if (attach.AchieveButton != null)
            attach.AchieveButton.isEnabled = false;
        if (attach.ProfileButton != null)
            attach.ProfileButton.isEnabled = false;
        if (attach.SynonymButton != null)
            attach.SynonymButton.isEnabled = false;
        if (attach.BackboneButton != null)
            attach.BackboneButton.isEnabled = false;
        if (attach.Money != null)
            attach.Money.text = "";
        if (attach.Crystal != null)
            attach.Crystal.text = "";
        if (attach.Medal != null)
            attach.Medal.text = "";

        if (attach.ChatButton != null)
            attach.ChatButton.isEnabled = false;
        if (attach.PartyButton != null)
            attach.PartyButton.isEnabled = false;
        if (attach.GuildButton != null)
            attach.GuildButton.isEnabled = false;
        if (attach.FriendButton != null)
            attach.FriendButton.isEnabled = false;
        if (attach.TradeButton != null)
            attach.TradeButton.isEnabled = false;
        if (attach.BlackListButton != null)
            attach.BlackListButton.isEnabled = false;
        if (attach.ReportButton != null)
            attach.ReportButton.isEnabled = false;
        if (attach.Profile != null)
            attach.Profile.text = "";
    }

    private void setupAttachObjects()
    {
        if (attach.PlayerName != null)
            attach.PlayerName.text = status.Name;
        if (attach.Synonym != null)
            attach.Synonym.text = synonym;
        if (attach.Backbone != null)
            attach.Backbone.text = backbone;
        if (attach.GuildName != null)
            attach.GuildName.text = status.GuildName;
        if (attach.Grade != null)
            attach.Grade.text = string.Format("グレード {0:D2}", status.PlayerGradeID);
        if (attach.GradeTitle != null)
            attach.GradeTitle.text = gradeTitle;
        if (attach.GradePoint != null)
            attach.GradePoint.value = (float)(playerGradeMasterData.UpgradePoint - status.GradePoint) / (float)playerGradeMasterData.UpgradePoint;
        if (attach.GradePointMin != null)
            attach.GradePointMin.text = string.Format("{0:D}", -playerGradeMasterData.StartPoint);
        if (attach.GradePointMax != null)
            attach.GradePointMax.text = string.Format("+{0:D}", playerGradeMasterData.UpgradePoint - playerGradeMasterData.StartPoint);
        if (attach.BattleCount != null)
            attach.BattleCount.text = string.Format("{0:D}", status.BattleCount);
        if (attach.WinCount != null)
            attach.WinCount.text = string.Format("{0:D}", status.WinCount);
        if (attach.LoseCount != null)
            attach.LoseCount.text = string.Format("{0:D}", status.LoseCount);
        if (attach.DrawCount != null)
            attach.DrawCount.text = string.Format("{0:D}", status.DrawCount);
        if (attach.KillCount != null)
            attach.KillCount.text = string.Format("{0:D}", status.KillCount);
        if (attach.TowerCount != null)
            attach.TowerCount.text = string.Format("{0:D}", status.TowerDefeatCount);
        if (attach.AchieveButton != null)
            attach.AchieveButton.isEnabled = false;
        if (attach.ProfileButton != null)
            attach.ProfileButton.isEnabled = false;
        if (attach.SynonymButton != null)
            attach.SynonymButton.isEnabled = false;
        if (attach.BackboneButton != null)
            attach.BackboneButton.isEnabled = false;
        if (attach.Money != null)
            attach.Money.text = string.Format("{0:D}", status.GameMoney);
        if (attach.Crystal != null)
            attach.Crystal.text = string.Format("{0:D}", status.AccountMoney);
        if (attach.Medal != null)
        {
            attach.Medal.text = string.Format("{0:D}", "");
            //attach.Medal.text = string.Format("{0:D}", status.PlayerMedal);	// パケットから項目を削除
        }

        if (attach.ChatButton != null)
            attach.ChatButton.isEnabled = false;
        if (attach.PartyButton != null)
            attach.PartyButton.isEnabled = false;
        if (attach.GuildButton != null)
            attach.GuildButton.isEnabled = false;
        if (attach.FriendButton != null)
            attach.FriendButton.isEnabled = false;
        if (attach.TradeButton != null)
            attach.TradeButton.isEnabled = false;
        if (attach.BlackListButton != null)
            attach.BlackListButton.isEnabled = false;
        if (attach.ReportButton != null)
            attach.ReportButton.isEnabled = false;
        if (attach.Profile != null)
            attach.Profile.text = status.Profile;
    }

   private void requestPlayerStatus()
    {
#if DEBUG_OFFLINE
        var pspp = new PlayerStatusPacketParameter(
            0,
            "ななしのごんべえ",
            (int)defaultAvatar,
            "ギルド名",
            defaultGrade,
            500,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            "プロフィール",
            0,
            0,
            0,
            1,
            3,
            2,
            1,
            1,
            false);
        var list = new List<PlayerStatusInfo>();
        list.Add(new PlayerStatusInfo(pspp));

		loadState = LoadState.Finished;
        setup(list);
#else
        if (loadState != LoadState.Loading)
        {
            LobbyPacket.SendPlayerStatus(GameController.GetPlayer().InFieldId);
			loadState = LoadState.Loading;
        }
#endif
    }

    #endregion
}
