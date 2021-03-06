/// <summary>
/// シンクロ合成結果データ
/// 
/// 2016/03/07
/// </summary>
using System;

namespace XUI.SynchroResult
{
	/// <summary>
	/// シンクロ合成結果データインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region アバタータイプ
		/// <summary>
		/// アバタータイプ変更イベント
		/// </summary>
		event EventHandler OnAvatarTypeChange;

		/// <summary>
		/// アバタータイプ
		/// </summary>
		AvatarType AvatarType { get; set; }

        /// <summary>
        ///  Skin Id
        /// </summary>
        int SkinId { get; set; }
        #endregion

        #region 生命力
        /// <summary>
        /// 生命力
        /// </summary>
        int HitPoint { get; }
		/// <summary>
		/// シンクロ生命力
		/// </summary>
		int SynchroHitPoint { get; }
		/// <summary>
		/// シンクロ生命力アップ分
		/// </summary>
		int HitPointUp { get; }

		/// <summary>
		/// 生命力変更イベント
		/// </summary>
		event EventHandler OnHitPointChange;
		/// <summary>
		/// 生命力設定
		/// </summary>
		void SetHitPoint(int hp, int synchroHp, int up);

		/// <summary>
		/// 生命力フォーマット変更イベント
		/// </summary>
		event EventHandler OnHitPointFormatChange;
		/// <summary>
		/// 生命力フォーマット
		/// </summary>
		string HitPointFormat { get; set; }
		/// <summary>
		/// シンクロ生命力フォーマット変更イベント
		/// </summary>
		event EventHandler OnSynchroHitPointFormatChange;
		/// <summary>
		/// シンクロ生命力フォーマット
		/// </summary>
		string SynchroHitPointFormat { get; set; }
		/// <summary>
		/// シンクロ生命力アップ分フォーマット変更イベント
		/// </summary>
		event EventHandler OnHitPointUpFormatChange;
		/// <summary>
		/// シンクロ生命力アップ分フォーマット
		/// </summary>
		string HitPointUpFormat { get; set; }
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力
		/// </summary>
		int Attack { get; }
		/// <summary>
		/// シンクロ攻撃力
		/// </summary>
		int SynchroAttack { get; }
		/// <summary>
		/// シンクロ攻撃力アップ
		/// </summary>
		int AttackUp { get; }

		/// <summary>
		/// 攻撃力設定
		/// </summary>
		event EventHandler OnAttackChange;
		/// <summary>
		/// 攻撃力設定
		/// </summary>
		void SetAttack(int atk, int synchroAtk, int up);

		/// <summary>
		/// 攻撃力フォーマット変更イベント
		/// </summary>
		event EventHandler OnAttackFormatChange;
		/// <summary>
		/// 攻撃力フォーマット
		/// </summary>
		string AttackFormat { get; set; }
		/// <summary>
		/// シンクロ攻撃力フォーマット変更イベント
		/// </summary>
		event EventHandler OnSynchroAttackFormatChange;
		/// <summary>
		/// シンクロ攻撃力フォーマット
		/// </summary>
		string SynchroAttackFormat { get; set; }
		/// <summary>
		/// 攻撃力アップ分フォーマット変更イベント
		/// </summary>
		event EventHandler OnAttackUpFormatChange;
		/// <summary>
		/// 攻撃力アップ分フォーマット
		/// </summary>
		string AttackUpFormat { get; set; }
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力
		/// </summary>
		int Defence { get; }
		/// <summary>
		/// シンクロ防御力
		/// </summary>
		int SynchroDefence { get; }
		/// <summary>
		/// 防御力アップ分
		/// </summary>
		int DefenceUp { get; }

		/// <summary>
		/// 防御力変更イベント
		/// </summary>
		event EventHandler OnDefenceChange;
		/// <summary>
		/// 防御力設定
		/// </summary>
		void SetDefence(int def, int synchroDef, int up);

		/// <summary>
		/// 防御力フォーマット変更イベント
		/// </summary>
		event EventHandler OnDefenceFormatChange;
		/// <summary>
		/// 防御力フォーマット
		/// </summary>
		string DefenceFormat { get; set; }
		/// <summary>
		/// シンクロ防御力フォーマット変更イベント
		/// </summary>
		event EventHandler OnSynchroDefenceFormatChange;
		/// <summary>
		/// シンクロ防御力フォーマット
		/// </summary>
		string SynchroDefenceFormat { get; set; }
		/// <summary>
		/// シンクロ防御力アップ分フォーマット変更イベント
		/// </summary>
		event EventHandler OnDefenceUpFormatChange;
		/// <summary>
		/// シンクロ防御力アップ分フォーマット
		/// </summary>
		string DefenceUpFormat { get; set; }
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力
		/// </summary>
		int Extra { get; }
		/// <summary>
		/// シンクロ特殊能力
		/// </summary>
		int SynchroExtra { get; }
		/// <summary>
		/// シンクロ特殊能力アップ分
		/// </summary>
		int ExtraUp { get; }

