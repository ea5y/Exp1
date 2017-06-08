/// <summary>
/// チュートリアル スクリプト基本クラス
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public abstract class TutorialScript : MonoBehaviour
{
	#region Fields & Properties

	public bool IsFinished
	{
		get
		{
			if (scriptFiber != null) return scriptFiber.IsFinished;
			else return false;
		}
	}
	private Fiber loadFiber;
	private Fiber setupFiber;
	private Fiber startFiber;
	private Fiber scriptFiber;
	//private UIWidget.Pivot pivot;

    #endregion

    #region Public Methods
    
    public void StartScript()
    {
        startTutorial();
    }
    
    public void StopScript()
    {
        stopTutorial();
    }
    
    #endregion
    
    #region MonoBehaviour
 
    /// <summary>
    /// 更新
    /// </summary>
    protected virtual void Update()
    {
 
    }
    
    /// <summary>
    /// デストラクタ
    /// </summary>
    protected virtual void OnDestroy()
    {
        stopTutorial();
        unloadResources();
    }

    #endregion
    
    #region Virtual Methods

    /// <summary>
    /// リソースのロードファイバー
    /// </summary>
    protected virtual IEnumerator loadResources()
    {
		yield break;
	}

    /// <summary>
    /// リソースのアンロード
    /// </summary>
    protected virtual void unloadResources()
    {
		FiberController.Remove(loadFiber);
		loadFiber = null;
	}

    /// <summary>
    /// セットアップファイバー
    /// </summary>
    protected virtual IEnumerator setup()
    {
        yield break;
    }
    
    /// <summary>
    /// チュートリアルの開始
    /// </summary>
    protected virtual void startTutorial()
    {
		startFiber = FiberController.AddFiber(startTutorialFiber());
    }

    /// <summary>
    /// チュートリアルの中止
    /// </summary>
    protected virtual void stopTutorial()
    {
		FiberController.Remove(startFiber);
		startFiber = null;
		
		FiberController.Remove(setupFiber);
		setupFiber = null;
		
		FiberController.Remove(scriptFiber);
		scriptFiber = null;

		resetMessageWindow();
        deleteTutorialNotice();
    }

    #endregion
    
    #region Abstract Methods
    
    /// <summary>
    /// スクリプト
    /// </summary>
    protected abstract IEnumerator script();
    
    #endregion

    #region Protected Methods

	[System.Diagnostics.Conditional("XW_DEBUG")]
	protected void gmCommand(string command)
	{
#if XW_DEBUG
		GMCommand.CommandSelf(command);
#endif
	}

	protected void setupMessageWindow()
	{
		if (GUIMessageWindow.Instance != null)
		{
			//pivot = GUIMessageWindow.LabelPivot;
			//GUIMessageWindow.Text = "";
			//GUIMessageWindow.SetMode(GUIMessageWindow.Mode.None, GUIMessageWindow.GuideMode.None);
			//GUIMessageWindow.LabelPivot = UIWidget.Pivot.Left;
		}
	}

	protected void resetMessageWindow()
	{
		if (GUIMessageWindow.Instance != null)
		{
			//GUIMessageWindow.Text = "";
			//GUIMessageWindow.SetMode(GUIMessageWindow.Mode.None, GUIMessageWindow.GuideMode.None);
			//GUIMessageWindow.LabelPivot = pivot;
		}
	}

    protected void openWindow(GUIMessageWindow.Mode mode)
    {
		//GUIMessageWindow.SetMode(mode);
    }
    
    protected void openNextWindow()
    {
		//GUIMessageWindow.SetMode(GUIMessageWindow.Mode.Next);
    }

    protected void openOkWindow()
    {
		//GUIMessageWindow.SetMode(GUIMessageWindow.Mode.OK);
    }

    protected void openYesNoWindow()
    {
		//GUIMessageWindow.SetMode(GUIMessageWindow.Mode.YesNo);
    }

    protected void openInputWindow()
    {
		//GUIMessageWindow.SetMode(GUIMessageWindow.Mode.Input);
    }

    protected void closeWindow()
    {
		//GUIMessageWindow.SetMode(GUIMessageWindow.Mode.None);
    }
    
    protected void setGuide(GUIMessageWindow.GuideMode mode)
    {
		//GUIMessageWindow.SetMode(mode);
    }

    protected void setGuidePlate()
    {
		//GUIMessageWindow.SetMode(GUIMessageWindow.GuideMode.Plate);
    }

    protected void setGuideIcon()
    {
		//GUIMessageWindow.SetMode(GUIMessageWindow.GuideMode.Icon);
    }

    protected void resetGuide()
    {
		//GUIMessageWindow.SetMode(GUIMessageWindow.GuideMode.None);
    }

    protected void setMessage(string msg)
    {
		//GUIMessageWindow.Text = msg;
    }
    
    protected void openCharaStorage()
    {
		//GUICharacterStorage.SetActive(true);
    }
    
    protected void closeCharaStorage()
    {
		//GUICharacterStorage.SetActive(false);
    }
    
    protected AsyncOperation loadSceneAsync(string sceneName)
    {
        return SceneManager.LoadSceneAsync(sceneName);
    }
    
    protected void moveStickEnabled(bool enabled)
    {
        GUIMoveStick.IsStickEnable = enabled;
    }
    
	protected void createTutorialNotice(GameObject anchorTarget, int offsetX, int offsetY, int sizeX, int sizeY)
	{
		GUITutorialNotice.SetActive(true);
		GUITutorialNotice.SetAnchor(anchorTarget, offsetX, offsetY, sizeX, sizeY);
	}
	
	protected void deleteTutorialNotice()
	{
		GUITutorialNotice.SetActive(false);
	}
	
	protected void setMoveStickNotice()
	{
		GameObject target = GUIMoveStick.Instance.Attach.tutorialTarget.gameObject;
		createTutorialNotice(target, 0, -10, 300, 300);
	}

	protected void setSkillButtonNotice()
	{
		GameObject target = GameObject.Find("Button1_attack");
		createTutorialNotice(target, -60, -10, 300, 300);
	}

	protected void lobbyMainMenuEnabled(bool enabled)
	{
		GUILobbyMenu.IsMainButtonEnable = enabled;
	}
	
	protected void lobbyBattleMenuEnabled(bool enabled)
	{
		GUILobbyMenu.IsBattleButtonEnable = enabled;
	}
	
	protected void setLobbyBattleButtonNotice()
	{
		GameObject target = GUILobbyMenu.TutorialTarget.gameObject;
		createTutorialNotice(target, 0, -10, 300, 300);
	}
	
    #endregion

	#region Private Methods

	/// <summary>
	/// チュートリアルの開始ファイバー
	/// </summary>
	private IEnumerator startTutorialFiber()
	{
		//pivot = UIWidget.Pivot.Center;

		// ロード.
		loadFiber = FiberController.AddFiber(loadResources());
		while (FiberController.Contains(loadFiber))
		{
			yield return null;
		}
		
		if (loadFiber.IsError)
		{
			yield break;
		}
		loadFiber = null;

		// セットアップ.
		setupFiber = FiberController.AddFiber(setup());
		while (FiberController.Contains(setupFiber))
		{
			yield return null;
		}
		if (setupFiber.IsError)
		{
			yield break;
		}
		setupFiber = null;
		
		// スクリプト開始
		scriptFiber = FiberController.AddFiber(script());
	}
	
	#endregion
}

