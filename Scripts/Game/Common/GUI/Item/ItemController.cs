/// <summary>
/// アイテム制御
/// 
/// 2016/03/16
/// </summary>
using UnityEngine;
using System;

namespace XUI.Item
{
	#region 定数
	/// <summary>
	/// アイテムの状態
	/// </summary>
	public enum ItemStateType
	{
		Empty,		// 空表示
		FillEmpty,	// 空表記なしの空状態
		ItemIcon,	// アイテムアイコン表示
		Icon,		// アイコンのみ表示
		Loading,	// 読み込み
	}

	/// <summary>
	/// 無効状態
	/// </summary>
	public enum DisableType
	{
		None,					// 設定なし
		Base,					// ベース
		Bait,					// 餌アイテム
		Lock,					// ロック
		Select,					// 選択
	}
	#endregion

	/// <summary>
	/// アイテム制御インターフェイス
	/// </summary>
	public interface IController
	{
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate { get; }

		/// <summary>
		/// データ初期化
		/// </summary>
		void Setup();

		#region アイテム状態
		/// <summary>
		/// アイテムの状態をセットする
		/// </summary>
		void SetItemState(ItemStateType state);

		/// <summary>
		/// 状態を取得する
		/// </summary>
		ItemStateType GetItemState();
		#endregion

		#region アイテム情報
		/// <summary>
		/// アイテム情報をセットする
		/// </summary>
		void SetItemInfo(ItemInfo itemInfo);

		/// <summary>
		/// アイテム情報を取得する
		/// </summary>
		ItemInfo GetItemInfo();
		#endregion

		#region 無効状態
		/// <summary>
		/// 無効状態をセットする
		/// </summary>
		void SetDisableState(DisableType state);

		/// <summary>
		/// 餌状態をセットする
		/// </summary>
		void SetBaitState(int baitIndex);

		/// <summary>
		/// 無効状態を取得する
		/// </summary>
		DisableType GetDisableState();
		#endregion

		#region 選択
		/// <summary>
		/// 選択設定
		/// </summary>
		void SetSelect(bool isSelect);

		/// <summary>
		/// /選択フラグを取得
		/// </summary>
		bool GetSelect();
		#endregion
	}

	/// <summary>
	/// アイテム制御
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
		/// アイテムアイコン
		/// </summary>
		private ItemIcon ItemIcon { get; set; }

		/// <summary>
		/// アイテムの状態
		/// </summary>
		private ItemStateType _state = ItemStateType.Empty;
		private ItemStateType State { get { return _state; } set { _state = value; } }

