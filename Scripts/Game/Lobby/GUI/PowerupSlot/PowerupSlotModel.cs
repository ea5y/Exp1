/// <summary>
/// 強化スロットデータ
/// 
/// 2016/03/02
/// </summary>
using System;

namespace XUI.PowerupSlot
{
	/// <summary>
	/// 強化スロットデータインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region ステータス関連
		#region 生命力
		/// <summary>
		/// 生命力変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnHitPointChange;
		/// <summary>
		/// 生命力
		/// </summary>
		int HitPoint { get; set; }

		/// <summary>
		/// 生命力試算後変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnHitPointCalcChange;
		/// <summary>
		/// 生命力試算後
		/// </summary>
		int HitPointCalc { get; set; }
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnAttackChange;
		/// <summary>
		/// 攻撃力
		/// </summary>
		int Attack { get; set; }

		/// <summary>
		/// 攻撃力試算後変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnAttackCalcChange;
		/// <summary>
		/// 攻撃力試算後
		/// </summary>
		int AttackCalc { get; set; }
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnDefenseChange;
		/// <summary>
		/// 防御力
		/// </summary>
		int Defense { get; set; }

		/// <summary>
		/// 防御力試算後変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnDefenseCalcChange;
		/// <summary>
		/// 防御力試算後
		/// </summary>
		int DefenseCalc { get; set; }
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnExtraChange;
		/// <summary>
		/// 特殊能力
		/// </summary>
		int Extra { get; set; }

		/// <summary>
		/// 特殊能力試算後変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnExtraCalcChange;
		/// <summary>
		/// 特殊能力試算後
		/// </summary>
		int ExtraCalc { get; set; }
		#endregion

		#region ステータス表示
		/// <summary>
		/// ステータスフォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnStatusFormatChange;
		/// <summary>
		/// ステータスフォーマット
		/// </summary>
		string StatusFormat { get; set; }

		/// <summary>
		/// ステータス試算後フォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnStatusCalcFormatChange;
		/// <summary>
		/// ステータス試算後フォーマット
		/// </summary>
		string StatusCalcFormat { get; set; }
		#endregion
		#endregion

		#region スロット数
		/// <summary>
		/// スロット個数変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnSlotCountChange;
		/// <summary>
		/// スロット個数
		/// </summary>
		int SlotCount { get; set; }

		/// <summary>
		/// スロット枠変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnSlotCapacityChange;
		/// <summary>
		/// スロット枠
		/// </summary>
		int SlotCapacity { get; set; }

		/// <summary>
		/// スロット数フォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnSlotNumFormatChange;
		/// <summary>
		/// スロット数フォーマット
		/// </summary>
		string SlotNumFormat { get; set; }
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnHaveMoneyChange;
		/// <summary>
		/// 所持金
		/// </summary>
		int HaveMoney { get; set; }

		/// <summary>
		/// 所持金フォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnHaveMoneyFormatChange;
		/// <summary>
		/// 所持金フォーマット
		/// </summary>
		string HaveMoneyFormat { get; set; }
		#endregion

		#region 費用
		/// <summary>
		/// 費用変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnNeedMoneyChange;
		/// <summary>
		/// 費用
		/// </summary>
		int NeedMoney { get; set; }

		/// <summary>
		/// 費用フォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnNeedMoneyFormatChange;
		/// <summary>
		/// 費用フォーマット
		/// </summary>
		string NeedMoneyFormat { get; set; }
		#endregion

		#region 追加料金
		/// <summary>
		/// 追加料金変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnAddOnChargeChange;
		/// <summary>
		/// 追加料金
		/// </summary>
		int AddOnCharge { get; set; }