/// <summary>
///  ファイバー用 Nextボタンが押されるまで待つ
/// </summary>
public class WaitNext : IFiberWait
{
    private bool flag;
    
    public WaitNext()
    {
        flag = false;
		//GUIMessageWindow.SetDelegateNext(OnNext);
    }
    
    public void OnNext()
    {
        flag = true;
    }

    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            return !flag;
        }
    }
    
    #endregion
}

/// <summary>
///  ファイバー用 Okボタンが押されるまで待つ
/// </summary>
public class WaitOk : IFiberWait
{
    private bool flag;
    
    public WaitOk()
    {
        flag = false;
		//GUIMessageWindow.SetDelegateOK(OnOk);
    }
    
    public void OnOk()
    {
        flag = true;
    }

    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            return !flag;
        }
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 YesNoボタンが押されるまで待つ
/// </summary>
public class WaitYesNo : IFiberWait
{
    public enum Buttons
    {
        None,
        Yes,
        No
    };
    
    public Buttons Result { get; set; }
    
    public WaitYesNo()
    {
        Result = Buttons.None;
		//GUIMessageWindow.SetDelegateYesNo(OnYes, OnNo);
    }
    
    public void OnYes()
    {
        Result = Buttons.Yes;
    }
    
    public void OnNo()
    {
        Result = Buttons.No;
    }
    
