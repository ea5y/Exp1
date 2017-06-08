/// <summary>
/// チュートリアル メイン処理
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;

public class TutorialMain : SceneMain<TutorialMain>
{
    #region Enumes

    public enum Mode
    {
		None,
        Introdaction,
        Lobby,
        Battle,
        Result,
        Conclusion,
    }
    
    #endregion
    
    #region Classes & Structs

	private class TutorialInfo
	{
		public string SceneName { get; private set; }
		public Mode NextMode { get; private set; }

		public TutorialInfo(string sceneName, Mode nextMode)
		{
			this.SceneName = sceneName;
			this.NextMode = nextMode;
		}
	}

	[System.Serializable]
	public class TutorialPrefab
	{
		public TutorialLobby tutorialLobby;
		public TutorialBattle tutorialBattle;
		public TutorialResult tutorialResult;
		public TutorialConclusion tutorialConclusion;
	}

    #endregion
    
    #region Fields & Properties
	const string SceneName = SceneController.SceneName.Tutorial;

	private Dictionary<Mode, TutorialInfo> tutorialTable;
#if XW_DEBUG
	private Mode currentMode;
#endif

	[SerializeField]
	private TutorialIntrodaction tutorialIntrodaction;

	[SerializeField]
	private TutorialPrefab tutorialPrefab;

#endregion
    
    #region Static Methods

	static public void LoadScene()
	{
		SceneController.FadeSceneChange(SceneName);
	}
	public void StartTutorial(Mode type)
	{
		if (Instance != null)
			Instance.startTutorial(type);
	}

    #endregion

    #region MonoBehaviour
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

		tutorialTable = new Dictionary<Mode, TutorialInfo>()
		{
			{ Mode.None, new TutorialInfo("", Mode.Introdaction) },
			{ Mode.Introdaction, new TutorialInfo("ScmTutorial", Mode.Lobby) },
			{ Mode.Lobby, new TutorialInfo("ScmLobby", Mode.Battle) },
			{ Mode.Battle, new TutorialInfo("ScmBattle", Mode.Result) },
			{ Mode.Result, new TutorialInfo("ScmResult", Mode.Conclusion) },
			{ Mode.Conclusion, new TutorialInfo("ScmLobby", Mode.None) },
		};

#if XW_DEBUG
		currentMode = Mode.None;
#endif
	}

	#region ISceneMain
	public override bool OnNetworkDisconnect()
	{
		return true;
	}

	public override void OnNetworkDisconnectByServer() { }
	#endregion
	/// <summary>
	/// 初期化
	/// </summary>
	void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}
    /// <summary>
    /// 更新
    /// </summary>
    void Update()
    {

    }
    
    /// <summary>
    /// デストラクタ
    /// </summary>
    void OnDestroy()
    {

    }

    /// <summary>
    /// シーンロード時
    /// </summary>
    /// <param name="level">Level.</param>
    void OnLevelWasLoaded(int level)
	{
#if XW_DEBUG
		if (ScmParam.Debug.File.IsTutorial)
		{
			if (tutorialTable[tutorialTable[currentMode].NextMode].SceneName == SceneManager.GetActiveScene().name)
			{
				startTutorial(tutorialTable[currentMode].NextMode);
			}
		}
#endif
	}
    
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
	
	private bool startTutorial(Mode mode)
	{
		if (mode == Mode.None)
			return false;

		TutorialInfo info;
		if (!tutorialTable.TryGetValue(mode, out info))
			return false;

		TutorialScript script = null;
		switch(mode)
		{
		case Mode.Introdaction:
			script = this.tutorialIntrodaction;
			break;
		case Mode.Lobby:
			script = Object.Instantiate(this.tutorialPrefab.tutorialLobby) as TutorialScript;
			break;
		case Mode.Battle:
			script = Object.Instantiate(this.tutorialPrefab.tutorialBattle) as TutorialScript;
			break;
		case Mode.Result:
			script = Object.Instantiate(this.tutorialPrefab.tutorialResult) as TutorialScript;
			break;
		case Mode.Conclusion:
			script = Object.Instantiate(this.tutorialPrefab.tutorialConclusion) as TutorialScript;
			break;
		}

		if (script == null)
		{
			Debug.LogError("Tutorial" + mode.ToString() + " is not exist.");
			return false;
		}

		if (SceneManager.GetActiveScene().name != info.SceneName)
		{
			Debug.LogError("Tutorial" + mode.ToString() + ": Scene is mismatch. (" + info.SceneName + ").");
			return false;
		}

		script.StartScript();
#if XW_DEBUG
		currentMode = mode;
#endif

		return true;
	}
	
	#endregion
}
