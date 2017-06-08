/// <summary>
/// チュートリアル カメラ制御
/// 
/// 2014/07/01
/// </summary>
using UnityEngine;
using System.Collections;

public class TutorialCamera : Singleton<TutorialCamera>
{
    #region Fields & Properties

    public bool IsAlive { get; private set; }
    public bool IsMove
    {
        get
        {
            return Mathf.Abs(stopAngle - currentAngle) > 0.001f;
        }
    }

    private CharacterCamera charaCamera;

    private GameObject position;
    private GameObject target;
    private float distance;
    private float stopAngle;
    private float time;

    private float currentAngle;
    private float currentVelocity;

    #endregion

    #region Static Methods

    public static void StartDirectCamera(GameObject position, GameObject target, float distance, float startAngle, float stopAngle, float time)
    {
        if (Instance != null)
            Instance.startDirectCamera(position, target, distance, startAngle, stopAngle, time);
    }

    public static void StopDirectCamera()
    {
        if (Instance != null)
            Instance.stopDirectCamera();
    }

    #endregion

    #region MonoBehaviour

    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    void Start()
    {
        charaCamera = GameController.CharacterCamera;
        IsAlive = false;
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        stopDirectCamera();
    }

    #endregion

    #region Public Methods
    #endregion

    #region Private Methods

    private void startDirectCamera(GameObject position, GameObject target, float distance, float startAngle, float stopAngle, float time)
    {
		if (position == null || target == null)
			return;

        this.position = position;
        this.target = target;
        this.distance = distance;
        this.stopAngle = stopAngle;
        this.time = time;

        this.currentAngle = startAngle;

        Vector3 vec = rotateVector(startAngle);
#if DEBUG_TUTORIAL
        Debug.Log(target.gameObject.name + ":" + vec.ToString());
#endif
        position.transform.position = target.transform.position + vec;
        charaCamera.CreateDirectCamera(this.position.transform, this.target.transform, updateDirectCamera);

        IsAlive = true;
    }
    
    private void stopDirectCamera()
    {
        IsAlive = false;
        charaCamera.RemoveDirectCamera();
    }
    
    private bool updateDirectCamera()
    {
        currentAngle = Mathf.SmoothDampAngle(currentAngle, stopAngle, ref currentVelocity, time);
        var vec = rotateVector(currentAngle);
        position.transform.position = target.transform.position + vec;

        return IsAlive;
    }

    private Vector3 rotateVector(float angle)
    {
        var vec = new Vector3(0.0f, 0.0f, distance);
        var mtx = new Matrix4x4();
        mtx.SetTRS(Vector3.zero, Quaternion.Euler(new Vector3(0.0f, angle, 0.0f)), Vector3.one);
        return mtx.MultiplyVector(vec);
    }

    #endregion
}

/// <summary>
/// ファイバー用 チュートリアルカメラの移動終了を待つ
/// </summary>
public class WaitTutorialCameraMove : IFiberWait
{
    public WaitTutorialCameraMove()
    {
    }

    #region IFiberWait

    public bool IsWait
    {
        get
        {
            if (TutorialCamera.Instance != null)
            {
                return TutorialCamera.Instance.IsMove;
            }

            return false;
        }
    }

    #endregion
}
