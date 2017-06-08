using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

public class GUIBattleFieldSelect : Singleton<GUIBattleFieldSelect>
{

    #region フィールド＆プロパティ
    /// <summary>
    /// 初期化時のアクティブ状態
    /// </summary>
    [SerializeField]
    bool _isStartActive = false;
    bool IsStartActive { get { return _isStartActive; } }
    public bool IsActive { get; private set; }

    private ScoreType scoreType = ScoreType.QuickMatching;

    /// <summary>
    /// アタッチオブジェクト
    /// </summary>
    [SerializeField]
    AttachObject _attach;
    public AttachObject Attach { get { return _attach; } }
    [System.Serializable]
    public class AttachObject
    {
        public UIPlayTween rootTween;
        public GameObject fg;
    }

    #endregion

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        this._SetActive(IsStartActive);
    }

    public static void SetScoreType(ScoreType type) {
        if (Instance != null) {
            Instance.scoreType = type;
        }
    }

    public static void SetActive(bool isActive)
    {
        if (Instance != null)
        {
            Instance._SetActive(isActive);
        }
    }

    void _SetActive(bool isActive)
    {
        this.IsActive = isActive;

        // アニメーション開始
        this.Attach.fg.SetActive(this.IsActive);
        this.Attach.rootTween.Play(this.IsActive);
    }

    void StartMatch(BattleFieldType battleFieldType)
    {
        SetActive(false);
        LobbyPacket.SendMatchingEntry(battleFieldType, scoreType);
    }

    public void OnStart()
    {
        Debug.Log(UIButton.current.name);
        string id = UIButton.current.name;
        id = id.Remove(0, 6);
        StartMatch((BattleFieldType)int.Parse(id));
    }
}
