/// <summary>
/// プレイヤー情報表示
/// 
/// 2015/12/10
/// </summary>
using UnityEngine;
using System;

namespace XUI.PlayerInfo
{
	/// <summary>
	/// プレイヤー情報表示インターフェイス
	/// </summary>
	public interface IView
	{
		#region アクティブ設定
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive);
		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();
		#endregion

		#region 石追加ボタン
		event EventHandler<EventArgs> OnAddStone;

		/// <summary>
		/// 石追加ボタンの有効化
		/// </summary>
		void SetAddStoneButtonEnable(bool isEnable);
		#endregion

		#region キャラアイコンボタン
		event EventHandler<EventArgs> OnCharaIcon;
		#endregion

		#region キャラアイコン
		/// <summary>
		/// キャラアイコン設定
		/// </summary>
		void SetCharaIcon(UIAtlas atlas, string spriteName);
		/// <summary>
		/// キャラアイコンボタンの有効化
		/// </summary>
		void SetCharaIconButtonEnable(bool isEnable);
		#endregion

		#region 名前
		/// <summary>
		/// プレイヤー名設定
		/// </summary>
		void SetName(string name, string format);
		#endregion

		#region グレード
		/// <summary>
		/// グレード設定
		/// </summary>
		void SetGrade(int grade, string format);
		#endregion

		#region レベル
		/// <summary>
		/// プレイヤーレベル設定
		/// </summary>
		void SetLv(int lv, string format);
		#endregion

		#region 経験値
		/// <summary>
		/// プレイヤーの経験値設定
		/// </summary>
		void SetExp(float sliderValue, string format);
		#endregion

		#region スタミナ
		/// <summary>
		/// プレイヤーのスタミナ設定
		/// </summary>
		void SetStamina(int now, int max, string format);
		#endregion

		#region スタミナ回復するまでの残り時間
		/// <summary>
		/// スタミナ回復するまでの残り時間
		/// </summary>
		void SetStaminaTime(int second, string format);
		#endregion
	}

	/// <summary>
	/// プレイヤー情報表示
	/// </summary>
	public class PlayerInfoView : GUIViewBase, IView
	{
		#region アクティブ設定
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive(bool isActive)
		{
			this.SetRootActive(isActive);
		}
		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState()
		{
			return this.GetRootActiveState();
		}
		#endregion

		#region 石追加ボタン
		public event EventHandler<EventArgs> OnAddStone = (sender, e) => { };

		/// <summary>
		/// 石追加ボタンイベント
		/// </summary>
		public void OnAddStoneEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnAddStone(this, eventArgs);
		}

		[SerializeField]
		UIButton _addStoneButton = null;
		UIButton AddStoneButton { get { return _addStoneButton; } }

		/// <summary>
		/// 石追加ボタンの有効化
		/// </summary>
		public void SetAddStoneButtonEnable(bool isEnable)
		{
			if (this.AddStoneButton != null)
			{
				this.AddStoneButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region キャラアイコンボタン
		public event EventHandler<EventArgs> OnCharaIcon = (sender, e) => { };

		/// <summary>
		/// キャラアイコンイベント
		/// </summary>
		public void OnCharaIconEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnCharaIcon(this, eventArgs);
		}
		#endregion

		#region キャラアイコン
		[SerializeField]
		UISprite _charaIconSprite = null;
		UISprite CharaIconSprite { get { return _charaIconSprite; } }

		[SerializeField]
		UIButton _charaIconButton = null;
		UIButton CharaIconButton { get { return _charaIconButton; } }

		/// <summary>
		/// キャラアイコン設定
		/// </summary>
		public void SetCharaIcon(UIAtlas atlas, string spriteName)
		{
			if (CharaIcon.SetIconSprite(this.CharaIconSprite, atlas, spriteName))
			{
				if (this.CharaIconButton != null)
				{
					// ボタンの通常スプライトの方にも適用しないとホバーした時とか元に戻ってしまう
					this.CharaIconButton.normalSprite = spriteName;
				}
			}
		}
		/// <summary>
		/// キャラアイコンボタンの有効化
		/// </summary>
		public void SetCharaIconButtonEnable(bool isEnable)
		{
			if (this.CharaIconButton != null)
			{
				this.CharaIconButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region 名前
		[SerializeField]
		UILabel _nameLabel = null;
		UILabel NameLabel { get { return _nameLabel; } }

		/// <summary>
		/// プレイヤー名設定
		/// </summary>
		public void SetName(string name, string format)
		{
			if (this.NameLabel != null)
			{
				this.NameLabel.text = string.Format(format, name);
			}
		}
		#endregion

		#region グレード
		[SerializeField]
		UILabel _gradeLabel = null;
		UILabel GradeLabel { get { return _gradeLabel; } }

		/// <summary>
		/// グレード設定
		/// </summary>
		public void SetGrade(int grade, string format)
		{
			if (this.GradeLabel != null)
			{
				this.GradeLabel.text = string.Format(format, grade);
			}
		}
		#endregion

		#region レベル
		[SerializeField]
		UILabel _lvLabel = null;
		UILabel LvLabel { get { return _lvLabel; } }

		/// <summary>
		/// プレイヤーレベル設定
		/// </summary>
		public void SetLv(int lv, string format)
		{
			if (this.LvLabel != null)
			{
				this.LvLabel.text = string.Format(format, lv);
			}
		}
		#endregion

		#region 経験値
		[SerializeField]
		UIProgressBar _expSlider = null;
		UIProgressBar ExpSlider { get { return _expSlider; } }

		[SerializeField]
		UILabel _expLable = null;
		UILabel ExpLabel { get { return _expLable; } }

		/// <summary>
		/// プレイヤーの経験値設定
		/// </summary>
		public void SetExp(float sliderValue, string format)
		{
			sliderValue = Mathf.Clamp01(sliderValue);
			if (this.ExpSlider != null)
			{
				this.ExpSlider.value = sliderValue;
			}
			if (this.ExpLabel != null)
			{
				this.ExpLabel.text = string.Format(format, sliderValue * 100f);
			}
		}
		#endregion

		#region スタミナ
		[SerializeField]
		UIProgressBar _staminaSlider = null;
		UIProgressBar StaminaSlider { get { return _staminaSlider; } }

		[SerializeField]
		UILabel _staminaLabel = null;
		UILabel StaminaLabel { get { return _staminaLabel; } }

		/// <summary>
		/// プレイヤーのスタミナ設定
		/// </summary>
		public void SetStamina(int now, int max, string format)
		{
			if (this.StaminaLabel != null)
			{
				this.StaminaLabel.text = string.Format(format, now, max);
			}
			if (this.StaminaSlider != null)
			{
				var t = 0f;
				if (max != 0)
				{
					t = (float)now / (float)max;
					t = Mathf.Clamp01(t);
				}
				this.StaminaSlider.value = t;
			}
		}
		#endregion

		#region スタミナ回復するまでの残り時間
		[SerializeField]
		UILabel _staminaTimeLabel = null;
		UILabel StaminaTimeLabel { get { return _staminaTimeLabel; } }

		/// <summary>
		/// スタミナ回復するまでの残り時間
		/// </summary>
		public void SetStaminaTime(int second, string format)
		{
			if (this.StaminaTimeLabel != null)
			{
				var min = second / 60;
				var sec = second % 60;
				this.StaminaTimeLabel.text = string.Format(format, min, sec);
			}
		}
		#endregion
	}
}
