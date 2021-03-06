/// <summary>
/// シンクロ合成結果表示
/// 
/// 2016/03/07
/// </summary>
using UnityEngine;
using System;

namespace XUI.SynchroResult
{
	/// <summary>
	/// シンクロ合成結果表示インターフェイス
	/// </summary>
	public interface IVIew
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

		#region 生命力
		/// <summary>
		/// シンクロ生命力を設定する
		/// </summary>
		void SetSynchroHitPoint(int synchroHp, string format, bool isUpEffectActive);
		/// <summary>
		/// シンクロ生命力アップ分を設定する
		/// </summary>
		void SetHitPointUp(int up, string format);
		#endregion

		#region 攻撃力
		/// <summary>
		/// シンクロ攻撃力を設定する
		/// </summary>
		void SetSynchroAttack(int synchroAttack, string format, bool isUpEffectActive);
		/// <summary>
		/// シンクロ攻撃力アップ分を設定する
		/// </summary>
		void SetAttackUp(int up, string format);
		#endregion

		#region 防御力
		/// <summary>
		/// シンクロ防御力を設定する
		/// </summary>
		void SetSynchroDefence(int synchroDefence, string format, bool isUpEffectActive);
		/// <summary>
		/// シンクロ特殊能力アップ分を設定する
		/// </summary>
		void SetDefenceUp(int up, string format);
		#endregion

		#region 特殊能力
		/// <summary>
		/// シンクロ特殊能力を設定する
		/// </summary>
		void SetSynchroExtra(int synchroExtra, string format, bool isUpEffectActive);
		/// <summary>
		/// シンクロ特殊能力アップ分を設定する
		/// </summary>
		void SetExtraUp(int up, string format);
		#endregion

		#region 警告メッセージ
		/// <summary>
		/// 警告メッセージを設定する
		/// </summary>
		void SetWarningMessage(string msg);
		#endregion

		#region OKボタン
		/// <summary>
		/// OKボタンイベント
		/// </summary>
		event EventHandler OnOK;

		/// <summary>
		/// OKボタンの有効設定
		/// </summary>
		void SetOKButtonEnable(bool isEnable);
		#endregion

