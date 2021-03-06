/// <summary>
/// シンクロ合成データ
/// 
/// 2016/02/24
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI
{
	namespace Synchro
	{
		#region シンクロ合成残り回数色クラス
		/// <summary>
		/// シンクロ合成残り回数色クラス
		/// </summary>
		public class SynchroRemainColorSettings
		{
			public SynchroRemainColorSettings(Color normal, Color warning)
			{
				this._normal = normal;
				this._warning = warning;
			}

			private readonly Color _normal;
			public Color Normal { get { return _normal; } }
			private readonly Color _warning;
			public Color Warning { get { return _warning; } }
		}
		#endregion

		/// <summary>
		/// シンクロ合成データインターフェイス
		/// </summary>
		public interface IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();
			#endregion

			#region 所持金
			/// <summary>
			/// 所持金変更イベント
			/// </summary>
			event EventHandler OnHaveMoneyChange;
			/// <summary>
			/// 所持金
			/// </summary>
			int HaveMoney { get; set; }

			/// <summary>
			/// 所持金フォーマット変更イベント
			/// </summary>
			event EventHandler OnHaveMoneyFormatChange;
			/// <summary>
			/// 所持金フォーマット
			/// </summary>
			string HaveMoneyFormat { get; set; }
			#endregion

			#region 費用
			/// <summary>
			/// 費用変更イベント
			/// </summary>
			event EventHandler OnNeedMoneyChange;
			/// <summary>
			/// 費用
			/// </summary>
			int NeedMoney { get; set; }

			/// <summary>
			/// 費用フォーマット変更イベント
			/// </summary>
			event EventHandler OnNeedMoneyFormatChange;
			/// <summary>
			/// 費用フォーマット
			/// </summary>
			string NeedMoneyFormat { get; set; }
			#endregion

			#region 追加料金
			/// <summary>
			/// 追加料金変更イベント
			/// </summary>
			event EventHandler OnAddOnChargeChange;
			/// <summary>
			/// 追加料金
			/// </summary>
			int AddOnCharge { get; set; }

			/// <summary>
			/// 追加料金フォーマット変更イベント
			/// </summary>
			event EventHandler OnAddOnChargeFormatChange;
			/// <summary>
			/// 追加料金フォーマット
			/// </summary>
			string AddOnChargeFormat { get; set; }
			#endregion

			#region ベースキャラス生命力
			/// <summary>
			/// 生命力シンクロボーナス変更イベント
			/// </summary>
			event EventHandler OnSynchroHitPointChange;
			/// <summary>
			/// 生命力シンクロボーナス
			/// </summary>
			int SynchroHitPoint { get; set; }
			#endregion

			#region ベースキャラ攻撃力
			/// <summary>
			/// 攻撃力シンクロボーナス変更イベント
			/// </summary>
			event EventHandler OnSynchroAttackChange;
			/// <summary>
			/// 攻撃力シンクロボーナス
			/// </summary>
			int SynchroAttack { get; set; }
			#endregion

			#region ベースキャラ防御力
			/// <summary>
			/// 防御力シンクロボーナス変更イベント
			/// </summary>
			event EventHandler OnSynchroDefenseChange;
			/// <summary>
			/// 防御力シンクロボーナス
			/// </summary>
			int SynchroDefense { get; set; }
			#endregion

			#region ベースキャラ特殊能力
			/// <summary>
			/// 特殊能力シンクロボーナス変更イベント
			/// </summary>
			event EventHandler OnSynchroExtraChange;
			/// <summary>
			/// 特殊能力シンクロボーナス
			/// </summary>
			int SynchroExtra { get; set; }
			#endregion

			#region ベースキャラシンクロ合成残り回数
			/// <summary>
			/// シンクロ合成残り回数変更イベント
			/// </summary>
			event EventHandler OnSynchroRemainChange;
			/// <summary>
			/// シンクロ合成残り回数
			/// </summary>
			int SynchroRemain { get; set; }

			/// <summary>
			/// シンクロ合成残り回数色
			/// </summary>
			event EventHandler OnSynchroRemainColorChange;
			SynchroRemainColorSettings SynchroRemainColor { get; set; }
			#endregion

			#region 合計強化量
			/// <summary>
			/// 合計強化量変更イベント
			/// </summary>
			event EventHandler OnTotalSynchroBonusChange;
			/// <summary>
			/// 合計強化量
			/// </summary>
			int TotalSynchroBonus { get; set; }

			/// <summary>
			/// 残り合計強化量変更イベント
			/// </summary>
			event EventHandler OnTotalSynchroBonusRemainChnage;
			/// <summary>
			/// 残り合計強化量
			/// </summary>
			int TotalSynchroBonusRemain { get; set; }
			#endregion

			#region シンクロ値表示フォーマット
			/// <summary>
			/// シンクロ値表示フォーマット変更イベント
			/// </summary>
			event EventHandler OnSynchroFromatChnage;
			/// <summary>
			/// シンクロ値表示フォーマット
			/// </summary>
			string SynchroFormat { get; set; }
			#endregion

			#region シンクロ増加値表示フォーマット
			/// <summary>
			/// シンクロ増加値表示フォーマット変更イベント
			/// </summary>
			event EventHandler OnSynchroUpFromatChnage;
			/// <summary>
			/// シンクロ増加値表示フォーマット
			/// </summary>
			string SynchroUpFormat { get; set; }
			#endregion

			#region シンクロ増加値最大時表示フォーマット
			/// <summary>
			/// シンクロ増加値最大時表示フォーマット変更イベント
			/// </summary>
			event EventHandler OnSynchroMaxFromatChnage;
			/// <summary>
			/// シンクロ増加値最大時表示フォーマット
			/// </summary>
			string SynchroMaxFormat { get; set; }
			#endregion

			#region キャラ情報リスト
			/// <summary>
			/// キャラ情報リストのセット
			/// </summary>
			event EventHandler OnCharaInfoListChange;
			void SetCharaInfoList(List<CharaInfo> charaInfoList);

			/// <summary>
			/// キャラ情報リストクリア
			/// </summary>
			event EventHandler OnClearCharaInfoList;
			void ClearCharaInfoList();

			/// <summary>
			/// キャラ情報一覧を取得
			/// </summary>
			List<CharaInfo> GetCharaInfoList();

			/// <summary>
			/// キャラマスタID指定で関連するアイテム情報一覧を取得
			/// </summary>
			bool TryGetCharaInfoByMasterId(int charaMasterId, out Dictionary<ulong, CharaInfo> uuidDic);
			#endregion
		}

		/// <summary>
		/// シンクロ合成データ
		/// </summary>
		public class Model : IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				this.OnHaveMoneyChange = null;
				this.OnHaveMoneyFormatChange = null;
				this.OnNeedMoneyChange = null;
				this.OnNeedMoneyFormatChange = null;
				this.OnAddOnChargeChange = null;
				this.OnAddOnChargeFormatChange = null;
				this.OnSynchroHitPointChange = null;
				this.OnSynchroAttackChange = null;
				this.OnSynchroDefenseChange = null;
				this.OnSynchroExtraChange = null;
				this.OnSynchroRemainChange = null;
				this.OnSynchroRemainColorChange = null;
				this.OnTotalSynchroBonusChange = null;
				this.OnTotalSynchroBonusRemainChnage = null;
				this.OnSynchroFromatChnage = null;
				this.OnSynchroUpFromatChnage = null;
				this.OnSynchroMaxFromatChnage = null;
				this.OnCharaInfoListChange = null;
				this.OnClearCharaInfoList = null;

			}
			#endregion

			#region 所持金
			/// <summary>
			/// 所持金変更イベント
			/// </summary>
			public event EventHandler OnHaveMoneyChange = (sender, e) => { };
			/// <summary>
			/// 所持金
			/// </summary>
			private int _haveMoney = 0;
			public int HaveMoney
			{
				get { return _haveMoney; }
				set
				{
					if (_haveMoney != value)
					{
						_haveMoney = value;

						// 通知
						this.OnHaveMoneyChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 所持金フォーマット変更イベント
			/// </summary>
			public event EventHandler OnHaveMoneyFormatChange = (sender, e) => { };
			/// <summary>
			/// 所持金フォーマット
			/// </summary>
			private string _haveMoneyFormat = "";
			public string HaveMoneyFormat
			{
				get { return _haveMoneyFormat; }
				set
				{
					if (_haveMoneyFormat != value)
					{
						_haveMoneyFormat = value;

						// 通知
						this.OnHaveMoneyFormatChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 費用
			/// <summary>
			/// 費用変更イベント
			/// </summary>
			public event EventHandler OnNeedMoneyChange = (sender, e) => { };
			/// <summary>
			/// 費用
			/// </summary>
			private int _needMoney = 0;
			public int NeedMoney
			{
				get { return _needMoney; }
				set
				{
					if (_needMoney != value)
					{
						_needMoney = value;

						// 通知
						this.OnNeedMoneyChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 費用フォーマット変更イベント
			/// </summary>
			public event EventHandler OnNeedMoneyFormatChange = (sender, e) => { };
			/// <summary>
			/// 費用フォーマット
			/// </summary>
			private string _needMoneyFormat = "";
			public string NeedMoneyFormat
			{
				get { return _needMoneyFormat; }
				set
				{
					if (_needMoneyFormat != value)
					{
						_needMoneyFormat = value;

						// 通知
						this.OnNeedMoneyFormatChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 追加料金
			/// <summary>
			/// 追加料金変更イベント
			/// </summary>
			public event EventHandler OnAddOnChargeChange = (sender, e) => { };
			/// <summary>
			/// 追加料金
			/// </summary>
			private int _addOnCharge = 0;
			public int AddOnCharge
			{
				get { return _addOnCharge; }
				set
				{
					if (_addOnCharge != value)
					{
						_addOnCharge = value;

						// 通知
						this.OnAddOnChargeChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 追加料金フォーマット変更イベント
			/// </summary>
			public event EventHandler OnAddOnChargeFormatChange = (sender, e) => { };
			/// <summary>
			/// 追加料金フォーマット
			/// </summary>
			private string _addOnChargeFormat = "";
			public string AddOnChargeFormat
			{
				get { return _addOnChargeFormat; }
				set
				{
					if (_addOnChargeFormat != value)
					{
						_addOnChargeFormat = value;

						// 通知
						this.OnAddOnChargeFormatChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region ベースキャラス生命力
			/// <summary>
			/// 生命力シンクロボーナス変更イベント
			/// </summary>
			public event EventHandler OnSynchroHitPointChange = (sender, e) => { };
			/// <summary>
			/// 生命力シンクロボーナス
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
			#endregion

			#region ベースキャラ攻撃力
			/// <summary>
			/// 攻撃力シンクロボーナス変更イベント
			/// </summary>
			public event EventHandler OnSynchroAttackChange = (sender, e) => { };
			/// <summary>
			/// 攻撃力シンクロボーナス
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
			#endregion

			#region ベースキャラ防御力
			/// <summary>
			/// 防御力シンクロボーナス変更イベント
			/// </summary>
			public event EventHandler OnSynchroDefenseChange = (sender, e) => { };
			/// <summary>
			/// 防御力シンクロボーナス
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
			#endregion

			#region ベースキャラ特殊能力
			/// <summary>
			/// 特殊能力シンクロボーナス変更イベント
			/// </summary>
			public event EventHandler OnSynchroExtraChange = (sender, e) => { };
			/// <summary>
			/// 特殊能力シンクロボーナス
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
			#endregion

			#region ベースキャラシンクロ合成残り回数
			/// <summary>
			/// シンクロ合成残り回数変更イベント
			/// </summary>
			public event EventHandler OnSynchroRemainChange = (sender, e) => { };
			/// <summary>
			/// シンクロ合成残り回数
			/// </summary>
			private int _synchroRemain = 0;
			public int SynchroRemain
			{
				get { return _synchroRemain; }
				set
				{
					_synchroRemain = value;

					// 通知
					this.OnSynchroRemainChange(this, EventArgs.Empty);
				}
			}

			/// <summary>
			/// シンクロ合成残り回数色
			/// </summary>
			public event EventHandler OnSynchroRemainColorChange = (sender, e) => { };
			private SynchroRemainColorSettings _synchroRemainColor = null;
			public SynchroRemainColorSettings SynchroRemainColor
			{
				get { return _synchroRemainColor; }
				set
				{
					if(_synchroRemainColor != value)
					{
						_synchroRemainColor = value;

						// 通知
						this.OnSynchroRemainColorChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 合計強化量
			/// <summary>
			/// 合計強化量変更イベント
			/// </summary>
			public event EventHandler OnTotalSynchroBonusChange = (sender, e) => { };
			/// <summary>
			/// 合計強化量
			/// </summary>
			private int _totalSynchroBonus = 0;
			public int TotalSynchroBonus
			{
				get { return _totalSynchroBonus; }
				set
				{
					if (_totalSynchroBonus != value)
					{
						_totalSynchroBonus = value;

						// 通知
						this.OnTotalSynchroBonusChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 残り合計強化量変更イベント
			/// </summary>
			public event EventHandler OnTotalSynchroBonusRemainChnage = (sender, e) => { };
			/// <summary>
			/// 残り合計強化量
			/// </summary>
			private int _totalSynchroBonusRemain = 0;
			public int TotalSynchroBonusRemain
			{
				get { return _totalSynchroBonusRemain; }
				set
				{
					if(_totalSynchroBonusRemain != value)
					{
						_totalSynchroBonusRemain = value;

						// 通知
						this.OnTotalSynchroBonusRemainChnage(this, EventArgs.Empty);
					}
				}
			
			}
			#endregion

			#region シンクロ値表示フォーマット
			/// <summary>
			/// シンクロ値表示フォーマット変更イベント
			/// </summary>
			public event EventHandler OnSynchroFromatChnage = (sender, e) => { };
			/// <summary>
			/// シンクロ値表示フォーマット
			/// </summary>
			private string _synchroFormat = string.Empty;
			public string SynchroFormat
			{
				get { return _synchroFormat; }
				set
				{
					if (_synchroFormat != value)
					{
						_synchroFormat = value;

						// 通知
						this.OnSynchroFromatChnage(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region シンクロ増加値表示フォーマット
			/// <summary>
			/// シンクロ増加値表示フォーマット変更イベント
			/// </summary>
			public event EventHandler OnSynchroUpFromatChnage = (sender, e) => { };
			/// <summary>
			/// シンクロ増加値表示フォーマット
			/// </summary>
			private string _synchroUpFormat = string.Empty;
			public string SynchroUpFormat
			{
				get { return _synchroUpFormat; }
				set
				{
					if (_synchroUpFormat != value)
					{
						_synchroUpFormat = value;

						// 通知
						this.OnSynchroUpFromatChnage(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region シンクロ増加値最大時表示フォーマット
			/// <summary>
			/// シンクロ増加値最大時表示フォーマット変更イベント
			/// </summary>
			public event EventHandler OnSynchroMaxFromatChnage = (sender, e) => { };
			/// <summary>
			/// シンクロ増加値最大時表示フォーマット
			/// </summary>
			private string _synchroMaxFormat = string.Empty;
			public string SynchroMaxFormat
			{
				get { return _synchroMaxFormat; }
				set
				{
					if (_synchroMaxFormat != value)
					{
						_synchroMaxFormat = value;

						// 通知
						this.OnSynchroMaxFromatChnage(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region キャラ情報リスト
			/// <summary>
			/// キャラ情報をキャラマスターIDで区分けした一覧
			/// Key = CharaMasterID
			/// Value = UUIDで区分けした一覧
			/// </summary>
			private Dictionary<int, Dictionary<ulong, CharaInfo>> charaInfoDic = new Dictionary<int, Dictionary<ulong, CharaInfo>>();

			/// <summary>
			/// キャラ情報リストのセット
			/// </summary>
			public event EventHandler OnCharaInfoListChange = (sender, e) => { };
			public void SetCharaInfoList(List<CharaInfo> infoList)
			{
				this.charaInfoDic.Clear();
				foreach (var info in infoList)
				{
					if (!this.charaInfoDic.ContainsKey(info.CharacterMasterID))
					{
						this.charaInfoDic.Add(info.CharacterMasterID, new Dictionary<ulong, CharaInfo>());
					}

					Dictionary<ulong, CharaInfo> uuidCharaInfoDic;
					if (this.charaInfoDic.TryGetValue(info.CharacterMasterID, out uuidCharaInfoDic))
					{
						if (!uuidCharaInfoDic.ContainsKey(info.UUID))
						{
							uuidCharaInfoDic.Add(info.UUID, info);
						}
					}
				}

				// 通知
				this.OnCharaInfoListChange(this, EventArgs.Empty);
			}

			/// <summary>
			/// キャラ情報リストのクリア
			/// </summary>
			public event EventHandler OnClearCharaInfoList = (sender, e) => { };
			public void ClearCharaInfoList()
			{
				if (this.charaInfoDic.Count > 0)
				{
					this.charaInfoDic.Clear();

					// 通知
					OnClearCharaInfoList(this, EventArgs.Empty);
				}
			}

			/// <summary>
			/// キャラ情報一覧を取得
			/// </summary>
			public List<CharaInfo> GetCharaInfoList()
			{
				var infoList = new List<CharaInfo>();
				foreach (var uuidDic in this.charaInfoDic.Values)
				{
					foreach (var info in uuidDic.Values)
					{
						infoList.Add(info);
					}
				}

				return infoList;
			}

			/// <summary>
			/// キャラマスタID指定で関連するキャラ情報一覧を取得
			/// </summary>
			public bool TryGetCharaInfoByMasterId(int charaMasterId, out Dictionary<ulong, CharaInfo> uuidDic)
			{
				return this.charaInfoDic.TryGetValue(charaMasterId, out uuidDic);
			}
			#endregion
		}
	}
}
