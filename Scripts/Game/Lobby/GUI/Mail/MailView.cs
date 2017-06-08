using UnityEngine;
using System.Collections;
using System;

namespace XUI.Mail
{
	/// <summary>
	/// タブ変更イベント引数
	/// </summary>
	public class TabChangeEventArgs : EventArgs
	{
		/// <summary>
		/// 変更後のタイプ
		/// </summary>
		public MailTabType TabType { get; private set; }

		public TabChangeEventArgs(MailTabType type)
		{
			TabType = type;
		}
	}

	public interface IView
	{
		#region === アクティブ ===

		/// <summary>
		/// アクティブ状態を取得する
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();

		#endregion === アクティブ ===

		#region === ホーム/閉じる ===

		event EventHandler<EventArgs> OnHome;

		event EventHandler<EventArgs> OnClose;

		#endregion === ホーム/閉じる ===

		#region === Event ===

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		event EventHandler<TabChangeEventArgs> OnTabChange;

		/// <summary>
		/// まとめて削除クリックイベント
		/// </summary>
		event EventHandler<EventArgs> OnAllDeleteClick;

		/// <summary>
		/// まとめて既読クリックイベント
		/// </summary>
		event EventHandler<EventArgs> OnAllReadClick;

		/// <summary>
		/// まとめて受け取りクリックイベント
		/// </summary>
		event EventHandler<EventArgs> OnAllItemReceiveClick;

		#endregion === Event ===
		
		/// <summary>
		/// 未読運営メール件数変更
		/// </summary>
		void SetUnreadMailCount(int count, string format);

		/// <summary>
		/// 未読アイテムメール件数変更
		/// </summary>
		void SetUnreadItemMailCount(int count, string format);

		/// <summary>
		/// タブ変更
		/// </summary>
		/// <param name="type"></param>
		void SetTabMode(MailTabType type);

		/// <summary>
		/// ボタンの有効変更
		/// </summary>
		/// <param name="enable"></param>
		void SetButtonEnable(bool enable);
	}


	[DisallowMultipleComponent]
	public class MailView : GUIScreenViewBase, IView
	{

		private const int MaxUnreadMailCount = 999;


		#region === Field ===

		[SerializeField]
		private UILabel mailCountLabel;

		[SerializeField]
		private UILabel itemMailCountLabel;

		[SerializeField]
		private UIButton mailTabButton;
		
		[SerializeField]
		private UIButton itemMailTabButton;

		[SerializeField]
		private UIButton allDeleteButton;

		[SerializeField]
		private UIButton allReceiveButton;

		[SerializeField]
		private UIButton allReadButton;

		#endregion === Field ===


		#region === アクティブ ===

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

		#endregion === アクティブ ===

		/// <summary>
		/// オブジェクトが破棄されたとき
		/// </summary>
		private void OnDestroy()
		{
			OnHome = null;
			OnClose = null;
		}


		#region === ホーム/閉じる ===

		public event EventHandler<EventArgs> OnHome = (sender, e) => { };

		public event EventHandler<EventArgs> OnClose = (sender, e) => { };

		/// <summary>
		/// ホームボタンイベント
		/// </summary>
		public override void OnHomeEvent()
		{
			// 通知
			this.OnHome(this, EventArgs.Empty);
		}

		/// <summary>
		/// 閉じるボタンイベント
		/// </summary>
		public override void OnCloseEvent()
		{
			// 通知
			this.OnClose(this, EventArgs.Empty);
		}

		#endregion === ホーム/閉じる ===


		#region === Event ===

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		public event EventHandler<TabChangeEventArgs> OnTabChange = (sender, e) => { };

		/// <summary>
		/// まとめて削除クリックイベント
		/// </summary>
		public event EventHandler<EventArgs> OnAllDeleteClick = (sender, e) => { };

		/// <summary>
		/// まとめて既読クリックイベント
		/// </summary>
		public event EventHandler<EventArgs> OnAllReadClick = (sender, e) => { };

		/// <summary>
		/// まとめて受け取りクリックイベント
		/// </summary>
		public event EventHandler<EventArgs> OnAllItemReceiveClick = (sender, e) => { };

		#endregion === Event ===

		
		#region === NGUIリフレクション ===

		/// <summary>
		/// メールタブクリック
		/// </summary>
		public void OnMailTabButtonClick()
		{
			OnTabChange(this, new TabChangeEventArgs(MailTabType.Mail));
		}

		/// <summary>
		/// アイテムメールタブクリック
		/// </summary>
		public void OnItemMailTabButtonClick()
		{
			OnTabChange(this, new TabChangeEventArgs(MailTabType.Item));
		}

		/// <summary>
		/// 全削除クリック
		/// </summary>
		public void OnAllDeleteButtonClick()
		{
			OnAllDeleteClick(this, EventArgs.Empty);
		}

		/// <summary>
		/// 全既読クリック
		/// </summary>
		public void OnAllReadButtonClick()
		{
			OnAllReadClick(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// 全既読クリック
		/// </summary>
		public void OnAllReceiveButtonClick()
		{
			OnAllItemReceiveClick(this, EventArgs.Empty);
		}


		#endregion === NGUIリフレクション ===

		/// <summary>
		/// 未読運営メール件数変更
		/// </summary>
		/// <param name="count"></param>
		/// <param name="format"></param>
		public void SetUnreadMailCount(int count, string format)
		{
			if(mailCountLabel == null) return;
			
			mailCountLabel.text = string.Format(format, Mathf.Clamp(count, 0, MaxUnreadMailCount));
		}

		/// <summary>
		/// 未読アイテムメール件数変更
		/// </summary>
		/// <param name="count"></param>
		/// <param name="format"></param>
		public void SetUnreadItemMailCount(int count, string format)
		{
			if(itemMailCountLabel == null) return;

			itemMailCountLabel.text = string.Format(format, Mathf.Clamp(count, 0, MaxUnreadMailCount));
		}

		/// <summary>
		/// タブ変更
		/// </summary>
		/// <param name="type"></param>
		public void SetTabMode(MailTabType type)
		{
			switch(type) {
				case MailTabType.Mail:
					if(allReadButton != null) allReadButton.gameObject.SetActive(true);
					if(allReceiveButton!= null) allReceiveButton.gameObject.SetActive(false);
					if(mailTabButton != null) mailTabButton.isEnabled = false;
					if(itemMailTabButton != null) itemMailTabButton.isEnabled = true;
					break;
				case MailTabType.Item:
					if(allReadButton != null) allReadButton.gameObject.SetActive(false);
					if(allReceiveButton != null) allReceiveButton.gameObject.SetActive(true);
					if(mailTabButton != null) mailTabButton.isEnabled = true;
					if(itemMailTabButton != null) itemMailTabButton.isEnabled = false;
					break;
				default:
					if(allReadButton != null) allReadButton.gameObject.SetActive(false);
					if(allReceiveButton != null) allReceiveButton.gameObject.SetActive(false);
					break;
			}
		}

		/// <summary>
		/// ボタンの有効変更
		/// </summary>
		public void SetButtonEnable(bool enable)
		{
			if(allDeleteButton != null) allDeleteButton.isEnabled = enable;
			if(allReadButton != null) allReadButton.isEnabled = enable;
			if(allReceiveButton != null) allReceiveButton.isEnabled = enable;
		}
	}
}

