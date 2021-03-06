/// <summary>
/// キャラページ付リスト制御
/// 
/// 2016/01/12
/// </summary>
using System;
using System.Collections.Generic;
using Scm.Common.XwMaster;

namespace XUI
{
	namespace CharaList
	{
		/// <summary>
		/// 登録されているキャラアイテムが押された時のディスパッチ
		/// </summary>
		public class ItemClickEventArgs : EventArgs
		{
			public GUICharaItem Item { get; private set; }
			public ItemClickEventArgs(GUICharaItem item)
			{
				this.Item = item;
			}
		}

		/// <summary>
		/// 登録されているキャラアイテムに変更があった時のディスパッチ
		/// </summary>
		public class ItemChangeEventArgs : EventArgs
		{
			public GUICharaItem Item { get; private set; }
			public ItemChangeEventArgs(GUICharaItem item)
			{
				this.Item = item;
			}
		}

		/// <summary>
		/// キャラページリスト制御インターフェイス
		/// </summary>
		public interface IPageListController
		{
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();

			/// <summary>
			/// データ初期化
			/// </summary>
			void Setup();

			/// <summary>
			/// スクロールビューにアイテムをセットする
			/// </summary>
			/// <param name="charaList"></param>
			/// <param name="mainOwnCharaId"></param>
			void SetupItems(List<CharaInfo> charaList);

			/// <summary>
			/// 現ページ内のアイテム更新
			/// </summary>
			void UpdateItems();

			/// <summary>
			/// 登録されているキャラアイテムが押された時のイベント通知用
			/// </summary>
			event EventHandler<ItemClickEventArgs> OnItemClickEvent;

			/// <summary>
			/// 登録されているキャラアイテムが長押しされた時のイベント通知用
			/// </summary>
			event EventHandler<ItemClickEventArgs> OnItemLongPressEvent;

			/// <summary>
			/// ページリストのキャラアイテムに変更があった時のイベント通知用
			/// </summary>
			event EventHandler<ItemChangeEventArgs> OnItemChangeEvent;

			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }

			/// <summary>
			/// 表示キャラ情報リストを更新
			/// </summary>
			void SetViewCharaInfoList(List<CharaInfo> charaInfoList);

			/// <summary>
			/// 所持数追加ボタンが押された時のイベント通知用
			/// </summary>
			event EventHandler OnAddCapacityClickEvent;

			/// <summary>
			/// 全てのアイテムが更新された時のイベント通知
			/// </summary>
			event EventHandler OnUpdateItemsEvent;

			/// <summary>
			/// 所持数追加の有効設定
			/// </summary>
			void SetAddCapacityEnable(bool isEnable);

			/// <summary>
			/// キャラソート画面のOKボタンが押された時に呼ばれる
			/// </summary>
			void HandleOKClickEvent(CharaSort.OKClickEventArgs e);

			/// <summary>
			/// キャラソート画面のソート項目に変化があった時に呼ばれる
			/// </summary>
			void HandleSortPatternChangeEvent();

			// キャラソートに関わるデータをすべて同期させる
			void SyncCharaSort();

			/// <summary>
			/// 最初のページに設定
			/// </summary>
			void BackEndPage();
		}

		/// <summary>
		/// キャラページリスト制御
		/// </summary>
		public class PageListController : IPageListController
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
			/// ページ付ビュー
			/// </summary>
			private readonly ICharaPageListView _pageListView;
			private ICharaPageListView PageListView { get { return _pageListView; } }

			/// <summary>
			/// 登録されているキャラアイテムが押された時のイベント通知用
			/// </summary>
			public event EventHandler<ItemClickEventArgs> OnItemClickEvent = (sender, e) => { };

			/// <summary>
			/// 登録されているキャラアイテムが長押しされた時のイベント通知用
			/// </summary>
			public event EventHandler<ItemClickEventArgs> OnItemLongPressEvent;

			/// <summary>
			/// ページリストのキャラアイテムに変更があった時のイベント通知用
			/// </summary>
			public event EventHandler<ItemChangeEventArgs> OnItemChangeEvent;