    #region IFiberWait

    public bool IsWait
    {
        get
        {
            return Result == Buttons.None;
        }
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 入力が終わるまで待つ
/// </summary>
public class WaitInput : IFiberWait
{
    public string Result { get; set; }
    private bool flag;
    private System.Action onChange;
    
    public WaitInput()
    {
        Result = null;
        flag = false;
		//GUIMessageWindow.SetDelegateInput(OnOk, null, this.OnChange);
    }
    
    public void OnOk()
    {
        if (!string.IsNullOrEmpty(Result))
            flag = true;
    }
    
    public void OnChange()
    {
        Result = UIInput.current.value;
    }
    
    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            return !flag;
        }
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 指定フレーム待つ
/// </summary>
public class WaitFrames : IFiberWait
{
    private int frameCount;
        
    public WaitFrames(int frames)
    {
        this.frameCount = Time.frameCount + frames;
    }
        
    #region IFiberWait

    public bool IsWait
    {
        get
        {
            return Time.frameCount < this.frameCount;
        }
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 非同期オペレーションの終了を待つ
/// </summary>
public class WaitAsyncOperation : IFiberWait
{
    private AsyncOperation async;
        
    public WaitAsyncOperation(AsyncOperation async)
    {
        this.async = async;
    }
        
    #region IFiberWait

    public bool IsWait
    {
        get
        {
            return !async.isDone;
        }
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 任意のTween再生終了を待つ
/// </summary>
public class WaitTween : IFiberWait
{
	private UIPlayTween tween;
	private EventDelegate eventDelegate;
	private bool isFinished;

	public WaitTween(UIPlayTween tween)
	{
		this.tween = tween;
		isFinished = false;

		eventDelegate = new EventDelegate(onFinished);
		tween.onFinished.Add(eventDelegate);
	}

	private void onFinished()
	{
		isFinished = true;
	}

	#region IFiberWait

	public bool IsWait
	{
		get
		{
			if (isFinished)
			{
				tween.onFinished.Remove(eventDelegate);
			}

			return !isFinished;
		}
	}

	#endregion
}

/// <summary>
/// ファイバー用 任意のボタンがクリックされるまで待つ
/// </summary>
public class WaitButtonClick : IFiberWait
{
	private UIButton button;
	private EventDelegate eventDelegate;
	private bool isClicked;

	public WaitButtonClick(UIButton button)
	{
		this.button = button;
		isClicked = false;

		eventDelegate = new EventDelegate(onClick);
		button.onClick.Add(eventDelegate);
	}

	private void onClick()
	{
		isClicked = true;
	}

	#region IFiberWait

	public bool IsWait
	{
		get
		{
			if (isClicked)
			{
				button.onClick.Remove(eventDelegate);
			}

			return !isClicked;
		}
	}

	#endregion
}
