/// <summary>
/// キャラクター詳細表示
/// 
/// 2016/03/25
/// </summary>
using UnityEngine;
using System;
using System.Collections;

namespace XUI.CharacterInfo
{
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

		#region	キャラクターネーム更新
		void SetCharaName(string name, string format);
		#endregion

		#region リビルド表示更新
		void SetRebuildTime(float time, string format);
		#endregion

		#region コスト表示更新
		void SetCharaCost(int characost, string format);
		#endregion

		#region ロック状態更新
		event EventHandler<EventArgs> OnLockChange;
		void SetLockSprite(bool isLock);
		#endregion

		#region 強化関連
		/// <summary>
		/// ランク表示更新
		/// </summary>
		void SetRank(int rank, string format);
		/// <summary>
		/// レベル表示更新
		/// </summary>
		void SetPowerupLevel(int level, int maxLevel, string format);
		/// <summary>
		/// Exp表示更新
		/// </summary>
		void SetPowerupExp(int exp, string format);
		void SetPowerupNextExp(int nextExp, string format);
		/// <summary>
		/// シンクロ合成残り回数
		/// </summary>
		void SetSynchroRemain(int remain, string format);
		/// <summary>
		/// 強化スロット
		/// </summary>
		void SetPowerupSlot(int slot, int slotMax, string format);

		#endregion

		#region ステータス表示

		#region 生命力
		// 合計
		void SetTotalHitPoint(int hitpoint, string format);
		// ベース
		void SetBaseHitPoint(int hitpoint, string format);
		// スロット
		void SetSlotHitPoint(int addHitpoint, string format);
		// シンクロ
		void SetSyncHitPoint(int addHitpoint, string format);
		#endregion

		#region 攻撃力
		// 合計
		void SetTotalAttack(int attack, string format);
		// ベース
		void SetBaseAttack(int attack, string format);
		// スロット
		void SetSlotAttack(int addAttack, string format);
		// シンクロ
		void SetSyncAttack(int addAttack, string format);
		#endregion

		#region 防御力
		// 合計
		void SetTotalDefense(int defense, string format);
		// ベース
		void SetBaseDefense(int defense, string format);
		// スロット
		void SetSlotDefense(int addDefense, string format);
		// シンクロ
		void SetSyncDefense(int addDefense, string format);
		#endregion

		#region 特殊能力
		// 合計
		void SetTotalExtra(int extra, string format);
		// ベース
		void SetBaseExtra(int extra, string format);
		// スロット
		void SetSlotExtra(int addExtra, string format);
		// シンクロ
		void SetSyncExtra(int addExtra, string format);
		#endregion

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


	public class CharacterInfoView : GUIScreenViewBase , IView
	{
		#region 破棄
		void OnDestroy()
		{
			this.OnHome = null;
			this.OnClose = null;
			this.OnLockChange = null;
		}
		#endregion

		#region アクティブ  
		/// <summary>
		/// アクティブ状態にする
		/// </summary>         
		public void SetActive(bool isActive, bool isTweenSkip)
		{

			// アクティブの設定
			if(isActive)
			{
				// ボードの削除
				this.RemoveBoard();
				// ボード（立ち絵）が消されてからウィンドウをアクティブ化する
				FiberController.AddFiber(this.WaitDeleteBoardRootActiveCoroutine(isTweenSkip));
			}
			else
			{
				this.SetRootActive(isActive, isTweenSkip);
			}

		}