			/// <summary>
			/// 所持数追加ボタンが押された時のイベント通知用
			/// </summary>
			public event EventHandler OnAddCapacityClickEvent = (sender, e) => { };

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
					if (this.PageListView == null) return false;
					return true;
				}
			}

			/// <summary>
			/// ソートタイプリスト
			/// </summary>
			LinkedList<CharaSort.Controller.SortPatternType> sortPatternList = new LinkedList<CharaSort.Controller.SortPatternType>();

			/// <summary>
			/// ソート項目をキーとした比較メソッド一覧
			/// </summary>
			Dictionary<CharaSort.Controller.SortPatternType, Func<CharaInfo, CharaInfo, bool, int>> compareDic = new Dictionary<CharaSort.Controller.SortPatternType, Func<CharaInfo, CharaInfo, bool, int>>();
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public PageListController(IModel model, IView view, ICharaPageListView pageListView)
			{
				if (model == null || view == null || pageListView == null) {  return; }

				// ビュー設定
				this._view = view;
				this._pageListView = pageListView;
				this.PageListView.OnNextPageClickEvent += OnNextPage;
				this.PageListView.OnNextEndClickEvent += OnNextEndPage;
				this.PageListView.OnBackPageClickEvent += OnBackPage;
				this.PageListView.OnBackEndClickEvent += OnBackEndPage;
				this.PageListView.OnAddCapacityClickEvent += OnAddCapacityClick;
				this.PageListView.OnSortClickEvent += this.HandleSortClickEvent;

				// モデル設定
				this._model = model;
				this.Model.OnCapacityChange += HandleCapacityChange;
				this.Model.OnCapacityFormatChange += HandleCapacityFormatChange;
				this.Model.OnTotalCapacityChange += HandleTotalCapacityChange;
				this.Model.OnCharaInfoListChange += HandleOwnCharaInfoListChange;
				this.Model.OnCountFormatChange += HandleCountFormatChange;
				this.Model.OnViewCharaInfoListChange += HandleViewCharaInfoListChange;
				this.Model.OnClearCharaInfoChange += HandleClearCharaInfoChange;

				// ページ付スクロールビューを初期化
				this.Model.ItemScrollView.Create(this.View.PageScrollViewAttach, null);

				// 売り切り版 入手順で並べるのでソート機能はOFF
				// ソートタイプリスト初期化
				//SetDefalutSortPatternList();
				//InitSortCompareList();

				// 所持数追加は有効設定に
				this.SetAddCapacityEnable(true);
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
				this.OnAddCapacityClickEvent = null;
				this.OnUpdateItemsEvent = null;
			}

			/// <summary>
			/// データ初期化
			/// </summary>
			public void Setup()
			{
				// テーブルクリア
				this.Clear();
				// 1ページ目にセット
				this.BackEndPage();
			}

			/// <summary>
			/// ソートで使用する比較メソッドを登録する
			/// </summary>
			private void InitSortCompareList()
			{
				this.compareDic.Clear();
				this.compareDic.Add(CharaSort.Controller.SortPatternType.Rank, (infoX, infoY, isAscend) => { return CharaInfo.RankCompare(infoX, infoY, isAscend); });
				this.compareDic.Add(CharaSort.Controller.SortPatternType.Cost, (infoX, infoY, isAscend) => { return CharaInfo.CostCompare(infoX, infoY, isAscend); });
				this.compareDic.Add(CharaSort.Controller.SortPatternType.Level, (infoX, infoY, isAscend) => { return CharaInfo.LevelCompare(infoX, infoY, isAscend); });
				this.compareDic.Add(CharaSort.Controller.SortPatternType.CharaType, (infoX, infoY, isAscend) => { return CharaInfo.TypeCompare(infoX, infoY, isAscend); });
				this.compareDic.Add(CharaSort.Controller.SortPatternType.HitPoint, (infoX, infoY, isAscend) => { return CharaInfo.HitPointCompare(infoX, infoY, isAscend); });
				this.compareDic.Add(CharaSort.Controller.SortPatternType.Attack, (infoX, infoY, isAscend) => { return CharaInfo.AttackCompare(infoX, infoY, isAscend); });
				this.compareDic.Add(CharaSort.Controller.SortPatternType.Defense, (infoX, infoY, isAscend) => { return CharaInfo.DefenseCompare(infoX, infoY, isAscend); });
				this.compareDic.Add(CharaSort.Controller.SortPatternType.Extra, (infoX, infoY, isAscend) => { return CharaInfo.ExtraCompare(infoX, infoY, isAscend); });
				this.compareDic.Add(CharaSort.Controller.SortPatternType.Obtaining, (infoX, infoY, isAscend) => { return CharaInfo.UUIDCompare(infoX, infoY, isAscend); });
			}
			/// <summary>
			/// デフォルトのソートリストセットする
			/// </summary>
			public void SetDefalutSortPatternList()
			{
				// ソート実行順にソートタイプを登録
				this.sortPatternList.Clear();
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.Rank);
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.Cost);
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.Level);
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.CharaType);
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.HitPoint);
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.Attack);
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.Defense);
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.Extra);
				this.sortPatternList.AddLast(CharaSort.Controller.SortPatternType.Obtaining);
			}

			/// <summary>
			/// キャラ情報リストやテーブルをクリアする
			/// </summary>
			private void Clear()
			{
				if (!CanUpdate) { return; }
				this.Model.ItemScrollView.Clear();
				this.Model.ClearCharaInfo();
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
				if (!CanUpdate) { return; }
				SyncCapacity();
			}
			/// <summary>
			/// 容量表示形式
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleCountFormatChange(object sender, EventArgs e)
			{
				SyncCapacity();
			}
			/// <summary>
			/// 容量表示形式変更ハンドラー
			/// </summary>
			private void HandleCapacityFormatChange(object sender, EventArgs e)
			{
				this.SyncCapacity();
			}
			/// <summary>
			/// アイテム総数同期
			/// </summary>
			private void SyncCapacity()
			{
				if (!CanUpdate) { return; }
				// 容量表示設定
				int count = this.Model.CharaInfoList.Count;
				int capacity = this.Model.Capacity;
				string countFormat = string.Format(this.Model.CountFormat, count);
				string capacityFormat = string.Format(this.Model.CapacityFormat, capacity);
				StatusColor.Type colorType = capacity < count ? StatusColor.Type.CapacityOver : StatusColor.Type.CapacityNormal;
				this.PageListView.SetCapacity(countFormat, capacityFormat, colorType);
			}

			/// <summary>
			/// Box総数がオーバーした分も含む合計数変更ハンドラー
			/// </summary>
			private void HandleTotalCapacityChange(object sender, EventArgs e)
			{
				this.SyncTotalCapacity();
			}
			/// <summary>
			/// Box総数がオーバーした分も含む合計数同期
			/// </summary>
			private void SyncTotalCapacity()
			{
				if (!this.CanUpdate) { return; }
				// ページスクロールビューのアイテム総数セット
				this.Model.ItemScrollView.Clear();
				this.Model.ItemScrollView.Setup(this.Model.TotalCapacity, 0);
			}
			#endregion

			#region スクロールビューにアイテムをセット
			/// <summary>
			/// スクロールビューにアイテムをセットする
			/// </summary>
			/// <param name="charaList"></param
			public void SetupItems(List<CharaInfo> charaList)
			{
				if (!CanUpdate) { return; }
				// 所有キャラリストセット
				this.Model.SetCharaInfoList(charaList);
			}

			/// <summary>
			/// 現ページ内のアイテム更新
			/// </summary>
			public void UpdateItems()
			{
				if (!CanUpdate) { return; }

				// 表示キャラ情報リストを取得
				List<CharaInfo> charaInfoList = this.Model.ViewCharaInfoList;

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
					if (charaInfoList.Count <= indexInTotal)
					{
						// 所持キャラ数よりも所有できる枠数の方が多ければ空枠とする
						item.SetState(CharaItem.Controller.ItemStateType.Empty, null);
						item.SetIndex(indexInTotal);
					}
					else
					{
						CharaInfo charaInfo = charaInfoList[indexInTotal];
						CharaItem.Controller.ItemStateType itemState;
						if(charaInfo == null)
						{
							// キャラ影アイコン
							itemState = CharaItem.Controller.ItemStateType.Exist;
						}
						else
						{
							// キャラが存在する枠
							itemState = CharaItem.Controller.ItemStateType.Icon;
						}

						// アイテムセット
						item.SetState(itemState, charaInfo);
						item.SetIndex(indexInTotal);
					}
				}

				// イベント通知
				this.OnUpdateItemsEvent(this, EventArgs.Empty);
			}
			#endregion

			#region キャラ情報リスト
			/// <summary>
			/// キャラ情報リストがセットされた時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleOwnCharaInfoListChange(object sender, EventArgs e)
			{
				SyncCharaInfoList();
			}

			/// <summary>
			/// キャラ情報リストがクリアされた時に呼ばれる
			/// </summary>
			private void HandleClearCharaInfoChange(object sender, EventArgs e)
			{
				this.SyncCharaInfoList();
			}

			/// <summary>
			/// キャラ情報リスト同期
			/// </summary>
			private void SyncCharaInfoList()
			{
				// アイテム総数同期
				SyncCapacity();
				// キャラ情報リストと表示キャラ情報リストを同期
				this.SetViewCharaInfoList(this.Model.CharaInfoList);
			}
			#endregion

			#region 表示キャラ情報リスト
			/// <summary>
			/// 表示キャラ情報リストを更新
			/// </summary>
			public void SetViewCharaInfoList(List<CharaInfo> charaInfoList)
			{
				if (this.Model == null || charaInfoList == null) { return; }

				// ソート後表示キャラ情報にセットする
				// 売り切り版 入手順に並べるのでソート機能はOFF
				//List<CharaInfo> viewList = this.SortExecute(charaInfoList);
				List<CharaInfo> viewList = charaInfoList;
				this.Model.SetViewCharaInfoList(viewList);
			}

			/// <summary>
			/// 表示キャラ情報リストが変更された時に呼び出される
			/// </summary>
			private void HandleViewCharaInfoListChange(object sender, EventArgs e)
			{
				this.SyncViewCharaList();
			}

			/// <summary>
			/// 表示キャラ情報リスト同期
			/// </summary>
			private void SyncViewCharaList()
			{
				UpdateItems();
			}
			#endregion

			#region ページ
			/// <summary>
			/// 次のページ
			/// </summary>
			private void OnNextPage(object sender, EventArgs e)
			{
				if (!CanUpdate) { return; }
				bool isPageChange = this.Model.ItemScrollView.SetNextPage(1);
				if (isPageChange)
				{
					ChangePage();
				}
			}

			/// <summary>
			/// 最後のページ
			/// </summary>
			private void OnNextEndPage(object sender, EventArgs e)
			{
				if (!CanUpdate) { return; }
				// 最後のページにする
				bool isPageChange = this.Model.ItemScrollView.SetPage(this.Model.ItemScrollView.PageMax - 1, 0);
				if (isPageChange)
				{
					ChangePage();
				}

			}

			/// <summary>
			/// 戻るページ
			/// </summary>
			private void OnBackPage(object sender, EventArgs e)
			{
				if (!CanUpdate) { return; }
				// ページを一つ前に戻す
				bool isPageChange = this.Model.ItemScrollView.SetNextPage(-1);
				if (isPageChange)
				{
					ChangePage();
				}
			}

			/// <summary>
			/// 最初のページボタンが押された時に呼ばれる
			/// </summary>
			private void OnBackEndPage(object sender, EventArgs e)
			{
				this.BackEndPage();
			}

			/// <summary>
			/// 最初のページに設定
			/// </summary>
			public void BackEndPage()
			{
				if (!CanUpdate) { return; }
				// 最初のページにする
				var isPageChange = this.Model.ItemScrollView.SetPage(0, 0);
				if (isPageChange)
				{
					ChangePage();
				}
			}

			/// <summary>
			/// ページが切り替わった時の処理
			/// </summary>
			private void ChangePage()
			{
				// ページが切り替わったのでアイテムの更新を行う
				UpdateItems();
			}
			#endregion

			#region アイテムイベント
			/// <summary>
			/// 登録されているキャラアイテムが押された時に呼び出される
			/// </summary>
			/// <param name="item"></param>
			private void OnItemClick(GUICharaItem item)
			{
				// 通知
				var eventArgs = new ItemClickEventArgs(item);
				this.OnItemClickEvent(this, eventArgs);
			}

			/// <summary>
			/// 登録されているキャラアイテムが長押しされた時に呼び出される
			/// </summary>
			/// <param name="item"></param>
			private void OnItemLongPress(GUICharaItem item)
			{
				// 通知
				var eventArgs = new ItemClickEventArgs(item);
				this.OnItemLongPressEvent(this, eventArgs);
			}

			/// <summary>
			/// 登録されいているキャラアイテムに変更があった時に呼び出される
			/// </summary>
			/// <param name="item"></param>
			private void OnItemChange(GUICharaItem item)
			{
				// 通知
				var eventArgs = new ItemChangeEventArgs(item);
				this.OnItemChangeEvent(this, eventArgs);
			}
			#endregion

			#region ソート
			/// <summary>
			/// ソートボタンが押された時に呼ばれる
			/// </summary>
			private void HandleSortClickEvent(object sender, EventArgs e)
			{
				// キャラソート画面表示
				var singleUI = new GUISingle(GUICharaSort.Open, GUICharaSort.Close);
				GUIController.SingleOpen(singleUI);
			}

			/// <summary>
			/// ソートリストに登録されている順にソートを実行させる
			/// </summary>
			private List<CharaInfo> SortExecute(List<CharaInfo> charaInfoList)
			{
				var sortList = new List<CharaInfo>();

				// キャラ情報リスト取得
				if (charaInfoList == null) { return sortList; }
				var infoList = new List<CharaInfo>();
				charaInfoList.ForEach((info) => { infoList.Add(info); });
				
				// ソート
				this.Sort(infoList);

				// 非選択表示設定
				if (GUICharaSort.GetIsSelectDisable())
				{
					// 選択可能なキャラのみ表示
					foreach (var info in infoList)
					{
						if (info != null && info.CanSelect)
						{
							sortList.Add(info);
						}
					}
				}
				else
				{
					// 非選択も含めたキャラを表示
					foreach (var info in infoList)
					{
						if (info != null)
						{
							sortList.Add(info);
						}
					}
				}

				// 非表示分はNULLをセット(キャラ存在アイコンにする)
				for (int count = sortList.Count; count < infoList.Count; ++count)
				{
					sortList.Add(null);
				}

				return sortList;
			}

			/// <summary>
			/// ソート項目リストに沿って並び替えを行う
			/// </summary>
			private void Sort(List<CharaInfo> charaInfoList)
			{
				// 昇順/降順フラグ取得
				bool isAscend = GUICharaSort.GetIsAscend();
				
				// ソートを行う
				charaInfoList.MargeSort((x, y) =>
				{
					return this.CharaInfoCompare(x, y, isAscend);
				});
			}

			/// <summary>
			/// キャラ情報の比較
			/// </summary>
			private int CharaInfoCompare(CharaInfo infoX, CharaInfo infoY, bool isAscend)
			{
				if (infoX == null || infoY == null) { return 0; }
				int ret = 0;
				foreach (CharaSort.Controller.SortPatternType type in this.sortPatternList)
				{
					Func<CharaInfo, CharaInfo, bool, int> compare;
					if (this.compareDic.TryGetValue(type, out compare))
					{
						ret = compare(infoX, infoY, isAscend);
						if (ret != 0)
						{
							return ret;
						}
					}
				}

				return ret;
			}
			#endregion

			#region キャラソート画面
			/// <summary>
			/// キャラソート画面のOKボタンが押された時に呼ばれる
			/// </summary>
			public void HandleOKClickEvent(CharaSort.OKClickEventArgs e)
			{
				// ソートリスト同期
				this.SyncSortPatternList();

				// キャラソート画面は閉じる
				GUIController.SingleClose();

				// 表示キャラ情報を更新
				this.SetViewCharaInfoList(this.Model.CharaInfoList);
			}
			/// <summary>
			/// ソート項目リスト同期
			/// </summary>
			private void SyncSortPatternList()
			{
				// デフォルト順にリセット
				this.SetDefalutSortPatternList();

				var sortPattern = GUICharaSort.GetSortPattern();
				// 選択されているソートタイプが最優先にソートされるようにリストに登録
				if(this.sortPatternList.Contains(sortPattern))
				{
					this.sortPatternList.Remove(sortPattern);
				}
				this.sortPatternList.AddFirst(sortPattern);
			}

			/// <summary>
			/// キャラソート画面のソート項目に変化があった時に呼ばれる
			/// </summary>
			public void HandleSortPatternChangeEvent()
			{
				// 選択されているソート項目同期
				this.SyncSelectSortPattern();
			}
			/// <summary>
			/// 選択されているソート項目を同期
			/// </summary>
			private void SyncSelectSortPattern()
			{
				if (!this.CanUpdate) { return; }

				// ソート項目表示設定
				var sortPattern = GUICharaSort.GetSortPattern();
				string typeName = MasterData.GetText(TextType.TX239_CharaList_Order);
				switch (sortPattern)
				{
					case CharaSort.Controller.SortPatternType.Rank:
						typeName += MasterData.GetText(TextType.TX251_Common_Rank);
						break;
					case CharaSort.Controller.SortPatternType.Cost:
						typeName += MasterData.GetText(TextType.TX259_Common_Cost);
						break;
					case CharaSort.Controller.SortPatternType.Level:
						typeName += MasterData.GetText(TextType.TX252_Common_Level);
						break;
					case CharaSort.Controller.SortPatternType.CharaType:
						typeName += MasterData.GetText(TextType.TX260_Sort_Type);
						break;
					case CharaSort.Controller.SortPatternType.Obtaining:
						typeName += MasterData.GetText(TextType.TX261_Sort_Obtaining);
						break;
					case CharaSort.Controller.SortPatternType.HitPoint:
						typeName += MasterData.GetText(TextType.TX253_CharaInfo_HitPoint);
						break;
					case CharaSort.Controller.SortPatternType.Attack:
						typeName += MasterData.GetText(TextType.TX254_CharaInfo_Attack);
						break;
					case CharaSort.Controller.SortPatternType.Defense:
						typeName += MasterData.GetText(TextType.TX255_CharaInfo_Defense);
						break;
					case CharaSort.Controller.SortPatternType.Extra:
						typeName += MasterData.GetText(TextType.TX256_CharaInfo_Extra);
						break;
				}

				this.PageListView.SetSortTypeName(typeName);
			}

			// キャラソートに関わるデータをすべて同期させる
			public void SyncCharaSort()
			{
				// 選択されている項目とソートリストを同期
				this.SyncSortPatternList();
				this.SyncSelectSortPattern();
			}
			#endregion

			#region 所持数追加
			/// <summary>
			/// 所持数追加ボタンが押された時に呼び出される
			/// </summary>
			private void OnAddCapacityClick(object sender, EventArgs e)
			{
				// 通知
				this.OnAddCapacityClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// 所持数追加の有効設定
			/// </summary>
			public void SetAddCapacityEnable(bool isEnable)
			{
				if (!this.CanUpdate) { return; }
				this.PageListView.SetAddCapacityEnable(isEnable);
			}
			#endregion
		}
	}
}