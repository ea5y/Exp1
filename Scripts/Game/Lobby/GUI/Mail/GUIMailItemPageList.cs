using UnityEngine;
using System;
using System.Collections;

using XUI.MailItemPageList;
using System.Collections.Generic;

/// <summary>
/// メールのスクロールビュー操作GUI
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MailItemPageListView))]
public class GUIMailItemPageList : MonoBehaviour
{
	#region === Field ===
	
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private MailItemPageListView viewAttach = null;

	/// <summary>
	/// スクロールビュー設定
	/// </summary>
	[SerializeField]
	private GUIMailItemScrollView itemScrollView = null;
	
	#endregion === Field ===

	#region === Property ===

	private MailItemPageListView ViewAttach { get { return viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }
	
	#endregion === Property ===

	
	#region === Event ===

	/// <summary>
	/// ページ変更イベント
	/// </summary>
	public event EventHandler<PageChangeEventArgs> OnPageChange = (sender, e) => { };

	#endregion === Event ===


	#region === 初期化 ===

	/// <summary>
	/// メンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Controller = null;
	}

	private void Awake()
	{
		this.MemberInit();
		this.Construct();
	}

	//private void Start()
	//{
	//	this.Construct();
	//}

	private void Construct()
	{
		// モデル生成
		var model = new Model(itemScrollView, viewAttach.PageScrollViewAttach);

		// ビュー生成
		IView view = null;
		if (this.ViewAttach != null)
		{
			view = this.ViewAttach.GetComponent(typeof(IView)) as IView;
		}
		
		// コントローラー生成
		var controller = new Controller(model, view);
		this.Controller = controller;
		Controller.OnPageChange += HandlePageChange;
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
	/// アクティブ設定
	/// </summary>
	public void SetActive(bool isActive)
	{
		if (this.Controller != null){
			this.Controller.SetActive(isActive, false);
		}
	}

	#endregion === アクティブ設定 ===

	#region === 各種情報更新 ===

	/// <summary>
	/// 初期設定
	/// </summary>
	public void Setup()
	{
		if (this.Controller != null){
			this.Controller.Setup();
		}
	}

	public void Clear()
	{
		if(this.Controller != null) {
			this.Controller.Clear();
		}
	}

	#endregion === 各種情報更新 ===

	/// <summary>
	/// アイテム数をセットする
	/// </summary>
	/// <param name="count"></param>
	public void SetItemCount(int count)
	{
		if(Controller == null) return;

		Controller.SetTotalItemCount(count);
	}

	/// <summary>
	/// 指定ページに変更する
	/// </summary>
	/// <param name="page"></param>
	public void SetPage(int page)
	{
		if(Controller == null) return;

		Controller.SetPage(page);
	}

	/// <summary>
	/// 再度ページを開く
	/// </summary>
	public void ReopenPage()
	{
		if(Controller == null) return;

		Controller.ReopenPage();
	}



	/// <summary>
	/// メールリスト更新
	/// </summary>
	/// <param name="mails"></param>
	public void SetViewMailList(List<MailInfo> mails)
	{
		if(Controller == null) return;

		Controller.SetViewItem(mails);
	}

	/// <summary>
	/// 今のリストを更新する
	/// </summary>
	public void UpdateCurrentList()
	{
		if(Controller == null) return;

		Controller.UpdateCurrentList();
	}


	/// <summary>
	/// ページ変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandlePageChange(object sender, PageChangeEventArgs e)
	{
		OnPageChange(this, e);
	}





	#region === デバッグ ===

#if UNITY_EDITOR && XW_DEBUG

	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();

	GUIDebugParam DebugParam { get { return _debugParam; } }

	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase
	{
		public GUIDebugParam()
		{
		}

		[SerializeField]
		TemprateEvent _sample = new TemprateEvent();
		
		public TemprateEvent Sample { get { return _sample; } }
		
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

		//d.ExecuteClose += Close;
		//d.ExecuteActive += () =>
		//{
		//	d.ReadMasterData();
		//	Open();
		//};

		d.Sample.Execute += () => { Debug.Log("Sample"); };
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
