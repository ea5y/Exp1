/// <summary>
/// 進化素材リスト制御インターフェイス
/// 
/// 2016/02/08
/// </summary>
using System;
using System.Collections.Generic;
using Scm.Common.XwMaster;

namespace XUI
{
	namespace CharaList
	{
		/// <summary>
		/// 進化素材リスト制御インターフェイス
		/// </summary>
		public interface IEvolutionMaterialListController
		{
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();

			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }

			#region アイテム総数
			/// <summary>
			/// アイテム総数を設定
			/// </summary>
			void SetupCapacity(int capacity);
			#endregion

			#region アイテム設定
			/// <summary>
			/// アイテムを設定する
			/// </summary>
			void SetupItems(CharaInfo baseCharaInfo, Dictionary<int, List<CharaInfo>> haveCharaInfoDic);
			#endregion

			#region 素材
			/// <summary>
			/// 進化素材を選択されている素材欄にセットする
			/// </summary>
			bool SetBaitMaterial(CharaInfo charaInfo);

			/// <summary>
			/// セットされている素材を解除する
			/// </summary>
			GUICharaItem RemoveBaitMaterial(CharaInfo charaInfo);
			#endregion

			#region キャラ情報
			/// <summary>
			/// セットされている素材キャラ情報のみを更新する
			/// </summary>
			void UpdateBaitMaterialCharaInfo(List<CharaInfo> charaInfoList);
			#endregion

			#region アイテム選択
			/// <summary>
			/// 選択アイテム
			/// </summary>
			GUICharaItem SelectItem { get; set; }

			/// <summary>
			/// 次選択するアイテムを取得する
			/// </summary>
			GUICharaItem GetNextSelectItem();
			#endregion

			#region 進化可能
			/// <summary>
			/// 進化可能状態か
			/// </summary>
			bool CanEvolution { get; }
			#endregion

			#region クリア
			/// <summary>
			/// 進化素材情報を削除
			/// </summary>
			bool ClearMaterial();
			#endregion

			#region アイテムイベント
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
			#endregion
		}

