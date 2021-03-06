/// <summary>
/// ボタン連打
/// 
/// 2013/01/23
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIRepeatButton : MonoBehaviour
{
    #region Lee add(Demo)
    public bool ReleaseButton = false;
    #endregion
    #region フィールド＆プロパティ
    public GameObject target;
    public string functionName;
    public float repeatTimer;

    public float Counter { get; private set; }
    public bool IsDown { get; set; }
    public int TouchFingerID { get; set; }
    #endregion

    #region 初期化
    protected virtual void Start()
    {
        if (this.target == null)
            this.target = this.gameObject;
    }
    #endregion

    #region 更新
    protected virtual void Update()
    {
        this.RepeatCounter(true);
        if (!ReleaseButton)
        {
            this.CheckTouchRelease();
        }
    }
    protected void RepeatCounter(bool isEnable)
    {
        if (isEnable)
        {
            if (0f < this.Counter)
            {
                this.Counter -= Time.deltaTime;
            }
            else
            {
                this.Counter = this.repeatTimer;
                this.ForceCheckPress();
            }
        }
    }
    public void ForceCheckPress()
    {
        if (this.IsDown)
        {
            if (ReleaseButton)
            {
                this.IsDown = false;
            }
            this.Send();
        }
    }

    /// <summary>
    /// フレームが飛ぶとリリースイベントが来ない時のチェック用
    /// </summary>
    private void CheckTouchRelease()
    {
        if (IsDown)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            // タッチIDをマウスIDに変換
            int touchID = Mathf.Abs(this.TouchFingerID + 1);

            // マウス判定
            if (Input.GetMouseButtonDown(touchID) || Input.GetMouseButton(touchID))
                return;
#endif
            // タッチ離し判定
            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.touches[i];
                if (touch.fingerId == this.TouchFingerID)
                    return;
            }

            this.OnPress(false);
        }
    }
    #endregion

    #region NGUI
    void OnPress(bool isDown)
    {
        if (isDown)
        {
//            DetectRay.Instance.Detect();
        }
        if (ReleaseButton)
        {
            this.IsDown = !isDown;
            this.Counter = this.repeatTimer;
            this.ForceCheckPress();
            this.TouchFingerID = (this.IsDown) ? UICamera.currentTouchID : -100;
        }
        else
        {
            this.IsDown = isDown;
            this.Counter = this.repeatTimer;
            this.ForceCheckPress();
            this.TouchFingerID = (this.IsDown) ? UICamera.currentTouchID : -100;
        }
    }
    protected virtual void Send()
    {
        target.SendMessage(functionName, this.gameObject, SendMessageOptions.DontRequireReceiver);
    }
    #endregion
}
