/// <summary>
/// アソビモWeb
/// 
/// 2016/06/16
/// </summary>
using UnityEngine;
using System;
using XUI.AsobimoWeb;

[DisallowMultipleComponent]
[RequireComponent(typeof(AsobimoWebView))]
public class GUIAsobimoWeb : Singleton<GUIAsobimoWeb>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビューアタッチ
	/// </summary>
	[SerializeField]
	private AsobimoWebView _viewAttach = null;
	private AsobimoWebView ViewAttach { get { return _viewAttach; } }

	/// <summary>
	/// コントローラ
	/// </summary>
	private IController Controller { get; set; }

	/// <summary>
	/// アクセス状態
	/// </summary>
	public static AccessType AccessState { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.AccessStateLog : AccessType.None; } }

	/// <summary>
	/// Webのアクセスが終了しているかどうか
	/// </summary>
	public static bool IsFinish { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.IsFinish : false; } }

	/// <summary>
	/// アクセス結果
	/// </summary>
	public static bool IsResult { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.IsResult : false; } }

	/// <summary>
	/// エラーメッセージ
	/// </summary>
	public static string Error { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.Error : string.Empty; } }

	/// <summary>
	/// HTTPステータス
	/// </summary>
	public static int HttpStatus { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.HttpStatus : -1; } }

	#region 審査情報
	/// <summary>
	/// 審査中か
	/// </summary>
	public static bool IsUnderReview { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.IsUnderReview : false; } }

	/// <summary>
	/// 審査バージョン
	/// </summary>
	public static string ReviewVersion { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.ReviewVersion : string.Empty; } }
	#endregion

	#region ゲームメンテナンス
	/// <summary>
	/// ゲームメンテナンスフラグ
	/// </summary>
	public static bool IsGameMaintenance { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.IsGameMaintenance : false; } }
	#endregion

	#region タイトル表示可否
	/// <summary>
	/// タイトル表示可否フラグ
	/// </summary>
	public static bool IsDisplayTitle { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.IsDisplayTitle : false; } }
	#endregion

	#region 審査チェックユーザ登録
	/// <summary>
	/// 審査チェックユーザ登録成功フラグ
	/// </summary>
	public static bool ReviewUserResult { get { return (Instance != null && Instance.Controller != null) ? Instance.Controller.ReviewUserResult : false; } }
	#endregion

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
		var model = new Model();

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}

		// コントローラ生成
		var controller = new Controller(model, view, Asobimo.WebAPI.AsobimoWebAPI.Instance);
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
		Instance.SetActive(false, false);
	}
	/// <summary>
	/// 開く
	/// </summary>
	public static void Open()
	{
		Instance.SetActive(true, false);
	}
	/// <summary>
	/// アクティブ設定
	/// </summary>
	private void SetActive(bool isActive, bool isTweenSkip)
	{
		if (this.Controller != null)
		{
			this.Controller.SetActive(isActive, isTweenSkip);
		}
	}
	#endregion

	#region アクセス
	/// <summary>
	/// Webにアクセス開始する
	/// </summary>
	public static void StartAccess(EventHandler<FinishEventArgs> callback = null)
	{
		if (Instance != null && Instance.Controller == null) { return; }
		Instance.Controller.StartAccess(callback);
	}
	#endregion
}