		/// <summary>
		/// 追加料金フォーマット変更イベント
		/// </summary>
		event EventHandler<EventArgs> OnAddOnChargeFormatChange;
		/// <summary>
		/// 追加料金フォーマット
		/// </summary>
		string AddOnChargeFormat { get; set; }
		#endregion
	}

	/// <summary>
	/// 強化スロットデータ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnHitPointChange = null;
			this.OnAttackChange = null;
			this.OnAttackCalcChange = null;
			this.OnDefenseChange = null;
			this.OnDefenseCalcChange = null;
			this.OnExtraChange = null;
			this.OnExtraCalcChange = null;
			this.OnStatusFormatChange = null;
			this.OnStatusCalcFormatChange = null;
			this.OnSlotCountChange = null;
			this.OnSlotCapacityChange = null;
			this.OnSlotNumFormatChange = null;
			this.OnHaveMoneyChange = null;
			this.OnHaveMoneyFormatChange = null;
			this.OnNeedMoneyChange = null;
			this.OnNeedMoneyFormatChange = null;
			this.OnAddOnChargeChange = null;
			this.OnAddOnChargeFormatChange = null;
		}
		#endregion

		#region ステータス関連
		#region 生命力
		/// <summary>
		/// 生命力変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnHitPointChange = (sender, e) => { };
		/// <summary>
		/// 生命力
		/// </summary>
		int _hitPoint = 0;
		public int HitPoint
		{
			get { return _hitPoint; }
			set
			{
				if (_hitPoint != value)
				{
					_hitPoint = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnHitPointChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// 生命力試算後変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnHitPointCalcChange = (sender, e) => { };
		/// <summary>
		/// 生命力試算後
		/// </summary>
		int _hitPointCalc = 0;
		public int HitPointCalc
		{
			get { return _hitPointCalc; }
			set
			{
				//if (_hitPointCalc != value)	// 同じ値でも増減スプライトのオンオフの変化があるので常に通知する
				{
					_hitPointCalc = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnHitPointCalcChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 攻撃力
		/// <summary>
		/// 攻撃力変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnAttackChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力
		/// </summary>
		int _attack = 0;
		public int Attack
		{
			get { return _attack; }
			set
			{
				if (_attack != value)
				{
					_attack = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnAttackChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// 攻撃力試算後変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnAttackCalcChange = (sender, e) => { };
		/// <summary>
		/// 攻撃力試算後
		/// </summary>
		int _attackCalc = 0;
		public int AttackCalc
		{
			get { return _attackCalc; }
			set
			{
				//if (_attackCalc != value)	// 同じ値でも増減スプライトのオンオフの変化があるので常に通知する
				{
					_attackCalc = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnAttackCalcChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 防御力
		/// <summary>
		/// 防御力変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnDefenseChange = (sender, e) => { };
		/// <summary>
		/// 防御力
		/// </summary>
		int _defense = 0;
		public int Defense
		{
			get { return _defense; }
			set
			{
				if (_defense != value)
				{
					_defense = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnDefenseChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// 防御力試算後変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnDefenseCalcChange = (sender, e) => { };
		/// <summary>
		/// 防御力試算後
		/// </summary>
		int _defenseCalc = 0;
		public int DefenseCalc
		{
			get { return _defenseCalc; }
			set
			{
				//if (_defenseCalc != value)	// 同じ値でも増減スプライトのオンオフの変化があるので常に通知する
				{
					_defenseCalc = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnDefenseCalcChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 特殊能力
		/// <summary>
		/// 特殊能力変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnExtraChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力
		/// </summary>
		int _extra = 0;
		public int Extra
		{
			get { return _extra; }
			set
			{
				if (_extra != value)
				{
					_extra = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnExtraChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// 特殊能力試算後変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnExtraCalcChange = (sender, e) => { };
		/// <summary>
		/// 特殊能力試算後
		/// </summary>
		int _extraCalc = 0;
		public int ExtraCalc
		{
			get { return _extraCalc; }
			set
			{
				//if (_extraCalc != value)	// 同じ値でも増減スプライトのオンオフの変化があるので常に通知する
				{
					_extraCalc = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnExtraCalcChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region ステータス表示
		/// <summary>
		/// ステータスフォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnStatusFormatChange = (sender, e) => { };
		/// <summary>
		/// ステータスフォーマット
		/// </summary>
		string _statusFormat = "";
		public string StatusFormat
		{
			get { return _statusFormat; }
			set
			{
				if (_statusFormat != value)
				{
					_statusFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnStatusFormatChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// ステータス試算後フォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnStatusCalcFormatChange = (sender, e) => { };
		/// <summary>
		/// ステータス試算後フォーマット
		/// </summary>
		string _statusCalcFormat = "";
		public string StatusCalcFormat
		{
			get { return _statusCalcFormat; }
			set
			{
				if (_statusCalcFormat != value)
				{
					_statusCalcFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnStatusCalcFormatChange(this, eventArgs);
				}
			}
		}
		#endregion
		#endregion

		#region スロット数
		/// <summary>
		/// スロット個数変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnSlotCountChange = (sender, e) => { };
		/// <summary>
		/// スロット個数
		/// </summary>
		int _slotCount = 0;
		public int SlotCount
		{
			get { return _slotCount; }
			set
			{
				if (_slotCount != value)
				{
					_slotCount = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotCountChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// スロット枠変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnSlotCapacityChange = (sender, e) => { };
		/// <summary>
		/// スロット枠
		/// </summary>
		int _slotCapacity = 0;
		public int SlotCapacity
		{
			get { return _slotCapacity; }
			set
			{
				if (_slotCapacity != value)
				{
					_slotCapacity = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotCapacityChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// スロット数フォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnSlotNumFormatChange = (sender, e) => { };
		/// <summary>
		/// スロット数フォーマット
		/// </summary>
		string _slotNumFormat = "";
		public string SlotNumFormat
		{
			get { return _slotNumFormat; }
			set
			{
				if (_slotNumFormat != value)
				{
					_slotNumFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotNumFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnHaveMoneyChange = (sender, e) => { };
		/// <summary>
		/// 所持金
		/// </summary>
		int _haveMoney = 0;
		public int HaveMoney
		{
			get { return _haveMoney; }
			set
			{
				if (_haveMoney != value)
				{
					_haveMoney = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnHaveMoneyChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// 所持金フォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnHaveMoneyFormatChange = (sender, e) => { };
		/// <summary>
		/// 所持金フォーマット
		/// </summary>
		string _haveMoneyFormat = "";
		public string HaveMoneyFormat
		{
			get { return _haveMoneyFormat; }
			set
			{
				if (_haveMoneyFormat != value)
				{
					_haveMoneyFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnHaveMoneyFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 費用
		/// <summary>
		/// 費用変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnNeedMoneyChange = (sender, e) => { };
		/// <summary>
		/// 費用
		/// </summary>
		int _needMoney = 0;
		public int NeedMoney
		{
			get { return _needMoney; }
			set
			{
				if (_needMoney != value)
				{
					_needMoney = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnNeedMoneyChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// 費用フォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnNeedMoneyFormatChange = (sender, e) => { };
		/// <summary>
		/// 費用フォーマット
		/// </summary>
		string _needMoneyFormat = "";
		public string NeedMoneyFormat
		{
			get { return _needMoneyFormat; }
			set
			{
				if (_needMoneyFormat != value)
				{
					_needMoneyFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnNeedMoneyFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 追加料金
		/// <summary>
		/// 追加料金変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnAddOnChargeChange = (sender, e) => { };
		/// <summary>
		/// 追加料金
		/// </summary>
		int _addOnCharge = 0;
		public int AddOnCharge
		{
			get { return _addOnCharge; }
			set
			{
				if (_addOnCharge != value)
				{
					_addOnCharge = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnAddOnChargeChange(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// 追加料金フォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnAddOnChargeFormatChange = (sender, e) => { };
		/// <summary>
		/// 追加料金フォーマット
		/// </summary>
		string _addOnChargeFormat = "";
		public string AddOnChargeFormat
		{
			get { return _addOnChargeFormat; }
			set
			{
				if (_addOnChargeFormat != value)
				{
					_addOnChargeFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnAddOnChargeFormatChange(this, eventArgs);
				}
			}
		}
		#endregion
	}
}
