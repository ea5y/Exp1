/// <summary>
/// シンクロ合成制御
/// 
/// 2016/02/24
/// </summary>
using System;
using System.Collections.Generic;
using Scm.Common.XwMaster;

namespace XUI
{
	namespace Synchro
	{
		#region イベント引数
		/// <summary>
		/// 合成イベント引数
		/// </summary>
		public class FusionEventArgs : EventArgs
		{
			public ulong BaseCharaUUID { get; set; }
			public ulong BaitCharaUUID { get; set; }
		}

		/// <summary>
		/// 合成試算イベント引数
		/// </summary>
		public class FusionCalcEventArgs : EventArgs
		{
			public ulong BaseCharaUUID { get; set; }
			public ulong BaitCharaUUID { get; set; }
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
		/// シンクロ合成制御インターフェイス
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
			/// 破棄
			/// </summary>
			void Dispose();

			/// <summary>
			/// 初期化
			/// </summary>
			void Setup();

			/// <summary>
			/// ステータス情報設定
			/// </summary>
			void SetupStatusInfo(int haveMoney);
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
		/// シンクロ合成制御
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
					if (this.Model == null) { return false; }
					if (this.View == null) { return false; }
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

			/// <summary>
			/// ベースキャラのUUID
			/// </summary>
			private ulong BaseCharaUUID
			{
				get
				{
					ulong uuid = 0;
					var info = this.BaseCharaInfo;
					if (info != null) { uuid = info.UUID; }
					return uuid;
				}
			}

			/// <summary>
			/// ベースキャラ情報
			/// </summary>
			private CharaInfo BaseCharaInfo { get { return (this.BaseChara != null ? this.BaseChara.GetCharaInfo() : null); } }

			/// <summary>
			/// ベースキャラが空かどうか
			/// </summary>
			private bool IsEmptyBaseChara { get { return (this.BaseCharaInfo == null ? true : false); } }

			/// <summary>
			/// 餌キャラ
			/// </summary>
			private readonly GUICharaItem _baitChara;
			private GUICharaItem BaitChara { get { return _baitChara; } }

			/// <summary>
			/// 餌キャラのUUID
			/// </summary>
			private ulong BaitCharaUUID
			{
				get
				{
					ulong uuid = 0;
					var info = this.BaitChara != null ? this.BaitChara.GetCharaInfo() : null;
					if (info != null) { uuid = info.UUID; }
					return uuid;
				}
			}
			
			/// <summary>
			/// 餌キャラが空かどうか(素材指定状態になっている時も空判定となる)
			/// </summary>
			private bool IsEmptyBaitChara { get { return (this.BaitCharaUUID <= 0 ? true : false); } }

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
			public Controller(IModel model, IView view, GUICharaPageList charaList, GUICharaItem baseChara, GUICharaItem baitChara)
			{
				if (model == null || view == null) { return; }

				// ページリストとベースキャラアイテムと餌キャラアイテムセット
				this._charaList = charaList;
				this._baseChara = baseChara;
				this._baitChara = baitChara;

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
				this.Model.OnSynchroHitPointChange += this.HandleSynchroHitPointChange;
				this.Model.OnSynchroAttackChange += this.HandleSynchroAttackChange;
				this.Model.OnSynchroDefenseChange += this.HandleSynchroDefenseChange;
				this.Model.OnSynchroExtraChange += this.HandleSynchroExtraChange;
				this.Model.OnTotalSynchroBonusChange += this.HandleTotalSynchroBonusChange;
				this.Model.OnTotalSynchroBonusRemainChnage += this.HandleTotalSynchroBonusRemainChnage;
				this.Model.OnSynchroRemainChange += this.HandleSynchroRemainChange;
				this.Model.OnSynchroRemainColorChange += this.HandleSynchroRemainColorChange;
				this.Model.OnSynchroFromatChnage += this.HandleSynchroFromatChnage;
				this.Model.OnSynchroUpFromatChnage += this.HandleSynchroUpFromatChnage;
				this.Model.OnSynchroMaxFromatChnage += this.HandleSynchroMaxFromatChnage;
				// キャラ情報リストイベント登録
				this.Model.OnCharaInfoListChange += this.HandleCharaInfoListChange;
				this.Model.OnClearCharaInfoList += this.HandleClearCharaInfoList;

				// モデル同期
				this.SyncHaveMoney();
				this.SyncNeedMoney();
				this.SyncStatus();
				this.SyncStatusUp();

				if(this.CharaList != null)
				{
					// キャラページリストイベント登録
					this.CharaList.OnUpdateItemsEvent += this.HandleCharaListUpdateItemsEvent;
					this.CharaList.OnItemClickEvent += this.HandleCharaListItemClickEvent;
				}
				if(this.BaseChara != null)
				{
					// ベースキャライベント登録
					this.BaseChara.OnItemClickEvent += this.HandleBaseCharaItemClickEvent;
					this.BaseChara.OnItemChangeEvent += this.HandleBeseCharaItemChangeEvent;
				}
				if(this.BaitChara != null)
				{
					// 餌キャライベント登録
					this.BaitChara.OnItemChangeEvent += this.HandleBaitItemChangeEvent;
					this.BaitChara.OnItemClickEvent += this.HandleBaitItemClickEvent;
				}

				// リストクリア同期
				this.SyncClearCharaInfoList();
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
				if(this.CanUpdate)
				{
					this.View.SetActive(isActive, isTweenSkip);

					// その他UIの表示設定
					GUILobbyResident.SetActive(!isActive);
					GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX330_Synchro_Fusion_ScreenTitle));
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

				// 合成可能状態か取得
				bool isEnable = true;

				// 餌キャラが選択されていない
				if (this.BaitCharaUUID <= 0) isEnable = false;
				// ベースキャラのシンクロ合成残り回数が0
				else if (this.BaseCharaInfo == null || this.BaseCharaInfo.SynchroRemain <= 0) isEnable = false;
				// 合計シンクロボーナス最大値
				else if (CharaInfo.GetTotalSynchroBonus(this.BaseCharaInfo) >= CharaInfo.GetTotalMaxSynchroBonus()) isEnable = false;
				// 所持金が足りない
				else if (this.Model.HaveMoney < this.Model.NeedMoney) isEnable = false;