		/// <summary>
		/// ボード（立ち絵）が消されてからウィンドウをアクティブ化する
		/// </summary>
		IEnumerator WaitDeleteBoardRootActiveCoroutine(bool isTweenSkip)
		{
			// ボードが消されていなかったら待機
			while (this.BoardRoot != null && this.BoardRoot.childCount >= 1)
			{
				yield return null;
			}

			this.SetRootActive(true, isTweenSkip);
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

		#region キャラクターネーム
		[SerializeField]
		UILabel _charaNameLabel = null;
		UILabel CharaNameLabel { get { return _charaNameLabel; } }

		/// <summary>
		/// キャラクターネーム更新
		/// </summary>
		/// <param name="name"></param>
		public void SetCharaName(string name, string format)
		{
			if (this.CharaNameLabel != null)
			{
				this.CharaNameLabel.text = string.Format(format, name);
			}
		}

		#endregion

		#region リビルド
		[SerializeField]
		UILabel _rebuildTimeLabel = null;
		UILabel RebuildTimeLabel { get { return _rebuildTimeLabel; } }

		public void SetRebuildTime(float time, string format)
		{
			if(this.RebuildTimeLabel != null)
			{
				this.RebuildTimeLabel.text = string.Format(format, time);
			}
		}
		#endregion

		#region コストイベント
		[SerializeField]
		UILabel _charaCostLabel = null;
		UILabel CharaCostLabel { get { return _charaCostLabel; } }

		public void SetCharaCost(int cost, string format)
		{

			if (this.CharaCostLabel != null)
			{
				this.CharaCostLabel.text = string.Format(format, cost);
			}
		}
		#endregion

		#region ロックボタンイベント
		public event EventHandler<EventArgs> OnLockChange = (sender, e) => { };

		// 非ロック状態の画像
		[SerializeField]
		UISprite _unlockButton = null;
		UISprite UnlockButton { get { return _unlockButton; } }

		// ロック状態の画像
		[SerializeField]
		UISprite _lockButton = null;
		UISprite LockButton { get { return _lockButton; } }



		/// <summary>
		/// ボタンのロックイベント
		/// </summary>
		public void OnLockEvent()
		{
			// 通知
			var eventArgs = new EventArgs();
			this.OnLockChange(this, eventArgs);
		}

		/// <summary>
		/// ロックボタンの表示切り替え
		/// </summary>
		public void SetLockSprite(bool isLock)
		{
			// 画像が存在しているのか
			if (LockButton != null && UnlockButton != null)
			{
				// ロック状態にする
				this.LockButton.gameObject.SetActive(isLock);
				this.UnlockButton.gameObject.SetActive(!isLock);
			}
		}
		#endregion

		#region ステータス表示

		#region 生命力
		/// <summary>
		/// 合計生命力
		/// </summary>
		[SerializeField]
		UILabel _totalHitPointLabel = null;
		UILabel TotalHitPointLabel { get { return _totalHitPointLabel; } }

		/// <summary>
		/// 合計生命力のセット
		/// </summary>
		public void SetTotalHitPoint(int hitpoint, string format)
		{
			if (this.TotalHitPointLabel != null)
			{
				this.TotalHitPointLabel.text = string.Format(format, hitpoint);
			}
		}

		/// <summary>
		/// 基礎生命力
		/// </summary>
		[SerializeField]
		UILabel _baseHitPointLabel = null;
		UILabel BaseHitPointLabel { get { return _baseHitPointLabel; } }

		/// <summary>
		/// 基礎生命力のセット
		/// </summary>
		public void SetBaseHitPoint(int hitpoint, string format)
		{
			if (this.BaseHitPointLabel != null)
			{
				this.BaseHitPointLabel.text = string.Format(format, hitpoint);
			}
		}

		/// <summary>
		/// 生命力のスロット補正
		/// </summary>
		[SerializeField]
		UILabel _slotHitPointLabel = null;
		UILabel SlotHitPointLabel { get { return _slotHitPointLabel; } }

		/// <summary>
		/// 生命力のスロット補正をセット
		/// </summary>
		public void SetSlotHitPoint(int addHitpoint, string format)
		{
			if (this.SlotHitPointLabel != null)
			{
				this.SlotHitPointLabel.text = string.Format(format, addHitpoint);
			}
		}

		/// <summary>
		/// 生命力のシンクロ補正
		/// </summary>
		[SerializeField]
		UILabel _syncHitPointLabel = null;
		UILabel SyncHitPointLabel { get { return _syncHitPointLabel; } }

		/// <summary>
		/// 生命力のシンクロ補正をセット
		/// </summary>
		public void SetSyncHitPoint(int addHitpoint, string format)
		{
			if (this.SyncHitPointLabel != null)
			{
				this.SyncHitPointLabel.text = string.Format(format, addHitpoint);
			}
		}
		#endregion

		#region 攻撃力
		/// <summary>
		/// 合計攻撃力
		/// </summary>
		[SerializeField]
		UILabel _totalAttackLabel = null;
		UILabel TotalAttackLabel { get { return _totalAttackLabel; } }

		/// <summary>
		/// 合計攻撃力のセット
		/// </summary>
		public void SetTotalAttack(int attack, string format)
		{
			if (this.TotalAttackLabel != null)
			{
				this.TotalAttackLabel.text = string.Format(format, attack);
			}
		}

		/// <summary>
		/// 基礎攻撃力
		/// </summary>
		[SerializeField]
		UILabel _baseAttackLabel = null;
		UILabel BaseAttackLabel { get { return _baseAttackLabel; } }

		/// <summary>
		/// 基礎攻撃力のセット
		/// </summary>
		public void SetBaseAttack(int attack, string format)
		{
			if (this.BaseAttackLabel != null)
			{
				this.BaseAttackLabel.text = string.Format(format, attack);
			}
		}

		/// <summary>
		/// 攻撃力のスロット補正
		/// </summary>
		[SerializeField]
		UILabel _slotAttackLabel = null;
		UILabel SlotAttackLabel { get { return _slotAttackLabel; } }

		/// <summary>
		/// 攻撃力のスロット補正をセット
		/// </summary>
		public void SetSlotAttack(int addeAttack, string format)
		{
			if (this.SlotAttackLabel != null)
			{
				this.SlotAttackLabel.text = string.Format(format, addeAttack);
			}
		}

		/// <summary>
		/// 攻撃力のシンクロ補正
		/// </summary>
		[SerializeField]
		UILabel _syncAttackLabel = null;
		UILabel SyncAttackLabel { get { return _syncAttackLabel; } }

		/// <summary>
		/// 攻撃力のシンクロ補正をセット
		/// </summary>
		public void SetSyncAttack(int addeAttack, string format)
		{
			if (this.SyncAttackLabel != null)
			{
				this.SyncAttackLabel.text = string.Format(format, addeAttack);
			}
		}
		#endregion

		#region 防御力
		/// <summary>
		/// 合計防御力
		/// </summary>
		[SerializeField]
		UILabel _totalDefenseLabel = null;
		UILabel TotalDefenseLabel { get { return _totalDefenseLabel; } }

		/// <summary>
		/// 合計防御力のセット
		/// </summary>
		public void SetTotalDefense(int defense, string format)
		{
			if (this.TotalDefenseLabel != null)
			{
				this.TotalDefenseLabel.text = string.Format(format, defense);
			}
		}

		/// <summary>
		/// 基礎防御力
		/// </summary>
		[SerializeField]
		UILabel _baseDefenseLabel = null;
		UILabel BaseDefenseLabel { get { return _baseDefenseLabel; } }

		/// <summary>
		/// 基礎防御力のセット
		/// </summary>
		public void SetBaseDefense(int defense, string format)
		{
			if (this.BaseDefenseLabel != null)
			{
				this.BaseDefenseLabel.text = string.Format(format, defense);
			}
		}

		/// <summary>
		/// 防御力のスロット補正
		/// </summary>
		[SerializeField]
		UILabel _slotDefenseLabel = null;
		UILabel SlotDefenseLabel { get { return _slotDefenseLabel; } }

		/// <summary>
		/// 防御力のスロット補正をセット
		/// </summary>
		public void SetSlotDefense(int addDefense, string format)
		{
			if (this.SlotDefenseLabel != null)
			{
				this.SlotDefenseLabel.text = string.Format(format, addDefense);
			}
		}

		/// <summary>
		/// 防御力のシンクロ補正
		/// </summary>
		[SerializeField]
		UILabel _syncDefenseLabel = null;
		UILabel SyncDefenseLabel { get { return _syncDefenseLabel; } }

		/// <summary>
		/// 防御力のシンクロ補正をセット
		/// </summary>
		public void SetSyncDefense(int addeDefense, string format)
		{
			if (this.SyncDefenseLabel != null)
			{
				this.SyncDefenseLabel.text = string.Format(format, addeDefense);
			}
		}
		#endregion

		#region 特殊能力
		/// <summary>
		/// 合計特殊能力
		/// </summary>
		[SerializeField]
		UILabel _totalExtraLabel = null;
		UILabel TotalExtraLabel { get { return _totalExtraLabel; } }

		/// <summary>
		/// 合計特殊能力のセット
		/// </summary>
		public void SetTotalExtra(int extra, string format)
		{
			if (this.TotalExtraLabel != null)
			{
				this.TotalExtraLabel.text = string.Format(format, extra);
			}
		}

		/// <summary>
		/// 基礎特殊能力
		/// </summary>
		[SerializeField]
		UILabel _baseExtraLabel = null;
		UILabel BaseExtraLabel { get { return _baseExtraLabel; } }

		/// <summary>
		/// 基礎特殊能力のセット
		/// </summary>
		public void SetBaseExtra(int extra, string format)
		{
			if (this.BaseExtraLabel != null)
			{
				this.BaseExtraLabel.text = string.Format(format, extra);
			}
		}

		/// <summary>
		/// 特殊能力のスロット補正
		/// </summary>
		[SerializeField]
		UILabel _slotExtraLabel = null;
		UILabel SlotExtraLabel { get { return _slotExtraLabel; } }

		/// <summary>
		/// 特殊能力のスロット補正をセット
		/// </summary>
		public void SetSlotExtra(int addExtra, string format)
		{
			if (this.SlotExtraLabel != null)
			{
				this.SlotExtraLabel.text = string.Format(format, addExtra);
			}
		}

		/// <summary>
		/// 特殊能力のシンクロ補正
		/// </summary>
		[SerializeField]
		UILabel _syncExtraLabel = null;
		UILabel SyncExtraLabel { get { return _syncExtraLabel; } }

		/// <summary>
		/// 特殊能力のシンクロ補正をセット
		/// </summary>
		public void SetSyncExtra(int addExtra, string format)
		{
			if (this.SyncExtraLabel != null)
			{
				this.SyncExtraLabel.text = string.Format(format, addExtra);
			}
		}
		#endregion

		#endregion

		#region 強化関連

		#region ランク
		/// <summary>	
		/// ランク
		/// </summary>
		[SerializeField]
		UILabel _rankLabel = null;
		UILabel RankLabel { get { return _rankLabel; } }

		/// <summary>
		/// ランクのセット
		/// </summary>
		public void SetRank(int rank, string format)
		{
			if (this.RankLabel != null)
			{
				this.RankLabel.text = string.Format(format,rank);
			}
		}
		#endregion

		#region レベル
		/// <summary>
		/// レベル
		/// </summary>
		[SerializeField]
		UILabel _levelLabel = null;
		UILabel LevelLabel { get { return _levelLabel; } }

		/// <summary>
		/// レベルのセット
		/// </summary>
		public void SetPowerupLevel(int level, int maxLevel, string format)
		{
			if (this.LevelLabel != null)
			{
				this.LevelLabel.text = string.Format(format, level, maxLevel );
			}
		}
		#endregion

		#region 経験値
		/// <summary>
		/// 所持経験値
		/// </summary>
		[SerializeField]
		UILabel _expLabel = null;
		UILabel ExpLabel { get { return _expLabel; } }

		/// <summary>
		/// 所持経験値
		/// </summary>
		public void SetPowerupExp(int exp, string format)
		{
			if (this.ExpLabel != null)
			{
				this.ExpLabel.text = string.Format(format, exp);
			}
		}

		/// <summary>
		/// Next経験値
		/// </summary>
		[SerializeField]
		UILabel _nextExpLabel = null;
		UILabel NextExpLabel { get { return _expLabel; } }

		/// <summary>
		/// Next経験値
		/// </summary>
		public void SetPowerupNextExp(int nextExp, string format)
		{
			if (this._nextExpLabel != null)
			{
				this._nextExpLabel.text = string.Format(format, nextExp);
			}
		}
		#endregion

		#region 残りシンクロ合成回数
		/// <summary>
		/// 残りシンクロ合成回数
		/// </summary>
		[SerializeField]
		UILabel _synchroRemainLabel = null;
		UILabel SynchroRemainLabel { get { return _synchroRemainLabel; } }

		/// <summary>
		/// 残りシンクロ合成回数
		/// </summary>
		public void SetSynchroRemain(int ramain, string format)
		{
			if (this._synchroRemainLabel != null)
			{
				this._synchroRemainLabel.text = string.Format(format, ramain);
			}
		}

		#endregion

		#region 強化スロット数
		/// <summary>
		/// 強化スロット数
		/// </summary>
		[SerializeField]
		UILabel _powerupSlotLabel = null;
		UILabel PowerupSlotLabel { get { return _powerupSlotLabel; } }

		/// <summary>
		/// 強化スロット数
		/// </summary>
		public void SetPowerupSlot(int slot,int slotMax, string format)
		{
			if (this._powerupSlotLabel != null)
			{
				this._powerupSlotLabel.text = string.Format(format, slot,slotMax);
			}
		}

		#endregion

		#endregion

		#region キャラクタボード
		#region 立ち絵
		[SerializeField]
		private Transform _boardRoot = null;
		private Transform BoardRoot { get { return _boardRoot; } }
		[SerializeField]
		private UIPlayTween _boardPlayTween = null;
		private UIPlayTween BoardPlayTween { get { return _boardPlayTween; } }

		/// <summary>
		/// 立ち絵設定
		/// </summary>
		public void SetBoardRoot(Transform boardTrans)
		{
			this.RemoveBoard();
			if (this.BoardRoot != null && boardTrans != null)
			{
				//boardTrans.parent = this.BoardRoot;
				boardTrans.gameObject.SetParentWithLayer(this.BoardRoot.gameObject);
			}

		}
		/// <summary>
		/// 立ち絵削除
		/// </summary>
		private void RemoveBoard()
		{
			if (this.BoardRoot != null)
			{
				this.BoardRoot.DestroyChildren();
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
		#endregion

		#region デバッグ
#if XW_DEBUG
		UILabel _debugLabel = null;
		UILabel DebugLabel { get { return _debugLabel; } }

		public GameObject DebugCopyLabel()
		{
			var srcLabel = this.BaseHitPointLabel;
			var parent = (this.CharaNameLabel != null ? this.CharaNameLabel.gameObject : null);
			if (srcLabel == null) return null;
			if (parent == null) return null;

			GameObject go = SafeObject.Instantiate(srcLabel.gameObject);
			if (go == null) return null;

			this._debugLabel = go.GetComponent<UILabel>();
			if (this.DebugLabel == null)
			{
				UnityEngine.Object.Destroy(go);
				return null;
			}

			this.DebugLabel.pivot = UIWidget.Pivot.TopLeft;
			this.DebugLabel.overflowMethod = UILabel.Overflow.ResizeFreely;

			this.DebugLabel.gameObject.SetParentWithLayer(parent, false);
			var t = this.DebugLabel.gameObject.transform;
			t.localPosition = new Vector3(0f, (float)-this.DebugLabel.fontSize, 0f);

			return go;
		}
		public void DebugSetLabel(string text)
		{
			if (this.DebugLabel != null)
			{
				this.DebugLabel.text = text;
			}
		}
#endif
		#endregion
	}
}