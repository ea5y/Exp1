/// <summary>
/// 選択キャラリスト制御
/// 
/// 2016/01/14
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI
{
	namespace CharaList
	{
		/// <summary>
		/// 選択キャラリスト制御インターフェイス
		/// </summary>
		public interface ISelectCharaListController
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
			/// 現ページ内から空いている枠を検索してインデックス値を取得する
			/// </summary>
			/// <returns>
			/// true = 空いている枠が存在
			/// false = 空いている枠が存在しない
			/// </returns>
			bool TryGetEmptyIndexInTotal(out int emptyIndexInTotal);

			/// <summary>
			/// 登録されているキャラアイテムが押された時のイベント通知用
			/// </summary>
			event EventHandler<ItemClickEventArgs> OnItemClickEvent;

			/// <summary>
			/// 登録されているキャラアイテムが長押しされた時のイベント通知用
			/// </summary>
			event EventHandler<ItemClickEventArgs> OnItemLongPressEvent;

			/// <summary>
			/// 登録されているキャラアイテムに変更があった時のイベント通知用
			/// </summary>
			event EventHandler<ItemChangeEventArgs> OnItemChangeEvent;

			/// <summary>
			/// 全てのアイテムが更新された時のイベント通知
			/// </summary>
			event EventHandler OnUpdateItemsEvent;

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

			/// <summary>
			/// ページ内の全アイテムのボタン有効設定
			/// </summary>
			void SetItemsButtonEnable(bool isEnable);
		}

		/// <summary>
		/// 選択キャラリスト制御
		/// </summary>
		public class SelectCharaListController : ISelectCharaListController
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
			/// 選択キャラリストビュー
			/// </summary>
			private readonly ISelectCharaListView _selectListView;
			private ISelectCharaListView SelectListView { get { return _selectListView; } }

			/// <summary>
			/// 登録されているキャラアイテムが押された時のイベント通知用
			/// </summary>
			public event EventHandler<ItemClickEventArgs> OnItemClickEvent = (sender, e) => { };

			/// <summary>
			/// 登録されているキャラアイテムが長押しされた時のイベント通知用
			/// </summary>
			public event EventHandler<ItemClickEventArgs> OnItemLongPressEvent = (sender, e) => { };

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
					if(this.SelectListView == null) return false;
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
			public SelectCharaListController(IModel model, IView view, ISelectCharaListView selectListView)
			{
				if (model == null || view == null || selectListView == null) { return; }

				// ビュー設定
				this._view = view;
				this._selectListView = selectListView;

				// モデル設定
				this._model = model;
				this.Model.OnCapacityChange += HandleCapacityChange;
				this.Model.OnAddCharaInfoChange += HandleAddOwnCharaInfoChange;
				this.Model.OnRemoveCharaInfoChange += HandleRemoveOwnCharaInfoChange;
				this.Model.OnClearCharaInfoChange += HandleClearCharaInfoChange;

				// ページ付スクロールビューを初期化
				this.Model.ItemScrollView.Create(this.View.PageScrollViewAttach, null);
				// スクロール設定
				if (this.IsScroll)
				{
					foreach (var item in this.Model.GetPageItemList())
					{
						item.SetDragScrollEnable(true);
					}
				}
				this.Reposition();
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

				this.OnItemClickEvent = null;
				this.OnItemLongPressEvent = null;
				this.OnItemChangeEvent = null;
				this.OnUpdateItemsEvent = null;
			}
			#endregion

			#region アイテムの総数設定
			/// <summary>
			/// アイテム総数が変更された時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleCapacityChange(object sender, EventArgs e)
			{
				SyncCapacity();
			}

			/// <summary>
			/// アイテム総数同期
			/// </summary>
			private void SyncCapacity()
			{
				if (!CanUpdate) { return; }
				// ページスクロールビューの総数セット
				this.Model.ItemScrollView.Setup(this.Model.Capacity, -1);
				// デーブル整形
				this.Reposition();
			}
			#endregion

			#region キャラ情報追加
			/// <summary>
			/// キャラ情報が追加された時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleAddOwnCharaInfoChange(object sender, EventArgs e)
			{
				SyncCharaInfoList();
			}
			#endregion

			#region キャラ情報削除
			/// <summary>
			/// キャラ情報が削除された時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleRemoveOwnCharaInfoChange(object sender,EventArgs e)
			{
				SyncCharaInfoList();
			}
			#endregion

			#region 空き枠
			/// <summary>
			/// 現ページ内から空いている枠を検索してインデックス値を取得する
			/// </summary>
			/// <returns></returns>
			public bool TryGetEmptyIndexInTotal(out int emptyIndexInTotal)
			{
				emptyIndexInTotal = -1;
				if (!CanUpdate) { return false; }

				// 現ページの最初のアイテムインデックスを取得
				int startIndex = this.Model.ItemScrollView.NowPageStartIndex;

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
					GUICharaItem item = this.Model.ItemScrollView.GetItem(itemIndex);
					if (item == null) { continue; }

					if(item.GetState() == CharaItem.Controller.ItemStateType.FillEmpty)
					{
						// 空いている枠
						emptyIndexInTotal = indexInTotal;
						return true;
					}
				}

				// 空いている枠がない
				return false;
			}
			#endregion

			#region キャラ情報更新
			/// <summary>
			/// キャラ情報同期
			/// </summary>
			private void SyncCharaInfoList()
			{
				UpdateItems();
			}

			/// <summary>
			/// 現ページ内に表示されているアイテムを更新する
			/// </summary>
			private void UpdateItems()
			{
				if (!CanUpdate) { return; }
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
					GUICharaItem item = this.Model.ItemScrollView.GetItem(itemIndex);
					if (item == null) { continue; }

					// 前回分のイベントは削除しておく
					item.OnItemClickEvent -= OnItemClick;
					item.OnItemLongPressEvent -= OnItemLongPress;
					item.OnItemChangeEvent -= OnItemChange;
					// アイテムボタンイベント登録
					item.OnItemClickEvent += OnItemClick;
					item.OnItemLongPressEvent += OnItemLongPress;
					item.OnItemChangeEvent += OnItemChange;

					// アイテムの種類を設定
					if (this.Model.CharaInfoList.Count <= indexInTotal)
					{
						// 所持キャラ数よりも所有できる枠数の方が多ければ空枠とする
						item.SetState(CharaItem.Controller.ItemStateType.FillEmpty, null);
						item.SetIndex(indexInTotal);
					}
					else
					{
						// キャラが存在する枠
						CharaInfo charaInfo = this.Model.CharaInfoList[indexInTotal];

						// アイテムセット
						item.SetState(CharaItem.Controller.ItemStateType.Icon, charaInfo);
						item.SetIndex(indexInTotal);
					}
				}

				// イベント通知
				this.OnUpdateItemsEvent(this, EventArgs.Empty);
			}
			#endregion

			#region クリア
			/// <summary>
			/// キャラ情報リストがクリアされた時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleClearCharaInfoChange(object sender, EventArgs e)
			{
				SyncCharaInfoList();
			}

			/// <summary>
			/// テーブルクリア
			/// </summary>
			private void ClearTable()
			{
				if (!CanUpdate) { return; }
				this.Model.ItemScrollView.Clear();
			}
			#endregion

			#region アイテムボタン有効設定
			/// <summary>
			/// ページ内の全アイテムのボタン有効設定
			/// </summary>
			public void SetItemsButtonEnable(bool isEnable)
			{
				if (!this.CanUpdate) { return; }
				foreach(var item in this.Model.GetPageItemList())
				{
					if (item == null) { continue; }
					item.SetButtonEnable(isEnable);
				}
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
				int endItemIndex = Math.Max(this.Model.CharaInfoList.Count - 1, 0);

				// スクロール位置を求める
				int centerIndex = 0;
				if (inPanelItemMax > 0)
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
			/// アイテムが押された時に呼び出される
			/// </summary>
			private void OnItemClick(GUICharaItem item)
			{
				// 通知
				var eventArgs = new ItemClickEventArgs(item);
				this.OnItemClickEvent(this, eventArgs);
			}

			/// <summary>
			/// アイテムが長押しされた時に呼び出される
			/// </summary>
			private void OnItemLongPress(GUICharaItem item)
			{
				// 通知
				var eventArgs = new ItemClickEventArgs(item);
				this.OnItemLongPressEvent(this, eventArgs);
			}

			/// <summary>
			/// アイテムに変更があった時に呼び出される
			/// </summary>
			/// <param name="item"></param>
			private void OnItemChange(GUICharaItem item)
			{
				// 通知
				var eventArgs = new ItemChangeEventArgs(item);
				this.OnItemChangeEvent(this, eventArgs);
			}
			#endregion
		}
	}
}