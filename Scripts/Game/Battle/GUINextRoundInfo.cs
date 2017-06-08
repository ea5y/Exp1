using UnityEngine;
using System.Collections;

public class GUINextRoundInfo : Singleton<GUINextRoundInfo> {

    #region Attack Object
    [System.Serializable]
    public class AttachObject {
        public Transform root;
        public UILabel countDownLabel;
    }
    #endregion

    /// <summary>
    /// アタッチオブジェクト
    /// </summary>
    [SerializeField]
    private AttachObject attach;
    public AttachObject Attach { get { return attach; } }

    private float _remainingSecond = 0;

    protected override void Awake() {
        base.Awake();
        gameObject.SetActive(false);
    }

    public static void Show(bool active) {
        if (Instance != null) {
            Instance.gameObject.SetActive(active);
        }
    }

    public static void UpdateRemainingTime(float remainingSecond) {
        if (Instance != null) {
            Instance._remainingSecond = remainingSecond;
        }
    }

    void Update() {
        _remainingSecond -= Time.deltaTime;
        _remainingSecond = Mathf.Max(0, _remainingSecond);
        if (Attach.countDownLabel != null) {
            Attach.countDownLabel.text = string.Format("{0}", Mathf.RoundToInt(_remainingSecond));
        }
    }

}
