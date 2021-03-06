/// <summary>
/// バッテリーデータ
/// 
/// 2015/12/08
/// </summary>
using System;
using System.Collections.Generic;
using UnityEngine;

namespace XUI
{
	namespace Battery
	{
		// バッテリーレベル変更イベント引数
		public class BatteryLevelChangeEventArgs : EventArgs { };
		// バッテリーレベルリスト変更イベント引数
		public class BatteryLevelListChangeEventArgs : EventArgs { };
		// バッテリー状態変更イベント引数
		public class BatteryStateChangeEventArgs : EventArgs { };

		// ネットワークレベル変更イベント引数
		public class NetworkLevelChangeEventArgs : EventArgs { };
		// ネットワークレベルリスト変更イベント引数
		public class NetworkLevelListChangeEventArgs : EventArgs { };
		// ネットワークタイプ変更イベント引数
		public class NetworkTypeChangeEventArgs : EventArgs { };

		// 時間変更イベント引数
		public class DateTimeChangeEventArgs : EventArgs { };
		// 時間フォーマット変更イベント引数
		public class TimeFormatChangeEventArgs : EventArgs { };

		/// <summary>
		/// レベルに応じた状態変化
		/// level 以下になったら色を変更させる
		/// </summary>
		[System.Serializable]
		public class LevelState
		{
			public LevelState Clone() { return (LevelState)MemberwiseClone(); }

			[SerializeField]
			int _level = 0;
			public int Level { get { return _level; } }
			[SerializeField]
			Color _color = new Color();
			public Color Color { get { return _color; } }
		}

		/// <summary>
		/// バッテリーデータインターフェイス
		/// </summary>
		public interface IModel
		{
			// バッテリーレベル変更イベント
			event EventHandler<BatteryLevelChangeEventArgs> OnBatteryLevelChange;
			/// <summary>
			/// バッテリーレベル(0 ～ 100)
			/// </summary>
			int BatteryLevel { get; set; }

			// バッテリーレベルリスト変更イベント
			event EventHandler<BatteryLevelListChangeEventArgs> OnBatteryLevelListChange;
			/// <summary>
			/// バッテリーレベルリスト設定
			/// </summary>
			void SetBatteryLevelList(List<LevelState> list);
			/// <summary>
			/// 現在のバッテリーレベルの色を取得する
			/// </summary>
			Color GetBatteryLevelColor();

			// バッテリー状態変更イベント
			event EventHandler<BatteryStateChangeEventArgs> OnBatteryStateChange;
			/// <summary>
			/// バッテリー状態
			/// </summary>
			BatteryInfo.BatteryState BatteryState { get; set; }

			// ネットワークレベル変更イベント
			event EventHandler<NetworkLevelChangeEventArgs> OnNetworkLevelChange;
			/// <summary>
			/// ネットワークレベル(0 ～ 100)
			/// </summary>
			int NetworkLevel { get; set; }

			// ネットワークレベルリスト変更イベント
			event EventHandler<NetworkLevelListChangeEventArgs> OnNetworkLevelListChange;
			/// <summary>
			/// ネットワークレベルリスト設定
			/// </summary>
			void SetNetworkLevelList(List<LevelState> list);
			/// <summary>
			/// 現在のネットワークレベルの色を取得する
			/// </summary>
			Color GetNetworkLevelColor();

			// ネットワークタイプ変更イベント
			event EventHandler<NetworkTypeChangeEventArgs> OnNetworkTypeChange;
			/// <summary>
			/// ネットワークタイプ
			/// </summary>
			NetworkInfo.NetworkType NetworkType { get; set; }

			// 時間変更イベント
			event EventHandler<DateTimeChangeEventArgs> OnDateTimeChange;
			/// <summary>
			/// 時間
			/// </summary>
			DateTime DateTime { get; set; }

			// 時間フォーマット変更イベント
			event EventHandler<TimeFormatChangeEventArgs> OnTimeFormatChange;
			/// <summary>
			/// 時間を出力するときのフォーマット
			/// </summary>
			string TimeFormat { get; set; }
		}

		/// <summary>
		/// バッテリーデータ
		/// </summary>
		public class Model : IModel
		{
			#region バッテリーレベル
			// バッテリーレベル変更イベント
			public event EventHandler<BatteryLevelChangeEventArgs> OnBatteryLevelChange = (sender, e) => { };