		#region 立ち絵
		/// <summary>
		/// 立ち絵設定
		/// </summary>
		void SetBoardRoot(Transform boardTrans);
		/// <summary>
		/// 立ち絵リプレイ
		/// </summary>
		void ReplayBoard(bool forward);
		#endregion
	}

	/// <summary>
	/// シンクロ合成結果表示
	/// </summary>
	public class SynchroResultView : GUIViewBase, IVIew
	{
		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void OnDestroy()
		{
			this.OnOK = null;
		}
		#endregion

		#region アクティブ
		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
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

		#region 生命力
		[SerializeField]
		private UILabel _synchroHitPointLabel = null;
		private UILabel SynchroHitPointLabel { get { return _synchroHitPointLabel; } }
		[SerializeField]
		private GameObject _synchroHitPointUpEffect = null;
		private GameObject SynchroHitPointUpEffect { get { return _synchroHitPointUpEffect; } }
		[SerializeField]
		private UILabel _hitPointUpLabel = null;
		private UILabel HitPointUpLabel { get { return _hitPointUpLabel; } }

		/// <summary>
		/// シンクロ生命力を設定する
		/// </summary>
		public void SetSynchroHitPoint(int synchroHp, string format, bool isUpEffectActive)
		{
			if (this.SynchroHitPointLabel == null) { return; }
			this.SynchroHitPointLabel.text = string.Format(format, synchroHp);
			if (this.SynchroHitPointUpEffect != null)
			{
				this.SynchroHitPointUpEffect.SetActive(isUpEffectActive);
			}
		}
		/// <summary>
		/// シンクロ生命力アップ分を設定する
		/// </summary>
		public void SetHitPointUp(int up, string format)
		{
			if (this.HitPointUpLabel == null) { return; }
			this.HitPointUpLabel.text = string.Format(format, up);
		}
		#endregion

		#region 攻撃力
		[SerializeField]
		private UILabel _synchroAttackLabel = null;
		private UILabel SynchroAttackLabel { get { return _synchroAttackLabel; } }
		[SerializeField]
		private GameObject _synchroAttackUpEffect = null;
		private GameObject SynchroAttackUpEffect { get { return _synchroAttackUpEffect; } }
		[SerializeField]
		private UILabel _attackUpLabel = null;
		private UILabel AttackUpLabel { get { return _attackUpLabel; } }

		/// <summary>
		/// シンクロ攻撃力を設定する
		/// </summary>
		public void SetSynchroAttack(int synchroAttack, string format, bool isUpEffectActive)
		{
			if (this.SynchroAttackLabel == null) { return; }
			this.SynchroAttackLabel.text = string.Format(format, synchroAttack);
			if(this.SynchroAttackUpEffect != null)
			{
				this.SynchroAttackUpEffect.SetActive(isUpEffectActive);
			}
		}
		/// <summary>
		/// シンクロ攻撃力アップ分を設定する
		/// </summary>
		public void SetAttackUp(int up, string format)
		{
			if (this.AttackUpLabel == null) { return; }
			this.AttackUpLabel.text = string.Format(format, up);
		}
		#endregion

		#region 防御力
		[SerializeField]
		private UILabel _synchroDefenceLabel = null;
		private UILabel SynchroDefenceLabel { get { return _synchroDefenceLabel; } }
		[SerializeField]
		private GameObject _synchroDefenceUpEffect = null;
		private GameObject SynchroDefenceUpEffect { get { return _synchroDefenceUpEffect; } }
		[SerializeField]
		private UILabel _defenceUpLabel = null;
		private UILabel DefenceUpLabel { get { return _defenceUpLabel; } }

		/// <summary>
		/// シンクロ防御力を設定する
		/// </summary>
		public void SetSynchroDefence(int synchroDefence, string format, bool isUpEffectActive)
		{
			if (this.SynchroDefenceLabel == null) { return; }
			this.SynchroDefenceLabel.text = string.Format(format, synchroDefence);
			if(this.SynchroDefenceUpEffect != null)
			{
				this.SynchroDefenceUpEffect.SetActive(isUpEffectActive);
			}
		}
		/// <summary>
		/// シンクロ特殊能力アップ分を設定する
		/// </summary>
		public void SetDefenceUp(int up, string format)
		{
			if (this.DefenceUpLabel == null) { return; }
			this.DefenceUpLabel.text = string.Format(format, up);
		}
		#endregion

		#region 特殊能力
		[SerializeField]
		private UILabel _synchroExtraLabel = null;
		private UILabel SynchroExtraLabel { get { return _synchroExtraLabel; } }
		[SerializeField]
		private GameObject _synchroExtraUpEffect = null;
		private GameObject SynchroExtraUpEffect { get { return _synchroExtraUpEffect; } }
		[SerializeField]
		private UILabel _extraUpLabel = null;
		private UILabel ExtraUpLabel { get { return _extraUpLabel; } }

		/// <summary>
		/// シンクロ特殊能力を設定する
		/// </summary>
		public void SetSynchroExtra(int synchroExtra, string format, bool isUpEffectActive)
		{
			if (this.SynchroExtraLabel == null) { return; }
			this.SynchroExtraLabel.text = string.Format(format, synchroExtra);
			if(this.SynchroExtraUpEffect != null)
			{
				this.SynchroExtraUpEffect.SetActive(isUpEffectActive);
			}
		}
		/// <summary>
		/// シンクロ特殊能力アップ分を設定する
		/// </summary>
		public void SetExtraUp(int up, string format)
		{
			if (this.ExtraUpLabel == null) { return; }
			this.ExtraUpLabel.text = string.Format(format, up);
		}
		#endregion

		#region 警告メッセージ
		[SerializeField]
		private UILabel _warningMessageLabel = null;
		private UILabel WarningMessageLabel { get { return _warningMessageLabel; } }

		/// <summary>
		/// 警告メッセージを設定する
		/// </summary>
		public void SetWarningMessage(string msg)
		{
			if (this.WarningMessageLabel == null) { return; }
			this.WarningMessageLabel.text = msg;
		}
		#endregion

		#region OKボタン
		/// <summary>
		/// OKボタンイベント
		/// </summary>
		public event EventHandler OnOK = (sender, e) => { };

		public void OnOKEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnOK(this, eventArgs);
		}

		[SerializeField]
		UIButton _okButton = null;
		UIButton OKButton { get { return _okButton; } }

		/// <summary>
		/// OKボタンの有効設定
		/// </summary>
		public void SetOKButtonEnable(bool isEnable)
		{
			if (this.OKButton != null)
			{
				this.OKButton.isEnabled = isEnable;
			}
		}
		#endregion

		#region 立ち絵
		[SerializeField]
		Transform _boardRoot = null;
		Transform BoardRoot { get { return _boardRoot; } }
		[SerializeField]
		UIPlayTween _boardPlayTween = null;
		UIPlayTween BoardPlayTween { get { return _boardPlayTween; } }

		/// <summary>
		/// 立ち絵設定
		/// </summary>
		public void SetBoardRoot(Transform boardTrans)
		{
			this.RemoveBoard();
			if (this.BoardRoot != null && boardTrans != null)
			{
				boardTrans.parent = this.BoardRoot;
			}
		}
		void RemoveBoard()
		{
			if (this.BoardRoot != null)
			{
				for (int i = 0; i < this.BoardRoot.childCount; i++)
				{
					var child = this.BoardRoot.GetChild(i);
					UnityEngine.Object.Destroy(child.gameObject);
				}
			}
		}
		/// <summary>
		/// 立ち絵リプレイ
		/// </summary>
		public void ReplayBoard(bool forward)
		{
			if (this.BoardPlayTween != null)
			{
				this.BoardPlayTween.Play(forward);
			}
		}
		#endregion
	}
}