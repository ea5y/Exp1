/// <summary>
/// スクロールメッセージスクリプト.
/// 一定時間過ぎたらメッセージのスクロールを行う.
/// フレーム内にメッセージが収まっている場合はスクロールを行わない.
/// .
/// 2014/05/16.
/// </summary>
using UnityEngine;
using System;
using System.Collections;

public class GUIScrollMessage : MonoBehaviour
{
	#region 状態

	private enum StateType
	{
		None,
		StartScroll,
		Scroll
	}

	#endregion

	#region 定数.
	
	/// <summary>
	/// スクロール量.
	/// </summary>
	private const float Scroll = 10f;
	
	#endregion
	
	#region スクロールデータ.

	[System.Serializable]
	public class StartScrollData
	{
		/// <summary>
		/// 開始スクロールス期間
		/// </summary>
		public float startScrollDuration;

		/// <summary>
		/// アニメーションカーブ
		/// </summary>
		public AnimationCurve animaCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

		/// <summary>
		/// 表示後一時停止する時間.
		/// </summary>
		public float startDelay = 5.0f;
	}

	/// <summary>
	/// スクロールする場合のデータ.
	/// </summary>
	[System.Serializable]
	public class ScrollEnableData
	{
		/// <summary>
		/// ループ数.
		/// 0の時は無現ループ.
		/// </summary>
		public int loopCount;
		
		/// <summary>
		/// スクロールスピード.
		/// </summary>
		public float scrollSpeed;

		/// <summary>
		/// アニメーションカーブ
		/// </summary>
		public AnimationCurve animaCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	}
	
	/// <summary>
	/// スクロールしない場合のデータ.
	/// </summary>
	[System.Serializable]
	public class ScrollDisableData
	{
		/// <summary>
		/// 表示期間.
		/// </summary>
		public float showDuration;
	}
	
	#endregion
	
	#region フィールド&プロパティ.

	/// <summary>
	/// 開始時のスクロールデータ
	/// </summary>
	[SerializeField]
	private StartScrollData startScrollData;

	/// <summary>
	/// スクロールする場合のデータ.
	/// </summary>
	[SerializeField]
	private ScrollEnableData scrollEnableData;
	
	/// <summary>
	/// スクロールしない場合のデータ.
	/// </summary>
	[SerializeField]
	private ScrollDisableData scrollDisableData;

	/// <summary>
	/// メッセージラベル.
	/// </summary>
	[SerializeField]
	private UILabel messageLabel;
	
	/// <summary>
	/// ウェジェット
	/// (主にスクロールサイズを見るために必要).
	/// </summary>
	[SerializeField]
	private UIWidget clippingWidget;
	
	/// <summary>
	/// 再生中かどうか.
	/// </summary>
	public bool IsPlay { get { return this.isPlay; } }
	private bool isPlay = false;

	/// <summary>
	/// メッセージ
	/// </summary>
	private string message;
	public string Message { get { return message; } }

	/// <summary>
	/// 開始時のスクロール終了イベント通知
	/// </summary>
	public event EventHandler StartScrollFinishEvevnt = (sender, e) => { };
	
	/// <summary>
	/// メッセージ移動用Tween.
	/// </summary>
	private TweenPosition messageTweenPos;
	
	/// <summary>
	/// スクロール処理更新用.
	/// </summary>
	private IEnumerator scrollUpdate = null;

	/// <summary>
	/// ループカウント.
	/// </summary>
	private int loopCounter = 0;

	/// <summary>
	/// 状態
	/// </summary>
	private StateType state = StateType.None;
	
	#endregion
	
	#region 開始.
	
	void Start ()
	{
		// 初期化
		Reset();
	}
	
	#endregion

	#region 破棄
	/// <summary>
	/// 破棄
	/// </summary>
	void OnDestroy()
	{
		this.StartScrollFinishEvevnt = null;
	}
	#endregion

	#region 更新.

	void Update ()
	{
		if(this.scrollUpdate == null)
			return;
		
		if(!this.scrollUpdate.MoveNext())
		{
			this.scrollUpdate = null;
		}
	}
	
	#endregion
	
	#region スクロール処理.

	/// <summary>
	/// スクロール処理.
	/// </summary>
	IEnumerator ScrollCoroutine()
	{
		// 固定時間処理.
		float nowTime = this.startScrollData.startDelay;
		while(nowTime > 0)
		{
			nowTime -= Time.deltaTime;
			yield return 0;
		}
		
		if(this.messageLabel.width > this.clippingWidget.width)
		{
			// 表示文字がフレームの幅を超えていた場合はスクロールを行う.
			// 現在の位置からスクロールを開始.
			// メインメッセージのスクロールとTween終了メソッドをセットする.
			this.messageTweenPos = ScrollSet(this.messageLabel.gameObject, false);
		}
		else
		{
			// 表示文字がフレーム内に収まっていた場合は一定時間だけ表示する.
			nowTime = 0;
			while(nowTime <= this.scrollDisableData.showDuration)
			{
				// 最大ループ回数が0の時は無限に表示し続ける.
				if(this.scrollEnableData.loopCount != 0)
				{
					nowTime += Time.deltaTime;
				}
				yield return 0;
			}
			// 再生終了
			ChangeState(StateType.None);
		}
	}

