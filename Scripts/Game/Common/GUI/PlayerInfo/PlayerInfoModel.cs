/// <summary>
/// プレイヤー情報データ
/// 
/// 2015/12/10
/// </summary>
using System;

namespace XUI
{
	namespace PlayerInfo
	{
		public class AvatarTypeChangeEventArgs : EventArgs { };

		public class NameChangeEventArgs : EventArgs { };

		public class GradeChangeEventArgs : EventArgs { };
		public class GradeFormatChangeEventArgs : EventArgs { };

		public class LvChangeEventArgs : EventArgs { };
		public class LvFormatChangeEventArgs : EventArgs { };

		public class ExpChangeEventArgs : EventArgs { };
		public class ExpMinChangeEventArgs : EventArgs { };
		public class ExpMaxChangeEventArgs : EventArgs { };
		public class ExpFormatChangeEventArgs : EventArgs { };

		public class StaminaChangeEventArgs : EventArgs { };
		public class StaminaMaxChangeEventArgs : EventArgs { };
		public class StaminaFormatChangeEventArgs : EventArgs { };

		public class StaminaTimeChangeEventArgs : EventArgs { };
		public class StaminaTimeFormatChangeEventArgs : EventArgs { };
		public class StaminaRecoveryTimeChangeEventArgs : EventArgs { };

		/// <summary>
		/// プレイヤー情報データインターフェイス
		/// </summary>
		public interface IModel
		{
			event EventHandler<AvatarTypeChangeEventArgs> OnAvatarTypeChange;
			AvatarType AvatarType { get; set; }

            int SkinId { get; set; }

			event EventHandler<NameChangeEventArgs> OnNameChange;
			string Name { get; set; }
			event EventHandler<EventArgs> OnNameFormatChange;
			string NameFormat { get; set; }

			event EventHandler<GradeChangeEventArgs> OnGradeChange;
			int Grade { get; set; }
			event EventHandler<GradeFormatChangeEventArgs> OnGradeFormatChange;
			string GradeFormat { get; set; }

			event EventHandler<LvChangeEventArgs> OnLvChange;
			int Lv { get; set; }
			event EventHandler<LvFormatChangeEventArgs> OnLvFormatChange;
			string LvFormat { get; set; }

			event EventHandler<ExpChangeEventArgs> OnExpChange;
			long Exp { get; set; }
			event EventHandler<ExpMinChangeEventArgs> OnExpMinChange;
			long ExpMin { get; set; }
			event EventHandler<ExpMaxChangeEventArgs> OnExpMaxChange;
			long ExpMax { get; set; }
			float ExpSliderValue { get; }
			event EventHandler<ExpFormatChangeEventArgs> OnExpFormatChange;
			string ExpFormat { get; set; }

			event EventHandler<StaminaChangeEventArgs> OnStaminaChange;
			int Stamina { get; set; }
			event EventHandler<StaminaMaxChangeEventArgs> OnStaminaMaxChange;
			int StaminaMax { get; set; }
			event EventHandler<StaminaFormatChangeEventArgs> OnStaminaFormatChange;
			string StaminaFormat { get; set; }

			event EventHandler<StaminaTimeChangeEventArgs> OnStaminaTimeChange;
			event EventHandler<StaminaTimeFormatChangeEventArgs> OnStaminaTimeFormatChange;
			string StaminaTimeFormat { get; set; }
			event EventHandler<StaminaRecoveryTimeChangeEventArgs> OnStaminaRecoveryTimeChange;
			DateTime StaminaRecoveryTime { get; set; }

			void SyncStaminaTime(DateTime nowTime);
			void CountDownStaminaTime(float second);
			int GetStaminaTime();
		}

		/// <summary>
		/// プレイヤー情報データ
		/// </summary>
		public class Model : IModel
		{
			#region AvatarType
			public event EventHandler<AvatarTypeChangeEventArgs> OnAvatarTypeChange = (sender, e) => { };
			AvatarType _avatarType = AvatarType.None;
			public AvatarType AvatarType
			{
				get { return _avatarType; }
				set
				{
					if (_avatarType != value)
					{
						_avatarType = value;

						// 通知
						var eventArgs = new AvatarTypeChangeEventArgs();
						this.OnAvatarTypeChange(this, eventArgs);
					}
				}
			}
            int _skinId = 0;
            public int SkinId {
                get {
                    return _skinId;
                }
                set {
                    if (_skinId != value) {
                        _skinId = value;

                        // 通知
                        var eventArgs = new AvatarTypeChangeEventArgs();
                        this.OnAvatarTypeChange(this, eventArgs);
                    }
                }
            }
            #endregion