				this.View.SetFusionButtonEnable(isEnable);
			}

			/// <summary>
			/// ヘルプメッセージの状態を更新する
			/// </summary>
			private void UpdateHelpMessage()
			{
				if (!this.CanUpdate) { return; }

				var state = this.View.GetActiveState();
				bool isActive = false;
				if (state == GUIViewBase.ActiveState.Opened || state == GUIViewBase.ActiveState.Opening)
				{
					isActive = true;
				}

				var baseCharaUUID = this.BaseCharaUUID;
				// ベースキャラを選択中
				if (baseCharaUUID <= 0) { GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX331_Synchro_Fusion_Base_HelpMessage)); }
				// ベースキャラのシンクロ可能回数が0
				else if (this.BaseCharaInfo == null || this.BaseCharaInfo.SynchroRemain <= 0) { GUIHelpMessage.PlayWarning(isActive, MasterData.GetText(TextType.TX410_Synchro_Fusion_SynchroRemain_HelpMessage)); }
				// シンクロ合成可能状態
				else if (!this.IsEmptyBaitChara) { GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX409_Synchro_Fusion_Enter_HelpMessage)); }
				// 素材選択中状態
				else if (this.IsHaveMaterialChara()) { GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX332_Synchro_Fusion_Bait_HelpMessage)); }
				// 素材にできるキャラを所持していない状態
				else { GUIHelpMessage.PlayWarning(isActive, MasterData.GetText(TextType.TX408_Synchro_Fusion_NotHave_HelpMessage)); }
			}
			#endregion

			#region BOX総数
			/// <summary>
			/// BOXの総数セット
			/// </summary>
			public void SetupCapacity(int capacity, int count)
			{
				if (this.CharaList != null)
				{
					this.CharaList.SetupCapacity(capacity, count);
				}
			}
			#endregion

			#region キャラ情報リスト
			#region セット
			/// <summary>
			/// キャラ情報リスト設定
			/// </summary>
			public void SetupCharaInfoList(List<CharaInfo> list)
			{
				if (!this.CanUpdate || list == null) { return; }
				// キャラ情報セット
				this.Model.SetCharaInfoList(list);
			}

			/// <summary>
			/// キャラ情報リストに変更があった時に呼ばれる
			/// </summary>
			private void HandleCharaInfoListChange(object sender, EventArgs e)
			{
				this.SyncCharaInfoList();
			}
			/// <summary>
			/// キャラ情報リスト同期
			/// </summary>
			private void SyncCharaInfoList()
			{
				if (this.CharaList == null) { return; }

				// キャラ情報リスト取得
				List<CharaInfo> charaInfoList = this.Model.GetCharaInfoList();

				// ベースキャラのキャラ情報更新
				this.UpdateBaseCharaInfo(charaInfoList);
				
				if (this.IsEmptyBaseChara)
				{
					// ベースキャラ選択時

					// 各キャラ情報の選択出来るかどうかを設定する
					charaInfoList.ForEach(this.SetBaseCharaCanSelect);
				}
				else
				{
					// 素材ランク取得
					int materialRank = 0;
					CharaSynchroRestrictionMasterData masterData;
					if(TryGetRestrictionMasterData(this.BaseCharaInfo, out masterData))
					{
						materialRank = masterData.Rank;
					}

					// 餌キャラ選択時
					foreach(var info in charaInfoList)
					{
						// 各キャラ情報の選択出来るかどうかを設定する
						this.SetBaitCharaCanSelect(info, materialRank);
					}
				}

				// 餌キャラ情報更新
				this.UpdateBaitCharaInfo(charaInfoList);
				// ページリスト内のキャラ情報更新
				this.UpdateCharaListInfo(charaInfoList);
			}

			#region 無効設定
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
			/// 餌キャラ選択時の各キャラ情報の選択できるかの設定
			/// </summary>
			private void SetBaitCharaCanSelect(CharaInfo info, int materialRank)
			{
				if (info == null) return;

				// 無効タイプを取得する
				CharaItem.Controller.DisableType disableType;
				this.GetBaitCharaDisableType(info, materialRank, out disableType);

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
				// ベース/ページ/餌の各キャラ情報を削除する
				this.ClearCharaList();
				this.ClearBaseChara();
				this.ClearBaitChara();
			}
			#endregion

			#region 取得
			/// <summary>
			/// 所持キャラの中から素材可能な一覧を取得
			/// </summary>
			private List<CharaInfo> GetMaterialHaveList()
			{
				var materialList = new List<CharaInfo>();

				// 指定素材情報取得
				CharaInfo targetInfo = this.BaitChara.GetCharaInfo();
				if (targetInfo == null) { return materialList; }

				var uuidDic = new Dictionary<ulong, CharaInfo>();
				if (this.Model.TryGetCharaInfoByMasterId(targetInfo.CharacterMasterID, out uuidDic))
				{
					foreach (var info in uuidDic.Values)
					{
						CharaItem.Controller.DisableType disableType;
						this.GetBaitCharaDisableType(info, this.BaitChara.GetMaterialRank(), out disableType);
						if (disableType == CharaItem.Controller.DisableType.None ||
							disableType == CharaItem.Controller.DisableType.Bait)
						{
							materialList.Add(info);
						}
					}
				}

				return materialList;
			}

			/// <summary>
			/// 所持しているキャラの中に指定されている素材キャラが存在しているかどうか
			/// </summary>
			private bool IsHaveMaterialChara()
			{
				var haveList = this.GetMaterialHaveList();
				return (haveList.Count > 0) ? true : false;
			}
			#endregion
			#endregion

			#region キャラページリスト
			#region アイテム更新イベント
			/// <summary>
			/// キャラページリストの全アイテムが更新された時に呼ばれる
			/// </summary>
			private void HandleCharaListUpdateItemsEvent()
			{
				if (this.CharaList != null)
				{
					var itemList = this.CharaList.GetNowPageItemList();

					// アイテム無効設定
					if (this.IsEmptyBaseChara)
					{
						// ベースキャラ選択時
						itemList.ForEach(this.SetBeseCharaDisableState);
					}
					else
					{
						// 餌キャラ選択時
						itemList.ForEach(this.SetBaitCharaDisableState);
					}
				}
			}

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
			/// 餌キャラ選択時の各アイテムの無効設定
			/// </summary>
			private void SetBaitCharaDisableState(GUICharaItem item)
			{
				if (item == null) return;

				// 素材ランク取得
				int materialRank = 0;
				CharaSynchroRestrictionMasterData masterData;
				if (TryGetRestrictionMasterData(this.BaseCharaInfo, out masterData))
				{
					materialRank = masterData.Rank;
				}

				// 無効タイプを設定する
				CharaItem.Controller.DisableType disableType = CharaItem.Controller.DisableType.None;
				this.GetBaitCharaDisableType(item.GetCharaInfo(), materialRank, out disableType);
				item.SetDisableState(disableType);
			}
			#endregion
			#endregion

			#region キャラ情報更新
			/// <summary>
			/// キャラページリスト内のキャラ情報を更新
			/// </summary>
			private void UpdateCharaListInfo(List<CharaInfo> charaInfoList)
			{
				if(this.IsEmptyBaseChara)
				{
					// ベースキャラ選択時

					// ベースキャラ枠が空ならサーバから受け取ったキャラ情報をセット
					this.BaseCharaListSetup();
				}
				else
				{
					// 餌キャラ選択時

					// ベースキャラ枠がセットされているならシンクロ合成素材一覧のみのキャラリストをセットする
					this.BaitCharaListSetup();
				}
			}
			#endregion

			#region キャラページリスト内のアイテム処理
			/// <summary>
			/// キャラページリスト内のアイテムが押された時に呼び出される
			/// </summary>
			private void HandleCharaListItemClickEvent(GUICharaItem obj)
			{
				// アイテムセット
				this.SetCharaListItem(obj);

				if(obj.GetCharaInfo() != null)
				{
					// キャラ情報同期
					this.SyncCharaInfoList();
				}
			}

			/// <summary>
			/// アイテム設定
			/// </summary>
			private void SetCharaListItem(GUICharaItem item)
			{
				if (item == null) { return; }

				var disableState = item.GetDisableState();
				var info = item.GetCharaInfo();
				switch(disableState)
				{
					case CharaItem.Controller.DisableType.None:
						if(this.IsEmptyBaseChara)
						{
							// ベースキャラ設定
							this.SetBaseChara(info);
						}
						else
						{
							// 餌キャラを追加
							this.AddBaitChara(info);
						}
						break;
					case CharaItem.Controller.DisableType.Base:
						// ベースキャラクリア
						this.ClearBaseChara();
						break;
					case CharaItem.Controller.DisableType.Bait:
						// 餌キャラを外す
						this.RemoveBaitChara(info);
						break;
				}
			}
			#endregion

			#region キャラリストセットアップ
			/// <summary>
			/// ベース選択時のキャラリスト設定処理
			/// </summary>
			private void BaseCharaListSetup()
			{
				if (!this.CanUpdate || this.CharaList == null) { return; }
				this.CharaList.SetupItems(this.Model.GetCharaInfoList());
			}

			/// <summary>
			/// 素材選択時のキャラリスト設定処理
			/// </summary>
			private void BaitCharaListSetup()
			{
				if (!this.CanUpdate || this.CharaList == null) { return; }

				// ベースキャラと同じ種類のキャラのみの一覧を作成
				if (BaseCharaInfo == null) { return; }
				var charaInfoList = new List<CharaInfo>();
				foreach (var info in this.Model.GetCharaInfoList())
				{
					// ベースキャラと同じ種類のみ表示 それ以外のキャラは存在アイコンを表示させる
					if (info.AvatarType == BaseCharaInfo.AvatarType || this.BaseCharaUUID == info.UUID)
					{
						charaInfoList.Add(info);
					}
					else
					{
						charaInfoList.Add(null);
					}
				}
				// セット
				this.CharaList.SetupItems(charaInfoList);
			}
			#endregion

			#region キャラリストクリア
			/// <summary>
			/// キャラリストクリア処理
			/// </summary>
			private void ClearCharaList()
			{
				if (this.CharaList == null) { return; }
				//リスト初期化
				this.CharaList.SetupItems(null);
				// 1ページ目に戻す
				this.CharaList.BackEndPage();
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
				// ベースキャラのステータスセット
				this.SetBaseCharaStatus();
				// 合成ボタン状態更新
				this.UpdateFusionButtonEnable();
				// ヘルプメッセージ更新
				this.UpdateHelpMessage();
				if (this.View != null)
				{
					// 素材フィルター表示設定
					this.View.SetFillMaterialActive(this.IsEmptyBaseChara);
				}
			}

			/// <summary>
			/// 設定されているベースキャラからステータスをデータにセットする
			/// </summary>
			private void SetBaseCharaStatus()
			{
				if (this.Model == null) { return; }

				int synchroHitPoint = 0;
				int synchroAttack = 0;
				int synchroDefense = 0;
				int synchroExtra = 0;
				int remain = 0;

				var baseCharaInfo = this.BaseCharaInfo;
				if (baseCharaInfo != null)
				{
					synchroHitPoint = baseCharaInfo.SynchroHitPoint;
					synchroAttack = baseCharaInfo.SynchroAttack;
					synchroDefense = baseCharaInfo.SynchroDefense;
					synchroExtra = baseCharaInfo.SynchroExtra;
					remain = baseCharaInfo.SynchroRemain;
				}

				this.Model.SynchroHitPoint = synchroHitPoint;
				this.Model.SynchroAttack = synchroAttack;
				this.Model.SynchroDefense = synchroDefense;
				this.Model.SynchroExtra = synchroExtra;
				this.Model.SynchroRemain = remain;
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

			#region ベースキャラ設定
			/// <summary>
			/// ベースキャラを設定する
			/// </summary>
			private void SetBaseChara(CharaInfo info)
			{
				if (this.BaseChara != null)
				{
					if(this.BaseCharaInfo != info)
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

					// 餌キャラアイテムを空状態にする
					this.ClearBaitChara();

				}
				else
				{
					// シンクロ合成餌選択時

					// 餌キャラアイテムに素材をセットする
					this.SetMaterialBaitChara(info);
				}
			}

			/// <summary>
			/// ベースキャラを外す
			/// </summary>
			private void ClearBaseChara()
			{
				this.SetBaseChara(null);
			}
			#endregion

			#region ベースキャラアイテム処理
			/// <summary>
			/// ベースキャラアイテムが押された時に呼び出される
			/// </summary>
			private void HandleBaseCharaItemClickEvent(GUICharaItem obj)
			{
				// ベースキャラが設定されていたら外す
				this.ClearBaseChara();
				// キャラ情報リスト同期
				this.SyncCharaInfoList();
			}

			/// <summary>
			/// ベースキャラアイテムを設定する
			/// </summary>
			private void SetBaseCharaItem(GUICharaItem item, CharaInfo info)
			{
				if (item == null) { return; }

				var state = CharaItem.Controller.ItemStateType.Icon;
				if (info == null || info.IsEmpty)
				{
					state = CharaItem.Controller.ItemStateType.FillEmpty;
				}
				item.SetState(state, info);
			}
			#endregion

			#region ベースキャラステータス
			#region 生命力
			// 生命力(シンクロボーナス)が変更された時に呼び出される
			private void HandleSynchroHitPointChange(object sender, EventArgs e)
			{
				SyncHitPoint();
			}
			/// <summary>
			/// 生命力同期
			/// </summary>
			private void SyncHitPoint()
			{
				if (!CanUpdate) { return; }
				this.View.SetHitPoint(string.Format(this.Model.SynchroFormat, this.Model.SynchroHitPoint));
			}
			#endregion

			#region 攻撃力
			// 攻撃力(シンクロボーナス)が変更された時に呼び出される
			private void HandleSynchroAttackChange(object sender, EventArgs e)
			{
				SyncAttack();
			}
			/// <summary>
			/// 攻撃力同期
			/// </summary>
			private void SyncAttack()
			{
				if (!CanUpdate) { return; }
				this.View.SetAttack(string.Format(this.Model.SynchroFormat, this.Model.SynchroAttack));
			}
			#endregion

			#region 防御力
			// 防御力(シンクロボーナス)が変更された時に呼び出される
			private void HandleSynchroDefenseChange(object sender, EventArgs e)
			{
				SyncDefense();
			}
			/// <summary>
			/// 防御力同期
			/// </summary>
			private void SyncDefense()
			{
				if (!CanUpdate) { return; }
				this.View.SetDefense(string.Format(this.Model.SynchroFormat, this.Model.SynchroDefense));
			}
			#endregion

			#region 特殊能力
			// 特殊能力(シンクロボーナス)が変更された時に呼び出される
			private void HandleSynchroExtraChange(object sender, EventArgs e)
			{
				SyncExtra();
			}
			/// <summary>
			/// 特殊能力同期
			/// </summary>
			private void SyncExtra()
			{
				if (!CanUpdate) { return; }
				this.View.SetExtra(string.Format(this.Model.SynchroFormat, this.Model.SynchroExtra));
			}
			#endregion

			#region シンクロボーナス合計値
			// シンクロボーナス合計値が変更された時に呼び出される
			private void HandleTotalSynchroBonusChange(object sender, EventArgs e)
			{
				this.SyncTotalSynchroBonus();
			}
			/// <summary>
			/// シンクロボーナス合計値同期
			/// </summary>
			private void SyncTotalSynchroBonus()
			{
				if (!CanUpdate) { return; }
				this.View.SetTotalSynchroBonus(this.Model.TotalSynchroBonus.ToString());

				// 残り強化値
				int remain = 0;
				if(!this.IsEmptyBaseChara)
				{
					remain = Math.Min(CharaInfo.GetTotalMaxSynchroBonus() - this.Model.TotalSynchroBonus, 0);
				}
				this.View.SetTotalSynchroBonusRemain(remain.ToString());
			}

			/// <summary>
			/// 残り合計シンクロボーナス値が変更された時に呼び出される
			/// </summary>
			private void HandleTotalSynchroBonusRemainChnage(object sender, EventArgs e)
			{
				this.SyncTotalSynchroBonusRemain();
			}
			/// <summary>
			/// 残り合計シンクロボーナス値同期
			/// </summary>
			private void SyncTotalSynchroBonusRemain()
			{
				if (!CanUpdate) { return; }
				this.View.SetTotalSynchroBonusRemain(this.Model.TotalSynchroBonusRemain.ToString());
			}
			#endregion

			#region シンクロ合成残り回数
			// シンクロ合成残り回数が変更された時に呼び出される
			private void HandleSynchroRemainChange(object sender, EventArgs e)
			{
				SyncSynchroRemain();
			}
			/// <summary>
			/// シンクロ合成残り回数色が変更された時に呼び出される
			/// </summary>
			private void HandleSynchroRemainColorChange(object sender, EventArgs e)
			{
				this.SyncSynchroRemain();
			}
			/// <summary>
			/// シンクロ合成残り回数同期
			/// </summary>
			private void SyncSynchroRemain()
			{
				if (!CanUpdate) { return; }
				this.View.SetSynchroRemain(this.Model.SynchroRemain.ToString());
			}
			#endregion

			#region シンクロ値表示フォーマット
			// シンクロ値表示フォーマットが変更された時に呼び出される
			private void HandleSynchroFromatChnage(object sender, EventArgs e)
			{
				this.SyncStatus();
			}
			// シンクロ増加値表示フォーマットが変更された時に呼び出される
			private void HandleSynchroUpFromatChnage(object sender, EventArgs e)
			{
				this.SyncStatusUp();
			}
			// シンクロ増加値最大時表示フォーマットが変更された時に呼び出される
			private void HandleSynchroMaxFromatChnage(object sender, EventArgs e)
			{
				this.SyncStatusUp();
			}
			#endregion

			/// <summary>
			/// ベースキャラのステータスを同期させる
			/// </summary>
			private void SyncStatus()
			{
				SyncHitPoint();
				SyncAttack();
				SyncDefense();
				SyncExtra();
				SyncTotalSynchroBonus();
				SyncSynchroRemain();
			}

			#region 増加分
			/// <summary>
			/// 生命力増加分同期
			/// </summary>
			private void SyncHitPointUp()
			{
				if (!CanUpdate) { return; }
				string format = string.Empty;
				StatusColor.Type type = StatusColor.Type.StatusNormal;

				if (!this.IsEmptyBaitChara)
				{
					bool isUp = (this.Model.SynchroHitPoint < CharaInfo.SynchroMaxHitPoint) ? true : false;
					format = isUp ? this.Model.SynchroUpFormat : this.Model.SynchroMaxFormat;
					type = isUp ? StatusColor.Type.Random : StatusColor.Type.StatusNormal;
				}
				this.View.SetHitPointUp(format);
				this.View.SetHitPointColor(type);
			}
			/// <summary>
			/// 攻撃力増加分同期
			/// </summary>
			private void SyncAttackUp()
			{
				if (!CanUpdate) { return; }
				string format = string.Empty;
				StatusColor.Type type = StatusColor.Type.StatusNormal;

				if (!this.IsEmptyBaitChara)
				{
					bool isUp = (this.Model.SynchroAttack < CharaInfo.SynchroMaxAttack) ? true : false;
					format = isUp ? this.Model.SynchroUpFormat : this.Model.SynchroMaxFormat;
					type = isUp ? StatusColor.Type.Random : StatusColor.Type.StatusNormal;
				}
				this.View.SetAttackUp(format);
				this.View.SetAttackColor(type);
			}
			/// <summary>
			/// 防御力増加分同期
			/// </summary>
			private void SyncDefenseUp()
			{
				if (!CanUpdate) { return; }
				string format = string.Empty;
				StatusColor.Type type = StatusColor.Type.StatusNormal;

				if (!this.IsEmptyBaitChara)
				{
					bool isUp = (this.Model.SynchroDefense < CharaInfo.SynchroMaxDefense) ? true : false;
					format = isUp ? this.Model.SynchroUpFormat : this.Model.SynchroMaxFormat;
					type = isUp ? StatusColor.Type.Random : StatusColor.Type.StatusNormal;
				}
				this.View.SetDefenseUp(format);
				this.View.SetDefenseColor(type);
			}
			/// <summary>
			/// 特殊能力増加分同期
			/// </summary>
			private void SyncExtraUp()
			{
				if (!CanUpdate) { return; }
				string format = string.Empty;
				StatusColor.Type type = StatusColor.Type.StatusNormal;

				if (!this.IsEmptyBaitChara)
				{
					bool isUp = (this.Model.SynchroExtra < CharaInfo.SynchroMaxExtra) ? true : false;
					format = isUp ? this.Model.SynchroUpFormat : this.Model.SynchroMaxFormat;
					type = isUp ? StatusColor.Type.Random : StatusColor.Type.StatusNormal;
				}
				this.View.SetExtraUp(format);
				this.View.SetExtraColor(type);
			}
			/// <summary>
			/// シンクロ残り回数減少分同期
			/// </summary>
			private void SyncSynchroRemainDown()
			{
				if (!CanUpdate) { return; }
				string format = string.Empty;
				StatusColor.Type type = StatusColor.Type.StatusNormal;

				if (!this.IsEmptyBaitChara)
				{
					int afterRemain = Math.Max(this.Model.SynchroRemain - 1, 0);
					format = afterRemain.ToString();

					if (afterRemain > this.Model.SynchroRemain)
					{
						type = StatusColor.Type.Up;
					}
					else if (afterRemain < this.Model.SynchroRemain)
					{
						type = StatusColor.Type.Down;
					}
					else
					{
						type = StatusColor.Type.StatusNormal;
					}
				}
				this.View.SetSynchroRemainDown(format);
				this.View.SetSynchroRemainDownColor(type);
			}
			/// <summary>
			/// ベースキャラのステータス増加分の同期
			/// </summary>
			private void SyncStatusUp()
			{
				this.SyncHitPointUp();
				this.SyncAttackUp();
				this.SyncDefenseUp();
				this.SyncDefenseUp();
				this.SyncExtraUp();
				this.SyncSynchroRemainDown();
			}
			#endregion
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

			#region 餌キャラ
			#region 餌キャラ情報同期
			/// <summary>
			/// 餌キャラ情報が変更された時に呼び出される
			/// </summary>
			private void HandleBaitItemChangeEvent(GUICharaItem item)
			{
				this.SyncBaitItem();
			}

			/// <summary>
			/// 餌キャラ情報同期
			/// </summary>
			private void SyncBaitItem()
			{
				if (this.BaitChara == null) { return; }

				// 素材ランク同期
				this.SyncMaterialRankBaitItem();
				// 所持設定同期
				this.SyncPossessionBaitCharaItem();
				// 選択更新
				this.UpdateSelectBait();
				// ベースキャラのステータス増加分同期
				this.SyncStatusUp();

				// 合成試算
				this.FusionCalc();
				// ヘルプメッセージ更新
				this.UpdateHelpMessage();
			}

			/// <summary>
			/// 餌キャラアイテムの素材ランクを同期する
			/// </summary>
			private void SyncMaterialRankBaitItem()
			{
				CharaInfo baseCharaInfo = this.BaseCharaInfo;
				int materialRank = 0;
				if (baseCharaInfo != null && this.BaitChara.GetCharaInfo() != null)
				{
					// シンクロ制限マスタ取得
					CharaSynchroRestrictionMasterData masterData;
					if (TryGetRestrictionMasterData(baseCharaInfo, out masterData))
					{
						// 素材ランク取得
						materialRank = masterData.Rank;
					}

					// ランク色セット
					this.BaitChara.SetRankColor(materialRank);
				}

				// 素材ランクセット
				this.BaitChara.SetMaterialRank(materialRank);
			}

			/// <summary>
			/// 餌キャラアイテムの所持設定の同期
			/// </summary>
			private void SyncPossessionBaitCharaItem()
			{
				// 所持状態を取得してアイテムにセットする
				CharaItem.Controller.PossessionStateType state = this.GetItemPossessionState(this.BaitChara);
				this.BaitChara.SetPossessionState(state);
			}

			/// <summary>
			///  所持状態を取得
			/// </summary>
			private CharaItem.Controller.PossessionStateType GetItemPossessionState(GUICharaItem item)
			{
				var state = CharaItem.Controller.PossessionStateType.None;

				CharaInfo materialInfo = item.GetCharaInfo();
				if (materialInfo == null || materialInfo.UUID > 0)
				{
					// 設定されている素材
					return CharaItem.Controller.PossessionStateType.None;
				}

				List<CharaInfo> haveList = this.GetMaterialHaveList();
				if (haveList.Count <= 0)
				{
					// 素材にすることが出来ない
					return CharaItem.Controller.PossessionStateType.NotPossession;
				}

				return state;
			}
			#endregion

			#region 餌キャラのキャラ情報更新
			/// <summary>
			/// 餌キャラのキャラ情報を更新する
			/// </summary>
			private void UpdateBaitCharaInfo(List<CharaInfo> charaInfoList)
			{
				if (charaInfoList == null || this.BaitChara == null) { return; }

				// キャラ情報がNULLの場合は更新しない
				var info = this.BaitChara.GetCharaInfo();
				if (info == null) { return; }
				
				// 素材アイコン状態の場合は同期のみ行う
				if(info.UUID == 0)
				{
					this.SyncBaitItem();
					return;
				}

				// 餌キャラのキャラ情報を更新する
				var newInfo = charaInfoList.Find(t => { return t.UUID == info.UUID; });
				CharaItem.Controller.ItemStateType stateType;
				if(newInfo != null)
				{
					stateType = this.BaitChara.GetState();
				}
				else
				{
					stateType = CharaItem.Controller.ItemStateType.FillEmpty;
				}
				this.BaitChara.SetState(stateType, newInfo.Clone());
			}
			#endregion

			#region 餌キャライベント
			/// <summary>
			/// 餌キャラアイテムが押された時に呼び出される
			/// </summary>
			private void HandleBaitItemClickEvent(GUICharaItem item)
			{
				if (this.BaitChara == null) { return; }
				if(this.BaitChara.GetCharaInfo() != null)
				{
					if(this.BaitChara.GetCharaInfo().UUID > 0)
					{
						// 餌がセットされている状態なら餌を外す
						this.RemoveBaitChara(this.BaitChara.GetCharaInfo());
					}
				}

				// キャラ情報リスト同期
				this.SyncCharaInfoList();
			}
			#endregion

			#region クリア
			/// <summary>
			/// 餌キャラアイテムを初期状態にする
			/// </summary>
			private void ClearBaitChara()
			{
				if (this.BaitChara == null) { return; }
				this.BaitChara.SetState(CharaItem.Controller.ItemStateType.FillEmpty, null);

			}
			#endregion

			#region 餌追加/削除
			/// <summary>
			/// 餌キャラを追加する
			/// </summary>
			private void AddBaitChara(CharaInfo charaInfo)
			{
				if (charaInfo == null || this.BaitChara == null) { return; }

				CharaInfo info = this.BaitChara.GetCharaInfo();
				if (info == null) { return; }

				if (charaInfo.AvatarType == info.AvatarType &&
					charaInfo.Rank >= this.BaitChara.GetMaterialRank())
				{
					// 同種類のキャラかる素材ランク以上のみセットする
					this.BaitChara.SetState(CharaItem.Controller.ItemStateType.Icon, charaInfo.Clone());
				}
			}

			/// <summary>
			/// 餌キャラを外す
			/// </summary>
			private void RemoveBaitChara(CharaInfo charaInfo)
			{
				if (charaInfo == null || this.BaitChara == null) { return; }

				CharaInfo info = this.BaitChara.GetCharaInfo();
				if (info == null) { return; }

				if(info.UUID > 0)
				{
					// 餌がセットされている
					if(charaInfo.AvatarType == info.AvatarType)
					{
						// 同種類なら餌を外し素材アイコン状態にセットする
						this.SetMaterialBaitChara(charaInfo);
					}
				}
			}
			#endregion

			#region 素材セット
			/// <summary>
			/// 餌キャラアイテムにシンクロ合成素材をセットする
			/// </summary>
			private void SetMaterialBaitChara(CharaInfo charaInfo)
			{
				if (charaInfo == null || this.BaitChara == null) { return; }

				// 同種類のキャラ素材をセットする
				var materialInfo = new CharaInfo(charaInfo.AvatarType);
				this.BaitChara.SetState(CharaItem.Controller.ItemStateType.Mono, materialInfo);
			}
			#endregion

			#region 素材選択
			/// <summary>
			/// 素材アイテムの選択更新
			/// </summary>
			private void UpdateSelectBait()
			{
				bool isSelect = false;
				if(!this.IsEmptyBaseChara)
				{
					// 所持しているかチェック
					if(this.BaitChara.GetPossessionState() == CharaItem.Controller.PossessionStateType.None)
					{
						isSelect = true;
					}
				}

				this.BaitChara.SetSelect(isSelect);
			}
			#endregion
			#endregion

			#region 合成試算
			/// <summary>
			/// 合成試算処理
			/// </summary>
			private void FusionCalc()
			{
				if (this.BaitCharaUUID > 0)
				{
					// 餌が設定されていれば試算パケット送信処理を行う
					var eventArgs = new FusionCalcEventArgs();
					eventArgs.BaseCharaUUID = this.BaseCharaUUID;
					eventArgs.BaitCharaUUID = this.BaitCharaUUID;

					// 通知
					this.OnFusionCalc(this, eventArgs);
				}
				else
				{
					// 餌設定されていない
					if (this.Model != null)
					{
						this.Model.NeedMoney = 0;
						this.Model.AddOnCharge = 0;
					}

					// 合成ボタン更新
					this.UpdateFusionButtonEnable();
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
				}

				if (this.CanUpdate)
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
			/// 合成メッセージに表示するためのデータを取得する
			/// </summary>
			private FusionMessage.SetupParam GetFusionMessageParam()
			{
				if (!this.CanUpdate) { return null; }

				// ベースキャラ情報取得
				var baseCharaInfo = this.BaseCharaInfo;
				if (baseCharaInfo == null) { return null; }

				// データセット
				FusionMessage.SetupParam param = new FusionMessage.SetupParam();

				// ランク
				param.RankBefore = baseCharaInfo.Rank;
				param.RankAfter = baseCharaInfo.Rank;
				// レベルと経験値
				param.LevelBefore = baseCharaInfo.PowerupLevel;
				param.LevelAfter = baseCharaInfo.PowerupLevel;
				param.Exp = baseCharaInfo.PowerupExp;
				param.TotalExp = (CharaInfo.IsMaxLevel(baseCharaInfo.Rank, baseCharaInfo.PowerupLevel) == true) ? 0 : CharaInfo.GetTotalExp(baseCharaInfo.Rank, baseCharaInfo.PowerupLevel);
				param.NextLvTotalExp = (CharaInfo.IsMaxLevel(baseCharaInfo.Rank, baseCharaInfo.PowerupLevel) == true) ? 0 : CharaInfo.GetTotalExp(baseCharaInfo.Rank, baseCharaInfo.PowerupLevel + 1);
				// シンクロ可能回数
				param.SynchroRemainBefore = baseCharaInfo.SynchroRemain;
				param.SynchroRemainAfter = Math.Max(this.Model.SynchroRemain - 1, 0);
				// 生命力
				param.HitPointBefore = baseCharaInfo.HitPoint;
				param.HitPointAfter = baseCharaInfo.HitPoint;
				param.HitPointBaseBefore = baseCharaInfo.PowerupHitPoint;
				param.HitPointBaseAfter = baseCharaInfo.PowerupHitPoint;
				param.SynchroHitPoint = baseCharaInfo.SynchroHitPoint;
				param.SlotHitPointBefore = baseCharaInfo.SlotHitPoint;
				param.SlotHitPointAfter = baseCharaInfo.SlotHitPoint;
				// 攻撃力
				param.AttackBefore = baseCharaInfo.Attack;
				param.AttackAfter = baseCharaInfo.Attack;
				param.AttackBaseBefore = baseCharaInfo.PowerupAttack;
				param.AttackBaseAfter = baseCharaInfo.PowerupAttack;
				param.SynchroAttack = baseCharaInfo.SynchroAttack;
				param.SlotAttackBefore = baseCharaInfo.SlotAttack;
				param.SlotAttackAfter = baseCharaInfo.SlotAttack;
				// 防御力
				param.DefenseBefore = baseCharaInfo.Defense;
				param.DefenseAfter = baseCharaInfo.Defense;
				param.DefenseBaseBefore = baseCharaInfo.PowerupDefense;
				param.DefenseBaseAfter = baseCharaInfo.PowerupDefense;
				param.SynchroDefense = baseCharaInfo.SynchroDefense;
				param.SlotDefenseBefore = baseCharaInfo.SlotDefense;
				param.SlotDefenseAfter = baseCharaInfo.SlotDefense;
				// 特殊能力
				param.ExtraBefore = baseCharaInfo.Extra;
				param.ExtraAfter = baseCharaInfo.Extra;
				param.ExtraBaseBefore = baseCharaInfo.PowerupExtra;
				param.ExtraBaseAfter = baseCharaInfo.PowerupExtra;
				param.SynchroExtra = baseCharaInfo.SynchroExtra;
				param.SlotExtraBefore = baseCharaInfo.SlotExtra;
				param.SlotExtraAfter = baseCharaInfo.SlotExtra;
				// シンクロ合成フラグ
				param.IsSynchroFusion = true;

				return param;
			}

			/// <summary>
			/// 合成処理
			/// </summary>
			private void Fusion()
			{
				if (this.BaseChara == null || this.BaitChara == null) { return; }

				// 餌がセットされているかチェック
				if (this.BaitCharaUUID <= 0) { return; }

				// ロックチェック処理へ
				this.CheckFusion();
			}

			/// <summary>
			/// シンクロ合成可能かチェック
			/// </summary>
			private void CheckFusion()
			{
				GUIFusionMessage.OpenYesNo(this.GetFusionMessageParam(), MasterData.GetText(TextType.TX333_Synchro_Fusion_Message), true, this.CheckLock, null);
			}

			/// <summary>
			/// ロックチェック処理
			/// </summary>
			private void CheckLock()
			{
				// ロックチェック
				bool isLock = false;
				if (this.BaitChara.GetCharaInfo() != null)
				{
					if (this.BaitChara.GetCharaInfo().IsLock)
					{
						isLock = true;
					}
				}

				// ロック分岐
				if (isLock)
				{
					GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX308_Powerup_Fusion_LockMessage), true, null);
				}
				else
				{
					// スロットチェック処理へ
					this.CheckSlot();
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
				if (this.BaitChara.GetCharaInfo() == null) { return; }
				
				// マスターデータ取得
				CharaSynchroRestrictionMasterData masterData;
				if (!this.TryGetRestrictionMasterData(this.BaseCharaInfo, out masterData)) { return; }
				
				// 高ランクチェック
				if (this.BaitChara.GetCharaInfo().Rank > masterData.Rank)
				{
					isInHighRank = true;
				}

				// 高ランク分岐
				if (isInHighRank)
				{
					GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX442_Synchro_Fusion_HighRankMessage), true, this.FusionExecute, null);
				}
				else
				{
					// 進化合成実行
					this.FusionExecute();
				}
			}

			/// <summary>
			/// 合成実行
			/// </summary>
			private void FusionExecute()
			{
				// 通知
				var eventArgs = new FusionEventArgs();
				eventArgs.BaseCharaUUID = this.BaseCharaUUID;
				eventArgs.BaitCharaUUID = this.BaitCharaUUID;
				
				this.OnFusion(this, eventArgs);
			}

			/// <summary>
			/// 合成結果
			/// </summary>
			public void FusionResult(Scm.Common.GameParameter.PowerupResult result, int money, int price, int addOnCharge, CharaInfo charaInfo)
			{
				if (result != Scm.Common.GameParameter.PowerupResult.Fail)
				{
					// 合成リザルト画面表示
					OpenResult(result, charaInfo);

					int total = (charaInfo != null) ? charaInfo.SynchroHitPoint + charaInfo.SynchroAttack + charaInfo.SynchroDefense + charaInfo.SynchroExtra : 0;
					if (total >= CharaInfo.GetTotalMaxSynchroBonus())
					{
						// 最大まで強化し終わったのでベースキャラを外す
						this.ClearBaseChara();
						// 餌キャラアイテムを空状態にする
						this.ClearBaitChara();
					}
					else
					{
						// 餌キャラを外す
						this.RemoveBaitChara(this.BaitChara.GetCharaInfo());
					}
				}
			}

			/// <summary>
			/// リザルト画面表示処理
			/// </summary>
			private void OpenResult(Scm.Common.GameParameter.PowerupResult result, CharaInfo charaInfoAfter)
			{
				// ベースキャラ情報取得
				CharaInfo charaInfoBefore = this.BaseCharaInfo;
				if (charaInfoBefore == null || charaInfoAfter == null) { return; }

				// リザルトデータ生成
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
				p.TotalExp = CharaInfo.GetTotalExp(charaInfoBefore.Rank, charaInfoBefore.PowerupLevel);
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
				// 合成結果
				p.IsPowerupResultEnable = true;
				p.PowerupResult = result;

				// 演出
				var param = new SynchroDirection.SynchroDirectionParam();
				param.BaseCharaId = charaInfoBefore.CharacterMasterID;
				// 同じだけど一応
				param.BaitCharaId = BaitChara.GetCharaInfo().CharacterMasterID;
				param.HpUp = charaInfoAfter.SynchroHitPoint - charaInfoBefore.SynchroHitPoint;
				param.IsHpMax = (charaInfoAfter.SynchroHitPoint == CharaInfo.SynchroMaxHitPoint);
				param.AtkUp = charaInfoAfter.SynchroAttack - charaInfoBefore.SynchroAttack;
				param.IsAtkMax = (charaInfoAfter.SynchroAttack== CharaInfo.SynchroMaxAttack);
				param.DefUp = charaInfoAfter.SynchroDefense - charaInfoBefore.SynchroDefense;
				param.IsDefMax = (charaInfoAfter.SynchroDefense == CharaInfo.SynchroMaxDefense);
				param.ExUp = charaInfoAfter.SynchroExtra - charaInfoBefore.SynchroExtra;
				param.IsExMax = (charaInfoAfter.SynchroExtra == CharaInfo.SynchroMaxExtra);

				param.Result = result;

				// 一度非表示
				this.SetActive(false, true);

				GUISynchroDirection.Open(param, () =>
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

				// ベースのキャラ情報を更新
				this.UpdateBaseCharaInfo(info);

				// 合成処理
				this.Fusion();
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

			#region 共通で使用する処理
			#region 無効状態取得
			/// <summary>
			/// ベースキャラ選択時の無効タイプを取得する
			/// </summary>
			private void GetBaseCharaDisableType(CharaInfo info, out CharaItem.Controller.DisableType disableType)
			{
				disableType = XUI.CharaItem.Controller.DisableType.None;
				if (info == null) return;

				// 以下無効にするかチェック
				// 最大レベルまで強化されている
				if (CharaInfo.GetTotalSynchroBonus(info) >= CharaInfo.GetTotalMaxSynchroBonus()) disableType = CharaItem.Controller.DisableType.PowerupLevelMax;
				// スロットに入っている
				else if (info.IsInSlot) disableType = XUI.CharaItem.Controller.DisableType.PowerupSlot;
			}

			/// <summary>
			/// 餌キャラ選択時の無効タイプを取得する
			/// </summary>
			private void GetBaitCharaDisableType(CharaInfo info, int materialRank, out CharaItem.Controller.DisableType disableType)
			{
				disableType = XUI.CharaItem.Controller.DisableType.None;
				if (info == null) return;

				// 以下無効にするかチェック
				// 優先順位があるので注意
				if (info.UUID == this.BaseCharaUUID)
				{
					// ベースキャラ選択中
					disableType = CharaItem.Controller.DisableType.Base;
				}
				else if (info.UUID == this.BaitCharaUUID)
				{
					// 餌キャラ設定中
					disableType = CharaItem.Controller.DisableType.Bait;
					return;
				}
				// ランク不足
				else if (info.Rank < materialRank) disableType = CharaItem.Controller.DisableType.RankShortage;
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

			#region シンクロ制限マスタ取得
			/// <summary>
			/// シンクロ制限マスタデータ取得
			/// </summary>
			private bool TryGetRestrictionMasterData(CharaInfo info, out CharaSynchroRestrictionMasterData masterData)
			{
				masterData = null;
				if (info == null) { return false; }

				// ボーナス合計値取得
				int total = CharaInfo.GetTotalSynchroBonus(info);
				return MasterData.TryGetSynchroRestrictionByTotalBonus(total, out masterData);
			}
			#endregion
			#endregion
		}
	}
}