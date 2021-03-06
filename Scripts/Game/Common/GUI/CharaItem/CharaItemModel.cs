/// <summary>
/// キャラアイテムデータ
/// 
/// 2016/01/07
/// </summary>
using System;
using UnityEngine;

namespace XUI
{
	namespace CharaItem
	{
		#region ランク色クラス
		/// <summary>
		/// 材料ランク色クラス
		/// </summary>
		[System.Serializable]
		public class RankColor
		{
			[SerializeField]
			private Color _spriteColor = Color.white;
			public Color SpriteColor { get { return _spriteColor; } }
			[SerializeField]
			private Color _gradientTop = Color.white;
			public Color GradientTop { get { return _gradientTop; } }
			[SerializeField]
			private Color _gradientBottom = Color.white;
			public Color GradientBottom { get { return _gradientBottom; } }

			public RankColor(Color spriteColor, Color gradientTop, Color gradientBottom)
			{
				this._spriteColor = spriteColor;
				this._gradientTop = gradientTop;
				this._gradientBottom = gradientBottom;
			}
		}
		#endregion

		/// <summary>
		/// キャラアイテムデータインターフェイス
		/// </summary>
		public interface IModel
		{
			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();

			/// <summary>
			/// キャラ情報
			/// </summary>
			event EventHandler OnSetCharaInfoChange;
			CharaInfo CharaInfo { get; set; }

			/// <summary>
			/// ボタン有効フラグ
			/// </summary>
			event EventHandler OnButtonEnableChange;
			bool IsButtonEnable { get; set; }

			/// <summary>
			/// アイテムのインデックス値
			/// </summary>
			event EventHandler OnIndexChange;
			int Index { get; set; }

			/// <summary>
			/// パラメータ表示フォーマット
			/// </summary>
			event EventHandler OnParameterFormatChange;
			string ParameterFormat { get; set; }

			/// <summary>
			/// レベル最大時フォーマット
			/// </summary>
			event EventHandler OnLvMaxFormatChange;
			string LvMaxFormat {get; set;}

			/// <summary>
			/// レベル最大時カラー
			/// </summary>
			event EventHandler OnLvMaxColorChange;
			Color LvMaxColor { get; set; }

			/// <summary>
			/// ランク色
			/// </summary>
			event EventHandler OnRankColorChange;
			RankColor RankColor { get; set; }

			/// <summary>
			/// 高ランク時の色
			/// </summary>
			event EventHandler OnHeightRankColorChange;
			RankColor HeightRankColor { get; set; }

			/// <summary>
			/// 材料ランク
			/// </summary>
			event EventHandler OnMaterialRankChange;
			int MaterialRank { get; set; }

			/// <summary>
			/// 素材ランク色
			/// </summary>
			event EventHandler OnMaterialRankColorChange;
			RankColor MaterialRankColor { get; set; }

			/// <summary>
			/// 素材高ランク時の色
			/// </summary>
			event EventHandler OnMaterialHeightRankColorChange;
			RankColor MaterialHeightRankColor { get; set; }

			/// <summary>
			/// 選択フラグ
			/// </summary>
			event EventHandler OnSelectChange;
			bool IsSelect { get; set; }
		}

		/// <summary>
		/// キャラ情報データ
		/// </summary>
		public class Model : IModel
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				this.OnSetCharaInfoChange = null;
				this.OnButtonEnableChange = null;
				this.OnIndexChange = null;
				this.OnParameterFormatChange = null;
				this.OnMaterialRankChange = null;
				this.OnRankColorChange = null;
				this.OnHeightRankColorChange = null;
				this.OnMaterialRankColorChange = null;
				this.OnMaterialHeightRankColorChange = null;
				this.OnSelectChange = null;
			}
			#endregion

			#region CharaInfo
			public event EventHandler OnSetCharaInfoChange = (sender, e) => { };
			CharaInfo _charaInfo = null;
			public CharaInfo CharaInfo
			{
				get { return this._charaInfo; }
				set
				{
					if(value == null)
					{
						if(this._charaInfo != value)
						{
							// NULLセット
							this._charaInfo = value;

							// 通知
							this.OnSetCharaInfoChange(this, EventArgs.Empty);
						}
					}
					else
					{
						if(this._charaInfo == null)
						{
							// 保持しているキャラ情報がNULLなのでセット
							this._charaInfo = value;

							// 通知
							this.OnSetCharaInfoChange(this, EventArgs.Empty);
						}
						else if(CanCharaInfoChange(value))
						{
							// キャラ情報の中身に変更があったのセット
							this._charaInfo = value;

							// 通知
							this.OnSetCharaInfoChange(this, EventArgs.Empty);
						}
					}
				}
			}

