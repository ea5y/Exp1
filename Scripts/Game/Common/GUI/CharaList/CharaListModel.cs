/// <summary>
/// キャラリストデータ
/// 
/// 2016/01/12
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI
{
	namespace CharaList
	{
		/// <summary>
		/// キャラリストデータインターフェイス
		/// </summary>
		public interface IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();
			#endregion

			#region ページ付スクロールビュー
			/// <summary>
			/// アイテムページ付スクロールビュー
			/// </summary>
			GUIItemScrollView ItemScrollView { get; }

			/// <summary>
			/// ページ付スクロールビューで生成している全てのアイテムを取得
			/// </summary>
			List<GUICharaItem> GetPageItemList();

			/// <summary>
			/// 現ページ内のアイテムリストを返す
			/// </summary>
			/// <returns></returns>
			List<GUICharaItem> GetNowPageItemList();
			#endregion

			#region キャラ情報リスト
			/// <summary>
			/// キャラ情報リスト
			/// </summary>
			List<CharaInfo> CharaInfoList { get; }

			/// <summary>
			/// キャラ情報リストにセットする
			/// </summary>
			/// <param name="info"></param>
			event EventHandler OnCharaInfoListChange;
			void SetCharaInfoList(List<CharaInfo> charaInfoList);

			/// <summary>
			/// キャラ情報追加
			/// </summary>
			/// <param name="charaInfo"></param>
			event EventHandler OnAddCharaInfoChange;
			void AddCharaInfo(CharaInfo info);

			/// <summary>
			/// キャラ情報の削除
			/// </summary>
			event EventHandler OnRemoveCharaInfoChange;
			bool RemoveCharaInfo(CharaInfo info);

			/// <summary>
			/// キャラ情報全削除
			/// </summary>
			event EventHandler OnClearCharaInfoChange;
			void ClearCharaInfo();
			#endregion

			#region 表示キャラ情報リスト
			/// <summary>
			/// 表示キャラ情報リスト
			/// </summary>
			List<CharaInfo> ViewCharaInfoList { get; }

			/// <summary>
			/// 表示キャラ情報リストにセットする
			/// </summary>
			/// <param name="info"></param>
			event EventHandler OnViewCharaInfoListChange;
			void SetViewCharaInfoList(List<CharaInfo> viewCharaInfoList);
			#endregion

			#region アイテム総数
			/// <summary>
			/// アイテムの総数
			/// </summary>
			event EventHandler OnCapacityChange;
			int Capacity { get; set; }

			/// <summary>
			/// Box総数がオーバーした分も含む合計数
			/// </summary>
			event EventHandler OnTotalCapacityChange;
			int TotalCapacity { get; set; }
			#endregion

			#region 容量
			/// <summary>
			/// 容量表示形式
			/// </summary>
			event EventHandler OnCountFormatChange;
			string CountFormat { get; set; }

			/// <summary>
			/// 最大容量表示形式
			/// </summary>
			event EventHandler OnCapacityFormatChange;
			string CapacityFormat { get; set; }
			#endregion
		}

		/// <summary>
		/// キャラリストデータ
		/// </summary>
		public class Model : IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				this.OnCharaInfoListChange = null;
				this.OnAddCharaInfoChange = null;
				this.OnTotalCapacityChange = null;
				this.OnCapacityFormatChange = null;
				this.OnRemoveCharaInfoChange = null;
				this.OnClearCharaInfoChange = null;
				this.OnViewCharaInfoListChange = null;
				this.OnCapacityChange = null;
				this.OnCountFormatChange = null;
			}
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="itemScrollView"></param>
			public Model(GUIItemScrollView itemScrollView)
			{
				_itemScrollView = itemScrollView;
			}
			#endregion

			#region ページ付スクロールビュー
			/// <summary>
			/// アイテムページ付スクロールビュー
			/// </summary>
			private readonly GUIItemScrollView _itemScrollView;
			public GUIItemScrollView ItemScrollView { get { return _itemScrollView; } }

			/// <summary>
			/// ページ付スクロールビューで生成している全てのアイテムを取得
			/// </summary>
			public List<GUICharaItem> GetPageItemList()
			{
				var itemList = new List<GUICharaItem>();

				foreach(var item in this.ItemScrollView.ItemList)
				{
					itemList.Add(item);
				}

				return itemList;
			}

			/// <summary>
			/// 現ページ内のアイテムリストを返す
			/// </summary>
			public List<GUICharaItem> GetNowPageItemList()
			{
				var itemList = new List<GUICharaItem>();

				// 現ページの最初のアイテムインデックスを取得
				int startIndex = this.ItemScrollView.NowPageStartIndex;

				// 現在のページ内のアイテムを更新
				for (int i = 0, max = this.ItemScrollView.NowPageItemMax; i < max; i++)
				{
					int indexInTotal = i + startIndex;
					if (!this.ItemScrollView.IsNowPage(indexInTotal))
					{
						// 現在のページ内のアイテムではない
						continue;
					}

					// スクロールビューからアイテムを取得
					int itemIndex = this.ItemScrollView.GetItemIndex(indexInTotal);
					GUICharaItem item = this.ItemScrollView.GetItem(itemIndex);

					// リストに追加
					itemList.Add(item);
				}

				return itemList;
			}
			#endregion

			#region キャラ情報
			/// <summary>
			/// 所有しているキャラリスト
			/// </summary>
			private List<CharaInfo> _CharaInfoList = new List<CharaInfo>();
			public List<CharaInfo> CharaInfoList
			{
				get { return _CharaInfoList; }
			}
			
			/// <summary>
			/// キャラ情報リストをセットする
			/// </summary>
			public event EventHandler OnCharaInfoListChange = (sender, e) => { };
			public void SetCharaInfoList(List<CharaInfo> charaInfoList)
			{
				if (charaInfoList == null) { return; }
				this.CharaInfoList.Clear();
				foreach(var info in charaInfoList)
				{
					// リストに登録
					if(info == null)
					{
						this.CharaInfoList.Add(info);
					}
					else
					{
						this.CharaInfoList.Add(info.Clone());
					}
				}

				// 通知
				this.OnCharaInfoListChange(this, EventArgs.Empty);
			}

			/// <summary>
			/// キャラ情報追加
			/// </summary>
			/// <param name="charaInfo"></param>
			public event EventHandler OnAddCharaInfoChange = (sender, e) => { };
			public void AddCharaInfo(CharaInfo charaInfo)
			{
				if(charaInfo == null)
				{
					this.CharaInfoList.Add(charaInfo);
				}
				else
				{
					this.CharaInfoList.Add(charaInfo.Clone());
				}

				// 通知
				OnAddCharaInfoChange(this, EventArgs.Empty);
			}

			/// <summary>
			/// キャラ情報の削除
			/// </summary>
			public event EventHandler OnRemoveCharaInfoChange = (sender, e) => { };
			public bool RemoveCharaInfo(CharaInfo info)
			{
				// 削除
				bool isRemove = this.CharaInfoList.Remove(info);

				if(isRemove)
				{
					// 通知
					OnRemoveCharaInfoChange(this, EventArgs.Empty);
				}

				return isRemove;
			}

			/// <summary>
			/// キャラ情報クリア
			/// </summary>
			public event EventHandler OnClearCharaInfoChange = (sender, e) => { };
			public void ClearCharaInfo()
			{
				// クリア
				this.CharaInfoList.Clear();
				// 通知
				OnClearCharaInfoChange(this, EventArgs.Empty);
			}
			#endregion

			#region 表示キャラ情報
			/// <summary>
			/// 表示キャラ情報リスト
			/// </summary>
			private List<CharaInfo> _viewCharaInfoList = new List<CharaInfo>();
			public List<CharaInfo> ViewCharaInfoList { get { return _viewCharaInfoList; } }

			/// <summary>
			/// 表示キャラ情報リストにセットする
			/// </summary>
			public event EventHandler OnViewCharaInfoListChange = (sender, e) => { };
			public void SetViewCharaInfoList(List<CharaInfo> viewCharaInfoList)
			{
				if (viewCharaInfoList == null) { return; }
				this.ViewCharaInfoList.Clear();
				foreach (var info in viewCharaInfoList)
				{
					// リストに登録
					this.ViewCharaInfoList.Add(info);
				}

				// 通知
				this.OnViewCharaInfoListChange(this, EventArgs.Empty);
			}
			#endregion

			#region アイテム総数
			/// <summary>
			/// アイテムの総数
			/// </summary>
			public event EventHandler OnCapacityChange = (sender, e) => { };
			private int _capacity = 0;
			public int Capacity
			{
				get { return _capacity; }
				set
				{
					if (_capacity != value)
					{
						// 変更された時のみセット&通知
						_capacity = value;
						OnCapacityChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// Box総数がオーバーした分も含む合計数
			/// </summary>
			public event EventHandler OnTotalCapacityChange = (sender, e) => { };
			private int _totalCapacity = 0;
			public int TotalCapacity
			{
				get { return _totalCapacity; }
				set
				{
					if (_totalCapacity != value)
					{
						_totalCapacity = value;

						// 通知
						OnTotalCapacityChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 容量フォーマット
			/// <summary>
			/// 容量表示形式
			/// </summary>
			public event EventHandler OnCountFormatChange = (sender, e) => { };
			private string _countFormat = "";
			public string CountFormat
			{
				get { return _countFormat; }
				set
				{
					if (_countFormat != value)
					{
						_countFormat = value;

						// 通知
						this.OnCountFormatChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 最大容量表示形式
			/// </summary>
			public event EventHandler OnCapacityFormatChange = (sender, e) => { };
			private string _capacityFormat = "";
			public string CapacityFormat
			{
				get { return _capacityFormat; }
				set
				{
					if (_capacityFormat != value)
					{
						_capacityFormat = value;

						// 通知
						this.OnCapacityFormatChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion
		}

		/// <summary>
		/// キャラアイテムリストのページ付スクロールビュークラス
		/// </summary>
		[Serializable]
		public class GUIItemScrollView : PageScrollView<GUICharaItem>
		{
			protected override GUICharaItem Create(GameObject prefab, UnityEngine.Transform parent, int itemIndex)
			{
				var item = GUICharaItem.Create(prefab, parent, itemIndex);
				item.SetState(CharaItem.Controller.ItemStateType.FillEmpty);
				return item;
			}
			protected override void ClearValue(GUICharaItem item)
			{
				item.SetState(CharaItem.Controller.ItemStateType.FillEmpty, null);
			}
		}
	}
}