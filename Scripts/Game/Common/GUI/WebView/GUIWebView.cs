/// <summary>
/// WebView
/// 
/// 2015/12/09
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIWebView : Singleton<GUIWebView>
{
	#region フィールド&プロパティ
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	XUI.Web.WebView webView = null;

	/// <summary>
	/// データ
	/// </summary>
	[SerializeField]
	XUI.Web.Model webModel = null;

	[SerializeField]
	Asobimo.WebView.PluginWebView pluginWebView = null;


	/// <summary>
	/// コントローラ
	/// </summary>
	private XUI.Web.IController controller;
	#endregion

	#region 初期化
	protected override void Awake()
	{
		base.Awake();
		Construct();
	}

	private void Construct()
	{
		// 2Dカメラの取得
		Camera screenCamera = NGUITools.FindCameraForLayer(this.gameObject.layer);
		// UIRoot取得
		UIRoot root = NGUITools.FindInParents<UIRoot>(this.gameObject);
		// コントローラの生成
		this.controller = new XUI.Web.Controller(this.webModel, this.webView, screenCamera, root, pluginWebView);
	}
	#endregion

	#region 表示
	/// <summary>
	/// 公式ページをブラウザで表示
	/// </summary>
	public static void OpenHome()
	{
		if (Instance == null) { return; }
		Instance.controller.OpenApplication(Asobimo.WebAPI.AsobimoWebAPI.Instance.BaseURL);
	}

	/// <summary>
	/// お知らせページを表示
	/// </summary>
	public static void OpenNews()
	{
		if (Instance == null) { return; }
		Instance.controller.OpenNews();
	}

	/// <summary>
	/// ゲーム起動時のみ1度だけお知らせページを表示する
	/// 2回目以降このメソッドを呼び出しても表示はされない
	/// </summary>
	public static void OpenNewsStartAppOnly()
	{
		if (Instance == null) { return; }
		Instance.controller.OpenNewsStartAppOnly();
	}

	/// <summary>
	/// 公式ツイッターページ表示
	/// </summary>
	public static void OpenTwitter()
	{
		if (Instance == null) { return; }
		Instance.controller.OpenApplication(Instance.webModel.TwitterURL);
	}

	/// <summary>
	/// お問い合わせページを表示
	/// </summary>
	public static void OpenContact()
	{
		if(Instance == null) { return; }
		Instance.controller.OpenContact();
	}


	/// <summary>
	/// ヘルプ表示
	/// </summary>
	public static void OpenHelp()
	{
		if (Instance == null) { return; }
		Instance.controller.Open(Instance.webModel.HelpURL);
	}

	/// <summary>
	/// アンケートページ表示
	/// </summary>
	public static void OpenEnquete()
	{
		if (Instance == null) { return; }
		Instance.controller.OpenEnquete();
	}

	/// <summary>
	/// GoOneページをブラウザで表示
	/// </summary>
	public static void OpenGoOne()
	{
		if (Instance == null) { return; }
		Instance.controller.OpenApplication(Instance.webModel.GoOneURL);
	}

	/// <summary>
	/// 規約ページを開く
	/// </summary>
	public static void OpenTerms()
	{
		if( Instance == null ) { return; }
		Instance.controller.OpenApplication( Instance.webModel.TermsURL );
	}

	/// <summary>
	/// 指定URLを開く
	/// </summary>
	/// <param name="url">開くURL</param>
	/// <param name="openApplication">別アプリで開くかどうか</param>
	public static void Open( string url, bool openApplication ) {

		if( Instance == null ) { return; }
		if( string.IsNullOrEmpty( url ) ) { return; }
		if( openApplication ) {
			Instance.controller.OpenApplication( url );
		} else {
			Instance.controller.Open( url );
		}
	}

	#endregion

	#region 閉じる
	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close()
	{
		if (Instance == null) { return; }
		Instance.controller.Close();
	}
	#endregion

	#region NGUIリフレクション
	/// <summary>
	/// 閉じるボタンが押された時
	/// </summary>
	public void OnClose()
	{
		Close();
	}
	#endregion


	#region デバッグ
#if UNITY_EDITOR && XW_DEBUG
	[SerializeField]
	private GUIDebugParam debugParam = new GUIDebugParam();
	private GUIDebugParam DebugParam { get { return debugParam; } }
	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public bool isWebDisplay;
	}

	void DebugInit()
	{
		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += OpenNews;
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
	void OnGUI()
	{
		var d = this.DebugParam;

		if (d.isWebDisplay)
		{
			XUI.Web.Controller webController = controller as XUI.Web.Controller;
			if (webController == null) { return; }

			Rect rect = webController.CalculateRect();
			GUI.Box(new Rect(rect.x, Screen.height - rect.yMax, rect.width, rect.height),
					"WebView\n" +
					"(Can not be displayed in editor.)");
		}
	}
#endif
	#endregion
}
