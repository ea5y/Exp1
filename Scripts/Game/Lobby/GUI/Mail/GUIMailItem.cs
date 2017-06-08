using UnityEngine;
using System;
using System.Collections;

using XUI.MailItem;

/// <summary>
/// 各メールアイテム操作GUI
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MailItemView))]
public class GUIMailItem : MonoBehaviour
{
	#region === Field ===
	
	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private MailItemView _viewAttach = null;

	#endregion === Field ===

	#region === Property ===

	private MailItemView ViewAttach { get { return _viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }

	// アイテムアイコン
	private ItemIcon ItemIcon { get { return ScmParam.Lobby.ItemIcon; } }

	// 汎用アイコン
	private CommonIcon CommonIcon { get { return ScmParam.Lobby.CommonIcon; } }

	#endregion === Property ===

	#region === Create ===

	public static GUIMailItem Create(GameObject prefab, Transform parent, int itemIndex)
	{
		UnityEngine.Assertions.Assert.IsNotNull(prefab, "GUIMailItem:'prefab' Not Found!!");

		// インスタンス化
		var go = SafeObject.Instantiate(prefab) as GameObject;
		UnityEngine.Assertions.Assert.IsNotNull(go, "GUIMailItem:'SafeObject.Instantiate(prefab) as GameObject' Not Found!!");

		// 名前
		go.name = string.Format("{0}{1}", prefab.name, itemIndex);
		// 親子付け
		go.transform.SetParent(parent, false);
		// 可視化
		if(!go.activeSelf) {
			go.SetActive(true);
		}

		// コンポーネント取得
		var item = go.GetComponent(typeof(GUIMailItem)) as GUIMailItem;
		UnityEngine.Assertions.Assert.IsNotNull(item, "GUIMailItem:'go.GetComponent(typeof(GUIMailItem)) as GUIMailItem' Not Found!!");
		item.Initialize();

		return item;
	}

	#endregion === Create ===

	
	#region === 初期化 ===

	/// <summary>
	/// メンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.Controller = null;
	}

	private void Initialize()
	{
		this.MemberInit();
		this.Construct();
		this.Setup();
	}

	/*
	private void Awake()
	{
		this.MemberInit();
	}

	private void Start()
	{
		this.Construct();
	}
	*/

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
		var controller = new Controller(model, view, ItemIcon, CommonIcon);
		Controller = controller;
		Controller.OnDetailClick += HandleDetailClick;
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
	private void SetActive(bool isActive)
	{
		if (this.Controller != null){
			this.Controller.SetActive(isActive);
		}
	}

	#endregion === アクティブ設定 ===

	#region === 各種情報更新 ===

	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup()
	{
		if (Controller != null){
			Controller.Setup();
		}
	}

	#endregion === 各種情報更新 ===

	/// <summary>
	/// メールインフォをセットする
	/// </summary>
	/// <param name="info"></param>
	public void SetMailInfo(MailInfo mail)
	{
		if(Controller == null) return;

		Controller.SetMailInfo(mail);
	}

	/// <summary>
	/// メールインフォを更新する
	/// </summary>
	public void UpdateMailInfo()
	{
		if(Controller == null) return;

		Controller.UpdateMailInfo();
	}


	/// <summary>
	/// 詳細クリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void HandleDetailClick(object sender, MailDetailEventArgs e)
	{
		OpenMailDetail(e.MailInfo);
	}
	
	/// <summary>
	/// 詳細を開く
	/// </summary>
	/// <param name="mail"></param>
	private void OpenMailDetail(MailInfo mail)
	{
		GUIMail.OpenMailDetail(mail);
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
