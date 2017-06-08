using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace XUI.Mail
{
	/// <summary>
	/// ページ変更イベント引数
	/// </summary>
	public class TabPageChangeEventArgs : EventArgs
	{
		/// <summary>
		/// ページ変更されたタブタイプ
		/// </summary>
		public MailTabType TabType { get; private set; }

		/// <summary>
		/// 変更後のページ
		/// </summary>
		public int Page { get; private set; }

		/// <summary>
		/// 変更後のページの最初のインデックス
		/// </summary>
		public int ItemIndex { get; private set; }

		/// <summary>
		/// 件数
		/// </summary>
		public int ItemCount { get; private set; }
		
		public TabPageChangeEventArgs(MailTabType tabType, int page, int itemIndex, int itemCount)
		{
			TabType = tabType;
			Page = page;
			ItemIndex = itemIndex;
			ItemCount = itemCount;
		}
	}
	
	/// <summary>
	/// 全件削除イベント引数
	/// </summary>
	public class AllMailDeleteEventArgs : EventArgs
	{
		/// <summary>
		/// タブタイプ
		/// </summary>
		public MailTabType TabType { get; private set; }

		public AllMailDeleteEventArgs(MailTabType tabType)
		{
			TabType = tabType;
		}
	}

	public interface IController
	{
		#region === Event ===

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		event EventHandler<TabChangeEventArgs> OnTabChange;

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		event EventHandler<TabPageChangeEventArgs> OnPageChange;

		/// <summary>
		/// まとめて既読イベント
		/// </summary>
		event EventHandler<EventArgs> OnAllMailRead;

		/// <summary>
		/// まとめて受け取りイベント
		/// </summary>
		event EventHandler<EventArgs> OnAllMailItemReceive;

		/// <summary>
		/// まとめて削除イベント
		/// </summary>
		event EventHandler<AllMailDeleteEventArgs> OnAllMailDelete;

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
		/// 指定タブ開く
		/// </summary>
		/// <param name="type"></param>
		void ForceOpenTab(MailTabType type);
		
		/// <summary>
		/// タブを初期化する
		/// </summary>
		void SetupTab();

		/// <summary>
		/// 今のリストを更新する
		/// </summary>
		void UpdateCurrentList();

		/// <summary>
		/// 今のページを再取得する
		/// </summary>
		void ReopenCurrentPage();

		/// <summary>
		/// 運営メール件数セット
		/// </summary>
		void SetMailCount(int total, int unread, int locked);

		/// <summary>
		/// アイテムメール件数セット
		/// </summary>
		void SetItemMailCount(int total, int unread, int locked);

		/// <summary>
		/// 指定範囲の運営メールを更新
		/// </summary>
		void UpdateMailList(List<MailInfo> mails, int start, int count);

		/// <summary>
		/// 指定範囲のアイテムメールを更新
		/// </summary>
		/// <param name="mails"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		void UpdateItemMailList(List<MailInfo> mails, int start, int count);

		/// <summary>
		/// 指定サーバインデックスの運営メールを開く
		/// </summary>
		/// <param name="index"></param>
		void OpenMailDetail(int index);

		/// <summary>
		/// 指定サーバインデックスのアイテムメールを開く
		/// </summary>
		/// <param name="index"></param>
		void OpenItemMailDetail(int index);
		
		/// <summary>
		/// 指定サーバインデックスの運営メールのロックをセットする
		/// </summary>
		/// <param name="index"></param>
		/// <param name="locked"></param>
		void ChangeMailLock(int index, bool locked);

		/// <summary>
		/// 指定サーバインデックスのアイテムメールのロックをセットする
		/// </summary>
		/// <param name="index"></param>
		/// <param name="locked"></param>
		void ChangeItemMailLock(int index, bool locked);

		/// <summary>
		/// 指定サーバインデックスの運営メールを削除する
		/// </summary>
		/// <param name="index"></param>
		void DeleteMail(int index);

		/// <summary>
		/// 指定サーバインデックスのアイテムメールを削除する
		/// </summary>
		/// <param name="index"></param>
		void DeleteItemMail(int index);

		/// <summary>
		/// すべての運営メールを既読にする
		/// </summary>
		void AllMailRead(bool result, int count);

		/// <summary>
		/// すべての運営メールを削除する
		/// </summary>
		void AllMailDelete(bool result, int count);

		/// <summary>
		/// すべてのアイテムメールを受け取る
		/// </summary>
		void AllItemReceive(int count, int expirationCount);

		/// <summary>
		/// すべてのアイテムメールを削除する
		/// </summary>
		void AllItemMailDelete(bool result, int count);
	}

	public class Controller : IController
	{
		#region === 文字列 ===

		private string MailStr_ScreenTitle { get { return MasterData.GetText(TextType.TX370_Mail_ScreenTitle); } }
		
		private string MailStr_HelpMessage { get { return MasterData.GetText(TextType.TX438_Mail_HelpMessage); } }

		private string MailStr_AllReadConfirmMessage { get { return MasterData.GetText(TextType.TX378_Mail_AllRead_ConfirmMessage); } }

		private string MailStr_AllReadMessage { get { return MasterData.GetText(TextType.TX379_Mail_AllRead_Message); } }
		
		private string MailStr_AllReadZeroMessage { get { return MasterData.GetText(TextType.TX439_Mail_AllRead_Zero_Message); } }

		private string MailStr_AllMailDeleteConfirmMessage { get { return MasterData.GetText(TextType.TX380_Mail_AllMailDelete_ConfirmMessage); } }

		private string MailStr_AllMailDeletedMessage { get { return MasterData.GetText(TextType.TX381_Mail_AllMailDelete_Message); } }

		private string MailStr_AllMailDeleteZeroMessage { get { return MasterData.GetText(TextType.TX440_Mail_AllMailDelete_Zero_Message); } }

		private string MailStr_AllReceiveConfirmMessage { get { return MasterData.GetText(TextType.TX382_Mail_AllReceive_ConfirmMessage); } }

		private string MailStr_AllReceivedMessage { get { return MasterData.GetText(TextType.TX383_Mail_AllReceive_Message); } }

		private string MailStr_AllReceivedDeadlineMessage { get { return MasterData.GetText(TextType.TX384_Mail_AllReceive_DeadlineMessage); } }

		private string MailStr_AllReceiveZeroMessage { get { return MasterData.GetText(TextType.TX441_Mail_AllReceive_Zero_Message); } }

		private string MailStr_AllItemMailDeleteConfirmMessage { get { return MasterData.GetText(TextType.TX385_Mail_AllItemMailDelete_ConfirmMessage); } }

		private string MailStr_AllItemMailDeletedMessage { get { return MasterData.GetText(TextType.TX386_Mail_AllItemMailDelete_Message); } }

		private string MailStr_AllItemMailDeleteZeroMessage { get { return MasterData.GetText(TextType.TX440_Mail_AllMailDelete_Zero_Message); } }


		#endregion === 文字列 ===

		#region === Field ===

		// モデル
		private readonly IModel model;
	
		// ビュー
		private readonly IView view;

		// メールリスト
		private GUIMailItemPageList mailItemPageList;

		private bool isUpdateTotalUnread = true;

		#endregion === Field ===

		#region === Property ===

		private IModel Model { get { return model; } }

		private IView View { get { return view; } }
		
		private GUIMailItemPageList MailItemPageList { get { return mailItemPageList; } }

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		private bool CanUpdate
		{
			get
			{
				if(Model == null) return false;
				if(View == null) return false;
				if(MailItemPageList == null) return false;
				return true;
			}
		}

		#endregion === Property ===


		#region === Event ===

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		public event EventHandler<TabChangeEventArgs> OnTabChange = (sender, e) => { };

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		public event EventHandler<TabPageChangeEventArgs> OnPageChange = (sender, e) => { };

		/// <summary>
		/// まとめて既読イベント
		/// </summary>
		public event EventHandler<EventArgs> OnAllMailRead = (sender, e) => { };

		/// <summary>
		/// まとめて受け取りイベント
		/// </summary>
		public event EventHandler<EventArgs> OnAllMailItemReceive = (sender, e) => { };

		/// <summary>
		/// まとめて削除イベント
		/// </summary>
		public event EventHandler<AllMailDeleteEventArgs> OnAllMailDelete = (sender, e) => { };
		
		#endregion === Event ===




		public Controller(IModel model, IView view, GUIMailItemPageList mailItemPageList)
		{
			UnityEngine.Assertions.Assert.IsNotNull(model, "MailController Constructor: Model is Null");
			UnityEngine.Assertions.Assert.IsNotNull(view, "MailController Constructor: View is Null");
			UnityEngine.Assertions.Assert.IsNotNull(mailItemPageList, "MailController Constructor: GUIMailItemPageList is Null");
			if (model == null || view == null || mailItemPageList == null) return;


			// ビュー設定
			this.view = view;
			View.OnHome += HandleHome;
			View.OnClose += HandleClose;
			View.OnTabChange += HandleTabChange;
			View.OnAllDeleteClick += HandleAllDeleteClick;
			View.OnAllItemReceiveClick += HandleAllItemReceiveClick;
			View.OnAllReadClick += HandleAllReadClick;
			
			
			// モデル設定
			this.model = model;
			Model.OnCurrentTabChange += HandleCurrentTabChange;
			Model.OnCurrentMaxCountChange += HandleCurrentTotalCountChange;

			Model.OnUnreadNumForamtChange += HandleUnreadNumForamtChange;
			Model.OnTotalMailCountChange += HandleTotalMailCountChange;
			Model.OnTotalItemMailCountChange += HandleTotalItemMailCountChange;
			Model.OnUnreadMailCountChange += HandleUnreadMailCountChange;
			Model.OnUnreadItemMailCountChange += HandleUnreadItemMailCountChange;

			Model.OnTotalUnreadCountChange += HandleTotalUnreadCountChange;

			// メールリスト
			this.mailItemPageList = mailItemPageList;
			MailItemPageList.OnPageChange += HandlePageChange;

			// Viewに反映
			SyncInit();
		}


		/// <summary>
		/// View初期化用
		/// </summary>
		private void SyncInit()
		{
			SyncUnreadItemMailCount();
			SyncUnreadMailCount();
		}


		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			if(!CanUpdate) return;

			// キャッシュクリア
			Model.Setup();

			// ページリスト
			MailItemPageList.Setup();
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

			OnPageChange = null;
			OnAllMailRead = null;
			OnAllMailItemReceive = null;
			OnAllMailDelete = null;
		}

		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if(!CanUpdate) return;
			
			this.View.SetActive(isActive, isTweenSkip);

			// その他UIの表示設定
			GUILobbyResident.SetActive(!isActive);
			GUIScreenTitle.Play(isActive, MailStr_ScreenTitle);
			GUIHelpMessage.Play(isActive, MailStr_HelpMessage);
		}

		#region ホーム、閉じるボタンイベント
		
		private void HandleHome(object sender, EventArgs e)
		{
			GUIController.Clear();
		}
		
		private void HandleClose(object sender, EventArgs e)
		{
			GUIController.Back();
		}

		#endregion



		#region === Mail Count ===

		/// <summary>
		/// 運営メール件数セット
		/// </summary>
		/// <param name="total"></param>
		/// <param name="unread"></param>
		/// <param name="locked"></param>
		public void SetMailCount(int total, int unread, int locked)
		{
			if(!CanUpdate) return;

			//isUpdateTotalUnread = false;
			Model.TotalMailCount = total;
			Model.UnreadMailCount = unread;
			Model.LockMailCount = locked;

			//isUpdateTotalUnread = true;
		}

		/// <summary>
		/// アイテムメール件数セット
		/// </summary>
		/// <param name="total"></param>
		/// <param name="unread"></param>
		/// <param name="locked"></param>
		public void SetItemMailCount(int total, int unread, int locked)
		{
			if(!CanUpdate) return;
			
			//isUpdateTotalUnread = false;

			Model.TotalItemMailCount = total;
			Model.UnreadItemMailCount = unread;
			Model.LockItemMailCount = locked;

			//isUpdateTotalUnread = true;
		}
		
		/// <summary>
		/// 未読数値フォーマットが変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleUnreadNumForamtChange(object sender, EventArgs e)
		{
			// 数値更新
			SyncUnreadMailCount();
			SyncUnreadItemMailCount();
		}
		
		/// <summary>
		/// 未読メール数が変わった
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleUnreadMailCountChange(object sender, EventArgs e)
		{
			SyncUnreadMailCount();
		}

		/// <summary>
		/// 未読アイテムメール数が変わった
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleUnreadItemMailCountChange(object sender, EventArgs e)
		{
			SyncUnreadItemMailCount();
		}

		/// <summary>
		/// 未読メールの件数を反映する
		/// </summary>
		private void SyncUnreadMailCount()
		{
			if(!CanUpdate) return;

			View.SetUnreadMailCount(Model.UnreadMailCount, Model.UnreadNumFormat);
		}

		/// <summary>
		/// 未読アイテムメールの件数を反映する
		/// </summary>
		private void SyncUnreadItemMailCount()
		{
			if(!CanUpdate) return;

			View.SetUnreadItemMailCount(Model.UnreadItemMailCount, Model.UnreadNumFormat);
		}
		
		/// <summary>
		/// 運営メール件数変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleTotalMailCountChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;
			
			// アイテム数更新
			if(Model.CurrentTab == MailTabType.Mail) {
				SetCurrentTotalCount(Model.TotalMailCount);
			}
		}

		/// <summary>
		/// アイテムメール件数変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleTotalItemMailCountChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			// アイテム数更新
			if(Model.CurrentTab == MailTabType.Item) {
				SetCurrentTotalCount(Model.TotalItemMailCount);
			}
		}

		/// <summary>
		/// 現在の件数を変更する
		/// </summary>
		/// <param name="count"></param>
		private void SetCurrentTotalCount(int count)
		{
			if(!CanUpdate) return;

			Model.CurrentTotalCount = count;
		}
		
		/// <summary>
		/// 現在のタブのメール件数変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleCurrentTotalCountChange(object sender, EventArgs e)
		{
			// 最大数が変わった
			SyncCurrentNum();
		}

		/// <summary>
		/// タブの件数を反映する
		/// </summary>
		private void SyncCurrentNum()
		{
			if(!CanUpdate) return;
			
			// 最大数が変わった
			MailItemPageList.SetItemCount(Model.CurrentTotalCount);

			// ボタンの有効を変更
			View.SetButtonEnable(Model.CurrentTotalCount > 0);
		}


		/// <summary>
		/// 合計未読数が変更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleTotalUnreadCountChange(object sender, EventArgs e)
		{
			// 反映させない
			if(!isUpdateTotalUnread) return;
			
			// 常駐メニューの未読メール件数設定
			GUILobbyResident.SetMailUnread(Model.TotalUnreadCount);
		}


		#endregion === Mail Count ===

		#region === Tab ===

		/// <summary>
		/// タブボタンを切り替え時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleTabChange(object sender, TabChangeEventArgs e)
		{
			if(!CanUpdate) return;

			Model.CurrentTab = e.TabType;
		}

		/// <summary>
		/// 指定タブを開く
		/// </summary>
		/// <param name="type"></param>
		public void ForceOpenTab(MailTabType type)
		{
			if(!CanUpdate) return;

			Model.CurrentTab = MailTabType.None;
			Model.CurrentTab = type;
		}

		/// <summary>
		/// 選択タブが変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleCurrentTabChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			// ない場合は何もしない
			if(Model.CurrentTab == MailTabType.None) return;

			// イベントに変更
			OnTabChange(this, new TabChangeEventArgs(Model.CurrentTab));

		}

		public void SetupTab()
		{
			// 最大値変更
			switch(Model.CurrentTab) {
				case MailTabType.Mail:
					SetCurrentTotalCount(Model.TotalMailCount);
					break;
				case MailTabType.Item:
					SetCurrentTotalCount(Model.TotalItemMailCount);
					break;
			}

			View.SetTabMode(Model.CurrentTab);

			View.SetButtonEnable(Model.CurrentTotalCount > 0);

			MailItemPageList.Clear();

			// ページ変更する
			MailItemPageList.SetPage(0);
		}

		#endregion === Tab ===
		
		#region === Page Change ===

		/// <summary>
		/// ページ変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandlePageChange(object sender, MailItemPageList.PageChangeEventArgs e)
		{
			PageChangeWithCacheCheck(
				Model.CurrentTab,
				e.Page,
				e.ItemIndex,
				e.ItemCount
			);
		}

		/// <summary>
		/// ページ変更
		/// </summary>
		/// <param name="type"></param>
		/// <param name="page"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		private void PageChangeWithCacheCheck(MailTabType type, int page, int start, int count)
		{
			if(!CanUpdate) return;
			
			// キャッシュチェック
			List<MailInfo> mails = null;
			switch(type) {
				case MailTabType.Mail:
					mails = Model.GetMailInfo(start, count);
					break;
				case MailTabType.Item:
					mails = Model.GetItemMailInfo(start, count);
					break;
			}

			if(mails != null && mails.Count == count) {
				mailItemPageList.SetViewMailList(mails);
			} else {

				// 無ければイベント通知して取得する
				TabPageChangeEventArgs args = new TabPageChangeEventArgs(
					type,
					page,
					start,
					count
				);

				OnPageChange(this, args);
			}
		}


		#endregion === Page Change ===

		/// <summary>
		/// 指定範囲の運営メールを更新
		/// </summary>
		/// <param name="mails"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		public void UpdateMailList(List<MailInfo> mails, int start, int count)
		{
			if(!CanUpdate) return;

			// メールリストセット
			Model.SetMailInfo(mails);

			// アイテム更新イベントだととりづらい
			MailItemPageList.SetViewMailList(Model.GetMailInfo(start, count));
		}

		/// <summary>
		/// 指定範囲のアイテムメールを更新
		/// </summary>
		/// <param name="mails"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		public void UpdateItemMailList(List<MailInfo> mails, int start, int count)
		{
			if(!CanUpdate) return;

			// メールリストセット
			Model.SetItemMailInfo(mails);
			// 
			MailItemPageList.SetViewMailList(Model.GetItemMailInfo(start, count));
		}

		/// <summary>
		/// 今のリストを更新する
		/// </summary>
		public void UpdateCurrentList()
		{
			MailItemPageList.UpdateCurrentList();
		}


		/// <summary>
		/// 今のページを再取得する
		/// </summary>
		public void ReopenCurrentPage()
		{
			MailItemPageList.ReopenPage();
		}


		#region === Detail ===

		/// <summary>
		/// 指定サーバインデックスの運営メールを開く
		/// </summary>
		/// <param name="index"></param>
		public void OpenMailDetail(int index)
		{
			if(!CanUpdate) return;
			
			MailInfo mail;
			if(Model.TryGetMailInfo(index, out mail)) {
				if(!mail.IsRead) {
					// 既読にする
					mail.Read();

					// 未読数減らす
					Model.UnreadMailCount--;
				}

				GUIMailDetail.Open(mail, Model.LockMailCount);
				
				MailItemPageList.UpdateCurrentList();
			}
		}

		/// <summary>
		/// 指定サーバインデックスのアイテムメールを開く
		/// </summary>
		/// <param name="index"></param>
		public void OpenItemMailDetail(int index)
		{
			if(!CanUpdate) return;

			MailInfo mail;
			if(Model.TryGetItemMailInfo(index, out mail)) {
				if(!mail.IsRead) {
					// 既読にする
					mail.Read();

					// 未読数減らす
					Model.UnreadItemMailCount--;
				}

				GUIMailDetail.Open(mail, Model.LockItemMailCount);
				
				MailItemPageList.UpdateCurrentList();
			}
		}

		#endregion === Detail ===

		#region === Lock ===
		
		/// <summary>
		/// 指定サーバインデックスの運営メールのロックをセットする
		/// </summary>
		/// <param name="index"></param>
		/// <param name="locked"></param>
		public void ChangeMailLock(int index, bool locked)
		{
			if(!CanUpdate) return;

			MailInfo mail;
			if(Model.TryGetMailInfo(index, out mail)) {
				// 既読にする
				mail.IsLocked = locked;

				// ロック数変更
				Model.LockMailCount += (locked ? 1 : -1);

				MailItemPageList.UpdateCurrentList();
			}
		}

		/// <summary>
		/// 指定サーバインデックスのアイテムメールのロックをセットする
		/// </summary>
		/// <param name="index"></param>
		/// <param name="locked"></param>
		public void ChangeItemMailLock(int index, bool locked)
		{
			if(!CanUpdate) return;

			MailInfo mail;
			if(Model.TryGetItemMailInfo(index, out mail)) {
				// 既読にする
				mail.IsLocked = locked;

				// ロック数変更
				Model.LockItemMailCount += (locked ? 1 : -1);

				MailItemPageList.UpdateCurrentList();
			}
		}

		#endregion === Lock ===

		#region === All Read ===

		/// <summary>
		/// すべて既読ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleAllReadClick(object sender, EventArgs e)
		{
			// 未読がなければ何もしない
			//if(Model.UnreadMailCount == 0) return;

			GUIMessageWindow.SetModeYesNo(
				MailStr_AllReadConfirmMessage,
				() => OnAllMailRead(this, EventArgs.Empty),
				null
			);
		}

		/// <summary>
		/// すべての運営メールを既読にする
		/// </summary>
		public void AllMailRead(bool result, int count)
		{
			if(!CanUpdate) return;

			if(result) {
				if(count == 0) {
					GUIMessageWindow.SetModeOK(MailStr_AllReadZeroMessage, null);
				} else {
					GUIMessageWindow.SetModeOK(
						string.Format(MailStr_AllReadMessage, count),
						() =>
						{
							Model.AllMailRead();
							// UpdateCurrentList();
							GUIMail.ReOpen();
						}
					);
				}
			} else {

			}
		}

		#endregion === All Read ===

		#region === All ItemReceive ===

		/// <summary>
		/// すべて受け取るボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleAllItemReceiveClick(object sender, EventArgs e)
		{
			GUIMessageWindow.SetModeYesNo(
				MailStr_AllReceiveConfirmMessage,
				() => OnAllMailItemReceive(this, EventArgs.Empty),
				null
			);
		}
		
		/// <summary>
		/// すべてのアイテムメールを受け取る
		/// </summary>
		public void AllItemReceive(int count, int expirationCount)
		{
			if(!CanUpdate) return;

			if(count > 0) {
				// エラー件数がある場合は多重で表示
				GUIMessageWindow.SetModeOK(
					string.Format(MailStr_AllReceivedMessage, count),
					() =>
					{
						if(expirationCount > 0) {
							GUIMessageWindow.SetModeOK(
								string.Format(MailStr_AllReceivedDeadlineMessage, expirationCount),
								() =>
								{
									Model.AllItemReceive();
									//UpdateCurrentList();
									GUIMail.ReOpen();
								}
							);
						} else {
							Model.AllItemReceive();
							//UpdateCurrentList();
							GUIMail.ReOpen();
						}
					}
				);
			} else if(expirationCount > 0) {
				GUIMessageWindow.SetModeOK(
					string.Format(MailStr_AllReceivedDeadlineMessage, expirationCount),
					() =>
					{
						Model.AllItemReceive();
						//UpdateCurrentList();
						GUIMail.ReOpen();
					}
				);
			} else {
				GUIMessageWindow.SetModeOK(MailStr_AllReceiveZeroMessage, 
					()=> {
						GUIMail.ReOpen();
					}
				);
			}
		}


		#endregion === All ItemReceive ===

		#region === All Delete ===

		/// <summary>
		/// 削除クリック時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleAllDeleteClick(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			string text = MailStr_AllMailDeleteConfirmMessage;
			if(Model.CurrentTab == MailTabType.Item) {
				text = MailStr_AllItemMailDeleteConfirmMessage;
			}

			GUIMessageWindow.SetModeYesNo(
				text,
				() => OnAllMailDelete(this, new AllMailDeleteEventArgs(Model.CurrentTab)),
				null
			);
			
		}

		/// <summary>
		/// すべての運営メールを削除する
		/// </summary>
		public void AllMailDelete(bool result, int count)
		{
			if(!CanUpdate) return;

			if(result) {
				if(count == 0) {
					GUIMessageWindow.SetModeOK(MailStr_AllMailDeleteZeroMessage,
						() => {
							GUIMail.ReOpen();
						}
					);
				} else {
					GUIMessageWindow.SetModeOK(
						string.Format(MailStr_AllMailDeletedMessage, count),
						() =>
						{
							Model.AllMailDelete();
							GUIMail.ReOpen();
						}
					);
				}
			} else {

			}
		}

		/// <summary>
		/// すべてのアイテムメールを削除する
		/// </summary>
		public void AllItemMailDelete(bool result, int count)
		{
			if(!CanUpdate) return;

			if(result) {
				if(count == 0) {
					GUIMessageWindow.SetModeOK(MailStr_AllItemMailDeleteZeroMessage,
						() => {
							GUIMail.ReOpen();
						}
					);
				} else {
					GUIMessageWindow.SetModeOK(
						string.Format(MailStr_AllItemMailDeletedMessage, count),
						() =>
						{
							Model.AllItemMailDelete();
							GUIMail.ReOpen();
						}
					);
				}
			} else {

			}
		}
		
		#endregion === All Delete ===

		#region === Delete ===
		
		/// <summary>
		/// 指定サーバインデックスの運営メールを削除する
		/// </summary>
		/// <param name="index"></param>
		public void DeleteMail(int index)
		{
			if(!CanUpdate) return;

			Model.RemoveMailInfo(index);

			// リスト更新
			//MailItemPageList.ReopenPage();
		}

		/// <summary>
		/// 指定サーバインデックスのアイテムメールを削除する
		/// </summary>
		/// <param name="index"></param>
		public void DeleteItemMail(int index)
		{
			if(!CanUpdate) return;

			Model.RemoveItemMailInfo(index);

			// リスト更新
			//MailItemPageList.ReopenPage();
		}


		#endregion === Delete ===
	}
}

