using UnityEngine;
using System.Collections;
using System;

namespace XUI.MailDetail
{

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



		#region === Event ===

		event EventHandler<EventArgs> OnDeleteClick;

		event EventHandler<EventArgs> OnReceiveClick;

		event EventHandler<EventArgs> OnLockClick;

		event EventHandler<EventArgs> OnCloseClick;

		#endregion === Event ===


		void Reposition();

		/// <summary>
		/// タイトルセット
		/// </summary>
		/// <param name="title"></param>
		void SetTitle(string title);
		
		/// <summary>
		/// 本文セット
		/// </summary>
		/// <param name="body"></param>
		void SetBody(string body);
		
		/// <summary>
		/// 受信時間
		/// </summary>
		/// <param name="time"></param>
		/// <param name="format"></param>
		void SetReceivedTime(DateTime time, string format);

		/// <summary>
		/// 期限表示セット
		/// </summary>
		/// <param name="visible"></param>
		void SetDeadlineVisible(bool visible);

		/// <summary>
		/// 期限
		/// </summary>
		/// <param name="time"></param>
		/// <param name="format"></param>
		void SetDeadline(DateTime time, string format);

		/// <summary>
		/// アイテム部分の表示をセット
		/// </summary>
		/// <param name="visible"></param>
		void SetItemVisible(bool visible);

		/// <summary>
		/// アイテム名表示
		/// </summary>
		/// <param name="name"></param>
		void SetItemName(string name);

		/// <summary>
		/// アイテム数表示
		/// </summary>
		/// <param name="count"></param>
		/// <param name="format"></param>
		void SetItemCount(int count, string format);

		// void SetItemIcon();

		/// <summary>
		/// アイテム部の受け取り済み表示
		/// </summary>
		/// <param name="visible"></param>
		void SetItemReceivedFlag(bool flag);


		/// <summary>
		/// 受け取りボタンの表示切り替え
		/// </summary>
		/// <param name="visible"></param>
		void SetReceiveButtonVisible(bool visible);

		/// <summary>
		/// 削除ボタンの表示切り替え
		/// </summary>
		/// <param name="visible"></param>
		void SetDeleteButtonVisible(bool visible);

		/// <summary>
		/// 削除ボタンの有効切り替え
		/// </summary>
		/// <param name="enable"></param>
		void SetDeleteButtonEnable(bool enable);

