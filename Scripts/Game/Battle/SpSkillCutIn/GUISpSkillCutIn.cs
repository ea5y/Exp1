/// <summary>
/// 超必殺技カットイン
/// 
/// 2015/12/03
/// </summary>
using UnityEngine;
using System.Collections;

public class GUISpSkillCutIn : Singleton<GUISpSkillCutIn>
{
    #region フィールド&プロパティ
    /// <summary>
    /// カットイン表示
    /// </summary>
    [SerializeField]
    private XUI.SpSkillCutIn.SpSkillCutInView cutInView = null;

    /// <summary>
    /// カットインデータ
    /// </summary>
    [SerializeField]
    private XUI.SpSkillCutIn.Model cutInModel = null;
 
    /// <summary>
    /// コントローラ
    /// </summary>
    private XUI.SpSkillCutIn.IController controller;
    #endregion

    #region 初期化
    protected override void Awake()
    {
     	base.Awake();
        Construct();
    }

    private void Construct()
    {
        // コントローラ生成
        this.controller = new XUI.SpSkillCutIn.Controller(this.cutInModel, this.cutInView);
        // 開始時は非表示に設定
        this.gameObject.SetActive(false);
    }
    #endregion

    #region カットイン再生
    /// <summary>
    /// カットインを再生させる
    /// </summary>
    /// <param name="avatarType"></param>
    /// <param name="skillName"></param>
    public static void PlayCutIn(AvatarType avatarType, int skinId, string skillName)
    {
        if (Instance == null) { return; }
        Instance.controller.Play(avatarType, skinId, skillName);
    }
    #endregion

    #region 更新
    void Update ()
    {
        if (this.controller == null) { return; }
        this.controller.Update();
    }
    #endregion


    #region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	GUIDebugParam debugParam = new GUIDebugParam();
	GUIDebugParam DebugParam { get { return debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public GUIDebugParam()
		{
			this.AddEvent(this.CutIn);
		}

		[SerializeField]
		private CutInEvent cutIn = new CutInEvent();
		public CutInEvent CutIn { get { return cutIn; } }
		[System.Serializable]
		public class CutInEvent : IDebugParamEvent
		{
			public event System.Action<AvatarType, int, bool, string> Execute = delegate { };
			[SerializeField]
			private bool execute = false;
			public AvatarType avatarType = AvatarType.Begin;
            public int skinId = 0;
			[Range(1, 5)]
			public int charaLv = 1;
			[Tooltip("手動でスキル名を表示したい場合はチェックを入れてください")]
			public bool isSkillNameManualInput = false;
			[Tooltip("isSkillNameManualInputにチェックが入っている場合のみこちらのスキル名が表示されます")]
			public string skillNamel;

			public void Update()
			{
				if(this.execute)
				{
					this.execute = false;
					this.Execute(this.avatarType, this.charaLv, this.isSkillNameManualInput, this.skillNamel);
				}
			}
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += () => { this.cutInView.SetActive(false); };
		d.ExecuteActive += () => { this.cutInView.SetActive(true); };

		d.CutIn.Execute += (avatarType, charaLv, isSkillNameManualInput, skillNamel) =>
			{
				d.ReadMasterData();
				int skillID = GameGlobal.GetSkillID(d.CutIn.avatarType, SkillButtonType.SpecialSkill, d.CutIn.charaLv);
				Scm.Common.Master.SkillMasterData skillData;
				if (MasterData.TryGetSkill(skillID, out skillData))
				{
					string skillName = (d.CutIn.isSkillNameManualInput) ? d.CutIn.skillNamel : skillData.DisplayName;
					PlayCutIn(d.CutIn.avatarType, d.CutIn.skinId, skillName);
				}
				else
				{
					Debug.LogWarning("NotFound AvatarType " + d.CutIn.avatarType);
				}
			};
	}
	bool isDebugInit = false;
	void DebugUpdate()
	{
		if(!this.isDebugInit)
		{
			this.isDebugInit = true;
			this.DebugInit();
		}
		this.DebugParam.Update();
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.DebugUpdate();
		}
	}
#endif
	#endregion
}
