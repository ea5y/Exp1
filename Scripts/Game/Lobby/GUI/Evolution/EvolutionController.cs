/// <summary>
/// 進化合成制御
/// 
/// 2016/02/02
/// </summary>
using System;
using System.Collections.Generic;
using Scm.Common;
using Scm.Common.XwMaster;
using XUI.CharaList;

namespace XUI
{
	namespace Evolution
	{
		#region イベント引数
		/// <summary>
		/// 合成イベント引数
		/// </summary>
		public class FusionEventArgs : EventArgs
		{
			public ulong BaseCharaUUID { get; set; }
			public List<ulong> BaitCharaUUIDList { get; set; }
		}

		/// <summary>
		/// 合成試算イベント引数
		/// </summary>
		public class FusionCalcEventArgs : EventArgs
		{
			public ulong BaseCharaUUID { get; set; }
			public List<ulong> BaitCharaUUIDList { get; set; }
		}

		/// <summary>
		/// プレイヤーキャラクター情報取得イベント引数
		/// </summary>
		public class PlayerCharacterEventArgs : EventArgs
		{
			public ulong UUID { get; set; }
		}
		#endregion

		/// <summary>
		/// 進化合成制御インターフェイス
		/// </summary>
		public interface IController
		{
			#region 更新チェック
			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }
			#endregion

			#region アクティブ
			/// <summary>
			/// アクティブ設定
			/// </summary>
			void SetActive(bool isActive, bool isTweenSkip);
			#endregion

			#region 初期化
			/// <summary>
			/// 初期化
			/// </summary>
			void Setup();

			/// <summary>
			/// ステータス情報設定
			/// </summary>
			void SetupStatusInfo(int haveMoney);

			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();
			#endregion

			#region BOX総数
			/// <summary>
			/// BOXの総数セット
			/// </summary>
			void SetupCapacity(int capacity, int count);
			#endregion

			#region キャラ情報リスト
			/// <summary>
			/// キャラ情報リスト初期設定
			/// </summary>
			void SetupCharaInfoList(List<CharaInfo> list);
			#endregion

			#region 合成試算
			/// <summary>
			/// 合成試算イベント
			/// </summary>
			event EventHandler<FusionCalcEventArgs> OnFusionCalc;

			/// <summary>
			/// 合成試算結果
			/// </summary>
			void FusionCalcResult(bool result, int money, int price, int addOnCharge);
			#endregion

			#region 合成
			/// <summary>
			/// 合成イベント
			/// </summary>
			event EventHandler<FusionEventArgs> OnFusion;

			/// <summary>
			/// 合成結果
			/// </summary>
			void FusionResult(bool result, int money, int price, int addOnCharge, int synchroBonus, CharaInfo charaInfo);
			#endregion

			#region プレイヤーキャラクター
			/// <summary>
			/// プレイヤーキャラクター取得イベント
			/// </summary>
			event EventHandler<PlayerCharacterEventArgs> OnPlayerCharacter;

			/// <summary>
			/// プレイヤーキャラクター情報を設定する
			/// </summary>
			void SetupPlayerCharacterInfo(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList);
			#endregion
		}

		/// <summary>
		/// 進化合成制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド&プロパティ
			/// <summary>
			/// モデル
			/// </summary>
			private readonly IModel _model;
			private IModel Model { get { return _model; } }

			/// <summary>
			/// ビュー
			/// </summary>
			private readonly IView _view;
			private IView View { get { return _view; } }

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

			/// <summary>
			/// キャラページリスト
			/// </summary>
			private readonly GUICharaPageList _charaList;
			private GUICharaPageList CharaList { get { return _charaList; } }

			/// <summary>
			/// ベースキャラ
			/// </summary>
			private readonly GUICharaItem _baseChara;
			private GUICharaItem BaseChara { get { return _baseChara; } }

			// ベースキャラUUID
			private ulong BaseCharaUUID
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
			private CharaInfo BaseCharaInfo { get { return (this.BaseChara != null ? this.BaseChara.GetCharaInfo() : null); } }
			
			// ベースキャラが空かどうか
			private bool IsEmptyBaseChara { get { return (this.BaseCharaInfo == null ? true : false); } }

			/// <summary>
			/// 進化素材リスト
			/// </summary>
			readonly GUIEvolutionMaterialList _materialList;
			GUIEvolutionMaterialList MaterialList { get { return _materialList; } }

			/// <summary>
			/// 進化キャラ
			/// </summary>
			private readonly GUICharaItem _evolutionChara;
			private GUICharaItem EvolutionChara { get { return _evolutionChara; } }

			/// <summary>
			/// 合成試算イベント
			/// </summary>
			public event EventHandler<FusionCalcEventArgs> OnFusionCalc = (sender, e) => { };

			/// <summary>
			/// 合成イベント
			/// </summary>
			public event EventHandler<FusionEventArgs> OnFusion = (sender, e) => { };

			/// <summary>
			/// プレイヤーキャラクター取得イベント
			/// </summary>
			public event EventHandler<PlayerCharacterEventArgs> OnPlayerCharacter = (sender, e) => { };
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public Controller(IModel model, IView view, GUICharaPageList charaList, GUICharaItem baseChara, GUIEvolutionMaterialList materialList, GUICharaItem evolutionChara)
			{
				if (model == null || view == null) { return; }

				// ページリストとベースキャラと進化素材と進化キャラセット
				this._charaList = charaList;
				this._baseChara = baseChara;
				this._materialList = materialList;
				this._evolutionChara = evolutionChara;

				// ビュー設定
				this._view = view;
				this.View.OnHome += this.HandleHome;
				this.View.OnClose += this.HandleClose;
				this.View.OnFusion += this.HandleFusion;

				// モデル設定
				this._model = model;
				// 所持金イベント登録
				this.Model.OnHaveMoneyChange += this.HandleHaveMoneyChange;
				this.Model.OnHaveMoneyFormatChange += this.HandleHaveMoneyFormatChange;
				// 費用イベント登録
				this.Model.OnNeedMoneyChange += this.HandleNeedMoneyChange;
				this.Model.OnNeedMoneyFormatChange += this.HandleNeedMoneyFormatChange;
				// ベースキャラステータスイベント登録
				this.Model.OnBaseSynchroRemainChange += HandleBaseSynchroRemainChange;
				this.Model.OnBaseRankChange += this.HandleBaseRankChange;
				// 進化キャラステータスイベント登録
				this.Model.OnEvolutionSynchroRemainChange += this.HandleEvolutionSynchroRemainChange;
				this.Model.OnEvolutionRankChange += this.HandleEvolutionRankChange;
				// キャラ情報リストイベント登録
				this.Model.OnClearCharaInfoList += this.HandleClearCharaInfoList;

				// モデル同期
				SyncHaveMoney();
				SyncNeedMoney();
				SyncBaseStatus();
				SyncEvolutionStatus();

				// イベント登録
				if(this.CharaList != null)
				{
					// キャラページリスト
					this.CharaList.OnItemChangeEvent += this.HandleCharaListItemChangeEvent;
					this.CharaList.OnItemClickEvent += this.HandleCharaListItemClickEvent;
					this.CharaList.OnUpdateItemsEvent += this.HandleCharaListUpdateItemsEvent;
				}
				if(this.BaseChara != null)
				{
					// ベースキャラ
					this.BaseChara.OnItemChangeEvent += this.HandleBeseCharaItemChangeEvent;
					this.BaseChara.OnItemClickEvent += this.HandleBaseCharaItemClickEvent;
				}
				if(this.MaterialList != null)
				{
					// 進化素材リスト
					this.MaterialList.OnUpdateItemsEvent += this.MaterialListUpdateItemsEvent;
					this.MaterialList.OnItemClickEvent += this.HandleMaterialListItemClickEvent;
				}
				if(this.EvolutionChara != null)
				{
					// 進化キャラ
					this.EvolutionChara.OnItemChangeEvent += this.HandleEvolutionCharaItemChangeEvent;
				}

				// 同期
				this.SyncClearCharaInfoList();
				this.SyncBaseChara();
				SyncEvolutionChara();

				// 進化キャラの初期設定
				this.EvolutionChara.SetButtonEnable(false);
			}

			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				if(this.CanUpdate)
				{
					this.Model.Dispose();
				}

