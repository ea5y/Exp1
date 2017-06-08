using UnityEngine;
using System;
using System.Collections;

using XUI.EvolutionDirection;


[DisallowMultipleComponent]
[RequireComponent(typeof(EvolutionDirectionView))]
public class GUIEvolutionDirection : Singleton<GUIEvolutionDirection>
{
	#region === Field ===
	
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private EvolutionDirectionView viewAttach = null;

	/// <summary>
	/// エフェクト操作
	/// </summary>
	[SerializeField]
	private GUIEffectDirection effectDirection = null;

	[SerializeField]
	private GUIEvolutionDirectionText directionText = null;

	[SerializeField]
	private TweenSync tweenSync = null;

	/// <summary>
	/// 初期化時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool isStartActive = false;


	// アイコン読み込んだか
	private bool isIconLoaded = false;

	// キャラ読み込んだか
	private bool isCharaLoaded = false;

	// クローズ時のコールバック
	private Action closeCallback = null;

	#endregion === Field ===

	#region === Property ===

	private bool IsStartActive { get { return isStartActive; } }
	
	private EvolutionDirectionView ViewAttach { get { return viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }

	// キャラアイコン
	private CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

	#endregion === Property ===


	#region === 初期化 ===

	/// <summary>
	/// メンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Controller = null;
	}

	protected override void Awake()
	{
		base.Awake();
		this.MemberInit();
	}

	private void Start()
	{
		this.Construct();

		// 初期アクティブ設定
		this.SetActive(this.IsStartActive, true);
	}

	private void Construct()
	{
		// モデル生成
		var model = new Model();

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}
		
		// コントローラー生成
		var controller = new Controller(model, view, effectDirection, directionText, CharaIcon, tweenSync);
		this.Controller = controller;
		Controller.OnCharaLoaded += HandleCharaLoaded;
		Controller.OnIconLoaded += HandleIconLoaded;
	}

	#endregion === 初期化 ===

	#region === 破棄 ===

	/// <summary>
	/// オブジェクトが破棄されたとき
	/// </summary>
	private void OnDestroy()
	{
		// コントローラーの破棄をする
		if(Controller != null){
			Controller.Dispose();
			Controller = null;
		}
	}

	#endregion === 破棄 ===


	#region === アクティブ設定 ===
	
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance != null){
			Instance._Close();
		}
	}


	/// <summary>
	/// 閉じる
	/// </summary>
	private void _Close()
	{
		SetActive(false, false);

		if(this.Controller != null) {
			Controller.Close();
		}

		// 各種フラグオフ
		isIconLoaded = false;
		isCharaLoaded = false;

		if(closeCallback != null) {
			closeCallback();
			closeCallback = null;
		}
	}



	/// <summary>
	/// 開く
	/// </summary>
	public static void Open(EvolutionDirectionParam param, Action closeCallback)
	{
		if (Instance != null){
			Instance._Open(param, closeCallback);
		}
	}

	private void _Open(EvolutionDirectionParam param, Action closeCallback)
	{
		SetActive(true, false);
		Setup(param, closeCallback);
	}
	
	/// <summary>
	/// アクティブ設定
	/// </summary>
	private void SetActive(bool isActive, bool isTweenSkip)
	{
		if (this.Controller != null){
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}

	#endregion === アクティブ設定 ===

	#region === 各種情報更新 ===

	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup(EvolutionDirectionParam param, Action closeCallback)
	{
		if (this.Controller != null){
			this.closeCallback = closeCallback;

			this.Controller.Setup(param);
		}
	}

	#endregion === 各種情報更新 ===




	/// <summary>
	/// アイコン読み込み終了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleIconLoaded(object sender, EventArgs e)
	{
		isIconLoaded = true;
		CheckExecute();
	}

	/// <summary>
	/// キャラ読み込み終了時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleCharaLoaded(object sender, EventArgs e)
	{
		isCharaLoaded = true;
		CheckExecute();
	}

	/// <summary>
	/// 実行チェック
	/// </summary>
	private void CheckExecute()
	{
		// 読み込みが終っていなければ実行しない
		if(!isIconLoaded || !isCharaLoaded) return;

		Controller.Execute();
	}


	#region === デバッグ ===

#if UNITY_EDITOR && XW_DEBUG

	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();

	GUIDebugParam DebugParam { get { return _debugParam; } }

	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public EvolutionDirectionParam directionParam;

		public GUIDebugParam()
		{
		}

		[System.Serializable]
		public class TemprateEvent : IDebugParamEvent
		{
			public event System.Action Execute = delegate { };

			[SerializeField]
			bool execute = false;

			public void Update()
			{
				if (this.execute)
				{
					this.execute = false;
					this.Execute();
				}
			}
		}
	}

	private void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () =>
		{
			d.ReadMasterData();
			Open(d.directionParam, null);
		};
	}

	bool _isDebugInit = false;

	private void DebugUpdate()
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

	#endregion === デバッグ ===

}
