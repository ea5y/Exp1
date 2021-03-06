/// <summary>
/// ロゴ表示
/// 
/// 2015/11/13
/// </summary>
using UnityEngine;
using System;

namespace XUI.Logo
{
	/// <summary>
	/// ロゴ表示インターフェイス
	/// </summary>
	public interface IView
	{
		#region アクティブ
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();
		#endregion

		#region スキップ
		event EventHandler<EventArgs> OnSkip;
		#endregion

		#region フェード設定
		/// <summary>
		/// セットしてあるTweenをクリアする
		/// </summary>
		void ClearTween();
		/// <summary>
		/// フェードイン表示設定
		/// </summary>
		void FadeIn(UITweener tween);
		/// <summary>
		/// フェードアウト表示設定
		/// </summary>
		void FadeOut(UITweener tween);
		#endregion
	}

	/// <summary>
	/// ロゴ表示
	/// </summary>
	public class LogoView : GUIViewBase, IView
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
		}
		#endregion

		#region アクティブ
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			this.SetRootActive(isActive, isTweenSkip);
		}
		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState()
		{
			return this.GetRootActiveState();
		}
		#endregion

		#region スキップ
		public event EventHandler<EventArgs> OnSkip = (sender, e) => { };

		/// <summary>
		/// スキップイベント
		/// </summary>
		public void OnSkipEvent()
		{
			// 通知
			this.OnSkip(this, EventArgs.Empty);
		}
		#endregion

		#region フェード設定
		UITweener _fadeTween = null;
		UITweener FadeTween { get { return _fadeTween; } set { _fadeTween = value; } }

		/// <summary>
		/// セットしてあるTweenをクリアする
		/// </summary>
		public void ClearTween()
		{
			this.FadeTween = null;
		}
		/// <summary>
		/// フェードイン表示設定
		/// </summary>
		public void FadeIn(UITweener tween)
		{
			// 前のTweenの表示をオフにする
			if (this.FadeTween != null)
			{
				this.FadeTween.Sample(0f, true);
			}

			this.FadeTween = tween;

			// 今回のTween表示のアニメーション開始
			if (this.FadeTween != null)
			{
				this.FadeTween.Sample(0f, false);
				this.FadeTween.PlayForward();
			}
		}
		/// <summary>
		/// フェードアウト表示設定
		/// </summary>
		public void FadeOut(UITweener tween)
		{
			this.FadeTween = tween;

			// 逆再生開始
			if (this.FadeTween != null)
			{
				this.FadeTween.Sample(1f, false);
				this.FadeTween.PlayReverse();
			}
		}
		#endregion
	}
}