		/// <summary>
		/// 非選択状態
		/// </summary>
		private DisableType _disableState = DisableType.None;
		public DisableType DisableState { get { return _disableState; } private set { _disableState = value; } }

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
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view, ItemIcon itemIcon)
		{
			if (model == null || view == null) { return; }

			// ビュー設定
			this._view = view;

			// モデル設定
			this._model = model;
			this.Model.OnSelectChange += this.HandleSelectChange;

			// アイテムアイコン設定
			this.ItemIcon = itemIcon;

			// データ初期化
			this.Setup();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if (this.CanUpdate)
			{
				this.Model.Dispose();
			}
		}

		/// <summary>
		/// データ初期化
		/// </summary>
		public void Setup()
		{
			this.SetItemState(ItemStateType.FillEmpty);
			this.SetItemInfo(null);
			this.SetDisableState(DisableType.None);
			this.SetSelect(false);
		}
		#endregion

		#region アイテム状態
		/// <summary>
		/// アイテムの状態をセットする
		/// </summary>
		public void SetItemState(ItemStateType state)
		{
			switch(state)
			{
				case ItemStateType.Empty:
					// 空アイテムに切り替え
					this.SetEmptyState();
					break;
				case ItemStateType.FillEmpty:
					// 空表記なしの空アイテムに切り替え
					this.SetFillEmptyState();
					break;
				case ItemStateType.ItemIcon:
				case ItemStateType.Icon:
				case ItemStateType.Loading:
					// ロード状態に切替
					this.SetLoadState(state);
					break;
			}
		}

		/// <summary>
		/// 読み込みアイコン状態をセット
		/// </summary>
		private void SetLoadState(ItemStateType nextState)
		{
			if (!this.CanUpdate) { return; }
			this.State = ItemStateType.Loading;

			this.View.SetItemIconActive(false);
			this.View.SetEmptyActive(false);
			// アイコン読み込み
			this.View.SetLoadingActive(true);
			if (this.Model.ItemInfo != null && (nextState == ItemStateType.Icon || nextState == ItemStateType.ItemIcon))
			{
				this.ItemIcon.GetItemIcon(this.Model.ItemInfo.ItemMasterID, false,
					(atlas, spriteName) =>
					{
						if (atlas != null && !string.IsNullOrEmpty(spriteName))
						{
							// アイコンセット
							this.View.SetItemIcon(atlas, spriteName);
							// 読み込みが終了したので状態を更新
							if(nextState == ItemStateType.ItemIcon)
							{
								this.SetItemIconState();
							}
							else if(nextState == ItemStateType.Icon)
							{
								this.SetIconState();
							}
						}
					}
				);
			}
		}

		/// <summary>
		/// 空アイコン状態をセット
		/// </summary>
		private void SetEmptyState()
		{
			if (!this.CanUpdate) { return; }
			this.State = ItemStateType.Empty;

			this.View.SetItemIconActive(false);
			this.View.SetLoadingActive(false);
			this.View.SetEmptyActive(true);
		}

		/// <summary>
		/// 空表記なしの空状態をセット
		/// </summary>
		private void SetFillEmptyState()
		{
			if (!this.CanUpdate) { return; }
			this.State = ItemStateType.FillEmpty;

			this.View.SetItemIconActive(false);
			this.View.SetLoadingActive(false);
			this.View.SetEmptyActive(false);
		}

		/// <summary>
		/// アイテムアイコン状態をセット
		/// </summary>
		private void SetItemIconState()
		{
			if (!this.CanUpdate) { return; }
			this.State = ItemStateType.ItemIcon;

			this.View.SetItemIconActive(true);
			this.View.SetLoadingActive(false);
			this.View.SetEmptyActive(false);
			ItemInfo info = this.Model.ItemInfo;
			if(info != null)
			{
				this.View.SetIconInfo(true);
				this.View.SetStack(info.Stack.ToString());
				this.View.SetNew(info.NewFlag);
			}
		}

		/// <summary>
		/// アイコンのみの状態をセット
		/// </summary>
		private void SetIconState()
		{
			if (!this.CanUpdate) { return; }
			this.State = ItemStateType.Icon;

			this.View.SetItemIconActive(true);
			this.View.SetIconInfo(false);
			this.View.SetLoadingActive(false);
			this.View.SetEmptyActive(false);
		}

		/// <summary>
		/// 状態を取得する
		/// </summary>
		public ItemStateType GetItemState()
		{
			return this.State;
		}
		#endregion

		#region アイテム情報
		/// <summary>
		/// アイテム情報をセットする
		/// </summary>
		public void SetItemInfo(ItemInfo itemInfo)
		{
			if (!this.CanUpdate) { return; }
			this.Model.ItemInfo = itemInfo;
		}

		/// <summary>
		/// アイテム情報の取得
		/// </summary>
		public ItemInfo GetItemInfo()
		{
			if(!this.CanUpdate)
			{
				return null;
			}
			return this.Model.ItemInfo;
		}
		#endregion

		#region 無効状態
		/// <summary>
		/// 無効状態をセットする
		/// </summary>
		public void SetDisableState(DisableType state)
		{
			if (!this.CanUpdate) { return; }

			switch(state)
			{
				case DisableType.None:
				{
					this.View.SetDisableActive(false);
					break;
				}
				case DisableType.Base:
				{
					this.View.SetDisableActive(true);
					this.View.SetDisableTextActive(false, string.Empty);
					this.View.SetSelectNumberActive(true, MasterData.GetText(TextType.TX245_CharaList_Base));
					break;
				}
				case DisableType.Bait:
				{
					this.View.SetDisableActive(true);
					this.View.SetDisableTextActive(false, string.Empty);
					this.SetBaitView(true, -1);
					break;
				}
				case DisableType.Lock:
				{
					this.View.SetDisableActive(true);
					this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX250_CharaList_Lock));
					this.View.SetSelectNumberActive(false, string.Empty);
					break;
				}
				case DisableType.Select:
				{
					this.View.SetDisableActive(true);
					this.View.SetDisableTextActive(false, string.Empty);
					this.View.SetSelectNumberActive(true, MasterData.GetText(TextType.TX314_CharaList_Select));
					break;
				}
			}

			// 状態更新
			this.DisableState = state;
		}

		/// <summary>
		/// 餌状態をセットする
		/// </summary>
		public void SetBaitState(int baitIndex)
		{
			// 状態変更
			this.SetDisableState(DisableType.Bait);

			// 表示設定
			this.SetBaitView(true, baitIndex);
		}

		/// <summary>
		/// 餌状態の表示設定
		/// </summary>
		private void SetBaitView(bool isActive, int baitIndex)
		{
			if (!this.CanUpdate) { return; }

			string selectText = string.Empty;
			if(isActive)
			{
				if(baitIndex > -1)
				{
					// 番号のテキストを表示
					selectText = (baitIndex + 1).ToString();
				}
			}
			else
			{
				selectText = string.Empty;
			}

			this.View.SetSelectNumberActive(isActive, selectText);
		}

		/// <summary>
		/// 無効状態を取得する
		/// </summary>
		public DisableType GetDisableState()
		{
			if (!this.CanUpdate) { return DisableType.None; }
			return this.DisableState;
		}
		#endregion

		#region 選択
		/// <summary>
		/// 選択設定
		/// </summary>
		public void SetSelect(bool isSelect)
		{
			if (!this.CanUpdate) { return; }
			this.Model.IsSelect = isSelect;
		}

		/// <summary>
		/// /選択フラグを取得
		/// </summary>
		public bool GetSelect()
		{
			if (!this.CanUpdate) { return false; }
			return this.Model.IsSelect;
		}

		/// <summary>
		/// 選択フラグに変更があった時に呼び出される
		/// </summary>
		private void HandleSelectChange(object sender, EventArgs e)
		{
			this.SyncSelect();
		}

		/// <summary>
		/// 選択同期
		/// </summary>
		private void SyncSelect()
		{
			if (!this.CanUpdate) { return; }
			this.View.SetSelectFrameActive(this.Model.IsSelect);
		}
		#endregion
	}
}