using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XUI.Mail
{
	public enum MailTabType
	{
		None = -1,
		Mail = 0,
		Item = 1,
	}

	public interface IModel
	{
		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		void Setup();

		#region === Event ===
		
		/// <summary>
		/// 未読フォーマット変更イベント
		/// </summary>
		event EventHandler OnUnreadNumForamtChange;

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		event EventHandler OnCurrentTabChange;

		/// <summary>
		/// 合計未読件数変更イベント
		/// </summary>
		event EventHandler OnTotalUnreadCountChange;

		/// <summary>
		/// 現在の最大件数変更イベント
		/// </summary>
		event EventHandler OnCurrentMaxCountChange;

		/// <summary>
		/// 運営メール最大件数変更イベント
		/// </summary>
		event EventHandler OnTotalMailCountChange;

		/// <summary>
		/// 運営メール未読件数変更イベント
		/// </summary>
		event EventHandler OnUnreadMailCountChange;

		/// <summary>
		/// アイテムメール最大件数変更イベント
		/// </summary>
		event EventHandler OnTotalItemMailCountChange;

		/// <summary>
		/// アイテムメール未読件数変更イベント
		/// </summary>
		event EventHandler OnUnreadItemMailCountChange;

		#endregion === Event ===

		#region === Property ===
		
		/// <summary>
		/// 未読数値フォーマット
		/// </summary>
		string UnreadNumFormat { get; set; }

		/// <summary>
		/// 現在のタブタイプ
		/// </summary>
		MailTabType CurrentTab { get; set; }

		/// <summary>
		/// 現在の最大メール件数
		/// </summary>
		int CurrentTotalCount { get; set; }

		/// <summary>
		/// 合計未読件数を取得
		/// </summary>
		int TotalUnreadCount { get; }

		/// <summary>
		/// メール件数
		/// </summary>
		int TotalMailCount { get; set; }
		
		/// <summary>
		/// 未読メール件数
		/// </summary>
		int UnreadMailCount { get; set; }

		/// <summary>
		/// ロックメール件数
		/// </summary>
		int LockMailCount { get; set; }
		
		/// <summary>
		/// アイテムメール件数
		/// </summary>
		int TotalItemMailCount { get; set; }

		/// <summary>
		/// 未読アイテムメール件数
		/// </summary>
		int UnreadItemMailCount { get; set; }

		/// <summary>
		/// ロックアイテムメール件数
		/// </summary>
		int LockItemMailCount { get; set; }

		#endregion === Property ===


		/// <summary>
		/// メール更新
		/// </summary>
		/// <param name="mails"></param>
		void SetMailInfo(IList<MailInfo> mails);
		
		/// <summary>
		/// サーバインデックスでメール削除
		/// </summary>
		/// <param name="index"></param>
		void RemoveMailInfo(int index);

		/// <summary>
		/// サーバインデックスのメールを取得する
		/// </summary>
		/// <param name="index"></param>
		bool TryGetMailInfo(int index, out MailInfo mail);

		/// <summary>
		/// 指定範囲のメール取得
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		List<MailInfo> GetMailInfo(int start, int count);

		/// <summary>
		/// すべての運営メールを既読にする
		/// </summary>
		void AllMailRead();

		/// <summary>
		/// すべての運営メールを削除する
		/// </summary>
		void AllMailDelete();

		/// <summary>
		/// アイテムメール更新
		/// </summary>
		/// <param name="mails"></param>
		void SetItemMailInfo(IList<MailInfo> mails);

		/// <summary>
		/// サーバインデックスでアイテムメール削除
		/// </summary>
		/// <param name="index"></param>
		void RemoveItemMailInfo(int index);

		/// <summary>
		/// サーバインデックスのアイテムメールを取得する
		/// </summary>
		/// <param name="index"></param>
		bool TryGetItemMailInfo(int index, out MailInfo mail);

		/// <summary>
		/// 指定範囲のアイテムメール取得
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		List<MailInfo> GetItemMailInfo(int start, int count);

		/// <summary>
		/// すべてのアイテムを受け取る
		/// </summary>
		void AllItemReceive();

		/// <summary>
		/// すべてのアイテムメールを削除する
		/// </summary>
		void AllItemMailDelete();

	}

	public class Model : IModel
	{
		// 未読の最大値
		private const int MaxUnreadMailCount = 99;

		private const int MaxMailCount = 500;

		private const int MaxItemMailCount = 100;


		#region === Field ===

		private string unreadNumFormat = "";
		
		private MailTabType currentTab = MailTabType.None;

		private int currentTotalCount = -1;

		private int totalUnreadCount = -1;

		private int totalMailCount = -1;

		private int unreadMailCount = -1;

		private int totalItemMailCount = -1;

		private int unreadItemMailCount = -1;


		// メールリスト
		private List<MailInfo> mailList = new List<MailInfo>(MaxMailCount / 4);

		// アイテムメールリスト
		private List<MailInfo> itemMailList = new List<MailInfo>(MaxItemMailCount);
		
		#endregion === Field ===


		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			OnUnreadNumForamtChange = null;
			OnCurrentTabChange = null;
			OnCurrentMaxCountChange = null;
			OnTotalUnreadCountChange = null;
			OnTotalMailCountChange = null;
			OnUnreadMailCountChange = null;
			OnTotalItemMailCountChange = null;
			OnUnreadItemMailCountChange = null;
		}


		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			currentTab = MailTabType.None;
			currentTotalCount = -1;
			totalUnreadCount = -1;
			totalMailCount = -1;
			unreadMailCount = -1;
			totalItemMailCount = -1;
			unreadItemMailCount = -1;

			mailList.Clear();
			itemMailList.Clear();
		}

		#region === Event ===

		/// <summary>
		/// 未読フォーマット変更イベント
		/// </summary>
		public event EventHandler OnUnreadNumForamtChange = (sender, e) => { };

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		public event EventHandler OnCurrentTabChange = (sender, e) => { };

		/// <summary>
		/// 合計未読件数変更イベント
		/// </summary>
		public event EventHandler OnTotalUnreadCountChange = (sender, e) => { };

		/// <summary>
		/// 現在の最大件数変更イベント
		/// </summary>
		public event EventHandler OnCurrentMaxCountChange = (sender, e) => { };

		/// <summary>
		/// 運営メール最大件数変更イベント
		/// </summary>
		public event EventHandler OnTotalMailCountChange = (sender, e) => { };

		/// <summary>
		/// 運営メール未読件数変更イベント
		/// </summary>
		public event EventHandler OnUnreadMailCountChange = (sender, e) => { };

		/// <summary>
		/// アイテムメール最大件数変更イベント
		/// </summary>
		public event EventHandler OnTotalItemMailCountChange = (sender, e) => { };

		/// <summary>
		/// アイテムメール未読件数変更イベント
		/// </summary>
		public event EventHandler OnUnreadItemMailCountChange = (sender, e) => { };

		#endregion === Event ===

		#region === Property ===

		/// <summary>
		/// 未読数値フォーマット
		/// </summary>
		public string UnreadNumFormat
		{
			get { return unreadNumFormat; }
			set
			{
				if(unreadNumFormat != value) {
					unreadNumFormat = value;

					OnUnreadNumForamtChange(this, EventArgs.Empty);
				}
			}
		}


		/// <summary>
		/// 現在のタブタイプ
		/// </summary>
		public MailTabType CurrentTab
		{
			get { return currentTab; }
			set
			{
				if(currentTab != value) {
					currentTab = value;
					// 件数を変更しておく
					currentTotalCount = -1;

					OnCurrentTabChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// メール件数
		/// </summary>
		public int CurrentTotalCount
		{
			get { return currentTotalCount; }
			set
			{
				if(currentTotalCount != value) {
					currentTotalCount = value;
					
					this.OnCurrentMaxCountChange(this, EventArgs.Empty);
				}
			}
		}


		/// <summary>
		/// 合計未読件数を取得
		/// </summary>
		public int TotalUnreadCount
		{
			get { return totalUnreadCount; }
			set
			{
				if(totalUnreadCount != value) {
					totalUnreadCount = value;

					this.OnTotalUnreadCountChange(this, EventArgs.Empty);
				}
			}
		}


		/// <summary>
		/// メール件数
		/// </summary>
		public int TotalMailCount
		{
			get { return totalMailCount; }
			set
			{
				if(totalMailCount != value) {
					totalMailCount = value;

					// キャッシュ削除
					mailList.Clear();

					this.OnTotalMailCountChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 未読メール件数
		/// </summary>
		public int UnreadMailCount
		{
			get { return unreadMailCount; }
			set
			{
				if(unreadMailCount != value) {
					unreadMailCount = value;
					
					this.OnUnreadMailCountChange(this, EventArgs.Empty);

					UpdateTotalUnreadCount();
				}
			}
		}

		/// <summary>
		/// ロックメール件数
		/// </summary>
		public int LockMailCount { get; set; }


		/// <summary>
		/// アイテムメール件数
		/// </summary>
		public int TotalItemMailCount
		{
			get { return totalItemMailCount; }
			set
			{
				if(totalItemMailCount != value) {
					totalItemMailCount = value;

					// キャッシュ削除
					itemMailList.Clear();

					this.OnTotalItemMailCountChange(this, EventArgs.Empty);
				}
			}
		}


		/// <summary>
		/// 未読アイテムメール件数
		/// </summary>
		public int UnreadItemMailCount
		{
			get { return unreadItemMailCount; }
			set
			{
				if(unreadItemMailCount != value) {
					unreadItemMailCount = value;

					this.OnUnreadItemMailCountChange(this, EventArgs.Empty);

					UpdateTotalUnreadCount();
				}
			}
		}

		/// <summary>
		/// ロックアイテムメール件数
		/// </summary>
		public int LockItemMailCount { get; set; }

		#endregion === Property ===

		/// <summary>
		/// 未読件数更新
		/// </summary>
		private void UpdateTotalUnreadCount()
		{
			TotalUnreadCount = UnreadMailCount + UnreadItemMailCount;
		}


		#region === Mail ===


		/// <summary>
		/// すべての運営メールを既読にする
		/// </summary>
		public void AllMailRead()
		{
			mailList.ForEach(x => x.Read());

			UnreadMailCount = 0;
		}

		/// <summary>
		/// メールを更新する
		/// </summary>
		/// <param name="mails"></param>
		public void SetMailInfo(IList<MailInfo> mails)
		{
			// 同一ローカルインデックスを削除する
			var indices = mails.Select(x => x.LocalIndex).ToList();
			mailList.RemoveAll(x => indices.Contains(x.LocalIndex));

			// 同一鯖インデックスを削除する
			indices = mails.Select(x => x.Index).ToList();
			mailList.RemoveAll(x => indices.Contains(x.Index));
			
			// 追加
			mailList.AddRange(mails);
		}

		

		/// <summary>
		/// 鯖インデックスから削除
		/// </summary>
		/// <param name="index"></param>
		public void RemoveMailInfo(int index)
		{
			var info = mailList.FirstOrDefault(x => x.Index == index);
			if(info == null) return;
			
			int localIndex = info.LocalIndex;
			mailList.Remove(info);

			// ローカルインデックス減らす。
			for(int i = 0; i < mailList.Count; i++) {
				if(mailList[i].LocalIndex >= localIndex) {
					mailList[i].LocalIndex--;
				}
			}

			//TotalMailCount--;
		}

		/// <summary>
		/// サーバインデックスのメールを取得する
		/// </summary>
		/// <param name="index"></param>
		public bool TryGetMailInfo(int index, out MailInfo mail)
		{
			mail = mailList.FirstOrDefault(x => x.Index == index);
			return mail != null;
		}

		/// <summary>
		/// 指定範囲のメールインフォ取り出す
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public List<MailInfo> GetMailInfo(int start, int count)
		{
			List<MailInfo> mails = new List<MailInfo>();

			// 指定範囲取り出す
			int end = start + count;
			mails.AddRange(mailList.Where(x => x.LocalIndex >= start && x.LocalIndex < end));

			// 新しい順にソート
			mails.Sort((x, y) => y.Index - x.Index);

			return mails;
		}

		/// <summary>
		/// すべての運営メールを削除する
		/// </summary>
		public void AllMailDelete()
		{
			mailList.Clear();
		}


		#endregion === Mail ===


		#region === ItemMail ===

		/// <summary>
		/// アイテムメールを更新する
		/// </summary>
		/// <param name="mails"></param>
		public void SetItemMailInfo(IList<MailInfo> mails)
		{
			// 同一ローカルインデックスを削除する
			var indices = mails.Select(x => x.LocalIndex).ToList();
			itemMailList.RemoveAll(x => indices.Contains(x.LocalIndex));
			
			// 同一鯖インデックスを削除する
			indices = mails.Select(x => x.Index).ToList();
			itemMailList.RemoveAll(x => indices.Contains(x.Index));

			// 追加
			itemMailList.AddRange(mails);
		}

		/// <summary>
		/// 鯖インデックスから削除
		/// </summary>
		/// <param name="index"></param>
		public void RemoveItemMailInfo(int index)
		{
			var info = itemMailList.FirstOrDefault(x => x.Index == index);
			if(info == null) return;
			
			int localIndex = info.LocalIndex;
			itemMailList.Remove(info);

			// ローカルインデックス減らす。
			for(int i = 0; i < itemMailList.Count; i++) {
				if(itemMailList[i].LocalIndex >= localIndex) {
					itemMailList[i].LocalIndex--;
				}
			}
			
			//TotalItemMailCount--;
		}

		/// <summary>
		/// サーバインデックスのメールを取得する
		/// </summary>
		/// <param name="index"></param>
		public bool TryGetItemMailInfo(int index, out MailInfo mail)
		{
			mail = itemMailList.FirstOrDefault(x => x.Index == index);
			return mail != null;
		}


		/// <summary>
		/// 指定範囲のメールインフォ取り出す
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public List<MailInfo> GetItemMailInfo(int start, int count)
		{
			List<MailInfo> mails = new List<MailInfo>();

			// 指定範囲取り出す
			int end = start + count;
			mails.AddRange(itemMailList.Where(x => x.LocalIndex >= start && x.LocalIndex < end));

			// 新しい順にソート
			mails.Sort((x, y) => y.Index - x.Index);

			return mails;
		}

		/// <summary>
		/// すべてアイテムを受け取る
		/// </summary>
		public void AllItemReceive()
		{
			itemMailList.ForEach(x => { x.Read(); x.ItemReceive(); });

			UnreadItemMailCount = 0;
		}

		/// <summary>
		/// すべてのアイテムメールを削除する
		/// </summary>
		public void AllItemMailDelete()
		{
			itemMailList.Clear();
		}


		#endregion === ItemMail ===
	}
}

