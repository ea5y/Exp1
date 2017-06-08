/// <summary>
/// キャラクター詳細データ
/// 
/// 2016/03/25
/// </summary>
using UnityEngine;
using System;

namespace XUI.CharacterInfo
{


	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region キャラクターネーム更新
		// キャラクターネーム変更イベント
		event EventHandler<EventArgs> OnCharaNameChange;
		string CharaName { get; set; }
		// キャラクターネームフォーマット変更イベント
		event EventHandler<EventArgs> OnCharaNameFormatChange;
		string CharaNameFormat { get; set; }
		#endregion

		#region リビルド更新
		// リビルド表示変更イベント
		event EventHandler<EventArgs> OnRebuildTimeCgange;
		float RebuidTime { get; set; }
		event EventHandler<EventArgs> OnRebuildTimeFormatChange;
		string RebuildTimeFormat { get; set; }
		#endregion

		#region コスト更新
		// コスト表示変更イベント
		event EventHandler<EventArgs> OnCharaCostCgange;
		int CharaCost { get; set; }
		event EventHandler<EventArgs> OnCharaCostFormatChange;
		string CharaCsotFormat { get; set; }
		#endregion

		#region ロック更新
		event EventHandler<EventArgs> OnLockChange;
		bool IsLock { get; set; }
		#endregion

		#region ステータス情報の更新

		#region 生命力

		#region 合計生命力
		event EventHandler<EventArgs> OnTotalHitPointChange;
		int TotalHitPoint { get; set; }
		event EventHandler<EventArgs> OnTotalHitPointFormatChange;
		string TotalHitPointFormat { get; set; }
		#endregion

		#region 基礎生命力
		event EventHandler<EventArgs> OnBaseHitPointChange;
		int BaseHitPoint { get; set; }
		event EventHandler<EventArgs> OnBaseHitPointFormatChange;
		string BaseHitPointFormat { get; set; }
		#endregion

		#region スロット生命力
		event EventHandler<EventArgs> OnSlotHitPointChange;
		int SlotHitPoint { get; set; }
		event EventHandler<EventArgs> OnSlotHitPointFormatChange;
		string SlotHitPointFormat { get; set; }
		#endregion

		#region シンクロ生命力
		event EventHandler<EventArgs> OnSyncHitPointChange;
		int SyncHitPoint { get; set; }
		event EventHandler<EventArgs> OnSyncHitPointFormatChange;
		string SyncHitPointFormat { get; set; }
		#endregion

		#endregion

		#region 攻撃力

		#region 合計攻撃力
		event EventHandler<EventArgs> OnTotalAttackChange;
		int TotalAttack { get; set; }
		event EventHandler<EventArgs> OnTotalAttackFormatChange;
		string TotalAttackFormat { get; set; }
		#endregion

		#region 基礎攻撃力
		event EventHandler<EventArgs> OnBaseAttackChange;
		int BaseAttack { get; set; }
		event EventHandler<EventArgs> OnBaseAttackFormatChange;
		string BaseAttackFormat { get; set; }
		#endregion

		#region スロット攻撃力
		event EventHandler<EventArgs> OnSlotAttackChange;
		int SlotAttack { get; set; }
		event EventHandler<EventArgs> OnSlotAttackFormatChange;
		string SlotAttackFormat { get; set; }
		#endregion

		#region シンクロ攻撃力
		event EventHandler<EventArgs> OnSyncAttackChange;
		int SyncAttack { get; set; }
		event EventHandler<EventArgs> OnSyncAttackFormatChange;
		string SyncAttackFormat { get; set; }
		#endregion

		#endregion

		#region 防御力

		#region 合計防御力
		event EventHandler<EventArgs> OnTotalDefenseChange;
		int TotalDefense { get; set; }
		event EventHandler<EventArgs> OnTotalDefenseFormatChange;
		string TotalDefenseFormat { get; set; }
		#endregion

		#region 基礎防御力
		event EventHandler<EventArgs> OnBaseDefenseChange;
		int BaseDefense { get; set; }
		event EventHandler<EventArgs> OnBaseDefenseFormatChange;
		string BaseDefenseFormat { get; set; }
		#endregion

