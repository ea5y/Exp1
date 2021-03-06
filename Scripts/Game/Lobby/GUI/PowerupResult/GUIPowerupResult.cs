/// <summary>
/// 強化合成結果表示
/// 
/// 2016/01/28
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Packet;

public class GUIPowerupResult : Singleton<GUIPowerupResult>
{
	#region フィールド＆プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	XUI.PowerupResult.PowerupResultView _viewAttach = null;
	XUI.PowerupResult.PowerupResultView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// レベル表示フォーマット
	/// </summary>
	[SerializeField]
	string _lvFormat = "Lv.{0}";
	string LvFormat { get { return _lvFormat; } }

	/// <summary>
	/// ステータス表示フォーマット
	/// </summary>
	[SerializeField]
	string _statusFormat = "{0}";
	string StatusFormat { get { return _statusFormat; } }

	/// <summary>
	/// ステータスアップ表示フォーマット
	/// </summary>
	[SerializeField]
	string _statusUpFormat = "{0} up!";
	string StatusUpFormat { get { return _statusUpFormat; } }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	bool _isStartActive = false;
	bool IsStartActive { get { return _isStartActive; } }

	// キャラボード
	CharaBoard CharaBoard { get { return ScmParam.Lobby.CharaBoard; } }

	// モデル
	XUI.PowerupResult.IModel Model { get; set; }
	// ビュー
	XUI.PowerupResult.IView View { get; set; }
	// コントローラー
	XUI.PowerupResult.IController Controller { get; set; }
	// シリアライズされていないメンバー初期化
	void MemberInit()
	{
		this.Model = null;
		this.View = null;
		this.Controller = null;
	}
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();
		this.MemberInit();
	}
	void Start()
	{
		this.Construct();
		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true);
	}
	void Construct()
	{
		// モデル生成
		var model = new XUI.PowerupResult.Model();
		this.Model = model;
		this.Model.BeforeLvFormat = this.LvFormat;
		this.Model.AfterLvFormat = this.LvFormat;
		this.Model.HitPointFormat = this.StatusFormat;
		this.Model.HitPointUpFormat = this.StatusUpFormat;
		this.Model.AttackFormat = this.StatusFormat;
		this.Model.AttackUpFormat = this.StatusUpFormat;
		this.Model.DefenceFormat = this.StatusFormat;
		this.Model.DefenceUpFormat = this.StatusUpFormat;
		this.Model.ExtraFormat = this.StatusFormat;
		this.Model.ExtraUpFormat = this.StatusUpFormat;

		// ビュー生成
		XUI.PowerupResult.IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(XUI.PowerupResult.IView)) as XUI.PowerupResult.IView;
		}
		this.View = view;

		// コントローラー生成
		var controller = new XUI.PowerupResult.Controller(model, view, this.CharaBoard);
		this.Controller = controller;
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
	public static void Open(XUI.PowerupResult.SetupParam p)
	{
		if (Instance != null) Instance._Open(p);
	}
	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen()
	{
		if (Instance != null) Instance.SetActive(true, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	void _Open(XUI.PowerupResult.SetupParam p)
	{
		this.SetActive(true, false);
		this.Setup(p);
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
	/// 初期設定
	/// </summary>
	void Setup(XUI.PowerupResult.SetupParam p)
	{
		if (this.Controller != null)
		{
			this.Controller.Setup(p);
		}
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
			public event System.Action<XUI.PowerupResult.SetupParam> Execute = delegate { };
			[SerializeField]
			bool executeDummy = false;
			[SerializeField]
			bool execute = false;

			[SerializeField]
			AvatarType avatarType = AvatarType.None;
			[SerializeField]
			int beforeLv = 0;
			[SerializeField]
			int afterLv = 0;
			[SerializeField]
			int exp = 0;
			[SerializeField]
			int totalExp = 0;
			[SerializeField]
			int nextLvTotalExp = 0;
			[SerializeField]
			int hitPoint = 0;
			[SerializeField]
			int hitPointUp = 0;
			[SerializeField]
			int attack = 0;
			[SerializeField]
			int attackUp = 0;
			[SerializeField]
			int defence = 0;
			[SerializeField]
			int defenceUp = 0;
			[SerializeField]
			int extra = 0;
			[SerializeField]
			int extraUp = 0;

			public void Update()
			{
				if (this.executeDummy)
				{
					this.executeDummy = false;

					this.avatarType = (AvatarType)UnityEngine.Random.Range((int)AvatarType.Begin, (int)AvatarType.End + 1);

					this.beforeLv = UnityEngine.Random.Range(1, 11);
					this.afterLv = this.beforeLv;
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						this.afterLv = UnityEngine.Random.Range(this.beforeLv, 11);
					}

					this.totalExp = UnityEngine.Random.Range(0, 10000);
					this.exp = UnityEngine.Random.Range(this.totalExp, 10000);
					this.nextLvTotalExp = UnityEngine.Random.Range(this.exp, 10000);

					this.hitPoint = UnityEngine.Random.Range(0, 1000);
					this.hitPointUp = 0;
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						this.hitPointUp = UnityEngine.Random.Range(0, this.hitPoint + 1);
					}

					this.attack = UnityEngine.Random.Range(0, 1000);
					this.attackUp = 0;
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						this.attackUp = UnityEngine.Random.Range(0, this.attack + 1);
					}

					this.defence = UnityEngine.Random.Range(0, 1000);
					this.defenceUp = 0;
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						this.defenceUp = UnityEngine.Random.Range(0, this.defence + 1);
					}

					this.extra = UnityEngine.Random.Range(0, 1000);
					this.extraUp = 0;
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						this.extraUp = UnityEngine.Random.Range(0, this.extra + 1);
					}

					this.execute = true;
				}
				if (this.execute)
				{
					this.execute = false;
					var args = new XUI.PowerupResult.SetupParam();
					args.AvatarType = this.avatarType;
					args.BeforeLv = this.beforeLv;
					args.AfterLv = this.afterLv;
					args.Exp = this.exp;
					args.TotalExp = this.totalExp;
					args.NextLvTotalExp = this.nextLvTotalExp;
					args.HitPoint = this.hitPoint;
					args.HitPointUp = this.hitPointUp;
					args.Attack = this.attack;
					args.AttackUp = this.attackUp;
					args.Defence = this.defence;
					args.DefenceUp = this.defenceUp;
					args.Extra = this.extra;
					args.ExtraUp = this.extraUp;
					this.Execute(args);
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

		d.Result.Execute += (args) => { this.Setup(args); };
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
