/// <summary>
/// 選択アイテムリスト制御
/// 
/// 2016/03/24
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI.ItemList
{
	/// <summary>
	/// 選択アイテムリスト制御インターフェイス
	/// </summary>
	public interface ISelectListController
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

		#region Box総数設定
		/// <summary>
		/// Box総数設定
		/// </summary>
		void SetCapacity(int capacity);
		#endregion

		#region アイテム情報リスト
		/// <summary>
		/// 現ページ内のアイテムリストを取得する
		/// </summary>
		List<GUIItem> GetNowPageList();

		/// <summary>
		/// 追加されているアイテムリストを取得する
		/// </summary>
		List<GUIItem> GetItemList();

		/// <summary>
		/// 追加されているアイテム情報リストを取得する
		/// </summary>
		List<ItemInfo> GetItemInfoList();
		#endregion

		#region アイテム情報リスト追加
		/// <summary>
		/// アイテム情報追加
		/// </summary>
		bool AddItem(ItemInfo itemInfo);
		#endregion

		#region アイテム情報リスト削除
		/// <summary>
		/// アイテム情報削除
		/// </summary>
		bool RemoveItem(ItemInfo itemInfo);
		#endregion

		#region アイテム情報リストクリア
		/// <summary>
		/// アイテム情報をクリアする
		/// </summary>
		bool ClearItem();
		#endregion

		#region アイテムイベント
		/// <summary>
		/// 登録されているキャラアイテムが押された時のイベント通知用
		/// </summary>
		event EventHandler<SelectListController.ItemClickEventArgs> OnItemClickEvent;

		/// <summary>
		/// 登録されているキャラアイテムに変更があった時のイベント通知用
		/// </summary>
		event EventHandler<SelectListController.ItemChangeEventArgs> OnItemChangeEvent;

		/// <summary>
		/// 全てのアイテムが更新された時のイベント通知
		/// </summary>
		event EventHandler OnUpdateItemsEvent;
		#endregion

		#region スクロール
		/// <summary>
		/// スクロールの更新
		/// </summary>
		void UpdateScroll();

		/// <summary>
		/// テーブル整形
		/// </summary>
		void Reposition();
		#endregion
	}

	/// <summary>
	/// 選択アイテムリスト制御
	/// </summary>
	public class SelectListController : ISelectListController
	{
		#region フィールド&プロパティ
		/// <summary>
		/// 共通モデル
		/// </summary>
		private readonly IModel _model;
		private IModel Model { get { return _model; } }

		/// <summary>
		/// 共通ビュー
		/// </summary>
		private readonly IView _view;
		private IView View { get { return _view; } }

		/// <summary>
		/// 登録されているキャラアイテムが押された時のイベント通知用
		/// </summary>
		public event EventHandler<ItemClickEventArgs> OnItemClickEvent = (sender, e) => { };

		/// <summary>
		/// 登録されているキャラアイテムに変更があった時のイベント通知用
		/// </summary>
		public event EventHandler<ItemChangeEventArgs> OnItemChangeEvent = (sender, e) => { };

		/// <summary>
		/// 全てのアイテムが更新された時のイベント通知
		/// </summary>
		public event EventHandler OnUpdateItemsEvent = (sender, e) => { };

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
		/// スクロール有効かどうか
		/// パネル内に収めるアイテム数以上アイテムが存在していたらスクロールを行うようにする
		/// </summary>
		private bool IsScroll
		{
			get
			{
				if (!this.CanUpdate) { return false; }
				return (this.Model.ItemScrollView.ItemMax > this.Model.ItemScrollView.InPanelItemMax) ? true : false;
			}
		}
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SelectListController(IModel model, IView view)
		{
			if (model == null || view == null) { return; }

			// ビュー設定
			this._view = view;
			
			// モデル設定
			this._model = model;
			this.Model.OnCapacityChange += this.HandleCapacityChange;
			this.Model.OnAddItemInfoList += this.HandleAddItemInfoList;
			this.Model.OnRemoveItemInfoList += this.HandleRemoveItemInfoList;
			this.Model.OnItemInfoListClear += this.HandleClearItemInfoList;

			// ページ付スクロールビューを初期化
			this.Model.ItemScrollView.Create(this.View.PageScrollViewAttach, null);
			// スクロール設定
			if(this.IsScroll)
			{
				foreach(var item in this.Model.GetItemList())
				{
					item.SetDragScrollEnable(true);
				}
			}

			// データ初期化
			this.Setup();
		}

		/// <summary>
		/// データ初期化
		/// </summary>
		public void Setup()
		{
			if (!this.CanUpdate) { return; }
			this.ClearItem();
			this.SetCapacity(0);
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

			this.OnItemClickEvent = null;
			this.OnItemChangeEvent = null;
			this.OnUpdateItemsEvent = null;
		}
		#endregion

		#region Box総数設定
		/// <summary>
		/// Box総数設定
		/// </summary>
		public void SetCapacity(int capacity)
		{
			if (!this.CanUpdate) { return; }
			this.Model.Capacity = capacity;
		}

		/// <summary>
		/// Box総数変更ハンドラー
		/// </summary>
		private void HandleCapacityChange(object sender, EventArgs e)
		{
			this.SyncCapacity();
		}
		/// <summary>
		/// アイテム総数同期
		/// </summary>
		private void SyncCapacity()
		{
			if (!this.CanUpdate) { return; }
			// ページスクロールビューの総数セット
			this.Model.ItemScrollView.Setup(this.Model.Capacity, 0);
			// テーブル整形
			this.Reposition();
		}
		#endregion

		#region アイテム情報リスト
		/// <summary>
		/// アイテム情報リスト同期
		/// </summary>
		private void SyncItemInfoList()
		{
			this.UpdateItems();
		}
		
		/// <summary>
		/// 現ページ内のアイテムリストを取得する
		/// </summary>
		public List<GUIItem> GetNowPageList()
		{
			if (!this.CanUpdate) { return new List<GUIItem>(); }
			return this.Model.GetNowPageItemList();
		}

		/// <summary>
		/// 追加されているアイテムリストを取得する
		/// </summary>
		public List<GUIItem> GetItemList()
		{
			List<GUIItem> itemList = new List<GUIItem>();
			if (!this.CanUpdate) { return itemList; }
			var pageItemList = this.Model.GetNowPageItemList();

			foreach(var item in pageItemList)
			{
				if (item.GetItemInfo() == null) { continue; }
				itemList.Add(item);
			}

			return itemList;
		}

		/// <summary>
		/// 追加されているアイテム情報リストを取得する
		/// </summary>
		public List<ItemInfo> GetItemInfoList()
		{
			if (!this.CanUpdate) { return new List<ItemInfo>(); }

			var infoList = new List<ItemInfo>();
			foreach(var info in this.Model.ItemInfoList)
			{
				if (info == null) { continue; }
				infoList.Add(info);
			}

			return infoList;
		}
		#endregion

		#region アイテム情報リスト追加
		/// <summary>
		/// アイテム情報追加
		/// </summary>
		public bool AddItem(ItemInfo itemInfo)
		{
			if (!this.CanUpdate || itemInfo == null) { return false; }

			// 空き枠チェック
			if (this.Model.ItemInfoList.Count >= this.Model.Capacity) { return false; }

			if(this.Model.ItemInfoListContains(itemInfo))
			{
				// すでに存在している場合は追加しない
				return false;
			}

			// 追加
			this.Model.AddItemInfoList(itemInfo);

			return true;
		}

		/// <summary>
		/// アイテム情報リスト追加ハンドラー
		/// </summary>
		private void HandleAddItemInfoList(object sender, EventArgs e)
		{
			// アイテム情報リスト同期
			this.SyncItemInfoList();
		}
		#endregion

		#region アイテム情報リスト削除
		/// <summary>
		/// アイテム情報削除
		/// </summary>
		public bool RemoveItem(ItemInfo	 itemInfo)
		{
			if (!this.CanUpdate || itemInfo == null) { return false; }

			// 削除するアイテムを取得
			ItemInfo removeInfo = this.Model.ItemInfoListFind(itemInfo);
			if(removeInfo == null)
			{
				// 削除するアイテムが存在しない
				return false;
			}

			// 削除
			return this.Model.RemoveItemInfoList(removeInfo);
		}

		/// <summary>
		/// アイテム情報リスト削除ハンドラー
		/// </summary>
		private void HandleRemoveItemInfoList(object sender, EventArgs e)
		{
			this.SyncItemInfoList();
		}
		#endregion

		#region アイテム情報クリア
		/// <summary>
		/// アイテム情報をクリアする
		/// </summary>
		public bool ClearItem()
		{
			if (!this.CanUpdate) { return false; }
			
			// アイテム情報がセットされているか検索
			bool isExecute = false;
			foreach(var info in this.Model.ItemInfoList)
			{
				if(info != null)
				{
					// ひとつでもアイテム情報が存在するならクリア処理を行う
					isExecute = true;
				}
			}

			if(isExecute)
			{
				this.Model.ClearItemInfoList();
			}

			return isExecute;
		}

		/// <summary>
		/// アイテム情報リストクリアハンドラー
		/// </summary>
		private void HandleClearItemInfoList(object sender, EventArgs e)
		{
			this.Model.ItemScrollView.Clear();
			this.SyncItemInfoList();
		}
		#endregion

		#region ページ内アイテム更新
		/// <summary>
		/// 現ページ内のアイテム更新
		/// </summary>
		private void UpdateItems()
		{
			if (!this.CanUpdate) { return; }

			// 現ページの最初のアイテムインデックスを取得
			int startIndex = this.Model.ItemScrollView.NowPageStartIndex;

			// 現在のページ内のアイテムを更新
			for (int i = 0, max = this.Model.ItemScrollView.NowPageItemMax; i < max; i++)
			{
				int indexInTotal = i + startIndex;
				if (!this.Model.ItemScrollView.IsNowPage(indexInTotal))
				{
					// 現在のページ内のアイテムではない
					continue;
				}

				// スクロールビューからアイテムを取得
				int itemIndex = this.Model.ItemScrollView.GetItemIndex(indexInTotal);
				GUIItem item = this.Model.ItemScrollView.GetItem(itemIndex);
				if (item == null) { continue; }

				// 前回分のイベントは削除しておく
				item.OnItemClickEvent -= this.HandleItemClick;
				item.OnItemChangeEvent -= this.HandleItemChange;
				// アイテムボタンイベント登録
				item.OnItemClickEvent += this.HandleItemClick;
				item.OnItemChangeEvent += this.HandleItemChange;

				// アイテムの種類を設定
				if (this.Model.ItemInfoList.Count <= indexInTotal)
				{
					// 所持キャラ数よりも所有できる枠数の方が多ければ空枠とする
					item.SetState(Item.ItemStateType.FillEmpty, null);
				}
				else
				{
					// アイテムが存在する枠
					ItemInfo ItemInfo = this.Model.ItemInfoList[indexInTotal];

					// アイテムセット
					item.SetState(Item.ItemStateType.ItemIcon, ItemInfo);
				}
			}

			// イベント通知
			this.OnUpdateItemsEvent(this, EventArgs.Empty);
		}
		#endregion

		#region スクロール
		/// <summary>
		/// スクロールの更新
		/// </summary>
		public void UpdateScroll()
		{
			if (!this.IsScroll) { return; }

			// パネル内のアイテムが全て追加されたら次のパネル内のアイテム個数分スクロールさせる
			int inPanelItemMax = this.Model.ItemScrollView.InPanelItemMax / 2;
			int endItemIndex = Math.Max(this.Model.ItemInfoList.Count - 1, 0);

			// スクロール位置を求める
			int centerIndex = 0;
			if(inPanelItemMax > 0)
			{
				centerIndex = (endItemIndex / inPanelItemMax) * inPanelItemMax;
			}

			// スクロール
			this.Model.ItemScrollView.CenterOn(centerIndex);
		}

		/// <summary>
		/// テーブル整形
		/// </summary>
		public void Reposition()
		{
			if (!this.CanUpdate) { return; }
			this.Model.ItemScrollView.Reposition();
			this.Model.ItemScrollView.ScrollReset();
			this.UpdateScroll();
		}
		#endregion

		#region アイテムイベント
		/// <summary>
		/// アイテムが押された時のハンドラー
		/// </summary>
		private void HandleItemClick(GUIItem item)
		{
			// 通知
			var eventArgs = new ItemClickEventArgs(item);
			this.OnItemClickEvent(this, eventArgs);
		}

		/// <summary>
		/// アイテムに変更があった時のハンドラー
		/// </summary>
		private void HandleItemChange(GUIItem item)
		{
			// 通知
			var eventArgs = new ItemChangeEventArgs(item);
			this.OnItemChangeEvent(this, eventArgs);
		}

		#region イベント引数
		/// <summary>
		/// 登録されているアイテムが押された時のディスパッチ
		/// </summary>
		public class ItemClickEventArgs : EventArgs
		{
			public GUIItem Item { get; private set; }
			public ItemClickEventArgs(GUIItem item)
			{
				this.Item = item;
			}
		}

		/// <summary>
		/// 登録されているアイテムに変更があった時のディスパッチ
		/// </summary>
		public class ItemChangeEventArgs : EventArgs
		{
			public GUIItem Item { get; private set; }
			public ItemChangeEventArgs(GUIItem item)
			{
				this.Item = item;
			}
		}
		#endregion
		#endregion
	}
}
