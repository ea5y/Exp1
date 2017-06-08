using UnityEngine;
using System.Collections;
using System;

namespace XUI.MailItem
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

		/// <summary>
		/// 詳細クリックイベント
		/// </summary>
		event EventHandler OnDetailClick;

		#endregion === Event ===

		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();


		/// <summary>
		/// タイトルセット
		/// </summary>
		void SetTitle(string title);

		/// <summary>
		/// 新着アイコン表示セット
		/// </summary>
		void SetNewIconVisible(bool visible);

		/// <summary>
		/// メールアイコンの表示セット
		/// </summary>
		/// <param name="visible"></param>
		void SetMailIconVisible(bool visible);


		/// <summary>
		/// メールアイコンスプライトセット
		/// </summary>
		/// <param name="atlas"></param>
		/// <param name="spriteName"></param>
		void SetMailIcon(UIAtlas atlas, string spriteName);

		/// <summary>
		/// メールアイコン(アイテム)スプライトセット
		/// </summary>
		/// <param name="atlas"></param>
		/// <param name="spriteName"></param>
		void SetItemIcon(UIAtlas atlas, string spriteName);

		/// <summary>
		/// アイテムの個数表示
		/// </summary>
		/// <param name="visible"></param>
		void SetItemCountVisible(bool visible);

		/// <summary>
		/// アイテムの個数セット
		/// </summary>
		void SetItemCount(string count);

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
		/// 期限 テキストのみ
		/// </summary>
		/// <param name="text"></param>
		void SetDeadline(string text);

		/// <summary>
		/// ロックアイコン表示セット
		/// </summary>
		/// <param name="visible"></param>
		void SetLockIconVisible(bool visible);
	}

	[DisallowMultipleComponent]
	public class MailItemView : GUIViewBase, IView
	{
		#region === Field ===
		
		[SerializeField]
		private UILabel titleLabel = null;

		[SerializeField]
		private UILabel receivedTimeLabel = null;

		[SerializeField]
		private UILabel deadlineLabel = null;

		[SerializeField]
		private GameObject newIconGroup = null;

		[SerializeField]
		private UISprite iconSprite = null;

		[SerializeField]
		private UILabel iconNumLabel = null;
		
		[SerializeField]
		private GameObject lockIcon = null;

		#endregion === Field ===

		#region === Event ===

		/// <summary>
		/// 詳細クリックイベント
		/// </summary>
		public event EventHandler OnDetailClick = (sender, e) => { };

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
			OnDetailClick = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			SetNewIconVisible(true);
			SetLockIconVisible(false);
			SetItemCountVisible(false);
			SetMailIconVisible(true);
			SetDeadlineVisible(false);
		}




		#region === NGUIリフレクション ===

		/// <summary>
		/// 詳細クリック時
		/// </summary>
		public void OnDetailButtonClick()
		{
			OnDetailClick(this, EventArgs.Empty);
		}

		#endregion === NGUIリフレクション ===

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
		/// 新着アイコン表示セット
		/// </summary>
		public void SetNewIconVisible(bool visible)
		{
			if(newIconGroup == null) return;

			if(newIconGroup.activeSelf != visible) {
				newIconGroup.SetActive(visible);
			}
		}

		/// <summary>
		/// メールアイコンの表示セット
		/// </summary>
		/// <param name="visible"></param>
		public void SetMailIconVisible(bool visible)
		{
			if(iconSprite == null) return;

			if(iconSprite.gameObject.activeSelf != visible) {
				iconSprite.gameObject.SetActive(visible);
			}
		}

		/// <summary>
		/// メールアイコンスプライトセット
		/// </summary>
		/// <param name="atlas"></param>
		/// <param name="spriteName"></param>
		public void SetMailIcon(UIAtlas atlas, string spriteName)
		{
			if(iconSprite == null) return;

			CommonIcon.SetIconSprite(iconSprite, atlas, spriteName);
		}


		/// <summary>
		/// メールアイコン(アイテム)スプライトセット
		/// </summary>
		/// <param name="atlas"></param>
		/// <param name="spriteName"></param>
		public void SetItemIcon(UIAtlas atlas, string spriteName)
		{
			if(iconSprite == null) return;

			ItemIcon.SetIconSprite(iconSprite, atlas, spriteName);
		}

		/// <summary>
		/// アイテムの個数表示
		/// </summary>
		/// <param name="visible"></param>
		public void SetItemCountVisible(bool visible)
		{
			if(iconNumLabel == null) return;

			if(iconNumLabel.gameObject.activeSelf != visible) {
				iconNumLabel.gameObject.SetActive(visible);
			}
		}

		/// <summary>
		/// アイテムの個数セット
		/// </summary>
		/// <param name="count"></param>
		public void SetItemCount(string count)
		{
			if(iconNumLabel == null) return;

			iconNumLabel.text = count;
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
		/// 期限 テキストのみ
		/// </summary>
		/// <param name="text"></param>
		public void SetDeadline(string text)
		{
			if(deadlineLabel == null) return;

			deadlineLabel.text = text;
		}


		/// <summary>
		/// ロックアイコン表示セット
		/// </summary>
		/// <param name="visible"></param>
		public void SetLockIconVisible(bool visible)
		{
			if(lockIcon == null) return;

			if(lockIcon.activeSelf != visible) {
				lockIcon.SetActive(visible);
			}
		}
	}
}

