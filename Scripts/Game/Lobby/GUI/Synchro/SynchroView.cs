/// <summary>
/// シンクロ合成表示
/// 
/// 2016/02/24
/// </summary>
using UnityEngine;
using System;

namespace XUI
{
	namespace Synchro
	{
		#region イベント引数
		public class HomeClickedEventArgs : EventArgs { }
		public class CloseClickedEventArgs : EventArgs { }
		#endregion

		/// <summary>
		/// シンクロ合成表示インターフェイス
		/// </summary>
		public interface IView
		{
			#region ホーム/閉じる
			// ホーム、閉じるイベント通知用
			event EventHandler<HomeClickedEventArgs> OnHome;
			event EventHandler<CloseClickedEventArgs> OnClose;
			#endregion

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

			#region 所持金/費用
			/// <summary>
			/// 所持金設定
			/// </summary>
			/// <param name="money"></param>
			/// <param name="format"></param>
			void SetHaveMoney(int money, string format);

			/// <summary>
			/// 費用設定
			/// </summary>
			/// <param name="money"></param>
			/// <param name="format"></param>
			void SetNeedMoney(int money, string format);
			#endregion

			#region 合成ボタン
			/// <summary>
			/// 合成ボタンイベント通知
			/// </summary>
			event EventHandler OnFusion;

			/// <summary>
			/// 合成ボタンの有効化
			/// </summary>
			void SetFusionButtonEnable(bool isEnable);
			#endregion

			#region ベースキャラステータス
			#region 生命力
			/// <summary>
			/// 生命力の設定
			/// </summary>
			void SetHitPoint(string synchroHitPoint);

			/// <summary>
			/// 生命力の増加設定
			/// </summary>
			void SetHitPointUp(string up);

			/// <summary>
			/// 生命力カラー設定
			/// </summary>
			void SetHitPointColor(StatusColor.Type type);
			#endregion

			#region 攻撃力
			/// <summary>
			/// 攻撃力の設定
			/// </summary>
			void SetAttack(string synchroAttack);

			/// <summary>
			/// 攻撃力の増加設定
			/// </summary>
			void SetAttackUp(string up);

			/// <summary>
			/// 攻撃力カラー設定
			/// </summary>
			void SetAttackColor(StatusColor.Type type);
			#endregion

			#region 防御力
			/// <summary>
			/// 防御力の設定
			/// </summary>
			void SetDefense(string synchroDefense);

			/// <summary>
			/// 防御力の増加設定
			/// </summary>
			void SetDefenseUp(string up);

			/// <summary>
			/// 防御力カラー設定
			/// </summary>
			void SetDefenseColor(StatusColor.Type type);
			#endregion

			#region 特殊能力
			/// <summary>
			/// 特殊能力の設定
			/// </summary>
			void SetExtra(string synchroExtra);

			/// <summary>
			/// 特殊能力の増加設定
			/// </summary>
			void SetExtraUp(string up);

			/// <summary>
			/// 特殊能力カラー設定
			/// </summary>
			void SetExtraColor(StatusColor.Type type);
			#endregion

			/// <summary>
			/// 合計強化量
			/// </summary>
			void SetTotalSynchroBonus(string total);
			#endregion

			#region シンクロ回数
			/// <summary>
			/// シンクロ合成残り回数の設定
			/// </summary>
			void SetSynchroRemain(string remain);

			/// <summary>
			/// シンクロ合成残り回数減少の設定
			/// </summary>
			void SetSynchroRemainDown(string up);

			/// <summary>
			/// シンクロ合成残り回数減少カラー設定
			/// </summary>
			void SetSynchroRemainDownColor(StatusColor.Type type);
			#endregion

			#region 素材
			/// <summary>
			/// 素材を隠すための表示設定
			/// </summary>
			void SetFillMaterialActive(bool isActive);
			#endregion

			#region 残り強化値
			/// <summary>
			/// 残り強化値の設定
			/// </summary>
			void SetTotalSynchroBonusRemain(string remain);
			#endregion
		}

