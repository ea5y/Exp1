/// <summary>
/// 合成メッセージデータ
/// 
/// 2016/05/10
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI.FusionMessage
{
	/// <summary>
	/// 合成メッセージデータインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region メッセージ
		/// <summary>
		/// メッセージ変更イベント
		/// </summary>
		event EventHandler OnMessageChange;
		/// <summary>
		/// メッセージ
		/// </summary>
		string Message { get; set; }
		#endregion

		#region ランク
		/// <summary>
		/// ランク変更イベント
		/// </summary>
		event EventHandler OnRankChange;
		/// <summary>
		/// ランク変化前
		/// </summary>
		int RankBefore { get; }
		/// <summary>
		/// ランク変化後
		/// </summary>
		int RankAfter { get; }
		/// <summary>
		/// ランク設定
		/// </summary>
		void SetRank(int before, int after);
		#endregion

		#region レベル
		/// <summary>
		/// レベル変更イベント
		/// </summary>
		event EventHandler OnLevelChange;
		/// <summary>
		/// レベル変化前
		/// </summary>
		int LevelBefore { get; }
		/// <summary>
		/// レベル変化後
		/// </summary>
		int LevelAfter { get; }
		/// <summary>
		/// レベル設定
		/// </summary>
		void SetLevel(int before, int after);
		#endregion

		#region 経験値
		/// <summary>
		/// 経験値変更イベント
		/// </summary>
		event EventHandler OnExpChange;
		/// <summary>
		/// 現在の累積経験値
		/// </summary>
		int Exp { get; }
		/// <summary>
		/// 現在のレベルになる為の累積経験値
		/// </summary>
		int TotalExp { get; }
		/// <summary>
		/// 次のレベルになる為の累積経験値
		/// </summary>
		int NextLvTotalExp { get; }
		/// <summary>
		/// 経験値設定
		/// </summary>
		void SetExp(int exp, int total, int nextLvTotalExp);
		/// <summary>
		/// 経験値バーの値を取得する
		/// </summary>
		float GetExpSlider();
		#endregion

		#region シンクロ可能回数
		/// <summary>
		/// シンクロ可能回数変更イベント
		/// </summary>
		event EventHandler OnSynchroRemainChange;
		/// <summary>
		/// シンクロ可能回数変化前
		/// </summary>
		int SynchroRemainBefore { get; }
		/// <summary>
		/// シンクロ可能回数変化後
		/// </summary>
		int SynchroRemainAfter { get; }
		/// <summary>
		/// シンクロ可能回数設定
		/// </summary>
		void SetSynchroRemain(int before, int after);
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力
		/// </summary>
		event EventHandler OnHitPointChange;
		/// <summary>
		/// 生命力変化前
		/// </summary>
		int HitPointBefore { get; }
		/// <summary>
		/// 生命力変化後
		/// </summary>
		int HitPointAfter { get; }
		/// <summary>
		/// 生命力設定
		/// </summary>
		void SetHitPoint(int before, int after);

		/// <summary>
		/// 生命力ベース変更イベント
		/// </summary>
		event EventHandler OnHitPointBaseChange;
		/// <summary>
		/// 生命力ベース変更前
		/// </summary>
		int HitPointBaseBefore { get; }
		/// <summary>
		/// 生命力ベース変更後
		/// </summary>
		int HitPointBaseAfter { get; }
		/// <summary>
		/// 生命力ベース設定
		/// </summary>
		void SetHitPointBase(int before, int after);

		/// <summary>
		/// シンクロ生命力変更イベント
		/// </summary>
		event EventHandler OnSynchroHitPointChange;
		/// <summary>
		/// シンクロ生命力
		/// </summary>
		int SynchroHitPoint { get; set; }

		/// <summary>
		/// スロット生命力変更イベント
		/// </summary>
		event EventHandler OnSlotHitPointChange;
		/// <summary>
		/// スロット生命力変化前
		/// </summary>
		int SlotHitPointBefore { get; }
		/// <summary>
		/// スロット生命力変化後
		/// </summary>
		int SlotHitPointAfter { get; }
		/// <summary>
		/// スロット生命力設定
		/// </summary>
		void SetSlotHitPoint(int before, int after);

		/// <summary>
		/// 生命力増加分変化イベント
		/// </summary>
		event EventHandler OnHitPointUpChange;
		/// <summary>
		/// 生命力増加分
		/// </summary>
		int HitPointUp { get; set;}
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力
		/// </summary>
		event EventHandler OnAttackChange;
		/// <summary>
		/// 攻撃力変化前
		/// </summary>
		int AttackBefore { get; }
		/// <summary>
		/// 攻撃力変化後
		/// </summary>
		int AttackAfter { get; }
		/// <summary>
		/// 攻撃力設定
		/// </summary>
		void SetAttack(int before, int after);

		/// <summary>
		/// 攻撃力ベース変更イベント
		/// </summary>
		event EventHandler OnAttackBaseChange;
		/// <summary>
		/// 攻撃力ベース変更前
		/// </summary>
		int AttackBaseBefore { get; }
		/// <summary>
		/// 攻撃力ベース変更後
		/// </summary>
		int AttackBaseAfter { get; }
		/// <summary>
		/// 攻撃力ベース設定
		/// </summary>
		void SetAttackBase(int before, int after);

		/// <summary>
		/// シンクロ攻撃力変更イベント
		/// </summary>
		event EventHandler OnSynchroAttackChange;
		/// <summary>
		/// シンクロ攻撃力
		/// </summary>
		int SynchroAttack { get; set; }

		/// <summary>
		/// スロット攻撃力変更イベント
		/// </summary>
		event EventHandler OnSlotAttackChange;
		/// <summary>
		/// スロット攻撃力変化前
		/// </summary>
		int SlotAttackBefore { get; }
		/// <summary>
		/// スロット攻撃力変化後
		/// </summary>
		int SlotAttackAfter { get; }
		/// <summary>
		/// スロット攻撃力設定
		/// </summary>
		void SetSlotAttack(int before, int after);

		/// <summary>
		/// 攻撃力増加分変化イベント
		/// </summary>
		event EventHandler OnAttackUpChange;
		/// <summary>
		/// 攻撃力増加分
		/// </summary>
		int AttackUp { get; set; }
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力
		/// </summary>
		event EventHandler OnDefenseChange;
		/// <summary>
		/// 防御力変化前
		/// </summary>
		int DefenseBefore { get; }
		/// <summary>
		/// 防御力変化後
		/// </summary>
		int DefenseAfter { get; }
		/// <summary>
		/// 防御力設定
		/// </summary>
		void SetDefense(int before, int after);

		/// <summary>
		/// 防御力ベース変更イベント
		/// </summary>
		event EventHandler OnDefenseBaseChange;
		/// <summary>
		/// 防御力ベース変更前
		/// </summary>
		int DefenseBaseBefore { get; }
		/// <summary>
		/// 防御力ベース変更後
		/// </summary>
		int DefenseBaseAfter { get; }
		/// <summary>
		/// 防御力ベース設定
		/// </summary>
		void SetDefenseBase(int before, int after);

		/// <summary>
		/// シンクロ防御力変更イベント
		/// </summary>
		event EventHandler OnSynchroDefenseChange;
		/// <summary>
		/// シンクロ防御力
		/// </summary>
		int SynchroDefense { get; set; }

		/// <summary>
		/// スロット防御力変更イベント
		/// </summary>
		event EventHandler OnSlotDefenseChange;
		/// <summary>
		/// スロット防御力変化前
		/// </summary>
		int SlotDefenseBefore { get; }
		/// <summary>
		/// スロット防御力変化後
		/// </summary>
		int SlotDefenseAfter { get; }
		/// <summary>
		/// スロット防御力設定
		/// </summary>
		void SetSlotDefense(int before, int after);

		/// <summary>
		/// 防御力増加分変化イベント
		/// </summary>
		event EventHandler OnDefenseUpChange;
		/// <summary>
		/// 防御力増加分
		/// </summary>
		int DefenseUp { get; set; }
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力
		/// </summary>
		event EventHandler OnExtraChange;
		/// <summary>
		/// 特殊能力変化前
		/// </summary>
		int ExtraBefore { get; }
		/// <summary>
		/// 特殊能力変化後
		/// </summary>
		int ExtraAfter { get; }
		/// <summary>
		/// 特殊能力設定
		/// </summary>
		void SetExtra(int before, int after);

		/// <summary>
		/// 特殊能力ベース変更イベント
		/// </summary>
		event EventHandler OnExtraBaseChange;
		/// <summary>
		/// 特殊能力ベース変更前
		/// </summary>
		int ExtraBaseBefore { get; }
		/// <summary>
		/// 特殊能力ベース変更後
		/// </summary>
		int ExtraBaseAfter { get; }
		/// <summary>
		/// 特殊能力ベース設定
		/// </summary>
		void SetExtraBase(int before, int after);

		/// <summary>
		/// シンクロ特殊能力変更イベント
		/// </summary>
		event EventHandler OnSynchroExtraChange;
		/// <summary>
		/// シンクロ特殊能力
		/// </summary>
		int SynchroExtra { get; set; }

		/// <summary>
		/// スロット特殊能力変更イベント
		/// </summary>
		event EventHandler OnSlotExtraChange;
		/// <summary>
		/// スロット特殊能力変化前
		/// </summary>
		int SlotExtraBefore { get; }
		/// <summary>
		/// スロット特殊能力変化後
		/// </summary>
		int SlotExtraAfter { get; }
		/// <summary>
		/// スロット特殊能力設定
		/// </summary>
		void SetSlotExtra(int before, int after);

		/// <summary>
		/// 特殊能力増加分変化イベント
		/// </summary>
		event EventHandler OnExtraUpChange;
		/// <summary>
		/// 特殊能力増加分
		/// </summary>
		int ExtraUp { get; set; }
		#endregion

		#region シンクロ合成かどうか
		/// <summary>
		/// シンクロ合成フラグの変化イベント
		/// </summary>
		event EventHandler OnIsSynchroFusionChange;
		/// <summary>
		/// シンクロ合成かどうか
		/// </summary>
		bool IsSynchroFusion { get; set; }
		#endregion

		#region フォーマット
		/// <summary>
		/// 合計ステータス値のフォーマット変更イベント
		/// </summary>
		event EventHandler OnTotalStatusFormatChange;
		/// <summary>
		/// 合計ステータス値のフォーマット
		/// </summary>
		string TotalStatusFormat { get; set; }

		/// <summary>
		/// ベースステータス値のフォーマット変更イベント
		/// </summary>
		event EventHandler OnBaseStatusFormatChange;
		/// <summary>
		/// ベースステータス値のフォーマット
		/// </summary>
		string BaseStatusFormat { get; set; }

		/// <summary>
		/// シンクロ値フォーマット変更イベント
		/// </summary>
		event EventHandler OnSynchroFormatChange;
		/// <summary>
		/// シンクロ値フォーマット
		/// </summary>
		string SynchroFormat { get; set; }

		/// <summary>
		/// スロット値フォーマット変更イベント
		/// </summary>
		event EventHandler OnSlotFormatChange;
		/// <summary>
		/// スロット値フォーマット
		/// </summary>
		string SlotFormat { get; set; }

		/// <summary>
		/// 増加値フォーマット変更イベント
		/// </summary>
		event EventHandler OnUpFormatChange;
		/// <summary>
		/// 増加値フォーマット
		/// </summary>
		string UpFormat { get; set; }

		/// <summary>
		/// 未確定フォーマット変更イベント
		/// </summary>
		event EventHandler OnPendingFormatChange;
		/// <summary>
		/// 未確定フォーマット
		/// </summary>
		string PendingFormat { get; set; }
		#endregion
	}

	/// <summary>
	/// 合成メッセージデータ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnMessageChange = null;

			this.OnRankChange = null;
			this.OnLevelChange = null;
			this.OnExpChange = null;
			this.OnSynchroRemainChange = null;

			this.OnHitPointChange = null;
			this.OnHitPointBaseChange = null;
			this.OnSynchroHitPointChange = null;
			this.OnSlotHitPointChange = null;
			this.OnHitPointUpChange = null;

			this.OnAttackChange = null;
			this.OnAttackBaseChange = null;
			this.OnSynchroAttackChange = null;
			this.OnSlotAttackChange = null;
			this.OnAttackUpChange = null;

			this.OnDefenseChange = null;
			this.OnDefenseBaseChange = null;
			this.OnSynchroDefenseChange = null;
			this.OnSlotDefenseChange = null;
			this.OnDefenseUpChange = null;

			this.OnExtraChange = null;
			this.OnExtraBaseChange = null;
			this.OnSynchroExtraChange = null;
			this.OnSlotExtraChange = null;
			this.OnExtraUpChange = null;

			this.OnIsSynchroFusionChange = null;

			this.OnTotalStatusFormatChange = null;
			this.OnSynchroFormatChange = null;
			this.OnSlotFormatChange = null;
			this.OnUpFormatChange = null;
			this.OnPendingFormatChange = null;
		}
		#endregion

		#region メッセージ
		/// <summary>
		/// メッセージ変更イベント
		/// </summary>
		public event EventHandler OnMessageChange = (sender, e) => { };
		/// <summary>
		/// メッセージ
		/// </summary>
		private string _message = string.Empty;
		public string Message
		{
			get { return _message; }
			set
			{
				if(_message != value)
				{
					_message = value;

					// 通知
					this.OnMessageChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region ランク
		/// <summary>
		/// ランク変更イベント
		/// </summary>
		public event EventHandler OnRankChange = (sender, e) => { };
		/// <summary>
		/// ランク変化前
		/// </summary>
		private int _rankBefore = 0;
		public int RankBefore { get { return _rankBefore; } private set { _rankBefore = value; } }
		/// <summary>
		/// ランク変化後
		/// </summary>
		private int _rankAfter = 0;
		public int RankAfter { get { return _rankAfter; } private set { _rankAfter = value; } }
		/// <summary>
		/// ランク設定
		/// </summary>
		public void SetRank(int before, int after)
		{
			if(this.RankBefore != before || this.RankAfter != after)
			{
				this.RankBefore = before;
				this.RankAfter = after;

				// 通知
				this.OnRankChange(this, EventArgs.Empty);
			}
		}
		#endregion

		#region レベル
		/// <summary>
		/// レベル変更イベント
		/// </summary>
		public event EventHandler OnLevelChange = (sender, e) => { };
		/// <summary>
		/// レベル変化前
		/// </summary>
		private int _levelBefore = 0;
		public int LevelBefore { get { return _levelBefore; } private set { _levelBefore = value; } }
		/// <summary>
		/// レベル変化後
		/// </summary>
		private int _levelAfter = 0;
		public int LevelAfter { get { return _levelAfter; } private set { _levelAfter = value; } }
		/// <summary>
		/// レベル設定
		/// </summary>
		public void SetLevel(int before, int after)
		{
			if(this.LevelBefore != before || this.LevelAfter != after)
			{
				this.LevelBefore = before;
				this.LevelAfter = after;

				// 通知
				this.OnLevelChange(this, EventArgs.Empty);
			}
		}
		#endregion

		#region 経験値
		/// <summary>
		/// 経験値変更イベント
		/// </summary>
		public event EventHandler OnExpChange = (sender, e) => { };
		/// <summary>
		/// 現在の累積経験値
		/// </summary>
		private int _exp = 0;
		public int Exp { get { return _exp; } private set { _exp = value; } }
		/// <summary>
		/// 現在のレベルになる為の累積経験値
		/// </summary>
		private int _totalExp = 0;
		public int TotalExp { get { return _totalExp; } private set { _totalExp = value; } }
		/// <summary>
		/// 次のレベルになる為の累積経験値
		/// </summary>
		private int _nextLvTotalExp = 0;
		public int NextLvTotalExp { get { return _nextLvTotalExp; } private set { _nextLvTotalExp = value; } }
		/// <summary>
		/// 経験値設定
		/// </summary>
		public void SetExp(int exp, int total, int nextLvTotalExp)
		{
			if(this.Exp != exp || this.TotalExp != total || this.NextLvTotalExp != nextLvTotalExp)
			{
				this.Exp = exp;
				this.TotalExp = total;
				this.NextLvTotalExp = nextLvTotalExp;

				// 通知
				this.OnExpChange(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// 経験値バーの値を取得する
		/// </summary>
		public float GetExpSlider()
		{
			return XUI.Powerup.Util.GetLerpValue((float)this.TotalExp, (float)this.NextLvTotalExp, (float)this.Exp);
		}
		#endregion

		#region シンクロ可能回数
		/// <summary>
		/// シンクロ可能回数変更イベント
		/// </summary>
		public event EventHandler OnSynchroRemainChange = (sender, e) => { };
		/// <summary>
		/// シンクロ可能回数変化前
		/// </summary>
		private int _synchroRemainBefore = 0;
		public int SynchroRemainBefore { get { return _synchroRemainBefore; } private set { _synchroRemainBefore = value; } }
		/// <summary>
		/// シンクロ可能回数変化後
		/// </summary>
		private int _synchroRemainAfter = 0;
		public int SynchroRemainAfter { get { return _synchroRemainAfter; } private set { _synchroRemainAfter = value; } }
		/// <summary>
		/// シンクロ可能回数設定
		/// </summary>
		public void SetSynchroRemain(int before, int after)
		{
			if (this.SynchroRemainBefore != before || this.SynchroRemainAfter != after)
			{
				this.SynchroRemainBefore = before;
				this.SynchroRemainAfter = after;

				// 通知
				this.OnSynchroRemainChange(this, EventArgs.Empty);
			}
		}
		#endregion

		#region 生命力
		/// <summary>
		/// 生命力変更イベント
		/// </summary>
		public event EventHandler OnHitPointChange = (sender, e) => { };
		/// <summary>
		/// 生命力変化前
		/// </summary>
		private int _hitPointBefore = 0;
		public int HitPointBefore { get { return _hitPointBefore; } private set { _hitPointBefore = value; } }
		/// <summary>
		/// 生命力変化後
		/// </summary>
		private int _hitPointAfter = 0;
		public int HitPointAfter { get { return _hitPointAfter; } private set { _hitPointAfter = value; } }
		/// <summary>
		/// 生命力設定
		/// </summary>
		public void SetHitPoint(int before, int after)
		{
			if(this.HitPointBefore != before || this.HitPointAfter != after)
			{
				this.HitPointBefore = before;
				this.HitPointAfter = after;

				// 通知
				this.OnHitPointChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 生命力ベース変更イベント
		/// </summary>
		public event EventHandler OnHitPointBaseChange = (sender, e) => { };
		/// <summary>
		/// 生命力ベース変更前
		/// </summary>
		private int _hitPointBaseBefore = 0;
		public int HitPointBaseBefore { get { return _hitPointBaseBefore; } private set { _hitPointBaseBefore = value; } }
		/// <summary>
		/// 生命力ベース変更後
		/// </summary>
		private int _hitPointBaseAfter = 0;
		public int HitPointBaseAfter { get { return _hitPointBaseAfter; } private set { _hitPointBaseAfter = value; } }
		/// <summary>
		/// 生命力ベース設定
		/// </summary>
		public void SetHitPointBase(int before, int after)
		{
			if(this.HitPointBaseBefore != before || this.HitPointBaseAfter != after)
			{
				this.HitPointBaseBefore = before;
				this.HitPointBaseAfter = after;

				// 通知
				this.OnHitPointBaseChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// シンクロ生命力変更イベント
		/// </summary>
		public event EventHandler OnSynchroHitPointChange = (sender, e) => { };
		/// <summary>
		/// シンクロ生命力
		/// </summary>
		private int _synchroHitPoint = 0;
		public int SynchroHitPoint
		{
			get { return _synchroHitPoint; }
			set
			{
				if (_synchroHitPoint != value)
				{
					_synchroHitPoint = value;

					// 通知
					this.OnSynchroHitPointChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// スロット生命力変更イベント
		/// </summary>
		public event EventHandler OnSlotHitPointChange = (sender, e) => { };
		/// <summary>
		/// スロット生命力変化前
		/// </summary>
		private int _slotHitPointBefore = 0;
		public int SlotHitPointBefore { get { return _slotHitPointBefore; } private set { _slotHitPointBefore = value; } }
		/// <summary>
		/// スロット生命力変化後
		/// </summary>
		private int _slotHitPointAfter = 0;
		public int SlotHitPointAfter { get { return _slotHitPointAfter; } private set { _slotHitPointAfter = value; } }
		/// <summary>
		/// スロット生命力設定
		/// </summary>
		public void SetSlotHitPoint(int before, int after)
		{
			if(this.SlotHitPointBefore != before || this.SlotHitPointAfter != after)
			{
				this.SlotHitPointBefore = before;
				this.SlotHitPointAfter = after;

				// 通知
				this.OnSlotHitPointChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 生命力増加分変化イベント
		/// </summary>
		public event EventHandler OnHitPointUpChange = (sender, e) => { };
		/// <summary>
		/// 生命力増加分
		/// </summary>
		private int _hitPointUp = 0;
		public int HitPointUp
		{
			get { return _hitPointUp; }
			set
			{
				if(this._hitPointUp != value)
				{
					this._hitPointUp = value;

					// 通知
					this.OnHitPointUpChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力変更イベント
		/// </summary>
		public event EventHandler OnAttackChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力変化前
		/// </summary>
		private int _attackBefore = 0;
		public int AttackBefore { get { return _attackBefore; } private set { _attackBefore = value; } }
		/// <summary>
		/// 攻撃力変化後
		/// </summary>
		private int _attackAfter = 0;
		public int AttackAfter { get { return _attackAfter; } private set { _attackAfter = value; } }
		/// <summary>
		/// 攻撃力設定
		/// </summary>
		public void SetAttack(int before, int after)
		{
			if (this.AttackBefore != before || this.AttackAfter != after)
			{
				this.AttackBefore = before;
				this.AttackAfter = after;

				// 通知
				this.OnAttackChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 攻撃力ベース変更イベント
		/// </summary>
		public event EventHandler OnAttackBaseChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力ベース変更前
		/// </summary>
		private int _attackBaseBefore = 0;
		public int AttackBaseBefore { get { return _attackBaseBefore; } private set { _attackBaseBefore = value; } }
		/// <summary>
		/// 攻撃力ベース変更後
		/// </summary>
		private int _attackBaseAfter = 0;
		public int AttackBaseAfter { get { return _attackBaseAfter; } private set { _attackBaseAfter = value; } }
		/// <summary>
		/// 攻撃力ベース設定
		/// </summary>
		public void SetAttackBase(int before, int after)
		{
			if (this.AttackBaseBefore != before || this.AttackBaseAfter != after)
			{
				this.AttackBaseBefore = before;
				this.AttackBaseAfter = after;

				// 通知
				this.OnAttackBaseChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// シンクロ攻撃力変更イベント
		/// </summary>
		public event EventHandler OnSynchroAttackChange = (sender, e) => { };
		/// <summary>
		/// シンクロ攻撃力
		/// </summary>
		private int _synchroAttack = 0;
		public int SynchroAttack
		{
			get { return _synchroAttack; }
			set
			{
				if (_synchroAttack != value)
				{
					_synchroAttack = value;

					// 通知
					this.OnSynchroAttackChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// スロット攻撃力変更イベント
		/// </summary>
		public event EventHandler OnSlotAttackChange = (sender, e) => { };
		/// <summary>
		/// スロット攻撃力変化前
		/// </summary>
		private int _slotAttackBefore = 0;
		public int SlotAttackBefore { get { return _slotAttackBefore; } private set { _slotAttackBefore = value; } }
		/// <summary>
		/// スロット攻撃力変化後
		/// </summary>
		private int _slotAttackAfter = 0;
		public int SlotAttackAfter { get { return _slotAttackAfter; } private set { _slotAttackAfter = value; } }
		/// <summary>
		/// スロット攻撃力設定
		/// </summary>
		public void SetSlotAttack(int before, int after)
		{
			if (this.SlotAttackBefore != before || this.SlotAttackAfter != after)
			{
				this.SlotAttackBefore = before;
				this.SlotAttackAfter = after;

				// 通知
				this.OnSlotAttackChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 攻撃力増加分変化イベント
		/// </summary>
		public event EventHandler OnAttackUpChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力増加分
		/// </summary>
		private int _attackUp = 0;
		public int AttackUp
		{
			get { return _attackUp; }
			set
			{
				if (this._attackUp != value)
				{
					this._attackUp = value;

					// 通知
					this.OnAttackUpChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力変更イベント
		/// </summary>
		public event EventHandler OnDefenseChange = (sender, e) => { };
		/// <summary>
		/// 防御力変化前
		/// </summary>
		private int _defenseBefore = 0;
		public int DefenseBefore { get { return _defenseBefore; } private set { _defenseBefore = value; } }
		/// <summary>
		/// 防御力変化後
		/// </summary>
		private int _defenseAfter = 0;
		public int DefenseAfter { get { return _defenseAfter; } private set { _defenseAfter = value; } }
		/// <summary>
		/// 防御力設定
		/// </summary>
		public void SetDefense(int before, int after)
		{
			if (this.DefenseBefore != before || this.DefenseAfter != after)
			{
				this.DefenseBefore = before;
				this.DefenseAfter = after;

				// 通知
				this.OnDefenseChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 防御力ベース変更イベント
		/// </summary>
		public event EventHandler OnDefenseBaseChange = (sender, e) => { };
		/// <summary>
		/// 防御力ベース変更前
		/// </summary>
		private int _defenseBaseBefore = 0;
		public int DefenseBaseBefore { get { return _defenseBaseBefore; } private set { _defenseBaseBefore = value; } }
		/// <summary>
		/// 防御力ベース変更後
		/// </summary>
		private int _defenseBaseAfter = 0;
		public int DefenseBaseAfter { get { return _defenseBaseAfter; } private set { _defenseBaseAfter = value; } }
		/// <summary>
		/// 防御力ベース設定
		/// </summary>
		public void SetDefenseBase(int before, int after)
		{
			if (this.DefenseBaseBefore != before || this.DefenseBaseAfter != after)
			{
				this.DefenseBaseBefore = before;
				this.DefenseBaseAfter = after;

				// 通知
				this.OnDefenseBaseChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// シンクロ防御力変更イベント
		/// </summary>
		public event EventHandler OnSynchroDefenseChange = (sender, e) => { };
		/// <summary>
		/// シンクロ防御力
		/// </summary>
		private int _synchroDefense = 0;
		public int SynchroDefense
		{
			get { return _synchroDefense; }
			set
			{
				if (_synchroDefense != value)
				{
					_synchroDefense = value;

					// 通知
					this.OnSynchroDefenseChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// スロット防御力変更イベント
		/// </summary>
		public event EventHandler OnSlotDefenseChange = (sender, e) => { };
		/// <summary>
		/// スロット防御力変化前
		/// </summary>
		private int _slotDefenseBefore = 0;
		public int SlotDefenseBefore { get { return _slotDefenseBefore; } private set { _slotDefenseBefore = value; } }
		/// <summary>
		/// スロット防御力変化後
		/// </summary>
		private int _slotDefenseAfter = 0;
		public int SlotDefenseAfter { get { return _slotDefenseAfter; } private set { _slotDefenseAfter = value; } }
		/// <summary>
		/// スロット防御力設定
		/// </summary>
		public void SetSlotDefense(int before, int after)
		{
			if (this.SlotDefenseBefore != before || this.SlotDefenseAfter != after)
			{
				this.SlotDefenseBefore = before;
				this.SlotDefenseAfter = after;

				// 通知
				this.OnSlotDefenseChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 防御力増加分変化イベント
		/// </summary>
		public event EventHandler OnDefenseUpChange = (sender, e) => { };
		/// <summary>
		/// 防御力増加分
		/// </summary>
		private int _defenseUp = 0;
		public int DefenseUp
		{
			get { return _defenseUp; }
			set
			{
				if (this._defenseUp != value)
				{
					this._defenseUp = value;

					// 通知
					this.OnDefenseUpChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力変更イベント
		/// </summary>
		public event EventHandler OnExtraChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力変化前
		/// </summary>
		private int _extraBefore = 0;
		public int ExtraBefore { get { return _extraBefore; } private set { _extraBefore = value; } }
		/// <summary>
		/// 特殊能力変化後
		/// </summary>
		private int _extraAfter = 0;
		public int ExtraAfter { get { return _extraAfter; } private set { _extraAfter = value; } }
		/// <summary>
		/// 特殊能力設定
		/// </summary>
		public void SetExtra(int before, int after)
		{
			if (this.ExtraBefore != before || this.ExtraAfter != after)
			{
				this.ExtraBefore = before;
				this.ExtraAfter = after;

				// 通知
				this.OnExtraChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 特殊能力ベース変更イベント
		/// </summary>
		public event EventHandler OnExtraBaseChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力ベース変更前
		/// </summary>
		private int _extraBaseBefore = 0;
		public int ExtraBaseBefore { get { return _extraBaseBefore; } private set { _extraBaseBefore = value; } }
		/// <summary>
		/// 特殊能力ベース変更後
		/// </summary>
		private int _extraBaseAfter = 0;
		public int ExtraBaseAfter { get { return _extraBaseAfter; } private set { _extraBaseAfter = value; } }
		/// <summary>
		/// 特殊能力ベース設定
		/// </summary>
		public void SetExtraBase(int before, int after)
		{
			if (this.ExtraBaseBefore != before || this.ExtraBaseAfter != after)
			{
				this.ExtraBaseBefore = before;
				this.ExtraBaseAfter = after;

				// 通知
				this.OnExtraBaseChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// シンクロ特殊能力変更イベント
		/// </summary>
		public event EventHandler OnSynchroExtraChange = (sender, e) => { };
		/// <summary>
		/// シンクロ特殊能力
		/// </summary>
		private int _synchroExtra = 0;
		public int SynchroExtra
		{
			get { return _synchroExtra; }
			set
			{
				if (_synchroExtra != value)
				{
					_synchroExtra = value;

					// 通知
					this.OnSynchroExtraChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// スロット特殊能力変更イベント
		/// </summary>
		public event EventHandler OnSlotExtraChange = (sender, e) => { };
		/// <summary>
		/// スロット特殊能力変化前
		/// </summary>
		private int _slotExtraBefore = 0;
		public int SlotExtraBefore { get { return _slotExtraBefore; } private set { _slotExtraBefore = value; } }
		/// <summary>
		/// スロット特殊能力変化後
		/// </summary>
		private int _slotExtraAfter = 0;
		public int SlotExtraAfter { get { return _slotExtraAfter; } private set { _slotExtraAfter = value; } }
		/// <summary>
		/// スロット特殊能力設定
		/// </summary>
		public void SetSlotExtra(int before, int after)
		{
			if (this.SlotExtraBefore != before || this.SlotExtraAfter != after)
			{
				this.SlotExtraBefore = before;
				this.SlotExtraAfter = after;

				// 通知
				this.OnSlotExtraChange(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 特殊能力増加分変化イベント
		/// </summary>
		public event EventHandler OnExtraUpChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力増加分
		/// </summary>
		private int _extraUp = 0;
		public int ExtraUp
		{
			get { return _extraUp; }
			set
			{
				if (this._extraUp != value)
				{
					this._extraUp = value;

					// 通知
					this.OnExtraUpChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region シンクロ合成かどうか
		/// <summary>
		/// シンクロ合成フラグの変化イベント
		/// </summary>
		public event EventHandler OnIsSynchroFusionChange = (sender, e) => { };
		/// <summary>
		/// シンクロ合成かどうか
		/// </summary>
		private bool _isSynchroFusion = false;
		public bool IsSynchroFusion
		{
			get { return _isSynchroFusion; }
			set
			{
				if(_isSynchroFusion != value)
				{
					_isSynchroFusion = value;

					// 通知
					this.OnIsSynchroFusionChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region フォーマット
		/// <summary>
		/// 合計ステータス値のフォーマット変更イベント
		/// </summary>
		public event EventHandler OnTotalStatusFormatChange = (sender, e) => { };
		/// <summary>
		/// 合計ステータス値のフォーマット
		/// </summary>
		private string _totalStatusFormat = string.Empty;
		public string TotalStatusFormat
		{
			get { return _totalStatusFormat; }
			set
			{
				if(_totalStatusFormat != value)
				{
					_totalStatusFormat = value;

					// 通知
					this.OnTotalStatusFormatChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// ベースステータス値のフォーマット変更イベント
		/// </summary>
		public event EventHandler OnBaseStatusFormatChange = (sender, e) => { };
		/// <summary>
		/// ベースステータス値のフォーマット
		/// </summary>
		private string _baseStatusFormat = string.Empty;
		public string BaseStatusFormat
		{
			get { return _baseStatusFormat; }
			set
			{
				if (_baseStatusFormat != value)
				{
					_baseStatusFormat = value;

					// 通知
					this.OnBaseStatusFormatChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// シンクロ値フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSynchroFormatChange = (sender, e) => { };
		/// <summary>
		/// シンクロ値フォーマット
		/// </summary>
		private string _synchroFormat = string.Empty;
		public string SynchroFormat
		{
			get { return _synchroFormat; }
			set
			{
				if(_synchroFormat != value)
				{
					_synchroFormat = value;

					// 通知
					this.OnSynchroFormatChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// スロット値フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSlotFormatChange = (sender, e) => { };
		/// <summary>
		/// スロット値フォーマット
		/// </summary>
		private string _slotFormat = string.Empty;
		public string SlotFormat
		{
			get { return _slotFormat; }
			set
			{
				if(_slotFormat != value)
				{
					_slotFormat = value;

					// 通知
					this.OnSlotFormatChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 増加値フォーマット変更イベント
		/// </summary>
		public event EventHandler OnUpFormatChange = (sender, e) => { };
		/// <summary>
		/// 増加値フォーマット
		/// </summary>
		private string _upFormat = string.Empty;
		public string UpFormat
		{
			get { return _upFormat; }
			set
			{
				if(_upFormat != value)
				{
					_upFormat = value;

					// 通知
					this.OnUpFormatChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 未確定フォーマット変更イベント
		/// </summary>
		public event EventHandler OnPendingFormatChange = (sender, e) => { };
		/// <summary>
		/// 未確定フォーマット
		/// </summary>
		private string _pendingFormat = string.Empty;
		public string PendingFormat
		{
			get { return _pendingFormat; }
			set
			{
				_pendingFormat = value;

				// 通知
				this.OnPendingFormatChange(this, EventArgs.Empty);
			}
		}
		#endregion
	}
}