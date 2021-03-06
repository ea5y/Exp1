/// <summary>
/// キャラBOX制御
/// 
/// 2016/04/29
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;
using Scm.Common.XwMaster;

namespace XUI.CharacterBox
{
	#region イベント引数
	/// <summary>
	/// まとめて売却試算イベント引数
	/// </summary>
	public class SellMultiCalcEventArgs : EventArgs
	{
		public List<ulong> SellCharaUUIDList { get; set; }
	}

	/// <summary>
	/// まとめて売却イベント引数
	/// </summary>
	public class SellMultiEventArgs : EventArgs
	{
		public List<ulong> SellCharaUUIDList { get; set; }
	}

	/// <summary>
	/// ロックイベント引数
	/// </summary>
	public class CharaLockEventArgs : EventArgs
	{
		public ulong UUID { get; set; }
		public bool IsLock { get; set; }
	}
	#endregion

	/// <summary>
	/// キャラBOX制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region 更新チェック
		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate { get; }
		#endregion

		#region 初期化
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// データ初期化
		/// </summary>
		void Setup();

		/// <summary>
		/// 再初期化
		/// </summary>
		void ReSetup();

		/// <summary>
		/// ステータス情報設定
		/// </summary>
		void SetupStatusInfo(int haveMoney);
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

		#region BOX総数
		/// <summary>
		/// BOXの総数設定
		/// </summary>
		void SetupCapacity(int capacity, int itemCount);
		#endregion

		#region キャラ情報リスト
		/// <summary>
		/// キャラ情報リスト設定
		/// </summary>
		void SetupCharaInfoList(List<CharaInfo> charaInfoList);
		#endregion

		#region キャラロック
		/// <summary>
		/// キャラロックイベント
		/// </summary>
		event EventHandler<CharaLockEventArgs> OnCharaLock;
		#endregion

		#region まとめて売却
		/// <summary>
		/// まとめて売却試算イベント
		/// </summary>
		event EventHandler<SellMultiCalcEventArgs> OnSellMultiCalc;

		/// <summary>
		/// 売却試算結果
		/// </summary>
		void MultiSellCalcResult(bool result, List<ulong> sellUUIDList, int money, int soldPrice, int addOnCharge);

		/// <summary>
		/// まとめて売却イベント
		/// </summary>
		event EventHandler<SellMultiEventArgs> OnSellMulti;

