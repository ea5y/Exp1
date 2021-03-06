/// <summary>
/// バッテリー制御
/// 
/// 2015/12/08
/// </summary>
using System;

namespace XUI
{
	namespace Battery
	{
		/// <summary>
		/// バッテリー制御インターフェイス
		/// </summary>
		public interface IController
		{
			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }
		}

		/// <summary>
		/// バッテリー制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド＆プロパティ
			// モデル
			readonly IModel _model;
			IModel Model { get { return _model; } }
			// ビュー
			readonly IView _view;
			IView View { get { return _view; } }
			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			public bool CanUpdate
			{
				get
				{
					if (this.Model == null) return false;
					if (this.View == null) return false;
					return true;
				}
			}
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public Controller(IModel model, IView view)
			{
				if (model == null || view == null) return;

				// ビュー設定
				this._view = view;

				// モデル設定
				this._model = model;
				this.Model.OnBatteryLevelChange += this.HandleBatteryLevelChange;
				this.Model.OnBatteryLevelListChange += this.HandleBatteryLevelListChange;
				this.Model.OnBatteryStateChange += this.HandleBatteryStateChange;
				this.Model.OnNetworkLevelChange += this.HandleNetworkLevelChange;
				this.Model.OnNetworkLevelListChange += this.HandleNetworkLevelListChange;
				this.Model.OnNetworkTypeChange += this.HandleNetworkTypeChange;
				this.Model.OnDateTimeChange += this.HandleDateTimeChange;
				this.Model.OnTimeFormatChange += this.HandleTimeFormatChange;

				// 同期
				this.SyncBatteryLevel();
				this.SyncBatteryState();
				this.SyncNetworkLevel();
				this.SyncNetworkType();
				this.SyncTime();
			}
			#endregion

			#region バッテリーレベル
			/// <summary>
			/// バッテリーレベル変更イベント
			/// </summary>
			void HandleBatteryLevelChange(object sender, BatteryLevelChangeEventArgs e)
			{
				this.SyncBatteryLevel();
			}
			/// <summary>
			/// バッテリーレベルリスト変更イベント
			/// </summary>
			void HandleBatteryLevelListChange(object sender, BatteryLevelListChangeEventArgs e)
			{
				this.SyncBatteryLevel();
			}
			/// <summary>
			/// バッテリーレベル同期
			/// </summary>
			void SyncBatteryLevel()
			{
				if (this.CanUpdate)
				{
					this.View.SetBatteryLevel(this.Model.BatteryLevel);
					this.View.SetBatteryColor(this.Model.GetBatteryLevelColor());
				}
			}
			#endregion

			#region バッテリー状態
			/// <summary>
			/// バッテリー状態変更イベント
			/// </summary>
			void HandleBatteryStateChange(object sender, BatteryStateChangeEventArgs e)
			{
				this.SyncBatteryState();
			}
			/// <summary>
			/// バッテリー状態同期
			/// </summary>
			void SyncBatteryState()
			{
				if (this.CanUpdate)
				{
					this.View.SetBatteryState(this.Model.BatteryState);
				}
			}
			#endregion

			#region ネットワークレベル
			/// <summary>
			/// ネットワークレベル変更イベント
			/// </summary>
			void HandleNetworkLevelChange(object sender, NetworkLevelChangeEventArgs e)
			{
				this.SyncNetworkLevel();
			}
			/// <summary>
			/// ネットワークレベルリスト変更イベント
			/// </summary>
			void HandleNetworkLevelListChange(object sender, NetworkLevelListChangeEventArgs e)
			{
				this.SyncNetworkLevel();
			}
			void SyncNetworkLevel()
			{
				if (this.CanUpdate)
				{
					this.View.SetNetworkLevel(this.Model.NetworkLevel);
					this.View.SetNetworkColor(this.Model.GetNetworkLevelColor());
				}
			}
			#endregion

			#region ネットワークタイプ
			/// <summary>
			/// ネットワークタイプ変更イベント
			/// </summary>
			void HandleNetworkTypeChange(object sender, NetworkTypeChangeEventArgs e)
			{
				this.SyncNetworkType();
			}
			/// <summary>
			/// ネットワークタイプ同期
			/// </summary>
			void SyncNetworkType()
			{
				if (this.CanUpdate)
				{
					this.View.SetNetworkType(this.Model.NetworkType);
				}
			}
			#endregion

			#region 時間
			/// <summary>
			/// 時間変更イベント
			/// </summary>
			void HandleDateTimeChange(object sender, DateTimeChangeEventArgs e)
			{
				this.SyncTime();
			}
			/// <summary>
			/// 時間フォーマット変更イベント
			/// </summary>
			void HandleTimeFormatChange(object sender, TimeFormatChangeEventArgs e)
			{
				this.SyncTime();
			}
			/// <summary>
			/// 時間同期
			/// </summary>
			void SyncTime()
			{
				if (this.CanUpdate)
				{
					this.View.SetTime(this.Model.DateTime, this.Model.TimeFormat);
				}
			}
			#endregion
		}
	}
}