		#region スロット防御力
		event EventHandler<EventArgs> OnSlotDefenseChange;
		int SlotDefense { get; set; }
		event EventHandler<EventArgs> OnSlotDefenseFormatChange;
		string SlotDefenseFormat { get; set; }
		#endregion

		#region シンクロ防御力
		event EventHandler<EventArgs> OnSyncDefenseChange;
		int SyncDefense { get; set; }
		event EventHandler<EventArgs> OnSyncDefenseFormatChange;
		string SyncDefenseFormat { get; set; }
		#endregion

		#endregion

		#region 特殊能力

		#region 合計特殊能力
		event EventHandler<EventArgs> OnTotalExtraChange;
		int TotalExtra { get; set; }
		event EventHandler<EventArgs> OnTotalExtraFormatChange;
		string TotalExtraFormat { get; set; }
		#endregion

		#region 基礎特殊能力
		event EventHandler<EventArgs> OnBaseExtraChange;
		int BaseExtra { get; set; }
		event EventHandler<EventArgs> OnBaseExtraFormatChange;
		string BaseExtraFormat { get; set; }
		#endregion

		#region スロット特殊能力
		event EventHandler<EventArgs> OnSlotExtraChange;
		int SlotExtra { get; set; }
		event EventHandler<EventArgs> OnSlotExtraFormatChange;
		string SlotExtraFormat { get; set; }
		#endregion

		#region シンクロ特殊能力
		event EventHandler<EventArgs> OnSyncExtraChange;
		int SyncExtra { get; set; }
		event EventHandler<EventArgs> OnSyncExtraFormatChange;
		string SyncExtraFormat { get; set; }
		#endregion

		#endregion

		#endregion

		#region 強化関連
		/// <summary>
		/// ランクの更新イベント
		/// </summary>
		event EventHandler<EventArgs> OnRankChange;
		int Rank { get; set; }
		event EventHandler<EventArgs> OnRankFormatChange;
		string RankFormat { get; set; }

		/// <summary>
		/// レベルの更新イベント
		/// </summary>
		event EventHandler<EventArgs> OnPowerupLevelChange;
		int PowerupLevel { get; set; }
		event EventHandler<EventArgs> OnPowerupLevelFormatChange;
		string PowerupLevelFormat { get; set; }

		/// <summary>
		/// 所持経験値の更新イベント
		/// </summary>
		event EventHandler<EventArgs> OnPowerupExpChange;
		int PowerupExp { get; set; }
		event EventHandler<EventArgs> OnPowerupExpFormatChange;
		string PowerupExpFormat { get; set; }

		/// <summary>
		/// 残りシンクロ回数の更新イベント
		/// </summary>
		event EventHandler<EventArgs> OnSynchroRemainChange;
		int SynchroRemain { get; set; }
		event EventHandler<EventArgs> OnSynchroRemainFormatChange;
		string SynchroRemainFormat { get; set; }

		/// <summary>
		/// スロット数イベント
		/// </summary>
		event EventHandler<EventArgs> OnPowerupSlotNumChange;
		int PowerupSlotNum { get; set; }
		event EventHandler<EventArgs> OnPowerupSlotChange;
		int PowerupSlot { get; set; }
		event EventHandler<EventArgs> OnPowerupSlotFormatChange;
		string PowerupSlotFormat { get; set; }
		#endregion

		#region アバタータイプ
		event EventHandler<EventArgs> OnAvatarTypeChange;
		AvatarType AvatarType { get; set; }
        int SkinId { get; set; }