		/// <summary>
		/// まとめて売却結果
		/// </summary>
		void MultiSellResult(bool result, List<ulong> sellUUIDList, int money, int soldPrice, int addOnCharge);
		#endregion
	}

	/// <summary>
	/// キャラBOX制御
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
				if (info != null) uuid = info.UUID;
				return uuid;
			}
		}

		// ベースキャラのキャラ情報
		private CharaInfo BaseCharaInfo { get { return (this.BaseChara != null ? this.BaseChara.GetCharaInfo() : null); } }

		// ベースキャラが空かどうか
		private bool IsEmptyBaseChara { get { return (this.BaseCharaInfo == null ? true : false); } }

		/// <summary>
		/// 売却キャラリスト
		/// </summary>
		private readonly GUISelectCharaList _sellCharaList;
		private GUISelectCharaList SellCharaList { get { return _sellCharaList; } }

		/// <summary>
		/// Boxのモード
		/// </summary>
		private ModeType Mode { get; set; }
		private enum ModeType : byte
		{
			CharaInfo,		// キャラ詳細モード
			MultiSell,		// まとめて売却モード
		}

		/// <summary>
		/// BOXモード切替リスト
		/// </summary>
		private Dictionary<ModeType, Action<bool>> changeModeExecuteDic = new Dictionary<ModeType, Action<bool>>();

		/// <summary>
		/// キャラロックイベント
		/// </summary>
		public event EventHandler<CharaLockEventArgs> OnCharaLock = (sender, e) => { };

		/// <summary>
		/// まとめて売却試算イベント
		/// </summary>
		public event EventHandler<SellMultiCalcEventArgs> OnSellMultiCalc = (sender, e) => { };

		/// <summary>
		/// まとめて売却イベント
		/// </summary>
		public event EventHandler<SellMultiEventArgs> OnSellMulti = (sender, e) => { };
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view, GUICharaPageList charaList, GUISelectCharaList sellCharaList, GUICharaItem baseChara)
		{
			if (model == null || view == null) { return; }

			// ページリストとベースキャラと売却キャラリストをセット
			this._baseChara = baseChara;
			this._sellCharaList = sellCharaList;
			this._charaList = charaList;

			// ビュー設定
			this._view = view;
			// 各ボタンイベント登録
			this.View.OnHome += this.HandleHome;
			this.View.OnClose += this.HandleClose;
			this.View.OnCharaInfoMode += this.HandleCharaInfoMode;
			this.View.OnSellMultiMode += this.HandleSellMultiMode;
			this.View.OnExecute += this.HandleExecute;
			this.View.OnLock += this.HandleLock;

			// モデル設定
			this._model = model;
			// 所持金イベント登録
			this.Model.OnHaveMoneyChange += this.HandleHaveMoneyChange;
			this.Model.OnHaveMoneyFormatChange += this.HandleHaveMoneyFormatChange;
			// 総売却額イベント登録
			this.Model.OnTotalSoldPriceChange += this.HandleTotalSoldPriceChange;
			this.Model.OnTotalSoldPriceFormatChange += this.HandleTotalSoldPriceFormatChange;
			// キャラ情報リストイベント登録
			this.Model.OnCharaInfoListChange += this.HandleCharaInfoListChange;
			this.Model.OnClearCharaInfoList += this.HandleClearCharaInfoList;
			// ベースキャラステータスイベント登録
			this.Model.OnCharaNameChange += this.HandleCharaNameChange;
			this.Model.OnLockChange += this.HandleIsLockChange;
			this.Model.OnSoldPriceChange += this.HandleSoldPriceChange;
			this.Model.OnRebuildTimeChange += this.HandleRebuildTimeChange;
			this.Model.OnCostChange += this.HandleCostChange;
			this.Model.OnExpChange += this.HandleExpChange;
			this.Model.OnExpFormatChange += this.HandleExpFormatChange;
			this.Model.OnSynchroRemainChange += this.HandleSynchroRemainChange;
			this.Model.OnHitPointChange += this.HandleHitPointChange;
			this.Model.OnHitPointFormatChange += this.HandleHitPointFormatChange;
			this.Model.OnAttackChange += this.HandleAttackChange;
			this.Model.OnAttackFormatChange += this.HandleAttackFormatChange;
			this.Model.OnDefenceChange += this.HandleDefenceChange;
			this.Model.OnDefenceFormatChange += this.HandleDefenceFormatChange;
			this.Model.OnExtraChange += this.HandleExtraChange;
			this.Model.OnExtraFormatChange += this.HandleExtraFormatChange;

			// キャラページリスト設定
			if(this.CharaList != null)
			{
				// イベント登録
				this.CharaList.OnItemClickEvent += this.HandleCharaListItemClick;
				this.CharaList.OnUpdateItemsEvent += this.HandleCharaListUpdateItems;
			}
			// ベースキャラ設定
			if(this.BaseChara != null)
			{
				// イベント登録
				this.BaseChara.OnItemChangeEvent += this.HandleBaseCharaItemChange;
				this.BaseChara.OnItemClickEvent += this.HandleBaseCharaItemClick;
				this.BaseChara.SetSelect(true);
			}
			// 売却リスト設定
			if(this.SellCharaList != null)
			{
				// イベント登録
				this.SellCharaList.OnItemClickEvent += this.HandleSellListItemClick;
				this.SellCharaList.OnUpdateItemsEvent += this.HandleSellListUpdateItems;
			}

			// モード切替実行処理をリストに登録
			SetChangeModeExecuteList();

			// 同期
			this.SyncBaseCharaStatus();
			this.SyncHaveMoney();
			this.SyncTotalSoldPrice();
			this.UpdateExecuteButtonEnable();
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

			this.OnSellMultiCalc = null;
			this.OnSellMulti = null;
			this.OnCharaLock = null;
		}

		/// <summary>
		/// モード切替実行処理をリストに登録する
		/// </summary>
		private void SetChangeModeExecuteList()
		{
			this.changeModeExecuteDic.Clear();
			this.changeModeExecuteDic.Add(ModeType.CharaInfo, this.CharaInfoMode);
			this.changeModeExecuteDic.Add(ModeType.MultiSell, this.SellMultiMode);
		}

		/// <summary>
		/// データ初期化
		/// </summary>
		public void Setup()
		{
			// キャラ情報リスト初期化
			this.ClearCharaInfoList();
			// キャラ情報モードに設定
			this.SetMode(ModeType.CharaInfo);
		}

		/// <summary>
		/// 再初期化
		/// </summary>
		public void ReSetup(){}

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
				// タイトル設定
				GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX412_CharaBox_ScreenTitle));
				// 各メッセージの状態を設定
				this.UpdateMessage();
			}
		}
		#endregion

		#region 各状態を更新する
		/// <summary>
		/// メッセージの状態を更新する
		/// </summary>
		private void UpdateMessage()
		{
			if (!this.CanUpdate) { return; }

			var state = this.View.GetActiveState();
			bool isActive = false;
			if (state == GUIViewBase.ActiveState.Opened || state == GUIViewBase.ActiveState.Opening)
			{
				isActive = true;
			}

			string help = string.Empty;
			switch (this.Mode)
			{
				case ModeType.CharaInfo:
					help = MasterData.GetText(TextType.TX413_CharaBox_Select_HelpMessage);
					break;
				case ModeType.MultiSell:
					help = MasterData.GetText(TextType.TX414_CharaBox_MultiSell_HelpMessage);
					break;
			}

			// メッセージセット
			GUIHelpMessage.Play(isActive, help);
		}
		#endregion

		#region BOX総数
		/// <summary>
		/// BOXの総数設定
		/// </summary>
		public void SetupCapacity(int capacity, int itemCount)
		{
			if (this.CharaList != null)
			{
				this.CharaList.SetupCapacity(capacity, itemCount);
			}
		}
		#endregion

		#region キャラ情報リスト
		#region セット
		/// <summary>
		/// キャラ情報リスト設定
		/// </summary>
		public void SetupCharaInfoList(List<CharaInfo> charaInfoList)
		{
			if (!this.CanUpdate) { return; }

			// 選択設定更新
			this.UpdateCanSelect(charaInfoList);

			// 売却アイテムリスト情報更新
			this.UpdateSellCharaInfoList(charaInfoList);
			// ページ内キャラリスト情報更新
			this.SetCharaList(charaInfoList);
			// キャラリスト設定
			this.Model.SetCharaInfoList(charaInfoList);
		}

		/// <summary>
		/// キャラ情報リスト変更ハンドラー
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
			if (!this.CanUpdate) { return; }

			// ベースキャラ情報更新
			this.UpdateBaseCharaItemInfo();
			// キャラページアイテム更新
			UpdateCharaItemList();
		}
		#endregion

		#region クリア
		/// <summary>
		/// キャラ情報リストクリア
		/// </summary>
		private void ClearCharaInfoList()
		{
			if (!this.CanUpdate) { return; }
			this.Model.ClearCharaInfoList();
		}

		/// <summary>
		/// キャラ情報リストクリアハンドラー
		/// </summary>
		private void HandleClearCharaInfoList(object sender, EventArgs e)
		{
			this.SyncClearCharaInfoList();
		}
		/// <summary>
		/// キャラ情報リストクリア同期
		/// </summary>
		private void SyncClearCharaInfoList()
		{
			// ベース/ページ/売却キャラリストを削除する
			this.ClearSellCharaList();
			this.ClearBaseChara();
			this.ClearCharaList();
		}
		#endregion

		#region 選択設定
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
			var sellIndex = -1;
			if(this.Mode == ModeType.MultiSell)
			{
				this.GetMultiSellItemDisableType(info, this.GetSellItemUUIDList(), out disableType, out sellIndex);
			}

			// 無効タイプから選択できるか設定する
			bool canSelect = false;
			if(disableType == CharaItem.Controller.DisableType.None ||
				disableType == CharaItem.Controller.DisableType.Base ||
				disableType == CharaItem.Controller.DisableType.Bait)
			{
				canSelect = true;
			}
			info.CanSelect = canSelect;
		}
		#endregion

		#region キャラ情報更新
		/// <summary>
		/// キャラ情報リスト更新
		/// </summary>
		private void UpdateCharaInfoList()
		{
			// キャラ情報リスト取得
			List<CharaInfo> charaInfoList = this.Model.GetCharaInfoList();

			// 選択設定更新
			this.UpdateCanSelect(charaInfoList);
			// ベースキャラ情報更新
			this.UpdateBaseCharaItemInfo();
			// ページ内キャラリスト情報更新
			this.SetCharaList(charaInfoList); ;
		}
		#endregion
		#endregion

		#region キャラページリスト
		#region セット
		/// <summary>
		/// キャラリストの設定
		/// </summary>
		private void SetCharaList(List<CharaInfo> charaInfoList)
		{
			if (this.CharaList == null) { return; }
			this.CharaList.SetupItems(charaInfoList);
		}
		#endregion

		#region クリア
		/// <summary>
		/// キャラリストクリア
		/// </summary>
		private void ClearCharaList()
		{
			if (this.CharaList == null) { return; }
			// データ初期化
			this.CharaList.Setup();
		}
		#endregion

		#region キャラ設定
		/// <summary>
		/// キャラページリスト内のアイテムが押下された時のハンドラー
		/// </summary>
		private void HandleCharaListItemClick(GUICharaItem item)
		{
			// アイテム設定
			this.SetCharaListItem(item);
		}

		/// <summary>
		/// キャラページリスト内のアイテム設定
		/// </summary>
		private void SetCharaListItem(GUICharaItem item)
		{
			if (item == null) { return; }

			var disableState = item.GetDisableState();
			var info = item.GetCharaInfo();
			if (info == null) { return; }

			switch (disableState)
			{
				case CharaItem.Controller.DisableType.None:
					if (this.Mode == ModeType.CharaInfo)
					{
						// 詳細モード時
						if (!info.IsEmpty)
						{
							// ベースキャラに設定
							this.SetBaseChara(info);
						}
					}
					else if (this.Mode == ModeType.MultiSell)
					{
						// まとめて選択時は売却リストに追加
						this.AddSellChara(info);
					}
					break;
				case CharaItem.Controller.DisableType.Bait:
					// 売却リストから外す
					this.RemoveSellChara(info);
					break;
				case CharaItem.Controller.DisableType.Select:
					// ベースキャラに設定されているアイテムならベースキャラを外す
					this.ClearBaseChara();
					// キャラ情報リスト更新
					this.UpdateCharaInfoList();
					break;
			}
		}
		#endregion

		#region ページ内キャラアイテム更新
		/// <summary>
		/// ページ内のアイテムが全て更新された時のハンドラー
		/// </summary>
		private void HandleCharaListUpdateItems()
		{
			this.UpdateCharaItemList();
		}

		/// <summary>
		/// ページ内キャラアイテムリストの更新処理
		/// </summary>
		private void UpdateCharaItemList()
		{
			if (this.CharaList == null) { return; }

			// 無効タイプ更新
			this.UpdateCharaItemListDisable();
		}

		#region 無効設定
		/// <summary>
		/// ページ内リストのアイテムの無効タイプを更新する
		/// </summary>
		private void UpdateCharaItemListDisable()
		{
			switch (this.Mode)
			{
				case ModeType.CharaInfo:
				{
					// キャラ詳細時の無効設定を行う
					var list = this.CharaList.GetNowPageItemList();
					list.ForEach(this.SetSelectItemDisableState);
					break;
				}
				case ModeType.MultiSell:
				{
					// まとめて売却時の無効設定を行う
					foreach (var item in this.CharaList.GetNowPageItemList())
					{
						this.SetMultiSellItemDisableState(item, this.GetSellItemUUIDList());
					}
					break;
				}
			}
		}
		/// <summary>
		/// キャラ選択時の無効設定
		/// </summary>
		private void SetSelectItemDisableState(GUICharaItem item)
		{
			if (item == null) { return; }

			// 無効タイプを設定する
			CharaItem.Controller.DisableType disableType;
			this.GetSelectItemDisableType(item.GetCharaInfo(), out disableType);
			item.SetDisableState(disableType);
		}
		/// <summary>
		/// まとめて選択時の無効設定
		/// </summary>
		private void SetMultiSellItemDisableState(GUICharaItem item, List<ulong> sellUUIDList)
		{
			if (item == null) { return; }

			// 無効タイプを設定する
			CharaItem.Controller.DisableType disableType;
			int sellIndex = -1;
			this.GetMultiSellItemDisableType(item.GetCharaInfo(), sellUUIDList, out disableType, out sellIndex);
			// 無効タイプを設定する
			if (sellIndex >= 0)
			{
				// 売却のインデックスがある場合は餌
				item.SetBaitState(sellIndex);
			}
			else
			{
				// それ以外は無効タイプを設定する
				item.SetDisableState(disableType);
			}
		}
		#endregion
		#endregion
		#endregion

		#region ベースキャラ
		#region ベースキャラ設定
		/// <summary>
		/// ベースキャラを設定
		/// </summary>
		private void SetBaseChara(CharaInfo charaInfo)
		{
			// ベースキャラ状態設定
			this.SetBaseCharaItemState(charaInfo);
			// キャラ情報リスト更新
			this.UpdateCharaInfoList();
		}
		#endregion

		#region アイテム状態
		/// <summary>
		/// ベースキャラアイテム状態設定
		/// </summary>
		private void SetBaseCharaItemState(CharaInfo charaInfo)
		{
			var state = CharaItem.Controller.ItemStateType.Icon;
			if (charaInfo == null || charaInfo.IsEmpty)
			{
				state = CharaItem.Controller.ItemStateType.FillEmpty;
			}
			this.BaseChara.SetState(state, charaInfo);
		}
		#endregion

		#region ベースキャラ外す
		/// <summary>
		/// ベースキャラを外す
		/// </summary>
		private void ClearBaseChara()
		{
			this.SetBaseCharaItemState(null);
		}
		#endregion

		#region ベースキャラ同期
		/// <summary>
		/// ベースキャラの変更ハンドラー
		/// </summary>
		private void HandleBaseCharaItemChange(GUICharaItem item)
		{
			this.SyncBaseCharaItem();
		}

		/// <summary>
		/// ベースキャラ同期
		/// </summary>
		private void SyncBaseCharaItem()
		{
			// ベースキャラのステータスセット
			this.SetBaseCharaStatus();
			// 実行ボタン更新
			this.UpdateExecuteButtonEnable();
		}

		/// <summary>
		/// ベースキャラのステータス情報をセットする
		/// </summary>
		private void SetBaseCharaStatus()
		{
			string name = string.Empty;
			bool isLock = false;
			int soldPrice = 0;
			float rebuildTime = 0;
			int cost = 0;
			int exp = 0;
			int synchroRemain = 0;
			int hitPoint = 0;
			int attack = 0;
			int defence = 0;
			int extra = 0;

			var baseCharaInfo = this.BaseCharaInfo;
			if (baseCharaInfo != null)
			{
				name = baseCharaInfo.Name;
				isLock = baseCharaInfo.IsLock;
				rebuildTime = baseCharaInfo.RebuildTime;
				cost = baseCharaInfo.DeckCost;
				exp = baseCharaInfo.PowerupExp;
				synchroRemain = baseCharaInfo.SynchroRemain;
				hitPoint = baseCharaInfo.HitPoint;
				attack = baseCharaInfo.Attack;
				defence = baseCharaInfo.Defense;
				extra = baseCharaInfo.Extra;
				soldPrice = Scm.Common.Utility.PlayerCharacter.GetSellingPrice(baseCharaInfo.Rank, baseCharaInfo.PowerupLevel);
			}

			// データセット
			this.Model.CharaName = name;
			this.Model.IsLock = isLock;
			this.Model.SoldPrice = soldPrice;
			this.Model.RebuildTime = rebuildTime;
			this.Model.Cost = cost;
			this.Model.Exp = exp;
			this.Model.SynchroRemain = synchroRemain;
			this.Model.HitPoint = hitPoint;
			this.Model.Attack = attack;
			this.Model.Defence = defence;
			this.Model.Extra = extra;
		}
		#endregion

		#region ベースキャラ更新
		/// <summary>
		/// ベースキャラ情報更新
		/// </summary>
		private void UpdateBaseCharaItemInfo()
		{
			if (!this.CanUpdate) { return; }
			if (this.IsEmptyBaseChara) { return; }

			// キャラ情報取得
			CharaInfo newInfo;
			if (this.Model.TryGetCharaInfo(this.BaseCharaInfo.CharacterMasterID, this.BaseCharaInfo.UUID, out newInfo))
			{
				// 更新
				this.SetBaseCharaItemState(newInfo);
			}
			else
			{
				// 存在しない場合はベースキャラを外す
				this.ClearBaseChara();
			}
		}
		#endregion

		#region ベースキャラ押下
		/// <summary>
		/// ベースキャラ押下ハンドラー
		/// </summary>
		private void HandleBaseCharaItemClick(GUICharaItem item)
		{
			if (!this.IsEmptyBaseChara)
			{
				// 設定されていたらベースキャラを外す
				this.ClearBaseChara();
				// キャラ情報リスト更新
				this.UpdateCharaInfoList();
			}
		}
		#endregion

		#region ベースキャラステータス
		/// <summary>
		/// ベースキャラ名変更ハンドラー
		/// </summary>
		private void HandleCharaNameChange(object sender, EventArgs e)
		{
			this.SyncCharaName();
		}
		/// <summary>
		/// ベースキャラ名同期
		/// </summary>
		private void SyncCharaName()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetCharaName(this.Model.CharaName);
		}

		/// <summary>
		/// ベースキャラロック変更ハンドラー
		/// </summary>
		private void HandleIsLockChange(object sender, EventArgs e)
		{
			this.SyncIsLock();
		}
		/// <summary>
		/// ベースキャラロック同期
		/// </summary>
		private void SyncIsLock()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetCharaLock(this.Model.IsLock);
		}

		/// <summary>
		/// ベースキャラの売却額変更ハンドラー
		/// </summary>
		private void HandleSoldPriceChange(object sender, EventArgs e)
		{
			this.SyncSoldPrice();
		}
		/// <summary>
		/// ベースキャラの売却額同期
		/// </summary>
		private void SyncSoldPrice()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetCharaSoldPrice(this.Model.SoldPrice, this.Model.SoldPriceFormat);
		}

		/// <summary>
		/// リビルドタイム変更イベントハンドラー
		/// </summary>
		private void HandleRebuildTimeChange(object sender, EventArgs e)
		{
			this.SyncRebuildTime();
		}
		/// <summary>
		/// リビルドタイム同期
		/// </summary>
		private void SyncRebuildTime()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetRebuildTime(this.Model.RebuildTime.ToString());
		}

		/// <summary>
		/// コスト変更イベントハンドラー
		/// </summary>
		private void HandleCostChange(object sender, EventArgs e)
		{
			this.SyncCost();
		}
		/// <summary>
		/// コスト同期
		/// </summary>
		private void SyncCost()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetCost(this.Model.Cost.ToString());
		}

		/// <summary>
		/// 経験値変更イベントハンドラー
		/// </summary>
		private void HandleExpChange(object sender, EventArgs e)
		{
			this.SyncExp();
		}
		/// <summary>
		/// 経験値フォーマット変更イベントハンドラー
		/// </summary>
		private void HandleExpFormatChange(object sender, EventArgs e)
		{
			this.SyncExp();
		}
		/// <summary>
		/// 経験値同期
		/// </summary>
		private void SyncExp()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetExp(string.Format(this.Model.ExpFormat, this.Model.Exp));
		}

		/// <summary>
		/// シンクロ可能回数変更イベントハンドラー
		/// </summary>
		private void HandleSynchroRemainChange(object sender, EventArgs e)
		{
			this.SyncSynchroRemain();
		}
		/// <summary>
		/// シンクロ可能回数同期
		/// </summary>
		private void SyncSynchroRemain()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetSynchroRemain(this.Model.SynchroRemain.ToString());
		}

		/// <summary>
		/// 生命力変更イベントハンドラー
		/// </summary>
		private void HandleHitPointChange(object sender, EventArgs e)
		{
			this.SyncHitPoint();
		}
		/// <summary>
		/// 生命力フォーマット変更イベントハンドラー
		/// </summary>
		private void HandleHitPointFormatChange(object sender, EventArgs e)
		{
			this.SyncHitPoint();
		}
		/// <summary>
		/// 生命力同期
		/// </summary>
		private void SyncHitPoint()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetHitPoint(string.Format(this.Model.HitPointFormat, this.Model.HitPoint));
		}

		/// <summary>
		/// 攻撃力変更イベントハンドラー
		/// </summary>
		private void HandleAttackChange(object sender, EventArgs e)
		{
			this.SyncAttack();
		}
		/// <summary>
		/// 攻撃力フォーマット変更イベントハンドラー
		/// </summary>
		private void HandleAttackFormatChange(object sender, EventArgs e)
		{
			this.SyncAttack();
		}
		/// <summary>
		/// 攻撃力同期
		/// </summary>
		private void SyncAttack()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetAttack(string.Format(this.Model.AttackFormat, this.Model.Attack));
		}

		/// <summary>
		/// 防御力変更イベントハンドラー
		/// </summary>
		private void HandleDefenceChange(object sender, EventArgs e)
		{
			this.SyncDefence();
		}
		/// <summary>
		/// 防御力フォーマット変更イベントハンドラー
		/// </summary>
		private void HandleDefenceFormatChange(object sender, EventArgs e)
		{
			this.SyncDefence();
		}
		/// <summary>
		/// 防御力同期
		/// </summary>
		private void SyncDefence()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetDefence(string.Format(this.Model.DefenceFormat, this.Model.Defence));
		}

		/// <summary>
		/// 特殊能力変更イベントハンドラー
		/// </summary>
		private void HandleExtraChange(object sender, EventArgs e)
		{
			this.SyncExtra();
		}
		/// <summary>
		/// 特殊能力フォーマット変更イベントハンドラー
		/// </summary>
		private void HandleExtraFormatChange(object sender, EventArgs e)
		{
			this.SyncExtra();
		}
		/// <summary>
		/// 特殊能力同期
		/// </summary>
		private void SyncExtra()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetExtra(string.Format(this.Model.ExtraFormat, this.Model.Extra));
		}

		/// <summary>
		/// 全てのベースキャラのステータスを同期する
		/// </summary>
		private void SyncBaseCharaStatus()
		{
			this.SyncCharaName();
			this.SyncIsLock();
			this.SyncSoldPrice();
			this.SyncRebuildTime();
			this.SyncCost();
			this.SyncExp();
			this.SyncSynchroRemain();
			this.SyncHitPoint();
			this.SyncAttack();
			this.SyncDefence();
			this.SyncExtra();
		}
		#endregion

		#region ベースキャラロック
		/// <summary>
		/// キャラロックボタンクリックイベントハンドラー
		/// </summary>
		private void HandleLock(object sender, EventArgs e)
		{
			// ベースキャラが設定されている状態かチェック
			if (this.IsEmptyBaseChara) { return; }

			// 通知
			var eventArgs = new CharaLockEventArgs();
			eventArgs.UUID = this.BaseCharaInfo.UUID;
			eventArgs.IsLock = !this.BaseCharaInfo.IsLock;
			this.OnCharaLock(this, eventArgs);
		}
		#endregion
		#endregion

		#region 売却リスト
		#region 追加/削除
		/// <summary>
		/// 売却リストに追加する
		/// </summary>
		private void AddSellChara(CharaInfo charaInfo)
		{
			if (this.SellCharaList == null || charaInfo == null) { return; }

			// 追加
			if (this.SellCharaList.AddChara(charaInfo))
			{
				// 売却リストの選択アイテム更新
				this.UpdateSelectSellCharaList();
				// 売却リストのスクロール更新
				this.SellCharaList.UpdateScroll();
				// キャラ情報リスト更新
				this.UpdateCharaInfoList();
			}
		}

		/// <summary>
		/// 売却リストから削除する
		/// </summary>
		private void RemoveSellChara(CharaInfo charaInfo)
		{
			if (this.SellCharaList == null || charaInfo == null) { return; }

			// 削除
			if (this.SellCharaList.RemoveChara(charaInfo))
			{
				// 売却リストの選択アイテム更新
				this.UpdateSelectSellCharaList();
				// キャラ情報リスト更新
				this.UpdateCharaInfoList();
			}
		}
		#endregion

		#region クリア
		/// <summary>
		/// 売却リストクリア
		/// </summary>
		private void ClearSellCharaList()
		{
			if (this.SellCharaList == null) { return; }
			this.SellCharaList.ClearChara();
			this.SellCharaList.UpdateScroll();

			// 売却リストの選択アイテム更新
			this.UpdateSelectSellCharaList();
		}
		#endregion

		#region 選択
		/// <summary>
		/// 売却リストの選択アイテム更新
		/// </summary>
		private void UpdateSelectSellCharaList()
		{
			if (this.SellCharaList == null) { return; }
			List<GUICharaItem> itemList = this.SellCharaList.GetNowPageItemList();
			int nextSetIndex = this.SellCharaList.GetCharaInfoList().Count;

			for (int index = 0; index < itemList.Count; ++index)
			{
				bool isSelect = false;
				if (index == nextSetIndex)
				{
					isSelect = true;
				}
				itemList[index].SetSelect(isSelect);
			}
		}
		#endregion

		#region アイテム押下イベント
		/// <summary>
		/// 売却リスト内のアイテムが押された時のハンドラー
		/// </summary>
		private void HandleSellListItemClick(GUICharaItem item)
		{
			if (item == null || item.GetCharaInfo() == null) { return; }
			CharaInfo charaInfo = item.GetCharaInfo();
			if (charaInfo.UUID > 0)
			{
				// キャラが追加されていたら削除する
				this.RemoveSellChara(charaInfo);
			}
		}
		#endregion

		#region 同期
		/// <summary>
		/// 売却リスト内のアイテムが全て更新された時のハンドラー
		/// </summary>
		private void HandleSellListUpdateItems()
		{
			this.SyncSellList();
		}

		/// <summary>
		/// 売却リスト同期
		/// </summary>
		private void SyncSellList()
		{
			// 試算
			this.MultiSellCalc();
		}
		#endregion

		#region 更新
		/// <summary>
		/// 売却アイテム情報更新
		/// </summary>
		private void UpdateSellCharaInfoList(List<CharaInfo> charaInfoList)
		{
			var sellUUIDList = this.GetSellItemUUIDList();
			// 新しい餌リスト初期化
			var newInfoList = new List<CharaInfo>(sellUUIDList.Count);
			sellUUIDList.ForEach(t => { newInfoList.Add(null); });

			// 更新させるキャラ情報を抽出
			var count = 0;
			foreach (var info in charaInfoList)
			{
				if (info == null) continue;
				var index = sellUUIDList.FindIndex(t => { return t == info.UUID; });
				if (index == -1) continue;

				newInfoList[index] = info;
				count++;
				if (count >= sellUUIDList.Count)
				{
					// 抽出完了
					break;
				}
			}

			// 更新
			this.SellCharaList.ClearChara();
			newInfoList.ForEach(info => { this.SellCharaList.AddChara(info); });

			// 売却リストの選択アイテム更新
			this.UpdateSelectSellCharaList();
			// キャラ情報リスト更新
			this.UpdateCharaInfoList();
		}
		#endregion

		#region 取得
		/// <summary>
		/// 売却アイテムのUUIDリストを取得する
		/// </summary>
		private List<ulong> GetSellItemUUIDList()
		{
			var uuidList = new List<ulong>();
			if (this.SellCharaList == null) { return uuidList; }

			List<CharaInfo> infoList = this.SellCharaList.GetCharaInfoList();
			if (infoList != null)
			{
				infoList.ForEach(t => { uuidList.Add(t.UUID); });
			}

			return uuidList;
		}
		#endregion
		#endregion

		#region Boxモード
		#region BOXモード設定処理
		/// <summary>
		/// BOXモードの設定
		/// </summary>
		private void SetMode(ModeType mode)
		{
			// 更新
			this.Mode = mode;

			foreach (KeyValuePair<ModeType, Action<bool>> kvp in this.changeModeExecuteDic)
			{
				bool isChange = false;
				if (kvp.Key == mode)
				{
					// 切り替る項目と一致していたら切替フラグをONにする
					isChange = true;
				}
				kvp.Value(isChange);
			}

			if (this.CanUpdate)
			{
				// モードによって表示物を切り替える
				if (mode == ModeType.MultiSell)
				{
					this.View.SetMultiSellGroupActive(true);
					this.View.SetSelectGroupActive(false);
				}
				else
				{
					this.View.SetMultiSellGroupActive(false);
					this.View.SetSelectGroupActive(true);
				}
			}

			// メッセージ更新
			this.UpdateMessage();
		}
		/// <summary>
		/// BOXモードの切り替え
		/// </summary>
		private void ChangeMode(ModeType mode)
		{
			// 同モードが指定されている場合は切替処理を行わない
			if (this.Mode == mode) { return; }
			// モード切替
			SetMode(mode);
		}

		/// <summary>
		/// キャラ詳細モード処理
		/// </summary>
		private void CharaInfoMode(bool isChange)
		{
			if (!this.CanUpdate) { return; }

			if (isChange)
			{
				// ベースキャラ外す
				this.ClearBaseChara();
				// 売却リスト初期化
				this.ClearSellCharaList();
				// キャラ情報リスト更新
				this.UpdateCharaInfoList();

				// 実行ボタン名セット
				this.View.SetExecuteLabel(MasterData.GetText(TextType.TX411_CharaBox_Info));
			}

			// ボタンの表示の切り替え
			this.View.SetCharaInfoModeEnable(isChange);
		}

		/// <summary>
		/// まとめて売却モード処理
		/// </summary>
		private void SellMultiMode(bool isChange)
		{
			if (!this.CanUpdate) { return; }

			if (isChange)
			{
				// キャラ情報リスト更新
				this.UpdateCharaInfoList();

				// ベースキャラが設定されていたら売却リストに登録
				if (!this.IsEmptyBaseChara)
				{
					var disableType = CharaItem.Controller.DisableType.None;
					int sellIndex = -1;
					this.GetMultiSellItemDisableType(this.BaseCharaInfo, this.GetSellItemUUIDList(), out disableType, out sellIndex);
					if (disableType == CharaItem.Controller.DisableType.None)
					{
						this.AddSellChara(this.BaseCharaInfo);
					}
				}

				// 実行ボタン名セット
				this.View.SetExecuteLabel(MasterData.GetText(TextType.TX353_ItemBox_MultiSell));
			}

			// ボタンの表示の切り替え
			this.View.SetSellMultiModeEnable(isChange);
		}
		#endregion

		#region モードボタン押下イベント
		/// <summary>
		/// キャラ詳細モードボタン押下イベントハンドラー
		/// </summary>
		private void HandleCharaInfoMode(object sender, EventArgs e)
		{
			// モードに切り替える
			this.ChangeMode(ModeType.CharaInfo);
		}

		/// <summary>
		/// まとめて売却ボタン押下イベントハンドラー
		/// </summary>
		private void HandleSellMultiMode(object sender, EventArgs e)
		{
			// まとめて売却モードに切り替える
			this.ChangeMode(ModeType.MultiSell);
		}
		#endregion
		#endregion

		#region まとめて売却
		#region まとめて売却試算
		/// <summary>
		/// まとめて売却の試算処理
		/// </summary>
		private void MultiSellCalc()
		{
			if (this.SellCharaList == null) { return; }

			// 売却するキャラのUUIDリスト取得
			List<ulong> sellUUIDList = this.GetSellItemUUIDList();
			if(sellUUIDList.Count > 0)
			{
				// 売却可能
				var eventArgs = new SellMultiCalcEventArgs();
				eventArgs.SellCharaUUIDList = sellUUIDList;

				// 通知
				this.OnSellMultiCalc(this, eventArgs);
			}
			else
			{
				// 売却不可
				if(this.Model != null)
				{
					this.Model.TotalSoldPrice = 0;
				}

				// 所持金同期
				this.SyncHaveMoney();
				// 実行ボタン有効設定更新
				this.UpdateExecuteButtonEnable();
			}
		}

		/// <summary>
		/// 売却試算結果
		/// </summary>
		public void MultiSellCalcResult(bool result, List<ulong> sellUUIDList, int money, int soldPrice, int addOnCharge)
		{
			if (!this.CanUpdate) { return; }
			if (!result) { return; }

			this.Model.TotalSoldPrice = soldPrice;
			this.Model.AddOnCharge = addOnCharge;

			// 所持金同期
			this.SyncHaveMoney();
			// 実行ボタン有効設定更新
			this.UpdateExecuteButtonEnable();
		}
		#endregion

		#region 売却処理
		/// <summary>
		/// まとめて売却処理
		/// </summary>
		private void SellMulti()
		{
			if (this.SellCharaList == null) { return; }

			// 売却リストにアイテムがセットされているかチェック
			if (this.GetSellItemUUIDList().Count <= 0) { return; }

			// ロックチェック処理へ
			this.CheckLock();

		}
		/// <summary>
		/// ロックチェック処理
		/// </summary>
		private void CheckLock()
		{
			// ロックチェック
			bool isLock = false;
			if (this.SellCharaList != null)
			{
				var list = this.SellCharaList.GetCharaInfoList();
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
				// 売却チェック処理へ
				this.CheckSell();
			}
		}
		/// <summary>
		/// 売却チェック
		/// </summary>
		private void CheckSell()
		{
			GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX417_CharaBox_CheckSell), true, this.CheckHaveMoney, null);
		}
		/// <summary>
		/// 所持金チェック処理
		/// </summary>
		private void CheckHaveMoney()
		{
			if (!this.CanUpdate) { return; }
			if (this.Model.HaveMoney >= MasterDataCommonSetting.Player.PlayerMaxGameMoney)
			{
				// 所持金が上限に達しているので確認メッセージを表示
				GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX347_SellItem_HaveMoneyOver), true, this.CheckSlot, null);
			}
			else
			{
				// スロットチェック処理へ
				this.CheckSlot();
			}
		}
		/// <summary>
		/// 売却キャラの中に装着しているスロットが存在しているかチェック
		/// </summary>
		private void CheckSlot()
		{
			if (!this.CanUpdate) { return; }
			if(this.Model.AddOnCharge > 0)
			{
				// 売却キャラの中にスロットに装着されているキャラが存在する
				string messgae = string.Format(MasterData.GetText(TextType.TX418_CharaBox_RemoveSlot), this.Model.AddOnCharge);
				GUIMessageWindow.SetModeYesNo(messgae, true, this.CheckPayRemoveSlot, null);
			}
			else
			{
				// スロットに装着しているキャラが存在しない
				this.MultiSellExecute();
			}

		}
		/// <summary>
		/// スロット解除支払チェック
		/// </summary>
		private void CheckPayRemoveSlot()
		{
			if (!this.CanUpdate) { return; }
			if (this.Model.HaveMoney < this.Model.AddOnCharge)
			{
				// スロット解除料金が足りない
				GUIMessageWindow.SetModeOK(MasterData.GetText(TextType.TX419_CharaBox_NotRemoveSlot), true, null);
			}
			else
			{
				// 解除支払可能
				this.MultiSellExecute();
			}
		}
		/// <summary>
		/// まとめて売却実行
		/// </summary>
		private void MultiSellExecute()
		{
			// 通知
			var eventArgs = new SellMultiEventArgs();
			eventArgs.SellCharaUUIDList = this.GetSellItemUUIDList();

			this.OnSellMulti(this, eventArgs);
		}
		/// <summary>
		/// まとめて売却結果
		/// </summary>
		public void MultiSellResult(bool result, List<ulong> sellUUIDList, int money, int soldPrice, int addOnCharge)
		{
			if (!result) { return; }

			// ベースキャラ外す
			this.ClearBaseChara();
			// 売却リストクリア
			this.ClearSellCharaList();
			// キャラ情報リスト更新
			this.UpdateCharaInfoList();

			// 売却額リセット
			this.Model.TotalSoldPrice = 0;
			this.Model.AddOnCharge = 0;
		}
		#endregion
		#endregion

		#region 実行ボタン
		/// <summary>
		/// 実行ボタンが押された時のイベントハンドラー
		/// </summary>
		private void HandleExecute(object sender, EventArgs e)
		{
			switch (this.Mode)
			{
				case ModeType.CharaInfo:
					// キャラ詳細画面表示
					this.OpenCharaInfo();
					break;
				case ModeType.MultiSell:
					// まとめて売却処理
					this.SellMulti();
					break;
			}
		}

		/// <summary>
		/// 実行ボタン有効設定
		/// </summary>
		private void UpdateExecuteButtonEnable()
		{
			if (!this.CanUpdate) { return; }

			bool isEnable = false;
			switch (this.Mode)
			{
				case ModeType.CharaInfo:
					{
						if (!this.IsEmptyBaseChara)
						{
							isEnable = true;
						}
						break;
					}
				case ModeType.MultiSell:
					{
						if (this.GetSellItemUUIDList().Count > 0)
						{
							isEnable = true;
						}
						break;
					}
			}

			// ボタン有効設定
			this.View.SetExecuteButtonEnable(isEnable);
		}
		#endregion

		#region 詳細画面
		/// <summary>
		/// 詳細画面表示
		/// </summary>
		private void OpenCharaInfo()
		{
			if (this.IsEmptyBaseChara) { return; }

			// 詳細画面表示
			ulong uuid = this.BaseCharaUUID;
			var screen = new GUIScreen(() => { GUICharacterInfo.Open(uuid); }, GUICharacterInfo.Close, () => { GUICharacterInfo.ReOpen(uuid); });
			GUIController.Open(screen);
		}
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金変更ハンドラー
		/// </summary>
		private void HandleHaveMoneyChange(object sender, EventArgs e)
		{
			this.SyncHaveMoney();
		}
		/// <summary>
		/// 所持金フォーマット変更ハンドラー
		/// </summary>
		private void HandleHaveMoneyFormatChange(object sender, EventArgs e)
		{
			this.SyncHaveMoney();
		}

		/// <summary>
		/// 所持金同期
		/// </summary>
		private void SyncHaveMoney()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetHaveMoney(this.Model.HaveMoney, this.Model.HaveMoneyFormat);
		}
		#endregion

		#region 総売却額
		/// <summary>
		/// 総売却額変更ハンドラー
		/// </summary>
		private void HandleTotalSoldPriceChange(object sender, EventArgs e)
		{
			this.SyncTotalSoldPrice();
		}
		/// <summary>
		/// 総売却額フォーマット変更ハンドラー
		/// </summary>
		private void HandleTotalSoldPriceFormatChange(object sender, EventArgs e)
		{
			this.SyncTotalSoldPrice();
		}
		/// <summary>
		/// 総売却額同期
		/// </summary>
		private void SyncTotalSoldPrice()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetTotalSoldPrice(this.Model.TotalSoldPrice, this.Model.TotalSoldPriceFormat);
		}
		#endregion

		#region 無効設定取得
		/// <summary>
		/// キャラ選択時の無効タイプを取得する
		/// </summary>
		private void GetSelectItemDisableType(CharaInfo charaInfo, out CharaItem.Controller.DisableType disableType)
		{
			disableType = CharaItem.Controller.DisableType.None;
			if (charaInfo == null) { return; }

			// 以下無効にするかチェック
			// 優先順位があるので注意
			bool isSelect = charaInfo.UUID == this.BaseCharaUUID;
			if (isSelect)
			{
				// アイテム選択中
				disableType = CharaItem.Controller.DisableType.Select;
			}
		}

		/// <summary>
		/// まとめて売却時の無効タイプを取得する
		/// </summary>
		private void GetMultiSellItemDisableType(CharaInfo charaInfo, List<ulong> sellUUIDList, out CharaItem.Controller.DisableType disableType, out int sellIndex)
		{
			disableType = CharaItem.Controller.DisableType.None;
			sellIndex = -1;
			if (charaInfo == null) { return; }

			// 以下無効にするかチェック
			// 優先順位があるので注意
			sellIndex = sellUUIDList.FindIndex((uuid) => { return uuid == charaInfo.UUID; });
			if (sellIndex >= 0)
			{
				// 売却アイテム選択中
				disableType = CharaItem.Controller.DisableType.Bait;
			}
			// スロットに入っている
			else if (charaInfo.IsInSlot) disableType = CharaItem.Controller.DisableType.PowerupSlot;
			// デッキに入っている
			else if (charaInfo.IsInDeck) disableType = CharaItem.Controller.DisableType.Deck;
			// シンボルに設定している
			else if (charaInfo.IsSymbol) disableType = CharaItem.Controller.DisableType.Symbol;
			// ロックされている
			else if (charaInfo.IsLock) disableType = CharaItem.Controller.DisableType.Lock;
		}
		#endregion

		#region ホーム、閉じるボタンイベント
		/// <summary>
		/// ホーム
		/// </summary>
		private void HandleHome(object sender, HomeClickedEventArgs e)
		{
			if (this.CharaList != null)
			{
				// Newフラグ一括解除
				this.CharaList.DeleteAllNewFlag();
			}

			GUIController.Clear();
		}

		/// <summary>
		/// 閉じる
		/// </summary>
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
	}
}