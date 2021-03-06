/// <summary>
/// 強化合成制御
/// 
/// 2016/01/08
/// </summary>
using System;
using UnityEngine;
using System.Collections.Generic;

using Scm.Common.GameParameter;

namespace XUI
{
	namespace Powerup
	{
		public class FusionEventArgs : EventArgs
		{
			public ulong BaseCharaUUID { get; set; }
			public List<ulong> BaitCharaUUIDList { get; set; }
		}
		public class FusionCalcEventArgs : EventArgs
		{
			public ulong BaseCharaUUID { get; set; }
			public List<ulong> BaitCharaUUIDList { get; set; }
		}
		public class PlayerCharacterEventArgs : EventArgs
		{
			public ulong BaseCharaUUID { get; set; }
		}

		public static class Util
		{
			/// <summary>
			/// 線形補間の媒介変数を取得する
			/// </summary>
			public static float GetLerpValue(float min, float max, float now)
			{
				var t1 = now - min;
				var t2 = max != 0f ? max - min : 0f;
				if (t2 == 0f)
				{
					return 0f;
				}
				return t1 / t2;
			}
		}

		/// <summary>
		/// ベース情報モード
		/// </summary>
		public enum BaseInfoMode : byte
		{
			None,	// なし
			Before,	// 合成前の情報
			After,	// 合成後の情報
		}

		/// <summary>
		/// 選択モード
		/// </summary>
		public enum SelectMode : byte
		{
			Base,	// ベースキャラ選択中
			Bait,	// 餌キャラ選択中
		}

		/// <summary>
		/// 強化合成制御インターフェイス
		/// </summary>
		public interface IController
		{
			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }

			/// <summary>
			/// アクティブ設定
			/// </summary>
			void SetActive(bool isActive, bool isTweenSkip);

			/// <summary>
			/// 初期化
			/// </summary>
			void Setup();

			/// <summary>
			/// ステータス情報設定
			/// </summary>
			void SetupStatusInfo(int haveMoney);

			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();
			#endregion

			/// <summary>
			/// キャラリスト枠を設定
			/// </summary>
			void SetupCapacity(int capacity, int count);

			/// <summary>
			/// キャラリストの中身設定
			/// </summary>
			void SetupItem(List<CharaInfo> list);

			#region プレイヤーキャラクター情報取得イベント
			/// <summary>
			/// プレイヤーキャラクター情報取得イベント
			/// </summary>
			event EventHandler<PlayerCharacterEventArgs> OnPlayerCharacter;

			/// <summary>
			/// プレイヤーキャラクター情報取得結果
			/// </summary>
			void PlayerCharacterResult(CharaInfo info, List<CharaInfo> slotList);
			#endregion

			/// <summary>
			/// 合成イベント
			/// </summary>
			event EventHandler<FusionEventArgs> OnFusion;
			/// <summary>
			/// 合成結果
			/// </summary>
			void FusionResult(Scm.Common.GameParameter.PowerupResult result, int money, int price, int addOnCharge, CharaInfo info);

			/// <summary>
			/// 合成試算イベント
			/// </summary>
			event EventHandler<FusionCalcEventArgs> OnFusionCalc;
			/// <summary>
			/// 合成試算結果
			/// </summary>
			void FusionCalcResult(int exp, int money, int price, int addOnCharge);
		}

		/// <summary>
		/// 強化合成制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド＆プロパティ
			/// <summary>
			/// 高ランク
			/// </summary>
			const int HighRank = 4;

			/// <summary>
			/// 合成イベント
			/// </summary>
			public event EventHandler<FusionEventArgs> OnFusion = (sender, e) => { };

			/// <summary>
			/// 合成試算イベント
			/// </summary>
			public event EventHandler<FusionCalcEventArgs> OnFusionCalc = (sender, e) => { };

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

			// キャラリスト
			readonly GUICharaPageList _charaList;
			GUICharaPageList CharaList { get { return _charaList; } }

			// ベースキャラ
			readonly GUICharaItem _baseChara;
			GUICharaItem BaseChara { get { return _baseChara; } }
			// ベースキャラUUID
			ulong BaseCharaUUID
			{
				get
				{
					ulong uuid = 0;
					var info = this.BaseCharaInfo;
					if (info != null) uuid = info.UUID;
					return uuid;
				}
			}
			// ベースキャラのキャラ情報
			CharaInfo BaseCharaInfo { get { return (this.BaseChara != null ? this.BaseChara.GetCharaInfo() : null); } }
			// ベースキャラが空かどうか
			bool IsEmptyBaseChara { get { return (this.BaseCharaInfo == null ? true : false); } }

			// 餌リスト
			readonly GUISelectCharaList _baitList;
			GUISelectCharaList BaitList { get { return _baitList; } }
			// 餌キャラUUIDリスト
			List<ulong> BaitCharaUUIDList
			{
				get
				{
					var list = new List<ulong>();
					if (this.BaitList != null)
					{
						var infoList = this.BaitList.GetCharaInfoList();
						if (infoList != null)
						{
							infoList.ForEach(t => { list.Add(t.UUID); });
						}
					}
					return list;
				}
			}

			// 現在の選択モード
			SelectMode SelectMode { get { return this.IsEmptyBaseChara ? SelectMode.Base : SelectMode.Bait; } }

			// ベースキャラが同期できるかどうか
			bool CanSyncBaseChara { get; set; }

