/// <summary>
/// ブラックリスト
/// 
/// 2014/08/04
/// </summary>
using UnityEngine;
using System.Collections;

public class GUIBlackList : Singleton<GUIBlackList>
{
	#region フィールド＆プロパティ
	const string TitleMsg = "ブラックリスト";
	const string HelpMsg = "ブラックリストです";

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
	}

	// アクティブ設定
	bool IsActive { get; set; }
	#endregion

	#region 初期化
	override protected void Awake()
	{
		base.Awake();

		// 表示設定
		this._SetActive(this.IsStartActive);
	}
	#endregion

	#region アクティブ設定
	public static void SetActive(bool isActive)
	{
		if (Instance == null)
			return;
		Instance._SetActive(isActive);
	}
	void _SetActive(bool isActive)
	{
		this.IsActive = isActive;
		if (isActive)
		{
			GUIScreenTitle.Play(isActive, TitleMsg);
			GUIHelpMessage.Play(isActive, HelpMsg);
		}
		else
		{
			GUIScreenTitle.Play(false);
			GUIHelpMessage.Play(false);
		}

		// アニメーション開始
		this.Attach.rootTween.Play(this.IsActive);
	}
	#endregion

	#region NGUIリフレクション
	public void OnHome()
	{
		this._SetActive(false);
	}
	public void OnClose()
	{
		this._SetActive(false);
		GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.Option);
	}
	#endregion
}
