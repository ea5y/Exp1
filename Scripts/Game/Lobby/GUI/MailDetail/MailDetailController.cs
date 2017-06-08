using UnityEngine;
using System.Collections;
using System;

namespace XUI.MailDetail
{
	/// <summary>
	/// メールイベント引数
	/// </summary>
	public class MailEventArgs : EventArgs
	{
		/// <summary>
		/// メールの種類
		/// </summary>
		public MailInfo.MailType Type { get; private set; }

		/// <summary>
		/// メールのサーバインデックス
		/// </summary>
		public int Index { get; private set; }

		public MailEventArgs(MailInfo.MailType type, int index)
		{
			Type = type;
			Index = index;
		}
	}

	/// <summary>
	/// メールアンロックイベント引数
	/// </summary>
	public class MailUnlockEventArgs : EventArgs
	{
		/// <summary>
		/// メールの種類
		/// </summary>
		public MailInfo.MailType Type { get; private set; }

		/// <summary>
		/// メールのサーバインデックス
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// 保持期限切れ
		/// </summary>
		public bool OverKeepDays { get; private set; }

		public MailUnlockEventArgs(MailInfo.MailType type, int index, bool over)
		{
			Type = type;
			Index = index;
			OverKeepDays = over;
		}
	}
	

	public interface IController
	{
		#region === Event ===

		/// <summary>
		/// メール削除イベント
		/// </summary>
		event EventHandler<MailEventArgs> OnMailDelete;

		/// <summary>
		/// メールロックイベント
		/// </summary>
		event EventHandler<MailEventArgs> OnMailLock;

		/// <summary>
		/// メールアンロックイベント
		/// </summary>
		event EventHandler<MailUnlockEventArgs> OnMailUnlock;

		/// <summary>
		/// アイテム受け取りイベント
		/// </summary>
		event EventHandler<MailEventArgs> OnItemReceive;

		#endregion === Event ===


		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();
		
		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// メールインフォセット
		/// </summary>
		void SetMailInfo(MailInfo mail);

		/// <summary>
		/// メールのロック件数セット
		/// </summary>
		void SetLockCount(int count);

		/// <summary>
		/// ロックフラグの表示を更新
		/// </summary>
		void UpdateLockFlag();

		/// <summary>
		/// アイテム受取り時
		/// </summary>
		/// <param name="result"></param>
		void ItemReceived(Scm.Common.GameParameter.ReceivePresentMailItemResult result);

		void DeleteMail();
	}

	public class Controller : IController
	{

		#region === 文字列 ===

		private string MailDetailStr_DeletedMessage { get { return MasterData.GetText(TextType.TX396_MailDetail_MailDelete_Message); } }

		private string MailDetailStr_OverLockCountMessage { get { return MasterData.GetText(TextType.TX397_MailDetail_OverLockCount_Message); } }

		private string MailDetailStr_OverKeepDaysMessage { get { return MasterData.GetText(TextType.TX398_MailDetail_OverKeepDays_Message); } }


		private string MailStr_ItemReceivedMessage { get { return MasterData.GetText(TextType.TX399_MailDetail_ItemReceived_Message); } }

		// 保留
		private string MailStr_ItemReceivedFailMessage { get { return "アイテムの受け取りに失敗しました。"; } }

		private string MailStr_OverReceiveDeadlineMessage { get { return MasterData.GetText(TextType.TX400_MailDetail_OverReceiveDeadline_Message); } }


		// MailItemControllerと同一

		private string MailStr_ReceivedTimeFormat { get { return MasterData.GetText(TextType.TX387_Mail_ReceivedTime_Format); } }

		private string MailStr_ItemReceived { get { return MasterData.GetText(TextType.TX401_Mail_ItemReceived); } }

		private string MailStr_ItemExpiration { get { return MasterData.GetText(TextType.TX388_Mail_ItemExpiration); } }

		private string MailStr_ItemNoneDeadline { get { return MasterData.GetText(TextType.TX389_Mail_ItemNoneDeadline); } }

		private string MailStr_ItemDeadlineDaysFormat { get { return MasterData.GetText(TextType.TX390_Mail_ItemReceive_Days_Format); } }

		private string MailStr_ItemDeadlineHoursFormat { get { return MasterData.GetText(TextType.TX391_Mail_ItemReceive_Hours_Format); } }

		private string MailStr_ItemDeadlineMinutesFormat { get { return MasterData.GetText(TextType.TX392_Mail_ItemReceive_Minutes_Format); } }

		private string MailStr_ItemDeadlineSecondsFormat { get { return MasterData.GetText(TextType.TX393_Mail_ItemReceive_Seconds_Format); } }

		#endregion === 文字列 ===


		#region === Field ===

		// モデル
		private readonly IModel model;

		// ビュー
		private readonly IView view;

