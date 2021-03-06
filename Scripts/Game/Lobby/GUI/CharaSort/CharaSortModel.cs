/// <summary>
/// キャラソートデータ
/// 
/// 2016/02/17
/// </summary>
using System;

namespace XUI
{
	namespace CharaSort
	{
		/// <summary>
		/// キャラソートデータインターフェイス
		/// </summary>
		public interface IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();
			#endregion

			#region 選択不可
			/// <summary>
			/// 選択しているものは表示不可にするかどうか
			/// </summary>
			event EventHandler OnIsSelectDisableChange;
			bool IsSelectDisable { get; set; }
			#endregion

			#region 昇順/降順
			/// <summary>
			/// 昇順か降順で並び替えるか
			/// </summary>
			event EventHandler OnIsAscendChange;
			bool IsAscend { get; set; }
			#endregion
		}

		/// <summary>
		/// キャラソートデータ
		/// </summary>
		public class Model : IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				this.OnIsSelectDisableChange = null;
				this.OnIsAscendChange = null;
			}
			#endregion

			#region 選択不可
			/// <summary>
			/// 選択しているものは表示不可にするかどうか
			/// </summary>
			public event EventHandler OnIsSelectDisableChange = (sender, e) => { };
			private bool _isSelectDisable = false;
			public bool IsSelectDisable
			{
				get { return _isSelectDisable; }
				set
				{
					if (_isSelectDisable != value)
					{
						this._isSelectDisable = value;
						// 通知
						this.OnIsSelectDisableChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 昇順/降順
			/// <summary>
			/// 昇順か降順で並び替えるか
			/// </summary>
			public event EventHandler OnIsAscendChange = (sender, e) => { };
			private bool _isAscend = true;
			public bool IsAscend
			{
				get { return _isAscend; }
				set
				{
					if(_isAscend != value)
					{
						_isAscend = value;
						// 通知
						this.OnIsAscendChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion
		}
	}
}