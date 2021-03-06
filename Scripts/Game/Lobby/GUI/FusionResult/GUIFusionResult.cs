/// <summary>
/// 合成結果
/// 
/// 2016/05/17
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XUI.FusionResult;

public class GUIFusionResult : Singleton<GUIFusionResult>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	private FusionResultView _viewAttach = null;
	private FusionResultView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;
	private bool IsStartActive { get { return _isStartActive; } }

	/// <summary>
	/// フォーマットアタッチ
	/// </summary>
	[SerializeField]
	private FormatAttachObject _formatAttach = null;
	private FormatAttachObject FormatAttach { get { return _formatAttach; } }
	[Serializable]
	public class FormatAttachObject
	{
		[SerializeField]
		private string _total = "{0}";
		public string Total { get { return _total; } }

		[SerializeField]
		private string _base = "{0}";
		public string Base { get { return _base; } }

		[SerializeField]
		private string _synchro = "{0}%";
		public string Synchro { get { return _synchro; } }

		[SerializeField]
		private string _slot = "{0}";
		public string Slot { get { return _slot; } }

		[SerializeField]
		private string _up = "{0}";
		public string Up { get { return _up; } }
	}

	/// <summary>
	/// コントローラ
	/// </summary>
	private IController Controller { get; set; }

	/// <summary>
	/// キャラボード
	/// </summary>
	private CharaBoard CharaBoard { get { return ScmParam.Lobby.CharaBoard; } }
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
	}
	void Start()
	{
		this.Constrcut();
		// 初期化アクティブ設定
		this.SetActive(this.IsStartActive, true);
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Controller = null;
	}
	private void Constrcut()
	{
		// モデル生成
		var model = new Model();
		if (this.FormatAttach != null)
		{
			model.TotalStatusFormat = this.FormatAttach.Total;
			model.BaseStatusFormat = this.FormatAttach.Base;
			model.SynchroFormat = this.FormatAttach.Synchro;
			model.SlotFormat = this.FormatAttach.Slot;
			model.UpFormat = this.FormatAttach.Up;
		}

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラ生成
		var controller = new Controller(model, view, this.CharaBoard);
		this.Controller = controller;
	}

	/// <summary>
	/// 破棄
	/// </summary>
	void OnDestroy()
	{
		if (this.Controller != null)
		{
			this.Controller.Dispose();
		}
	}
	#endregion

	#region アクティブ設定
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null) Instance.SetActive(false, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open(SetupParam param)
	{
		if (Instance != null)
		{
			Instance._Open(param);
		}
	}
	private void _Open(SetupParam param)
	{
		this.Setup(param);
		this.SetActive(true, false);
	}
	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen()
	{
		if (Instance != null) Instance.SetActive(true, false);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	void SetActive(bool isActive, bool isTweenSkip)
	{
		if (this.Controller != null)
		{
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}
	#endregion

	#region 設定
	/// <summary>
	/// 設定
	/// </summary>
	private void Setup(SetupParam param)
	{
		if (this.Controller == null) { return; }
		this.Controller.Setup(param);
	}
	#endregion


	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();
	GUIDebugParam DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public GUIDebugParam()
		{
			this.AddEvent(this.Result);
		}

		[SerializeField]
		ResultEvent _result = new ResultEvent();
		public ResultEvent Result { get { return _result; } }
		[System.Serializable]
		public class ResultEvent : IDebugParamEvent
		{
			public event System.Action<SetupParam> Execute = delegate { };
			[SerializeField]
			bool execute = false;
			[SerializeField]
			bool isDummy = true;
			[SerializeField]
			private Param param = new Param();
			[Serializable]
			public class Param
			{
				public AvatarType avatarType = AvatarType.Begin;
				public int rankBefore = 0;
				public int rankAfter = 0;
				public int levelBefore = 0;
				public int levelAfter = 0;
				public int exp = 0;
				public int totalExp = 0;
				public int nextLvTotalExp = 0;
				public int synchroRemainBefore = 0;
				public int synchroRemainAfter = 0;
				public int hitPointBefore = 0;
				public int hitPointAfter = 0;
				public int hitPointBaseBefore = 0;
				public int hitPointBaseAfter = 0;
				public int synchroHitPointBefore = 0;
				public int synchroHitPointAfter = 0;
				public int slotHitPointBefore = 0;
				public int slotHitPointAfter = 0;
				public int attackBefore = 0;
				public int attackAfter = 0;
				public int attackBaseBefore = 0;
				public int attackBaseAfter = 0;
				public int synchroAttackBefore = 0;
				public int synchroAttackAfter = 0;
				public int slotAttackBefore = 0;
				public int slotAttackAfter = 0;
				public int defenseBefore = 0;
				public int defenseAfter = 0;
				public int defenseBaseBefore = 0;
				public int defenseBaseAfter = 0;
				public int synchroDefenseBefore = 0;
				public int synchroDefenseAfter = 0;
				public int slotDefenseBefore = 0;
				public int slotDefenseAfter = 0;
				public int extraBefore = 0;
				public int extraAfter = 0;
				public int extraBaseBefore = 0;
				public int extraBaseAfter = 0;
				public int synchroExtraBefore = 0;
				public int synchroExtraAfter = 0;
				public int slotExtraBefore = 0;
				public int slotExtraAfter = 0;
				public Scm.Common.GameParameter.PowerupResult powerupResultType;
				public bool isPowerupResultEnable = false;

				public void SetDummy()
				{
					this.avatarType = (AvatarType)UnityEngine.Random.Range((int)AvatarType.Begin, (int)AvatarType.End + 1);
					this.rankBefore = UnityEngine.Random.Range(1, 6);
					this.rankAfter = UnityEngine.Random.Range(1, 6);
					this.levelBefore = UnityEngine.Random.Range(1, 100);
					this.levelAfter = UnityEngine.Random.Range(1, 100);
					this.totalExp = UnityEngine.Random.Range(0, 10000);
					this.exp = UnityEngine.Random.Range(this.totalExp, 10000);
					this.nextLvTotalExp = UnityEngine.Random.Range(this.exp, 10000);
					this.synchroRemainBefore = UnityEngine.Random.Range(0, MasterDataCommonSetting.Fusion.MaxSynchroCount + 1);
					this.synchroRemainAfter = UnityEngine.Random.Range(0, MasterDataCommonSetting.Fusion.MaxSynchroCount + 1);
					this.hitPointBefore = UnityEngine.Random.Range(0, 10000);
					this.hitPointAfter = UnityEngine.Random.Range(0, 10000);
					this.hitPointBaseBefore = UnityEngine.Random.Range(0, 10000);
					this.hitPointBaseAfter = UnityEngine.Random.Range(0, 10000);
					this.synchroHitPointBefore = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxHitPoint + 1);
					this.synchroHitPointAfter = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxHitPoint + 1);
					this.slotHitPointBefore = UnityEngine.Random.Range(0, 10000);
					this.slotHitPointAfter = UnityEngine.Random.Range(0, 10000);
					this.attackBefore = UnityEngine.Random.Range(0, 10000);
					this.attackAfter = UnityEngine.Random.Range(0, 10000);
					this.attackBaseBefore = UnityEngine.Random.Range(0, 10000);
					this.attackBaseAfter = UnityEngine.Random.Range(0, 10000);
					this.synchroAttackBefore = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxAttack + 1);
					this.synchroAttackAfter = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxAttack + 1);
					this.slotAttackBefore = UnityEngine.Random.Range(0, 10000);
					this.slotAttackAfter = UnityEngine.Random.Range(0, 10000);
					this.defenseBefore = UnityEngine.Random.Range(0, 10000);
					this.defenseAfter = UnityEngine.Random.Range(0, 10000);
					this.defenseBaseBefore = UnityEngine.Random.Range(0, 10000);
					this.defenseBaseAfter = UnityEngine.Random.Range(0, 10000);
					this.synchroDefenseBefore = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxDefense + 1);
					this.synchroDefenseAfter = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxDefense + 1);
					this.slotDefenseBefore = UnityEngine.Random.Range(0, 10000);
					this.slotDefenseAfter = UnityEngine.Random.Range(0, 10000);
					this.extraBefore = UnityEngine.Random.Range(0, 10000);
					this.extraAfter = UnityEngine.Random.Range(0, 10000);
					this.extraBaseBefore = UnityEngine.Random.Range(0, 10000);
					this.extraBaseAfter = UnityEngine.Random.Range(0, 10000);
					this.synchroExtraBefore = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxExtra + 1);
					this.synchroExtraAfter = UnityEngine.Random.Range(0, CharaInfo.SynchroMaxExtra + 1);
					this.slotExtraBefore = UnityEngine.Random.Range(0, 10000);
					this.slotExtraAfter = UnityEngine.Random.Range(0, 10000);
					this.powerupResultType = (Scm.Common.GameParameter.PowerupResult)UnityEngine.Random.Range((int)Scm.Common.GameParameter.PowerupResult.Good, (int)Scm.Common.GameParameter.PowerupResult.SuperSuccess + 1);
					this.isPowerupResultEnable = (UnityEngine.Random.Range(0, 2) == 1) ? true : false;
				}

				public SetupParam GetConvertSetupParam()
				{
					var param = new SetupParam();
					param.AvatarType = this.avatarType;
					param.RankBefore = this.rankBefore;
					param.RankAfter = this.rankAfter;
					param.LevelBefore = this.levelBefore;
					param.LevelAfter = this.levelAfter;
					param.Exp = this.exp;
					param.TotalExp = this.totalExp;
					param.NextLvTotalExp = this.nextLvTotalExp;
					param.SynchroRemainBefore = this.synchroRemainBefore;
					param.SynchroRemainAfter = this.synchroRemainAfter;
					param.HitPointBefore = this.hitPointBefore;
					param.HitPointAfter = this.hitPointAfter;
					param.HitPointBaseBefore = this.hitPointBaseBefore;
					param.HitPointBaseAfter = this.hitPointBaseAfter;
					param.SynchroHitPointBefore = this.synchroHitPointBefore;
					param.SynchroHitPointAfter = this.synchroHitPointAfter;
					param.SlotHitPointBefore = this.slotHitPointBefore;
					param.SlotHitPointAfter = this.slotHitPointAfter;
					param.AttackBefore = this.attackBefore;
					param.AttackAfter = this.attackAfter;
					param.AttackBaseBefore = this.attackBaseBefore;
					param.AttackBaseAfter = this.attackBaseAfter;
					param.SynchroAttackBefore = this.synchroAttackBefore;
					param.SynchroAttackAfter = this.synchroAttackAfter;
					param.SlotAttackBefore = this.slotAttackBefore;
					param.SlotAttackAfter = this.slotAttackAfter;
					param.DefenseBefore = this.defenseBefore;
					param.DefenseAfter = this.defenseAfter;
					param.DefenseBaseBefore = this.defenseBaseBefore;
					param.DefenseBaseAfter = this.defenseBaseAfter;
					param.SynchroDefenseBefore = this.synchroDefenseBefore;
					param.SynchroDefenseAfter = this.synchroDefenseAfter;
					param.SlotDefenseBefore = this.slotDefenseBefore;
					param.SlotDefenseAfter = this.slotDefenseAfter;
					param.ExtraBefore = this.extraBefore;
					param.ExtraAfter = this.extraAfter;
					param.ExtraBaseBefore = this.extraBaseBefore;
					param.ExtraBaseAfter = this.extraBaseAfter;
					param.SynchroExtraBefore = this.synchroDefenseBefore;
					param.SynchroExtraAfter = this.synchroDefenseAfter;
					param.SlotExtraBefore = this.slotExtraBefore;
					param.SlotExtraAfter = this.slotExtraAfter;
					param.PowerupResult = this.powerupResultType;
					param.IsPowerupResultEnable = this.isPowerupResultEnable;

					return param;
				}
			}

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					if (this.isDummy)
					{
						this.param.SetDummy();
					}
					SetupParam param = this.param.GetConvertSetupParam();
					this.Execute(param);
				}
			}
		}
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () =>
		{
			d.ReadMasterData();
			this.SetActive(true, false);
		};
		d.Result.Execute += (param) =>
		{
			d.ReadMasterData();
			Open(param);
		};
	}
	bool _isDebugInit = false;
	void DebugUpdate()
	{
		if (!this._isDebugInit)
		{
			this._isDebugInit = true;
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
