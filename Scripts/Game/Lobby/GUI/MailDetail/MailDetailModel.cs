using UnityEngine;
using System.Collections;
using System;

namespace XUI.MailDetail
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

		#region === Event ===

		/// <summary>
		/// メールインフォ更新イベント
		/// </summary>
		event EventHandler OnMailInfoChange;

		/// <summary>
		/// タイトル変更イベント
		/// </summary>
		event EventHandler OnTitleChange;

		/// <summary>
		/// 本文変更イベント
		/// </summary>
		event EventHandler OnBodyChange;

		/// <summary>
		/// 受信時間変更イベント
		/// </summary>
		event EventHandler OnReceivedTimeChange;
		/// <summary>
		/// ロックフラグ変更イベント
		/// </summary>
		event EventHandler OnLockFlagChange;

		/// <summary>
		/// 添付アイテム変更イベント
		/// </summary>
		event EventHandler OnItemInfoChange;


		#endregion === Event ===

		#region === Property ===

		/// <summary>
		/// アイテム個数フォーマット
		/// </summary>
		string ItemCountFormat { get; set; }
		

		/// <summary>
		/// ロック件数
		/// </summary>
		int LockCount { get; set; }
		
		/// <summary>
		/// 保持期限
		/// </summary>
		int KeepDays{ get; }

		/// <summary>
		/// 最大ロック件数
		/// </summary>
		int MaxLockCount { get; }

		/// <summary>
		/// ロック件数が最大か
		/// </summary>
		bool OverLockCount { get; }

		/// <summary>
		/// 保持期限を過ぎているか
		/// </summary>
		bool OverKeepDays { get; }



		/// <summary>
		/// メールインフォ
		/// </summary>
		MailInfo MailInfo { get; set; }

		/// <summary>
		/// メールタイトル
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// 本文
		/// </summary>
		string Body { get; set; }

		/// <summary>
		/// 受信時間
		/// </summary>
		DateTime ReceivedTime { get; set; }
		
		/// <summary>
		/// ロックフラグ
		/// </summary>
		bool IsLock { get; set; }

		/// <summary>
		/// 添付アイテム
		/// </summary>
		ItemInfo ItemInfo { get; set; }



		#endregion === Property ===


	}

	public class Model : IModel
	{
		#region === Field ===

		private MailInfo mailInfo = null;

		private string itemCountFormat = "{0}";
		
		private string title = "";

		private string body = "";

		private DateTime receivedTime = DateTime.MinValue;
		
		private bool isLock = false;

		private ItemInfo itemInfo = null;

		#endregion === Field ===


		#region === Event ===

		/// <summary>
		/// メールインフォ更新イベント
		/// </summary>
		public event EventHandler OnMailInfoChange = (sender, e) => { };

		/// <summary>
		/// タイトル変更イベント
		/// </summary>
		public event EventHandler OnTitleChange = (sender, e) => { };
		
		/// <summary>
		/// 本文変更イベント
		/// </summary>
		public event EventHandler OnBodyChange = (sender, e) => { };

		/// <summary>
		/// 受信時間変更イベント
		/// </summary>
		public event EventHandler OnReceivedTimeChange = (sender, e) => { };

		/// <summary>
		/// ロックフラグ変更イベント
		/// </summary>
		public event EventHandler OnLockFlagChange = (sender, e) => { };

		/// <summary>
		/// 添付アイテム変更イベント
		/// </summary>
		public event EventHandler OnItemInfoChange = (sender, e) => { };
		
		#endregion === Event ===


		#region === Property ===


		/// <summary>
		/// アイテム個数フォーマット
		/// </summary>
		public string ItemCountFormat
		{
			get { return itemCountFormat; }
			set { itemCountFormat = value; }
		}

		/// <summary>
		/// ロック件数
		/// </summary>
		public int LockCount { get; set; }
		
		/// <summary>
		/// 保持期限
		/// </summary>
		public int KeepDays { get; private set; }
		
		/// <summary>
		/// 最大ロック件数
		/// </summary>
		public int MaxLockCount { get; private set; }
		
		/// <summary>
		/// ロック件数が最大か
		/// </summary>
		public bool OverLockCount
		{
			get {
				return (LockCount >= MaxLockCount);
			}
		}

		/// <summary>
		/// 保持期限を過ぎているか
		/// </summary>
		public bool OverKeepDays {
			get
			{
				if(mailInfo != null) {
					var span = DateTime.Now - mailInfo.ReceivedTime;
					return (span.TotalDays >= KeepDays);
				}
				return false;
			}
		}



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


					if(mailInfo != null) {
						if(mailInfo.Type == MailInfo.MailType.Admin) {
							KeepDays = MasterDataCommonSetting.Mail.AdminMailKeepDays;
							MaxLockCount = MasterDataCommonSetting.Mail.AdminMailMaxLockCount;
						} else if(mailInfo.Type == MailInfo.MailType.Present) {

							KeepDays = MasterDataCommonSetting.Mail.PresentMailKeepDays;
							MaxLockCount = MasterDataCommonSetting.Mail.PresentMailMaxLockCount;
						}
					}

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
		/// 本文
		/// </summary>
		public string Body
		{
			get { return body; }
			set
			{
				if(body != value) {
					body = value;

					OnBodyChange(this, EventArgs.Empty);
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


		#endregion === Property ===


		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			mailInfo = null;
			OnMailInfoChange = null;
			OnTitleChange = null;
			OnBodyChange = null;
			OnReceivedTimeChange = null;
			OnLockFlagChange = null;
			OnItemInfoChange = null;
		}

		public void Setup()
		{
			mailInfo = null;
			isLock = true;
			itemInfo = null;
			title = "";
			body = "";

		}
	}
}

