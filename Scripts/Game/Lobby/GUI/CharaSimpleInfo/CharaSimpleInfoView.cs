/// <summary>
/// キャラ簡易情報表示
/// 
/// 2016/01/25
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace CharaSimpleInfo
	{
		/// <summary>
		/// キャラ簡易情報表示インターフェイス
		/// </summary>
		public interface IView
		{
			/// <summary>
			/// アクティブ状態にする
			/// </summary>
			/// <param name="isActive"></param>
			/// <param name="isTweenSkip"></param>
			void SetActive(bool isActive, bool isTweenSkip);

			/// <summary>
			/// 現在のアクティブ状態を取得する
			/// </summary>
			/// <returns></returns>
			GUIViewBase.ActiveState GetActiveState();

			/// <summary>
			/// 位置
			/// </summary>
			Vector3 Position { set; }

			/// <summary>
			/// ウィンドウの幅
			/// </summary>
			int Width { get; }

			/// <summary>
			/// ウィンドウの高さ
			/// </summary>
			int Height { get; }

			/// <summary>
			/// ウィンドウの四隅のワールド座標
			/// </summary>
			Vector3[] WorldCorners { get; }

			/// <summary>
			/// キャラ名の設定
			/// </summary>
			/// <param name="charaName"></param>
			void SetCharaName(string charaName);

			/// <summary>
			/// ランクの設定
			/// </summary>
			/// <param name="rank"></param>
			void SetRank(string rank);

			/// <summary>
			/// 強化レベルの設定
			/// </summary>
			/// <param name="level"></param>
			/// <param name="synchroLevel"></param>
			void SetLevel(string level);

			/// <summary>
			/// 生命力の設定
			/// </summary>
			/// <param name="hitPoint"></param>
			void SetHitPoint(string hitPoint);

			/// <summary>
			/// 攻撃力の設定
			/// </summary>
			/// <param name="attack"></param>
			void SetAttack(string attack);

			/// <summary>
			/// 防御力の設定
			/// </summary>
			/// <param name="defense"></param>
			void SetDefense(string defense);

			/// <summary>
			/// 特殊能力の設定
			/// </summary>
			/// <param name="extra"></param>
			void SetExtra(string extra);

			/// <summary>
			/// ロックのアクティブ設定
			/// </summary>
			/// <param name="isLock"></param>
			void SetLockActive(bool isLock);

			/// <summary>
			/// 背景側が押された時の通知用
			/// </summary>
			event EventHandler OnBGClickEvent;

			/// <summary>
			/// お気に入りボタンが押され時の通知用
			/// </summary>
			event EventHandler OnFavoriteClickEvent;

			/// <summary>
			/// 詳細ボタンが押された時の通知用
			/// </summary>
			event EventHandler OnCharaInfoClickEvent;
		}

		/// <summary>
		/// キャラ簡易情報表示
		/// </summary>
		public class CharaSimpleInfoView : GUIViewBase, IView
		{
			#region フィールド&プロパティ
			/// <summary>
			/// キャラ名ラベル
			/// </summary>
			[SerializeField]
			private UILabel _charaNameLabel = null;
			public UILabel CharaNameLabel { get { return _charaNameLabel; } }

			/// <summary>
			/// ランクラベル
			/// </summary>
			[SerializeField]
			private UILabel _rankLabel = null;
			public UILabel RankLabel { get { return _rankLabel; } }

			/// <summary>
			/// 強化レベルラベル
			/// </summary>
			[SerializeField]
			private UILabel _levelLabel = null;
			public UILabel LevelLabel { get { return _levelLabel; } }

			/// <summary>
			/// ステータスオブジェクト
			/// </summary>
			[SerializeField]
			private StatusAttachObject _statusAttach = null;
			public StatusAttachObject StatusAttach { get { return _statusAttach; } }
			[Serializable]
			public class StatusAttachObject
			{
				/// <summary>
				/// 生命力ラベル
				/// </summary>
				[SerializeField]
				private UILabel _hitPointLabel = null;
				public UILabel HitPointLabel { get { return _hitPointLabel; } }

				/// <summary>
				/// 攻撃力ラベル
				/// </summary>
				[SerializeField]
				private UILabel _attackLabel = null;
				public UILabel AttackLabel { get { return _attackLabel; } }

				/// <summary>
				/// 防御力ラベル
				/// </summary>
				[SerializeField]
				private UILabel _defenseLabel = null;
				public UILabel DefenseLabel { get { return _defenseLabel; } }

				/// <summary>
				/// 特殊能力ラベル
				/// </summary>
				[SerializeField]
				private UILabel _extraLabel = null;
				public UILabel ExtraLabel { get { return _extraLabel; } }
			}

			/// <summary>
			/// ロックオブジェクト
			/// </summary>
			[SerializeField]
			private LockAttachObject _lockAttach = null;
			private LockAttachObject LockAttach { get { return _lockAttach; } }
			[Serializable]
			private class LockAttachObject
			{
				/// <summary>
				/// ロックスONプライト
				/// </summary>
				[SerializeField]
				private UISprite _onSprite = null;
				public UISprite OnSprite { get { return _onSprite; } }

				/// <summary>
				/// ロックOFFスプライト
				/// </summary>
				[SerializeField]
				private UISprite _offSprite = null;
				public UISprite OffSprite { get { return _offSprite; } }

				/// <summary>
				/// ロックOFFの背景スプライト
				/// </summary>
				[SerializeField]
				private UISprite _offBGSprite = null;
				public UISprite OffBGSprite { get { return _offBGSprite; } }
			}

			/// <summary>
			/// 簡易情報ウィンドウのウェジェット
			/// </summary>
			[SerializeField]
			private UIWidget _windowWidget = null;
			public UIWidget WindowWidget { get { return _windowWidget; } }

			/// <summary>
			/// 位置
			/// </summary>
			public Vector3 Position { set { this.transform.position = value; } }

			/// <summary>
			/// ウィンドウの幅
			/// </summary>
			public int Width { get { return (WindowWidget != null) ? WindowWidget.width : 0; } }

			/// <summary>
			/// ウィンドウの高さ
			/// </summary>
			public int Height { get { return (WindowWidget != null) ? WindowWidget.height : 0; } }

			/// <summary>
			/// ウィンドウの四隅のワールド座標
			/// </summary>
			public Vector3[] WorldCorners { get { return (WindowWidget != null) ? WindowWidget.worldCorners : new Vector3[4]; } }

			/// <summary>
			/// 背景側が押された時の通知用
			/// </summary>
			public event EventHandler OnBGClickEvent = (sender, e) => { };

			/// <summary>
			/// お気に入りボタンが押され時の通知用
			/// </summary>
			public event EventHandler OnFavoriteClickEvent = (sender, e) => { };

			/// <summary>
			/// 詳細ボタンが押された時の通知用
			/// </summary>
			public event EventHandler OnCharaInfoClickEvent = (sender, e) => { };
			#endregion

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
			public void OnDestroy()
			{
				this.OnBGClickEvent = null;
				this.OnFavoriteClickEvent = null;
				this.OnCharaInfoClickEvent = null;
			}
			#endregion

			#region キャラ名
			/// <summary>
			/// キャラ名の設定
			/// </summary>
			/// <param name="charaName"></param>
			public void SetCharaName(string charaName)
			{
				if (this.CharaNameLabel == null) { return; }
				this.CharaNameLabel.text = charaName;
			}
			#endregion

			#region ランク
			/// <summary>
			/// ランクの設定
			/// </summary>
			/// <param name="rank"></param>
			public void SetRank(string rank)
			{
				if (this.RankLabel == null) { return; }
				this.RankLabel.text = rank;
			}
			#endregion

			#region レベル
			/// <summary>
			/// 強化レベルの設定
			/// </summary>
			/// <param name="level"></param>
			/// <param name="synchroLevel"></param>
			public void SetLevel(string level)
			{
				if (this.LevelLabel == null) { return; }
				this.LevelLabel.text = level;
			}
			#endregion

			#region ステータス
			/// <summary>
			/// 生命力の設定
			/// </summary>
			/// <param name="hitPoint"></param>
			public void SetHitPoint(string hitPoint)
			{
				if (this.StatusAttach == null) { return; }

				if (this.StatusAttach.HitPointLabel != null)
				{
					this.StatusAttach.HitPointLabel.text = hitPoint;
				}
			}

			/// <summary>
			/// 攻撃力の設定
			/// </summary>
			/// <param name="attack"></param>
			/// <param name="synchroAttack"></param>
			public void SetAttack(string attack)
			{
				if (this.StatusAttach == null) { return; }

				if (this.StatusAttach.AttackLabel != null)
				{
					this.StatusAttach.AttackLabel.text = attack;
				}
			}

			/// <summary>
			/// 防御力の設定
			/// </summary>
			/// <param name="defense"></param>
			/// <param name="synchroDefense"></param>
			public void SetDefense(string defense)
			{
				if (this.StatusAttach == null) { return; }

				if (this.StatusAttach.DefenseLabel != null)
				{
					this.StatusAttach.DefenseLabel.text = defense;
				}
			}

			/// <summary>
			/// 特殊能力の設定
			/// </summary>
			/// <param name="extra"></param>
			/// <param name="synchroExtra"></param>
			public void SetExtra(string extra)
			{
				if (this.StatusAttach == null) { return; }

				if (this.StatusAttach.ExtraLabel != null)
				{
					this.StatusAttach.ExtraLabel.text = extra;
				}
			}
			#endregion

			#region ロック
			/// <summary>
			/// ロックのアクティブ設定
			/// </summary>
			public void SetLockActive(bool isLock)
			{
				if (this.LockAttach == null) { return; }

				if(this.LockAttach.OnSprite != null)
				{
					this.LockAttach.OnSprite.gameObject.SetActive(!isLock);
				}
				if(this.LockAttach.OffSprite != null)
				{
					this.LockAttach.OffSprite.gameObject.SetActive(isLock);
				}
				if(this.LockAttach.OffBGSprite != null)
				{
					this.LockAttach.OffBGSprite.gameObject.SetActive(!isLock);
				}
			}
			#endregion

			#region NGUIリフレクション
			/// <summary>
			/// 背景側が押された時
			/// </summary>
			public void OnBGClick()
			{
				// 通知
				this.OnBGClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// お気に入りボタンが押された時
			/// </summary>
			public void OnFavoriteClick()
			{
				// 通知
				this.OnFavoriteClickEvent(this, EventArgs.Empty);
			}

			/// <summary>
			/// 詳細ボタンが押された時
			/// </summary>
			public void OnCharaInfoClick()
			{
				// 通知
				this.OnCharaInfoClickEvent(this, EventArgs.Empty);
			}
			#endregion

		}
	}
}