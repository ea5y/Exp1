/// <summary>
/// ビュー基底クラス
/// 
/// 2015/12/14
/// </summary>
using UnityEngine;

/// <summary>
/// ビュー基底クラス
/// </summary>
public abstract class GUIViewBase : MonoBehaviour
{
	#region フィールド＆プロパティ
	/// <summary>
	/// アクティブ状態
	/// </summary>
	public enum ActiveState : int
	{
		None = 0,	// なし
		Closed = 1,	// 閉じてる
		Closing = 2,	// 閉じてる途中
		Opening = 4,	// 開いている途中
		Opened = 8,	// 開いてる
	}

	/// <summary>
	/// ルート開閉用PlayTween
	/// </summary>
	[SerializeField]
	UIPlayTween _rootTween = null;
	UIPlayTween RootTween { get { return _rootTween; } }

	// アクティブフラグ
	public bool IsActive { get; private set; }
	// ルートのアクティブ状態
	ActiveState RootActiveState { get; set; }

	// アクティブフラグを元に現在の状態を取得する
	ActiveState ActiveFlagState { get { return this.IsActive ? ActiveState.Opened : ActiveState.Closed; } }
	#endregion

	#region ルート設定
	/// <summary>
	/// 初期化
	/// </summary>
	protected virtual void Awake()
	{
		this.IsActive = this.gameObject.activeSelf;
		this.RootActiveState = this.ActiveFlagState;
	}
	/// <summary>
	/// ルートのアクティブ状態を取得する
	/// </summary>
	protected ActiveState GetRootActiveState()
	{
		return this.RootActiveState;
	}
	/// <summary>
	/// ルートのアクティブ設定をする
	/// </summary>
	protected void SetRootActive(bool isActive)
	{
		this.SetRootActive(isActive, false);
	}
	/// <summary>
	/// ルートのアクティブ設定をする
	/// </summary>
	protected void SetRootActive(bool isActive, bool isTweenSkip)
	{
		this.IsActive = isActive;

		if (!isTweenSkip && this.RootTween != null)
		{
			// 「開いている」状態の時に開く、または「閉じている」状態の時に閉じる場合は何もしない
			//
			// NGUI側で既に開いている時にまた開くと OnFinish が呼ばれずに
			// 「開いている途中」の状態のままになるので前の状態を見て
			// アニメーションしないようなら何もしないように対処
			if (this.IsActive)
			{
				if (this.RootActiveState != ActiveState.Opened)
				{
					EventDelegate.Add(this.RootTween.onFinished, this.OnRootTweenFinish, true);
					this.RootTween.Play(this.IsActive);

					this.RootActiveState = ActiveState.Opening;
				}
			}
			else
			{
				if (this.RootActiveState != ActiveState.Closed)
				{
					EventDelegate.Add(this.RootTween.onFinished, this.OnRootTweenFinish, true);
					this.RootTween.Play(this.IsActive);

					this.RootActiveState = ActiveState.Closing;
				}
			}
		}
		else
		{
			this.gameObject.SetActive(this.IsActive);
			this.RootActiveState = this.ActiveFlagState;
		}
	}
	/// <summary>
	/// ルートのTween終了時に呼ばれるイベント
	/// </summary>
	protected virtual void OnRootTweenFinish()
	{
		this.RootActiveState = this.ActiveFlagState;
	}
	#endregion
}





/// <summary>
/// 画面系ビュー基底クラス
/// </summary>
public abstract class GUIScreenViewBase : GUIViewBase
{
	#region フィールド＆プロパティ
	[SerializeField]
	UIButton _homeButton = null;
	UIButton HomeButton { get { return _homeButton; } }

	[SerializeField]
	UIButton _closeButton = null;
	UIButton CloseButton { get { return _closeButton; } }
	#endregion

	#region 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	protected override void Awake()
	{
		base.Awake();

		if (this.HomeButton != null)	EventDelegate.Add(this.HomeButton.onClick, this.OnHomeEvent);
		if (this.CloseButton != null)	EventDelegate.Add(this.CloseButton.onClick, this.OnCloseEvent);
	}
	#endregion

	#region ホーム、閉じるボタンイベント
	/// <summary>
	/// ホームボタンイベント
	/// </summary>
	public abstract void OnHomeEvent();

	/// <summary>
	/// 閉じるボタンイベント
	/// </summary>
	public abstract void OnCloseEvent();
	#endregion

	#region 設定
	#endregion
}