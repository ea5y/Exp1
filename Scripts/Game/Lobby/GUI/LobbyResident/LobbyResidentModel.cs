/// <summary>
/// 常駐メニューデータ
/// 
/// 2016/05/27
/// </summary>
using System;

namespace XUI.LobbyResident
{
	/// <summary>
	/// 常駐メニューデータインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion 破棄

		#region ロビー選択
		/// <summary>
		/// ロビー番号変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnLobbyNoChange;
		/// <summary>
		/// ロビー番号
		/// </summary>
		int LobbyNo { get; set; }

		/// <summary>
		/// ロビー番号フォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnLobbyNoFormatChange;
		/// <summary>
		/// ロビー番号フォーマット
		/// </summary>
		string LobbyNoFormat { get; set; }
		#endregion ロビー選択

		#region ロビーメンバー
		/// <summary>
		/// ロビーメンバー人数変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnLobbyMemberCountChange;
		/// <summary>
		/// ロビーメンバー人数
		/// </summary>
		int LobbyMemberCount { get; set; }

		/// <summary>
		/// ロビー収容人数変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnLobbyMemberCapacityChange;
		/// <summary>
		/// ロビー収容人数
		/// </summary>
		int LobbyMemberCapacity { get; set; }

		/// <summary>
		/// ロビーメンバーフォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnLobbyMemberFormatChange;
		/// <summary>
		/// ロビーメンバーフォーマット
		/// </summary>
		string LobbyMemberFormat { get; set; }
		#endregion ロビーメンバー

		#region 通知系
		/// <summary>
		/// 未取得アチーブメント数変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnAchieveUnreceivedChange;
		/// <summary>
		/// 未取得アチーブメント数
		/// </summary>
		int AchieveUnreceived { get; set; }

		/// <summary>
		/// 未読メール数変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnMailUnreadChange;
		/// <summary>
		/// 未読メール数
		/// </summary>
		int MailUnread { get; set; }

		/// <summary>
		/// 未読メール数変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnApplyUnprocessedChange;
		/// <summary>
		/// 未読メール数
		/// </summary>
		int ApplyUnprocessed { get; set; }

		/// <summary>
		/// 通知フォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnAlertFormatChange;
		/// <summary>
		/// 通知フォーマット
		/// </summary>
		string AlertFormat { get; set; }
		#endregion 通知系

		#region プレイヤー情報
		/// <summary>
		/// プレイヤー名変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnPlayerNameChange;
		/// <summary>
		/// プレイヤー名
		/// </summary>
		string PlayerName { get; set; }

		/// <summary>
		/// プレイヤー勝利数変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnPlayerWinChange;
		/// <summary>
		/// プレイヤー勝利数
		/// </summary>
		int PlayerWin { get; set; }

		/// <summary>
		/// プレイヤー敗北数変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnPlayerLoseChange;
		/// <summary>
		/// プレイヤー敗北数
		/// </summary>
		int PlayerLose { get; set; }

