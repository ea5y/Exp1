using UnityEngine;
using System.Collections;
using System;

namespace XUI.MailItem
{

	public interface IModel
	{
		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();

		/// <summary>
		/// メールインフォ変更イベント
		/// </summary>
		event EventHandler OnMailInfoChange;

		/// <summary>
		/// タイトル変更イベント
		/// </summary>
		event EventHandler OnTitleChange;

		/// <summary>
		/// 受信時間変更イベント
		/// </summary>
		event EventHandler OnReceivedTimeChange;

		/// <summary>
		/// 既読フラグ変更イベント
		/// </summary>
		event EventHandler OnReadFlagChange;
		
		/// <summary>
		/// ロックフラグ変更イベント
		/// </summary>
		event EventHandler OnLockFlagChange;

		/// <summary>
		/// メールアイコンID変更イベント
		/// </summary>
		event EventHandler OnMailIconIdChange;

		/// <summary>
		/// 添付アイテム変更イベント
		/// </summary>
		event EventHandler OnItemInfoChange;

		/// <summary>
		/// アイテム受け取りフラグ変更イベント
		/// </summary>
		event EventHandler OnItemReceivedChange;

		/// <summary>
		/// 期限切れフラグ変更イベント
		/// </summary>
		event EventHandler OnReceiveExpirationChange;

		/// <summary>
		/// アイテム受け取り期限変更イベント
		/// </summary>
		event EventHandler OnItemDeadlineChange;

		/// <summary>
		/// メールインフォ
		/// </summary>
		MailInfo MailInfo { get; set; }

		/// <summary>
		/// メールタイトル
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// 受信時間
		/// </summary>
		DateTime ReceivedTime { get; set; }

		/// <summary>
		/// 既読フラグ
		/// </summary>
		bool IsRead { get; set; }

		/// <summary>
		/// ロックフラグ
		/// </summary>
		bool IsLock { get; set; }

		/// <summary>
		/// メールアイコン
		/// </summary>
		int MailIconId { get; set; }

		/// <summary>
		/// 添付アイテム
		/// </summary>
		ItemInfo ItemInfo { get; set; }

		/// <summary>
		/// アイテム受け取り済みか
		/// </summary>
		bool IsItemReceived { get; set; }

		/// <summary>
		/// アイテム受け取り期限が切れているか
		/// </summary>
		bool IsReceiveExpiration { get; set; }

		/// <summary>
		/// アイテム受け取り期限
		/// </summary>
		DateTime? DeadlineTime { get; set; }

	}

	public class Model : IModel
	{
		#region === Field ===

		private MailInfo mailInfo = null;

		private string title = "";

		private DateTime receivedTime = DateTime.MinValue;

		private bool isRead = false;

		private bool isLock = false;

		private int mailIconId = 0;

		private ItemInfo itemInfo = null;

		private bool isItemReceived = false;

		private bool isReceiveExpiration = false;

		private DateTime? deadlineTime = null;

		#endregion === Field ===

		#region === Event ===

		/// <summary>
		/// メールインフォ変更イベント
		/// </summary>
		public event EventHandler OnMailInfoChange = (sender, e) => { };

		/// <summary>
		/// タイトル変更イベント
		/// </summary>
		public event EventHandler OnTitleChange = (sender, e) => { };

		/// <summary>
		/// 受信時間変更イベント
		/// </summary>
		public event EventHandler OnReceivedTimeChange = (sender, e) => { };

		/// <summary>
		/// 既読フラグ変更イベント
		/// </summary>
		public event EventHandler OnReadFlagChange = (sender, e) => { };

		/// <summary>
		/// ロックフラグ変更イベント
		/// </summary>
		public event EventHandler OnLockFlagChange = (sender, e) => { };

		/// <summary>
		/// メールアイコンID変更イベント
		/// </summary>
		public event EventHandler OnMailIconIdChange = (sender, e) => { };

		/// <summary>
		/// 添付アイテム変更イベント
		/// </summary>
		public event EventHandler OnItemInfoChange = (sender, e) => { };

		/// <summary>
		/// アイテム受け取りフラグ変更イベント
		/// </summary>
		public event EventHandler OnItemReceivedChange = (sender, e) => { };

		/// <summary>
		/// 期限切れフラグ変更イベント
		/// </summary>
		public event EventHandler OnReceiveExpirationChange = (sender, e) => { };

		/// <summary>
		/// アイテム受け取り期限変更イベント
		/// </summary>
		public event EventHandler OnItemDeadlineChange = (sender, e) => { };

		#endregion === Event ===

		#region === Property ===

		/// <summary>
		/// メールインフォ
		/// </summary>
		public MailInfo MailInfo
		{
			get { return mailInfo; }
			set
			{
				if(mailInfo != value) {
					mailInfo = value;

					OnMailInfoChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// メールタイトル
		/// </summary>
		public string Title
		{
			get { return title; }
			set
			{
				if(title != value) {
					title = value;

					OnTitleChange(this, EventArgs.Empty);
				}
			}
		}
		
		/// <summary>
		/// 受信時間
		/// </summary>
		public DateTime ReceivedTime
		{
			get { return receivedTime; }
			set
			{
				if(receivedTime != value) {
					receivedTime = value;

					OnReceivedTimeChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 既読フラグ
		/// </summary>
		public bool IsRead
		{
			get { return isRead; }
			set
			{
				if(isRead != value) {
					isRead = value;

					OnReadFlagChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// ロックフラグ
		/// </summary>
		public bool IsLock
		{
			get { return isLock; }
			set
			{
				if(isLock != value) {
					isLock = value;
					
					OnLockFlagChange(this, EventArgs.Empty);
				}
			}
		}


		/// <summary>
		/// メールアイコン
		/// </summary>
		public int MailIconId
		{
			get { return mailIconId; }
			set
			{
				if(mailIconId != value) {
					mailIconId = value;

					OnMailIconIdChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 添付アイテム
		/// </summary>
		public ItemInfo ItemInfo
		{
			get { return itemInfo; }
			set
			{
				if(itemInfo != value) {
					itemInfo = value;

					OnItemInfoChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// アイテム受け取り済みか
		/// </summary>
		public bool IsItemReceived
		{
			get { return isItemReceived; }
			set
			{
				if(isItemReceived != value) {
					isItemReceived = value;

					OnItemReceivedChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// アイテム受け取り期限が切れているか
		/// </summary>
		public bool IsReceiveExpiration
		{
			get { return isReceiveExpiration; }
			set
			{
				if(isReceiveExpiration != value) {
					isReceiveExpiration = value;

					OnReceiveExpirationChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// アイテム受け取り期限
		/// </summary>
		public DateTime? DeadlineTime
		{
			get { return deadlineTime; }
			set
			{
				if(deadlineTime != value){
					deadlineTime = value;

					OnItemDeadlineChange(this, EventArgs.Empty);
				}
			}
		}

		#endregion === Property ===

		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			mailInfo = null;
			itemInfo = null;
			deadlineTime = null;
			OnMailInfoChange = null;
			OnTitleChange = null;
			OnReceivedTimeChange = null;
			OnReadFlagChange = null;
			OnLockFlagChange = null;
			OnMailIconIdChange = null;
			OnItemInfoChange = null;
			OnItemReceivedChange = null;
			OnReceiveExpirationChange = null;
			OnItemDeadlineChange = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			mailInfo = null;
			mailIconId = 0;
			isRead = false;
			isLock = false;
			isItemReceived = false;
			isReceiveExpiration = false;
			itemInfo = null;
			deadlineTime = null;
		}
	}
}

