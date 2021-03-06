/// <summary>
/// キャラソート制御
/// 
/// 2016/02/17
/// </summary>
using System;
using System.Collections.Generic;

namespace XUI
{
	namespace CharaSort
	{
		#region イベント引数
		/// <summary>
		/// OKボタンイベント通知時の引数
		/// </summary>
		public class OKClickEventArgs : EventArgs
		{
			private readonly Controller.SortPatternType _sortPattern;
			public Controller.SortPatternType SortPattern { get { return _sortPattern; } }

			private readonly bool _isSelectDisable;
			public bool IsSelectDisable { get { return _isSelectDisable; } }

			private readonly bool _isAscend;
			public bool IsAscend { get { return _isAscend; } }

			public OKClickEventArgs(Controller.SortPatternType sortPattern, bool isSelectDisable, bool isAscend)
			{
				this._sortPattern = sortPattern;
				this._isSelectDisable = isSelectDisable;
				this._isAscend = isAscend;
			}
		}
		#endregion

		/// <summary>
		/// キャラソート制御インターフェイス
		/// </summary>
		public interface IController
		{
			#region モデル/ビュー
			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }
			#endregion

			#region アクティブ設定
			/// <summary>
			/// アクティブを設定する
			/// </summary>
			void SetActive(bool isActive, bool isTweenSkip);
			#endregion

			#region 初期化
			/// <summary>
			/// データセットアップ処理
			/// </summary>
			void Setup(Controller.SortPatternType pattern, bool isSelectDisable, bool isAscend);

			/// <summary>
			/// 破棄
			/// </summary>
			void Dispose();
			#endregion

			#region ソート項目
			/// <summary>
			/// 現在有効状態になっているソート項目
			/// </summary>
			Controller.SortPatternType SortPattern { get; }

			/// <summary>
			/// 現在有効状態になっているソート項目が変化された時の通知
			/// </summary>
			event EventHandler OnSortPatternChangeEvent;
			#endregion

			#region 選択不可
			/// <summary>
			/// 選択しているものは表示不可にするかどうか
			/// </summary>
			bool IsSelectDisable { get; }
			#endregion

			#region 昇順/降順
			/// <summary>
			/// 昇順か降順で並び替えるか
			/// </summary>
			bool IsAscend { get; }
			#endregion

			#region OKボタン
			/// <summary>
			/// OKボタンを押した時のイベント通知
			/// </summary>
			event EventHandler<OKClickEventArgs> OKClickEvent;
			#endregion
		}

		/// <summary>
		/// キャラソート制御
		/// </summary>
		public class Controller : IController
		{
			#region モデル/ビュー
			/// <summary>
			/// モデル
			/// </summary>
			private readonly IModel _model;
			private IModel Model { get { return _model; } }

			/// <summary>
			/// ビュー
			/// </summary>
			private readonly IView _view;
			private IView View { get { return _view; } }

			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			public bool CanUpdate
			{
				get
				{
					if (this.Model == null) { return false; }
					if (this.View == null) { return false; }
					return true;
				}
			}
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public Controller(IView view)
			{
				if (view == null) { return; }

				// ビュー設定
				this._view = view;
				this.View.OnCloseClickEvent += this.HandleCloseClickEvent;
				this.View.OnRankClickEvent += this.HandleRankClickEvent;
				this.View.OnCostClickEvent += this.HandleCostClickEvent;
				this.View.OnLevelClickEvent += this.HandleLevelClickEvent;
				this.View.OnCharaTypeClickEvent += this.HandleCharaTypeClickEvent;
				this.View.OnObtainingClickEvent += this.HandleObtainingClickEvent;
				this.View.OnHitPointClickEvent += this.HandleHitPointClickEvent;
				this.View.OnAttackClickEvent += this.HandleAttackClickEvent;
				this.View.OnDefenseClickEvent += this.HandleDefenseClickEvent;
				this.View.OnExtraClickEvent += this.HandleExtraClickEvent;
				this.View.OnSelectDisableClickEvent += this.HandleSelectDisableClickEvent;
				this.View.OnAscendClickEvent += this.HandleAscendClickEvent;
				this.View.OnDescendClickEvent += this.HandleDescendClickEvent;
				this.View.OnOkClickEvent += this.HandleOkClickEvent;

				// モデル生成
				this._model = new XUI.CharaSort.Model();
				this.Model.OnIsSelectDisableChange += this.HandleIsSelectDisableChange;
				this.Model.OnIsAscendChange += this.HandleIsAscendChange;

				// ソート項目初期化
				this.InitSortPattern();

				// 同期
				this.SyncIsAscend();
				this.SyncIsSelectDisable();
			}

			/// <summary>
			/// データセットアップ処理
			/// </summary>
			public void Setup(SortPatternType pattern, bool isSelectDisable, bool isAscend)
			{
				if (!this.CanUpdate) { return; }

				this.SetupSortPattern(pattern);
				this.Model.IsSelectDisable = isSelectDisable;
				this.Model.IsAscend = isAscend;
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

				this.OnSortPatternChangeEvent = null;
				this.OKClickEvent = null;
			}
			#endregion

