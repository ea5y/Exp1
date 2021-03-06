/// <summary>
/// バッテリー表示
/// 
/// 2015/12/08
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI
{
	namespace Battery
	{
		/// <summary>
		/// バッテリー表示インターフェイス
		/// </summary>
		public interface IView
		{
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			void SetActive(bool isActive);

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			GUIViewBase.ActiveState GetActiveState();

			/// <summary>
			/// バッテリーレベル設定(0～100)
			/// </summary>
			void SetBatteryLevel(int level);

			/// <summary>
			/// バッテリー色設定
			/// </summary>
			void SetBatteryColor(Color color);

			/// <summary>
			/// バッテリー状態設定
			/// </summary>
			void SetBatteryState(BatteryInfo.BatteryState state);

			/// <summary>
			/// ネットワークレベル設定(0～100)
			/// </summary>
			void SetNetworkLevel(int level);

			/// <summary>
			/// ネットワーク色設定
			/// </summary>
			void SetNetworkColor(Color color);

			/// <summary>
			/// ネットワークタイプ設定
			/// </summary>
			void SetNetworkType(NetworkInfo.NetworkType type);

			/// <summary>
			/// 時間表示
			/// </summary>
			void SetTime(DateTime dateTime, string format);
		}

		/// <summary>
		/// バッテリー表示
		/// </summary>
		public class BatteryView : GUIViewBase, IView
		{
			#region アクティブ
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			public void SetActive(bool isActive)
			{
				this.SetRootActive(isActive);
			}
			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			public GUIViewBase.ActiveState GetActiveState()
			{
				return this.GetRootActiveState();
			}
			#endregion

			#region バッテリーレベル
			[SerializeField]
			UIProgressBar _batterySlider = null;
			UIProgressBar BatterySlider { get { return _batterySlider; } }

			/// <summary>
			/// バッテリーレベル設定(0～100)
			/// </summary>
			public void SetBatteryLevel(int level)
			{
				if (this.BatterySlider != null)
				{
					var t = (float)level * 0.01f;
					this.BatterySlider.value = Mathf.Clamp01(t);
				}
			}
			#endregion

			#region バッテリー色
			[SerializeField]
			UISprite _batterySprite = null;
			UISprite BatterySprite { get { return _batterySprite; } }

			/// <summary>
			/// バッテリー色設定
			/// </summary>
			public void SetBatteryColor(Color color)
			{
				if (this.BatterySprite != null)
				{
					this.BatterySprite.color = color;
				}
			}
			#endregion

			#region バッテリー状態
			[SerializeField]
			List<BatteryState> _batteryStateList = new List<BatteryState>();
			List<BatteryState> BatteryStateList { get { return _batteryStateList; } }
			[System.Serializable]
			public class BatteryState
			{
				[SerializeField]
				BatteryInfo.BatteryState _state = BatteryInfo.BatteryState.STATUS_NONE;
				public BatteryInfo.BatteryState State { get { return _state; } }

				[SerializeField]
				GameObject _root = null;
				public GameObject Root { get { return _root; } }
			}

			/// <summary>
			/// バッテリー状態設定
			/// </summary>
			public void SetBatteryState(BatteryInfo.BatteryState state)
			{
				if (this.BatteryStateList == null) return;

				BatteryState batteryState = null;
				foreach (var t in this.BatteryStateList)
				{
					// 現在の状態を記憶
					if (t.State == state) batteryState = t;
					// いったん非表示にする
					if (t.Root != null)
					{
						t.Root.SetActive(false);
					}
				}
				// 現在の状態を表示する
				if (batteryState != null && batteryState.Root != null)
				{
					batteryState.Root.SetActive(true);
				}
			}
			#endregion

			#region ネットワークレベル
			[SerializeField]
			UIProgressBar _networkSlider = null;
			UIProgressBar NetworkSlider { get { return _networkSlider; } }

			/// <summary>
			/// ネットワークレベル設定(0～100)
			/// </summary>
			public void SetNetworkLevel(int level)
			{
				if (this.NetworkSlider != null)
				{
					var t = (float)level * 0.01f;
					this.NetworkSlider.value = Mathf.Clamp01(t);
				}
			}
			#endregion

			#region ネットワーク色
			[SerializeField]
			UISprite _networkSprite = null;
			UISprite NetworkSprite { get { return _networkSprite; } }

			/// <summary>
			/// ネットワーク色設定
			/// </summary>
			public void SetNetworkColor(Color color)
			{
				if (this.NetworkSprite != null)
				{
					this.NetworkSprite.color = color;
				}
			}
			#endregion

			#region ネットワークタイプ
			[SerializeField]
			List<NetworkType> _networkTypeList = new List<NetworkType>();
			List<NetworkType> NetworkTypeList { get { return _networkTypeList; } }
			[System.Serializable]
			public class NetworkType
			{
				[SerializeField]
				NetworkInfo.NetworkType _type = NetworkInfo.NetworkType.TYPE_NONE;
				public NetworkInfo.NetworkType Type { get { return _type; } }

				[SerializeField]
				GameObject _root = null;
				public GameObject Root { get { return _root; } }
			}

			/// <summary>
			/// ネットワークタイプ設定
			/// </summary>
			public void SetNetworkType(NetworkInfo.NetworkType type)
			{
				if (this.NetworkTypeList == null) return;

				NetworkType networkType = null;
				foreach (var t in this.NetworkTypeList)
				{
					// 現在の状態を記憶
					if (t.Type == type) networkType = t;
					// いったん非表示にする
					if (t.Root != null)
					{
						t.Root.SetActive(false);
					}
				}
				// 現在の状態を表示する
				if (networkType != null && networkType.Root != null)
				{
					networkType.Root.SetActive(true);
				}
			}
			#endregion

			#region 時間
			[SerializeField]
			UILabel _timeLabel = null;
			UILabel TimeLabel { get { return _timeLabel; } }

			/// <summary>
			/// 時間表示
			/// </summary>
			public void SetTime(DateTime dateTime, string format)
			{
				if (this.TimeLabel != null)
				{
					// カスタム日時書式指定文字列
					// https://msdn.microsoft.com/ja-jp/library/8kb3ddd4(v=vs.110).aspx
					this.TimeLabel.text = dateTime.ToString(format);
				}
			}
			#endregion
		}
	}
}
