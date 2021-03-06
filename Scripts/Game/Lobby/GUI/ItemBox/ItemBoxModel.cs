/// <summary>
/// アイテムBOXデータ
/// 
/// 2016/03/28
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace XUI.ItemBox
{
	/// <summary>
	/// アイテムBOXデータインターフェイス
	/// </summary>
	public interface IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金変更イベント
		/// </summary>
		event EventHandler OnHaveMoneyChange;
		/// <summary>
		/// 所持金
		/// </summary>
		int HaveMoney { get; set; }

		/// <summary>
		/// 所持金フォーマット変更イベント
		/// </summary>
		event EventHandler OnHaveMoneyFormatChange;
		/// <summary>
		/// 所持金フォーマット
		/// </summary>
		string HaveMoneyFormat { get; set; }
		#endregion

		#region 総売却額
		/// <summary>
		/// 総売却額変更イベント
		/// </summary>
		event EventHandler OnTotalSoldPriceChange;
		int TotalSoldPrice { get; set; }
		
		/// <summary>
		/// 総売却額フォーマット変更イベント
		/// </summary>
		event EventHandler OnTotalSoldPriceFormatChange;
		string TotalSoldPriceFormat { get; set; }
		#endregion

		#region アイテムの売却額
		/// <summary>
		/// アイテム売却額
		/// </summary>
		event EventHandler OnSoldPriceChange;
		int SoldPrice { get; set; }

		/// <summary>
		/// アイテム売却額フォーマット
		/// </summary>
		event EventHandler OnSoldPriceFormatChange;
		string SoldPriceFormat { get; set; }
		#endregion

		#region アイテム名
		/// <summary>
		/// アイテム名
		/// </summary>
		event EventHandler OnItemNameChange;
		string ItemName { get; set; }
		#endregion

		#region アイテム個数
		/// <summary>
		/// アイテム個数
		/// </summary>
		event EventHandler OnItemCountChange;
		int ItemCount { get; set; }
		#endregion

		#region アイテム説明
		/// <summary>
		/// アイテム説明
		/// </summary>
		event EventHandler OnDescriptionChange;
		string Description { get; set; }
		#endregion

		#region アイテムロックフラグ
		/// <summary>
		/// アイテムロックフラグ
		/// </summary>
		event EventHandler OnLockChange;
		bool IsLock { get; set; }
		#endregion

		#region アイテム情報リスト
		/// <summary>
		/// アイテム情報リストのセット
		/// </summary>
		event EventHandler OnItemInfoListChange;
		void SetItemInfoList(List<ItemInfo> itemInfoList);

		/// <summary>
		/// アイテム情報リストクリア
		/// </summary>
		event EventHandler OnClearItemInfoList;
		void ClearItemInfoList();

		/// <summary>
		/// アイテム情報一覧を取得
		/// </summary>
		List<ItemInfo> GetItemInfoList();
		
		/// <summary>
		/// アイテムマスタID指定で関連するアイテム情報一覧を取得
		/// </summary>
		bool TryGetItemInfoByMasterId(int itemMasterId, out Dictionary<int, ItemInfo> indexIdDic);

		/// <summary>
		/// 指定するIDとIndexからアイテム情報を取得する
		/// </summary>
		bool TryGetItemInfo(int itemMasterId, int index, out ItemInfo itemInfo);
		#endregion
	}

	/// <summary>
	/// アイテムBOXデータ
	/// </summary>
	public class Model : IModel
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			this.OnHaveMoneyChange = null;
			this.OnHaveMoneyFormatChange = null;
			this.OnTotalSoldPriceChange = null;
			this.OnTotalSoldPriceFormatChange = null;
			this.OnSoldPriceChange = null;
			this.OnItemNameChange = null;
			this.OnItemCountChange = null;
			this.OnDescriptionChange = null;
			this.OnLockChange = null;
			this.OnItemInfoListChange = null;
			this.OnClearItemInfoList = null;
		}
		#endregion

		#region 所持金
		/// <summary>
		/// 所持金変更イベント
		/// </summary>
		public event EventHandler OnHaveMoneyChange = (sender, e) => { };
		/// <summary>
		/// 所持金
		/// </summary>
		private int _haveMoney = 0;
		public int HaveMoney
		{
			get { return _haveMoney; }
			set
			{
				if (_haveMoney != value)
				{
					_haveMoney = value;

					// 通知
					this.OnHaveMoneyChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 所持金フォーマット変更イベント
		/// </summary>
		public event EventHandler OnHaveMoneyFormatChange = (sender, e) => { };
		/// <summary>
		/// 所持金フォーマット
		/// </summary>
		private string _haveMoneyFormat = "";
		public string HaveMoneyFormat
		{
			get { return _haveMoneyFormat; }
			set
			{
				if (_haveMoneyFormat != value)
				{
					_haveMoneyFormat = value;

					// 通知
					this.OnHaveMoneyFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 総売却額
		/// <summary>
		/// 総売却額変更イベント
		/// </summary>
		public event EventHandler OnTotalSoldPriceChange = (sender, e) => { };
		/// <summary>
		/// 総売却額
		/// </summary>
		private int _totalSoldPrice = 0;
		public int TotalSoldPrice
		{
			get { return _totalSoldPrice; }
			set
			{
				if (_totalSoldPrice != value)
				{
					_totalSoldPrice = value;

					// 通知
					this.OnTotalSoldPriceChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 総売却額フォーマット変更イベント
		/// </summary>
		public event EventHandler OnTotalSoldPriceFormatChange = (sender, e) => { };
		/// <summary>
		/// 総売却額フォーマット
		/// </summary>
		private string _totalSoldPriceFormat = "";
		public string TotalSoldPriceFormat
		{
			get { return _totalSoldPriceFormat; }
			set
			{
				if (_totalSoldPriceFormat != value)
				{
					_totalSoldPriceFormat = value;

					// 通知
					this.OnTotalSoldPriceFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region アイテムの売却額
		/// <summary>
		/// アイテム売却額
		/// </summary>
		public event EventHandler OnSoldPriceChange = (sender, e) => { };
		/// <summary>
		/// アイテム売却額
		/// </summary>
		private int _soldPrice = 0;
		public int SoldPrice
		{
			get { return _soldPrice; }
			set
			{
				if (_soldPrice != value)
				{
					_soldPrice = value;

					// 通知
					this.OnSoldPriceChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// アイテム売却額フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSoldPriceFormatChange = (sender, e) => { };
		/// <summary>
		/// アイテム売却額フォーマット
		/// </summary>
		private string _soldPriceFormat = "";
		public string SoldPriceFormat
		{
			get { return _soldPriceFormat; }
			set
			{
				if (_soldPriceFormat != value)
				{
					_soldPriceFormat = value;

					// 通知
					this.OnSoldPriceFormatChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region アイテム名
		/// <summary>
		/// アイテム名変更イベント
		/// </summary>
		public event EventHandler OnItemNameChange = (sender, e) => { };
		/// <summary>
		/// アイテム名
		/// </summary>
		private string _itemName = "";
		public string ItemName
		{
			get { return _itemName; }
			set
			{
				if(_itemName != value)
				{
					_itemName = value;

					// 通知
					this.OnItemNameChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region アイテム個数
		/// <summary>
		/// アイテム個数変更イベント
		/// </summary>
		public event EventHandler OnItemCountChange = (sender, e) => { };
		/// <summary>
		/// アイテム個数
		/// </summary>
		private int _itemCount = 0;
		public int ItemCount
		{
			get { return _itemCount; }
			set
			{
				if(_itemCount != value)
				{
					_itemCount = value;

					// 通知
					this.OnItemCountChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region アイテム説明
		/// <summary>
		/// アイテム説明変更イベント
		/// </summary>
		public event EventHandler OnDescriptionChange = (sender, e) => { };
		/// <summary>
		/// アイテム説明
		/// </summary>
		private string _description = "";
		public string Description
		{
			get { return _description; }
			set
			{
				if(_description != value)
				{
					_description = value;

					// 通知
					this.OnDescriptionChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region アイテムロックフラグ
		/// <summary>
		/// アイテムロックフラグ変更イベント
		/// </summary>
		public event EventHandler OnLockChange = (sender, e) => { };
		/// <summary>
		/// アイテムロックフラグ
		/// </summary>
		private bool _isLock = false;
		public bool IsLock
		{
			get { return _isLock; }
			set
			{
				if(_isLock != value)
				{
					_isLock = value;

					// 通知
					this.OnLockChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region アイテム情報リスト
		/// <summary>
		/// アイテム情報をアイテムマスタIDで区分けした一覧
		/// Key=ItemMasterID
		/// Value = インデックスで区分けした一覧
		/// </summary>
		private Dictionary<int, Dictionary<int, ItemInfo>> itemInfoDic = new Dictionary<int,Dictionary<int,ItemInfo>>();
		
		/// <summary>
		/// アイテム情報リストのセット
		/// </summary>
		public event EventHandler OnItemInfoListChange = (sender, e) => { };
		public void SetItemInfoList(List<ItemInfo> infoList)
		{
			this.itemInfoDic.Clear();
			foreach (var info in infoList)
			{
				if (!this.itemInfoDic.ContainsKey(info.ItemMasterID))
				{
					this.itemInfoDic.Add(info.ItemMasterID, new Dictionary<int, ItemInfo>());
				}

				Dictionary<int, ItemInfo> indexIdItemInfoDic;
				if (this.itemInfoDic.TryGetValue(info.ItemMasterID, out indexIdItemInfoDic))
				{
					if (!indexIdItemInfoDic.ContainsKey(info.Index))
					{
						indexIdItemInfoDic.Add(info.Index, info);
					}
				}
			}

			// 通知
			OnItemInfoListChange(this, EventArgs.Empty);
		}

		/// <summary>
		/// アイテム情報リストのクリア
		/// </summary>
		public event EventHandler OnClearItemInfoList = (sender, e) => { };
		public void ClearItemInfoList()
		{
			if(this.itemInfoDic.Count > 0)
			{
				this.itemInfoDic.Clear();

				// 通知
				OnClearItemInfoList(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// アイテム情報一覧を取得
		/// </summary>
		public List<ItemInfo> GetItemInfoList()
		{
			var infoList = new List<ItemInfo>();
			foreach(var indexIdDic in this.itemInfoDic.Values)
			{
				foreach(var info in indexIdDic.Values)
				{
					infoList.Add(info);
				}
			}

			return infoList;
		}

		/// <summary>
		/// アイテムマスタID指定で関連するアイテム情報一覧を取得
		/// </summary>
		public bool TryGetItemInfoByMasterId(int itemMasterId, out Dictionary<int, ItemInfo> indexIdDic)
		{
			return this.itemInfoDic.TryGetValue(itemMasterId, out indexIdDic);
		}

		/// <summary>
		/// 指定するIDとIndexからアイテム情報を取得する
		/// </summary>
		public bool TryGetItemInfo(int itemMasterId, int index, out ItemInfo itemInfo)
		{
			itemInfo = null;
			Dictionary<int, ItemInfo> indexItemInfoDic = new Dictionary<int, ItemInfo>();
			if(!this.TryGetItemInfoByMasterId(itemMasterId, out indexItemInfoDic))
			{
				// IDが見つからない
				return false;
			}
			if(!indexItemInfoDic.TryGetValue(index, out itemInfo))
			{
				// Indexが見つからない
				return false;
			}

			return true;
		}
		#endregion
	}
}
