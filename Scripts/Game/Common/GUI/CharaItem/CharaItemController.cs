/// <summary>
/// キャラアイテム制御
/// 
/// 2016/01/08
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace CharaItem
	{
		/// <summary>
		/// キャラアイテム制御インターフェイス
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
			/// 状態をセットする
			/// </summary>
			/// <param name="mode"></param>
			void SetItemState(Controller.ItemStateType state);

			/// <summary>
			/// 状態を取得する
			/// </summary>
			/// <returns></returns>
			Controller.ItemStateType GetItemState();

			/// <summary>
			/// アイテムの状態
			/// </summary>
			Controller.DisableType DisableState { get; }

			/// <summary>
			/// 無効状態をセットする
			/// </summary>
			/// <param name="state"></param>
			void SetDisableState(Controller.DisableType state);

			/// <summary>
			/// 素材状態をセットする
			/// </summary>
			/// <param name="state"></param>
			void SetBaitState(int baitIndex);

			/// <summary>
			/// 指定されたランクとキャラアイコンのランクと比べてランク色を設定する
			/// </summary>
			void SetRankColor(int materialRankRank);

			/// <summary>
			/// 材料ランク色をセットする
			/// </summary>
			void SetMaterialRankColor(int rank);

			/// <summary>
			/// 所有状態設定
			/// </summary>
			/// <param name="state"></param>
			void SetPossessionState(Controller.PossessionStateType state);

			/// <summary>
			/// 所有状態取得
			/// </summary>
			Controller.PossessionStateType GetPossessionState();
		}

		/// <summary>
		/// キャラアイテム制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド&プロパティ
			/// <summary>
			/// モデル
			/// </summary>
			private readonly CharaItem.IModel _model;
			private CharaItem.IModel Model { get { return _model; } }

			/// <summary>
			/// ビュー
			/// </summary>
			private readonly IView _view;
			private IView View { get { return _view; } }

			/// <summary>
			/// キャラアイコン
			/// </summary>
			private CharaIcon CharaIcon { get; set; }

			/// <summary>
			/// アイテムの状態
			/// </summary>
			private ItemStateType _state = ItemStateType.Empty;
			private ItemStateType State { get { return _state; } set { _state = value; } }
			public enum ItemStateType
			{
				Empty,		// 空
				FillEmpty,	// 空表記なし
				Frame,		// フレームのみ
				Icon,		// アイコン
				Material,	// 材料アイコン		(廃止予定 Monoタイプに変更)
				Mono,		// モノクロアイコン
				Loading,	// 読み込み
				Exist,		// キャラ存在
			}

			/// <summary>
			/// 非選択状態
			/// </summary>
			private DisableType _disableState = DisableType.None;
			public DisableType DisableState { get { return _disableState; } private set { _disableState = value; } }
			public enum DisableType
			{
				None,					// 設定なし
				Base,					// ベースキャラ
				Bait,					// 餌キャラ
				Material,				// 素材キャラ
				PowerupSlot,			// 強化スロット装着中
				Deck,					// デッキに編成中
				Symbol,					// シンボルキャラ
				PowerupLevelMax,		// 強化済
				PowerupLevelShortage,	// 強化レベル不足
				RankShortage,			// ランク不足
				Lock,					// ロック
				NotSelected,			// 非選択
				Select,					// 選択
			}

			/// <summary>
			/// 所有状態
			/// </summary>
			private PossessionStateType _possessionState = PossessionStateType.None;
			public PossessionStateType PossessionState { get { return _possessionState; } private set { _possessionState = value; } }
			public enum PossessionStateType
			{
				None,				// 設定なし
				Possession,			// 所有
				NotPossession,		// 未所持
			}

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
			/// <param name="model"></param>
			/// <param name="view"></param>
			/// <param name="charaIcon"></param>
			public Controller(CharaItem.IModel model, IView view, CharaIcon charaIcon)
			{
				if (model == null || view == null) { return; }

				// ビュー設定
				this._view = view;

				// モデル設定
				this._model = model;
				this.Model.OnButtonEnableChange += this.HandleOnButtonEnableChange;
				this.Model.OnParameterFormatChange += this.HandleOnParameterFormatChange;
				this.Model.OnLvMaxFormatChange += this.HandleLvMaxFormatChange;
				this.Model.OnLvMaxColorChange += this.HandleLvMaxColorChange;
				this.Model.OnMaterialRankChange += this.HandleOnMaterialRankChange;
				this.Model.OnSelectChange += this.HandleOnSelectChange;
				this.Model.OnRankColorChange += this.HandleOnRankColorChange;
				this.Model.OnHeightRankColorChange += this.HandleOnHeightRankColorChange;
				this.Model.OnMaterialRankColorChange += this.HandleOnMaterialRankChange;
				this.Model.OnMaterialHeightRankColorChange += this.HandleOnMaterialHeightRankColorChange;

				// キャラアイコン設定
				this.CharaIcon = charaIcon;

				// 同期
				SyncParameterFormat();
				SyncRankColor(0);
				SyncMaterialRank();
				SyncSelect();
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
			#endregion

			#region アイテム状態
			/// <summary>
			/// アイテムの状態をセットする
			/// </summary>
			/// <param name="state"></param>
			public void SetItemState(ItemStateType state)
			{
				switch(state)
				{
					case ItemStateType.Frame:
						// フレーム状態に切替
						SetFrameState();
						break;
					case ItemStateType.Empty:
						// 空アイテムに切り替え
						SetEmptyState();
						break;
					case ItemStateType.FillEmpty:
						// 空表記なし状態に切替
						SetFillEmptyState();
						break;
					case ItemStateType.Icon:
					case ItemStateType.Material:
					case ItemStateType.Mono:
					case ItemStateType.Loading:
						// ロード状態に切り替え
						SetLoadState(state);
						break;
					case ItemStateType.Exist:
						// キャラ存在アイコン状態に切替え
						SetExistState();
						break;
				}
			}

			/// <summary>
			/// 読み込みアイコン状態セット
			/// </summary>
			/// <param name="nextState"></param>
			private void SetLoadState(ItemStateType nextState)
			{
				if (!CanUpdate) { return; }
				this.State = ItemStateType.Loading;

				this.View.SetIconActive(true);
				this.View.SetCharaIconActive(false);
				this.View.SetEmptyActive(false);
				this.View.SetExistActive(false);
				this.View.SetLoadingActive(true);

				// アイコン読み込み
				switch (nextState)
				{
					case ItemStateType.Icon:
						LoadCharaIcon();
						break;
					case ItemStateType.Material:
						LoadMaterialIcon();
						break;
					case ItemStateType.Mono:
						LoadMonoIcon();
						break;
				}
			}

			/// <summary>
			/// フレーム状態セット
			/// </summary>
			private void SetFrameState()
			{
				if (!CanUpdate) { return; }
				this.State = ItemStateType.Frame;

				this.View.SetIconActive(false);
			}

			/// <summary>
			/// 空アイコン状態セット
			/// </summary>
			private void SetEmptyState()
			{
				if (!CanUpdate) { return; }
				this.State = ItemStateType.Empty;

				this.View.SetIconActive(true);
				this.View.SetCharaIconActive(false);
				this.View.SetEmptyActive(true);
				this.View.SetLoadingActive(false);
				this.View.SetExistActive(false);
			}

			/// <summary>
			/// 空表記なし状態セット
			/// </summary>
			public void SetFillEmptyState()
			{
				if (!CanUpdate) { return; }
				this.State = ItemStateType.FillEmpty;

				this.View.SetIconActive(true);
				this.View.SetCharaIconActive(false);
				this.View.SetEmptyActive(false);
				this.View.SetLoadingActive(false);
				this.View.SetExistActive(false);
			}

			/// <summary>
			/// キャラアイコンの読み込み処理
			/// </summary>
			private void LoadCharaIcon()
			{
				this.CharaIcon.GetIcon(this.Model.CharaInfo.AvatarType, this.Model.CharaInfo.SkinId, false,
						(atlas, spriteName) =>
						{
							if (atlas != null && !string.IsNullOrEmpty(spriteName))
							{
								// アイコンセット
								this.View.SetCharaIcon(atlas, spriteName);
								// 読み込み終了したので状態を更新
								SetIconState();
							}
						}
					);
			}
			/// <summary>
			/// キャラアイコン状態セット
			/// </summary>
			private void SetIconState()
			{
				if (!CanUpdate) { return; }
				this.State = ItemStateType.Icon;

				this.View.SetIconActive(true);
				this.View.SetCharaIconActive(true);
				this.View.SetLoadingActive(false);
				this.View.SetExistActive(false);
				if (this.Model.CharaInfo != null)
				{
					//CharaInfo info = this.Model.CharaInfo;
					//this.View.SetRankActive(true, info.Rank.ToString());
					//this.View.SetLockActive(info.IsLock);
					//this.View.SetNew(info.IsNew);
					// 売り切り版には無いため非表示
					this.View.SetRankActive(false, "");
					this.View.SetLockActive(false);
					this.View.SetNew(false);

					// レベル設定
					this.SyncParameterFormat();
				}
			}

			/// <summary>
			/// 素材キャラアイコンの読み込み処理
			/// </summary>
			private void LoadMaterialIcon()
			{
				this.CharaIcon.GetIcon(this.Model.CharaInfo.AvatarType, this.Model.CharaInfo.SkinId, false,
						(atlas, spriteName) =>
						{
							if (atlas != null && !string.IsNullOrEmpty(spriteName))
							{
								// アイコンセット
								this.View.SetCharaIcon(atlas, spriteName);
								// 読み込み終了したので状態を更新
								SetMaterialState();
							}
						}
					);
			}
			/// <summary>
			/// 素材アイコン状態セット
			/// </summary>
			private void SetMaterialState()
			{
				if (!CanUpdate) { return; }
				this.State = ItemStateType.Material;

				this.View.SetIconActive(true);
				this.View.SetCharaIconActive(true);
				this.View.SetLoadingActive(false);
				this.View.SetRankActive(false, "");
				this.View.SetParameterActive(false, string.Empty, Color.white);
				this.View.SetLockActive(false);
				this.View.SetNew(false);
			}

			/// <summary>
			/// モノクロアイコンの読み込み処理
			/// </summary>
			private void LoadMonoIcon()
			{
				this.CharaIcon.GetMonoIcon(this.Model.CharaInfo.AvatarType, this.Model.CharaInfo.SkinId, false,
						(atlas, spriteName) =>
						{
							if (atlas != null && !string.IsNullOrEmpty(spriteName))
							{
								// アイコンセット
								this.View.SetCharaIcon(atlas, spriteName);
								// 読み込み終了したので状態を更新
								SetMonoState();
							}
						}
					);
			}
			/// <summary>
			/// モノクロアイコン状態セット
			/// </summary>
			private void SetMonoState()
			{
				if (!CanUpdate) { return; }
				this.State = ItemStateType.Mono;

				this.View.SetIconActive(true);
				this.View.SetCharaIconActive(true);
				this.View.SetLoadingActive(false);
				this.View.SetRankActive(false, "");
				this.View.SetParameterActive(false, string.Empty, Color.white);
				this.View.SetLockActive(false);
				this.View.SetNew(false);
			}

			/// <summary>
			/// キャラ存在アイコン状態セット
			/// </summary>
			private void SetExistState()
			{
				if (!CanUpdate) { return; }
				this.State = ItemStateType.Exist;

				this.View.SetIconActive(true);
				this.View.SetCharaIconActive(false);
				this.View.SetEmptyActive(false);
				this.View.SetLoadingActive(false);
				this.View.SetExistActive(true);
			}

			/// <summary>
			/// 状態を取得する
			/// </summary>
			/// <returns></returns>
			public Controller.ItemStateType GetItemState()
			{
				return this.State;
			}
			#endregion

			#region 無効状態
			/// <summary>
			/// 無効状態をセットする
			/// </summary>
			/// <param name="state"></param>
			public void SetDisableState(DisableType state)
			{
				if (!CanUpdate) { return; }

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
					case DisableType.Material:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX314_CharaList_Select));
						this.View.SetSelectNumberActive(false, string.Empty);
						break;
					}
					case DisableType.PowerupSlot:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX246_CharaList_Slot));
						this.View.SetSelectNumberActive(false, string.Empty);
						break;
					}
					case DisableType.Deck:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX247_CharaList_Deck));
						this.View.SetSelectNumberActive(false, string.Empty);
						break;
					}
					case DisableType.Symbol:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX248_CharaList_Symbol));
						this.View.SetSelectNumberActive(false, string.Empty);
						break;
					}
					case DisableType.PowerupLevelMax:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX249_CharaList_PowerupLevelMax));
						this.View.SetSelectNumberActive(false, string.Empty);
						break;
					}
					case DisableType.PowerupLevelShortage:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX312_CharaList_PowerupLevelShortage));
						this.View.SetSelectNumberActive(false, string.Empty);
						break;
					}
					case DisableType.RankShortage:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX313_CharaList_RankShortage));
						this.View.SetSelectNumberActive(false, string.Empty);
						break;
					}
					case DisableType.Lock:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(true, MasterData.GetText(TextType.TX250_CharaList_Lock));
						this.View.SetSelectNumberActive(false, string.Empty);
						break;
					}
					case DisableType.NotSelected:
					{
						this.View.SetDisableActive(true);
						this.View.SetDisableTextActive(false, string.Empty);
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
			/// <param name="state"></param>
			public void SetBaitState(int baitIndex)
			{
				// 状態変更
				SetDisableState(DisableType.Bait);

				// 表示設定
				this.SetBaitView(true, baitIndex);
			}

			/// <summary>
			/// 餌状態の表示設定
			/// </summary>
			private void SetBaitView(bool isActive, int baitIndex)
			{
				if (this.View == null) { return; }
				string selecText;
				if(isActive)
				{
					if(baitIndex > -1)
					{
						// 番号のテキストを表示
						selecText = (baitIndex + 1).ToString();
					}
					else
					{
						// インデックス値が-1以下なら選択済テキストを表示
						selecText = MasterData.GetText(TextType.TX314_CharaList_Select);
					}
				}
				else
				{
					selecText = string.Empty;
				}

				this.View.SetSelectNumberActive(isActive, selecText);
			}
			#endregion

			#region ボタン有効
			/// <summary>
			/// ボタンの有効フラグに変化があった時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleOnButtonEnableChange(object sender, EventArgs e)
			{
				SyncButtonEnable();
			}

			/// <summary>
			/// ボタンの有効設定同期
			/// </summary>
			private void SyncButtonEnable()
			{
				if (this.View == null) { return; }
				this.View.SetButtonEnable(this.Model.IsButtonEnable);
			}
			#endregion

			#region パラメータフォーマット
			/// <summary>
			/// パラメータフォーマットに変更があった時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleOnParameterFormatChange(object sender, EventArgs e)
			{
				SyncParameterFormat();
			}
			/// <summary>
			/// レベル最大時フォーマットに変更があった時に呼び出される
			/// </summary>
			private void HandleLvMaxFormatChange(object sender, EventArgs e)
			{
				SyncParameterFormat();
			}
			/// <summary>
			/// レベル最大時色に変更があった時に呼び出される
			/// </summary>
			private void HandleLvMaxColorChange(object sender, EventArgs e)
			{
				SyncParameterFormat();
			}
			/// <summary>
			/// パラメータフォーマット同期
			/// </summary>
			private void SyncParameterFormat()
			{
				//if (!CanUpdate) { return; }
				//var info = this.Model.CharaInfo;

				//string text = string.Empty;
				//Color color = Color.white;

				//if (info != null)
				//{
				//	if (CharaInfo.IsMaxLevel(info.Rank, info.PowerupLevel))
				//	{
				//		text = this.Model.LvMaxFormat;
				//		color = this.Model.LvMaxColor;
				//	}
				//	else
				//	{
				//		text = string.Format(this.Model.ParameterFormat, info.PowerupLevel);
				//	}
				//}
				//this.View.SetParameterActive(true, text, color);

				// 売り切り版には無いため非表示
				this.View.SetParameterActive(false, string.Empty, Color.white);
			}
			#endregion

			#region ランク色
			/// <summary>
			/// 指定されたランクとキャラアイコンのランクと比べてランク色を設定する
			/// </summary>
			public void SetRankColor(int materialRank)
			{
				SyncRankColor(materialRank);
			}

			/// <summary>
			/// ランク色に変更があった時に呼び出される
			/// </summary>
			private void HandleOnRankColorChange(object sender, EventArgs e)
			{
				SyncRankColor(0);
			}

			/// <summary>
			/// 高ランク色に変更があった時に呼び出される
			/// </summary>
			private void HandleOnHeightRankColorChange(object sender, EventArgs e)
			{
				SyncRankColor(0);
			}

			/// <summary>
			/// ランク色同期
			/// </summary>
			private void SyncRankColor(int materialRank)
			{
				if (!this.CanUpdate) { return; }

				if (this.Model.CharaInfo != null && this.Model.CharaInfo.Rank > materialRank)
				{
					RankColor heightRankColor = this.Model.HeightRankColor;
					if (heightRankColor != null)
					{
						// 高ランク色セット
						this.View.SetRankColor(heightRankColor.SpriteColor, heightRankColor.GradientTop, heightRankColor.GradientBottom);
					}
				}
				else
				{
					RankColor rankColor = this.Model.RankColor;
					if (rankColor != null)
					{
						// 通常色セット
						this.View.SetRankColor(rankColor.SpriteColor, rankColor.GradientTop, rankColor.GradientBottom);
					}
				}
			}
			#endregion

			#region 材料ランク
			/// <summary>
			/// 材料ランクに変更があった時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleOnMaterialRankChange(object sender, EventArgs e)
			{
				SyncMaterialRank();
			}

			/// <summary>
			/// 材料ランク同期
			/// </summary>
			private void SyncMaterialRank()
			{
				if (!CanUpdate) { return; }

				// 材料ランクセット
				if(this.Model.MaterialRank > 0)
				{
					this.View.SetMaterialRankActive(true, this.Model.MaterialRank.ToString());
				}
				else
				{
					// ランクが0以下だった場合は非表示にする
					this.View.SetMaterialRankActive(false, string.Empty);
				}

				// 色同期
				SyncMaterialRankColor(0);
			}

			/// <summary>
			/// 材料ランク色をセットする
			/// </summary>
			public void SetMaterialRankColor(int rank)
			{
				SyncMaterialRankColor(rank);
			}

			/// <summary>
			/// 材料ランク色に変更があった時に呼び出される
			/// </summary>
			private void HandleOnMaterialRankColorChange(object sender, EventArgs e)
			{
				SyncMaterialRankColor(0);
			}

			/// <summary>
			/// 材料高ランク色に変更があった時に呼び出される
			/// </summary>
			private void HandleOnMaterialHeightRankColorChange(object sender, EventArgs e)
			{
				SyncMaterialRankColor(0);
			}

			/// <summary>
			/// 材料ランク色同期
			/// </summary>
			private void SyncMaterialRankColor(int rank)
			{
				if (!this.CanUpdate) { return; }

				if(this.Model.MaterialRank < rank)
				{
					RankColor heightRankColor = this.Model.MaterialHeightRankColor;
					if (heightRankColor != null)
					{
						// 高ランク色セット
						this.View.SetMaterialRankColor(heightRankColor.SpriteColor, heightRankColor.GradientTop, heightRankColor.GradientBottom);
					}
				}
				else
				{
					RankColor rankColor = this.Model.MaterialRankColor;
					if (rankColor != null)
					{
						// 通常色セット
						this.View.SetMaterialRankColor(rankColor.SpriteColor, rankColor.GradientTop, rankColor.GradientBottom);
					}
				}
			}
			#endregion

			#region 選択
			/// <summary>
			/// 選択フラグに変更があった時に呼び出される
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void HandleOnSelectChange(object sender, EventArgs e)
			{
				SyncSelect();
			}

			/// <summary>
			/// 選択同期
			/// </summary>
			private void SyncSelect()
			{
				if (!CanUpdate) { return; }
				this.View.SetSelectFrameActive(this.Model.IsSelect);
			}
			#endregion

			#region 所有状態
			/// <summary>
			/// 所有状態設定
			/// </summary>
			/// <param name="state"></param>
			public void SetPossessionState(PossessionStateType state)
			{
				if (this.View == null) { return; }

				this.PossessionState = state;
				if(this.PossessionState == PossessionStateType.Possession)
				{
					this.View.SetPossessionActive(true, true);
				}
				else if(this.PossessionState == PossessionStateType.NotPossession)
				{
					this.View.SetPossessionActive(true, false);
				}
				else
				{
					this.View.SetPossessionActive(false, false);
				}
			}

			/// <summary>
			/// 所有状態取得
			/// </summary>
			public Controller.PossessionStateType GetPossessionState()
			{
				return this.PossessionState;
			}
			#endregion
		}
	}
}