            #region 名前
            public event EventHandler<NameChangeEventArgs> OnNameChange = (sender, e) => { };
			string _name = "";
			public string Name
			{
				get { return _name; }
				set
				{
					if (_name != value)
					{
						_name = value;

						// 通知
						var eventArgs = new NameChangeEventArgs();
						this.OnNameChange(this, eventArgs);
					}
				}
			}

			public event EventHandler<EventArgs> OnNameFormatChange = (sender, e) => { };
			string _nameFormat = "";
			public string NameFormat
			{
				get { return _nameFormat; }
				set
				{
					if (_nameFormat != value)
					{
						_nameFormat = value;

						// 通知
						var eventArg = new EventArgs();
						this.OnNameFormatChange(this, eventArg);
					}
				}
			}
			#endregion

			#region グレード
			public event EventHandler<GradeChangeEventArgs> OnGradeChange = (sender, e) => { };
			int _grade = 0;
			public int Grade
			{
				get { return _grade; }
				set
				{
					if (_grade != value)
					{
						_grade = value;

						// 通知
						var eventArg = new GradeChangeEventArgs();
						this.OnGradeChange(this, eventArg);
					}
				}
			}

			public event EventHandler<GradeFormatChangeEventArgs> OnGradeFormatChange = (sender, e) => { };
			string _gradeFormat = "";
			public string GradeFormat
			{
				get { return _gradeFormat; }
				set
				{
					if (_gradeFormat != value)
					{
						_gradeFormat = value;

						// 通知
						var eventArg = new GradeFormatChangeEventArgs();
						this.OnGradeFormatChange(this, eventArg);
					}
				}
			}
			#endregion

			#region プレイヤーレベル
			public event EventHandler<LvChangeEventArgs> OnLvChange = (sender, e) => { };
			int _lv = 0;
			public int Lv
			{
				get { return _lv; }
				set
				{
					if (_lv != value)
					{
						_lv = value;

						// 通知
						var eventArg = new LvChangeEventArgs();
						this.OnLvChange(this, eventArg);
					}
				}
			}

			public event EventHandler<LvFormatChangeEventArgs> OnLvFormatChange = (sender, e) => { };
			string _lvFormat = "";
			public string LvFormat
			{
				get { return _lvFormat; }
				set
				{
					if (_lvFormat != value)
					{
						_lvFormat = value;

						// 通知
						var eventArg = new LvFormatChangeEventArgs();
						this.OnLvFormatChange(this, eventArg);
					}
				}
			}
			#endregion

			#region プレイヤー経験値
			public event EventHandler<ExpChangeEventArgs> OnExpChange = (sender, e) => { };
			long _exp = 0;
			public long Exp
			{
				get { return _exp; }
				set
				{
					if (_exp != value)
					{
						_exp = value;

						// 通知
						var eventArg = new ExpChangeEventArgs();
						this.OnExpChange(this, eventArg);
					}
				}
			}

			public event EventHandler<ExpMinChangeEventArgs> OnExpMinChange = (sender, e) => { };
			long _expMin = 0;
			public long ExpMin
			{
				get { return _expMin; }
				set
				{
					if (_expMin != value)
					{
						_expMin = value;

						// 通知
						var eventArg = new ExpMinChangeEventArgs();
						this.OnExpMinChange(this, eventArg);
					}
				}
			}

			public event EventHandler<ExpMaxChangeEventArgs> OnExpMaxChange = (sender, e) => { };
			long _expMax = 0;
			public long ExpMax
			{
				get { return _expMax; }
				set
				{
					if (_expMax != value)
					{
						_expMax = value;

						// 通知
						var eventArg = new ExpMaxChangeEventArgs();
						this.OnExpMaxChange(this, eventArg);
					}
				}
			}

			public float ExpSliderValue
			{
				get
				{
					var t1 = (float)(this.Exp - this.ExpMin);
					var t2 = (float)(this.ExpMax - this.ExpMin);
					var sliderValue = t2 != 0f ? t1 / t2 : 0f;
					return sliderValue;
				}
			}

