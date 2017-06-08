/// <summary>
/// キャラクター詳細制御
/// 
/// 2016/03/25
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace XUI.CharacterInfo
{

	/// <summary>
	/// キャラクターのロック
	/// </summary>
	public class SetLockPlayerCharacterEventArgs : EventArgs
	{
		public ulong UUID { get; set; }
		public bool IsLock { get; set; }
	}


	public interface IController
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region 初期化
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

		#region キャラクター情報の更新
		/// <summary>
		/// キャラクタ詳細の設定
		/// </summary>
		void SetCharaInfo(CharaInfo info, int slotHitPoint, int slotAttack, int slotDefense, int slotExtra, List<CharaInfo> slotList);

		#endregion

		#region キャラクターロック失敗メッセージの表示
		/// <summary>
		/// キャラクターロック失敗メッセージの表示
		/// </summary>
		void CharacterLockError();

		#endregion

		#region ロック状態の更新
		event EventHandler<SetLockPlayerCharacterEventArgs> OnLock;

		/// <summary>
		/// ロック状態の設定
		/// </summary>
		void SetIsLock(bool isLock);
		#endregion
	}


	public class Controller : IController
	{
		#region 文字列
		string ScreenTitle { get { return MasterData.GetText( TextType.TX405_CharacterInfo_Title ); } }
		string HelpMessage { get { return MasterData.GetText( TextType.TX406_CharacterInfo_HelpMessage ); } }
		string LockErrorMessage { get { return MasterData.GetText( TextType.TX407_CharacterInfo_LockErrorMessage ); } }
		#endregion

		#region フィールド＆プロパティ
		// モデル
		readonly IModel _model;
		IModel Model { get { return _model; } }
		// ビュー
		readonly IView _view;
		IView View { get { return _view; } }
		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate
		{
			get
			{
				if (this.Model == null) return false;
				if (this.View == null) return false;
				return true;
			}
		}

		// ベースキャラ
		readonly GUICharaItem _charaItem;
		GUICharaItem CharaItem { get { return _charaItem; } }
		// ベースキャラのキャラ情報
		CharaInfo CharaItemInfo { get { return (this.CharaItem != null ? this.CharaItem.GetCharaInfo() : null); } }
        // スロットリスト
        readonly GUICharaSlotList _slotList;
		GUICharaSlotList SlotList { get { return _slotList; } }

		/// <summary>
		/// キャラボード
		/// </summary>
		private readonly CharaBoard _charaBoard;
		private CharaBoard CharaBoard { get { return _charaBoard; } }

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

			this.OnLock = null;
		}
		#endregion

		#region 初期化
		// シリアライズされていないメンバーの初期化
		void MemberInit()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view,GUICharaItem charaItem, GUICharaSlotList slotList, CharaBoard board)
		{
			if (model == null || view == null) return;

			// キャラアイテムの設定
			this._charaItem = charaItem;
			this._charaItem.SetButtonEnable(false);

			this._slotList = slotList;
			this.SlotList.SetItemsButtonEnable(false);

			this._charaBoard = board;

			// ビュー設定
			this._view = view;
			this._view.OnHome += this.HandleHome;
			this._view.OnClose += this.HandleClose;
			this._view.OnLockChange += this.HandleOnLockChange;

			// モデル設定
			this._model = model;
			// 名前
			this.Model.OnCharaNameChange += this.HandleCharaNameChange;
			this.Model.OnCharaNameFormatChange += this.HandleCharaNameFormatChange;
			// リビルド時間
			this.Model.OnRebuildTimeCgange += this.HandleRebuildTimeChange;
			this.Model.OnRebuildTimeFormatChange += this.HandleRebuildTimeFormatChange;
			// キャラコスト
			this.Model.OnCharaCostCgange += this.HandleCharaCostChange;
			this.Model.OnCharaCostFormatChange += this.HandleCharaCostFormatChange;
			// ロック状態
			this.Model.OnLockChange += this.HandleLockChange;
			// ランク・レベル
			this.Model.OnRankChange += this.HandleRankChange;
			this.Model.OnRankFormatChange += this.HandleRankFormatChange;
			this.Model.OnPowerupLevelChange += this.HandleLevelChange;
			this.Model.OnPowerupLevelFormatChange += this.HandleLevelFormatChange;
			this.Model.OnPowerupExpChange += this.HandleExpChange;
			this.Model.OnPowerupExpFormatChange += this.HandleExpFormatChange;
			this.Model.OnSynchroRemainChange += this.HandleSynchroRemainChange;
			this.Model.OnSynchroRemainFormatChange += this.HandleSynchroRemainFormatChange;
			this.Model.OnPowerupSlotChange += this.HandlePowerupSlotChange;
			this.Model.OnPowerupSlotNumChange += this.HandlePowerupSlotChange;
			this.Model.OnPowerupSlotFormatChange += this.HandlePowerupSlotFormatChange;
			// ステータス情報の更新
			// 生命力の更新
			this.Model.OnTotalHitPointChange += this.HandleTotalHitPointChange;
			this.Model.OnTotalHitPointFormatChange += this.HandleTotalHitPointFormatChange;
			this.Model.OnBaseHitPointChange += this.HandleBaseHitPointChange;
			this.Model.OnBaseHitPointFormatChange += this.HandleBaseHitPointFormatChange;
			this.Model.OnSlotHitPointChange += this.HandleSlotHitPointChange;
			this.Model.OnSlotHitPointFormatChange += this.HandleSlotHitPointFormatChange;
			this.Model.OnSyncHitPointChange += this.HandleSyncHitPointChange;
			this.Model.OnSyncHitPointFormatChange += this.HandleSyncHitPointFormatChange;
			// 攻撃力の更新
			this.Model.OnTotalAttackChange += this.HandleTotalAttackChange;
			this.Model.OnTotalAttackFormatChange += this.HandleTotalAttackFormatChange;
			this.Model.OnBaseAttackChange += this.HandleBaseAttackChange;
			this.Model.OnBaseAttackFormatChange += this.HandleBaseAttackFormatChange;
			this.Model.OnSlotAttackChange += this.HandleSlotAttackChange;
			this.Model.OnSlotAttackFormatChange += this.HandleSlotAttackFormatChange;
			this.Model.OnSyncAttackChange += this.HandleSyncAttackChange;
			this.Model.OnSyncAttackFormatChange += this.HandleSyncAttackFormatChange;
			// 防御力の更新
			this.Model.OnTotalDefenseChange += this.HandleTotalDefenseChange;
			this.Model.OnTotalDefenseFormatChange += this.HandleTotalDefenseFormatChange;
			this.Model.OnBaseDefenseChange += this.HandleBaseDefenseChange;
			this.Model.OnBaseDefenseFormatChange += this.HandleBaseDefenseFormatChange;
			this.Model.OnSlotDefenseChange += this.HandleSlotDefenseChange;
			this.Model.OnSlotDefenseFormatChange += this.HandleSlotDefenseFormatChange;
			this.Model.OnSyncDefenseChange += this.HandleSyncDefenseChange;
			this.Model.OnSyncDefenseFormatChange += this.HandleSyncDefenseFormatChange;
			// 特殊能力の更新
			this.Model.OnTotalExtraChange += this.HandleTotalExtraChange;
			this.Model.OnTotalExtraFormatChange += this.HandleTotalExtraFormatChange;
			this.Model.OnBaseExtraChange += this.HandleBaseExtraChange;
			this.Model.OnBaseExtraFormatChange += this.HandleBaseExtraFormatChange;
			this.Model.OnSlotExtraChange += this.HandleSlotExtraChange;
			this.Model.OnSlotExtraFormatChange += this.HandleSlotExtraFormatChange;
			this.Model.OnSyncExtraChange += this.HandleSyncExtraChange;
			this.Model.OnSyncExtraFormatChange += this.HandleSyncExtraFormatChange;
			// アバタータイプ更新
			this.Model.OnAvatarTypeChange += this.HandleAvatarTypeChange;
			// 強化スロットリストの更新
			if (this.SlotList != null)
			{
				this.SlotList.OnItemChangeEvent += this.HandleSlotListItemChangeEvent;
				this.SlotList.OnItemClickEvent += this.HandleSlotListItemClickEvent;
				this.SlotList.OnItemLongPressEvent += this.HandleSlotListItemLongPressEvent;
				this.SlotList.OnUpdateItemsEvent += this.HandleSlotListUpdateItemsEvent;
			}
			// キャラアイテムの更新
			if(this.CharaItem != null)
			{
				this.CharaItem.OnItemChangeEvent += this.HandleCharaItemChangeEvent;
				this.CharaItem.OnItemClickEvent += this.HandleCharaItemClickEvent;
				this.CharaItem.OnItemLongPressEvent += this.HandleCharaItemLongPressEvent;
			}



			// 同期
			this.SyncCharaName();       // キャラネーム
			this.SyncRebuildTime();     // リビルド時間
			this.SyncCharaCost();       // キャラコスト
			this.SyncLock();            // ロック
			this.SyncRank();            // ランク
			this.SyncLevel();           // レベル
			this.SyncExp();             // 所持経験値
			this.SyncSynchroRemain();	// 残りシンクロ合成回数
			this.SyncPowerupSlot();		// 強化スロット
			this.SyncTotalHitPoint();   // 合計生命力の更新
			this.SyncTotalAttack();     // 合計攻撃力の更新
			this.SyncTotalDefense();    // 合計防御力の更新
			this.SyncTotalExtra();		// 合計特殊能力の更新
			this.SyncBaseHitPoint();	// 基礎生命力の更新
			this.SyncBaseAttack();		// 基礎攻撃力の更新
			this.SyncBaseDefense();		// 基礎防御力の更新
			this.SyncBaseExtra();		// 基礎特殊能力の更新
			this.SyncSlotHitPoint();	// スロット生命力の更新
			this.SyncSlotAttack();		// スロット攻撃力の更新
			this.SyncSlotDefense();		// スロット防御力の更新
			this.SyncSlotExtra();		// スロット特殊能力の更新
			this.SyncSyncHitPoint();	// シンクロ生命力の更新
			this.SyncSyncAttack();		// シンクロ攻撃力の更新
			this.SyncSyncDefense();		// シンクロ防御力の更新
			this.SyncSyncExtra();		// シンクロ特殊能力の更新


		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			this.MemberInit();
			this.ClearCharaItem();
			this.ClearSlotList();

			// 同期

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
				GUIScreenTitle.Play(isActive, this.ScreenTitle);
				GUIHelpMessage.Play(isActive, this.HelpMessage);

			}
		}
		#endregion

		#region ホーム、閉じるボタンイベント
		void HandleHome(object sender, EventArgs e)
		{
			GUIController.Clear();
		}
		void HandleClose(object sender, EventArgs e)
		{
			GUIController.Back();
		}
		#endregion

		#region キャラクターネーム
		// キャラクターネーム更新
		public void HandleCharaNameChange(object sender,EventArgs e)
		{
			// キャラクターネーム更新
			this.SyncCharaName();
		}

		// キャラクターネームフォーマット更新
		public void HandleCharaNameFormatChange(object sender, EventArgs e)
		{
			// キャラクターネーム更新
			this.SyncCharaName();
		}

		// 同期
		void SyncCharaName()
		{
			// キャラクターネーム更新
			if(this.CanUpdate) this.View.SetCharaName(this.Model.CharaName, this.Model.CharaNameFormat);
		}

		#endregion

		#region リビルド
		public void HandleRebuildTimeChange(object sender, EventArgs e)
		{
			// リビルドタイム更新
			this.SyncRebuildTime();

		}

		// キャラクターネームフォーマット
		public void HandleRebuildTimeFormatChange(object sender, EventArgs e)
		{
			// リビルドタイム更新
			this.SyncRebuildTime();

		}

		// 同期
		void SyncRebuildTime()
		{
			// リビルドタイム更新
			if(this.CanUpdate)this.View.SetRebuildTime(this.Model.RebuidTime, this.Model.RebuildTimeFormat);
		}

		#endregion

		#region キャラクターコスト更新
		// キャラクターコスト更新
		public void HandleCharaCostChange(object sender, EventArgs e)
		{
			// コスト更新
			this.SyncCharaCost();
		}

		// キャラクターネームフォーマット
		public void HandleCharaCostFormatChange(object sender, EventArgs e)
		{
			// コスト更新
			this.SyncCharaCost();
		}

		// 同期
		void SyncCharaCost()
		{
			// コスト更新
			if(this.CanUpdate)this.View.SetCharaCost(this.Model.CharaCost, this.Model.CharaCsotFormat);
		}
		#endregion

		#region キャラクターロック失敗メッセージの表示
		/// <summary>
		/// キャラクターロック失敗メッセージの表示
		/// </summary>
		public void CharacterLockError() {

			GUIMessageWindow.SetModeOK( LockErrorMessage, null );
		}
		#endregion

		#region ロック状態の更新
		/// <summary>
		/// ロックイベント
		/// </summary>
		public event EventHandler<SetLockPlayerCharacterEventArgs> OnLock = (sender, e) => { };

		/// <summary>
		/// ロック状態の更新
 		/// </summary>
		public void HandleOnLockChange(object sender, EventArgs e)
		{
			this.SetOnLockChange();
		}

		/// <summary>
		/// ロック状態の更新を送信
		/// </summary>
		public void SetOnLockChange()
		{
			if (this.CanUpdate)
			{
				// 通知
				var eventArgs = new SetLockPlayerCharacterEventArgs();
				eventArgs.UUID = this.CharaItemInfo.UUID;
				eventArgs.IsLock = !this.Model.IsLock;
				this.OnLock(this, eventArgs);
			}
		}
		/// <summary>
		/// ロック状態のデータ設定
		/// </summary>
		public void SetIsLock(bool isLock)
		{
			// ロック状態の更新
			if(this.CanUpdate)this.Model.IsLock = isLock;
		}

		/// <summary>
		/// ロックボタンの表示切り替えイベント
		/// </summary>
		public void HandleLockChange(object sender,EventArgs e)
		{
			this.SyncLock();
		}

		/// <summary>
		/// ロックアイコンの同期
		/// </summary>
		void SyncLock()
		{
			// ロック状態の更新
			if(this.CanUpdate)this.View.SetLockSprite(this.Model.IsLock);
		}
		#endregion  

		#region キャラクタ情報の更新
		/// <summary>
		/// キャラクター情報の登録
		/// </summary>
		public void SetCharaInfo(CharaInfo info, int slotHitPoint,int slotAttack,int slotDefense,int slotExtra,List<CharaInfo> slotList)
		{
			if (this.CanUpdate)
			{
				if (info != null)
				{
					this.Model.SlotHitPoint = slotHitPoint;             // スロット生命力
					this.Model.SlotAttack = slotAttack;                 // スロット攻撃力
					this.Model.SlotDefense = slotDefense;               // スロット防御力
					this.Model.SlotExtra = slotExtra;					// スロット特殊能力
					this.Model.PowerupSlot = slotList.Count;            // スロット数

					// 外部アイテム関連
					this.SetChara(info);									// キャラの更新
					this.UpdateSlotList(slotList);     // スロット更新
				}
			}
		}


		#endregion

		#region 強化関連

		#region ランク
		/// <summary>
		/// ランクの更新イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleRankChange(object sender,EventArgs e)
		{
			this.SyncRank();
		}
		public void HandleRankFormatChange(object sender, EventArgs e)
		{
			this.SyncRank();
		}
		// 同期
		void SyncRank()
		{
			if (this.CanUpdate) this.View.SetRank(this.Model.Rank,this.Model.RankFormat);
		}
		#endregion

		#region レベル
		/// <summary>
		/// レベルの更新イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleLevelChange(object sender, EventArgs e)
		{
			this.SyncLevel();
		}
		public void HandleLevelFormatChange(object sender, EventArgs e)
		{
			this.SyncLevel();
		}
		// 同期
		void SyncLevel()
		{
			if (this.CanUpdate) this.View.SetPowerupLevel(this.Model.PowerupLevel,CharaInfo.GetMaxLevel( this.Model.Rank ), this.Model.PowerupLevelFormat);
		}


		/// <summary>
		/// 所持経験値の更新イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleExpChange(object sender, EventArgs e)
		{
			this.SyncExp();
		}
		public void HandleExpFormatChange(object sender, EventArgs e)
		{
			this.SyncExp();
		}
		// 同期
		void SyncExp()
		{
			if (this.CanUpdate)
			{
				int exp = 0, nextLvTotalExp = 0;

				//　情報取得
				exp = this.Model.PowerupExp;

				// 今のレベルは最大レベルか
				if (!CharaInfo.IsMaxLevel(this.Model.Rank, this.Model.PowerupLevel))
				{
					// 次のレベルの必要経験値を取得する
					nextLvTotalExp = CharaInfo.GetTotalExp(this.Model.Rank, this.Model.PowerupLevel + 1);

					// 描画する
					this.View.SetPowerupExp(exp, this.Model.PowerupExpFormat);
					this.View.SetPowerupNextExp(nextLvTotalExp - exp, this.Model.PowerupExpFormat);

				}
				else
				{
					// 描画する
					this.View.SetPowerupExp(exp, this.Model.PowerupExpFormat);
					this.View.SetPowerupNextExp(0, this.Model.PowerupExpFormat);
				}

			}
		}

		#endregion

		#region シンクロ回数
		/// <summary>
		/// 残りシンクロ合成回数
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleSynchroRemainChange(object sender, EventArgs e)
		{
			this.SyncSynchroRemain();
		}
		public void HandleSynchroRemainFormatChange(object sender, EventArgs e)
		{
			this.SyncSynchroRemain();
		}
		// 同期
		void SyncSynchroRemain()
		{
			if (this.CanUpdate) this.View.SetSynchroRemain(this.Model.SynchroRemain, this.Model.SynchroRemainFormat);
		}

		#endregion

		#region 強化スロット
		void HandlePowerupSlotChange(object sender, EventArgs e)
		{
			this.SyncPowerupSlot();
		}

		public void HandlePowerupSlotFormatChange(object sender, EventArgs e)
		{
			this.SyncPowerupSlot();
		}

		void SyncPowerupSlot()
		{
			if (this.CanUpdate) this.View.SetPowerupSlot(this.Model.PowerupSlot, this.Model.PowerupSlotNum, this.Model.PowerupSlotFormat);
		}

		/// <summary>
		/// スロット枠の設定
		/// </summary>
		void SetSlotCapacity(int capacity, int slotCount)
		{
			if(this.SlotList != null)
			{
				this.SlotList.SetupCapacity(capacity, slotCount);
			}
		}
		/// <summary>
		/// スロットキャラの追加
		/// </summary>
		void AddSlotChara(CharaInfo info,bool isRequest)
		{
			if(this.SlotList != null)
			{
				this.SlotList.AddChara(info);
			}
		}

		/// <summary>
		/// スロットキャラ削除
		/// </summary>
		void RemoveSlotChara(CharaInfo info, bool isRequest)
		{
			if (this.SlotList != null)
			{
				this.SlotList.RemoveChara(info);
			}
		}

		/// <summary>
		/// スロットリストクリア
		/// </summary>
		void ClearSlotList()
		{
			if (this.SlotList != null)
			{
				this.SlotList.ClearChara();
			}
		}

		/// <summary>
		/// スロットリストを同期
		/// </summary>
		void SyncSlotCharaList()
		{
		}

		/// <summary>
		///  強化スロットの更新
		/// </summary>
		/// <param name="slotList"></param>
		void UpdateSlotList(List<CharaInfo> slotList)
		{
			if (slotList == null) return;
			if (this.SlotList == null) return;

			// スロットリストのキャラ情報を更新する
			this.ClearSlotList();
			slotList.ForEach(t => { this.AddSlotChara(t, false); });
		}

		void HandleSlotListItemChangeEvent(GUICharaItem obj) { }
		void HandleSlotListItemClickEvent(GUICharaItem obj){}
		void HandleSlotListItemLongPressEvent(GUICharaItem obj) { }
		void HandleSlotListUpdateItemsEvent() { }
		void HandleSlotListAllClearClickEvent() { }
		#endregion

		#endregion

		#region ステータス情報の更新

		#region 生命力

		#region 合計生命力
		// 合計生命力の更新
		public void HandleTotalHitPointChange(object sender,EventArgs e)
		{
			this.SyncTotalHitPoint();
		}
		// 合計生命力のフォーマット更新
		public void HandleTotalHitPointFormatChange(object sender, EventArgs e)
		{
			this.SyncTotalHitPoint();
		}

		// 同期
		void SyncTotalHitPoint()
		{
			if(this.CanUpdate)this.View.SetTotalHitPoint(this.Model.TotalHitPoint, this.Model.TotalHitPointFormat);
		}
		#endregion

		#region 基礎生命力
		// 基礎生命力の更新
		public void HandleBaseHitPointChange(object sender, EventArgs e)
		{
			this.SyncBaseHitPoint();
		}
		// 合計生命力のフォーマット更新
		public void HandleBaseHitPointFormatChange(object sender, EventArgs e)
		{
			this.SyncBaseHitPoint();
		}

		// 同期
		void SyncBaseHitPoint()
		{
			if(this.CanUpdate)this.View.SetBaseHitPoint(this.Model.BaseHitPoint, this.Model.BaseHitPointFormat);
		}

		#endregion

		#region スロット生命力
		// スロット生命力の更新
		public void HandleSlotHitPointChange(object sender, EventArgs e)
		{
			this.SyncSlotHitPoint();
		}
		// スロット生命力のフォーマット更新
		public void HandleSlotHitPointFormatChange(object sender, EventArgs e)
		{
			this.SyncSlotHitPoint();
		}

		// 同期
		void SyncSlotHitPoint()
		{
			if (this.CanUpdate) this.View.SetSlotHitPoint(this.Model.SlotHitPoint, this.Model.SlotHitPointFormat);
		}

		#endregion

		#region シンクロ生命力
		// シンクロ生命力の更新
		public void HandleSyncHitPointChange(object sender, EventArgs e)
		{
			this.SyncSyncHitPoint();
		}
		// シンクロ生命力のフォーマット更新
		public void HandleSyncHitPointFormatChange(object sender, EventArgs e)
		{
			this.SyncSyncHitPoint();
		}

		// 同期
		void SyncSyncHitPoint()
		{
			if (this.CanUpdate) this.View.SetSyncHitPoint(this.Model.SyncHitPoint, this.Model.SyncHitPointFormat);
		}

		#endregion

		#endregion

		#region 攻撃力

		#region 合計攻撃力
		// 合計攻撃力の更新
		public void HandleTotalAttackChange(object sender, EventArgs e)
		{
			this.SyncTotalAttack();
		}
		// 合計攻撃力のフォーマット更新
		public void HandleTotalAttackFormatChange(object sender, EventArgs e)
		{
			this.SyncTotalAttack();
		}

		// 同期
		void SyncTotalAttack()
		{
			if (this.CanUpdate) this.View.SetTotalAttack(this.Model.TotalAttack, this.Model.TotalAttackFormat);
		}
		#endregion

		#region 基礎攻撃力
		// 基礎攻撃力の更新
		public void HandleBaseAttackChange(object sender, EventArgs e)
		{
			this.SyncBaseAttack();
		}
		// 合計攻撃力のフォーマット更新
		public void HandleBaseAttackFormatChange(object sender, EventArgs e)
		{
			this.SyncBaseAttack();
		}

		// 同期
		void SyncBaseAttack()
		{
			if (this.CanUpdate) this.View.SetBaseAttack(this.Model.BaseAttack, this.Model.BaseAttackFormat);
		}

		#endregion

		#region スロット攻撃力
		// スロット攻撃力の更新
		public void HandleSlotAttackChange(object sender, EventArgs e)
		{
			this.SyncSlotAttack();
		}
		// スロット攻撃力のフォーマット更新
		public void HandleSlotAttackFormatChange(object sender, EventArgs e)
		{
			this.SyncSlotAttack();
		}

		// 同期
		void SyncSlotAttack()
		{
			if (this.CanUpdate) this.View.SetSlotAttack(this.Model.SlotAttack, this.Model.SlotAttackFormat);
		}

		#endregion

		#region シンクロ攻撃力
		// シンクロ攻撃力の更新
		public void HandleSyncAttackChange(object sender, EventArgs e)
		{
			this.SyncSyncAttack();
		}
		// シンクロ攻撃力のフォーマット更新
		public void HandleSyncAttackFormatChange(object sender, EventArgs e)
		{
			this.SyncSyncAttack();
		}

		// 同期
		void SyncSyncAttack()
		{
			if (this.CanUpdate) this.View.SetSyncAttack(this.Model.SyncAttack, this.Model.SyncAttackFormat);
		}

		#endregion

		#endregion

		#region 防御力

		#region 合計防御力
		// 合計防御力の更新
		public void HandleTotalDefenseChange(object sender, EventArgs e)
		{
			this.SyncTotalDefense();
		}
		// 合計防御力のフォーマット更新
		public void HandleTotalDefenseFormatChange(object sender, EventArgs e)
		{
			this.SyncTotalDefense();
		}

		// 同期
		void SyncTotalDefense()
		{
			if (this.CanUpdate) this.View.SetTotalDefense(this.Model.TotalDefense, this.Model.TotalDefenseFormat);
		}
		#endregion

		#region 基礎防御力
		// 基礎防御力の更新
		public void HandleBaseDefenseChange(object sender, EventArgs e)
		{
			this.SyncBaseDefense();
		}
		// 合計防御力のフォーマット更新
		public void HandleBaseDefenseFormatChange(object sender, EventArgs e)
		{
			this.SyncBaseDefense();
		}

		// 同期
		void SyncBaseDefense()
		{
			if (this.CanUpdate) this.View.SetBaseDefense(this.Model.BaseDefense, this.Model.BaseDefenseFormat);
		}

		#endregion

		#region スロット防御力
		// スロット防御力の更新
		public void HandleSlotDefenseChange(object sender, EventArgs e)
		{
			this.SyncSlotDefense();
		}
		// スロット防御力のフォーマット更新
		public void HandleSlotDefenseFormatChange(object sender, EventArgs e)
		{
			this.SyncSlotDefense();
		}

		// 同期
		void SyncSlotDefense()
		{
			if (this.CanUpdate) this.View.SetSlotDefense(this.Model.SlotDefense, this.Model.SlotDefenseFormat);
		}

		#endregion

		#region シンクロ防御力
		// シンクロ防御力の更新
		public void HandleSyncDefenseChange(object sender, EventArgs e)
		{
			this.SyncSyncDefense();
		}
		// シンクロ防御力のフォーマット更新
		public void HandleSyncDefenseFormatChange(object sender, EventArgs e)
		{
			this.SyncSyncDefense();
		}

		// 同期
		void SyncSyncDefense()
		{
			if (this.CanUpdate) this.View.SetSyncDefense(this.Model.SyncDefense, this.Model.SyncDefenseFormat);
		}

		#endregion

		#endregion

		#region 特殊能力

		#region 合計特殊能力
		// 合計特殊能力の更新
		public void HandleTotalExtraChange(object sender, EventArgs e)
		{
			this.SyncTotalExtra();
		}
		// 合計特殊能力のフォーマット更新
		public void HandleTotalExtraFormatChange(object sender, EventArgs e)
		{
			this.SyncTotalExtra();
		}

		// 同期
		void SyncTotalExtra()
		{
			if (this.CanUpdate) this.View.SetTotalExtra(this.Model.TotalExtra, this.Model.TotalExtraFormat);
		}
		#endregion

		#region 基礎特殊能力
		// 基礎特殊能力の更新
		public void HandleBaseExtraChange(object sender, EventArgs e)
		{
			this.SyncBaseExtra();
		}
		// 合計特殊能力のフォーマット更新
		public void HandleBaseExtraFormatChange(object sender, EventArgs e)
		{
			this.SyncBaseExtra();
		}

		// 同期
		void SyncBaseExtra()
		{
			if (this.CanUpdate) this.View.SetBaseExtra(this.Model.BaseExtra, this.Model.BaseExtraFormat);
		}

		#endregion

		#region スロット特殊能力
		// スロット特殊能力の更新
		public void HandleSlotExtraChange(object sender, EventArgs e)
		{
			this.SyncSlotExtra();
		}
		// スロット特殊能力のフォーマット更新
		public void HandleSlotExtraFormatChange(object sender, EventArgs e)
		{
			this.SyncSlotExtra();
		}

		// 同期
		void SyncSlotExtra()
		{
			if (this.CanUpdate) this.View.SetSlotExtra(this.Model.SlotExtra, this.Model.SlotExtraFormat);
		}

		#endregion

		#region シンクロ特殊能力
		// シンクロ特殊能力の更新
		public void HandleSyncExtraChange(object sender, EventArgs e)
		{
			this.SyncSyncExtra();
		}
		// シンクロ特殊能力のフォーマット更新
		public void HandleSyncExtraFormatChange(object sender, EventArgs e)
		{
			this.SyncSyncExtra();
		}

		// 同期
		void SyncSyncExtra()
		{
			if (this.CanUpdate) this.View.SetSyncExtra(this.Model.SyncExtra, this.Model.SyncExtraFormat);
		}

		#endregion

		#endregion

		#endregion

		#region キャラアイテム
		// キャラアイテム更新
		void HandleCharaItemChangeEvent(GUICharaItem obj) { this.SyncCharaItem(); }
		void HandleCharaItemClickEvent(GUICharaItem obj) { }
		void HandleCharaItemLongPressEvent(GUICharaItem obj) { }

		/// <summary>
		/// キャラアイコンの設定
		/// </summary>
		void SetCharaItem(GUICharaItem item, CharaInfo info)
		{
			if (item == null) return;

			var state = XUI.CharaItem.Controller.ItemStateType.Icon;
			if (info == null || info.IsEmpty)
			{
				state = XUI.CharaItem.Controller.ItemStateType.Empty;
			}

			item.SetState(state, info);
		}

		/// <summary>
		/// キャラアイテムをセットする
		/// </summary>
		void SetChara(CharaInfo info)
		{
			if (this.CharaItem == null) return;
			this.SetCharaItem(this.CharaItem, info);
		}

		/// <summary>
		/// キャラデータを同期
		/// </summary>
		void SyncCharaItem()
		{
			if (this.CanUpdate)
			{
				// キャラデータ更新
				string name = string.Empty;
				int dCost = 0; 
				float reTime = 0f;
				AvatarType AvatarType = 0;
                int skinId = 0;
				int rank = 0, lv = 0, exp = 0, sync = 0,slotNum = 0;
				int hp = 0, atk = 0, def = 0, extra = 0;
				int pHP = 0, pAtk = 0, pDef = 0, pExtra = 0;
				int sHP = 0, sAtk = 0, sDef = 0, sExtra = 0;
				bool isLock = false;

				var info = this.CharaItemInfo;
				if(info != null)
				{
					name		= info.Name;
					dCost		= info.DeckCost;
					reTime		= info.RebuildTime; 
					AvatarType	= info.AvatarType;
					rank		= info.Rank;
					lv			= info.PowerupLevel;
					exp			= info.PowerupExp;
					sync		= info.SynchroRemain;
					slotNum		= info.PowerupSlotNum;
					hp			= info.HitPoint;
					atk			= info.Attack;
					def			= info.Defense;
					extra		= info.Extra;
					pHP			= info.PowerupHitPoint;
					pAtk		= info.PowerupAttack;
					pDef		= info.PowerupDefense;
					pExtra		= info.PowerupExtra;
					sHP			= info.SynchroHitPoint;
					sAtk		= info.SynchroAttack;
					sDef		= info.SynchroDefense;
					sExtra		= info.SynchroExtra;
					isLock		= info.IsLock;
                    skinId      = info.SkinId;
                }

				// ベースキャラのデータ更新
				this.Model.CharaName = name;			// 名前
				this.Model.CharaCost = dCost;			// コスト
				this.Model.RebuidTime =reTime;			// リビルト
				this.Model.IsLock = isLock;             // ロック状態
				this.Model.AvatarType = AvatarType;     // 立ち絵情報
                this.Model.SkinId = skinId;             // Skin id
				// 強化関連
				this.Model.Rank = rank;					// ランク
				this.Model.PowerupLevel = lv;			// レベル
				this.Model.PowerupExp = exp;			// 所持経験値
				this.Model.SynchroRemain = sync;			// 所持経験値
				this.Model.PowerupSlotNum = slotNum;	// 強化スロット開放数
				//　基礎・合計
				this.Model.TotalHitPoint = hp;			// 合計生命力
				this.Model.TotalAttack = atk;			// 合計攻撃力
				this.Model.TotalDefense =def;			// 合計防御力
				this.Model.TotalExtra =extra;           // 合計特殊能力
				this.Model.BaseHitPoint = pHP;			// 基礎生命力
				this.Model.BaseAttack = pAtk;			// 基礎攻撃力
				this.Model.BaseDefense = pDef;			// 基礎防御力
				this.Model.BaseExtra = pExtra;			// 基礎合計特殊能力
				// シンクロ
				this.Model.SyncHitPoint = sHP;			// シンクロ生命力
				this.Model.SyncAttack = sAtk;			// シンクロ合計攻撃力
				this.Model.SyncDefense = sDef;			// シンクロ合計防御力
				this.Model.SyncExtra = sExtra;          // シンクロ合計特殊能力

				// スロット
				this.SetSlotCapacity( MasterDataCommonSetting.Fusion.MaxPowerupSlotNum, Model.PowerupSlotNum );

				// EXPの同期処理を行う
				// 累積経験値が0の場合は更新がかからないのでここで更新
				this.SyncExp();

			}
		}

		/// <summary>
		/// キャラを初期化する
		/// </summary>
		void ClearCharaItem()
		{
			this.SetChara(null);
		}

		#endregion

		#region キャラボード更新
		private void HandleAvatarTypeChange(object sender, EventArgs e)
		{
			this.SyncAvatarType();
		}
		private void SyncAvatarType()
		{
			if (!this.CanUpdate || this.CharaBoard == null) { return; }

			if (this.Model.AvatarType != AvatarType.None)
			{
				this.CharaBoard.GetBoard(this.Model.AvatarType, this.Model.SkinId, false,
					(resource) =>
					{
						this.CreateBoard(resource, this.Model.AvatarType);
					});
			}
		}
		/// <summary>
		/// 立ち絵生成
		/// </summary>
		private void CreateBoard(GameObject resource, AvatarType avatarType)
		{
			if (!this.CanUpdate) { return; }

			// リソース読み込み完了
			if (resource == null) { return; }
			// インスタンス化
			var go = SafeObject.Instantiate(resource) as GameObject;
			if (go == null) { return; }

			// 読み込み中に別のキャラに変更していたら破棄する
			if (this.Model.AvatarType != avatarType)
			{
				UnityEngine.Object.Destroy(go);
				return;
			}

			// 名前設定
			go.name = resource.name;
			// 親子付け
			var t = go.transform;
			this.View.SetBoardRoot(t);
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			this.View.ReplayBoard(true);
		}
		#endregion
	}

}