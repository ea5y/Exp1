/// <summary>
/// 強化メニュー
/// 
/// 2016/03/18
/// </summary>
using UnityEngine;
using System;

namespace XUI.PowerupMenu
{
	/// <summary>
	/// 強化スロット表示インターフェイス
	/// </summary>
	public interface IView
	{
		#region アクティブ
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();
		#endregion

		#region ホーム、閉じるボタンイベント
		event EventHandler<EventArgs> OnHome;
		event EventHandler<EventArgs> OnClose;
		#endregion

		#region 強化合成イベント
		event EventHandler<EventArgs> OnPowerup;
		void SetPowerupButtonEnable(bool isEnable);
		#endregion

		#region 進化合成イベント
		event EventHandler<EventArgs> OnEvolution;
		void SetEvolutionButtonEnable(bool isEnable);
		#endregion

		#region シンクロイベント
		event EventHandler<EventArgs> OnSynchro;
		void SetSynchroButtonEnable(bool isEnable);
		#endregion

		#region スロットイベント
		event EventHandler<EventArgs> OnSlot;
		void SetSlotButtonEnable(bool isEnable);
		#endregion

		#region スキルイベント
		event EventHandler<EventArgs> OnSkill;
		void SetSkillButtonEnable(bool isEnable);
		#endregion


	}

	/// <summary>
	/// 強化メニュー表示
	/// </summary>
	public class PowerupMenuView : GUIScreenViewBase, IView
	{
		#region アクティブ  
		/// <summary>
		/// アクティブ状態にする
		/// </summary>         
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			// アクティブの設定
			this.SetRootActive(isActive, isTweenSkip);
		}
		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState()
		{
			return this.GetRootActiveState();
		}
		#endregion

		#region ホーム、閉じるイベント
		public event EventHandler<EventArgs> OnHome = (sender, e) => { };
		public event EventHandler<EventArgs> OnClose = (sender, e) => { };

		/// <summary>
		/// ホームボタンイベント
		/// </summary>
		public override void OnHomeEvent()
		{
			var eventArgs = new EventArgs();
			this.OnHome(this, eventArgs);
		}
		/// <summary>
		/// 閉じるイベント
		/// </summary>
		public override void OnCloseEvent()
		{
			var eventArgs = new EventArgs();
			this.OnClose(this, eventArgs);
		}
		#endregion

		#region 強化合成ボタン
		public event EventHandler<EventArgs> OnPowerup = (sender, e) => { };

		/// <summary>
		/// 強化合成ボタンの有効化
		/// </summary>
		public void OnPowerupEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnPowerup(this, eventArgs);
		}

		[SerializeField]
		UIButton _powerupButton = null;
		UIButton PowerupButton { get { return _powerupButton; } }

		/// <summary>
		/// 強化合成ボタンの有効化
		/// </summary>
		/// <param name="isEnable">表示フラグ</param>
		public void SetPowerupButtonEnable(bool isEnable)
		{
			if(this.PowerupButton != null)
			{
				this.PowerupButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region 進化合成ボタン
		public event EventHandler<EventArgs> OnEvolution = (sender, e) => { };

		/// <summary>
		/// 進化合成ボタンの有効化
		/// </summary>
		public void OnEvolutionEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnEvolution(this, eventArgs);
		}

		[SerializeField]
		UIButton _evolutionButton = null;
		UIButton EvolutionButton { get { return _evolutionButton; } }

		/// <summary>
		/// 進化合成ボタンの有効化
		/// </summary>
		/// <param name="isEnable">表示フラグ</param>
		public void SetEvolutionButtonEnable(bool isEnable)
		{
			if (this.EvolutionButton != null)
			{
				this.EvolutionButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region シンクロボタン
		public event EventHandler<EventArgs> OnSynchro = (sender, e) => { };

		/// <summary>
		/// 強化合成ボタンの有効化
		/// </summary>
		public void OnSynchroEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnSynchro(this, eventArgs);
		}

		[SerializeField]
		UIButton _synchroButton = null;
		UIButton SynchroButton { get { return _synchroButton; } }

		/// <summary>
		/// 強化合成ボタンの有効化
		/// </summary>
		/// <param name="isEnable">表示フラグ</param>
		public void SetSynchroButtonEnable(bool isEnable)
		{
			if (this.SynchroButton != null)
			{
				this.SynchroButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region スロットボタン
		public event EventHandler<EventArgs> OnSlot = (sender, e) => { };

		/// <summary>
		/// 強化合成ボタンの有効化
		/// </summary>
		public void OnSlotEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnSlot(this, eventArgs);
		}

		[SerializeField]
		UIButton _slotButton = null;
		UIButton SlotButton { get { return _slotButton; } }

		/// <summary>
		/// 強化合成ボタンの有効化
		/// </summary>
		/// <param name="isEnable">表示フラグ</param>
		public void SetSlotButtonEnable(bool isEnable)
		{
			if (this.SynchroButton != null)
			{
				this.SynchroButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region スキルボタン
		public event EventHandler<EventArgs> OnSkill = (sender, e) => { };

		/// <summary>
		/// 強化合成ボタンの有効化
		/// </summary>
		public void OnSkillEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnSkill(this, eventArgs);
		}

		[SerializeField]
		UIButton _skillButton = null;
		UIButton SkillButton { get { return _skillButton; } }

		/// <summary>
		/// 強化合成ボタンの有効化
		/// </summary>
		/// <param name="isEnable">表示フラグ</param>
		public void SetSkillButtonEnable(bool isEnable)
		{
			if (this.SkillButton != null)
			{
				this.SkillButton.isEnabled = isEnable;
			}
		}
		#endregion


	}
}