		private GUIItem presentItem;

		#endregion === Field ===

		#region === Property ===

		private IModel Model { get { return model; } }

		private IView View { get { return view; } }

		private GUIItem PresentItem { get { return presentItem; } }

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		private bool CanUpdate
		{
			get
			{
				if(this.Model == null) return false;
				if(this.View == null) return false;
				return true;
			}
		}


		#endregion === Property ===

		#region === Event ===

		/// <summary>
		/// メール削除イベント
		/// </summary>
		public event EventHandler<MailEventArgs> OnMailDelete = (sender, e) => { };

		/// <summary>
		/// メールロックイベント
		/// </summary>
		public event EventHandler<MailEventArgs> OnMailLock = (sender, e) => { };

		/// <summary>
		/// メールアンロックイベント
		/// </summary>
		public event EventHandler<MailUnlockEventArgs> OnMailUnlock = (sender, e) => { };

		/// <summary>
		/// アイテム受け取りイベント
		/// </summary>
		public event EventHandler<MailEventArgs> OnItemReceive = (sender, e) => { };

		#endregion === Event ===




		public Controller(IModel model, IView view, GUIItem presentItem)
		{
			if(model == null || view == null) return;

			// ビュー設定
			this.view = view;
			View.OnCloseClick += HandleCloseClick;
			View.OnReceiveClick += HandleReceiveClick;
			View.OnDeleteClick += HandleDeleteClick;
			View.OnLockClick += HandleLockClick;

			// モデル設定
			this.model = model;
			Model.OnMailInfoChange += HandolMailInfoChange;
			Model.OnTitleChange += HandleTitleChange;
			Model.OnBodyChange += HandleBodyChange;
			Model.OnReceivedTimeChange += HandleReceivedTimeChange;
			Model.OnLockFlagChange += HandleLockFlagChange;
			Model.OnItemInfoChange += HandleItemInfoChange;

			this.presentItem = presentItem;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			if(!CanUpdate) return;

			Model.Setup();

			SyncLockFlag();

			SyncMailInfo();

			View.Reposition();
		}

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			// モデル破棄
			if(Model != null) {
				Model.Dispose();
			}