		/// <summary>
		/// プレイヤー勝敗数フォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnPlayerWinLoseFormatChange;
		/// <summary>
		/// プレイヤー勝敗数フォーマット
		/// </summary>
		string PlayerWinLoseFormat { get; set; }
		#endregion プレイヤー情報
	}

	/// <summary>
	/// 常駐メニューデータ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnLobbyNoChange = null;
			this.OnLobbyNoFormatChange = null;
			this.OnLobbyMemberCountChange = null;
			this.OnLobbyMemberCapacityChange = null;
			this.OnLobbyMemberFormatChange = null;
			this.OnAchieveUnreceivedChange = null;
			this.OnMailUnreadChange = null;
			this.OnApplyUnprocessedChange = null;
			this.OnAlertFormatChange = null;
			this.OnPlayerNameChange = null;
			this.OnPlayerWinChange = null;
			this.OnPlayerLoseChange = null;
			this.OnPlayerWinLoseFormatChange = null;
		}
		#endregion 破棄

		#region ロビー選択
		/// <summary>
		/// ロビー番号変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnLobbyNoChange = (sender, e) => { };
		/// <summary>
		/// ロビー番号
		/// </summary>
		int _lobbyNo = 0;
		public int LobbyNo
		{
			get { return _lobbyNo; }
			set
			{
				if (_lobbyNo != value)
				{
					_lobbyNo = value;
					this.OnLobbyNoChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// ロビー番号フォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnLobbyNoFormatChange = (sender, e) => { };
		/// <summary>
		/// ロビー番号フォーマット
		/// </summary>
		string _lobbyNoFormat = "";
		public string LobbyNoFormat
		{
			get { return _lobbyNoFormat; }
			set
			{
				if (_lobbyNoFormat != value)
				{
					_lobbyNoFormat = value;
					this.OnLobbyNoFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion ロビー選択

		#region ロビーメンバー
		/// <summary>
		/// ロビーメンバー人数変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnLobbyMemberCountChange = (sender, e) => { };
		/// <summary>
		/// ロビーメンバー人数
		/// </summary>
		int _lobbyMemberCount = 0;
		public int LobbyMemberCount
		{
			get { return _lobbyMemberCount; }
			set
			{
				if (_lobbyMemberCount != value)
				{
					_lobbyMemberCount = value;
					this.OnLobbyMemberCountChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// ロビー収容人数変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnLobbyMemberCapacityChange = (sender, e) => { };
		/// <summary>
		/// ロビー収容人数
		/// </summary>
		int _lobbyMemberCapacity = 0;
		public int LobbyMemberCapacity
		{
			get { return _lobbyMemberCapacity; }
			set
			{
				if (_lobbyMemberCapacity != value)
				{
					_lobbyMemberCapacity = value;
					this.OnLobbyMemberCapacityChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// ロビーメンバーフォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnLobbyMemberFormatChange = (sender, e) => { };
		/// <summary>
		/// ロビーメンバーフォーマット
		/// </summary>
		string _lobbyMemberFormat = "";
		public string LobbyMemberFormat
		{
			get { return _lobbyMemberFormat; }
			set
			{
				if (_lobbyMemberFormat != value)
				{
					_lobbyMemberFormat = value;
					this.OnLobbyMemberFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion ロビーメンバー

		#region 通知系
		/// <summary>
		/// 未取得アチーブメント数変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnAchieveUnreceivedChange = (sender, e) => { };
		/// <summary>
		/// 未取得アチーブメント数
		/// </summary>
		int _achieveUnreceived = 0;
		public int AchieveUnreceived
		{
			get { return _achieveUnreceived; }
			set
			{
				if (_achieveUnreceived != value)
				{
					_achieveUnreceived = value;
					this.OnAchieveUnreceivedChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 未読メール数変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnMailUnreadChange = (sender, e) => { };
		/// <summary>
		/// 未読メール数
		/// </summary>
		int _mailUnread = 0;
		public int MailUnread
		{
			get { return _mailUnread; }
			set
			{
				if (_mailUnread != value)
				{
					_mailUnread = value;
					this.OnMailUnreadChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 未読メール数変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnApplyUnprocessedChange = (sender, e) => { };
		/// <summary>
		/// 未読メール数
		/// </summary>
		int _applyUnprocessed = 0;
		public int ApplyUnprocessed
		{
			get { return _applyUnprocessed; }
			set
			{
				if (_applyUnprocessed != value)
				{
					_applyUnprocessed = value;
					this.OnApplyUnprocessedChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 通知フォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnAlertFormatChange = (sender, e) => { };
		/// <summary>
		/// 通知フォーマット
		/// </summary>
		string _alertFormat = "";
		public string AlertFormat
		{
			get { return _alertFormat; }
			set
			{
				if (_alertFormat != value)
				{
					_alertFormat = value;
					this.OnAlertFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion 通知系

		#region プレイヤー情報
		/// <summary>
		/// プレイヤー名変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnPlayerNameChange = (sender, e) => { };
		/// <summary>
		/// プレイヤー名
		/// </summary>
		string _playerName = "";
		public string PlayerName
		{
			get { return _playerName; }
			set
			{
				if (_playerName != value)
				{
					_playerName = value;
					this.OnPlayerNameChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// プレイヤー勝利数変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnPlayerWinChange = (sender, e) => { };
		/// <summary>
		/// プレイヤー勝利数
		/// </summary>
		int _playerWin = 0;
		public int PlayerWin
		{
			get { return _playerWin; }
			set
			{
				if (_playerWin != value)
				{
					_playerWin = value;
					this.OnPlayerWinChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// プレイヤー敗北数変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnPlayerLoseChange = (sender, e) => { };
		/// <summary>
		/// プレイヤー敗北数
		/// </summary>
		int _playerLose = 0;
		public int PlayerLose
		{
			get { return _playerLose; }
			set
			{
				if (_playerLose != value)
				{
					_playerLose = value;
					this.OnPlayerLoseChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// プレイヤー勝敗数フォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnPlayerWinLoseFormatChange = (sender, e) => { };
		/// <summary>
		/// プレイヤー勝敗数フォーマット
		/// </summary>
		string _playerWinLoseFormat = "";
		public string PlayerWinLoseFormat
		{
			get { return _playerWinLoseFormat; }
			set
			{
				if (_playerWinLoseFormat != value)
				{
					_playerWinLoseFormat = value;
					this.OnPlayerWinLoseFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion プレイヤー情報
	}
}
