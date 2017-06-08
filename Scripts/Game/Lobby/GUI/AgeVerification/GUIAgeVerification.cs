/// <summary>
/// 年齢認証
/// </summary>

using UnityEngine;
using XUI.AgeVerification;

[DisallowMultipleComponent]
[RequireComponent( typeof( AgeVerificationView))]
public class GUIAgeVerification : Singleton<GUIAgeVerification> {

	#region ==== フィールド ====

	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private AgeVerificationView _viewAttach = null;

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// ビュー
	/// </summary>
	private AgeVerificationView ViewAttach { get { return _viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	private bool IsStartActive { get { return _isStartActive; } }

	#endregion ==== プロパティ ====

	#region ==== 初期化 ====

	/// <summary>
	/// 起動
	/// </summary>
	protected override void Awake() {

		base.Awake();
		MemberInit();
	}

	/// <summary>
	/// 開始
	/// </summary>
	private void Start() {

		Construct();

		// 初期アクティブ設定
		SetActive( IsStartActive, true, IsStartActive );
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit() {

		Controller = null;
	}

	/// <summary>
	/// 作成
	/// </summary>
	private void Construct() {

		// モデル生成
		var model = new Model();

		// ビュー生成
		IView view = null;
		if( ViewAttach != null ) {
			view = ViewAttach.GetComponent( typeof( IView ) ) as IView;
		}

		// コントローラー生成
		Controller = new Controller( model, view );
	}

	#endregion ==== 初期化 ====

	#region ==== 破棄 ====

	/// <summary>
	/// 破棄
	/// </summary>
	private void OnDestroy() {

		if( Controller != null ) {
			Controller.Dispose();
		}
	}

	#endregion ==== 破棄 ====

	#region ==== 更新 ====

	/// <summary>
	/// 更新
	/// </summary>
	private void Update() {

		// マッチング中であればクローズ
		if( GUIMatchingState.IsMatching ) {
			GUIController.Clear();
		}
	}

	#endregion ==== 更新 ====

	#region ==== アクティブ設定 ====

	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close() {
		if( Instance != null ) Instance.SetActive( false, false, false );
	}

	/// <summary>
	/// 開く
	/// </summary>
	public static void Open() {
		if( Instance != null ) Instance.SetActive( true, false, true );
	}

	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen() {
		if( Instance != null ) Instance.SetActive( true, false, false );
	}

	/// <summary>
	/// アクティブ設定
	/// </summary>
	private void SetActive( bool isActive, bool isTweenSkip, bool isSetup ) {

		// 開き直させない
		if( isActive && !isSetup ) {
			// GUIを全て閉じる
			GUIController.Clear();
			return;
		}

		// セットアップ
		if( isSetup ) {
			Setup();
		}

		// コントローラのアクティブ設定
		if( Controller != null ) {
			Controller.SetActive( isActive, isTweenSkip );
		}
	}

	#endregion ==== アクティブ設定 ====

	#region ==== 各種情報更新 ====

	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup() {

		// コントローラセットアップ
		if( Controller != null ) {
			Controller.Setup();
		}
	}

	#endregion ==== 各種情報更新 ====



	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();
	GUIDebugParam DebugParam { get { return _debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase {
		public GUIDebugParam() {
			this.AddEvent(this.Language);
		}

		public LanguageEvent Language = new LanguageEvent();
		[System.Serializable]
		public class LanguageEvent : IDebugParamEvent {

			public MasterTextSetterList text = null;
			public bool execute = false;
			public Scm.Common.GameParameter.Language language = Scm.Common.GameParameter.Language.Japanese;

			public void Update() {
				if (this.execute)
				{
					this.execute = false;
					Scm.Common.Utility.Language = this.language;
					if (this.text != null) this.text.UpdateText();
				}
			}
		}
	}
	void DebugInit() {
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () =>
		{
			d.ReadMasterData();
			Open();
		};
	}
	bool _isDebugInit = false;
	void DebugUpdate() {
		if( !this._isDebugInit ) {
			this._isDebugInit = true;
			this.DebugInit();
		}

		this.DebugParam.Update();
	}
	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate() {
		if( Application.isPlaying ) {
			this.DebugUpdate();
		}
	}
#endif
	#endregion
}