			OnMailDelete = null;
			OnMailLock = null;
			OnMailUnlock = null;
		}

		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if(this.CanUpdate) {
				this.View.SetActive(isActive, isTweenSkip);

				if(!isActive) {
					SetMailInfo(null);
				}
			}
		}

		/// <summary>
		/// メールインフォセット
		/// </summary>
		public void SetMailInfo(MailInfo mail)
		{
			if(!CanUpdate) return;

			Model.MailInfo = mail;
		}

		/// <summary>
		/// メールのロック件数セット
		/// </summary>
		public void SetLockCount(int count)
		{
			if(!CanUpdate) return;

			Model.LockCount = count;
		}



		/// <summary>
		/// アイテム受取り時
		/// </summary>
		/// <param name="result"></param>
		public void ItemReceived(Scm.Common.GameParameter.ReceivePresentMailItemResult result)
		{
			if(!CanUpdate) return;

			if(Model.MailInfo == null) return;

			if(result == Scm.Common.GameParameter.ReceivePresentMailItemResult.Success) {
				// 確認表示
				GUIMessageWindow.SetModeOK(
					MailStr_ItemReceivedMessage,
					() =>
					{
						Model.MailInfo.ItemReceive();
						SyncMailInfo();
						// アイテムリスト更新
						GUIMail.UpdateCurrentList();

					}
				);
			} else if(result == Scm.Common.GameParameter.ReceivePresentMailItemResult.Expiration) {
				// 期限切れ
				GUIMessageWindow.SetModeOK(
					MailStr_OverReceiveDeadlineMessage,
					() =>
					{
						SyncMailInfo();
						// アイテムリスト更新
						GUIMail.UpdateCurrentList();
					}
				);
			} else {
				//// 失敗
				//GUIMessageWindow.SetModeOK(
				//	MailStr_ItemReceivedFailMessage,
				//	null
				//);
			}
		}



		/// <summary>
		/// メール更新時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandolMailInfoChange(object sender, EventArgs e)
		{
			SyncMailInfo();
		}

		/// <summary>
		/// メール情報をビューに反映
		/// </summary>
		private void SyncMailInfo()
		{
			if(!CanUpdate) return;

			if(Model.MailInfo == null) return;

			// タイトル
			Model.Title = Model.MailInfo.Title;

			// 受信時間
			Model.ReceivedTime = Model.MailInfo.ReceivedTime;

			// 本文セット
			Model.Body = Model.MailInfo.Body;

			// ロックフラグセット
			Model.IsLock = Model.MailInfo.IsLocked;

			// アイテム更新
			Model.ItemInfo = Model.MailInfo.ItemInfo;




			// アイコン
			// プレハブが使えるっぽい


			// アイテムがあれば期限表示
			if(Model.MailInfo.HasItem) {
				PresentItem.SetState(Item.ItemStateType.ItemIcon, Model.MailInfo.ItemInfo);

				// アイテム部分の表示
				View.SetItemVisible(true);
				View.SetDeadlineVisible(true);

				// アイテム名とか
				View.SetItemName(Model.MailInfo.ItemInfo.Name);
				View.SetItemCount(Model.MailInfo.ItemInfo.Stack, Model.ItemCountFormat);

				// ボタンを削除にしておく
				View.SetReceiveButtonVisible(false);
				View.SetDeleteButtonVisible(true);


				// 期限
				string format = MailStr_ItemExpiration;
				var time = new DateTime(0);

				if(Model.MailInfo.IsItemReceived) {
					// 受取り済み
					format = MailStr_ItemReceived;
					View.SetItemReceivedFlag(true);

				} else if(!Model.MailInfo.HasDeadline) {
					// 期限がない
					format = MailStr_ItemNoneDeadline;

					View.SetItemReceivedFlag(false);
					View.SetReceiveButtonVisible(true);
					View.SetDeleteButtonVisible(false);

				} else if(!Model.MailInfo.IsReceiveExpiration) {
					// 期限付きで、期限が切れていない時
					View.SetItemReceivedFlag(false);

					View.SetReceiveButtonVisible(true);
					View.SetDeleteButtonVisible(false);

					var span = Model.MailInfo.ItemDeadlineTimeSpan;

					if(span.TotalSeconds >= 0) {
						time = time.AddSeconds(span.TotalSeconds);
					}

					if(span.TotalDays > 0) {
						format = MailStr_ItemDeadlineDaysFormat;
					} else if(span.TotalHours > 0) {
						format = MailStr_ItemDeadlineHoursFormat;
					} else if(span.TotalMinutes > 0) {
						format = MailStr_ItemDeadlineMinutesFormat;
					} else if(span.TotalSeconds >= 0) {
						format = MailStr_ItemDeadlineSecondsFormat;
					}
				} else {
					// 期限切れ
					View.SetItemReceivedFlag(false);
				}


				View.SetDeadline(time, format);

			} else {
				// アイテム部分の非表示
				View.SetItemVisible(false);
				View.SetDeadlineVisible(false);

				View.SetReceiveButtonVisible(false);
				View.SetDeleteButtonVisible(true);
			}

			View.Reposition();
		}

		#region === Title ===

		/// <summary>
		/// タイトル変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleTitleChange(object sender, EventArgs e)
		{
			SyncTitle();
		}

		/// <summary>
		/// タイトル反映
		/// </summary>
		private void SyncTitle()
		{
			if(!CanUpdate) return;

			View.SetTitle(Model.Title);
		}

		#endregion === Title ===

		#region === Body ===

		/// <summary>
		/// 本文変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleBodyChange(object sender, EventArgs e)
		{
			SyncBody();
		}

		/// <summary>
		/// 本文反映
		/// </summary>
		private void SyncBody()
		{
			if(!CanUpdate) return;

			View.SetBody(Model.Body);
		}

		#endregion === Body ===

		#region === Receive Time ===

		/// <summary>
		/// 受信時間変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleReceivedTimeChange(object sender, EventArgs e)
		{
			SyncReceiveTime();
		}

		/// <summary>
		/// 受信時間反映
		/// </summary>
		private void SyncReceiveTime()
		{
			if(!CanUpdate) return;

			// 受信時間
			View.SetReceivedTime(Model.ReceivedTime, MailStr_ReceivedTimeFormat);
		}

		#endregion === Receive Time ===

		#region === Lock Flag ===

		/// <summary>
		/// ロックフラグ変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleLockFlagChange(object sender, EventArgs e)
		{
			SyncLockFlag();
		}

		/// <summary>
		/// ロックアイコン反映
		/// </summary>
		private void SyncLockFlag()
		{
			if(!CanUpdate) return;

			View.SetLockFlag(Model.IsLock);
			View.SetDeleteButtonEnable(!Model.IsLock);
		}

		#endregion === Lock Flag ===
		
		#region === ItemInfo ===

		/// <summary>
		/// 添付アイテム変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleItemInfoChange(object sender, EventArgs e)
		{
			SyncItemInfo();
		}
		
		/// <summary>
		/// 添付アイテム情報反映
		/// </summary>
		private void SyncItemInfo()
		{
			if(!CanUpdate) return;

			// アイテムがあれば期限表示
			if(Model.ItemInfo != null) {
				// アイコン
				PresentItem.SetState(Item.ItemStateType.ItemIcon, Model.ItemInfo);

				// アイテム部分の表示
				View.SetItemVisible(true);
				View.SetDeadlineVisible(true);

				// アイテム名とか
				View.SetItemName(Model.ItemInfo.Name);
				View.SetItemCount(Model.ItemInfo.Stack, Model.ItemCountFormat);

				// ボタンを削除にしておく
				View.SetReceiveButtonVisible(false);
				View.SetDeleteButtonVisible(true);


				// 期限
				string format = MailStr_ItemExpiration;
				var time = new DateTime(0);

				if(Model.MailInfo.IsItemReceived) {
					// 受取り済み
					format = MailStr_ItemReceived;
					View.SetItemReceivedFlag(true);

				} else if(!Model.MailInfo.HasDeadline) {
					// 期限がない
					format = MailStr_ItemNoneDeadline;

					View.SetReceiveButtonVisible(true);
					View.SetDeleteButtonVisible(false);

				} else if(!Model.MailInfo.IsReceiveExpiration) {
					// 期限付きで、期限が切れていない時
					View.SetItemReceivedFlag(false);

					View.SetReceiveButtonVisible(true);
					View.SetDeleteButtonVisible(false);

					var span = Model.MailInfo.ItemDeadlineTimeSpan;

					if(span.TotalSeconds >= 0) {
						time = time.AddSeconds(span.TotalSeconds);
					}

					if(span.TotalDays > 0) {
						format = MailStr_ItemDeadlineDaysFormat;
					} else if(span.TotalHours > 0) {
						format = MailStr_ItemDeadlineHoursFormat;
					} else if(span.TotalMinutes > 0) {
						format = MailStr_ItemDeadlineMinutesFormat;
					} else if(span.TotalSeconds >= 0) {
						format = MailStr_ItemDeadlineSecondsFormat;
					}
				} else {
					// 期限切れ
					View.SetItemReceivedFlag(false);
				}

				View.SetDeadline(time, format);

			} else {

				// アイテム部分の非表示
				View.SetItemVisible(false);
				View.SetDeadlineVisible(false);

				View.SetReceiveButtonVisible(false);
				View.SetDeleteButtonVisible(true);
			}
		}
		
		#endregion === ItemInfo ===


		/// <summary>
		/// 閉じる時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleCloseClick(object sender, EventArgs e)
		{
			GUIMailDetail.Close();
		}


		/// <summary>
		/// ロッククリック時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleLockClick(object sender, EventArgs e)
		{
			// GUIMailにロックするのを通知
			if(!CanUpdate) return;

			if(Model.MailInfo.IsLocked) {
				// ロック解除
				MailUnlock();
			} else {
				// ロック
				MailLock();
			}
			
		}

		/// <summary>
		/// メールロックを行う
		/// </summary>
		private void MailLock()
		{
			if(Model.OverLockCount) {
				// ロック件数が超えている
				GUIMessageWindow.SetModeOK(
					string.Format(MailDetailStr_OverLockCountMessage, Model.MaxLockCount),
					null
				);
				return;
			}
			
			// ロックイベント
			OnMailLock(this, new MailEventArgs(Model.MailInfo.Type, Model.MailInfo.Index));
		}

		/// <summary>
		/// メールのロック解除
		/// </summary>
		private void MailUnlock()
		{
			if(Model.OverKeepDays) {
				// 受信期限越え
				GUIMessageWindow.SetModeYesNo(
					string.Format(MailDetailStr_OverKeepDaysMessage, Model.KeepDays),
					() => OnMailUnlock(this, new MailUnlockEventArgs(Model.MailInfo.Type, Model.MailInfo.Index, Model.OverKeepDays)),
					null
				);
				return;
			}

			// アンロックイベント
			OnMailUnlock(this, new MailUnlockEventArgs(Model.MailInfo.Type, Model.MailInfo.Index, Model.OverKeepDays));
		}


		/// <summary>
		/// ロックフラグの表示を更新する
		/// </summary>
		public void UpdateLockFlag()
		{
			if(!CanUpdate) return;

			View.SetLockFlag(Model.MailInfo.IsLocked);
			View.SetDeleteButtonEnable(!Model.MailInfo.IsLocked);
		}

		/// <summary>
		/// 受け取りクリック時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleReceiveClick(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			OnItemReceive(this, new MailEventArgs(Model.MailInfo.Type, Model.MailInfo.Index));
		}

		/// <summary>
		/// 削除クリック時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleDeleteClick(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			OnMailDelete(this, new MailEventArgs(Model.MailInfo.Type, Model.MailInfo.Index));
		}

		/// <summary>
		/// 削除時
		/// </summary>
		public void DeleteMail()
		{
			if(!CanUpdate) return;

			GUIMessageWindow.SetModeOK(
				MailDetailStr_DeletedMessage,
				() => GUIMailDetail.Close()
			);
		}
	}
}

