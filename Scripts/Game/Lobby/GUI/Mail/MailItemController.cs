using UnityEngine;
using System.Collections;
using System;

namespace XUI.MailItem
{
	public class MailDetailEventArgs : EventArgs
	{
		public MailInfo MailInfo { get; private set; }

		public MailDetailEventArgs(MailInfo mail)
		{
			MailInfo = mail;
		}
	}

	public interface IController
	{
		event EventHandler<MailDetailEventArgs> OnDetailClick;

		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive);
		
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();
		
		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		void SetMailInfo(MailInfo mail);

		void UpdateMailInfo();
	}

	public class Controller : IController
	{

		#region === 文字列 ===
		
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


		private ItemIcon itemIcon;

		private CommonIcon commonIcon;


		#endregion === Field ===

		#region === Property ===

		private IModel Model { get { return model; } }

		private IView View { get { return view; } }

		private CommonIcon CommonIcon { get { return commonIcon; } }

		private ItemIcon ItemIcon { get { return itemIcon; } }


		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		private bool CanUpdate
		{
			get
			{
				if (this.Model == null) return false;
				if (this.View == null) return false;
				return true;
			}
		}
		
		#endregion === Property ===


		#region === Event ===

		public event EventHandler<MailDetailEventArgs> OnDetailClick = (sender, e) => { };

		#endregion === Event ===



		public Controller(IModel model, IView view, ItemIcon itemIcon, CommonIcon commonIcon)
		{
			if (model == null || view == null) return;

			// ビュー設定
			this.view = view;
			View.OnDetailClick += HandleDetailClick;

			// モデル設定
			this.model = model;
			Model.OnMailInfoChange += HandleMailInfoChange;
			Model.OnTitleChange += HandleTitleChange;
			Model.OnReceivedTimeChange += HandleReceiveTimeChange;
			Model.OnReadFlagChange += HandleReadFlagChange;
			Model.OnLockFlagChange += HandleLockFlagChange;
			Model.OnMailIconIdChange += HandleMailIconIdChange;
			Model.OnItemInfoChange += HandleItemInfoChange;
			Model.OnItemReceivedChange += HandleItemReceivedChange;
			Model.OnReceiveExpirationChange += HandleReceiveExpirationChange;
			Model.OnItemDeadlineChange += HandleItemDeadlineChange;


			View.SetDeadline(MailStr_ItemNoneDeadline);

			this.itemIcon = itemIcon;
			this.commonIcon = commonIcon;

		}


		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			if(!CanUpdate) return;

			View.Setup();
			Model.Setup();
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

			OnDetailClick = null;
		}
		
		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive(bool isActive)
		{
			if(!CanUpdate) return;
			
			this.View.SetActive(isActive, true);
		}

		/// <summary>
		/// メール情報をセットする
		/// </summary>
		/// <param name="mailInfo"></param>
		public void SetMailInfo(MailInfo mail)
		{
			if(!CanUpdate) return;

			Model.MailInfo = mail;
		}


		public void UpdateMailInfo()
		{
			SyncMailInfo();
		}

		private void HandleMailInfoChange(object sender, EventArgs e)
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

			// 新着
			Model.IsRead = Model.MailInfo.IsRead;

			// ロックアイコン
			Model.IsLock = Model.MailInfo.IsLocked;

			// アイテム更新
			Model.ItemInfo = Model.MailInfo.ItemInfo;

			if(Model.ItemInfo != null) {
				Model.MailIconId = 0;

				// アイテム受け取り済み変更
				Model.IsItemReceived = Model.MailInfo.IsItemReceived;

				// アイテム期限切れ変更
				Model.IsReceiveExpiration = Model.MailInfo.IsReceiveExpiration;

				// 期限セット
				Model.DeadlineTime = Model.MailInfo.ItemDeadlineTime;
			} else {
				Model.MailIconId = Model.MailInfo.IconID;
			}
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

		#region === Receive Time ===
		
		/// <summary>
		/// 受信時間変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleReceiveTimeChange(object sender, EventArgs e)
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

		#region === Read Flag ===
		
		/// <summary>
		/// 既読フラグ変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleReadFlagChange(object sender, EventArgs e)
		{
			SyncReadFlag();
		}

		/// <summary>
		/// 既読アイコン反映
		/// </summary>
		private void SyncReadFlag()
		{
			if(!CanUpdate) return;

			View.SetNewIconVisible(!Model.IsRead);
		}

		#endregion === Read Flag ===

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

			View.SetLockIconVisible(Model.IsLock);
		}

		#endregion === Lock Flag ===

		#region === MailIconID ===

		private void HandleMailIconIdChange(object sender, EventArgs e)
		{
			SyncMailIcon();
		}

		private void SyncMailIcon()
		{
			if(!CanUpdate) return;
			
			if(Model.ItemInfo == null) {
				LoadMailIcon(Model.MailIconId);
			}
		}


		/// <summary>
		/// 読み込みアイコン状態をセット
		/// </summary>
		private void LoadMailIcon(int iconId)
		{
			return;

			//if(!CanUpdate) return;

			//this.View.SetMailIconVisible(false);

			//if(Model.MailIconId <= 0) return;

			//// アイコン読み込み
			//CommonIcon.GetIcon(iconId, false,
			//	(atlas, spriteName) =>
			//	{
			//		if(atlas != null && !string.IsNullOrEmpty(spriteName)) {
			//			// アイコンセット
			//			View.SetMailIcon(atlas, spriteName);
			//			this.View.SetMailIconVisible(true);
			//		} else {
			//			this.View.SetMailIconVisible(false);
			//		}
			//	}
			//);
			
		}

		#endregion === MailIconID ===

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
				LoadIcon(Model.ItemInfo.ItemMasterID);
				
				// スタック個数
				View.SetItemCountVisible(true);
				View.SetItemCount(Model.ItemInfo.Stack.ToString());
				
				// 期限ラベル表示
				View.SetDeadlineVisible(true);

				//// 期限
				//string format = MailStr_ItemExpiration;
				//var time = new DateTime(0);

				//if(Model.MailInfo.IsItemReceived) {
				//	// 受取り済み
				//	format = MailStr_ItemReceived;
				//} else if(!Model.MailInfo.HasDeadline) {
				//	// 期限がない
				//	format = MailStr_ItemNoneDeadline;
				//} else if(!Model.MailInfo.IsReceiveExpiration) {
				//	// 期限付きで、期限が切れていない時
				//	var span = Model.MailInfo.ItemDeadlineTime;
					
				//	if(span.TotalSeconds >= 0) {
				//		time = time.AddSeconds(span.TotalSeconds);
				//	}

				//	if(span.TotalDays > 0) {
				//		format = MailStr_ItemDeadlineDaysFormat;
				//	} else if(span.TotalHours > 0) {
				//		format = MailStr_ItemDeadlineHoursFormat;
				//	} else if(span.TotalMinutes > 0) {
				//		format = MailStr_ItemDeadlineMinutesFormat;
				//	} else if(span.TotalSeconds >= 0) {
				//		format = MailStr_ItemDeadlineSecondsFormat;
				//	}
				//}

				//View.SetDeadline(time, format);

			} else {
				// 期限非表示
				View.SetDeadlineVisible(false);

				// アイテムの個数非表示
				View.SetItemCountVisible(false);

				// アイコン仮で非表示
				//View.SetMailIconVisible(false);
				// LoadIcon(Model.MailInfo.ItemInfo.ItemMasterID);
			}
			
		}

		
		/// <summary>
		/// 読み込みアイコン状態をセット
		/// </summary>
		private void LoadIcon(int iconId)
		{
			if(!CanUpdate) return;
			
			this.View.SetMailIconVisible(false);


			// アイコン読み込み
			if(Model.MailInfo.ItemInfo != null) {
				ItemIcon.GetItemIcon(iconId, false,
					(atlas, spriteName) =>
					{
						if(atlas != null && !string.IsNullOrEmpty(spriteName)) {
							// アイコンセット
							View.SetItemIcon(atlas, spriteName);
							this.View.SetMailIconVisible(true);
						} else {
							this.View.SetMailIconVisible(false);
						}
					}
				);
			}
		}

		#endregion === ItemInfo ===

		#region === ItemReceived ===

		private void HandleItemReceivedChange(object sender, EventArgs e)
		{
			SyncItemReceived();
		}

		private void SyncItemReceived()
		{
			if(!CanUpdate) return;

			if(Model.ItemInfo == null) return;

			// 受け取り済み
			if(Model.IsItemReceived) {
				// 期限ラベル表示
				View.SetDeadline(MailStr_ItemReceived);
			}
		}


		#endregion === ItemReceived ===


		#region === ReceiveExpiration ===

		private void HandleReceiveExpirationChange(object sender, EventArgs e)
		{
			SyncReceiveExpiration();
		}

		private void SyncReceiveExpiration()
		{
			if(!CanUpdate) return;

			if(Model.ItemInfo == null) return;

			// 期限切れ
			if(Model.IsReceiveExpiration) {
				View.SetDeadline(MailStr_ItemExpiration);
			}
		}

		#endregion === ReceiveExpiration ===



		#region === Item Deadline ===

		private void HandleItemDeadlineChange(object sender, EventArgs e)
		{
			SyncItemDeadline();
		}

		private void SyncItemDeadline()
		{
			if(!CanUpdate) return;

			if(Model.ItemInfo == null) return;

			// 期限切れ、受け取り済みの時は何もしない
			if(Model.IsReceiveExpiration || Model.IsItemReceived) return;


			// 期限
			string format = MailStr_ItemNoneDeadline;
			var time = new DateTime(0);
			
			if(Model.DeadlineTime != null && Model.MailInfo != null) {
				// 期限付きで、期限が切れていない時
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
			}
			
			View.SetDeadline(time, format);
		}


		#endregion === Item Deadline ===

		/// <summary>
		/// 詳細クリック時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleDetailClick(object sender, EventArgs e)
		{
			OnDetailClick(this, new MailDetailEventArgs(Model.MailInfo));
		}



	}
}