		/// <summary>
		/// 特殊能力変更イベント
		/// </summary>
		event EventHandler OnExtraChange;
		/// <summary>
		/// 特殊能力設定
		/// </summary>
		void SetExtra(int extra, int synchroExtra, int up);

		/// <summary>
		/// 特殊能力フォーマット変更イベント
		/// </summary>
		event EventHandler OnExtraFormatChange;
		/// <summary>
		/// 特殊能力フォーマット
		/// </summary>
		string ExtraFormat { get; set; }
		/// <summary>
		/// シンクロ特殊能力フォーマット変更イベント
		/// </summary>
		event EventHandler OnSynchroExtraFormatChange;
		/// <summary>
		/// シンクロ特殊能力フォーマット
		/// </summary>
		string SynchroExtraFormat { get; set; }
		/// <summary>
		/// シンクロ特殊能力アップ分フォーマット変更イベント
		/// </summary>
		event EventHandler OnExtraUpFormatChange;
		/// <summary>
		/// シンクロ特殊能力アップ分フォーマット
		/// </summary>
		string ExtraUpFormat { get; set; }
		#endregion

		#region 合計シンクロボーナス
		/// <summary>
		/// 合計シンクロボーナス変更イベント
		/// </summary>
		event EventHandler OnTotalSynchroBonusChange;
		/// <summary>
		/// 合計シンクロボーナス
		/// </summary>
		int TotalSynchroBonus { get; set; }
		#endregion
	}

	/// <summary>
	/// シンクロ合成結果データ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnAvatarTypeChange = null;
			this.OnHitPointChange = null;
			this.OnHitPointFormatChange = null;
			this.OnSynchroHitPointFormatChange = null;
			this.OnHitPointUpFormatChange = null;
			this.OnAttackFormatChange = null;
			this.OnAttackFormatChange = null;
			this.OnSynchroAttackFormatChange = null;
			this.OnAttackUpFormatChange = null;
			this.OnDefenceChange = null;
			this.OnDefenceFormatChange = null;
			this.OnSynchroDefenceFormatChange = null;
			this.OnDefenceUpFormatChange = null;
			this.OnExtraChange = null;
			this.OnExtraFormatChange = null;
			this.OnSynchroExtraFormatChange = null;
			this.OnExtraUpFormatChange = null;
			this.OnTotalSynchroBonusChange = null;

		}
		#endregion
		
		#region アバタータイプ
		/// <summary>
		/// アバタータイプ変更イベント
		/// </summary>
		public event EventHandler OnAvatarTypeChange = (sender, e) => { };

		private AvatarType _avatarType = AvatarType.None;
		public AvatarType AvatarType
		{
			get { return _avatarType; }
			set
			{
				if (_avatarType != value)
				{
					_avatarType = value;

					// 通知
					this.OnAvatarTypeChange(this, EventArgs.Empty);
				}
			}
		}

        private int _skinId = 0;
        public int SkinId {
            get {
                return _skinId;
            }
            set {
                if (_skinId != value) {
                    _skinId = value;
                    // 通知
                    this.OnAvatarTypeChange(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region 生命力
        /// <summary>
        /// 生命力
        /// </summary>
        private int _hitPoint = 0;
		public int HitPoint{ get { return _hitPoint; } private set { _hitPoint = value; } }
		/// <summary>
		/// シンクロ生命力
		/// </summary>
		private int _synchroHitPoint = 0;
		public int SynchroHitPoint { get { return _synchroHitPoint; } private set { _synchroHitPoint = value; } }
		/// <summary>
		/// シンクロ生命力アップ分
		/// </summary>
		private int _hitPointUp = 0;
		public int HitPointUp { get { return _hitPointUp; } private set { _hitPointUp = value; } }

		/// <summary>
		/// 生命力変更イベント
		/// </summary>
		public event EventHandler OnHitPointChange = (sender, e) => { };
		/// <summary>
		/// 生命力設定
		/// </summary>
		public void SetHitPoint(int hp, int synchroHp, int up)
		{
			if (this.HitPoint != hp || this.SynchroHitPoint != synchroHp || this.HitPointUp != up)
			{
				this.HitPoint = hp;
				this.SynchroHitPoint = synchroHp;
				this.HitPointUp = up;

				// 通知
				this.OnHitPointChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 生命力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnHitPointFormatChange = (sender, e) => { };
		/// <summary>
		/// 生命力フォーマット
		/// </summary>
		private string _hitPointFormat = "";
		public string HitPointFormat
		{
			get { return _hitPointFormat; }
			set
			{
				if (_hitPointFormat != value)
				{
					_hitPointFormat = value;

					// 通知
					this.OnHitPointFormatChange(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// シンクロ生命力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSynchroHitPointFormatChange = (sender, e) => { };
		/// <summary>
		/// シンクロ生命力フォーマット
		/// </summary>
		private string _synchroHitPointFormat = "";
		public string SynchroHitPointFormat
		{
			get { return _synchroHitPointFormat; }
			set
			{
				if(_synchroHitPointFormat != value)
				{
					_synchroHitPointFormat = value;

					// 通知
					this.OnSynchroHitPointFormatChange(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// シンクロ生命力アップ分フォーマット変更イベント
		/// </summary>
		public event EventHandler OnHitPointUpFormatChange = (sender, e) => { };
		/// <summary>
		/// シンクロ生命力アップ分フォーマット
		/// </summary>
		private string _hitPointUpFormat = "";
		public string HitPointUpFormat
		{
			get { return _hitPointUpFormat; }
			set
			{
				if(_hitPointUpFormat != value)
				{
					_hitPointUpFormat = value;

					// 通知
					this.OnHitPointUpFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力
		/// </summary>
		private int _attack = 0;
		public int Attack { get { return _attack; } private set { _attack = value; } }
		/// <summary>
		/// シンクロ攻撃力
		/// </summary>
		private int _synchroAttack = 0;
		public int SynchroAttack { get { return _synchroAttack; } private set { _synchroAttack = value; } }
		/// <summary>
		/// シンクロ攻撃力アップ分
		/// </summary>
		private int _attackUp = 0;
		public int AttackUp { get { return _attackUp; } private set { _attackUp = value; } }

		/// <summary>
		/// 攻撃力変更イベント
		/// </summary>
		public event EventHandler OnAttackChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力設定
		/// </summary>
		public void SetAttack(int atk, int synchroAtk, int up)
		{
			if(this.Attack != atk || this.SynchroAttack != synchroAtk || this.AttackUp != up)
			{
				this.Attack = atk;
				this.SynchroAttack = synchroAtk;
				this.AttackUp = up;

				// 通知
				this.OnAttackChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 攻撃力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnAttackFormatChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力フォーマット
		/// </summary>
		private string _attackFormat = "";
		public string AttackFormat
		{
			get { return _attackFormat; }
			set
			{
				if (_attackFormat != value)
				{
					_attackFormat = value;

					// 通知
					this.OnAttackFormatChange(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// シンクロ攻撃力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSynchroAttackFormatChange = (sender, e) => { };
		/// <summary>
		/// シンクロ攻撃力フォーマット
		/// </summary>
		private string _synchroAttackFormat = "";
		public string SynchroAttackFormat
		{
			get { return _synchroAttackFormat; }
			set
			{
				if(_synchroAttackFormat != value)
				{
					_synchroAttackFormat = value;

					// 通知
					this.OnSynchroAttackFormatChange(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// シンクロ攻撃力アップ分フォーマット変更イベント
		/// </summary>
		public event EventHandler OnAttackUpFormatChange = (sender, e) => { };
		/// <summary>
		/// シンクロ攻撃力アップ分フォーマット
		/// </summary>
		private string _attackUpFormat = "";
		public string AttackUpFormat
		{
			get { return _attackUpFormat; }
			set
			{
				if(_attackUpFormat != value)
				{
					_attackUpFormat = value;

					// 通知
					this.OnAttackUpFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力
		/// </summary>
		private int _defence = 0;
		public int Defence { get { return _defence; } private set { _defence = value; } }
		/// <summary>
		/// シンクロ防御力
		/// </summary>
		private int _synchroDefence = 0;
		public int SynchroDefence { get { return _synchroDefence; } private set { _synchroDefence = value; } }
		/// <summary>
		/// シンクロ防御力アップ分
		/// </summary>
		private int _defenceUp = 0;
		public int DefenceUp { get { return _defenceUp; } private set { _defenceUp = value; } }

		/// <summary>
		/// 防御力変更イベント
		/// </summary>
		public event EventHandler OnDefenceChange = (sender, e) => { };
		/// <summary>
		/// 防御力設定
		/// </summary>
		public void SetDefence(int def, int synchroDef, int up)
		{
			if(this.Defence != def || this.SynchroDefence != synchroDef || this.DefenceUp != up)
			{
				this.Defence = def;
				this.SynchroDefence = synchroDef;
				this.DefenceUp = up;

				// 通知
				this.OnDefenceChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 防御力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnDefenceFormatChange = (sender, e) => { };
		/// <summary>
		/// 防御力フォーマット
		/// </summary>
		private string _defenceFormat = "";
		public string DefenceFormat
		{
			get { return _defenceFormat; }
			set
			{
				if (_defenceFormat != value)
				{
					_defenceFormat = value;

					// 通知
					this.OnDefenceFormatChange(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// シンクロ防御力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSynchroDefenceFormatChange = (sender, e) => { };
		/// <summary>
		/// シンクロ防御力フォーマット
		/// </summary>
		private string _synchroDefenceFormat = "";
		public string SynchroDefenceFormat
		{
			get { return _synchroDefenceFormat; }
			set
			{
				if(_synchroDefenceFormat != value)
				{
					_synchroDefenceFormat = value;

					// 通知
					this.OnSynchroDefenceFormatChange(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// シンクロ防御力アップ分フォーマット変更イベント
		/// </summary>
		public event EventHandler OnDefenceUpFormatChange = (sender, e) => { };
		/// <summary>
		/// シンクロ防御力アップ分フォーマット
		/// </summary>
		private string _defenceUpFormat = "";
		public string DefenceUpFormat
		{
			get { return _defenceUpFormat; }
			set
			{
				if(_defenceUpFormat != value)
				{
					_defenceUpFormat = value;

					// 通知
					this.OnDefenceUpFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力
		/// </summary>
		private int _extra = 0;
		public int Extra { get { return _extra; } private set { _extra = value; } }
		/// <summary>
		/// シンクロ特殊能力
		/// </summary>
		private int _synchroExtra = 0;
		public int SynchroExtra { get { return _synchroExtra; } private set { _synchroExtra = value; } }
		/// <summary>
		/// シンクロ特殊能力アップ分
		/// </summary>
		private int _extraUp = 0;
		public int ExtraUp { get { return _extraUp; } private set { _extraUp = value; } }

		/// <summary>
		/// 特殊能力変更イベント
		/// </summary>
		public event EventHandler OnExtraChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力設定
		/// </summary>
		public void SetExtra(int extra, int synchroExtra, int up)
		{
			if(this.Extra != extra || this.SynchroExtra != synchroExtra || this.ExtraUp != up)
			{
				this.Extra = extra;
				this.SynchroExtra = synchroExtra;
				this.ExtraUp = up;

				// 通知
				this.OnExtraChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 特殊能力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnExtraFormatChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力フォーマット
		/// </summary>
		private string _extraFormat = "";
		public string ExtraFormat
		{
			get { return _extraFormat; }
			set
			{
				if (_extraFormat != value)
				{
					_extraFormat = value;

					// 通知
					this.OnExtraFormatChange(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// シンクロ特殊能力フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSynchroExtraFormatChange = (sender, e) => { };
		/// <summary>
		/// シンクロ特殊能力フォーマット
		/// </summary>
		private string _synchroExtraFormat = "";
		public string SynchroExtraFormat
		{
			get { return _synchroExtraFormat; }
			set
			{
				if(_synchroExtraFormat != value)
				{
					_synchroExtraFormat = value;

					// 通知
					this.OnSynchroExtraFormatChange(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// 特殊能力アップ分フォーマット変更イベント
		/// </summary>
		public event EventHandler OnExtraUpFormatChange = (sender, e) => { };
		private string _extraUpFormat = "";
		public string ExtraUpFormat
		{
			get { return _extraUpFormat; }
			set
			{
				if(_extraUpFormat != value)
				{
					_extraUpFormat = value;

					// 通知
					this.OnExtraUpFormatChange(this, EventArgs.Empty);
				}

			}
		}
		#endregion

		#region 合計シンクロボーナス
		/// <summary>
		/// 合計シンクロボーナス変更イベント
		/// </summary>
		public event EventHandler OnTotalSynchroBonusChange = (sender, e) => { };
		/// <summary>
		/// 合計シンクロボーナス
		/// </summary>
		private int _totalSynchroBonus = 0;
		public int TotalSynchroBonus
		{
			get { return _totalSynchroBonus; }
			set
			{
				if(_totalSynchroBonus != value)
				{
					_totalSynchroBonus = value;

					// 通知
					this.OnTotalSynchroBonusChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion
	}
}