			// シリアライズされていないメンバーの初期化
			void MemberInit()
			{
				this.CanSyncBaseChara = true;
			}
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public Controller(IModel model, IView view, GUICharaPageList charaList, GUICharaItem baseChara, GUISelectCharaList baitList)
			{
				if (model == null || view == null) return;

				this._charaList = charaList;
				this._baseChara = baseChara;
				this._baitList = baitList;

				this.MemberInit();

				// ビュー設定
				this._view = view;
				this.View.OnHome += this.HandleHome;
				this.View.OnClose += this.HandleClose;
				this.View.OnFusion += this.HandleFusion;
				this.View.OnAllClear += this.HandleAllClear;

				// モデル設定
				this._model = model;
				// 獲得経験値
				this.Model.OnTakeExpChange += this.HandleTakeExpChange;
				this.Model.OnTakeExpFormatChange += this.HandleTakeExpFormatChange;
				// 経験値
				this.Model.OnExpChange += this.HandleExpChange;
				this.Model.OnExpFormatChange += this.HandleExpFormatChange;
				// 合成前のレベルデータ
				this.Model.OnBeforeLvDataChange += this.HandleBeforeLvDataChange;
				this.Model.OnLvFormatChange += this.HandleLvFormatChange;
				this.Model.OnTotalExpFormatChange += this.HandleTotalExpFormatChange;
				this.Model.OnNextLvTotalExpFormatChange += this.HandleNextLvTotalExpFormatChange;
				this.Model.OnNextLvExpFormatChange += this.HandleNextLvExpFormatChange;
				// 合成後の経験値
				this.Model.OnAfterExpChange += this.HandleAfterExpChange;
				this.Model.OnAfterExpFormatChange += this.HandleAfterExpFormatChange;
				// 合成後のレベルデータ
				this.Model.OnAfterLvDataChange += this.HandleAfterLvDataChange;
				this.Model.OnAfterLvFormatChange += this.HandleAfterLvFormatChange;
				this.Model.OnAfterTotalExpFormatChange += this.HandleAfterTotalExpFormatChange;
				this.Model.OnAfterNextLvTotalExpFormatChange += this.HandleAfterNextLvTotalExpFormatChange;
				this.Model.OnAfterNextLvExpFormatChange += this.HandleAfterNextLvExpFormatChange;
				this.Model.OnAfterOverflowExpChange += this.HandleAfterOverflowExpChange;
				this.Model.OnAfterOverflowExpFormatChange += this.HandleAfterOverflowExpFormatChange;
				// 所持金
				this.Model.OnHaveMoneyChange += this.HandleHaveMoneyChange;
				this.Model.OnHaveMoneyFormatChange += this.HandleHaveMoneyFormatChange;
				// 費用
				this.Model.OnNeedMoneyChange += this.HandleNeedMoneyChange;
				this.Model.OnNeedMoneyFormatChange += this.HandleNeedMoneyFormatChange;
				// 追加料金
				this.Model.OnAddOnChargeChange += this.HandleAddOnChargeChange;
				this.Model.OnAddOnChargeFormatChange += this.HandleAddOnChargeFormatChange;

				// 同期
				this.SyncTakeExp();
				this.SyncExp();
				this.SyncBeforeLvData();
				this.SyncAfterExp();
				this.SyncAfterLvData();
				this.SyncHaveMoney();
				this.SyncNeedMoney();

				// イベント登録
				if (this.CharaList != null)
				{
					this.CharaList.OnItemChangeEvent += this.HandleCharaListItemChangeEvent;
					this.CharaList.OnItemClickEvent += this.HandleCharaListItemClickEvent;
					this.CharaList.OnItemLongPressEvent += this.HandleCharaListItemLongPressEvent;
					this.CharaList.OnUpdateItemsEvent += this.HandleCharaListUpdateItemsEvent;
				}
				if (this.BaseChara != null)
				{
					this.BaseChara.OnItemChangeEvent += this.HandleBeseCharaItemChangeEvent;
					this.BaseChara.OnItemClickEvent += this.HandleBaseCharaItemClickEvent;
					this.BaseChara.OnItemLongPressEvent += this.HandleBaseCharaItemLongPressEvent;
				}
				if (this.BaitList != null)
				{
					this.BaitList.OnItemChangeEvent += this.HandleBaitListItemChangeEvent;
					this.BaitList.OnItemClickEvent += this.HandleBaitListItemClickEvent;
					this.BaitList.OnItemLongPressEvent += this.HandleBaitListItemLongPressEvent;
				}
				// 同期
				this.SyncBaseChara();
				this.SyncBaitCharaList();
				this.SyncCharaList();
			}
			/// <summary>
			/// 初期化
			/// </summary>
			public void Setup()
			{
				this.ClearCharaList();
				this.ClearBaseChara();
				this.ClearBaitList();
				// 同期
				this.SyncBaseChara();
				this.SyncBaitCharaList();
				this.SyncCharaList();
			}
			/// <summary>
			/// ステータス情報設定
			/// </summary>
			public void SetupStatusInfo(int haveMoney)
			{
				if (this.CanUpdate)
				{
					this.Model.HaveMoney = haveMoney;
				}
			}
			#endregion

			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				if (this.CanUpdate)
				{
					this.Model.Dispose();
				}