			/// <summary>
			/// バッテリーレベル(0～100)
			/// </summary>
			int _batteryLevel = 100;
			public int BatteryLevel
			{
				get { return _batteryLevel; }
				set
				{
					var t =Math.Max(0, Math.Min(100, value));
					if (_batteryLevel != t)
					{
						_batteryLevel = t;

						// 通知
						var eventArgs = new BatteryLevelChangeEventArgs();
						this.OnBatteryLevelChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region バッテリーレベルリスト
			// バッテリーレベルリスト変更イベント
			public event EventHandler<BatteryLevelListChangeEventArgs> OnBatteryLevelListChange = (sender, e) => { };

			/// <summary>
			/// バッテリーレベルリスト設定
			/// </summary>
			List<LevelState> _batteryLevelList = new List<LevelState>();
			List<LevelState> BatteryLevelList { get { return _batteryLevelList; } }
			public void SetBatteryLevelList(List<LevelState> list)
			{
				if (list != null && !this.BatteryLevelList.Equals(list))
				{
					this.BatteryLevelList.Clear();
					// 昇順ソート
					list.Sort(delegate(LevelState a, LevelState b) { return a.Level.CompareTo(b.Level); });
					list.ForEach(a => { this.BatteryLevelList.Add(a.Clone()); });

					// 通知
					var eventArgs = new BatteryLevelListChangeEventArgs();
					this.OnBatteryLevelListChange(this, eventArgs);
				}
			}

			/// <summary>
			/// 現在のバッテリーレベルの色を取得する
			/// </summary>
			public Color GetBatteryLevelColor()
			{
				Color color = Color.white;
				foreach (var t in this.BatteryLevelList)
				{
					if (t.Level >= this.BatteryLevel)
					{
						break;
					}
					color = t.Color;
				}
				return color;
			}
			#endregion

			#region バッテリー状態
			// バッテリー状態変更イベント
			public event EventHandler<BatteryStateChangeEventArgs> OnBatteryStateChange = (sender, e) => { };

			/// <summary>
			/// バッテリー状態
			/// </summary>
			BatteryInfo.BatteryState _batteryState = BatteryInfo.BatteryState.STATUS_NONE;
			public BatteryInfo.BatteryState BatteryState
			{
				get { return _batteryState; }
				set
				{
					if (_batteryState != value)
					{
						_batteryState = value;

						var eventArgs = new BatteryStateChangeEventArgs();
						this.OnBatteryStateChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region ネットワークレベル
			// ネットワークレベル変更イベント
			public event EventHandler<NetworkLevelChangeEventArgs> OnNetworkLevelChange = (sender, e) => { };

			/// <summary>
			/// ネットワークレベル(0～100)
			/// </summary>
			int _networkLevel = 100;
			public int NetworkLevel
			{
				get { return _networkLevel; }
				set
				{
					var t = Math.Max(0, Math.Min(100, value));
					if (_networkLevel != t)
					{
						_networkLevel = t;

						// 通知
						var eventArgs = new NetworkLevelChangeEventArgs();
						this.OnNetworkLevelChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region ネットワークレベルリスト
			// ネットワークレベルリスト変更イベント
			public event EventHandler<NetworkLevelListChangeEventArgs> OnNetworkLevelListChange = (sender, e) => { };

			/// <summary>
			/// ネットワークレベルリスト設定
			/// </summary>
			List<LevelState> _networkLevelList = new List<LevelState>();
			List<LevelState> NetworkLevelList { get { return _networkLevelList; } }
			public void SetNetworkLevelList(List<LevelState> list)
			{
				if (list != null && !this.NetworkLevelList.Equals(list))
				{
					this.NetworkLevelList.Clear();
					// 昇順ソート
					list.Sort(delegate(LevelState a, LevelState b) { return a.Level.CompareTo(b.Level); });
					list.ForEach(a => { this.NetworkLevelList.Add(a.Clone()); });

					// 通知
					var eventArgs = new NetworkLevelListChangeEventArgs();
					this.OnNetworkLevelListChange(this, eventArgs);
				}
			}

			/// <summary>
			/// 現在のネットワークレベルの色を取得する
			/// </summary>
			public Color GetNetworkLevelColor()
			{
				Color color = Color.white;
				foreach (var t in this.NetworkLevelList)
				{
					if (t.Level >= this.NetworkLevel)
					{
						break;
					}
					color = t.Color;
				}
				return color;
			}
			#endregion

			#region ネットワークタイプ
			// ネットワークタイプ変更イベント
			public event EventHandler<NetworkTypeChangeEventArgs> OnNetworkTypeChange = (sender, e) => { };

			/// <summary>
			/// ネットワークタイプ
			/// </summary>
			NetworkInfo.NetworkType _networkType = NetworkInfo.NetworkType.TYPE_NONE;
			public NetworkInfo.NetworkType NetworkType
			{
				get { return _networkType; }
				set
				{
					if (_networkType != value)
					{
						_networkType = value;

						var eventArgs = new NetworkTypeChangeEventArgs();
						this.OnNetworkTypeChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 時間
			// 時間変更イベント
			public event EventHandler<DateTimeChangeEventArgs> OnDateTimeChange = (sender, e) => { };

			/// <summary>
			/// 時間
			/// </summary>
			DateTime _dateTime;
			public DateTime DateTime
			{
				get { return _dateTime; }
				set
				{
					if (_dateTime != value)
					{
						_dateTime = value;

						// 通知
						var eventArgs = new DateTimeChangeEventArgs();
						this.OnDateTimeChange(this, eventArgs);
					}
				}
			}
			#endregion

			#region 時間フォーマット
			// 時間フォーマット変更イベント
			public event EventHandler<TimeFormatChangeEventArgs> OnTimeFormatChange = (sender, e) => { };

			/// <summary>
			/// 時間を出力するときのフォーマット
			/// </summary>
			string _timeFormat = "";
			public string TimeFormat
			{
				get { return _timeFormat; }
				set
				{
					if (_timeFormat != value)
					{
						_timeFormat = value;

						// 通知
						var eventArgs = new TimeFormatChangeEventArgs();
						this.OnTimeFormatChange(this, eventArgs);
					}
				}
			}
			#endregion
		}
	}
}