		#endregion

	}

	public class Model : IModel
	{

		#region 破棄
		public void Dispose()
		{
			this.OnBaseHitPointChange = null;
			this.OnBaseHitPointFormatChange = null;
			this.OnCharaCostCgange = null;
			this.OnCharaCostFormatChange = null;
			this.OnRankChange = null;
			this.OnRankFormatChange = null;
			this.OnPowerupLevelChange = null;
			this.OnPowerupLevelFormatChange = null;
			this.OnPowerupExpChange = null;
			this.OnPowerupExpFormatChange = null;
			this.OnCharaNameChange = null;
			this.OnCharaNameFormatChange = null;
			this.OnLockChange = null;
			this.OnRebuildTimeCgange = null;
			this.OnRebuildTimeFormatChange = null;
			this.OnTotalHitPointChange = null;
			this.OnTotalHitPointFormatChange = null;
		}
		#endregion

		#region キャラクターネーム変更
		// キャラクターネーム変更イベント
		public event EventHandler<EventArgs> OnCharaNameChange = (sender, e) => { };

		/// <summary>
		/// キャラネーム更新
		/// </summary>
		string _charaName = string.Empty;
		public string CharaName
		{
			get { return _charaName; }
			set
			{
				if (_charaName != value)
				{
					_charaName = value;
					var eventArgs = new EventArgs();
					this.OnCharaNameChange(this, eventArgs);
				}

			}
		}

		/// <summary>
		/// キャラネームフォーマット更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnCharaNameFormatChange = (sender, e) => { };

		string _charaNameFormat = "{0}";
		public string CharaNameFormat
		{
			get { return _charaNameFormat; }
			set
			{
				if(_charaNameFormat != value)
				{
					_charaNameFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnCharaNameFormatChange(this, eventArgs);
				}
			}
		}


		#endregion

		#region リビルドタイム更新

		// リビルドタイム更新イベント
		public event EventHandler<EventArgs> OnRebuildTimeCgange = (sender, e) => { };

		// リビルドタイム
		float _rebuildTime = 0;
		public float RebuidTime
		{
			get { return _rebuildTime; }
			set
			{
				if(_rebuildTime != value)
				{
					_rebuildTime = value;
					var eventArgas = new EventArgs();
					this.OnRebuildTimeCgange(this, eventArgas);
				}
			}
		}

		/// <summary>
		/// リビルドタイム変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnRebuildTimeFormatChange = (sender, e) => { };

		/// <summary>
		/// リビルドタイムフォーマット
		/// </summary>
		string _rebuildTimeFormat = "{0}";
		public string RebuildTimeFormat
		{
			get { return _rebuildTimeFormat; }
			set
			{
				if(_rebuildTimeFormat != value)
				{
					_rebuildTimeFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnRebuildTimeFormatChange(this, eventArgs);
				}
			}
		}

		#endregion

		#region キャラクターコスト更新

		/// <summary>
		/// キャラクターコスト更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnCharaCostCgange = (sender, e) => { };

		/// <summary>
		/// キャラクターコスト
		/// </summary>
		int _charaCost = 0;
		public int CharaCost
		{
			get { return _charaCost; }
			set
			{
				if (_charaCost != value)
				{
					_charaCost = value;
					var eventArgas = new EventArgs();
					this.OnCharaCostCgange(this, eventArgas);
				}
			}
		}

		/// <summary>
		/// キャラクターコストフォーマット変更イベント
		/// </summary>
		public event EventHandler<EventArgs> OnCharaCostFormatChange = (sender, e) => { };

		/// <summary>
		/// キャラクターコストフォーマット
		/// </summary>
		string _charaCostFormat = "{0}";
		public string CharaCsotFormat
		{
			get { return _charaCostFormat; }
			set
			{
				if(_charaCostFormat != value)
				{
					_charaCostFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnCharaCostFormatChange(this, eventArgs);
				}
			}
		}


		#endregion

		#region ロック状態の更新
		/// <summary>
		/// ロック更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnLockChange = (sender, e) => { };

		bool _isLock = false;
		public bool IsLock
		{
			get { return _isLock; }
			set
			{
				if (_isLock != value)
				{
					_isLock = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnLockChange(this, eventArgs);
				}
			}
		}

		#endregion

		#region ステータス情報の更新

		#region 生命力

		#region 合計生命力
		// 合計生命力の更新イベント
		public event EventHandler<EventArgs> OnTotalHitPointChange = (sender, e) => { };
		int _totalHitPoint = 0;
		public int TotalHitPoint
		{
			get { return _totalHitPoint; }
			set
			{
				if (_totalHitPoint != value)
				{
					_totalHitPoint = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalHitPointChange(this, eventArgs);
				}
			}
		}

		// 合計生命力のフォーマット
		public event EventHandler<EventArgs> OnTotalHitPointFormatChange = (sender, e) => { };
		string _totalHitPointFormat = "{0}";
		public string TotalHitPointFormat
		{
			get { return _totalHitPointFormat; }
			set
			{
				if (_totalHitPointFormat != value)
				{
					_totalHitPointFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalHitPointFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 基礎生命力
		// 基礎生命力の更新イベント
		public event EventHandler<EventArgs> OnBaseHitPointChange = (sender, e) => { };
		int _baseHitPoint = 0;
		public int BaseHitPoint
		{
			get { return _baseHitPoint; }
			set
			{
				if (_baseHitPoint != value)
				{
					_baseHitPoint = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnBaseHitPointChange(this, eventArgs);
				}
			}
		}

		// 基礎生命力のフォーマット
		public event EventHandler<EventArgs> OnBaseHitPointFormatChange = (sender, e) => { };
		string _baseHitPointFormat = "{0}";
		public string BaseHitPointFormat
		{
			get { return _baseHitPointFormat; }
			set
			{
				if (_baseHitPointFormat != value)
				{
					_baseHitPointFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnBaseHitPointFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region スロット生命力
		// スロット生命力の更新イベント
		public event EventHandler<EventArgs> OnSlotHitPointChange = (sender, e) => { };
		int _slotHitPoint = 0;
		public int SlotHitPoint
		{
			get { return _slotHitPoint; }
			set
			{
				if (_slotHitPoint != value)
				{
					_slotHitPoint = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotHitPointChange(this, eventArgs);
				}
			}
		}

		// スロット生命力のフォーマット
		public event EventHandler<EventArgs> OnSlotHitPointFormatChange = (sender, e) => { };
		string _slotHitPointFormat = "{0}";
		public string SlotHitPointFormat
		{
			get { return _slotHitPointFormat; }
			set
			{
				if (_slotHitPointFormat != value)
				{
					_slotHitPointFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalHitPointFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region シンクロ生命力
		// シンクロ生命力の更新イベント
		public event EventHandler<EventArgs> OnSyncHitPointChange = (sender, e) => { };
		int _syncHitPoint = 0;
		public int SyncHitPoint
		{
			get { return _syncHitPoint; }
			set
			{
				if (_syncHitPoint != value)
				{
					_syncHitPoint = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnSyncHitPointChange(this, eventArgs);
				}
			}
		}

		// シンクロ生命力のフォーマット
		public event EventHandler<EventArgs> OnSyncHitPointFormatChange = (sender, e) => { };
		string _syncHitPointFormat = "{0}";
		public string SyncHitPointFormat
		{
			get { return _syncHitPointFormat; }
			set
			{
				if (_syncHitPointFormat != value)
				{
					_syncHitPointFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSyncHitPointFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#endregion

		#region 攻撃力

		#region 合計攻撃力
		// 合計攻撃力の更新イベント
		public event EventHandler<EventArgs> OnTotalAttackChange = (sender, e) => { };
		int _totalAttack = 0;
		public int TotalAttack
		{
			get { return _totalAttack; }
			set
			{
				if (_totalAttack != value)
				{
					_totalAttack = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalAttackChange(this, eventArgs);
				}
			}
		}

		// 合計攻撃力のフォーマット
		public event EventHandler<EventArgs> OnTotalAttackFormatChange = (sender, e) => { };
		string _totalAttackFormat = "{0}";
		public string TotalAttackFormat
		{
			get { return _totalAttackFormat; }
			set
			{
				if (_totalAttackFormat != value)
				{
					_totalAttackFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalAttackFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 基礎攻撃力
		// 基礎攻撃力の更新イベント
		public event EventHandler<EventArgs> OnBaseAttackChange = (sender, e) => { };
		int _baseAttack = 0;
		public int BaseAttack
		{
			get { return _baseAttack; }
			set
			{
				if (_baseAttack != value)
				{
					_baseAttack = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnBaseAttackChange(this, eventArgs);
				}
			}
		}

		// 基礎攻撃力のフォーマット
		public event EventHandler<EventArgs> OnBaseAttackFormatChange = (sender, e) => { };
		string _baseAttackFormat = "{0}";
		public string BaseAttackFormat
		{
			get { return _baseAttackFormat; }
			set
			{
				if (_baseAttackFormat != value)
				{
					_baseAttackFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnBaseAttackFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region スロット攻撃力
		// スロット攻撃力の更新イベント
		public event EventHandler<EventArgs> OnSlotAttackChange = (sender, e) => { };
		int _slotAttack = 0;
		public int SlotAttack
		{
			get { return _slotAttack; }
			set
			{
				if (_slotAttack != value)
				{
					_slotAttack = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotAttackChange(this, eventArgs);
				}
			}
		}

		// スロット攻撃力のフォーマット
		public event EventHandler<EventArgs> OnSlotAttackFormatChange = (sender, e) => { };
		string _slotAttackFormat = "{0}";
		public string SlotAttackFormat
		{
			get { return _slotAttackFormat; }
			set
			{
				if (_slotAttackFormat != value)
				{
					_slotAttackFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotAttackFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region シンクロ攻撃力
		// シンクロ攻撃力の更新イベント
		public event EventHandler<EventArgs> OnSyncAttackChange = (sender, e) => { };
		int _syncAttack = 0;
		public int SyncAttack
		{
			get { return _syncAttack; }
			set
			{
				if (_syncAttack != value)
				{
					_syncAttack = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnSyncAttackChange(this, eventArgs);
				}
			}
		}

		// シンクロ攻撃力のフォーマット
		public event EventHandler<EventArgs> OnSyncAttackFormatChange = (sender, e) => { };
		string _syncAttackFormat = "{0}";
		public string SyncAttackFormat
		{
			get { return _syncAttackFormat; }
			set
			{
				if (_syncAttackFormat != value)
				{
					_syncAttackFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSyncAttackFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#endregion

		#region 防御力

		#region 合計防御力
		// 合計防御力の更新イベント
		public event EventHandler<EventArgs> OnTotalDefenseChange = (sender, e) => { };
		int _totalDefense = 0;
		public int TotalDefense
		{
			get { return _totalDefense; }
			set
			{
				if (_totalDefense != value)
				{
					_totalDefense = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalDefenseChange(this, eventArgs);
				}
			}
		}

		// 合計防御力のフォーマット
		public event EventHandler<EventArgs> OnTotalDefenseFormatChange = (sender, e) => { };
		string _totalDefenseFormat = "{0}";
		public string TotalDefenseFormat
		{
			get { return _totalDefenseFormat; }
			set
			{
				if (_totalDefenseFormat != value)
				{
					_totalDefenseFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalDefenseFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 基礎防御力
		// 基礎防御力の更新イベント
		public event EventHandler<EventArgs> OnBaseDefenseChange = (sender, e) => { };
		int _baseDefense = 0;
		public int BaseDefense
		{
			get { return _baseDefense; }
			set
			{
				if (_baseDefense != value)
				{
					_baseDefense = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnBaseDefenseChange(this, eventArgs);
				}
			}
		}

		// 基礎防御力のフォーマット
		public event EventHandler<EventArgs> OnBaseDefenseFormatChange = (sender, e) => { };
		string _baseDefenseFormat = "{0}";
		public string BaseDefenseFormat
		{
			get { return _baseDefenseFormat; }
			set
			{
				if (_baseDefenseFormat != value)
				{
					_baseDefenseFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalDefenseFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region スロット防御力
		// スロット防御力の更新イベント
		public event EventHandler<EventArgs> OnSlotDefenseChange = (sender, e) => { };
		int _slotDefense = 0;
		public int SlotDefense
		{
			get { return _slotDefense; }
			set
			{
				if (_slotDefense != value)
				{
					_slotDefense = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotDefenseChange(this, eventArgs);
				}
			}
		}

		// スロット防御力のフォーマット
		public event EventHandler<EventArgs> OnSlotDefenseFormatChange = (sender, e) => { };
		string _slotDefenseFormat = "{0}";
		public string SlotDefenseFormat
		{
			get { return _slotDefenseFormat; }
			set
			{
				if (_slotDefenseFormat != value)
				{
					_slotDefenseFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotDefenseFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region シンクロ防御力
		// シンクロ防御力の更新イベント
		public event EventHandler<EventArgs> OnSyncDefenseChange = (sender, e) => { };
		int _syncDefense = 0;
		public int SyncDefense
		{
			get { return _syncDefense; }
			set
			{
				if (_syncDefense != value)
				{
					_syncDefense = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnSyncDefenseChange(this, eventArgs);
				}
			}
		}

		// シンクロ防御力のフォーマット
		public event EventHandler<EventArgs> OnSyncDefenseFormatChange = (sender, e) => { };
		string _syncDefenseFormat = "{0}";
		public string SyncDefenseFormat
		{
			get { return _syncDefenseFormat; }
			set
			{
				if (_syncDefenseFormat != value)
				{
					_syncDefenseFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSyncDefenseFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#endregion

		#region 特殊能力

		#region 合計特殊能力
		// 合計特殊能力の更新イベント
		public event EventHandler<EventArgs> OnTotalExtraChange = (sender, e) => { };
		int _totalExtra = 0;
		public int TotalExtra
		{
			get { return _totalExtra; }
			set
			{
				if (_totalExtra != value)
				{
					_totalExtra = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalExtraChange(this, eventArgs);
				}
			}
		}

		// 合計特殊能力のフォーマット
		public event EventHandler<EventArgs> OnTotalExtraFormatChange = (sender, e) => { };
		string _totalExtraFormat = "{0}";
		public string TotalExtraFormat
		{
			get { return _totalExtraFormat; }
			set
			{
				if (_totalExtraFormat != value)
				{
					_totalExtraFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnTotalExtraFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 基礎特殊能力
		// 基礎特殊能力の更新イベント
		public event EventHandler<EventArgs> OnBaseExtraChange = (sender, e) => { };
		int _baseExtra = 0;
		public int BaseExtra
		{
			get { return _baseExtra; }
			set
			{
				if (_baseExtra != value)
				{
					_baseExtra = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnBaseExtraChange(this, eventArgs);
				}
			}
		}

		// 基礎特殊能力のフォーマット
		public event EventHandler<EventArgs> OnBaseExtraFormatChange = (sender, e) => { };
		string _baseExtraFormat = "{0}";
		public string BaseExtraFormat
		{
			get { return _baseExtraFormat; }
			set
			{
				if (_baseExtraFormat != value)
				{
					_baseExtraFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnBaseExtraFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region スロット特殊能力
		// スロット特殊能力の更新イベント
		public event EventHandler<EventArgs> OnSlotExtraChange = (sender, e) => { };
		int _slotExtra = 0;
		public int SlotExtra
		{
			get { return _slotExtra; }
			set
			{
				if (_slotExtra != value)
				{
					_slotExtra = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotExtraChange(this, eventArgs);
				}
			}
		}

		// スロット特殊能力のフォーマット
		public event EventHandler<EventArgs> OnSlotExtraFormatChange = (sender, e) => { };
		string _slotExtraFormat = "{0}";
		public string SlotExtraFormat
		{
			get { return _slotExtraFormat; }
			set
			{
				if (_slotExtraFormat != value)
				{
					_slotExtraFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSlotExtraFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region シンクロ特殊能力
		// シンクロ特殊能力の更新イベント
		public event EventHandler<EventArgs> OnSyncExtraChange = (sender, e) => { };
		int _syncExtra = 0;
		public int SyncExtra
		{
			get { return _syncExtra; }
			set
			{
				if (_syncExtra != value)
				{
					_syncExtra = value;
					// 通知
					var eventArgs = new EventArgs();
					this.OnSyncExtraChange(this, eventArgs);
				}
			}
		}

		// シンクロ特殊能力のフォーマット
		public event EventHandler<EventArgs> OnSyncExtraFormatChange = (sender, e) => { };
		string _syncExtraFormat = "{0}";
		public string SyncExtraFormat
		{
			get { return _syncExtraFormat; }
			set
			{
				if (_syncExtraFormat != value)
				{
					_syncExtraFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSyncExtraFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#endregion

		#endregion

		#region 強化関連

		#region ランク
		/// <summary>
		/// ランクと登録
		/// </summary>
		public event EventHandler<EventArgs> OnRankChange = (sender, e) => { };

		int _rank = 0;
		public int Rank
		{
			get { return _rank; }
			set
			{
				if(_rank != value)
				{
					_rank = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnRankChange(this, eventArgs);
				}
			}

		}

		// ランクのフォーマット
		public event EventHandler<EventArgs> OnRankFormatChange = (sender, e) => { };
		string _rankFormat = "{0}";
		public string RankFormat
		{
			get { return _rankFormat; }
			set
			{
				if (_rankFormat != value)
				{
					_rankFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnRankFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region レベル
		/// <summary>
		/// レベル更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnPowerupLevelChange = (sender, e) => { };

		int _powerupLevel = 0;
		public int PowerupLevel
		{
			get { return _powerupLevel; }
			set
			{
				if (_powerupLevel != value)
				{
					_powerupLevel = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnPowerupLevelChange(this, eventArgs);
				}
			}

		}

		// レベルのフォーマット
		public event EventHandler<EventArgs> OnPowerupLevelFormatChange = (sender, e) => { };
		string _PowerupLevelFormat = "{0}";
		public string PowerupLevelFormat
		{
			get { return _PowerupLevelFormat; }
			set
			{
				if (_PowerupLevelFormat != value)
				{
					_PowerupLevelFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnPowerupLevelFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 所持件経験値
		/// <summary>
		/// 所持経験値イベント
		/// </summary>
		public event EventHandler<EventArgs> OnPowerupExpChange = (sender, e) => { };

		int _powerupExp = 0;
		public int PowerupExp
		{
			get { return _powerupExp; }
			set
			{
				if (_powerupExp != value)
				{
					_powerupExp = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnPowerupExpChange(this, eventArgs);
				}
			}

		}

		// 所持経験値のフォーマット
		public event EventHandler<EventArgs> OnPowerupExpFormatChange = (sender, e) => { };
		string _powerupExpFormat = "{0:#,0}";
		public string PowerupExpFormat
		{
			get { return _powerupExpFormat; }
			set
			{
				if (_powerupExpFormat != value)
				{
					_powerupExpFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnPowerupExpFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 残りシンクロ合成回数
		/// <summary>
		/// 残りシンクロ合成回数イベント
		/// </summary>
		public event EventHandler<EventArgs> OnSynchroRemainChange = (sender, e) => { };

		int _synchroRemain = 0;
		public int SynchroRemain
		{
			get { return _synchroRemain; }
			set
			{
				if (_synchroRemain != value)
				{
					_synchroRemain = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSynchroRemainChange(this, eventArgs);
				}
			}

		}
		// 所持経験値のフォーマット
		public event EventHandler<EventArgs> OnSynchroRemainFormatChange = (sender, e) => { };
		string _synchroRemainFormat = "{0}";
		public string SynchroRemainFormat
		{
			get { return _synchroRemainFormat; }
			set
			{
				if (_synchroRemainFormat != value)
				{
					_synchroRemainFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnSynchroRemainFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#region 強化スロット
		/// <summary>
		/// 強化スロット最大数更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnPowerupSlotNumChange = (sender, e) => { };

		int _powerupSlotNum = 0;
		public int PowerupSlotNum
		{
			get { return _powerupSlotNum; }
			set
			{
				if (_powerupSlotNum != value)
				{
					_powerupSlotNum = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnPowerupSlotNumChange(this, eventArgs);
				}
			}

		}
		/// <summary>
		/// 強化スロット更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnPowerupSlotChange = (sender, e) => { };

		int _poweruSlot = 0;
		public int PowerupSlot
		{
			get { return _poweruSlot; }
			set
			{
				if (_poweruSlot != value)
				{
					_poweruSlot = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnPowerupSlotChange(this, eventArgs);
				}
			}

		}


		// 強化スロットのフォーマット
		public event EventHandler<EventArgs> OnPowerupSlotFormatChange = (sender, e) => { };

		string _powerupSlotFormat = "{0}/{1}";
		public string PowerupSlotFormat
		{
			get { return _powerupSlotFormat; }
			set
			{
				if (_powerupSlotFormat != value)
				{
					_powerupSlotFormat = value;

					// 通知
					var eventArgs = new EventArgs();
					this.OnPowerupSlotFormatChange(this, eventArgs);
				}
			}
		}
		#endregion

		#endregion

		#region アバタータイプ
		/// <summary>
		/// アバタータイプ更新イベント
		/// </summary>
		public event EventHandler<EventArgs> OnAvatarTypeChange = (sender, e) => { };

		AvatarType _avatarType = 0;
		public AvatarType AvatarType
		{
			get { return _avatarType; }
			set
			{
				if (_avatarType != value)
				{
					_avatarType = value;

					// 通知
					var eventArgs = new EventArgs();
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
                    var eventArgs = new EventArgs();
                    this.OnAvatarTypeChange(this, eventArgs);
                }
                
            }
        }
		#endregion

	}

}