		/// <summary>
		/// ロック設定
		/// </summary>
		/// <param name="locked"></param>
		void SetLockFlag(bool locked);

	}

	[DisallowMultipleComponent]
	public class MailDetailView : GUIViewBase, IView
	{
		#region === Field ===

		[SerializeField]
		private UIButton receiveButton = null;

		[SerializeField]
		private UIButton deleteButton = null;
		
		[SerializeField]
		private UISprite lockSprite = null;

		[SerializeField]
		private UISprite unlockSprite = null;

		[SerializeField]
		private UISprite unlockBGSprite = null;

		[SerializeField]
		private UILabel titleLabel = null;

		[SerializeField]
		private UILabel receivedTimeLabel = null;

		[SerializeField]
		private UILabel deadlineLabel = null;

		[SerializeField]
		private UIScrollView scrollView = null;

		[SerializeField]
		private UITable bodyTable = null;

		[SerializeField]
		private UILabel bodyLabel = null;

		[SerializeField]
		private GameObject itemGroup = null;
		
		[SerializeField]
		private UILabel itemNameLabel = null;
		
		[SerializeField]
		private UILabel itemCountLabel = null;
		
		[SerializeField]
		private UILabel itemReceivedLabel = null;

		#endregion === Field ===


		#region === Event ===

		public event EventHandler<EventArgs> OnDeleteClick = (sender, e) => { };

		public event EventHandler<EventArgs> OnReceiveClick = (sender, e) => { };

		public event EventHandler<EventArgs> OnLockClick = (sender, e) => { };

		public event EventHandler<EventArgs> OnCloseClick = (sender, e) => { };

		#endregion === Event ===


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
			OnDeleteClick = null;
			OnReceiveClick = null;
			OnLockClick = null;
			OnCloseClick = null;
		}

		#region === NGUIリフレクション ===

		/// <summary>
		/// 表示アニメーション終了時
		/// </summary>
		public void OnTweenFinished()
		{
			Reposition();
		}

		/// <summary>
		/// 削除ボタンクリック時
		/// </summary>
		public void OnDeleteButtonClick()
		{
			OnDeleteClick(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// 受け取りボタンクリック時
		/// </summary>
		public void OnReceiveButtonClick()
		{
			OnReceiveClick(this, EventArgs.Empty);
		}

		/// <summary>
		/// ロックボタンクリック時
		/// </summary>
		public void OnLockButtonClick()
		{
			OnLockClick(this, EventArgs.Empty);
		}


		/// <summary>
		/// 閉じるボタン押したとき
		/// </summary>
		public void OnCloseButtonClick()
		{
			OnCloseClick(this, EventArgs.Empty);
		}


		#endregion === NGUIリフレクション ===

		public void Reposition()
		{
			if(scrollView != null) {
				scrollView.ResetPosition();
			}

			if(bodyTable != null) {
				//bodyTable.Reposition();
				bodyTable.repositionNow = true;
			}
		}


		/// <summary>
		/// タイトルセット
		/// </summary>
		/// <param name="title"></param>
		public void SetTitle(string title)
		{
			if(titleLabel == null) return;

			titleLabel.text = title;
		}

		/// <summary>
		/// 本文セット
		/// </summary>
		/// <param name="body"></param>
		public void SetBody(string body)
		{
			if(bodyLabel == null) return;

			bodyLabel.text = body;
		}


		/// <summary>
		/// 受信時間
		/// </summary>
		/// <param name="time"></param>
		/// <param name="format"></param>
		public void SetReceivedTime(DateTime time, string format)
		{
			if(receivedTimeLabel == null) return;

			receivedTimeLabel.text = time.ToString(format);
		}


		/// <summary>
		/// 期限表示セット
		/// </summary>
		/// <param name="visible"></param>
		public void SetDeadlineVisible(bool visible)
		{
			if(deadlineLabel == null) return;

			if(deadlineLabel.gameObject.activeSelf != visible) {
				deadlineLabel.gameObject.SetActive(visible);
			}
		}

		/// <summary>
		/// 期限
		/// </summary>
		/// <param name="time"></param>
		/// <param name="format"></param>
		public void SetDeadline(DateTime time, string format)
		{
			if(deadlineLabel == null) return;

			deadlineLabel.text = time.ToString(format);
		}



		/// <summary>
		/// アイテム部分の表示をセット
		/// </summary>
		/// <param name="visible"></param>
		public void SetItemVisible(bool visible)
		{
			if(itemGroup == null) return;

			if(itemGroup.activeSelf != visible) {
				itemGroup.SetActive(visible);
			}
		}

		/// <summary>
		/// アイテム名表示
		/// </summary>
		/// <param name="name"></param>
		public void SetItemName(string name)
		{
			if(itemNameLabel == null) return;

			itemNameLabel.text = name;
		}

		/// <summary>
		/// アイテム数表示
		/// </summary>
		/// <param name="count"></param>
		/// <param name="format"></param>
		public void SetItemCount(int count, string format)
		{
			if(itemCountLabel == null) return;

			itemCountLabel.text = string.Format(format, count);
		}

		// void SetItemIcon();

		/// <summary>
		/// アイテム部の受け取り済み表示
		/// </summary>
		/// <param name="received">受け取り済みか</param>
		public void SetItemReceivedFlag(bool received)
		{
			// 受け取り済み表示
			if(itemReceivedLabel != null) {
				if(itemReceivedLabel.gameObject.activeSelf != received) {
					itemReceivedLabel.gameObject.SetActive(received);
				}
			}
		}


		/// <summary>
		/// 受け取りボタンの表示切り替え
		/// </summary>
		/// <param name="visible"></param>
		public void SetReceiveButtonVisible(bool visible)
		{
			if(receiveButton == null) return;

			if(receiveButton.gameObject.activeSelf != visible) {
				receiveButton.gameObject.SetActive(visible);
			}
		}

		/// <summary>
		/// 削除ボタンの表示切り替え
		/// </summary>
		/// <param name="visible"></param>
		public void SetDeleteButtonVisible(bool visible)
		{
			if(deleteButton == null) return;

			if(deleteButton.gameObject.activeSelf != visible) {
				deleteButton.gameObject.SetActive(visible);
			}
		}

		/// <summary>
		/// 削除ボタンの有効切り替え
		/// </summary>
		/// <param name="enable"></param>
		public void SetDeleteButtonEnable(bool enable)
		{
			if(deleteButton == null) return;

			deleteButton.isEnabled = enable;
		}

		/// <summary>
		/// ロックの切り替え
		/// </summary>
		/// <param name="locked"></param>
		public void SetLockFlag(bool locked)
		{
			if(lockSprite == null || unlockBGSprite == null || unlockSprite == null) return;

			if(locked) {
				lockSprite.gameObject.SetActive(true);
				unlockBGSprite.gameObject.SetActive(false);
				unlockSprite.gameObject.SetActive(false);
			} else {
				lockSprite.gameObject.SetActive(false);
				unlockBGSprite.gameObject.SetActive(true);
				unlockSprite.gameObject.SetActive(true);
			}
		}
	}
}