				this.OnPlayerCharacter = null;
				this.OnFusion = null;
				this.OnFusionCalc = null;
			}
			#endregion

			#region アクティブ設定
			/// <summary>
			/// アクティブ設定
			/// </summary>
			public void SetActive(bool isActive, bool isTweenSkip)
			{
				if (this.CanUpdate)
				{
					this.View.SetActive(isActive, isTweenSkip);

					// その他UIの表示設定
					GUILobbyResident.SetActive(!isActive);
					GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX272_Powerup_ScreenTitle));
					// ヘルプメッセージの状態を更新する
					this.UpdateHelpMessage();
				}
			}
			#endregion

			#region 各状態を更新する
			/// <summary>
			/// 合成ボタンの状態を更新する
			/// </summary>
			void UpdateFusionButtonEnable()
			{
				if (!this.CanUpdate) return;

				bool isEnable = true;

				// 餌キャラが選択されていない
				var list = this.BaitCharaUUIDList;
				if (list.Count <= 0) isEnable = false;
				// 所持金が足りない
				else if (this.Model.HaveMoney < this.Model.NeedMoney) isEnable = false;

				this.View.SetFusionButtonEnable(isEnable);
			}
			/// <summary>
			/// 全てを外すボタンの状態を更新する
			/// </summary>
			void UpdateAllClearButtonEnable()
			{
				if (!this.CanUpdate) return;

				bool isEnable = true;

				// 餌キャラが選択されていない
				var list = this.BaitCharaUUIDList;
				if (list.Count <= 0) isEnable = false;

				this.View.SetAllClearButtonEnable(isEnable);
			}
			/// <summary>
			/// ヘルプメッセージの状態を更新する
			/// </summary>
			void UpdateHelpMessage()
			{
				if (!this.CanUpdate) return;

				var state = this.View.GetActiveState();
				var isActive = state == GUIViewBase.ActiveState.Opened || state == GUIViewBase.ActiveState.Opening;
				if (!isActive)
				{
					GUIHelpMessage.Play(false);
					return;
				}

				switch (this.SelectMode)
				{
					case SelectMode.Base:
						// ベースキャラを選択中
						GUIHelpMessage.Play(true, MasterData.GetText(TextType.TX273_Powerup_Base_HelpMessage));
						break;
					case SelectMode.Bait:
						// 素材キャラを選択中
						GUIHelpMessage.Play(true, MasterData.GetText(TextType.TX274_Powerup_Bait_HelpMessage));
						break;
					default:
						GUIHelpMessage.Play(true);
						break;
				}
			}
			/// <summary>
			/// 素材フィルターを更新する
			/// </summary>
			void UpdateBaitFill()
			{
				if (!this.CanUpdate) return;

				switch (this.SelectMode)
				{
					case SelectMode.Base:
						this.View.SetBaitFillActive(true);
						break;
					case SelectMode.Bait:
						this.View.SetBaitFillActive(false);
						break;
				}
			}
			/// <summary>
			/// 選択枠を更新する
			/// </summary>
			void UpdateSelectFrame()
			{
				if (this.BaseChara == null) return;
				if (this.BaitList == null) return;

				bool isBaseSelect = false;
				int baitSelectIndex = -1;
				switch (this.SelectMode)
				{
					case SelectMode.Base:
						isBaseSelect = true;
						break;
					case SelectMode.Bait:
						baitSelectIndex = this.BaitCharaUUIDList.Count;
						break;
				}

				this.BaseChara.SetSelect(isBaseSelect);

				var list = this.BaitList.GetNowPageItemList();
				for (int i = 0, max = list.Count; i < max; i++)
				{
					bool isSelect = i == baitSelectIndex;
					list[i].SetSelect(isSelect);
				}
			}
			#endregion

			#region 表示直結系
			#region 獲得経験値
			void HandleTakeExpChange(object sender, TakeExpChangeEventArgs e) { this.SyncTakeExp(); }
			void HandleTakeExpFormatChange(object sender, TakeExpFormatChangeEventArgs e) { this.SyncTakeExp(); }
			void SyncTakeExp()
			{
				if (this.CanUpdate)
				{
					this.View.SetTakeExp(this.Model.TakeExp, this.Model.TakeExpFormat);

					if (this.Model.TakeExp > 0)
					{
						this.View.SetTakeExpColor(StatusColor.Type.Up);
					}
					else if (this.Model.TakeExp < 0)
					{
						this.View.SetTakeExpColor(StatusColor.Type.Down);
					}
					else
					{
						this.View.SetTakeExpColor(StatusColor.Type.StatusNormal);
					}
				}
				this.SyncNextLvExp();
				this.SyncAfterNextLvExp();
			}
			#endregion

			#region 経験値
			void HandleExpChange(object sender, ExpChangeEventArgs e) { this.SyncExp(); }
			void HandleExpFormatChange(object sender, ExpFormatChangeEventArgs e) { this.SyncExp(); }
			void SyncExp()
			{
				if (this.CanUpdate)
				{
					this.View.SetExp(this.Model.Exp, this.Model.ExpFormat);
				}
			}
			#endregion

			#region 合成前のレベルデータ
			void HandleBeforeLvDataChange(object sender, BeforeLvDataChangeEventArgs e) { this.SyncBeforeLvData(); }
			void SyncBeforeLvData()
			{
				this.SyncLv();
				this.SyncTotalExp();
				this.SyncNextLvTotalExp();
				this.SyncNextLvExp();
			}
			#endregion

			#region レベル
			void HandleLvFormatChange(object sender, LvFormatChangeEventArgs e) { this.SyncLv(); }
			void SyncLv()
			{
				if (this.CanUpdate)
				{
					this.View.SetLv(this.Model.Lv, this.Model.LvFormat);
				}
			}
			#endregion

			#region 現在のレベルになる為の累積経験値
			void HandleTotalExpFormatChange(object sender, TotalExpFormatChangeEventArgs e) { this.SyncTotalExp(); }
			void SyncTotalExp()
			{
			}
			#endregion

			#region 次のレベルになる為の累積経験値
			void HandleNextLvTotalExpFormatChange(object sender, NextLvTotalExpFormatChangeEventArgs e) { this.SyncNextLvTotalExp();}
			void SyncNextLvTotalExp()
			{
			}
			#endregion

			#region 次のレベルまでの経験値
			void HandleNextLvExpFormatChange(object sender, NextLvExpFormatChangeEventArgs e) { this.SyncNextLvExp(); }
			void SyncNextLvExp()
			{
				if (this.CanUpdate)
				{
					this.View.SetNextLvExp(this.Model.GetNextLvExp(), this.Model.NextLvExpFormat);
				}
			}
			#endregion

			#region 合成後の経験値
			void HandleAfterExpChange(object sender, AfterExpChangeEventArgs e) { this.SyncAfterExp(); }
			void HandleAfterExpFormatChange(object sender, AfterExpFormatChangeEventArgs e) { this.SyncAfterExp(); }
			void SyncAfterExp()
			{
				if (this.CanUpdate)
				{
					this.View.SetAfterExp(this.Model.AfterExp, this.Model.AfterExpFormat);

					if (this.Model.AfterExp > this.Model.Exp)
					{
						this.View.SetAfterExpColor(StatusColor.Type.Up);
					}
					else if (this.Model.AfterExp < this.Model.Exp)
					{
						this.View.SetAfterExpColor(StatusColor.Type.Down);
					}
					else
					{
						this.View.SetAfterExpColor(StatusColor.Type.StatusNormal);
					}
				}
			}
			#endregion

			#region 合成後のレベルデータ
			void HandleAfterLvDataChange(object sender, AfterLvDataChangeEventArgs e) { this.SyncAfterLvData(); }
			void SyncAfterLvData()
			{
				this.SyncAfterLv();
				this.SyncAfterTotalExp();
				this.SyncAfterNextLvTotalExp();
				this.SyncAfterNextLvExp();
				this.SyncAfterOverflowExp();
			}
			#endregion

			#region 合成後のレベル
			void HandleAfterLvFormatChange(object sender, AfterLvFormatChangeEventArgs e) { this.SyncAfterLv(); }
			void SyncAfterLv()
			{
				if (this.CanUpdate)
				{
					this.View.SetAfterLv(this.Model.AfterLv, this.Model.AfterLvFormat);

					if (this.Model.AfterLv > this.Model.Lv)
					{
						this.View.SetAfterLvColor(StatusColor.Type.Up);
					}
					else if (this.Model.AfterLv < this.Model.Lv)
					{
						this.View.SetAfterLvColor(StatusColor.Type.Down);
					}
					else
					{
						this.View.SetAfterLvColor(StatusColor.Type.StatusNormal);
					}
				}
			}
			#endregion

			#region 合成後のレベルになる為の累積経験値
			void HandleAfterTotalExpFormatChange(object sender, AfterTotalExpFormatChangeEventArgs e) { this.SyncAfterTotalExp(); }
			void SyncAfterTotalExp()
			{
			}
			#endregion

			#region 合成後の次のレベルになる為の累積経験値
			void HandleAfterNextLvTotalExpFormatChange(object sender, AfterNextLvTotalExpFormatChangeEventArgs e) { this.SyncAfterNextLvTotalExp(); }
			void SyncAfterNextLvTotalExp()
			{
			}
			#endregion

			#region 合成後の次のレベルまでの経験値
			void HandleAfterNextLvExpFormatChange(object sender, AfterNextLvExpFormatChangeEventArgs e) { this.SyncAfterNextLvExp(); }
			void SyncAfterNextLvExp()
			{
				if (this.CanUpdate)
				{
					this.View.SetAfterNextLvExp(this.Model.GetAfterNextLvExp(), this.Model.AfterNextLvExpFormat);
				}
			}
			#endregion

			#region 合成後の余剰経験値
			void HandleAfterOverflowExpChange(object sender, AfterOverflowExpChangeEventArgs e) { this.SyncAfterOverflowExp(); }
			void HandleAfterOverflowExpFormatChange(object sender, AfterOverflowExpFormatChangeEventArgs e) { this.SyncAfterOverflowExp(); }
			void SyncAfterOverflowExp()
			{
				if (this.CanUpdate)
				{
					this.View.SetAfterOverflowExp(this.Model.AfterOverflowExp, this.Model.AfterOverflowExpFormat);
				}
			}
			#endregion

			#region 所持金
			void HandleHaveMoneyChange(object sender, HaveMoneyChangeEventArgs e) { this.SyncHaveMoney(); }
			void HandleHaveMoneyFormatChange(object sender, HaveMoneyFormatChangeEventArgs e) { this.SyncHaveMoney(); }
			void SyncHaveMoney()
			{
				if (this.CanUpdate)
				{
					this.View.SetHaveMoney(this.Model.HaveMoney, this.Model.HaveMoneyFormat);
				}
			}
			#endregion

			#region 費用
			void HandleNeedMoneyChange(object sender, NeedMoneyChangeEventArgs e) { this.SyncNeedMoney(); }
			void HandleNeedMoneyFormatChange(object sender, NeedMoneyFormatChangeEventArgs e) { this.SyncNeedMoney(); }
			void SyncNeedMoney()
			{
				if (this.CanUpdate)
				{
					this.View.SetNeedMoney(this.Model.NeedMoney, this.Model.NeedMoneyFormat);
				}
			}
			#endregion

			#region 追加料金
			void HandleAddOnChargeChange(object sender, AddOnChargeChangeEventArgs e) { this.SyncAddOnCharge(); }
			void HandleAddOnChargeFormatChange(object sender, AddOnChargeFormatChangeEventArgs e) { this.SyncAddOnCharge(); }
			void SyncAddOnCharge()
			{
			}
			#endregion
			#endregion

			#region キャラリスト設定
			#region キャラアイテム操作
			void HandleCharaListItemChangeEvent(GUICharaItem obj) { this.UpdateItemDisableType(obj); }
			void HandleCharaListItemClickEvent(GUICharaItem obj) { this.SwitchOperationItem(obj); }
			void HandleCharaListItemLongPressEvent(GUICharaItem obj) { }
			void HandleCharaListUpdateItemsEvent()
			{
				if (this.CharaList != null)
				{
					var list = this.CharaList.GetNowPageItemList();
					list.ForEach(this.UpdateItemDisableType);
				}
			}
			/// <summary>
			/// キャラリストを同期
			/// </summary>
			void SyncCharaList()
			{
				if (this.CharaList == null) return;

				// 各キャラ情報が選択できるか設定する
				var list = this.CharaList.GetCharaInfo();
				this.UpdateCanSelect(list);

				// キャラリスト更新
				this.CharaList.SetupItems(list);
			}

			/// <summary>
			/// キャラリスト枠を設定
			/// </summary>
			public void SetupCapacity(int capacity, int count)
			{
				if (this.CharaList != null)
				{
					this.CharaList.SetupCapacity(capacity, count);
				}
			}
			/// <summary>
			/// キャラリストの中身設定
			/// </summary>
			public void SetupItem(List<CharaInfo> list)
			{
				// 選択できるかどうかの情報を更新する
				this.UpdateCanSelect(list);

				// リストからベースキャラのキャラ情報を更新する
				this.UpdateBaseChara(list);
				// リストから餌リストのキャラ情報を更新する
				this.UpdateBaitList(list);

				// キャラリストの中身を更新する
				if (this.CharaList != null)
				{
					this.CharaList.SetupItems(list);
				}
			}
			/// <summary>
			/// キャラリストからベースキャラの中身を更新する
			/// </summary>
			void UpdateBaseChara(List<CharaInfo> list)
			{
				if (list == null) return;
				if (this.BaseChara == null) return;

				var info = this.BaseChara.GetCharaInfo();
				if (info == null) return;

				// ベースキャラのキャラ情報を更新する
				var newInfo = list.Find(t => { return t.UUID == info.UUID; });
				this.SetCharaItem(this.BaseChara, newInfo);
			}
			/// <summary>
			/// キャラリストから餌リストの中身を更新する
			/// </summary>
			void UpdateBaitList(List<CharaInfo> list)
			{
				if (list == null) return;
				if (this.BaitList == null) return;

				var uuidList = this.BaitCharaUUIDList;
				if (uuidList.Count <= 0) return;

				// 新しい餌リスト初期化
				var newInfoList = new List<CharaInfo>(uuidList.Count);
				uuidList.ForEach(t => { newInfoList.Add(null); });

				// 新しい餌リストを抽出する
				var count = 0;
				foreach (var info in list)
				{
					if (info == null) continue;
					var index = uuidList.FindIndex(t => { return t == info.UUID; });
					if (index == -1) continue;

					newInfoList[index] = info;
					count++;
					if (count >= uuidList.Count)
					{
						// 抽出完了
						break;
					}
				}

				// 餌リストのキャラ情報を更新する
				this.ClearBaitList();
				newInfoList.ForEach(info => { this.AddBaitChara(info); });
			}
			/// <summary>
			/// 現在の状態によってアイテムの処理を分岐させる
			/// </summary>
			void SwitchOperationItem(GUICharaItem item)
			{
				if (item == null) return;

				var disableState = item.GetDisableState();
				var info = item.GetCharaInfo();
				switch (disableState)
				{
					case CharaItem.Controller.DisableType.None:
						if (this.SelectMode == SelectMode.Base)
						{
							// ベースキャラ設定
							this.SetBaseChara(info);
						}
						else
						{
							// 餌追加
							this.AddBaitChara(info);
						}
						break;
					case CharaItem.Controller.DisableType.Base:
						// ベースキャラクリア
						this.ClearBaseChara();
						break;
					case CharaItem.Controller.DisableType.Bait:
						// 餌削除
						this.RemoveBaitChara(info);
						break;
					default:
						break;
				}
			}
			/// <summary>
			/// キャラリストクリア
			/// </summary>
			void ClearCharaList()
			{
				if (this.CharaList != null)
				{
					this.CharaList.SetupItems(null);
				}
			}
			#endregion

			#region キャラ情報
			/// <summary>
			/// 選択できるかどうかの情報を更新する
			/// </summary>
			void UpdateCanSelect(List<CharaInfo> list)
			{
				if (list == null) return;

				// 各キャラ情報の選択出来るかどうかの情報を更新する
				list.ForEach(this.UpdateCanSelect);
			}
			/// <summary>
			/// 選択できるかどうかの情報を更新する
			/// </summary>
			void UpdateCanSelect(CharaInfo info)
			{
				if (info == null) return;

				// 無効タイプを取得する
				var disableType = CharaItem.Controller.DisableType.None;
				var baitIndex = -1;
				switch (this.SelectMode)
				{
					case SelectMode.Base:
						this.GetBaseCharaDisableType(info, out disableType);
						break;
					case SelectMode.Bait:
						this.GetBaitCharaDisableType(info, out disableType, out baitIndex);
						break;
				}

				// 無効タイプから選択できるか設定する
				var canSelect = false;
				switch (disableType)
				{
				case CharaItem.Controller.DisableType.None:
				case CharaItem.Controller.DisableType.Base:
				case CharaItem.Controller.DisableType.Bait:
					canSelect = true;
					break;
				}
				info.CanSelect = canSelect;
			}
			/// <summary>
			/// ベースキャラ選択時の無効タイプを取得する
			/// </summary>
			void GetBaseCharaDisableType(CharaInfo info, out CharaItem.Controller.DisableType disableType)
			{
				disableType = XUI.CharaItem.Controller.DisableType.None;
				if (info == null) return;

				// 以下無効にするかチェック
				// スロットに入っている
				if (info.IsInSlot) disableType = XUI.CharaItem.Controller.DisableType.PowerupSlot;
				// レベル最大
				else if (CharaInfo.IsMaxLevel(info.Rank, info.PowerupLevel)) disableType = XUI.CharaItem.Controller.DisableType.PowerupLevelMax;
			}
			/// <summary>
			/// 餌キャラ選択時の無効タイプを取得する
			/// </summary>
			void GetBaitCharaDisableType(CharaInfo info, out CharaItem.Controller.DisableType disableType, out int baitIndex)
			{
				disableType = XUI.CharaItem.Controller.DisableType.None;
				baitIndex = -1;
				if (info == null) return;

				// 以下無効にするかチェック
				// 優先順位があるので注意
				var baitUUIDList = this.BaitCharaUUIDList;
				baitIndex = baitUUIDList.FindIndex((uuid) => { return uuid == info.UUID; });
				if (info.UUID == this.BaseCharaUUID)
				{
					// ベースキャラ選択中
					disableType = CharaItem.Controller.DisableType.Base;
				}
				else if (baitIndex >= 0)
				{
					// 素材キャラ選択中
					disableType = CharaItem.Controller.DisableType.Bait;
					return;
				}
				// スロットに入っている
				else if (info.IsInSlot) disableType = CharaItem.Controller.DisableType.PowerupSlot;
				// デッキに入っている
				else if (info.IsInDeck) disableType = CharaItem.Controller.DisableType.Deck;
				// シンボルに設定している
				else if (info.IsSymbol) disableType = CharaItem.Controller.DisableType.Symbol;
				// ロック中
				else if (info.IsLock) disableType = CharaItem.Controller.DisableType.Lock;
			}
			#endregion

			#region キャラアイテム
			/// <summary>
			/// アイテムの無効タイプ更新
			/// </summary>
			void UpdateItemDisableType(GUICharaItem item)
			{
				if (item == null) return;

				// 無効タイプを取得する
				var disableType = CharaItem.Controller.DisableType.None;
				var baitIndex = -1;
				switch (this.SelectMode)
				{
					case SelectMode.Base:
						this.GetBaseCharaDisableType(item.GetCharaInfo(), out disableType);
						break;
					case SelectMode.Bait:
						this.GetBaitCharaDisableType(item.GetCharaInfo(), out disableType, out baitIndex);
						break;
				}

				// 無効タイプを設定する
				if (baitIndex >= 0)
				{
					// 餌のインデックスがある場合は餌
					item.SetBaitState(baitIndex);
				}
				else
				{
					// それ以外は無効タイプを設定する
					item.SetDisableState(disableType);
				}
			}
			/// <summary>
			/// キャラアイテムを設定する
			/// </summary>
			void SetCharaItem(GUICharaItem item, CharaInfo info)
			{
				if (item == null) return;

				var state = CharaItem.Controller.ItemStateType.Icon;
				if (info == null || info.IsEmpty)
				{
					state = CharaItem.Controller.ItemStateType.FillEmpty;
				}
				item.SetState(state, info);
			}
			#endregion
			#endregion

			#region ベースキャラ設定
			void HandleBeseCharaItemChangeEvent(GUICharaItem obj) { this.SyncBaseChara(); this.SyncCharaList(); }
			void HandleBaseCharaItemClickEvent(GUICharaItem obj) { this.ClearBaseChara(); }
			void HandleBaseCharaItemLongPressEvent(GUICharaItem obj) { }
			/// <summary>
			/// ベースキャラ設定
			/// </summary>
			void SetBaseChara(CharaInfo info)
			{
				if (this.BaseChara != null)
				{
					this.SetCharaItem(this.BaseChara, info);
				}
			}
			/// <summary>
			/// ベースキャラを外す
			/// </summary>
			void ClearBaseChara()
			{
				this.SetBaseChara(null);
			}
			/// <summary>
			/// ベースキャラを同期
			/// </summary>
			void SyncBaseChara()
			{
				if (!this.CanSyncBaseChara) return;

				if (this.IsEmptyBaseChara)
				{
					// 餌キャラを外す
					this.ClearBaitList();
				}

				// ベースキャラ情報更新
				if (this.CanUpdate)
				{
					// ベースキャラのデータ更新
					int lv = 0, exp = 0, totalExp = 0, nextLvTotalExp = 0;
					var info = this.BaseCharaInfo;
					if (info != null)
					{
						lv = info.PowerupLevel;
						exp = info.PowerupExp;
						// 現在のレベルになる為の累積経験値を取得する
						totalExp = CharaInfo.GetTotalExp(info.Rank, info.PowerupLevel);
						if (!CharaInfo.IsMaxLevel(info.Rank, info.PowerupLevel))
						{
							nextLvTotalExp = CharaInfo.GetTotalExp(info.Rank, info.PowerupLevel + 1);
						}
					}
					this.Model.Exp = exp;
					this.Model.SetBeforeLvData(lv, totalExp, nextLvTotalExp);
					this.SetFusionCalcResult(lv, totalExp, nextLvTotalExp, exp, 0, 0, 0, 0);

					// データを元にUI更新
					var sliderValue = this.Model.GetExpSliderValue();
					this.View.SetExpSlider(sliderValue);
					this.View.SetAfterExpSlider(sliderValue, sliderValue);
					// 合成ボタンの状態を更新する
					this.UpdateFusionButtonEnable();
					// ヘルプメッセージの状態を更新する
					this.UpdateHelpMessage();
				}

				// 素材フィルターを更新する
				this.UpdateBaitFill();
				// 選択枠を更新する
				this.UpdateSelectFrame();
			}
			#endregion

			#region 餌リスト設定
			void HandleBaitListItemChangeEvent(GUICharaItem obj) { this.SyncBaitCharaList(); this.SyncCharaList(); }
			void HandleBaitListItemClickEvent(GUICharaItem obj) { this.RemoveBaitChara(obj != null ? obj.GetCharaInfo() : null); }
			void HandleBaitListItemLongPressEvent(GUICharaItem obj) { }
			/// <summary>
			/// 餌キャラ追加
			/// </summary>
			void AddBaitChara(CharaInfo info)
			{
				if (this.BaitList != null)
				{
					this.BaitList.AddChara(info);
				}
				// 全てを外すボタンの状態を更新する
				this.UpdateAllClearButtonEnable();
			}
			/// <summary>
			/// 餌キャラ削除
			/// </summary>
			void RemoveBaitChara(CharaInfo info)
			{
				if (this.BaitList != null)
				{
					this.BaitList.RemoveChara(info);
				}
				// 全てを外すボタンの状態を更新する
				this.UpdateAllClearButtonEnable();
			}
			/// <summary>
			/// 餌リストクリア
			/// </summary>
			void ClearBaitList()
			{
				if (this.BaitList != null)
				{
					this.BaitList.ClearChara();
				}
				// 全てを外すボタンの状態を更新する
				this.UpdateAllClearButtonEnable();
			}
			/// <summary>
			/// 餌リストを同期
			/// </summary>
			void SyncBaitCharaList()
			{
				if (this.BaitList != null && this.CanUpdate)
				{
					var list = this.BaitList.GetCharaInfoList();
					if (list.Count > 0)
					{
						// 餌が一つでも選択されている場合は
						// 合成後の情報にする
						this.View.SetBaseInfoMode(BaseInfoMode.After);
					}
					else
					{
						// 餌が一つも選択されていない場合は
						// 合成前の情報にする
						this.View.SetBaseInfoMode(BaseInfoMode.Before);
					}
				}

				// 合成試算
				this.FusionCalc();
				// 素材フィルターを更新する
				this.UpdateBaitFill();
				// 選択枠を更新する
				this.UpdateSelectFrame();
			}
			/// <summary>
			/// 合成試算
			/// </summary>
			void FusionCalc()
			{
				if (this.BaseChara == null) return;
				if (this.BaitList == null) return;

				var eventArgs = new FusionCalcEventArgs();
				eventArgs.BaseCharaUUID = this.BaseCharaUUID;
				eventArgs.BaitCharaUUIDList = this.BaitCharaUUIDList;
				if (eventArgs.BaitCharaUUIDList.Count > 0)
				{
					// 通知
					this.OnFusionCalc(this, eventArgs);
				}
				else
				{
					// データを空にする
					int lv = 0, exp = 0, totalExp = 0, nextLvTotalExp = 0;
					float sliderValue = 0f;
					if (this.CanUpdate)
					{
						lv = this.Model.Lv;
						exp = this.Model.Exp;
						totalExp = this.Model.TotalExp;
						nextLvTotalExp = this.Model.NextLvTotalExp;
						sliderValue = this.Model.GetExpSliderValue();
					}
					this.SetFusionCalcResult(lv, totalExp, nextLvTotalExp, exp, 0, 0, 0, 0);

					// データを元にUI更新
					this.View.SetLvMax(false);
					this.View.SetAfterExpSlider(sliderValue, sliderValue);
					// 合成ボタンの状態を更新する
					this.UpdateFusionButtonEnable();
				}
			}
			/// <summary>
			/// 合成試算結果
			/// </summary>
			public void FusionCalcResult(int exp, int money, int price, int addOnCharge)
			{
				var list = this.BaitCharaUUIDList;
				if (list.Count <= 0) return;

				// 合成後のデータを設定する
				int takeExp = 0, afterExp = 0, afterLv = 0, afterTotalExp = 0, afterNextLvTotalExp = 0, afterOverflowExp = 0;
				bool isLvMax = false;
				if (this.CanUpdate)
				{
					takeExp = exp - this.Model.Exp;
					afterExp = this.Model.Exp + takeExp;

					var info = this.BaseCharaInfo;
					if (info != null)
					{
						afterLv = GetLvFromExp(info.Rank, afterExp);
						// 合成後のレベルになる為の累積経験値を取得する
						afterTotalExp = CharaInfo.GetTotalExp(info.Rank, afterLv);
						isLvMax = CharaInfo.IsMaxLevel(info.Rank, afterLv);
						if (!isLvMax)
						{
							afterNextLvTotalExp = CharaInfo.GetTotalExp(info.Rank, afterLv + 1);
						}
						else
						{
							afterOverflowExp = exp - afterTotalExp;
							// 最大レベルなので累積経験値をクランプする
							afterExp = Mathf.Min(afterExp, afterTotalExp);
						}
					}
				}
				this.SetFusionCalcResult(afterLv, afterTotalExp, afterNextLvTotalExp, afterExp, afterOverflowExp, takeExp, price, addOnCharge);

				// データを元にUI更新
				if (this.CanUpdate)
				{
					this.View.SetLvMax(isLvMax);
					this.View.SetAfterExpSlider(this.Model.GetAfterExpSliderValue(), this.Model.GetAfterTakeExpSliderValue());
				}
				// 合成ボタンの状態を更新する
				this.UpdateFusionButtonEnable();
			}
			/// <summary>
			/// 合成後のデータを設定する
			/// </summary>
			void SetFusionCalcResult(int afterLv, int afterTotalExp, int afterNextLvTotalExp, int afterExp, int afterOverflowExp, int takeExp, int price, int addOnCharge)
			{
				if (this.CanUpdate)
				{
					this.Model.AfterExp = afterExp;
					this.Model.AfterOverflowExp = afterOverflowExp;
					this.Model.SetAfterLvData(afterLv, afterTotalExp, afterNextLvTotalExp);
					// 獲得経験値は経験値系のデータを設定した後にしないと
					// 次のレベルまでの経験値が古いデータで計算してしまう
					this.Model.TakeExp = takeExp;
					this.Model.NeedMoney = price;
					this.Model.AddOnCharge = addOnCharge;
				}
			}
			/// <summary>
			/// 累積経験値からレベルを逆算する
			/// </summary>
			static int GetLvFromExp(int rank, int exp)
			{
				int lv = 0;
				Scm.Common.XwMaster.CharaPowerupLevelMasterData data, maxData;
				if (MasterData.TryGetCharaMaxPowerupLevel(rank, out maxData))
				{
					for (int i = 0; i <= maxData.PowerupLevel; i++)
					{
						if (MasterData.TryGetCharaPowerupLevel(rank, i, out data))
						{
							if (exp < data.Exp) break;
							lv = i;
						}
					}
				}
				return lv;
			}
			#endregion

			#region ホーム、閉じるボタンイベント
			void HandleHome(object sender, HomeClickedEventArgs e)
			{
				if (this.CharaList != null)
				{
					// Newフラグ一括解除
					this.CharaList.DeleteAllNewFlag();
				}

				GUIController.Clear();
			}
			void HandleClose(object sender, CloseClickedEventArgs e)
			{
				if (this.CharaList != null)
				{
					// Newフラグ一括解除
					this.CharaList.DeleteAllNewFlag();
				}

				GUIController.Back();
			}
			#endregion

			#region 合成ボタンイベント
			#region 合成チェック
			/// <summary>
			/// 合成ボタンイベントハンドラー
			/// </summary>
			void HandleFusion(object sender, FusionClickedEventArgs e)
			{
				// ベースキャラのスロット込み情報取得
				this.PlayerCharacterRequest();
			}
			void CheckFusion(FusionMessage.SetupParam p)
			{
				GUIFusionMessage.OpenYesNo(p, MasterData.GetText(TextType.TX309_Powerup_Fusion_Message), true, this.CheckLock, null);
			}
			void CheckLock()
			{
				// ロックチェック
				bool isLock = false;
				if (this.BaitList != null)
				{
					var list = this.BaitList.GetCharaInfoList();
					foreach (var info in list)
					{
						if (info == null) continue;
						if (!info.IsLock) continue;

						isLock = true;
						break;
					}
				}

				// ロック分岐
				if (isLock)
				{
					GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX308_Powerup_Fusion_LockMessage), true, null);
				}
				else
				{
					this.CheckSlot();
				}
			}
			void CheckSlot()
			{
				// スロット分岐
				if (this.Model.AddOnCharge > 0)
				{
					string addOnCharge = string.Format(this.Model.AddOnChargeFormat, this.Model.AddOnCharge);
					string text = string.Format(MasterData.GetText(TextType.TX310_Powerup_Fusion_SlotMessage), addOnCharge);
					GUIMessageWindow.SetModeYesNo(text, true, this.CheckHighRank, null);
				}
				else
				{
					this.CheckHighRank();
				}
			}
			void CheckHighRank()
			{
				// 高ランクチェック
				bool isInHighRank = false;
				if (this.BaitList != null)
				{
					var list = this.BaitList.GetCharaInfoList();
					foreach (var info in list)
					{
						if (info == null) continue;
						if (info.Rank < HighRank) continue;

						isInHighRank = true;
						break;
					}
				}

				// 高ランク分岐
				if (isInHighRank)
				{
					GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX311_Powerup_Fusion_HighRankMessage), true, this.FusionRequest, null);
				}
				else
				{
					this.FusionRequest();
				}
			}
			#endregion

			#region 合成実行
			void FusionRequest()
			{
				// 通知
				var eventArgs = new FusionEventArgs();
				eventArgs.BaseCharaUUID = this.BaseCharaUUID;
				eventArgs.BaitCharaUUIDList = this.BaitCharaUUIDList;
				this.OnFusion(this, eventArgs);
			}
			/// <summary>
			/// 合成結果
			/// </summary>
			public void FusionResult(Scm.Common.GameParameter.PowerupResult result, int money, int price, int addOnCharge, CharaInfo info)
			{
				switch (result)
				{
					case Scm.Common.GameParameter.PowerupResult.Fail:
						this.FusionFail();
						break;
					case Scm.Common.GameParameter.PowerupResult.Good:
					case Scm.Common.GameParameter.PowerupResult.BigSuccess:
					case Scm.Common.GameParameter.PowerupResult.SuperSuccess:
						this.FusionSuccess(result, money, price, addOnCharge, info);
						break;
				}
			}
			void FusionFail()
			{
			}
			void FusionSuccess(Scm.Common.GameParameter.PowerupResult result, int money, int price, int addOnCharge, CharaInfo info)
			{
				if (info == null) return;
				var baseInfo = this.BaseCharaInfo;
				if (baseInfo == null) return;

				// 一度非表示
				this.SetActive(false, true);

				// 演出用パラメータ
				var directionParam = this.GetPowerupDirectionParam(result, baseInfo, info);
				GUIPowerupDirection.Open(directionParam, () =>
				{
					var afterLv = GetLvFromExp(info.Rank, info.PowerupExp);
					if (CharaInfo.IsMaxLevel(info.Rank, afterLv))
					{
						this.ClearBaseChara();
					}

					// 合成結果パラメータ
					var p = this.GetFusionResultParam(baseInfo, info, result);
					var screen = new GUIScreen(() => { GUIFusionResult.Open(p); }, GUIFusionResult.Close, GUIFusionResult.ReOpen);
					GUIController.Open(screen);
				});

				// 餌キャラを外す
				this.ClearBaitList();
			}
			#endregion

			#region プレイヤーキャラクター情報取得イベント
			/// <summary>
			/// プレイヤーキャラクター情報取得イベント
			/// </summary>
			public event EventHandler<PlayerCharacterEventArgs> OnPlayerCharacter = (sender, e) => { };

			/// <summary>
			/// プレイヤーキャラクター情報取得リクエスト
			/// </summary>
			void PlayerCharacterRequest()
			{
				if (this.IsEmptyBaseChara) return;

				// 通知
				var eventArgs = new PlayerCharacterEventArgs();
				eventArgs.BaseCharaUUID = this.BaseCharaUUID;
				this.OnPlayerCharacter(this, eventArgs);
			}
			/// <summary>
			/// プレイヤーキャラクター情報取得結果
			/// </summary>
			public void PlayerCharacterResult(CharaInfo info, List<CharaInfo> slotList)
			{
				// ベースキャラの情報を更新する
				var baseInfo = this.BaseCharaInfo;
				if (baseInfo == null) return;
				if (baseInfo.UUID == info.UUID)
				{
					// ベースキャラの同期処理を一旦切って情報だけ更新する
					this.CanSyncBaseChara = false;
					this.SetCharaItem(this.BaseChara, info);
					this.CanSyncBaseChara = true;
				}

				// 合成確認パラメータ
				var p = this.GetFusionMessageParam(info, slotList);
				this.CheckFusion(p);
			}
			#endregion

			#region 各種パラメータ取得
			/// <summary>
			/// 合成メッセージに表示するためのデータを取得する
			/// </summary>
			FusionMessage.SetupParam GetFusionMessageParam(CharaInfo info, List<CharaInfo> slotList)
			{
				var p = new FusionMessage.SetupParam();
				if (!this.CanUpdate) return p;

				// 強化後の情報
				// ベースキャラに装着されているスロット情報リストを作成
				var afterTotalInfo = new Scm.Common.Fusion.PowerupSlot(
					info.CharacterMasterID, info.Rank, this.Model.AfterLv,
					info.SynchroHitPoint, info.SynchroAttack,
					info.SynchroExtra, info.SynchroDefense);
				{
					var powerupSlotList = new List<Scm.Common.XwDb.IPlayerCharacter>();
					foreach (var t in slotList)
					{
						if (t == null) continue;
						var ps = new Scm.Common.Fusion.PowerupSlot(
							t.CharacterMasterID, t.Rank, t.PowerupLevel,
							t.SynchroHitPoint, t.SynchroAttack,
							t.SynchroExtra, t.SynchroDefense);
						powerupSlotList.Add(ps);
					}
					afterTotalInfo.SetSlot(powerupSlotList);
				}
				var afterPowerupFusion = new Scm.Common.Fusion.PowerupFusion(info.CharacterMasterID, info.Rank);

				// ランク
				p.RankBefore	= info.Rank;
				p.RankAfter		= info.Rank;
				// レベル
				p.LevelBefore	= info.PowerupLevel;
				p.LevelAfter	= this.Model.AfterLv;
				// 経験値
				p.Exp				= this.Model.AfterExp;
				p.TotalExp			= this.Model.AfterTotalExp;
				p.NextLvTotalExp	= this.Model.NextLvTotalExp;
				// シンクロ可能回数
				p.SynchroRemainBefore	= info.SynchroRemain;
				p.SynchroRemainAfter	= info.SynchroRemain;
				// 生命力
				p.HitPointBefore		= info.HitPoint;
				p.HitPointAfter			= afterTotalInfo.HitPoint;
				p.HitPointBaseBefore	= info.PowerupHitPoint;
				p.HitPointBaseAfter		= afterPowerupFusion.GetPowerupHitPoint(this.Model.AfterLv);
				p.SynchroHitPoint		= info.SynchroHitPoint;
				p.SlotHitPointBefore	= info.SlotHitPoint;
				p.SlotHitPointAfter		= info.SlotHitPoint;
				// 攻撃力
				p.AttackBefore			= info.Attack;
				p.AttackAfter			= afterTotalInfo.Attack;
				p.AttackBaseBefore		= info.PowerupAttack;
				p.AttackBaseAfter		= afterPowerupFusion.GetPowerupAttack(this.Model.AfterLv);
				p.SynchroAttack			= info.SynchroAttack;
				p.SlotAttackBefore		= info.SlotAttack;
				p.SlotAttackAfter		= info.SlotAttack;
				// 防御力
				p.DefenseBefore			= info.Defense;
				p.DefenseAfter			= afterTotalInfo.Defense;
				p.DefenseBaseBefore		= info.PowerupDefense;
				p.DefenseBaseAfter		= afterPowerupFusion.GetPowerupDefense(this.Model.AfterLv);
				p.SynchroDefense		= info.SynchroDefense;
				p.SlotDefenseBefore		= info.SlotDefense;
				p.SlotDefenseAfter		= info.SlotDefense;
				// 特殊能力
				p.ExtraBefore			= info.Extra;
				p.ExtraAfter			= afterTotalInfo.Extra;
				p.ExtraBaseBefore		= info.PowerupExtra;
				p.ExtraBaseAfter		= afterPowerupFusion.GetPowerupExtra(this.Model.AfterLv);
				p.SynchroExtra			= info.SynchroExtra;
				p.SlotExtraBefore		= info.SlotExtra;
				p.SlotExtraAfter		= info.SlotExtra;
				// シンクロ合成フラグ
				p.IsSynchroFusion = false;

				return p;
			}
			/// <summary>
			/// 合成結果に表示するためのデータを取得する
			/// </summary>
			FusionResult.SetupParam GetFusionResultParam(CharaInfo beforeInfo, CharaInfo afterInfo, Scm.Common.GameParameter.PowerupResult result)
			{
				var p = new FusionResult.SetupParam();
				if (!this.CanUpdate) return p;

				// アバタータイプ
				p.AvatarType	= beforeInfo.AvatarType;
				// ランク
				p.RankBefore	= beforeInfo.Rank;
				p.RankAfter		= afterInfo.Rank;
				// レベル
				p.LevelBefore	= beforeInfo.PowerupLevel;
				p.LevelAfter	= afterInfo.PowerupLevel;
				// 経験値
				p.Exp				= this.Model.AfterExp;
				p.TotalExp			= this.Model.AfterTotalExp;
				p.NextLvTotalExp	= this.Model.NextLvTotalExp;
				// シンクロ可能回数
				p.SynchroRemainBefore	= beforeInfo.SynchroRemain;
				p.SynchroRemainAfter	= afterInfo.SynchroRemain;
				// 生命力
				p.HitPointBefore		= beforeInfo.HitPoint;
				p.HitPointAfter			= afterInfo.HitPoint;
				p.HitPointBaseBefore	= beforeInfo.PowerupHitPoint;
				p.HitPointBaseAfter		= afterInfo.PowerupHitPoint;
				p.SynchroHitPointBefore	= beforeInfo.SynchroHitPoint;
				p.SynchroHitPointAfter	= afterInfo.SynchroHitPoint;
				p.SlotHitPointBefore	= beforeInfo.SlotHitPoint;
				p.SlotHitPointAfter		= afterInfo.SlotHitPoint;
				// 攻撃力
				p.AttackBefore			= beforeInfo.Attack;
				p.AttackAfter			= afterInfo.Attack;
				p.AttackBaseBefore		= beforeInfo.PowerupAttack;
				p.AttackBaseAfter		= afterInfo.PowerupAttack;
				p.SynchroAttackBefore	= beforeInfo.SynchroAttack;
				p.SynchroAttackAfter	= afterInfo.SynchroAttack;
				p.SlotAttackBefore		= beforeInfo.SlotAttack;
				p.SlotAttackAfter		= afterInfo.SlotAttack;
				// 防御力
				p.DefenseBefore			= beforeInfo.Defense;
				p.DefenseAfter			= afterInfo.Defense;
				p.DefenseBaseBefore		= beforeInfo.PowerupDefense;
				p.DefenseBaseAfter		= afterInfo.PowerupDefense;
				p.SynchroDefenseBefore	= beforeInfo.SynchroDefense;
				p.SynchroDefenseAfter	= afterInfo.SynchroDefense;
				p.SlotDefenseBefore		= beforeInfo.SlotDefense;
				p.SlotDefenseAfter		= afterInfo.SlotDefense;
				// 特殊能力
				p.ExtraBefore			= beforeInfo.Extra;
				p.ExtraAfter			= afterInfo.Extra;
				p.ExtraBaseBefore		= beforeInfo.PowerupExtra;
				p.ExtraBaseAfter		= afterInfo.PowerupExtra;
				p.SynchroExtraBefore	= beforeInfo.SynchroExtra;
				p.SynchroExtraAfter		= afterInfo.SynchroExtra;
				p.SlotExtraBefore		= beforeInfo.SlotExtra;
				p.SlotExtraAfter		= afterInfo.SlotExtra;
				// シンクロ合成フラグ
				p.IsPowerupResultEnable	= true;
				p.PowerupResult			= result;

				return p;
			}
			/// <summary>
			/// 合成演出に必要なデータを取得する
			/// </summary>
			/// <returns></returns>
			PowerupDirection.PowerupDirectionParam GetPowerupDirectionParam(Scm.Common.GameParameter.PowerupResult result, CharaInfo beforeInfo, CharaInfo afterInfo)
			{
				var p = new PowerupDirection.PowerupDirectionParam();
				if (this.BaitList == null) return p;

				var afterLv = GetLvFromExp(afterInfo.Rank, afterInfo.PowerupExp);
				var totalExp = CharaInfo.GetTotalExp(afterInfo.Rank, afterLv);
				var nextLvTotalExp = CharaInfo.GetTotalExp(afterInfo.Rank, afterLv + 1);

				// 演出用パラメータ
				p.BaseCharaId = beforeInfo.CharacterMasterID;
				var infoList = this.BaitList.GetCharaInfoList();
				if (infoList != null)
				{
					int[] baitIds = new int[infoList.Count];
					for (int i = 0; i < infoList.Count; i++)
					{
						baitIds[i] = infoList[i].CharacterMasterID;
					}
					p.BaitCharaIds = baitIds;
				}

				// 開始時の経験値の位置
				var beforeLv = GetLvFromExp(beforeInfo.Rank, beforeInfo.PowerupExp);
				var bfTotalExp = CharaInfo.GetTotalExp(beforeInfo.Rank, beforeLv);
				var bfNextLvTotalExp = CharaInfo.GetTotalExp(beforeInfo.Rank, beforeLv + 1);
				p.StartExpRate = XUI.Powerup.Util.GetLerpValue((float)bfTotalExp, (float)bfNextLvTotalExp, (float)beforeInfo.PowerupExp);
				p.EndExpRate = XUI.Powerup.Util.GetLerpValue((float)totalExp, (float)nextLvTotalExp, (float)afterInfo.PowerupExp);
				p.Result = result;
				p.LvUpCount = afterLv - this.Model.Lv;
				p.LvMax = CharaInfo.IsMaxLevel(afterInfo.Rank, afterLv);

				return p;
			}
			#endregion
			#endregion

			#region 全てを外すボタンイベント
			void HandleAllClear(object sender, EventArgs e)
			{
				this.ClearBaitList();
			}
			#endregion
		}
	}
}
