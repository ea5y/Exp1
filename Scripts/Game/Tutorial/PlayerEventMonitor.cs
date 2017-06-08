/// <summary>
/// プレイヤーのイベントモニタ
/// 
/// 2014/06/30
/// </summary>
using UnityEngine;
using System.Collections;

public class PlayerEventMonitor : Singleton<PlayerEventMonitor>
{
    #region Fields & Properties
    
    public bool IsCapturing { get; private set; }

    public float MoveTime { get; private set; }
    public int MoveFrame { get; private set; }
    public float CameraDragTime { get; private set; }
    public int CameraDragFrame { get; private set; }
    public float CameraPinchTime { get; private set; }
    public int CameraPinchFrame { get; private set; }
    public int SkillCount { get; private set; }

	private Vector3 prevRotation;
	private float prevZoom;
    private Player player;
    private Character.StateProc prevPlayerState;
    private Character.StateProc playerState;
    private CharacterCamera charaCamera;

    #endregion
    
    #region MonoBehaviour
    
    IEnumerator Start()
    {
        IsCapturing = false;

        while ((charaCamera = GameController.CharacterCamera) == null)
        {
            yield return null;
        }

        while ((player = GameController.GetPlayer()) == null)
        {
            yield return null;
        }

		Reset();
    }
    
    void LateUpdate()
    {
        if (IsCapturing)
        {
            if (GUIMoveStick.IsMove)
            {
                MoveTime += Time.deltaTime;
                MoveFrame++;
            }
                
            if (charaCamera != null)
            {
                if (charaCamera.Rotation != prevRotation)
                {
                    CameraDragTime += Time.deltaTime;
                    CameraDragFrame++;
					prevRotation = charaCamera.Rotation;
                }
                
                if (charaCamera.Zoom != prevZoom)
                {
                    CameraPinchTime += Time.deltaTime;
                    CameraPinchFrame++;
					prevZoom = charaCamera.Zoom;
                }
            }

            if (player != null)
            {
                playerState = player.State;

                if (prevPlayerState != Character.StateProc.SkillMotion && playerState == Character.StateProc.SkillMotion)
                {
                    SkillCount++;
                }

                prevPlayerState = playerState;
            }
        }
    }
    
    #endregion
    
    #region Public Methods
    
    public void StartCapture()
    {
        Reset();
        IsCapturing = true;
    }
    
    public void StopCapture()
    {
        IsCapturing = false;
    }
    
    public void Reset()
    {
        MoveTime = 0.0f;
        MoveFrame = 0;
        CameraDragTime = 0.0f;
        CameraDragFrame = 0;
        CameraPinchTime = 0.0f;
        CameraPinchFrame = 0;
        SkillCount = 0;
		
		prevRotation = charaCamera.Rotation;
		prevZoom = charaCamera.Zoom;
		prevPlayerState = player.State;
		playerState = player.State;
    }

    #endregion
}

/// <summary>
/// ファイバー用 指定時間プレイヤーが操作されるまで待つ
/// </summary>
public class WaitPlayerMove : IFiberWait
{
    private float time;
    
    public WaitPlayerMove(float time)
    {
        this.time = time;
        
        if (PlayerEventMonitor.Instance != null)
            PlayerEventMonitor.Instance.StartCapture();
    }
    
    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            if (PlayerEventMonitor.Instance != null)
            {
                if (PlayerEventMonitor.Instance.MoveTime >= time)
                {
                    PlayerEventMonitor.Instance.StopCapture();
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            return true;
        }
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 指定時間カメラがドラッグされるまで待つ
/// </summary>
public class WaitCameraDrag : IFiberWait
{
    private float time;
    
    public WaitCameraDrag(float time)
    {
        this.time = time;
        
        if (PlayerEventMonitor.Instance != null)
            PlayerEventMonitor.Instance.StartCapture();
    }
    
    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            if (PlayerEventMonitor.Instance != null)
            {
                if (PlayerEventMonitor.Instance.CameraDragTime >= time)
                {
                    PlayerEventMonitor.Instance.StopCapture();
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            return true;
        }
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 指定回数スキルが発動するまで待つ
/// </summary>
public class WaitPlayerSkill : IFiberWait
{
    private int count;
    
    public WaitPlayerSkill(int count)
    {
        this.count = count;
        
        if (PlayerEventMonitor.Instance != null)
            PlayerEventMonitor.Instance.StartCapture();
    }
    
    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            if (PlayerEventMonitor.Instance != null)
            {
                if (PlayerEventMonitor.Instance.SkillCount >= count)
                {
                    PlayerEventMonitor.Instance.StopCapture();
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            return true;
        }
    }
    
    #endregion
}