			#region アクティブ設定
			/// <summary>
			/// アクティブを設定する
			/// </summary>
			public void SetActive(bool isActive, bool isTweenSkip)
			{
				if (!this.CanUpdate) { return; }
				this.View.SetActive(isActive, isTweenSkip);

				// 同期
				this.SetupSortPattern(this.SortPattern);
				this.SyncIsAscend();
				this.SyncIsSelectDisable();
			}
			#endregion

			#region 閉じるボタン
			/// <summary>
			/// 閉じるボタンが押された時に呼び出される
			/// </summary>
			private void HandleCloseClickEvent(object sender, EventArgs e)
			{
				// 閉じる
				GUIController.SingleClose();
			}
			#endregion

			#region ソート項目
			/// <summary>
			/// 現在有効状態になっているソート項目
			/// </summary>
			public event EventHandler OnSortPatternChangeEvent = (sender, e) => { };
			private SortPatternType _sortPattern = SortPatternType.None;
			public SortPatternType SortPattern
			{
				get { return this._sortPattern; }
				private set
				{
					if(this._sortPattern != value)
					{
						this._sortPattern = value;
						
						// 通知
						this.OnSortPatternChangeEvent(this, EventArgs.Empty);
					}
				}
			}
			/// <summary>
			/// ソート項目タイプ
			/// </summary>
			public enum SortPatternType
			{
				None,					// 設定なし
				Rank,					// ランク
				Cost,					// コスト
				Level,					// レベル
				CharaType,				// 種類
				Obtaining,				// 入手
				HitPoint,				// 生命力
				Attack,					// 攻撃力
				Defense,				// 防御力
				Extra,					// 特殊能力
			}

			/// <summary>
			/// ソート項目有効設定リスト
			/// </summary>
			private Dictionary<SortPatternType, Action<bool>> setPatternEnableList = new Dictionary<SortPatternType, Action<bool>>();

			/// <summary>
			/// 一時フラグ(OKボタンが押されるまでに変化されたフラグを保持しておくデータ)
			/// </summary>
			private SortPatternType tempSortPattern = SortPatternType.Rank;

			/// <summary>
			/// ソート項目の初期化
			/// </summary>
			private void InitSortPattern()
			{
				if (!this.CanUpdate) { return; }

				// ソート項目有効設定リストを作成
				this.setPatternEnableList.Clear();
				this.setPatternEnableList.Add(SortPatternType.Rank, this.View.SetRankEnable);
				this.setPatternEnableList.Add(SortPatternType.Cost, this.View.SetCostEnable);
				this.setPatternEnableList.Add(SortPatternType.Level, this.View.SetLevelEnable);
				this.setPatternEnableList.Add(SortPatternType.CharaType, this.View.SetCharaTypeEnable);
				this.setPatternEnableList.Add(SortPatternType.Obtaining, this.View.SetObtainingEnable);
				this.setPatternEnableList.Add(SortPatternType.HitPoint, this.View.SetHitPointEnable);
				this.setPatternEnableList.Add(SortPatternType.Attack, this.View.SetAttackEnable);
				this.setPatternEnableList.Add(SortPatternType.Defense, this.View.SetDefenseEnable);
				this.setPatternEnableList.Add(SortPatternType.Extra, this.View.SetExtraEnable);
			}

			/// <summary>
			/// ソート項目のセットアップ
			/// </summary>
			private void SetupSortPattern(SortPatternType pattern)
			{
				this.ChangeSortPattern(pattern, false);
			}

			/// <summary>
			/// ソート項目切替処理
			/// </summary>
			private void ChangeSortPattern(SortPatternType pattern, bool isUpdateTemp)
			{
				// 有効設定
				foreach(KeyValuePair<SortPatternType, Action<bool>> kvp in this.setPatternEnableList)
				{
					bool isEnable = false;
					if(kvp.Key == pattern)
					{
						// 切り替る項目と一致していたら有効状態にする
						isEnable = true;
					}
					kvp.Value(isEnable);
				}

				if(isUpdateTemp)
				{
					// 一時データのみ更新
					this.tempSortPattern = pattern;
				}
				else
				{
					// 現項目と一時用データを更新
					this.SortPattern = pattern;
					this.tempSortPattern = pattern;
				}
			}

