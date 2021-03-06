/// <summary>
/// スキルUI
/// 
/// 2014/07/24
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;

public class GUISkill : Singleton<GUISkill>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	AttachObject _attach;
	AttachObject Attach { get { return _attach; } }
	[System.Serializable]
	public class AttachObject
	{
		public UIPlayTween rootTween;
		public GUITargetButton targetButton;
		public List<GUISkillButton> skillButtonList;
	    public GameObject linkTip;
	}

    public void ShowLinkTip(bool pActive)
    {
        if (pActive != Attach.linkTip.activeSelf)
        {
            Attach.linkTip.SetActive(pActive);
        }
    }

	bool IsActive { get; set; }

	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.IsActive = false;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();

		// 表示設定
		this._SetActive(this.IsStartActive);
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 表示自体のオフ
	/// </summary>
	public static void SetActive(bool isActive)
	{
		if (Instance != null) Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;

		if (isActive)
		{
			if(PlayerManager.Instance)
			{
				var player = PlayerManager.Instance.Player;
				if(player != null)
				{
					ChangeButtonName(player.AvatarType, player.Level);
				}
				else
				{
					var pInfo = NetworkController.ServerValue.PlayerInfo;
					if(pInfo != null)
					{
						ChangeButtonName((AvatarType)pInfo.Id, pInfo.Level);
					}
				}
			}
		}

		// アニメーション開始
		this.Attach.rootTween.Play(this.IsActive);
	}
	#endregion

	#region 有効設定
	/// <summary>
	/// ボタンを押せなくするかどうかの設定
	/// </summary>
	public static void SetEnable(bool isEnable)
	{
		if (Instance != null) Instance._SetEnable(isEnable);
	}
	void _SetEnable(bool isEnable)
	{
		if (this.Attach.targetButton != null && this.Attach.targetButton.Button != null)
			this.Attach.targetButton.Button.enabled = isEnable;

		foreach (var t in this.Attach.skillButtonList)
			t.IsActive = isEnable;
	}
	#endregion

	#region クールタイム
	/// <summary>
	/// クールタイムクリア
	/// </summary>
	public static void ClearCoolTime()
	{
		if (Instance != null) Instance._ClearCoolTime();
	}
	void _ClearCoolTime()
	{
		foreach (var t in this.Attach.skillButtonList)
			t.CoolTime = 0f;
	}
	/// <summary>
	/// クールタイムリセット,Except SP Skill
	/// </summary>
	public static void ResetCoolTime(AvatarType avatarType, int level)
	{
		if (Instance != null) Instance._ResetCoolTime(avatarType, level);
	}
	void _ResetCoolTime(AvatarType avatarType, int level)
	{
		foreach (var t in this.Attach.skillButtonList)
		{
            if (t.SkillButtonType == SkillButtonType.SpecialSkill) {
                t.ResetGauge();
            } else {
                t.CoolTime = 0;
            }			
		}
	}
	#endregion

	#region ボタン名変更
	/// <summary>
	/// ボタン名変更
	/// </summary>
	public static void ChangeButtonName(AvatarType avatarType, int lv)
	{
		if (Instance != null) Instance._ChangeButtonName(avatarType, lv);
	}
	void _ChangeButtonName(AvatarType avatarType, int lv)
	{
		foreach (var t in this.Attach.skillButtonList)
		{
			t.ChangeButtonName(avatarType, lv);
		}
	}
	#endregion

	#region 強制的にボタンを離した状態にする
	public static void ForceReleaseButtons()
	{
		if (Instance != null) Instance._ForceReleaseButtons();
	}
	void _ForceReleaseButtons()
	{
		foreach( var btn in this.Attach.skillButtonList )
		{
			btn.IsDown = false;
		}
	}
	#endregion

	#region ボタンの押下状態を即座に確認する.
	public static void ForceCheckPress()
	{
		if (Instance != null) Instance._ForceCheckPress();
	}
	void _ForceCheckPress()
	{
		foreach( var btn in this.Attach.skillButtonList )
		{
			btn.ForceCheckPress();
		}
	}
    #endregion

    #region Find a button by condition
    public static GUISkillButton FindButton(System.Func<GUISkillButton, bool> pred) {
        if (Instance != null) return Instance._FindButton(pred);
        return null;
    }

    GUISkillButton _FindButton(System.Func<GUISkillButton, bool> pred) {
        int count = this.Attach.skillButtonList.Count;
        for (int i = 0; i < count; ++i) {
            if (pred(this.Attach.skillButtonList[i])) {
                return this.Attach.skillButtonList[i];
            }
        }
        return null;
    }
    #endregion

    #region デバッグ
#if UNITY_EDITOR && XW_DEBUG

    [System.Serializable]
	public class DebugParam
	{
		public bool execute;
		public Scm.Common.GameParameter.Language language = Scm.Common.GameParameter.Language.Japanese;
		public AvatarType avatarType;
	}

	[SerializeField]
	private DebugParam debug = new DebugParam();
	private bool isReadMasterData;

	private void DebugUpdate()
	{
		var t = this.debug;
		if (!t.execute) return;
		t.execute = false;

		if (!this.isReadMasterData)
		{
			MasterData.Read();
			this.isReadMasterData = true;
		}

		// 言語設定
		Scm.Common.Utility.Language = t.language;

		ChangeButtonName(t.avatarType, 1);
	}
	private void OnValidate()
	{
		if (Application.isPlaying) this.DebugUpdate();
	}
#endif
	#endregion
}