		/// <summary>
		/// 進化素材リスト制御インターフェイス
		/// </summary>
		public class EvolutionMaterialListController : IEvolutionMaterialListController
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
			/// 進化素材リストビュー
			/// </summary>
			private readonly IEvolutionMaterialListView _materialListView;
			private IEvolutionMaterialListView MaterialListView { get { return _materialListView; } }

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
					if (this.MaterialListView == null) return false;
					return true;
				}
			}

			/// <summary>
			/// 選択アイテム
			/// </summary>
			private GUICharaItem _selectItem = null;
			public GUICharaItem SelectItem
			{
				get { return this._selectItem; }
				set
				{
					if(_selectItem != value)
					{
						if(this._selectItem != value)
						{
							_selectItem = value;
							// 同期
							SyncSelectItem();
						}
					}
				}
			}

			/// <summary>
			/// ソートに使用する比較メソッドリスト
			/// </summary>
			LinkedList<Func<CharaInfo, CharaInfo, int>> compareList = new LinkedList<Func<CharaInfo, CharaInfo, int>>();
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public EvolutionMaterialListController(IModel model, IView view, IEvolutionMaterialListView materialListView)
			{
				if (model == null || view == null || materialListView == null) { return; }

				// ビュー設定
				this._view = view;
				this._materialListView = materialListView;

				// モデル設定
				this._model = model;
				this.Model.OnCapacityChange += this.HandleCapacityChange;
				this.Model.OnCharaInfoListChange += this.HandleCharaInfoListChange;
				this.Model.OnClearCharaInfoChange += this.HandleClearCharaInfoChange;

				// ページ付スクロールビューを初期化
				this.Model.ItemScrollView.Create(this.View.PageScrollViewAttach, null);
				ClearTable();

				// ソート比較リスト初期化
				InitSortCompareList();
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
				this.OnItemLongPressEvent = null;
				this.OnItemChangeEvent = null;
				this.OnUpdateItemsEvent = null;
			}

			/// <summary>
			/// ソートで使用する比較メソッドを登録する
			/// </summary>
			private void InitSortCompareList()
			{
				// 登録順にソートされるので注意
				this.compareList.Clear();
				this.compareList.AddLast((infoX, infoY) => { return CharaInfo.RankCompare(infoX, infoY, true); });
				this.compareList.AddLast((infoX, infoY) => { return CharaInfo.LevelCompare(infoX, infoY, true); });
				this.compareList.AddLast((infoX, infoY) => { return CharaInfo.TotalSynchroBonusCompare(infoX, infoY, true); });
				this.compareList.AddLast((infoX, infoY) => { return CharaInfo.PowerupSlotNumCompare(infoX, infoY, true); });
				this.compareList.AddLast((infoX, infoY) => { return CharaInfo.UUIDCompare(infoX, infoY, false); });
			}
			#endregion

			#region アイテム総数
			/// <summary>
			/// アイテム総数を設定する
			/// </summary>
			public void SetupCapacity(int capacity)
			{
				if (this.Model == null) { return; }
				this.Model.Capacity = capacity;
			}

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
			}
			#endregion

			#region アイテムセット
			/// <summary>
			/// アイテムを設定する
			/// </summary>
			public void SetupItems(CharaInfo baseCharaInfo, Dictionary<int, List<CharaInfo>> haveCharaInfoDic)
			{
				// 進化素材キャラマスター取得
				var dataList = new List<CharaEvolutionMaterialMasterData>();
				if (baseCharaInfo != null)
				{
					if (!MasterData.TryGetCharaEvolutionMaterial((int)baseCharaInfo.AvatarType, Math.Min(baseCharaInfo.Rank + 1, 5), out dataList))
					{
						// マスタデータ取得失敗
						string msg = string.Format("CharaEvolutionMaterialMaster NotFound. AvatarType = {0} Rank = {1}", baseCharaInfo.AvatarType, baseCharaInfo.Rank);
						BugReportController.SaveLogFile(msg);
						UnityEngine.Debug.LogWarning(msg);
					}
				}

				// 進化素材キャラリストを取得
				List<CharaInfo> setList = new List<CharaInfo>();
				foreach (var masterData in dataList)
				{
					// 素材と同じ所持キャラの一覧を取得
					var haveCharaInfoList = new List<CharaInfo>();
					if(haveCharaInfoDic.TryGetValue(masterData.MaterialCharacterMasterId, out haveCharaInfoList))
					{
						// 所持キャラの中から自動で素材に設定するキャラ情報を取得
						var setCharaInfo = this.GetMaterialAutoSetCharaInfo(haveCharaInfoList, setList);
						if (setCharaInfo != null)
						{
							// 所持キャラセット
							setList.Add(setCharaInfo);
						}
						else
						{
							// 所持キャラに素材可能なキャラが存在しない
							setList.Add(new CharaInfo((AvatarType)masterData.MaterialCharacterMasterId));
						}
					}
					else
					{
						// 所持キャラに素材可能なキャラが存在しない(基本はここの処理は通らない バグ等で正常なデータが設定されていない場合の処理)
						setList.Add(new CharaInfo((AvatarType)masterData.MaterialCharacterMasterId));
					}
				}

				// キャラ情報登録
				this.Model.SetCharaInfoList(setList);
			}

			/// <summary>
			/// 所持キャラの中から自動で素材に設定するキャラ情報を取得
			/// </summary>
			private CharaInfo GetMaterialAutoSetCharaInfo(List<CharaInfo> charaInfoList, List<CharaInfo> setCharaList)
			{
				CharaInfo setInfo = null;

				if (charaInfoList.Count > 0)
				{
					// 最優先で設定するためのソートを行う
					charaInfoList.Sort((infoX, infoY) =>
					{
						return this.MaterialAutoSetCompare(infoX, infoY);
					});

					foreach(var info in charaInfoList)
					{
						// すでに自動設定対象に設定されているキャラかどうか検索
						if(!setCharaList.Exists(x => x.UUID == info.UUID))
						{
							setInfo = info;
							break;
						}
					}
				}
				
				return setInfo;
			}

			/// <summary>
			/// 自動で素材に設定するためのキャラ情報比較処理
			/// </summary>
			private int MaterialAutoSetCompare(CharaInfo infoX, CharaInfo infoY)
			{
				if (infoX == null || infoY == null) { return 0; }
				int ret = 0;
				foreach(var compare in this.compareList)
				{
					ret = compare(infoX, infoY);
					if(ret != 0)
					{
						return ret;
					}
				}

				return ret;
			}
			#endregion

			#region アイテム更新
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
						// キャラ情報数よりも所有できる枠数の方が多ければアイテム枠とする
						item.SetState(CharaItem.Controller.ItemStateType.FillEmpty, null);
						item.SetIndex(indexInTotal);
					}
					else
					{
						// キャラが存在する枠
						CharaInfo charaInfo = this.Model.CharaInfoList[indexInTotal];
						CharaItem.Controller.ItemStateType itemState;
						if(charaInfo.UUID > 0)
						{
							// キャラアイコンをセットする
							itemState = CharaItem.Controller.ItemStateType.Icon;
						}
						else
						{
							// 素材アイコンをセットする
							itemState = CharaItem.Controller.ItemStateType.Mono;
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

			#region 素材
			/// <summary>
			/// 進化素材を選択されている素材欄にセットする
			/// </summary>
			public bool SetBaitMaterial(CharaInfo charaInfo)
			{
				if (this.Model == null || charaInfo == null) { return false; }

				// 現在選択されているアイテムを取得
				GUICharaItem selectItem = SelectItem;
				if (selectItem == null || selectItem.GetCharaInfo() == null) { return false; }

				// 選択アイテムが素材アイコン状態かチェック
				if (selectItem.GetCharaInfo().UUID > 0) { return false; }

				// 素材をセット出来るかどうかチェック
				if (charaInfo.AvatarType != selectItem.GetCharaInfo().AvatarType ||
					charaInfo.Rank < selectItem.GetMaterialRank())
				{
					// 進化素材の条件に満たしていないのでセットしない
					return false;
				}

				// キャラ情報リスト更新
				this.AddCharaInfoList(selectItem.GetIndex(), charaInfo);

				return true;
			}

			/// <summary>
			/// セットされている素材を解除する
			/// </summary>
			public GUICharaItem RemoveBaitMaterial(CharaInfo charaInfo)
			{
				if (this.Model == null || charaInfo == null) { return null; }
				
				// 削除するインデックス値を取得
				int removeIndex = -1;
				for (int index = 0; index < this.Model.CharaInfoList.Count; ++index)
				{
					CharaInfo info = this.Model.CharaInfoList[index];
					if (info != null && info.UUID == charaInfo.UUID)
					{
						removeIndex = index;
						break;
					}
				}
				if (removeIndex == -1)
				{
					// 削除するキャラ情報が見つからない
					return null;
				}

				// セットされている素材を解除し素材アイコン状態にする
				this.AddCharaInfoList(removeIndex, new CharaInfo(charaInfo.AvatarType));

				// セットした素材アイコンを取得
				GUICharaItem removeMaterialItem = null;
				foreach (var item in this.Model.GetNowPageItemList())
				{
					if (item.GetIndex() == removeIndex)
					{
						removeMaterialItem = item;
						break;
					}
				}

				return removeMaterialItem;
			}
			#endregion

			#region キャラ情報
			/// <summary>
			/// キャラ情報リストがセットされた時に呼び出される
			/// </summary>
			private void HandleCharaInfoListChange(object sender, EventArgs e)
			{
				this.SyncCharaInfoList();
			}

			/// <summary>
			/// キャラ情報同期
			/// </summary>
			private void SyncCharaInfoList()
			{
				UpdateItems();
			}

			/// <summary>
			/// キャラリストのインデックス値にキャラ情報を追加する
			/// </summary>
			private void AddCharaInfoList(int index, CharaInfo info)
			{
				if (this.Model == null) { return; }
				if(index < this.Model.CharaInfoList.Count || index > -1)
				{
					// キャラリスト更新
					this.Model.CharaInfoList[index] = info;
					UpdateItems();
				}
			}

			/// <summary>
			/// セットされている素材キャラ情報のみを更新する
			/// </summary>
			public void UpdateBaitMaterialCharaInfo(List<CharaInfo> charaInfoList)
			{
				if (this.Model == null) { return; }
				foreach (var charaInfo in charaInfoList)
				{
					for (int index = 0; index < this.Model.CharaInfoList.Count; ++index)
					{
						CharaInfo info = this.Model.CharaInfoList[index];
						if (info != null && info.UUID > 0)
						{
							if (info.UUID == charaInfo.UUID)
							{
								this.Model.CharaInfoList[index] = charaInfo.Clone();
								break;
							}
						}
					}
				}

				// アイテム更新
				this.UpdateItems();
			}
			#endregion

			#region 選択アイテム
			/// <summary>
			/// 選択アイテム同期
			/// </summary>
			private void SyncSelectItem()
			{
				UpdateSelectItems();
			}

			/// <summary>
			/// 選択アイテム更新
			/// </summary>
			private void UpdateSelectItems()
			{
				if (this.Model == null) { return; }
				foreach(var item in this.Model.GetNowPageItemList())
				{
					if(this.SelectItem != null && this.SelectItem == item)
					{
						item.SetSelect(true);
					}
					else
					{
						item.SetSelect(false);
					}
				}
			}

			/// <summary>
			/// 次選択するアイテムを取得する
			/// </summary>
			public GUICharaItem GetNextSelectItem()
			{
				// 追加順に検索し素材アイコンで選択されていなければ次の選択先のアイテムを取得する
				foreach (var item in this.Model.GetNowPageItemList())
				{
					// キャラ情報取得
					// (アイテムからキャラ情報取得だとまだアイコンが読み込み中の可能性があるため取得できないことがある)
					int index = item.GetIndex();
					if (this.Model.CharaInfoList.Count <= index || index <= -1) { continue; }
					var charaInfo = this.Model.CharaInfoList[index];

					if (charaInfo != null && charaInfo.UUID == 0 && !item.GetSelect())
					{
						// 次に選択するアイテム
						return item;
					}
				}

				// 次に選択するアイテムが存在しない場合は現在選択されているアイテムを返す
				return this.SelectItem;
			}
			#endregion

			#region クリア
			/// <summary>
			/// 進化素材情報を削除
			/// </summary>
			public bool ClearMaterial()
			{
				if (this.Model == null) { return false; }

				// 進化情報がセットされているか検索
				bool isExecute = false;
				foreach(var info in this.Model.CharaInfoList)
				{
					if (info != null)
					{
						// ひとつでもキャラ情報が存在するならクリア処理を行う
						isExecute = true;
						break;
					}
				}

				if(isExecute)
				{
					this.Model.ClearCharaInfo();
				}

				return isExecute;
			}

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

			#region 進化可能
			/// <summary>
			/// 進化可能状態か
			/// </summary>
			public bool CanEvolution
			{
				get
				{
					if (this.Model == null || this.Model.CharaInfoList.Count == 0) { return false; }

					foreach(var info in this.Model.CharaInfoList)
					{
						if(info.UUID <= 0)
						{
							// 一つでもキャラ進化素材がセットされていないければ進化不能
							return false;
						}
					}

					return true;
				}
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