		/// <summary>
		/// シンクロ合成表示
		/// </summary>
		public class SynchroView : GUIScreenViewBase, IView
		{
			#region 破棄
			/// <summary>
			/// 破棄
			/// </summary>
			void OnDestroy()
			{
				this.OnHome = null;
				this.OnClose = null;
				this.OnFusion = null;

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

			#region ホーム、閉じるボタンイベント
			/// <summary>
			/// ホーム、閉じるイベント通知用
			/// </summary>
			public event EventHandler<HomeClickedEventArgs> OnHome = (sender, e) => { };
			public event EventHandler<CloseClickedEventArgs> OnClose = (sender, e) => { };

			/// <summary>
			/// ホームボタンイベント
			/// </summary>
			public override void OnHomeEvent()
			{
				// 通知
				var eventArgs = new HomeClickedEventArgs();
				this.OnHome(this, eventArgs);
			}

			/// <summary>
			/// 閉じるボタンイベント
			/// </summary>
			public override void OnCloseEvent()
			{
				// 通知
				var eventArgs = new CloseClickedEventArgs();
				this.OnClose(this, eventArgs);
			}
			#endregion

			#region 所持金
			[SerializeField]
			private UILabel _haveMoneyLabel = null;
			private UILabel HaveMoneyLabel { get { return _haveMoneyLabel; } }

			/// <summary>
			/// 所持金設定
			/// </summary>
			public void SetHaveMoney(int money, string format)
			{
				if (this.HaveMoneyLabel != null)
				{
					this.HaveMoneyLabel.text = string.Format(format, money);
				}
			}
			#endregion

			#region 費用
			[SerializeField]
			private UILabel _needMoneyLabel = null;
			private UILabel NeedMoneyLabel { get { return _needMoneyLabel; } }

			/// <summary>
			/// 費用設定
			/// </summary>
			public void SetNeedMoney(int money, string format)
			{
				if (this.NeedMoneyLabel != null)
				{
					this.NeedMoneyLabel.text = string.Format(format, money);
				}
			}
			#endregion

			#region 合成ボタン
			/// <summary>
			/// イベント通知
			/// </summary>
			public event EventHandler OnFusion = (sender, e) => { };

			/// <summary>
			/// 合成ボタンイベント
			/// </summary>
			public void OnFusionEvent()
			{
				// 通知
				this.OnFusion(this, EventArgs.Empty);
			}

			/// <summary>
			/// 合成ボタン
			/// </summary>
			[SerializeField]
			private UIButton _fusionButton = null;
			private UIButton FusionButton { get { return this._fusionButton; } }

			/// <summary>
			/// 合成ボタンの有効化
			/// </summary>
			/// <param name="isEnable"></param>
			public void SetFusionButtonEnable(bool isEnable)
			{
				if (this.FusionButton == null) { return; }
				this.FusionButton.isEnabled = isEnable;
			}
			#endregion

			#region ベースキャラステータス
			/// <summary>
			/// 生命力
			/// </summary>
			[SerializeField]
			private StatusAttachObject _hitPointAttach = null;
			private StatusAttachObject HitPointAttach { get { return _hitPointAttach; } }

			/// <summary>
			/// 攻撃力
			/// </summary>
			[SerializeField]
			private StatusAttachObject _attackAttach = null;
			private StatusAttachObject AttackAttach { get { return _attackAttach; } }

			/// <summary>
			/// 防御力
			/// </summary>
			[SerializeField]
			private StatusAttachObject _defenseAttach = null;
			private StatusAttachObject DefenseAttach { get { return _defenseAttach; } }

			/// <summary>
			/// 特殊能力
			/// </summary>
			[SerializeField]
			private StatusAttachObject _extraAttach = null;
			private StatusAttachObject ExtraAttach { get { return _extraAttach; } }

			/// <summary>
			/// ベースキャラステータスアタッチオブジェクト
			/// </summary>
			[Serializable]
			public class StatusAttachObject
			{
				/// <summary>
				/// ベースキャラのポイント
				/// </summary>
				[SerializeField]
				private UILabel _basePointLabel = null;
				public UILabel BasePointLabel { get { return _basePointLabel; } }

				/// <summary>
				/// ベースキャラのポイント(Grow)
				/// </summary>
				[SerializeField]
				private UILabel _basePointGrowLabel = null;
				public UILabel BasePointGrowLabel { get { return _basePointGrowLabel; } }

				/// <summary>
				/// アップ分のポイント
				/// </summary>
				[SerializeField]
				private UILabel _pointUpLabel = null;
				public UILabel PointUpLabel { get { return _pointUpLabel; } }

				/// アップ分のポイント(Grow)
				/// </summary>
				[SerializeField]
				private UILabel _pointUpGrowLabel = null;
				public UILabel PointUpGrowLabel { get { return _pointUpGrowLabel; } }
			}

			#region 生命力
			/// <summary>
			/// 生命力の設定
			/// </summary>
			public void SetHitPoint(string synchroHitPoint)
			{
				var hitPoint = this.HitPointAttach;
				if (hitPoint == null) { return; }

				if (hitPoint.BasePointLabel == null) { return; }
				hitPoint.BasePointLabel.text = synchroHitPoint;
			}

			/// <summary>
			/// 生命力の増加設定
			/// </summary>
			public void SetHitPointUp(string up)
			{
				var hitPoint = this.HitPointAttach;
				if (hitPoint == null) { return; }

				if (hitPoint.PointUpLabel == null) { return; }
				hitPoint.PointUpLabel.text = up;
			}

			/// <summary>
			/// 生命力カラー設定
			/// </summary>
			public void SetHitPointColor(StatusColor.Type type)
			{
				var t = this.HitPointAttach;
				if (t == null) { return; }

				StatusColor.Set(type, t.BasePointLabel, t.BasePointGrowLabel);
				StatusColor.Set(type, t.PointUpLabel, t.PointUpGrowLabel);
			}
			#endregion

			#region 攻撃力
			/// <summary>
			/// 攻撃力の設定
			/// </summary>
			public void SetAttack(string synchroAttack)
			{
				var attack = this.AttackAttach;
				if (attack == null) { return; }

				if (attack.BasePointLabel == null) { return; }
				attack.BasePointLabel.text = synchroAttack;
			}

			/// <summary>
			/// 攻撃力の増加設定
			/// </summary>
			public void SetAttackUp(string up)
			{
				var attack = this.AttackAttach;
				if (attack == null) { return; }

				if (attack.PointUpLabel == null) { return; }
				attack.PointUpLabel.text = up;
			}

			/// <summary>
			/// 攻撃力カラー設定
			/// </summary>
			public void SetAttackColor(StatusColor.Type type)
			{
				var t = this.AttackAttach;
				if (t == null) { return; }

				StatusColor.Set(type, t.BasePointLabel, t.BasePointGrowLabel);
				StatusColor.Set(type, t.PointUpLabel, t.PointUpGrowLabel);
			}
			#endregion

			#region 防御力
			/// <summary>
			/// 防御力の設定
			/// </summary>
			public void SetDefense(string synchroDefense)
			{
				var defense = this.DefenseAttach;
				if (defense == null) { return; }

				if (defense.BasePointLabel == null) { return; }
				defense.BasePointLabel.text = synchroDefense;
			}

			/// <summary>
			/// 防御力の増加設定
			/// </summary>
			public void SetDefenseUp(string up)
			{
				var defense = this.DefenseAttach;
				if (defense == null) { return; }

				if (defense.PointUpLabel == null) { return; }
				defense.PointUpLabel.text = up;
			}

			/// <summary>
			/// 防御力カラー設定
			/// </summary>
			public void SetDefenseColor(StatusColor.Type type)
			{
				var t = this.DefenseAttach;
				if (t == null) { return; }

				StatusColor.Set(type, t.BasePointLabel, t.BasePointGrowLabel);
				StatusColor.Set(type, t.PointUpLabel, t.PointUpGrowLabel);
			}
			#endregion

			#region 特殊能力
			/// <summary>
			/// 特殊能力の設定
			/// </summary>
			public void SetExtra(string synchroExtra)
			{
				var extra = this.ExtraAttach;
				if (extra == null) { return; }

				if (extra.BasePointLabel == null) { return; }
				extra.BasePointLabel.text = synchroExtra;
			}

			/// <summary>
			/// 特殊能力の増加設定
			/// </summary>
			public void SetExtraUp(string up)
			{
				var extra = this.ExtraAttach;
				if (extra == null) { return; }

				if (extra.PointUpLabel == null) { return; }
				extra.PointUpLabel.text = up;
			}

			/// <summary>
			/// 特殊能力カラー設定
			/// </summary>
			public void SetExtraColor(StatusColor.Type type)
			{
				var t = this.ExtraAttach;
				if (t == null) { return; }

				StatusColor.Set(type, t.BasePointLabel, t.BasePointGrowLabel);
				StatusColor.Set(type, t.PointUpLabel, t.PointUpGrowLabel);
			}
			#endregion

			#region シンクロ回数
			/// <summary>
			/// シンクロ回数アタッチ
			/// </summary>
			[SerializeField]
			private SynchroRemainAttachObject _synchroRemainAttach = null;
			private SynchroRemainAttachObject SynchroRemainAttach { get { return _synchroRemainAttach; } }
			[Serializable]
			public class SynchroRemainAttachObject
			{
				/// <summary>
				/// 残り回数ラベル
				/// </summary>
				[SerializeField]
				private UILabel _baseRemainLabel = null;
				public UILabel BaseRemainLabel { get { return _baseRemainLabel; } }

				/// <summary>
				/// 残り回数減少分ラベル
				/// </summary>
				[SerializeField]
				private UILabel _remainDownLabel = null;
				public UILabel RemainDownLabel { get { return _remainDownLabel; } }

				/// <summary>
				/// 残り回数減少分ラベル(Grow)
				/// </summary>
				[SerializeField]
				private UILabel _remainDownGrowLabel = null;
				public UILabel RemainDownGrowLabel { get { return _remainDownGrowLabel; } }

				/// <summary>
				/// 矢印
				/// </summary>
				[SerializeField]
				private GameObject _arrowObj = null;
				public GameObject ArrowObj { get { return _arrowObj; } }
			}

			/// <summary>
			/// シンクロ合成残り回数の設定
			/// </summary>
			public void SetSynchroRemain(string remain)
			{
				var synchroRemain = this.SynchroRemainAttach;
				if (synchroRemain == null) { return; }

				if (synchroRemain.BaseRemainLabel == null) { return; }
				synchroRemain.BaseRemainLabel.text = remain;
			}

			/// <summary>
			/// シンクロ合成残り回数減少の設定
			/// </summary>
			public void SetSynchroRemainDown(string up)
			{
				var synchroRemain = this.SynchroRemainAttach;
				if (synchroRemain == null) { return; }

				if (synchroRemain.RemainDownLabel != null)
				{
					synchroRemain.RemainDownLabel.text = up;
				}

				// 矢印の表示設定
				bool isActive = !string.IsNullOrEmpty(up);
				if(synchroRemain.ArrowObj != null)
				{
					synchroRemain.ArrowObj.SetActive(isActive);
				}
			}

			/// <summary>
			/// シンクロ合成残り回数減少カラー設定
			/// </summary>
			public void SetSynchroRemainDownColor(StatusColor.Type type)
			{
				var t = this.SynchroRemainAttach;
				if (t == null) { return; }

				StatusColor.Set(type, t.RemainDownLabel, t.RemainDownGrowLabel);
			}
			#endregion

			[SerializeField]
			private UILabel _totalSynchroBonusLabel = null;
			private UILabel TotalSynchroBonusLabel { get { return _totalSynchroBonusLabel; } }

			/// <summary>
			/// 合計強化量
			/// </summary>
			public void SetTotalSynchroBonus(string total)
			{
				if(this.TotalSynchroBonusLabel != null)
				{
					this.TotalSynchroBonusLabel.text = total;
				}
			}
			#endregion

			#region 素材
			/// <summary>
			/// 素材を隠す表示物
			/// </summary>
			[SerializeField]
			private GameObject _fillMaterialObj = null;
			private GameObject FillMaterialObj { get { return _fillMaterialObj; } }

			/// <summary>
			/// 素材を隠すための表示設定
			/// </summary>
			public void SetFillMaterialActive(bool isActive)
			{
				if (this.FillMaterialObj == null) { return; }
				this.FillMaterialObj.SetActive(isActive);
			}
			#endregion

			#region 残り強化値
			/// <summary>
			/// 残り強化値
			/// </summary>
			[SerializeField]
			private UILabel _totalSynchroBonusRemainLabel = null;
			private UILabel TotalSynchroBonusRemainLabel { get { return _totalSynchroBonusRemainLabel; } }

			/// <summary>
			/// 残り強化値の設定
			/// </summary>
			public void SetTotalSynchroBonusRemain(string remain)
			{
				if (this.TotalSynchroBonusRemainLabel != null)
				{
					this.TotalSynchroBonusRemainLabel.text = remain;
				}
			}
			#endregion
		}
	}
}
