/// <summary>
/// キャラソート表示
/// 
/// 2016/02/17
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace CharaSort
	{
		/// <summary>
		/// キャラソート表示インターフェイス
		/// </summary>
		public interface IView
		{
			#region アクティブ設定
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			void SetActive(bool isActive, bool isTweenSkip);

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			GUIViewBase.ActiveState GetActiveState();
			#endregion

			#region 閉じるボタン
			/// <summary>
			/// 閉じるボタンを押した時のイベント通知
			/// </summary>
			event EventHandler OnCloseClickEvent;
			#endregion

			#region ソート項目
			/// <summary>
			/// ランクボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnRankClickEvent;
			/// <summary>
			/// ランクの有効設定
			/// </summary>
			void SetRankEnable(bool isEnable);

			/// <summary>
			/// コストボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnCostClickEvent;
			/// <summary>
			/// コストの有効設定
			/// </summary>
			void SetCostEnable(bool isEnable);

			/// <summary>
			/// レベルボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnLevelClickEvent;
			/// <summary>
			/// レベルの有効設定
			/// </summary>
			void SetLevelEnable(bool isEnable);

			/// <summary>
			/// 種類ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnCharaTypeClickEvent;
			/// <summary>
			/// 種類の有効設定
			/// </summary>
			void SetCharaTypeEnable(bool isEnable);

			/// <summary>
			/// 入手ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnObtainingClickEvent;
			/// <summary>
			/// 入手の有効設定
			/// </summary>
			void SetObtainingEnable(bool isEnable);

			/// <summary>
			/// 生命力ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnHitPointClickEvent;
			/// <summary>
			/// 生命力の有効設定
			/// </summary>
			void SetHitPointEnable(bool isEnable);

			/// <summary>
			/// 攻撃力ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnAttackClickEvent;
			/// <summary>
			/// 攻撃力の有効設定
			/// </summary>
			void SetAttackEnable(bool isEnable);

			/// <summary>
			/// 防御力ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnDefenseClickEvent;
			/// <summary>
			/// 防御力の有効設定
			/// </summary>
			void SetDefenseEnable(bool isEnable);

			/// <summary>
			/// 特殊能力ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnExtraClickEvent;
			/// <summary>
			/// 特殊能力の有効設定
			/// </summary>
			void SetExtraEnable(bool isEnable);
			#endregion

			#region 選択不可
			/// <summary>
			/// 選択不可ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnSelectDisableClickEvent;
			/// <summary>
			/// 選択不可設定
			/// </summary>
			void SetSelectDisable(bool isDisable);
			#endregion

			#region 昇順/降順
			/// <summary>
			/// 昇順ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnAscendClickEvent;
			/// <summary>
			/// 昇順有効設定
			/// </summary>
			void SetAscendEnable(bool isEnable);

			/// <summary>
			/// 降順ボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnDescendClickEvent;
			/// <summary>
			/// 降順有効設定
			/// </summary>
			void SetDescendEnable(bool isEnable);
			#endregion

			#region OKボタン
			/// <summary>
			/// OKボタンが押された時のイベント通知
			/// </summary>
			event EventHandler OnOkClickEvent;
			#endregion
		}

		/// <summary>
		/// キャラソート表示
		/// </summary>
		public class CharaSortView : GUIViewBase, IView
		{
			#region アクティブ設定
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			/// <param name="isActive"></param>
			/// <param name="isTweenSkip"></param>
			public void SetActive(bool isActive, bool isTweenSkip)
			{
				this.SetRootActive(isActive, isTweenSkip);
			}

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			/// <returns></returns>
			public GUIViewBase.ActiveState GetActiveState()
			{
				return this.GetRootActiveState();
			}
			#endregion

			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void OnDestroy()
			{
				this.OnCloseClickEvent = null;
				this.OnRankClickEvent = null;
				this.OnCostClickEvent = null;
				this.OnLevelClickEvent = null;
				this.OnCharaTypeClickEvent = null;
				this.OnObtainingClickEvent = null;
				this.OnHitPointClickEvent = null;
				this.OnAttackClickEvent = null;
				this.OnDefenseClickEvent = null;
				this.OnExtraClickEvent = null;
				this.OnSelectDisableClickEvent = null;
				this.OnAscendClickEvent = null;
				this.OnDescendClickEvent = null;
				this.OnOkClickEvent = null;
			}
			#endregion

			#region 閉じるボタン
			/// <summary>
			/// 閉じるボタンを押した時のイベント通知
			/// </summary>
			public event EventHandler OnCloseClickEvent = (sender, e) => { };
			public void OnCloseClick()
			{
				// 通知
				this.OnCloseClickEvent(this, EventArgs.Empty);
			}
			#endregion

			#region ソート項目
			/// <summary>
			/// 項目オブジェクト
			/// </summary>
			[SerializeField]
			private SortPatternAttachObject _sortPatternAttach = null;
			private SortPatternAttachObject SortPatternAttach { get { return _sortPatternAttach; } }
			[Serializable]
			private class SortPatternAttachObject
			{
				/// <summary>
				/// ランクボタン
				/// </summary>
				[SerializeField]
				private XUIButton _rankButton = null;
				public XUIButton RankButton { get { return _rankButton; } }

				/// <summary>
				/// コストボタン
				/// </summary>
				[SerializeField]
				private XUIButton _costButton = null;
				public XUIButton CostButton { get { return _costButton; } }

				/// <summary>
				/// レベルボタン
				/// </summary>
				[SerializeField]
				private XUIButton _levelButton = null;
				public XUIButton LevelButton { get { return _levelButton; } }

				/// <summary>
				/// 種類ボタン
				/// </summary>
				[SerializeField]
				private XUIButton _charaTypeButton = null;
				public XUIButton CharaTypeButton { get { return _charaTypeButton; } }

				/// <summary>
				/// 入手ボタン
				/// </summary>
				[SerializeField]
				private XUIButton _obtainingButton = null;
				public XUIButton ObtainingButton { get { return _obtainingButton; } }

				/// <summary>
				/// 生命力ボタン
				/// </summary>
				[SerializeField]
				private XUIButton _hitPointButton = null;
				public XUIButton HitPointButton { get { return _hitPointButton; } }

				/// <summary>
				/// 攻撃力ボタン
				/// </summary>
				[SerializeField]
				private XUIButton _attackButton = null;
				public XUIButton AttackButton { get { return _attackButton; } }

				/// <summary>
				/// 防御力ボタン
				/// </summary>
				[SerializeField]
				private XUIButton _defenseButton = null;
				public XUIButton DefenseButton { get { return _defenseButton; } }

				/// <summary>
				/// 特殊能力ボタン
				/// </summary>
				[SerializeField]
				private XUIButton _extraButton = null;
				public XUIButton ExtraButton { get { return _extraButton; } }
			}

			/// <summary>
			/// ランクボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnRankClickEvent = (sender, e) => { };
			public void OnRankClick()
			{
				// 通知
				this.OnRankClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// ランクの有効設定
			/// </summary>
			public void SetRankEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.RankButton == null) { return; }
				this.SortPatternAttach.RankButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// コストボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnCostClickEvent = (sender, e) => { };
			public void OnCostClick()
			{
				// 通知
				this.OnCostClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// コストの有効設定
			/// </summary>
			public void SetCostEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.RankButton == null) { return; }
				this.SortPatternAttach.CostButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// レベルボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnLevelClickEvent = (sender, e) => { };
			public void OnLevelClick()
			{
				// 通知
				this.OnLevelClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// レベルの有効設定
			/// </summary>
			public void SetLevelEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.LevelButton == null) { return; }
				this.SortPatternAttach.LevelButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// 種類ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnCharaTypeClickEvent = (sender, e) => { };
			public void OnCharaTypeClick()
			{
				// 通知
				this.OnCharaTypeClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// 種類の有効設定
			/// </summary>
			public void SetCharaTypeEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.CharaTypeButton == null) { return; }
				this.SortPatternAttach.CharaTypeButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// 入手ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnObtainingClickEvent = (sender, e) => { };
			public void OnObtainingClick()
			{
				// 通知
				this.OnObtainingClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// 入手の有効設定
			/// </summary>
			public void SetObtainingEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.ObtainingButton == null) { return; }
				this.SortPatternAttach.ObtainingButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// 生命力ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnHitPointClickEvent = (sender, e) => { };
			public void OnHitPointClick()
			{
				// 通知
				this.OnHitPointClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// 生命力の有効設定
			/// </summary>
			public void SetHitPointEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.HitPointButton == null) { return; }
				this.SortPatternAttach.HitPointButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// 攻撃力ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnAttackClickEvent = (sender, e) => { };
			public void OnAttackClick()
			{
				// 通知
				this.OnAttackClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// 攻撃力の有効設定
			/// </summary>
			public void SetAttackEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.AttackButton == null) { return; }
				this.SortPatternAttach.AttackButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// 防御力ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnDefenseClickEvent = (sender, e) => { };
			public void OnDefenseClick()
			{
				// 通知
				this.OnDefenseClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// 防御力の有効設定
			/// </summary>
			public void SetDefenseEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.DefenseButton == null) { return; }
				this.SortPatternAttach.DefenseButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// 特殊能力ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnExtraClickEvent = (sender, e) => { };
			public void OnExtraClick()
			{
				// 通知
				this.OnExtraClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// 特殊能力の有効設定
			/// </summary>
			public void SetExtraEnable(bool isEnable)
			{
				if (this.SortPatternAttach == null || this.SortPatternAttach.ExtraButton == null) { return; }
				this.SortPatternAttach.ExtraButton.isEnabled = !isEnable;
			}
			#endregion

			#region 選択不可
			/// <summary>
			/// 選択不可チェックスプライト
			/// </summary>
			[SerializeField]
			private UISprite _selectCheckSprite = null;
			private UISprite SelectCheckSprite { get { return _selectCheckSprite; } }

			/// <summary>
			/// 選択不可ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnSelectDisableClickEvent = (sender, e) => { };
			public void OnSelectDisableClick()
			{
				// 通知
				this.OnSelectDisableClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// 選択不可設定
			/// </summary>
			public void SetSelectDisable(bool isDisable)
			{
				if (this.SelectCheckSprite == null) { return; }
				this.SelectCheckSprite.gameObject.SetActive(isDisable);
			}
			#endregion

			#region 昇順/降順
			/// <summary>
			/// 昇順ボタン
			/// </summary>
			[SerializeField]
			private XUIButton _ascendButton = null;
			private XUIButton AscendButton { get { return _ascendButton; } }
			/// <summary>
			/// 降順ボタン
			/// </summary>
			[SerializeField]
			private XUIButton _descendButton = null;
			private XUIButton DescendButton { get { return _descendButton; } }

			/// <summary>
			/// 昇順ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnAscendClickEvent = (sender, e) => { };
			public void OnAscendClick()
			{
				// 通知
				this.OnAscendClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// 昇順有効設定
			/// </summary>
			public void SetAscendEnable(bool isEnable)
			{
				if (this.AscendButton == null) { return; }
				this.AscendButton.isEnabled = !isEnable;
			}

			/// <summary>
			/// 降順ボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnDescendClickEvent = (sender, e) => { };
			public void OnDescendClick()
			{
				// 通知
				this.OnDescendClickEvent(this, EventArgs.Empty);
			}
			/// <summary>
			/// 降順有効設定
			/// </summary>
			public void SetDescendEnable(bool isEnable)
			{
				if (this.DescendButton == null) { return; }
				this.DescendButton.isEnabled = !isEnable;
			}
			#endregion

			#region OKボタン
			/// <summary>
			/// OKボタンが押された時のイベント通知
			/// </summary>
			public event EventHandler OnOkClickEvent = (sender, e) => { };
			public void OnOkClick()
			{
				// 通知
				this.OnOkClickEvent(this, EventArgs.Empty);
			}
			#endregion
		}
	}
}