	/// <summary>
	/// スクロール値セット.
	/// </summary>
	/// <param name='duration'>
	/// 期間.
	/// </param>
	/// <param name='isInitPosition'>
	/// 端から開始するか.
	/// </param>
	private TweenPosition ScrollSet(GameObject gameObj, bool isOuter)
	{
		float x = (this.clippingWidget.width/2f) + (this.messageLabel.width / 2f);
		Vector3 src = gameObj.transform.localPosition;
		if(isOuter)
		{
			// 橋から開始.
			src.x = x;
		}
		Vector3 to = gameObj.transform.localPosition;
		to.x = -x;

		// 表示期間 = スクロールする距離 / スクロールスピード.
		float duration = Mathf.Abs(to.x-src.x) / (this.scrollEnableData.scrollSpeed * Scroll);
		TweenPosition tweenPos = TweenPosition.Begin(gameObj, duration, to);
		tweenPos.ignoreTimeScale = false;
		tweenPos.from = src;
		tweenPos.animationCurve = this.scrollEnableData.animaCurve;

		return tweenPos;
	}
	
	#region Tween終了時処理.

	/// <summary>
	/// スクロール時のTween終了時の処理.
	/// </summary>
	private void ScrollTweenOnFinished()
	{
		if(this.state == StateType.StartScroll)
		{
			// 通知
			this.StartScrollFinishEvevnt(this, EventArgs.Empty);
			// 通常スクロールに変更
			ChangeState(StateType.Scroll);
		}
		else
		{
			if(this.scrollEnableData.loopCount > 0)
			{
				this.loopCounter--;
			}
			
			if(this.loopCounter > 0 || this.scrollEnableData.loopCount <= 0)
			{
				// 開始スクロールを再生
				ChangeState(StateType.StartScroll);
			}
			else
			{
				// 再生終了
				ChangeState(StateType.None);
			}
		}
	}

	#endregion
	#endregion
	
	#region　スクロール再生&リセット.
	
	/// <summary>
	/// スクロール再生処理.
	/// </summary>
	public void Play()
	{
		if(this.messageLabel == null && this.clippingWidget == null)
		{
			return;
		}
		
		// 再生中にする.
		this.isPlay = true;
		
		if(this.messageTweenPos)
		{
			this.messageTweenPos.enabled = false;
		}

		// ループ数セット.
		this.loopCounter = this.scrollEnableData.loopCount;
		
		// 開始スクロール再生
		ChangeState(StateType.StartScroll);
	}

	/// <summary>
	/// スクロールリセット処理.
	/// </summary>
	[ContextMenu("ReStart")]
	public void ReStart()
	{
		this.scrollUpdate = null;
		Play();
	}

	/// <summary>
	/// メッセージを初期状態にする
	/// スクロールは再生されない
	/// </summary>
	public void Reset()
	{
		// 位置を初期化
		Vector3 position = this.messageLabel.transform.localPosition;
		position.x = (this.clippingWidget.width/2f) + (this.messageLabel.width / 2f);
		this.messageLabel.transform.localPosition = position;
		this.loopCounter = 0;
		// Tweenが再生中なら停止
		if(this.messageTweenPos != null)
		{
			this.messageTweenPos.enabled = false;
		}
	}

	#endregion

	#region 開始時のスクロール再生

	/// <summary>
	/// 開始時のスクロールを再生する
	/// </summary>
	private void StartScrollPlay()
	{
		this.messageTweenPos = StartScrollSet(this.messageLabel.gameObject);
		this.messageTweenPos.SetOnFinished(ScrollTweenOnFinished);
	}
	
	/// <summary>
	/// 開始時のスクロールセット
	/// </summary>
	private TweenPosition StartScrollSet(GameObject gameObj)
	{
		float x = (this.clippingWidget.width/2f) + (this.messageLabel.width / 2f);
		// 開始位置
		Vector3 src = gameObj.transform.localPosition;
		src.x = x;
		// 終了位置
		Vector3 to = gameObj.transform.localPosition;
		to.x = 0;
		if(this.messageLabel.width > this.clippingWidget.width)
		{
			// フレーム幅に文字が収まり切らない場合は開始文字位置を一番端にセットする.
			to.x = ((this.messageLabel.width/2f)-(this.clippingWidget.width/2f));
		}

		// 表示期間 = スクロールする距離 / スクロールスピード.
		float duration = this.startScrollData.startScrollDuration;
		TweenPosition tweenPos = TweenPosition.Begin(gameObj, duration, to);
		tweenPos.ignoreTimeScale = false;
		tweenPos.from = src;
		tweenPos.animationCurve = this.startScrollData.animaCurve;
		
		return tweenPos;
	}

	#endregion

	#region スクロールメッセージ終了

	/// <summary>
	/// スクロールメッセージ終了処理
	/// </summary>
	private void ScrollFinished()
	{
		// 再生終了にする.
		this.isPlay = false;
	}
	
	#endregion

	#region メッセージセット.
	
	/// <summary>
	/// メッセージセット.
	/// </summary>
	/// <param name='message'>
	/// セットするメッセージ.
	/// </param>
	public void SetMessage(string message)
	{
		if(this.messageLabel == null)
			return;

		this.message = message;
		this.messageLabel.text = message;
	}
	
	#endregion

	#region 状態遷移

	/// <summary>
	/// 状態を変更する
	/// </summary>
	private void ChangeState(StateType state)
	{
		switch(state)
		{
			case StateType.StartScroll:
			{
				StartScrollPlay();
				break;
			}
			case StateType.Scroll:
			{
				// メインのスクロール処理開始
				this.scrollUpdate = ScrollCoroutine();
				break;
			}
			case StateType.None:
			{
				ScrollFinished();
				break;
			}
		}
		this.state = state;
	}

	#endregion
}