			public event EventHandler<ExpFormatChangeEventArgs> OnExpFormatChange = (sender, e) => { };
			string _expFormat = "";
			public string ExpFormat
			{
				get { return _expFormat; }
				set
				{
					if (_expFormat != value)
					{
						_expFormat = value;

						// 通知
						var eventArg = new ExpFormatChangeEventArgs();
						this.OnExpFormatChange(this, eventArg);
					}
				}
			}
			#endregion

			#region スタミナ
			public event EventHandler<StaminaChangeEventArgs> OnStaminaChange = (sender, e) => { };
			int _stamina = 0;
			public int Stamina
			{
				get { return _stamina; }
				set
				{
					if (_stamina != value)
					{
						_stamina = value;

						// 通知
						var eventArg = new StaminaChangeEventArgs();
						this.OnStaminaChange(this, eventArg);
					}
				}
			}

			public event EventHandler<StaminaMaxChangeEventArgs> OnStaminaMaxChange = (sender, e) => { };
			int _staminaMax = 0;
			public int StaminaMax
			{
				get { return _staminaMax; }
				set
				{
					if (_staminaMax != value)
					{
						_staminaMax = value;

						// 通知
						var eventArg = new StaminaMaxChangeEventArgs();
						this.OnStaminaMaxChange(this, eventArg);
					}
				}
			}

			public event EventHandler<StaminaFormatChangeEventArgs> OnStaminaFormatChange = (sender, e) => { };
			string _staminaFormat = "";
			public string StaminaFormat
			{
				get { return _staminaFormat; }
				set
				{
					if (_staminaFormat != value)
					{
						_staminaFormat = value;

						// 通知
						var eventArg = new StaminaFormatChangeEventArgs();
						this.OnStaminaFormatChange(this, eventArg);
					}
				}
			}
			#endregion

			#region スタミナ回復までの残り時間
			public event EventHandler<StaminaTimeChangeEventArgs> OnStaminaTimeChange = (sender, e) => { };
			float _staminaTime = 0f;
			float StaminaTime
			{
				get { return _staminaTime; }
				set
				{
					if (_staminaTime != value)
					{
						_staminaTime = value <= 0f ? 0f : value;
						this.StaminaTimeInteger = (int)_staminaTime;
					}
				}
			}
			int _staminaTimeInteger = 0;
			int StaminaTimeInteger
			{
				get { return _staminaTimeInteger; }
				set
				{
					if (_staminaTimeInteger != value)
					{
						_staminaTimeInteger = value;

						// 通知
						var eventArg = new StaminaTimeChangeEventArgs();
						this.OnStaminaTimeChange(this, eventArg);
					}
				}
			}

			public event EventHandler<StaminaTimeFormatChangeEventArgs> OnStaminaTimeFormatChange = (sender, e) => { };
			string _staminaTimeFormat = "";
			public string StaminaTimeFormat
			{
				get { return _staminaTimeFormat; }
				set
				{
					if (_staminaTimeFormat != value)
					{
						_staminaTimeFormat = value;

						// 通知
						var eventArg = new StaminaTimeFormatChangeEventArgs();
						this.OnStaminaTimeFormatChange(this, eventArg);
					}
				}
			}

			public event EventHandler<StaminaRecoveryTimeChangeEventArgs> OnStaminaRecoveryTimeChange = (sender, e) => { };
			DateTime _staminaRecoveryTime = DateTime.Now;
			public DateTime StaminaRecoveryTime
			{
				get { return _staminaRecoveryTime; }
				set
				{
					if (_staminaRecoveryTime != value)
					{
						_staminaRecoveryTime = value;

						// 通知
						var eventArg = new StaminaRecoveryTimeChangeEventArgs();
						this.OnStaminaRecoveryTimeChange(this, eventArg);
					}
				}
			}

			public void SyncStaminaTime(DateTime nowTime)
			{
				TimeSpan ts = this.StaminaRecoveryTime - nowTime;
				this.StaminaTime = (float)ts.TotalSeconds;
			}
			public void CountDownStaminaTime(float second)
			{
				this.StaminaTime -= second;
			}
			public int GetStaminaTime()
			{
				return this.StaminaTimeInteger;
			}
			#endregion
		}
	}
}