				this.OnFusionCalc = null;
				this.OnFusion = null;
				this.OnPlayerCharacter = null;
			}

			/// <summary>
			/// 初期化
			/// </summary>
			public void Setup()
			{
				this.ClearCharaInfoList();
				// 同期
				this.SyncBaseChara();
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
					GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX326_Evolution_Fusion_ScreenTitle));
					// ヘルプメッセージの状態を更新する
					this.UpdateHelpMessage();
				}
			}
			#endregion

			#region 各状態を更新する
			/// <summary>
			/// 合成ボタンの状態を更新する
			/// </summary>
			private void UpdateFusionButtonEnable()
			{
				if (!this.CanUpdate) return;

				bool isEnable = true;

				// ベースキャラLV不足のため進化不能
				if (this.BaseCharaInfo == null || !CharaInfo.IsMaxLevel(this.BaseCharaInfo.Rank, this.BaseCharaInfo.PowerupLevel)) isEnable = false;
				// 素材リスト側で進化可能状態ではない
				else if (this.MaterialList == null || !this.MaterialList.CanEvolution) isEnable = false;
				// 所持金が足りない
				else if (this.Model.HaveMoney < this.Model.NeedMoney) isEnable = false;

				this.View.SetFusionButtonEnable(isEnable);
			}

			/// <summary>
			/// ヘルプメッセージの状態を更新する
			/// </summary>
			private void UpdateHelpMessage()
			{
				if (!this.CanUpdate) return;

				var state = this.View.GetActiveState();
				bool isActive = false;
				if (state == GUIViewBase.ActiveState.Opened || state == GUIViewBase.ActiveState.Opening)
				{
					isActive = true;
				}

				var baseCharaUUID = this.BaseCharaUUID;
				// ベースキャラを選択中
				if (baseCharaUUID <= 0) { GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX327_Evolution_Fusion_Base_HelpMessage)); }
				// ベースキャラが最大レベルでない
				else if (!CharaInfo.IsMaxLevel(this.BaseCharaInfo.Rank, this.BaseCharaInfo.PowerupLevel)) { GUIHelpMessage.PlayWarning(isActive, MasterData.GetText(TextType.TX334_MaterialList_PowerupLevelShortage)); }
				// 進化可能状態
				else if (this.MaterialList.CanEvolution) { GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX368_Evolution_Fusion_Enter_HelpMessage)); }
				// 素材選択中状態
				else if(this.IsHaveMaterialChara()) { GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX328_Evolution_Fusion_Bait_HelpMessage)); }
				// 素材にできるキャラを所持していない状態
				else { GUIHelpMessage.PlayWarning(isActive, MasterData.GetText(TextType.TX369_Evolution_Fusion_NotHave_HelpMessage)); }
			}
			#endregion

			#region BOX総数
			/// <summary>
			/// BOXの総数セット
			/// </summary>
			public void SetupCapacity(int capacity, int count)
			{
				if(this.CharaList != null)
				{
					this.CharaList.SetupCapacity(capacity, count);
				}
			}
			#endregion

			#region キャラ情報リスト
			#region セット
			/// <summary>
			/// キャラ情報リスト初期設定
			/// </summary>
			public void SetupCharaInfoList(List<CharaInfo> list)
			{
				if (!this.CanUpdate || list == null) { return; }
				List<CharaInfo> charaInfoList = list;

				// 選択設定
				this.SetCanSelect(charaInfoList);

				// キャラ情報セット
				this.Model.SetCharaInfoList(charaInfoList);
				// 素材リストのキャラ情報更新
				this.UpdateMaterialCharaInfoList(charaInfoList);
				// ページリスト内のキャラ情報更新
				this.UpdateCharaListInfo(charaInfoList);
				// ベースキャラのキャラ情報更新
				this.UpdateBaseCharaInfo(charaInfoList);
				// 進化キャラのキャラ情報更新
				this.UpdateEvolutionCharaInfo(charaInfoList);

				// ベースキャラの初期設定によって素材リストの初期状態を設定する
				if (this.IsEmptyBaseChara)
				{
					// ベースキャラ空時は進化素材キャラ外す
					this.ClearMaterialList();
				}
				else
				{
					// 進化素材選択時は進化素材のみのキャラページリストセット
					this.SetBaseCharaMaterialList();
				}
			}

			#region 選択設定
			/// <summary>
			/// 選択設定を行う
			/// </summary>
			private void SetCanSelect(List<CharaInfo> charaInfoList)
			{
				if (this.IsEmptyBaseChara)
				{
					// ベースキャラ選択時

					// 各キャラ情報の選択出来るかどうかを設定する
					charaInfoList.ForEach(this.SetBaseCharaCanSelect);
				}
				else
				{
					// 素材キャラ選択時

					// 進化素材マスターデータ取得
					List<CharaEvolutionMaterialMasterData> materialMasterList;
					this.TryGetMaterialMasterData(out materialMasterList);
					// 各キャラ情報の選択出来るかどうかを設定する
					foreach (var info in charaInfoList) { this.SetMaterialCharaCanSelect(info, materialMasterList); }
				}
			}
			/// <summary>
			/// ベースキャラ選択時の各キャラ情報の選択できるかの設定
			/// </summary>
			private void SetBaseCharaCanSelect(CharaInfo info)
			{
				if (info == null) return;

				// 無効タイプを取得する
				CharaItem.Controller.DisableType disableType;
				this.GetBaseCharaDisableType(info, out disableType);

				// 無効タイプから選択できるか設定する
				var canSelect = false;
				if (disableType == CharaItem.Controller.DisableType.None ||
					disableType == CharaItem.Controller.DisableType.Base ||
					disableType == CharaItem.Controller.DisableType.Bait)
				{
					canSelect = true;
				}
				info.CanSelect = canSelect;
			}

			/// <summary>
			/// 素材キャラ選択時の各キャラ情報の選択できるかの設定
			/// </summary>
			private void SetMaterialCharaCanSelect(CharaInfo info, List<CharaEvolutionMaterialMasterData> materialMasterList)
			{
				if (info == null) return;

				// 無効タイプを取得する
				CharaItem.Controller.DisableType disableType;
				int baitIndex = 0;
				this.GetMaterialCharaDisableType(info, materialMasterList, out disableType, out baitIndex);

				// 無効タイプから選択できるか設定する
				var canSelect = false;
				if (disableType == CharaItem.Controller.DisableType.None ||
					disableType == CharaItem.Controller.DisableType.Base ||
					disableType == CharaItem.Controller.DisableType.Bait)
				{
					canSelect = true;
				}
				info.CanSelect = canSelect;
			}
			#endregion
			#endregion

			#region キャラ情報更新
			/// <summary>
			/// キャラ情報リスト更新
			/// </summary>
			private void UpdateCharaInfoList()
			{
				// キャラ情報リスト取得
				List<CharaInfo> charaInfoList = this.Model.GetCharaInfoList();

				// 選択設定
				this.SetCanSelect(charaInfoList);
				// ページリスト内のキャラ情報更新
				this.UpdateCharaListInfo(charaInfoList);
			}
			#endregion

			#region 削除
			/// <summary>
			/// キャラ情報リスト削除
			/// </summary>
			private void ClearCharaInfoList()
			{
				if (!this.CanUpdate) { return; }
				this.Model.ClearCharaInfoList();
			}
			/// <summary>
			/// キャラ情報リスト内が削除された時に呼ばれる
			/// </summary>
			private void HandleClearCharaInfoList(object sender, EventArgs e)
			{
				this.SyncClearCharaInfoList();
			}
			/// <summary>
			/// キャラ情報削除時の同期
			/// </summary>
			private void SyncClearCharaInfoList()
			{
				// ベース/ページ/素材/進化の各キャラ情報を削除する
				this.ClearCharaList();
				this.ClearBaseChara();
				this.ClearMaterialList();
				this.ClearEvolutionChara();
			}
			#endregion
			#endregion

			#region キャラページリスト
			#region キャラリスト同期
			/// <summary>
			/// キャラページリストのアイテムに変更があった時に呼び出される
			/// </summary>
			private void HandleCharaListItemChangeEvent(GUICharaItem item){}
			#endregion

			#region アイテム更新
			/// <summary>
			/// キャラページリストの全アイテムが更新された時に呼ばれる
			/// </summary>
			private void HandleCharaListUpdateItemsEvent()
			{
				if(this.CharaList != null)
				{
					// アイテム無効設定
					if (this.IsEmptyBaseChara)
					{
						// ベースキャラ選択時

						var itemList = this.CharaList.GetNowPageItemList();
						itemList.ForEach(this.SetBeseCharaDisableState);
					}
					else
					{
						// 素材キャラ選択時

						// 進化素材マスターデータ取得
						List<CharaEvolutionMaterialMasterData> materialMasterList;
						this.TryGetMaterialMasterData(out materialMasterList);

						var itemList = this.CharaList.GetNowPageItemList();
						itemList.ForEach((item) => { this.SetMaterialCharaDisableState(item, materialMasterList); });
					}
				}
			}
			#endregion

			#region キャラ情報更新
			/// <summary>
			/// キャラページリスト内のキャラ情報を更新
			/// </summary>
			private void UpdateCharaListInfo(List<CharaInfo> charaInfoList)
			{
				if (this.IsEmptyBaseChara)
				{
					// ベースキャラ選択時

					// ベースキャラ枠が空ならサーバから受け取ったキャラ情報をセット
					this.SetEvolutionCharaList();
				}
				else
				{
					// 素材キャラ選択時

					// ベースキャラ枠がセットされているなら進化素材一覧のみのキャラリストをセットする
					this.SetMaterialCharaList();
				}
			}
			#endregion

			#region 無効設定
			/// <summary>
			/// ベースキャラ選択時の各アイテムの無効設定
			/// </summary>
			private void SetBeseCharaDisableState(GUICharaItem item)
			{
				if (item == null) return;

				// 無効タイプを設定する
				CharaItem.Controller.DisableType disableType;
				this.GetBaseCharaDisableType(item.GetCharaInfo(), out disableType);
				item.SetDisableState(disableType);
			}

			/// <summary>
			/// ベースキャラ選択時の無効タイプを取得する
			/// </summary>
			private void GetBaseCharaDisableType(CharaInfo info, out CharaItem.Controller.DisableType disableType)
			{
				disableType = XUI.CharaItem.Controller.DisableType.None;
				if (info == null) return;

				// 以下無効にするかチェック
				// スロットに入っている
				if (info.IsInSlot) disableType = XUI.CharaItem.Controller.DisableType.PowerupSlot;
			}

			/// <summary>
			/// 素材キャラ選択時の各アイテムの無効設定
			/// </summary>
			private void SetMaterialCharaDisableState(GUICharaItem item, List<CharaEvolutionMaterialMasterData> materialMasterList)
			{
				if (item == null) return;
				
				// 無効タイプを設定する
				CharaItem.Controller.DisableType disableType = CharaItem.Controller.DisableType.None;
				int baitIndex = -1;
				this.GetMaterialCharaDisableType(item.GetCharaInfo(), materialMasterList, out disableType, out baitIndex);
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
				};
			}

			/// <summary>
			/// 素材キャラ選択時の無効タイプを取得する
			/// </summary>
			private void GetMaterialCharaDisableType(CharaInfo info, List<CharaEvolutionMaterialMasterData> materialMasterList, out CharaItem.Controller.DisableType disableType, out int baitIndex)
			{
				disableType = XUI.CharaItem.Controller.DisableType.None;
				baitIndex = -1;

				if(info == null || info.UUID <= 0)
				{
					// キャラアイコン状態ではないので無効タイプを設定なしにする
					disableType = CharaItem.Controller.DisableType.None;
					return;
				}

				// 素材リストで選択されているキャラの種類と同じ場合は各無効状態チェックして設定する

				// 以下無効にするかチェック
				// 優先順位があるので注意
				// 非選択状態にするか取得
				CharaInfo selectInfo = this.MaterialList.SelectItem != null ? this.MaterialList.SelectItem.GetCharaInfo() : null;
				bool isNotSelected = (selectInfo == null || selectInfo.UUID > 0 || selectInfo.AvatarType != info.AvatarType) ? true : false;
				// 選択番号取得
				baitIndex = this.MaterialList.GetCharaInfoList().FindIndex((materialInfo) => { return (materialInfo != null && info.UUID == materialInfo.UUID); });
				// ランク不足検索
				bool isRankshortage = true;
				foreach (var data in materialMasterList)
				{
					if (info.CharacterMasterID == data.MaterialCharacterMasterId &&
						info.Rank >= data.MaterialRank)
					{
						// ひとつでもランク不足がなければランク不足設定しない
						isRankshortage = false;
						break;
					}
				}

				if (info.UUID == this.BaseCharaUUID)
				{
					// ベースキャラ選択中
					disableType = CharaItem.Controller.DisableType.Base;
				}
				else if (baitIndex > -1)
				{
					// 素材キャラ設定中
					disableType = CharaItem.Controller.DisableType.Bait;
					return;
				}
				// ランク不足
				else if (isRankshortage) disableType = CharaItem.Controller.DisableType.RankShortage;
				// スロットに入っている
				else if (info.IsInSlot) disableType = CharaItem.Controller.DisableType.PowerupSlot;
				// デッキに入っている
				else if (info.IsInDeck) disableType = CharaItem.Controller.DisableType.Deck;
				// シンボルに設定している
				else if (info.IsSymbol) disableType = CharaItem.Controller.DisableType.Symbol;
				// ロック中
				else if (info.IsLock) disableType = CharaItem.Controller.DisableType.Lock;
				// 素材リストで選択されいるキャラの種類が同じ種類ではないまたは何も選択されていないまたはすでに選択されている枠に餌キャラが設定されている場合は非選択状態にする
				else if (isNotSelected) disableType = CharaItem.Controller.DisableType.NotSelected;
			}
			#endregion

			#region キャラリスト内のアイテム
			/// <summary>
			/// キャラページリスト内のアイテムが押された時に呼び出される
			/// </summary>
			private void HandleCharaListItemClickEvent(GUICharaItem obj)
			{
				// アイテムセット
				this.SetItem(obj);
				
				if(obj.GetCharaInfo() != null)
				{
					// 元のキャラ情報を更新
					this.UpdateCharaInfoList();
				}
			}
			/// <summary>
			/// アイテム設定
			/// </summary>
			/// <param name="item"></param>
			private void SetItem(GUICharaItem item)
			{
				if (item == null) { return; }

				var disableState = item.GetDisableState();
				var info = item.GetCharaInfo();
				switch (disableState)
				{
					case CharaItem.Controller.DisableType.None:
						if (this.IsEmptyBaseChara)
						{
							// ベースキャラ設定
							this.SetBaseChara(info);
						}
						else
						{
							// 進化素材追加
							this.AddBaitMaterialChara(info);
						}
						break;
					case CharaItem.Controller.DisableType.Base:
						// ベースキャラクリア
						this.ClearBaseChara();
						break;
					case CharaItem.Controller.DisableType.Bait:
						// 進化素材削除
						this.RemoveBaitMaterialChara(item.GetCharaInfo());
						break;
					default:
						break;
				}
			}
			#endregion

			#region キャラリストクリア
			/// <summary>
			/// キャラリストクリア
			/// </summary>
			private void ClearCharaList()
			{
				if (this.CharaList == null) { return; }
				this.CharaList.SetupItems(null);
				// 1ページ目に戻す
				this.CharaList.BackEndPage();
			}
			#endregion

			#region キャラリスト設定
			/// <summary>
			/// 進化可能順になるようにキャラリストを設定する
			/// </summary>
			private void SetEvolutionCharaList()
			{
				if (!this.CanUpdate || this.CharaList == null) { return; }
				this.CharaList.SetupItems(this.Model.GetCharaInfoList());
			}

			/// <summary>
			/// 進化素材のみのキャラリストをセットする
			/// </summary>
			private void SetMaterialCharaList()
			{
				if (!this.CanUpdate || this.CharaList == null) { return; }

				CharaInfo baseCharaInfo = this.BaseCharaInfo;
				if (baseCharaInfo == null) { return; }

				// 進化素材マスターデータ取得
				List<CharaEvolutionMaterialMasterData> materialMasterList;
				this.TryGetMaterialMasterData(out materialMasterList);

				// 進化素材のみのキャラ一覧を作成
				var materialCharaInfoList = new List<CharaInfo>();
				foreach(var info in this.Model.GetCharaInfoList())
				{
					// 進化可能な素材とベースキャラのみ表示 それ以外のキャラは存在アイコンを表示する
					if (this.IsMaterial(info, materialMasterList) || info.UUID == this.BaseCharaInfo.UUID)
					{
						materialCharaInfoList.Add(info);
					}
					else
					{
						materialCharaInfoList.Add(null);
					}

				}
				// ページリストに登録
				this.CharaList.SetupItems(materialCharaInfoList);
			}
			#endregion
			#endregion

			#region ベースキャラ
			#region ベースキャラ同期
			/// <summary>
			/// ベースキャラアイテムに変更があった時に呼び出される
			/// </summary>
			private void HandleBeseCharaItemChangeEvent(GUICharaItem obj)
			{
				this.SyncBaseChara();
			}
			/// <summary>
			/// ベースキャラ同期
			/// </summary>
			private void SyncBaseChara()
			{
				// ベースキャラ選択
				this.SetSelectBaseChara(this.IsEmptyBaseChara);
				// ベースキャラのステータスをセット
				SetBaseCharaStatus();
				if (this.View != null)
				{
					// 素材リストフィルター表示設定
					this.View.SetFillMaterialListActive(this.IsEmptyBaseChara);
				}
				// 合成ボタン状態更新
				this.UpdateFusionButtonEnable();
				// ヘルプメッセージ更新
				this.UpdateHelpMessage();
			}

			/// <summary>
			/// 設定されているベースキャラからステータスをデータにセットする
			/// </summary>
			private void SetBaseCharaStatus()
			{
				if (this.Model == null) { return; }
				int remain = 0;
				int rank = 0;

				var baseCharaInfo = this.BaseCharaInfo;
				if (baseCharaInfo != null)
				{
					remain = baseCharaInfo.SynchroRemain;
					rank = baseCharaInfo.Rank;
				}

				// ベースキャラステータス更新
				this.Model.BaseSynchroRemain = remain;
				this.Model.BaseRank = rank;
			}
			#endregion

			#region ベースキャライベント
			/// <summary>
			/// ベースキャラアイテムが押された時に呼び出される
			/// </summary>
			private void HandleBaseCharaItemClickEvent(GUICharaItem obj)
			{
				// ベースキャラが設定されていたら外す
				this.ClearBaseChara();
				// 元のキャラ情報を更新
				this.UpdateCharaInfoList();
			}
			#endregion

			#region ベースキャラアイテム
			/// <summary>
			/// ベースキャラ設定
			/// </summary>
			private void SetBaseChara(CharaInfo info)
			{
				if (this.BaseChara != null)
				{
					if (this.BaseChara.GetCharaInfo() != info)
					{
						// キャラページリストを1ページ目に戻す
						if (this.CharaList != null)
						{
							this.CharaList.BackEndPage();
						}
					}
					this.SetBaseCharaItem(this.BaseChara, info);
				}

				if (this.IsEmptyBaseChara)
				{
					// ベースキャラ空時

					// 進化素材キャラ外す
					this.ClearMaterialList();
				}
				else
				{
					// 進化素材選択時

					// 進化素材のみのキャラページリストセット
					this.SetBaseCharaMaterialList();
				}
			}
	
			/// <summary>
			/// ベースキャラを外す
			/// </summary>
			private void ClearBaseChara()
			{
				this.SetBaseChara(null);
			}

			/// <summary>
			/// ベースキャラアイテムを設定する
			/// </summary>
			private void SetBaseCharaItem(GUICharaItem item, CharaInfo info)
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

			#region ベースキャラステータス
			#region シンクロ合成残り回数
			// ベースキャラのシンクロ合成残り回数が変更された時に呼び出される
			private void HandleBaseSynchroRemainChange(object sender, EventArgs e)
			{
				this.SyncBaseSynchroRemain();
			}
			/// <summary>
			/// ベースキャラのシンクロ合成残り回数同期
			/// </summary>
			private void SyncBaseSynchroRemain()
			{
				if (!CanUpdate) { return; }
				this.View.SetBaseSynchroRemain(this.Model.BaseSynchroRemain.ToString());
			}
			#endregion

			#region ランク
			/// <summary>
			/// ベースキャラのランクが変更された時に呼び出される
			/// </summary>
			private void HandleBaseRankChange(object sender, EventArgs e)
			{
				this.SyncBaseRank();
			}
			/// <summary>
			/// ベースキャラのランク同期
			/// </summary>
			private void SyncBaseRank()
			{
				if (!CanUpdate) { return; }
				this.View.SetBaseRank(this.Model.BaseRank.ToString());
			}
			#endregion

			/// <summary>
			/// ベースキャラのステータスを同期させる
			/// </summary>
			private void SyncBaseStatus()
			{
				this.SyncBaseSynchroRemain();
				this.SyncBaseRank();
			}
			#endregion

			#region ベースキャラのキャラ情報更新
			/// <summary>
			/// ベースキャラのキャラ情報を更新する
			/// </summary>
			private void UpdateBaseCharaInfo(List<CharaInfo> charaInfoList)
			{
				if (charaInfoList == null || this.BaseChara == null) { return; }

				var info = this.BaseChara.GetCharaInfo();
				if (info == null) { return; }

				// ベースキャラのキャラ情報を更新する
				var newInfo = charaInfoList.Find(t => { return t.UUID == info.UUID; });
				this.SetBaseCharaItem(this.BaseChara, newInfo);
			}
			/// <summary>
			/// ベースキャラ情報更新
			/// </summary>
			private void UpdateBaseCharaInfo(CharaInfo charaInfo)
			{
				if (charaInfo == null || this.BaseChara == null) { return; }

				var info = this.BaseChara.GetCharaInfo();
				if (info == null) { return; }

				// ベースキャラのキャラ情報を更新する
				var newInfo = charaInfo;
				if (newInfo.UUID == info.UUID)
				{
					this.SetBaseCharaItem(this.BaseChara, newInfo);
				}
			}
			#endregion

			#region ベースキャラ選択
			/// <summary>
			/// ベースキャラ選択
			/// </summary>
			private void SetSelectBaseChara(bool isSelect)
			{
				if (this.BaseChara == null) { return; }
				// ベースキャラ選択状態に設定
				this.BaseChara.SetSelect(isSelect);
			}
			#endregion
			#endregion

			#region 進化素材リスト
			#region 進化素材リスト同期
			/// <summary>
			/// 進化素材リストの全てのアイテムに変更があった時に呼び出される
			/// </summary>
			private void MaterialListUpdateItemsEvent()
			{
				// 進化素材リスト内アイテム同期
				this.SyncMaterialListItems();
				// 進化素材リスト同期
				this.SyncMaterialList();
			}

			/// <summary>
			/// 進化素材リストのアイテムを全て同期させる
			/// </summary>
			private void SyncMaterialListItems()
			{
				if (this.MaterialList == null) { return; }
				// 進化素材マスタ取得
				List<CharaEvolutionMaterialMasterData> dataList;
				this.TryGetMaterialMasterData(out dataList);
				// 所持リスト取得
				Dictionary<int, List<CharaInfo>> charaInfoHaveList = this.GetMaterialHaveList();
				foreach (var item in this.MaterialList.GetNowPageItemList())
				{
					this.SyncMaterialListItem(item, dataList, charaInfoHaveList);
				}
			}

			/// <summary>
			/// 進化素材リスト同期
			/// </summary>
			private void SyncMaterialList()
			{
				if (this.MaterialList == null) { return; }

				// 合成試算
				this.FusionCalc();
				// ヘルプメッセージ更新
				this.UpdateHelpMessage();
			}

			/// <summary>
			/// 進化素材リストアイテム同期
			/// </summar
			private void SyncMaterialListItem(GUICharaItem item, List<CharaEvolutionMaterialMasterData> dataList, Dictionary<int, List<CharaInfo>> charaInfoHaveList)
			{
				if (item == null) { return; }
				
				this.SyncMaterialItemRank(item, dataList);
				this.SyncMaterialListItemPossession(item, charaInfoHaveList);
			}

			/// <summary>
			/// 進化素材アイテムの素材ランクを同期
			/// </summary
			private void SyncMaterialItemRank(GUICharaItem item, List<CharaEvolutionMaterialMasterData> dataList)
			{
				// 素材ランクをセット
				int materialRank = 0;
				foreach (var data in dataList)
				{
					if (item.GetCharaInfo() != null &&
							item.GetCharaInfo().CharacterMasterID == data.MaterialCharacterMasterId)
					{
							materialRank = data.MaterialRank;
					}
				}
				item.SetMaterialRank(materialRank);
				if(item.GetCharaInfo() != null)
				{
					item.SetRankColor(materialRank);
				}
			}

			/// <summary>
			/// 進化素材リストのアイテムの所持設定の同期
			/// </summary>
			private void SyncMaterialListItemPossession(GUICharaItem item, Dictionary<int, List<CharaInfo>> charaInfoHaveList)
			{
				if (this.CharaList == null) { return; }

				// 所持状態を取得してアイテムにセットする
				//CharaItem.Controller.PossessionStateType state = this.GetItemPossessionStateOld(item);
				CharaItem.Controller.PossessionStateType state = this.GetItemPossessionState(item, charaInfoHaveList);
				item.SetPossessionState(state);
			}

			///// <summary>
			///// 所持状態を取得
			///// UI改修前の仕様 改修前の状態を確認するため、残しておく
			///// </summary>
			private CharaItem.Controller.PossessionStateType GetItemPossessionStateOld(GUICharaItem item)
			{
				if (this.IsEmptyBaseChara)
				{
					// ベースキャラが設定されていない場合は所持設定なし
					return CharaItem.Controller.PossessionStateType.None;
				}
				else
				{
					if (item == null || item.GetCharaInfo() == null || item.GetCharaInfo().UUID > 0)
					{
						// 素材状態ではない 所持設定はNone
						return CharaItem.Controller.PossessionStateType.None;
					}

					if (!CharaInfo.IsMaxLevel(this.BaseCharaInfo.Rank, this.BaseCharaInfo.PowerupLevel))
					{
						// ベースキャラLV不足のため進化不能 状態は未所持設定
						return CharaItem.Controller.PossessionStateType.NotPossession;
					}

					CharaItem.Controller.PossessionStateType state = CharaItem.Controller.PossessionStateType.NotPossession;
					foreach (var info in this.CharaList.GetCharaInfo())
					{
						// 進化素材を所持しているか検索
						if (info == null) { continue; }
						if (info.AvatarType == item.GetCharaInfo().AvatarType && info.Rank >= item.GetMaterialRank())
						{
							// 所持している
							state = CharaItem.Controller.PossessionStateType.Possession;
							break;
						}
					}

					return state;
				}
			}

			/// <summary>
			///  所持状態を取得
			/// </summary>
			private CharaItem.Controller.PossessionStateType GetItemPossessionState(GUICharaItem item, Dictionary<int, List<CharaInfo>> charaInfoHaveList)
			{
				var state = CharaItem.Controller.PossessionStateType.None;

				CharaInfo materialInfo = item.GetCharaInfo();
				if (materialInfo == null || materialInfo.UUID > 0)
				{
					// 設定されている素材
					return CharaItem.Controller.PossessionStateType.None;
				}

				List<CharaInfo> haveList;
				if(charaInfoHaveList.TryGetValue(materialInfo.CharacterMasterID, out haveList))
				{
					if(haveList.Count <= 0)
					{
						// 素材にすることが出来ない
						return CharaItem.Controller.PossessionStateType.NotPossession;
					}
				}
				else
				{
					// 所持していない
					return CharaItem.Controller.PossessionStateType.NotPossession;
				}

				return state;
			}
			#endregion

			#region 進化素材アイテムイベント
			/// <summary>
			/// 進化素材リストのアイテムが押された時に呼び出される
			/// </summary>
			private void HandleMaterialListItemClickEvent(GUICharaItem item)
			{
				if (this.CharaList == null || item.GetCharaInfo() == null) { return; }

				// 選択する進化素材のアイテムを設定
				CharaItem.Controller.ItemStateType state = item.GetState();
				if(state == CharaItem.Controller.ItemStateType.Mono)
				{
					if(!item.GetSelect())
					{
						// 選択されていない素材アイテムでなら選択する
						this.SetMaterialItemSelect(item);
					}
				}
				else if(state == CharaItem.Controller.ItemStateType.Icon)
				{
					// セットされている素材を解除
					this.RemoveBaitMaterialChara(item.GetCharaInfo());
				}

				// 元のキャラ情報を更新
				this.UpdateCharaInfoList();
			}
			#endregion

			#region 進化素材追加
			/// <summary>
			/// 進化素材リストに素材を追加
			/// </summary>
			/// <param name="charaInfo"></param>
			private void AddBaitMaterialChara(CharaInfo charaInfo)
			{
				if (this.MaterialList == null) { return; }
				// 素材追加
				bool isSuccess = this.MaterialList.SetBaitMaterial(charaInfo);
				if(isSuccess)
				{
					// 追加成功なら選択アイテム更新
					this.SetNextMaterialItemSelect();
				}
			}
			#endregion

			#region 進化素材削除
			/// <summary>
			/// 進化素材リストから素材を削除する
			/// </summary>
			private void RemoveBaitMaterialChara(CharaInfo charaInfo)
			{
				if (this.MaterialList == null || charaInfo == null) { return; }
				// 削除
				GUICharaItem removeMaterialItem = this.MaterialList.RemoveBaitMaterial(charaInfo);
				if (removeMaterialItem != null)
				{
					// 削除成功なら削除したアイテムを選択状態にする
					this.SetMaterialItemSelect(removeMaterialItem);
				}
			}
			#endregion

			#region 進化素材クリア
			/// <summary>
			/// 進化素材リストクリア
			/// </summary>
			private void ClearMaterialList()
			{
				if (this.MaterialList == null) { return; }
				// クリア
				bool isSuccess = this.MaterialList.ClearMaterial();
				if(isSuccess)
				{
					// 選択アイテム解除
					this.MaterialList.SelectItem = null;
				}
			}
			#endregion

			#region ベースキャラの進化素材一覧セット
			/// <summary>
			/// ベースキャラの進化素材アイテムをセットする
			/// </summary>
			private void SetBaseCharaMaterialList()
			{
				// 進化素材がすでにセットされている状態ならセット処理を行わない
				if (this.MaterialList == null || this.IsEmptyBaseChara || this.MaterialList.GetCharaInfoList().Count > 0) { return; }
				// アイテム設定
				this.MaterialList.SetupItems(this.BaseCharaInfo, this.GetMaterialHaveList());
				// 進化素材アイテム選択
				this.SetNextMaterialItemSelect();
			}

			/// <summary>
			/// 所持キャラの中から素材可能な一覧を取得
			/// Key = 素材にするキャラマスターデータID
			/// Value = 素材可能な所持しているキャラ一覧
			/// </summary>
			private Dictionary<int, List<CharaInfo>> GetMaterialHaveList()
			{
				// 所持キャラの中から素材可能な一覧を取得
				var materialDic = new Dictionary<int, List<CharaInfo>>();
				var dataList = new List<CharaEvolutionMaterialMasterData>();
				if (!this.TryGetMaterialMasterData(out dataList)) { return materialDic; }
				foreach (var data in dataList)
				{
					if (!materialDic.ContainsKey(data.MaterialCharacterMasterId))
					{
						materialDic.Add(data.MaterialCharacterMasterId, new List<CharaInfo>());
					}

					var uuidDic = new Dictionary<ulong, CharaInfo>();
					if (this.Model.TryGetCharaInfoByMasterId(data.MaterialCharacterMasterId, out uuidDic))
					{
						foreach (var info in uuidDic.Values)
						{
							List<CharaInfo> list;
							if (materialDic.TryGetValue(data.MaterialCharacterMasterId, out list))
							{
								CharaItem.Controller.DisableType disableType;
								int baitIndex;
								this.GetMaterialCharaDisableType(info, dataList, out disableType, out baitIndex);
								if (disableType == CharaItem.Controller.DisableType.None ||
									disableType == CharaItem.Controller.DisableType.Bait ||
									disableType == CharaItem.Controller.DisableType.NotSelected)
								{
									list.Add(info);
								}
							}
						}
					}
				}

				return materialDic;
			}

			/// <summary>
			/// 所持していない素材キャラがいるかどうか
			/// </summary>
			private bool IsHaveMaterialChara()
			{
				var haveDic = this.GetMaterialHaveList();
				foreach (var list in haveDic.Values)
				{
					if (list.Count <= 0)
					{
						// 所持していない素材キャラがいる
						return false;
					}
				}

				return true;
			}
			#endregion

			#region 素材選択
			/// <summary>
			/// 進化素材リストのアイテムを選択する
			/// </summary>
			/// <param name="selectItem"></param>
			private void SetMaterialItemSelect(GUICharaItem selectItem)
			{
				if (this.MaterialList == null) { return; }

				if (selectItem.GetPossessionState() == CharaItem.Controller.PossessionStateType.None)
				{
					if (this.CanSelectItem(selectItem, this.GetMaterialHaveList()))
					{
						// 指定されたアイテムが選択可能状態なら選択
						this.MaterialList.SelectItem = selectItem;
					}
				}
				else
				{
					// 指定されたアイテムが選択不能なら次の選択するアイテムを設定
					this.SetNextMaterialItemSelect();
				}
			}

			/// <summary>
			/// 進化素材リストの次に選択するアイテムを選択状態にセットする
			/// </summary>
			private void SetNextMaterialItemSelect()
			{
				if (this.MaterialList == null) { return; }

				GUICharaItem currentSelectItem = this.MaterialList.SelectItem;
				if (currentSelectItem != null && currentSelectItem.GetCharaInfo() != null)
				{
					// 現在選択されているアイテムが存在する
					if (currentSelectItem.GetCharaInfo().UUID <= 0 && currentSelectItem.GetPossessionState() == CharaItem.Controller.PossessionStateType.None)
					{
						// 選択されているアイテムが素材状態で所持している状態なら継続して選択し続ける
						return;
					}
				}

				GUICharaItem selectItem = null;
				GUICharaItem materialSetLastItem = null;

				// 次に選択する素材アイテムを検索
				var itemList = this.MaterialList.GetNowPageItemList();
				Dictionary<int, List<CharaInfo>> haveDic = this.GetMaterialHaveList();
				foreach(var item in itemList)
				{
					if (item.GetCharaInfo() == null) { continue; }

					if (item.GetCharaInfo().UUID == 0)
					{
						if (this.CanSelectItem(item, haveDic))
						{
							// アイテムが素材状態かつ選択可能なら選択状態にする
							selectItem = item;
							break;
						}
					}
					else
					{
						// 素材がセットされている最後尾のアイテムを保存
						materialSetLastItem = item;
					}
				}

				// 次に選択素材アイテムが存在しない場合はすでに素材がセットされているアイテムを選択状態にする
				if(selectItem == null)
				{
					if (currentSelectItem != null && currentSelectItem.GetPossessionState() == CharaItem.Controller.PossessionStateType.None)
					{
						// 現在選択されているアイテムを継続して選択
						selectItem = currentSelectItem;
					}
					else
					{
						// 最後尾のアイテムを選択
						selectItem = materialSetLastItem;
					}
				}

				this.MaterialList.SelectItem = selectItem;
			}

			/// <summary>
			/// 進化素材リストで選択可能なアイテムかを取得
			/// </summary>
			private bool CanSelectItem(GUICharaItem selectItem, Dictionary<int, List<CharaInfo>> haveDic)
			{
				// すでに選択されているの選択不能
				if (selectItem.GetSelect()) { return false; }

				var itemCharaInfo = selectItem.GetCharaInfo();
				if (itemCharaInfo == null) { return false; }

				// 素材を所持しているかチェック
				List<CharaInfo> haveCharaInfoList;
				if (haveDic.TryGetValue(itemCharaInfo.CharacterMasterID, out haveCharaInfoList))
				{
					if (haveCharaInfoList.Count > 0) { return true; }
				}

				return false;
			}
			#endregion

			#region 進化素材リストのキャラ情報更新
			/// <summary>
			/// 進化素材リスト内のキャラ情報を更新する
			/// </summary>
			private void UpdateMaterialCharaInfoList(List<CharaInfo> charaInfoList)
			{
				if (charaInfoList == null || this.MaterialList == null) { return; }
				this.MaterialList.UpdateBaitMaterialCharaInfo(charaInfoList);
				this.SetNextMaterialItemSelect();
			}
			#endregion
			#endregion

			#region 進化キャラ
			#region 進化キャラ同期
			/// <summary>
			/// 進化キャラアイテムに変更があった時に呼び出される
			/// </summary>
			private void HandleEvolutionCharaItemChangeEvent(GUICharaItem obj)
			{
				this.SyncEvolutionChara();
			}
			/// <summary>
			/// 進化キャラ同期
			/// </summary>
			private void SyncEvolutionChara()
			{
				// 進化キャラのステータスをセット
				SetEvolutionCharaStatus();
			}

			/// <summary>
			/// 設定されている進化キャラからステータスをデータにセットする
			/// </summary>
			private void SetEvolutionCharaStatus()
			{
				if (this.Model == null) { return; }

				var charaInfo = this.EvolutionChara.GetCharaInfo();
				if (charaInfo != null)
				{
					int remain = charaInfo.SynchroRemain;
					int rank = charaInfo.Rank;

					// 進化キャラステータス更新
					this.Model.EvolutionSynchroRemain = remain;
					this.Model.EvolutionRank = rank;
					this.View.SetEvolutionStatusActive(true);
				}
				else
				{
					// 進化キャラが設定されていない場合はステータスを非表示にする
					this.View.SetEvolutionStatusActive(false);
				}
			}
			#endregion

			#region 進化キャラアイテム
			/// <summary>
			/// 進化キャラ設定
			/// </summary>
			private void SetEvolutionChara(CharaInfo baseCharaInfo)
			{
				CharaInfo evolutionInfo = null;
				if (baseCharaInfo != null)
				{
					// 進化後のキャラ情報を求める
					AvatarType avatarType = baseCharaInfo.AvatarType;
					int rank = Math.Min(baseCharaInfo.Rank + 1, MasterDataCommonSetting.Player.PlayerMaxRank);
					int synchroRemain = Math.Min(baseCharaInfo.SynchroRemain + 1, MasterDataCommonSetting.Fusion.MaxSynchroCount);
					int powerupLv = (baseCharaInfo.Rank == rank) ? baseCharaInfo.PowerupLevel : 1;
					bool isLock = baseCharaInfo.IsLock;

					// 進化後のキャラ情報生成
					evolutionInfo = new CharaInfo(avatarType, rank, synchroRemain, powerupLv, isLock);
				}

				// セット
				this.SetEvolutionCharaItem(evolutionInfo);
			}

			/// <summary>
			/// 進化キャラを外す
			/// </summary>
			private void ClearEvolutionChara()
			{
				this.SetEvolutionChara(null);
			}

			/// <summary>
			/// 進化キャラアイテムを設定する
			/// </summary>
			private void SetEvolutionCharaItem(CharaInfo info)
			{
				var item = this.EvolutionChara;
				if (item == null) return;

				var state = CharaItem.Controller.ItemStateType.Icon;
				if (info == null)
				{
					state = CharaItem.Controller.ItemStateType.FillEmpty;
				}
				item.SetState(state, info);
			}
			#endregion

			#region 進化キャラステータス
			#region シンクロ合成残り回数
			// 進化キャラのシンクロ合成残り回数が変更された時に呼び出される
			private void HandleEvolutionSynchroRemainChange(object sender, EventArgs e)
			{
				this.SyncEvolutionSynchroRemain();
			}
			/// <summary>
			/// 進化キャラのシンクロ合成残り回数同期
			/// </summary>
			private void SyncEvolutionSynchroRemain()
			{
				if (!CanUpdate) { return; }
				this.View.SetEvolutionSynchroRemain(this.Model.EvolutionSynchroRemain.ToString());

				if (this.Model.EvolutionSynchroRemain > this.Model.BaseSynchroRemain)
				{
					this.View.SetEvolutionSynchroRemainColor(StatusColor.Type.Up);
				}
				else if (this.Model.EvolutionSynchroRemain < this.Model.BaseSynchroRemain)
				{
					this.View.SetEvolutionSynchroRemainColor(StatusColor.Type.Down);
				}
				else
				{
					this.View.SetEvolutionSynchroRemainColor(StatusColor.Type.StatusNormal);
				}
			}
			#endregion

			#region ランク
			/// <summary>
			/// 進化キャラのランクが変更された時に呼び出される
			/// </summary>
			private void HandleEvolutionRankChange(object sender, EventArgs e)
			{
				this.SyncEvolutionRank();
			}
			/// <summary>
			/// 進化キャラのランク同期
			/// </summary>
			private void SyncEvolutionRank()
			{
				if (!CanUpdate) { return; }
				this.View.SetEvolutionRank(this.Model.EvolutionRank.ToString());

				if (this.Model.EvolutionRank > this.Model.BaseRank)
				{
					this.View.SetEvolutionRankColor(StatusColor.Type.Up);
				}
				else if (this.Model.EvolutionRank < this.Model.BaseRank)
				{
					this.View.SetEvolutionRankColor(StatusColor.Type.Down);
				}
				else
				{
					this.View.SetEvolutionRankColor(StatusColor.Type.RankNormal);
				}
			}
			#endregion

			/// <summary>
			/// 進化キャラのステータスを同期させる
			/// </summary>
			private void SyncEvolutionStatus()
			{
				this.SyncEvolutionSynchroRemain();
				this.SyncEvolutionRank();
			}
			#endregion

			#region 進化キャラのキャラ情報更新
			/// <summary>
			/// 進化キャラのキャラ情報を更新する
			/// </summary>
			private void UpdateEvolutionCharaInfo(List<CharaInfo> charaInfoList)
			{
				if (charaInfoList == null || this.EvolutionChara == null || this.EvolutionChara.GetCharaInfo() == null) { return; }

				var info = this.BaseCharaInfo;
				if (info == null) { return; }

				// 進化キャラのキャラ情報を更新する
				var newInfo = charaInfoList.Find(t => { return t.UUID == info.UUID; });
				this.SetEvolutionChara(newInfo);
			}
			#endregion
			#endregion

			#region 合成試算
			/// <summary>
			/// 合成試算処理
			/// </summary>
			private void FusionCalc()
			{
				if (this.BaseChara == null || this.MaterialList == null) { return; }

				if (this.MaterialList.CanEvolution)
				{
					// 進化可能状態なら試算パケット送信処理を行う
					var eventArgs = new FusionCalcEventArgs();
					eventArgs.BaseCharaUUID = this.BaseCharaUUID;
					eventArgs.BaitCharaUUIDList = this.MaterialList.GetBaitCharaUUIDList();

					// 通知
					this.OnFusionCalc(this, eventArgs);
				}
				else
				{
					// 進化不可
					if(this.Model != null)
					{
						this.Model.NeedMoney = 0;
						this.Model.AddOnCharge = 0;
					}
					// 合成ボタン状態更新
					this.UpdateFusionButtonEnable();
					// 進化キャラを外す
					this.ClearEvolutionChara();
				}
			}

			/// <summary>
			/// 合成試算結果
			/// </summary>
			public void FusionCalcResult(bool result, int money, int price, int addOnCharge)
			{
				int setNeedMoney = 0;
				int setAddOnCharge = 0;
				if (result)
				{
					setNeedMoney = price;
					setAddOnCharge = addOnCharge;

					// 進化キャラセット
					this.SetEvolutionChara(this.BaseCharaInfo);
				}
				else
				{
					// 進化キャラを外す
					this.ClearEvolutionChara();
				}

				if(this.Model != null)
				{
					// データセット
					this.Model.NeedMoney = setNeedMoney;
					this.Model.AddOnCharge = setAddOnCharge;
				}
				// 合成ボタン状態更新
				this.UpdateFusionButtonEnable();
			}
			#endregion

			#region 合成
			#region 合成ボタンイベント
			/// <summary>
			/// 合成ボタンが押された時に呼び出される
			/// </summary>
			private void HandleFusion(object sender, EventArgs e)
			{
				// スロット情報取得のためベースキャラ情報をサーバーから取得する
				this.SendPlayerCharacter();
			}
			#endregion

			/// <summary>
			/// 合成処理
			/// </summary>
			private void Fusion(List<Scm.Common.XwDb.IPlayerCharacter> baseCharaSlotList)
			{
				if (this.BaseChara == null || this.MaterialList == null) { return; }

				// すべての進化素材がセットされているかチェック
				if (!this.MaterialList.CanEvolution) { return; }

				// 合成メッセージを表示するためのデータを作成
				var setupParam = this.GetFusionMessageParam(baseCharaSlotList);

				// 進化合成チェック処理へ
				this.CheckFusion(setupParam);
			}

			/// <summary>
			/// 合成メッセージに表示するためのデータを取得する
			/// </summary>
			private FusionMessage.SetupParam GetFusionMessageParam(List<Scm.Common.XwDb.IPlayerCharacter> baseCharaSlotList)
			{
				// ベースと進化キャラ情報取得
				if (this.EvolutionChara == null) { return null; }
				var evolutionCharaInfo = this.EvolutionChara.GetCharaInfo();
				var baseCharaInfo = this.BaseCharaInfo;
				if (evolutionCharaInfo == null || baseCharaInfo == null) { return null; }
				// 進化キャラの強化スロット情報作成
				var evolutionCharaPowerupSlot = new Scm.Common.Fusion.PowerupSlot(evolutionCharaInfo.CharacterMasterID, evolutionCharaInfo.Rank, evolutionCharaInfo.PowerupLevel, baseCharaInfo.SynchroHitPoint, baseCharaInfo.SynchroAttack, baseCharaInfo.SynchroExtra, baseCharaInfo.SynchroDefense);
				evolutionCharaPowerupSlot.SetSlot(baseCharaSlotList);

				// データセット
				FusionMessage.SetupParam param = new FusionMessage.SetupParam();
				// ランク
				param.RankBefore = baseCharaInfo.Rank;
				param.RankAfter = evolutionCharaInfo.Rank;
				// レベルと経験値
				param.LevelBefore = baseCharaInfo.PowerupLevel;
				param.LevelAfter = evolutionCharaInfo.PowerupLevel;
				param.Exp = 0;
				param.TotalExp = 0;
				param.NextLvTotalExp = 0;
				// シンクロ可能回数
				param.SynchroRemainBefore = baseCharaInfo.SynchroRemain;
				param.SynchroRemainAfter = evolutionCharaInfo.SynchroRemain;
				// 生命力
				param.HitPointBefore = baseCharaInfo.HitPoint;
				param.HitPointAfter = evolutionCharaPowerupSlot.HitPoint;
				param.HitPointBaseBefore = baseCharaInfo.PowerupHitPoint;
				param.HitPointBaseAfter = evolutionCharaInfo.PowerupHitPoint;
				param.SynchroHitPoint = baseCharaInfo.SynchroHitPoint;
				param.SlotHitPointBefore = baseCharaInfo.SlotHitPoint;
				param.SlotHitPointAfter = baseCharaInfo.SlotHitPoint;
				// 攻撃力
				param.AttackBefore = baseCharaInfo.Attack;
				param.AttackAfter = evolutionCharaPowerupSlot.Attack;
				param.AttackBaseBefore = baseCharaInfo.PowerupAttack;
				param.AttackBaseAfter = evolutionCharaInfo.PowerupAttack;
				param.SynchroAttack = baseCharaInfo.SynchroAttack;
				param.SlotAttackBefore = baseCharaInfo.SlotAttack;
				param.SlotAttackAfter = baseCharaInfo.SlotAttack;
				// 防御力
				param.DefenseBefore = baseCharaInfo.Defense;
				param.DefenseAfter = evolutionCharaPowerupSlot.Defense;
				param.DefenseBaseBefore = baseCharaInfo.PowerupDefense;
				param.DefenseBaseAfter = evolutionCharaInfo.PowerupDefense;
				param.SynchroDefense = baseCharaInfo.SynchroDefense;
				param.SlotDefenseBefore = baseCharaInfo.SlotDefense;
				param.SlotDefenseAfter = baseCharaInfo.SlotDefense;
				// 特殊能力
				param.ExtraBefore = baseCharaInfo.Extra;
				param.ExtraAfter = evolutionCharaPowerupSlot.Extra;
				param.ExtraBaseBefore = baseCharaInfo.PowerupExtra;
				param.ExtraBaseAfter = evolutionCharaInfo.PowerupExtra;
				param.SynchroExtra = baseCharaInfo.SynchroExtra;
				param.SlotExtraBefore = baseCharaInfo.SlotExtra;
				param.SlotExtraAfter = baseCharaInfo.SlotExtra;

				return param;
			}

			/// <summary>
			/// 進化合成チェック処理
			/// </summary>
			private void CheckFusion(FusionMessage.SetupParam param)
			{
				if (this.BaseChara == null) { return; }
				GUIFusionMessage.OpenYesNo(param, MasterData.GetText(TextType.TX317_Evolution_Fsuion_Message), true, this.CheckLock, null);
			}

			/// <summary>
			/// ロックチェック処理
			/// </summary>
			private void CheckLock()
			{
				// ロックチェック
				bool isLock = false;
				if (this.MaterialList != null)
				{
					var list = this.MaterialList.GetBaitCharaInfoList();
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
					if (this.BaseCharaInfo.Rank >= MasterDataCommonSetting.Player.PlayerMaxRank)
					{
						// 最大ランク以上なので進化不可 シンクロ合成回数チェック処理へ
						this.CheckSynchro();
					}
					else
					{
						// スロットチェック処理へ
						this.CheckSlot();
					}
				}
			}

			/// <summary>
			/// シンクロ合成回数チェック
			/// </summary>
			private void CheckSynchro()
			{
				if (this.BaseChara == null) { return; }

				// シンクロ合成回数分岐
				if (this.BaseCharaInfo.SynchroRemain >= MasterDataCommonSetting.Fusion.MaxSynchroCount)
				{
					// シンクロ合成回数最大なので増加不可
					GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX318_Evolution_Fusion_EmptyMessage), true, this.CheckSlot, null);
				}
				else
				{
					// シンクロ合成回数増加可能
					GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX319_Evolution_Fusion_SynchroUpMessage), true, this.CheckSlot, null);
				}

			}

			/// <summary>
			/// スロットチェック
			/// </summary>
			private void CheckSlot()
			{
				// スロット分岐
				if (this.Model.AddOnCharge > 0)
				{
					// 餌キャラに一つでもスロットが装着されている
					string addOnCharge = string.Format(this.Model.AddOnChargeFormat, this.Model.AddOnCharge);
					string text = string.Format(MasterData.GetText(TextType.TX310_Powerup_Fusion_SlotMessage), addOnCharge);
					GUIMessageWindow.SetModeYesNo(text, true, this.CheckHighRank, null);
				}
				else
				{
					// 高ランクチェック処理へ
					this.CheckHighRank();
				}
			}

			/// <summary>
			/// 餌キャラに指定ランクより大きいランクがないかチェック
			/// </summary>
			private void CheckHighRank()
			{
				bool isInHighRank = false;
				if (this.MaterialList != null)
				{
					// マスターデータ取得
					List<CharaEvolutionMaterialMasterData> masterDataList;
					if (this.TryGetMaterialMasterData(out masterDataList))
					{
						var list = this.MaterialList.GetBaitCharaInfoList();
						foreach (var info in list)
						{
							if (info == null) continue;
							foreach (var data in masterDataList)
							{
								if (info.CharacterMasterID != data.MaterialCharacterMasterId ||
									info.Rank <= data.MaterialRank)
								{
									continue;
								}

								isInHighRank = true;
								break;
							}
						}
					}
				}

				// 高ランク分岐
				if (isInHighRank)
				{
					GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX320_Evolution_Fusion_HighRankMessage), true, this.FusionExecute, null);
				}
				else
				{
					// 進化合成実行
					this.FusionExecute();
				}
			}

			/// <summary>
			/// 進化合成実行処理
			/// </summary>
			private void FusionExecute()
			{
				if (this.MaterialList == null) { return; }

				// 通知
				var eventArgs = new FusionEventArgs();
				eventArgs.BaseCharaUUID = this.BaseCharaUUID;
				eventArgs.BaitCharaUUIDList = this.MaterialList.GetBaitCharaUUIDList();

				this.OnFusion(this, eventArgs);
			}

			/// <summary>
			/// 合成結果
			/// </summary>
			public void FusionResult(bool result, int money, int price, int addOnCharge, int synchroBonus, CharaInfo charaInfo)
			{
				if(result)
				{
					// リザルト画面表示
					this.OpenResult(synchroBonus, charaInfo);

					// 素材一覧をクリア
					this.ClearMaterialList();
				}
			}

			/// <summary>
			/// リザルト画面を表示
			/// </summary>
			private void OpenResult(int synchroBonus, CharaInfo charaInfoAfter)
			{
				// ベースキャラ情報取得
				CharaInfo charaInfoBefore = this.BaseCharaInfo;
				if (charaInfoBefore == null || charaInfoAfter == null) { return; }

				var p = new FusionResult.SetupParam();
				// アバタータイプ
				p.AvatarType = charaInfoAfter.AvatarType;
				// ランク
				p.RankBefore = charaInfoBefore.Rank;
				p.RankAfter = charaInfoAfter.Rank;
				// レベル
				p.LevelBefore = charaInfoBefore.PowerupLevel;
				p.LevelAfter = charaInfoAfter.PowerupLevel;
				// 経験値
				p.Exp = charaInfoAfter.PowerupExp;
				p.TotalExp = CharaInfo.GetTotalExp(charaInfoBefore.Rank, charaInfoAfter.PowerupLevel);
				p.NextLvTotalExp = CharaInfo.GetTotalExp(charaInfoAfter.Rank, charaInfoAfter.PowerupLevel + 1);
				// シンクロ可能回数
				p.SynchroRemainBefore = charaInfoBefore.SynchroRemain;
				p.SynchroRemainAfter = charaInfoAfter.SynchroRemain;
				// 生命力
				p.HitPointBefore = charaInfoBefore.HitPoint;
				p.HitPointAfter = charaInfoAfter.HitPoint;
				p.HitPointBaseBefore = charaInfoBefore.PowerupHitPoint;
				p.HitPointBaseAfter = charaInfoAfter.PowerupHitPoint;
				p.SynchroHitPointBefore = charaInfoBefore.SynchroHitPoint;
				p.SynchroHitPointAfter = charaInfoAfter.SynchroHitPoint;
				p.SlotHitPointBefore = charaInfoBefore.SlotHitPoint;
				p.SlotHitPointAfter = charaInfoAfter.SlotHitPoint;
				// 攻撃力
				p.AttackBefore = charaInfoBefore.Attack;
				p.AttackAfter = charaInfoAfter.Attack;
				p.AttackBaseBefore = charaInfoBefore.PowerupAttack;
				p.AttackBaseAfter = charaInfoAfter.PowerupAttack;
				p.SynchroAttackBefore = charaInfoBefore.SynchroAttack;
				p.SynchroAttackAfter = charaInfoAfter.SynchroAttack;
				p.SlotAttackBefore = charaInfoBefore.SlotAttack;
				p.SlotAttackAfter = charaInfoAfter.SlotAttack;
				// 防御力
				p.DefenseBefore = charaInfoBefore.Defense;
				p.DefenseAfter = charaInfoAfter.Defense;
				p.DefenseBaseBefore = charaInfoBefore.PowerupDefense;
				p.DefenseBaseAfter = charaInfoAfter.PowerupDefense;
				p.SynchroDefenseBefore = charaInfoBefore.SynchroDefense;
				p.SynchroDefenseAfter = charaInfoAfter.SynchroDefense;
				p.SlotDefenseBefore = charaInfoBefore.SlotDefense;
				p.SlotDefenseAfter = charaInfoAfter.SlotDefense;
				// 特殊能力
				p.ExtraBefore = charaInfoBefore.Extra;
				p.ExtraAfter = charaInfoAfter.Extra;
				p.ExtraBaseBefore = charaInfoBefore.PowerupExtra;
				p.ExtraBaseAfter = charaInfoAfter.PowerupExtra;
				p.SynchroExtraBefore = charaInfoBefore.SynchroExtra;
				p.SynchroExtraAfter = charaInfoAfter.SynchroExtra;
				p.SlotExtraBefore = charaInfoBefore.SlotExtra;
				p.SlotExtraAfter = charaInfoAfter.SlotExtra;

				// 演出
				var param = new EvolutionDirection.EvolutionDirectionParam();
				param.BaseCharaId = charaInfoAfter.CharacterMasterID;
				var infoList = MaterialList.GetCharaInfoList();
				if(infoList != null) {
					int []matIds = new int[infoList.Count];
					for(int i = 0; i < infoList.Count; i++) {
						matIds[i] = infoList[i].CharacterMasterID;
					}
					param.MaterialIds = matIds;
				}
				param.OldRank = charaInfoBefore.Rank;
				param.NewRank = charaInfoAfter.Rank;
				param.SynchroRemainUpCount = charaInfoAfter.SynchroRemain - charaInfoBefore.SynchroRemain;


				// 一度非表示
				this.SetActive(false, true);

				GUIEvolutionDirection.Open(param, () =>
				{
					// 合成リザルト画面表示
					var screen = new GUIScreen(() => { GUIFusionResult.Open(p); }, GUIFusionResult.Close, GUIFusionResult.ReOpen);
					GUIController.Open(screen);
				});

			}
			#endregion

			#region プレイヤーキャラクター取得
			/// <summary>
			/// プレイヤーキャラクター情報を取得
			/// </summary>
			private void SendPlayerCharacter()
			{
				// ベースキャラ情報を取得し直す
				if (this.BaseCharaInfo == null) { return; }
				// 通知
				var eventArgs = new PlayerCharacterEventArgs();
				eventArgs.UUID = this.BaseCharaInfo.UUID;
				this.OnPlayerCharacter(this, eventArgs);
			}

			/// <summary>
			/// プレイヤーキャラクター情報を設定する
			/// </summary>
			public void SetupPlayerCharacterInfo(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList)
			{
				if (!this.CanUpdate) { return; }
				if (this.BaseCharaUUID != info.UUID) { return; }

				// ベースキャラ情報更新
				this.UpdateBaseCharaInfo(info);

				// ベースキャラに装着されているスロット情報リストを作成
				var powerupSlotList = new List<Scm.Common.XwDb.IPlayerCharacter>();
				foreach(var slotInfo in slotList)
				{
					if (slotInfo == null) { continue; }
					var powerupSlot = new Scm.Common.Fusion.PowerupSlot(slotInfo.CharacterMasterID, slotInfo.Rank, slotInfo.PowerupLevel, slotInfo.SynchroHitPoint, slotInfo.SynchroAttack, slotInfo.SynchroExtra, slotInfo.SynchroDefense);
					powerupSlotList.Add(powerupSlot);
				}

				// 合成処理
				this.Fusion(powerupSlotList);
			}
			#endregion

			#region 所持金
			/// <summary>
			/// 所持金が変更された時に呼び出される
			/// </summary>
			private void HandleHaveMoneyChange(object sender, EventArgs e)
			{
				this.SyncHaveMoney();
			}
			/// <summary>
			/// 所持金フォーマットが変更された時に呼び出される
			/// </summary>
			void HandleHaveMoneyFormatChange(object sender, EventArgs e)
			{
				this.SyncHaveMoney();
			}

			/// <summary>
			/// 所持金同期
			/// </summary>
			private void SyncHaveMoney()
			{
				if (this.CanUpdate)
				{
					this.View.SetHaveMoney(this.Model.HaveMoney, this.Model.HaveMoneyFormat);
				}
			}
			#endregion

			#region 費用
			/// <summary>
			/// 費用が変更された時に呼び出される
			/// </summary>
			private void HandleNeedMoneyChange(object sender, EventArgs e)
			{
				this.SyncNeedMoney();
			}
			/// <summary>
			/// 費用フォーマットが変更された時に呼び出される
			/// </summary>
			private void HandleNeedMoneyFormatChange(object sender, EventArgs e)
			{
				this.SyncNeedMoney();
			}
			
			/// <summary>
			/// 費用同期
			/// </summary>
			private void SyncNeedMoney()
			{
				if (this.CanUpdate)
				{
					this.View.SetNeedMoney(this.Model.NeedMoney, this.Model.NeedMoneyFormat);
				}
			}
			#endregion

			#region ホーム、閉じるボタンイベント
			/// <summary>
			/// ホーム
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleHome(object sender, HomeClickedEventArgs e)
			{
				if(this.CharaList != null)
				{
					// Newフラグ一括解除
					this.CharaList.DeleteAllNewFlag();
				}

				GUIController.Clear();
			}

			/// <summary>
			/// 閉じる
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleClose(object sender, CloseClickedEventArgs e)
			{
				if (this.CharaList != null)
				{
					// Newフラグ一括解除
					this.CharaList.DeleteAllNewFlag();
				}

				GUIController.Back();
			}
			#endregion

			#region 進化素材
			/// <summary>
			/// 進化素材マスターデータを取得する
			/// </summary>
			private bool TryGetMaterialMasterData(out List<CharaEvolutionMaterialMasterData> dataList)
			{
				dataList = new List<CharaEvolutionMaterialMasterData>();
				if (this.BaseCharaInfo == null) { return false; }
				// 進化合成素材マスタ取得
				if (!MasterData.TryGetCharaEvolutionMaterial((int)this.BaseCharaInfo.AvatarType, Math.Min(this.BaseCharaInfo.Rank + 1, 5), out dataList))
				{
					// マスタデータ取得失敗
					string msg = string.Format("CharaEvolutionMaterialMaster NotFound. AvatarType = {0} Rank = {1}", this.BaseCharaInfo.AvatarType, this.BaseCharaInfo.Rank);
					BugReportController.SaveLogFile(msg);
					UnityEngine.Debug.LogWarning(msg);
					return false;
				}

				return true;
			}

			/// <summary>
			/// 進化素材かどうかを返す
			/// </summary>
			private bool IsMaterial(CharaInfo targetInfo, List<CharaEvolutionMaterialMasterData> materialMasterList)
			{
				if (targetInfo == null) { return false; }

				// 進化素材かどうかチェック
				foreach (var masterData in materialMasterList)
				{
					if ((int)targetInfo.AvatarType == masterData.MaterialCharacterMasterId)
					{
						return true;
					}
				}

				return false;
			}
			#endregion

			#region キャラランクマスタ
			/// <summary>
			/// キャラランクマスターデータを取得する
			/// </summary>
			private bool TryGetCharaRankMasterData(int charaId, int rank, out CharaRankMasterData masterData)
			{
				masterData = null;
				if(MasterData.TryGetCharaRank(charaId, rank, out masterData))
				{
					return true;
				}
				else
				{
					// 取得失敗
					string msg = string.Format("CharaRankMasteData NotFound CharaID={0} Rank={2}", charaId, rank);
					UnityEngine.Debug.LogWarning(msg);
					BugReportController.SaveLogFile(msg);
					return false;
				}
			}
			#endregion
		}
	}
}