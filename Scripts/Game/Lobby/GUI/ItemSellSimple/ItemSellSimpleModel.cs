/// <summary>
/// アイテム売却データ
/// 
/// 2016/04/08
/// </summary>
using System;

namespace XUI.ItemSellSimple
{
	/// <summary>
	/// アイテム売却データインターフェイス
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

		#region 所持数
		/// <summary>
		/// 所持アイテム数変更イベント
		/// </summary>
		event EventHandler OnHaveItemCountChange;
		/// <summary>
		/// 所持アイテム数
		/// </summary>
		int HaveItemCount { get; set; }
		#endregion

		#region 売却数
		/// <summary>
		/// 売却数変更イベント
		/// </summary>
		event EventHandler OnSellItemCountChange;
		/// <summary>
		/// 売却数
		/// </summary>
		int SellItemCount { get; set; }
		#endregion

		#region 売却額
		/// <summary>
		/// 売却額変更イベント
		/// </summary>
		event EventHandler OnSoldPriceChange;
		/// <summary>
		/// 売却額
		/// </summary>
		int SoldPrice { get; set; }

		/// <summary>
		/// 売却額フォーマット変更イベント
		/// </summary>
		event EventHandler OnSoldPriceFormatChange;
		/// <summary>
		/// 売却額フォーマット
		/// </summary>
		string SoldPriceFormat { get; set; }
		#endregion

		#region アイテム名
		/// <summary>
		/// アイテム名変更イベント
		/// </summary>
		event EventHandler OnItemNameChange;
		/// <summary>
		/// アイテム名
		/// </summary>
		string ItemName { get; set; }
		#endregion
	}

	/// <summary>
	/// アイテム売却データ
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
			this.OnHaveItemCountChange = null;
			this.OnSellItemCountChange = null;
			this.OnSoldPriceChange = null;
			this.OnSoldPriceFormatChange = null;
			this.OnItemNameChange = null;
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

		#region 所持数
		/// <summary>
		/// 所持アイテム数変更イベント
		/// </summary>
		public event EventHandler OnHaveItemCountChange = (sender, e) => { };
		/// <summary>
		/// 所持アイテム数
		/// </summary>
		private int _haveItemCount = 0;
		public int HaveItemCount
		{
			get { return _haveItemCount; }
			set
			{
				if(_haveItemCount != value)
				{
					_haveItemCount = value;

					// 通知
					this.OnHaveItemCountChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 売却数
		/// <summary>
		/// 売却数変更イベント
		/// </summary>
		public event EventHandler OnSellItemCountChange = (sender, e) => { };
		/// <summary>
		/// 売却数
		/// </summary>
		private int _sellItemCount = 0;
		public int SellItemCount
		{
			get { return _sellItemCount; }
			set
			{
				if(_sellItemCount != value)
				{
					_sellItemCount = value;

					// 通知
					this.OnSellItemCountChange(this, EventArgs.Empty);
				}
			}
		}
		#endregion

		#region 売却額
		/// <summary>
		/// 売却額変更イベント
		/// </summary>
		public event EventHandler OnSoldPriceChange = (sender, e) => { };
		/// <summary>
		/// 売却額
		/// </summary>
		private int _soldPrice = 0;
		public int SoldPrice
		{
			get { return _soldPrice; }
			set
			{
				if(_soldPrice != value)
				{
					_soldPrice = value;

					// 通知
					this.OnSoldPriceChange(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 売却額フォーマット変更イベント
		/// </summary>
		public event EventHandler OnSoldPriceFormatChange = (sender, e) => { };
		/// <summary>
		/// 売却額フォーマット
		/// </summary>
		private string _soldPriceFormat = string.Empty;
		public string SoldPriceFormat
		{
			get { return _soldPriceFormat; }
			set
			{
				if(_soldPriceFormat != value)
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
		private string _itemName = string.Empty;
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
	}
}