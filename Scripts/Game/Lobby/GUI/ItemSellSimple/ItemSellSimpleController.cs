/// <summary>
/// アイテム売却制御
/// 
/// 2016/04/08
/// </summary>
using UnityEngine;
using System;
using Scm.Common.XwMaster;

namespace XUI.ItemSellSimple
{
	#region 初期化パラメータ
	/// <summary>
	/// セットアップ時に設定するパラメータ
	/// </summary>
	public struct SetupParam
	{
		public ItemInfo ItemInfo { get; set; }
		public int HaveItemCount { get; set; }
	}
	#endregion

	#region イベント引数
	/// <summary>
	/// 売却イベント引数
	/// </summary>
	public class SellItemEventArgs : EventArgs
	{
		public int ItemMasterId { get; set; }
		public int Stack { get; set; }
	}
	#endregion

	/// <summary>
	/// アイテム売却制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region 初期化
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// データ初期化
		/// </summary>
		void Setup(SetupParam param);

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

		#region 売却
		/// <summary>
		/// 売却イベント
		/// </summary>
		event EventHandler<SellItemEventArgs> OnSell;

		/// <summary>
		/// 売却結果
		/// </summary>
		void SellResult(bool result, int itemMasterId, int stack, int money, int soldPrice);
		#endregion
	}

	/// <summary>
	/// アイテム売却制御
	/// </summary>
	public class Controller : IController
	{
		#region 定数
		/// <summary>
		/// 初期売却数
		/// </summary>
		private readonly int DefaultSellCount = 1;
		#endregion

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
		/// 売却アイテム
		/// </summary>
		private readonly GUIItem _sellItem;
		private GUIItem SellItem { get { return _sellItem; } }

		/// <summary>
		/// 売却アイテム情報
		/// </summary>
		private ItemInfo SellItemInfo { get { return (this.SellItem != null ? this.SellItem.GetItemInfo() : null); } }

		/// <summary>
		/// 売却アイテムが空かどうか
		/// </summary>
		private bool IsEmptySellItem { get { return (this.SellItemInfo == null ? true : false); } }

		/// <summary>
		/// 売却アイテムのマスターID
		/// </summary>
		private int SellItemMasterId
		{
			get
			{
				int id = 0;
				var info = this.SellItemInfo;
				if (info != null) { id = info.ItemMasterID; }
				return id;
			}
		}

		/// <summary>
		/// 売却イベント
		/// </summary>
		public event EventHandler<SellItemEventArgs> OnSell = (sender, e) => { };
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view, GUIItem sellItem)
		{
			if (model == null || view == null) { return; }

			// ビュー設定
			this._view = view;
			this.View.OnClose += this.HandleClose;
			this.View.OnAddSellCount += this.HandleAddSellCount;
			this.View.OnSubSellCount += this.HandleSubSellCount;
			this.View.OnSellItemCountSliderChange += this.HandleSellItemCountSliderChange;
			this.View.OnSell += this.HandleSell;

			// モデル設定
			this._model = model;
			this.Model.OnHaveMoneyChange += this.HandleHaveMoneyChange;
			this.Model.OnHaveMoneyFormatChange += this.HandleHaveMoneyFormatChange;
			this.Model.OnItemNameChange += this.HandleItemNameChange;
			this.Model.OnHaveItemCountChange += this.HandleHaveItemCountChange;
			this.Model.OnSellItemCountChange += this.HandleSellItemCountChange;
			this.Model.OnSoldPriceChange += this.HandleSoldPriceChange;
			this.Model.OnSoldPriceFormatChange += this.HandleSoldPriceFormatChange;

			// 売却アイテム設定
			this._sellItem = sellItem;
			this.SellItem.OnItemChangeEvent += this.HandleItemChangeEvent;

			// 同期
			this.SyncItemName();
			this.SyncHaveItemCount();
			this.SyncSellItem();
			this.SyncSellItemCount();
			this.SyncSoldPrice();
			this.UpdateSellItemCountSlider(0);
			this.SyncHaveMoney();
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

			this.OnSell = null;
		}

		/// <summary>
		/// データ初期化
		/// </summary>
		public void Setup(SetupParam param)
		{
			// 売却アイテムセット
			this.SetSellItem(param.ItemInfo);
			// 所持数セット
			this.SetHaveSellItemCount(param.HaveItemCount);
			// 売却数リセット
			this.SetSellItemCount(DefaultSellCount);
			// スライダー更新
			this.UpdateSellItemCountSlider(DefaultSellCount);
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
			if (!this.CanUpdate) { return; }
			this.View.SetActive(isActive, isTweenSkip);
			GUIScreenTitle.Play(isActive, MasterData.GetText(TextType.TX349_SellItem_Title));
			GUIHelpMessage.Play(isActive, MasterData.GetText(TextType.TX350_SellItem_HelpMessage));
		}
		#endregion

		#region 売却アイテム
		/// <summary>
		/// 売却アイテムセットする
		/// </summary>
		private void SetSellItem(ItemInfo itemInfo)
		{
			if (this.SellItem == null) { return; }

			Item.ItemStateType state = Item.ItemStateType.Empty;
			ItemInfo info = null;
			if(itemInfo != null)
			{
				state = Item.ItemStateType.Icon;
				info = itemInfo.Clone();
			}

			// 状態セット
			this.SellItem.SetState(state, info);
		}

		/// <summary>
		/// 売却アイテム変更ハンドラー
		/// </summary>
		private void HandleItemChangeEvent(GUIItem item)
		{
			this.SyncSellItem();
		}

		/// <summary>
		/// 売却アイテム同期
		/// </summary>
		private void SyncSellItem()
		{
			if (!this.CanUpdate) { return; }

			ItemInfo itemInfo = this.SellItemInfo;
			// アイテム名セット
			this.SetItemName(itemInfo);
		}
		#endregion

		#region アイテム名
		/// <summary>
		/// アイテム名セット
		/// </summary>
		private void SetItemName(ItemInfo itemInfo)
		{
			if (!this.CanUpdate) { return; }

			string name = string.Empty;
			if(itemInfo != null)
			{
				name = itemInfo.Name;
			}

			// セット
			this.Model.ItemName = name;
		}

		/// <summary>
		/// アイテム名変更ハンドラー
		/// </summary>
		private void HandleItemNameChange(object sender, EventArgs e)
		{
			this.SyncItemName();
		}

		/// <summary>
		/// アイテム名同期
		/// </summary>
		private void SyncItemName()
		{
			if (!this.CanUpdate) { return; }

			// ビュー設定
			this.View.SetItemName(this.Model.ItemName);
		}
		#endregion

		#region 所持数
		/// <summary>
		/// 所持数セット
		/// </summary>
		private void SetHaveSellItemCount(int count)
		{
			if (!this.CanUpdate) { return; }
			this.Model.HaveItemCount = count;
		}

		/// <summary>
		/// 所持数変更ハンドラー
		/// </summary>
		private void HandleHaveItemCountChange(object sender, EventArgs e)
		{
			if (!this.CanUpdate) { return; }
			this.SyncHaveItemCount();
		}

		/// <summary>
		/// 所持数同期
		/// </summary>
		private void SyncHaveItemCount()
		{
			this.View.SetHaveItemCountLabel(this.Model.HaveItemCount.ToString());
		}
		#endregion

		#region 売却数
		/// <summary>
		/// 売却数セット
		/// </summary>
		private void SetSellItemCount(int count)
		{
			if (!this.CanUpdate) { return; }
			this.Model.SellItemCount = count;
		}

		/// <summary>
		/// 売却数変更ハンドラー
		/// </summary>
		private void HandleSellItemCountChange(object sender, EventArgs e)
		{
			this.SyncSellItemCount();
		}

		/// <summary>
		/// 売却数同期
		/// </summary>
		private void SyncSellItemCount()
		{
			if (!this.CanUpdate) { return; }

			// ビュー側セット
			this.View.SetSellItemCount(this.Model.SellItemCount.ToString());

			// 売却額セット
			int soldPrice = 0;

			if (!this.IsEmptySellItem)
			{
				ItemMasterData masterData;
				if (MasterData.TryGetItem(this.SellItemMasterId, out masterData))
				{
					soldPrice = masterData.SellGameMoney * this.Model.SellItemCount;
				}
				else
				{
					// 取得失敗
					string msg = string.Format("ItemMasterData NotFound. ID = {0}", this.SellItemMasterId);
					BugReportController.SaveLogFile(msg);
					UnityEngine.Debug.LogWarning(msg);
				}
			}
			this.SetSoldPrice(soldPrice);

			// 売却ボタン有効設定更新
			this.UpdateSellButtonEnable();
		}
		#endregion

		#region 売却数増減
		/// <summary>
		/// 売却数増加ボタンイベントハンドラー
		/// </summary>
		private void HandleAddSellCount(object sender, EventArgs e)
		{
			// 売却数増加
			this.AddSellItemCount(1);
		}

		/// <summary>
		/// 売却数減少ボタンイベントハンドラー
		/// </summary>
		private void HandleSubSellCount(object sender, EventArgs e)
		{
			// 売却数減少
			this.SubSellItemCount(1);
		}

		/// <summary>
		/// 売却数増加
		/// </summary>
		private void AddSellItemCount(int addCount)
		{
			if (!this.CanUpdate) { return; }
			
			// 売却数セット
			int value = this.Model.SellItemCount + addCount;
			int count = value > this.Model.HaveItemCount ? 0 : value;
			
			// スライダー更新
			this.UpdateSellItemCountSlider(count);
		}
		/// <summary>
		/// 売却数減少
		/// </summary>
		private void SubSellItemCount(int subCount)
		{
			if (!this.CanUpdate) { return; }
			
			// 売却数セット
			int value = this.Model.SellItemCount - subCount;
			int count = value < 0 ? this.Model.HaveItemCount : value;
			
			// スライダー更新
			this.UpdateSellItemCountSlider(count);
		}
		#endregion

		#region 売却数スライダー
		/// <summary>
		/// 売却数のスライダーを更新する
		/// </summary>
		private void UpdateSellItemCountSlider(int itemCount)
		{
			if (!this.CanUpdate) { return; }

			// 売却数からスライダー値を求める
			float value = 0;
			if (this.Model.HaveItemCount > 0)
			{
				value = (float)itemCount / this.Model.HaveItemCount;
			}

			// スライダーセット
			this.View.SetSellItemCountSliderValue(value);
		}

		/// <summary>
		/// 売却数スライダー変化イベントハンドラー
		/// </summary>
		private void HandleSellItemCountSliderChange(object sender, SellItemCountSliderChangeEventArgs e)
		{
			if (!this.CanUpdate) { return; }

			// 売却数セット
			int count = Mathf.RoundToInt(this.Model.HaveItemCount * e.Value);
			this.SetSellItemCount(count);
		}
		#endregion

		#region 売却額
		/// <summary>
		/// 売却額セット
		/// </summary>
		private void SetSoldPrice(int soldPrice)
		{
			if (!this.CanUpdate) { return; }
			this.Model.SoldPrice = soldPrice;
		}

		/// <summary>
		/// 売却額変更ハンドラー
		/// </summary>
		private void HandleSoldPriceChange(object sender, EventArgs e)
		{
			this.SyncSoldPrice();
		}
		/// <summary>
		/// 売額数フォーマット変更ハンドラー
		/// </summary>
		private void HandleSoldPriceFormatChange(object sender, EventArgs e)
		{
			this.SyncSoldPrice();
		}

		/// <summary>
		/// 売却額同期
		/// </summary>
		private void SyncSoldPrice()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetSoldPrice(this.Model.SoldPrice.ToString());
			// 所持金同期
			SyncHaveMoney();
		}
		#endregion

		#region 売却
		/// <summary>
		/// 売却ボタンイベントハンドラー
		/// </summary>
		private void HandleSell(object sender, EventArgs e)
		{
			this.Sell();
		}

		/// <summary>
		/// 売却処理
		/// </summary>
		private void Sell()
		{
			if (!this.CanUpdate) { return; }

			// 売却するアイテムが設定されているかチェック
			if (this.Model.SellItemCount <= 0 || this.SellItemMasterId <= 0)
			{
				// 売却不能状態
				
				// 売却ボタン有効設定更新
				this.UpdateSellButtonEnable();
				return;
			}

			// 売却するかどうか確認
			string msg = MasterData.GetText(TextType.TX348_SellItem_SellCheck,  new string[]{ this.Model.ItemName, this.Model.SellItemCount.ToString()});
			GUIMessageWindow.SetModeYesNo(msg, true, this.CheckHaveMoney, null);
		}

		/// <summary>
		/// 所持金チェック処理
		/// </summary>
		private void CheckHaveMoney()
		{
			if (!this.CanUpdate) { return; }

			// 所持金取得
			int haveMoney = this.Model.HaveMoney;

			if (haveMoney >= MasterDataCommonSetting.Player.PlayerMaxGameMoney)
			{
				// 所持金が上限に達しているので確認メッセージを表示
				GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX347_SellItem_HaveMoneyOver), true, this.SellExecute, null);
			}
			else
			{
				// 売却実行
				this.SellExecute();
			}
		}

		/// <summary>
		/// 売却実行
		/// </summary>
		private void SellExecute()
		{
			// 通知
			var eventArgs = new SellItemEventArgs();
			eventArgs.ItemMasterId = this.SellItemMasterId;
			eventArgs.Stack = this.Model.SellItemCount;

			this.OnSell(this, eventArgs);
		}

		/// <summary>
		/// 売却結果
		/// </summary>
		public void SellResult(bool result, int itemMasterId, int stack, int money, int soldPrice)
		{
			// 売却したら閉じる
			GUIController.SingleClose();
		}

		/// <summary>
		/// 売却ボタンの有効設定更新
		/// </summary>
		private void UpdateSellButtonEnable()
		{
			if (!this.CanUpdate) { return; }

			bool isEnable = false;
			if(this.Model.SellItemCount > 0)
			{
				isEnable = true;
			}

			this.View.SetSellButtonEnable(isEnable);
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
			
			// 売却試算を求める
			int gameMoneyMax = MasterDataCommonSetting.Player.PlayerMaxGameMoney;
			int haveMoneyCalc = Math.Min(this.Model.HaveMoney + this.Model.SoldPrice, gameMoneyMax);
			this.View.SetHaveMoney(haveMoneyCalc, this.Model.HaveMoneyFormat);
		}
		#endregion

		#region ホーム、閉じるボタンイベント
		/// <summary>
		/// 閉じる
		/// </summary>
		private void HandleClose(object sender, CloseClickedEventArgs e)
		{
			GUIController.SingleClose();
		}
		#endregion
	}
}