			/// <summary>
			/// キャラ情報に変化があったかどうか
			/// </summary>
			private bool CanCharaInfoChange(CharaInfo charaInfo)
			{
				if(/*this._charaInfo != charaInfo ||*/					// 現状チェックを外しても問題ない 当初は必要だったが、合成周りの設計変更でチェックする必要がなくなったと思われる
					this._charaInfo.Equals(charaInfo))
				{
					return true;
				}

				return false;
			}
			#endregion

			#region ボタン有効
			/// <summary>
			/// ボタン有効フラグ
			/// </summary>
			public event EventHandler OnButtonEnableChange = (sender, e) => { };
			private bool _isButtonEnable = true;
			public bool IsButtonEnable
			{
				get { return this._isButtonEnable; }
				set
				{
					if (this._isButtonEnable == value) { return; }
					this._isButtonEnable = value;

					// 通知
					this.OnButtonEnableChange(this, EventArgs.Empty);
				}
			}
			#endregion

			#region アイテムインデックス値
			/// <summary>
			/// アイテムのインデックス値
			/// </summary>
			public event EventHandler OnIndexChange = (sender, e) => { };
			private int index = -1;
			public int Index
			{
				get { return this.index; }
				set
				{
					if (this.index == value) { return; }
					this.index = value;

					// 通知
					this.OnIndexChange(this, EventArgs.Empty);
				}
			}
			#endregion

			#region パラメータ表示フォーマット
			/// <summary>
			/// パラメータ表示フォーマット
			/// </summary>
			public event EventHandler  OnParameterFormatChange = (sender, e) => { };
			private string _parameterFormat = "";
			public string ParameterFormat
			{
				get { return _parameterFormat; }
				set
				{
					if(_parameterFormat != value)
					{
						_parameterFormat = value;

						// 通知
						this.OnParameterFormatChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// レベル最大時フォーマット
			/// </summary>
			public event EventHandler OnLvMaxFormatChange = (sender, e) => { };
			private string _lvMaxFormat = "";
			public string LvMaxFormat
			{
				get { return _lvMaxFormat; }
				set
				{
					if(_lvMaxFormat != value)
					{
						_lvMaxFormat = value;

						// 通知
						this.OnLvMaxFormatChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// レベル最大時カラー
			/// </summary>
			public event EventHandler OnLvMaxColorChange = (sender, e) => { };
			private Color _lvMaxColor = Color.red;
			public Color LvMaxColor
			{
				get { return _lvMaxColor; }
				set
				{
					if(_lvMaxColor != value)
					{
						_lvMaxColor = value;

						// 通知
						this.OnLvMaxColorChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region ランク色
			/// <summary>
			/// ランク色
			/// </summary>
			public event EventHandler OnRankColorChange = (sender, e) => { };
			private RankColor _rankColor = null;
			public RankColor RankColor
			{
				get { return _rankColor; }
				set
				{
					if (_rankColor != value)
					{
						_rankColor = value;

						// 通知
						this.OnRankColorChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 高ランク時の色
			/// </summary>
			public event EventHandler OnHeightRankColorChange = (sender, e) => { };
			private RankColor _heightRankColor = null;
			public RankColor HeightRankColor
			{
				get { return _heightRankColor; }
				set
				{
					if (_heightRankColor != value)
					{
						_heightRankColor = value;

						// 通知
						this.OnHeightRankColorChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 材料ランク
			/// <summary>
			/// 材料ランク
			/// </summary>
			public event EventHandler OnMaterialRankChange = (sender, e) => { };
			private int _materialRank = 0;
			public int MaterialRank
			{
				get { return _materialRank; }
				set
				{
					if(_materialRank != value)
					{
						_materialRank = value;

						// 通知
						this.OnMaterialRankChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 素材ランク色
			/// </summary>
			public event EventHandler OnMaterialRankColorChange = (sender, e) => { };
			private RankColor _materialRankColor = null;
			public RankColor MaterialRankColor
			{
				get { return _materialRankColor; }
				set
				{
					if(_materialRankColor != value)
					{
						_materialRankColor = value;

						// 通知
						this.OnMaterialRankColorChange(this, EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// 素材高ランク時の色
			/// </summary>
			public event EventHandler OnMaterialHeightRankColorChange = (sender, e) => { };
			private RankColor _materialHeightRankColor = null;
			public RankColor MaterialHeightRankColor
			{
				get { return _materialHeightRankColor; }
				set
				{
					if(_materialHeightRankColor != value)
					{
						_materialHeightRankColor = value;

						// 通知
						this.OnMaterialHeightRankColorChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion

			#region 選択フラグ
			/// <summary>
			/// 選択フラグ
			/// </summary>
			public event EventHandler OnSelectChange = (sender, e) => { };
			private bool _isSelect = false;
			public bool IsSelect
			{
				get { return _isSelect; }
				set
				{
					if(_isSelect != value)
					{
						_isSelect = value;

						// 通知
						this.OnSelectChange(this, EventArgs.Empty);
					}
				}
			}
			#endregion
		}
	}
}
