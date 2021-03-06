/// <summary>
/// キャラ簡易情報データ
/// 
/// 2016/01/25
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace CharaSimpleInfo
	{
		/// <summary>
		/// キャラ簡易情報データインターフェイス
		/// </summary>
		public interface IModel
		{
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();

			/// <summary>
			/// キャラ情報
			/// </summary>
			event EventHandler OnCharaInfoChange;
			CharaInfo CharaInfo { get; set; }

			/// <summary>
			/// 表示位置
			/// </summary>
			event EventHandler OnPositionChange;
			Vector3 Position { get; set; }

			#region ステータス
			/// <summary>
			/// 生命力変更イベント
			/// </summary>
			event EventHandler OnHitPointChange;
			/// <summary>
			/// 生命力
			/// </summary>
			int HitPoint { get; set; }

			/// <summary>
			/// 攻撃力変更イベント
			/// </summary>
			event EventHandler OnAttackChange;
			/// <summary>
			/// 攻撃力
			/// </summary>
			int Attack { get; set; }

			/// <summary>
			/// 防御力変更イベント
			/// </summary>
			event EventHandler OnDefenseChange;
			/// <summary>
			/// 防御力
			/// </summary>
			int Defense { get; set; }

			/// <summary>
			/// 特殊能力変更イベント
			/// </summary>
			event EventHandler OnExtraChange;
			int Extra { get; set; }

			/// <summary>
			/// ステータスフォーマット変更イベント
			/// </summary>
			event EventHandler OnStatusFormatChange;
			string StatusFormat { get; set; }
			#endregion

			/// <summary>
			/// レベル表示フォーマット
			/// </summary>
			event EventHandler OnLevelFormatChange;
			string LevelFormat { get; set; }
		}

		/// <summary>
		/// キャラ簡易情報データ
		/// </summary>
		public class Model : IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				this.OnCharaInfoChange = null;
				this.OnPositionChange = null;
				this.OnHitPointChange = null;
				this.OnAttackChange = null;
				this.OnDefenseChange = null;
				this.OnExtraChange = null;
				this.OnStatusFormatChange = null;
				this.OnLevelFormatChange = null;
			}
			#endregion

			#region キャラ情報
			public event EventHandler OnCharaInfoChange = (sender, e) => { };
			private CharaInfo _charaInfo = null;
			public CharaInfo CharaInfo
			{
				get { return _charaInfo; }
				set
				{
					if(_charaInfo != value)
					{
						_charaInfo = value;

						// 通知
						this.OnCharaInfoChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 表示位置
			public event EventHandler OnPositionChange = (sender, e) => { };
			private Vector3 _position = new Vector3();
			public Vector3 Position
			{
				get { return _position; }
				set
				{
					if(_position != value)
					{
						_position = value;

						// 通知
						this.OnPositionChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region ステータス
			/// <summary>
			/// 生命力変更イベント
			/// </summary>
			public event EventHandler OnHitPointChange = (sender, e) => { };
			/// <summary>
			/// 生命力
			/// </summary>
			private int _hitPoint = 0;
			public int HitPoint
			{
				get { return _hitPoint; }
				set
				{
					if(_hitPoint != value)
					{
						_hitPoint = value;

						// 通知
						this.OnHitPointChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 攻撃力変更イベント
			/// </summary>
			public event EventHandler OnAttackChange = (sender, e) => { };
			/// <summary>
			/// 攻撃力
			/// </summary>
			private int _attack = 0;
			public int Attack
			{
				get { return _attack; }
				set
				{
					if(_attack != value)
					{
						_attack = value;

						// 通知
						this.OnAttackChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 防御力変更イベント
			/// </summary>
			public event EventHandler OnDefenseChange = (sender, e) => { };
			/// <summary>
			/// 防御力
			/// </summary>
			private int _defense = 0;
			public int Defense
			{
				get { return _defense; }
				set
				{
					if(_defense != value)
					{
						_defense = value;

						// 通知
						this.OnDefenseChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 特殊能力変更イベント
			/// </summary>
			public event EventHandler OnExtraChange = (sender, e) => { };
			private int _extra = 0;
			public int Extra
			{
				get { return _extra; }
				set
				{
					if(_extra != value)
					{
						_extra = value;

						// 通知
						this.OnExtraChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// ステータスフォーマット変更イベント
			/// </summary>
			public event EventHandler OnStatusFormatChange = (sender, e) => { };
			private string _statusFormat = string.Empty;
			public string StatusFormat
			{
				get { return _statusFormat; }
				set
				{
					if(_statusFormat != value)
					{
						_statusFormat = value;

						// 通知
						this.OnStatusFormatChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region レベルフォーマット
			/// <summary>
			/// レベル表示フォーマット
			/// </summary>
			public event EventHandler OnLevelFormatChange = (sender, e) => { };
			private string _levelFormat = string.Empty;
			public string LevelFormat
			{
				get { return _levelFormat; }
				set
				{
					if(_levelFormat != value)
					{
						_levelFormat = value;

						// 通知
						this.OnLevelFormatChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion
		}
	}
}