			#region イベント
			/// <summary>
			/// ランクボタンが押された時に呼び出される
			/// </summary>
			private void HandleRankClickEvent(object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.Rank, true);
			}
			/// <summary>
			/// コストボタンが押された時に呼び出される
			/// </summary>>
			private void HandleCostClickEvent(object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.Cost, true);
			}
			/// <summary>
			/// レベルボタンが押された時に呼び出される
			/// </summary>
			private void HandleLevelClickEvent(Object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.Level, true);
			}
			/// <summary>
			/// 種類ボタンが押された時に呼び出される
			/// </summary>
			private void HandleCharaTypeClickEvent(Object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.CharaType, true);
			}
			/// <summary>
			/// 入手ボタンが押された時に呼び出される
			/// </summary>
			private void HandleObtainingClickEvent(Object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.Obtaining, true);
			}
			/// <summary>
			/// 生命力ボタンが押された時に呼び出される
			/// </summary>
			private void HandleHitPointClickEvent(Object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.HitPoint, true);
			}
			/// <summary>
			/// 攻撃力ボタンが押された時に呼び出される
			/// </summary>
			private void HandleAttackClickEvent(Object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.Attack, true);
			}
			/// <summary>
			/// 防御力ボタンが押された時に呼び出される
			/// </summary>
			private void HandleDefenseClickEvent(Object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.Defense, true);
			}
			/// <summary>
			/// 特殊能力ボタンが押された時に呼び出される
			/// </summary>
			private void HandleExtraClickEvent(object sender, EventArgs e)
			{
				this.ChangeSortPattern(SortPatternType.Extra, true);
			}
			#endregion
			#endregion

			#region 選択表示不可
			/// <summary>
			/// 選択不可表示フラグ
			/// </summary>
			public bool IsSelectDisable
			{
				get
				{
					if (!this.CanUpdate) { return false; }
					return this.Model.IsSelectDisable;
				}
			}
			/// <summary>
			/// 一時フラグ(OKボタンが押されるまでに変化されたフラグを保持しておくデータ)
			/// </summary>
			private bool tempIsSelectDisable;

			/// <summary>
			/// 選択表示不可フラグに変更があった時に呼び出される
			/// </summary>
			private void HandleIsSelectDisableChange(object sender, EventArgs e)
			{
				this.SyncIsSelectDisable();
			}

			/// <summary>
			/// 選択表示不可同期
			/// </summary>
			private void SyncIsSelectDisable()
			{
				if (!this.CanUpdate) { return; }
				this.View.SetSelectDisable(this.Model.IsSelectDisable);
				this.tempIsSelectDisable = this.Model.IsSelectDisable;
			}

			/// <summary>
			/// 選択表示不可のボタンが押された時に呼び出される
			/// </summary>
			private void HandleSelectDisableClickEvent(object sender, EventArgs e)
			{
				if (!this.CanUpdate) { return; }
				// 一時フラグと表示切替
				this.tempIsSelectDisable = !this.tempIsSelectDisable;
				this.View.SetSelectDisable(this.tempIsSelectDisable);
			}
			#endregion

			#region 昇順/降順
			/// <summary>
			/// 昇順か降順で並び替えるか
			/// </summary>
			public bool IsAscend
			{
				get
				{
					if (!this.CanUpdate) { return false; }
					return this.Model.IsAscend;
				}
			}
			/// <summary>
			/// 一時フラグ(OKボタンが押されるまでに変化されたフラグを保持しておくデータ)
			/// </summary>
			private bool tempIsAscend = false;

			/// <summary>
			/// 昇順フラグに変化があった時に呼び出される
			/// </summary>
			private void HandleIsAscendChange(object sender, EventArgs e)
			{
				this.SyncIsAscend();
			}

			/// <summary>
			/// 昇順/降順同期
			/// </summary>
			private void SyncIsAscend()
			{
				if (!this.CanUpdate) { return; }
				this.View.SetAscendEnable(this.Model.IsAscend);
				this.View.SetDescendEnable(!this.Model.IsAscend);
				this.tempIsAscend = this.Model.IsAscend;
			}

			/// <summary>
			/// 昇順ボタンが押された時に呼ばれる
			/// </summary>
			private void HandleAscendClickEvent(object sender, EventArgs e)
			{
				if (!this.CanUpdate) { return; }
				// 一時フラグと表示切替
				this.tempIsAscend = true;
				this.View.SetAscendEnable(this.tempIsAscend);
				this.View.SetDescendEnable(!this.tempIsAscend);
			}
			/// <summary>
			/// 降順ボタンが押された時に呼び出される
			/// </summary>
			private void HandleDescendClickEvent(object sender, EventArgs e)
			{
				if (!this.CanUpdate) { return; }
				// 一時フラグと表示切替
				this.tempIsAscend = false;
				this.View.SetAscendEnable(this.tempIsAscend);
				this.View.SetDescendEnable(!this.tempIsAscend);
			}
			#endregion

			#region OKボタン
			/// <summary>
			/// OKボタンを押した時のイベント通知
			/// </summary>
			public event EventHandler<OKClickEventArgs> OKClickEvent = (seder, e) => { };
			/// <summary>
			/// OKボタンが押された時に呼び出される
			/// </summary>
			private void HandleOkClickEvent(object sender, EventArgs e)
			{
				if (!CanUpdate) { return; }
				
				// 一時データから元データに更新
				Setup(this.tempSortPattern, this.tempIsSelectDisable, this.tempIsAscend);

				// 通知
				var eventArgs = new OKClickEventArgs(this.SortPattern, this.Model.IsSelectDisable, this.Model.IsAscend);
				this.OKClickEvent(this, eventArgs);
			}
			#endregion
		}
	}
}
