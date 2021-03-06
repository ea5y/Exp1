/// <summary>
/// 強化合成結果制御
/// 
/// 2016/01/28
/// </summary>
using System;
using UnityEngine;
using System.Collections.Generic;

using Scm.Common.GameParameter;

namespace XUI
{
	namespace PowerupResult
	{
		/// <summary>
		/// 初期化パラメータ
		/// </summary>
		public struct SetupParam
		{
			public AvatarType AvatarType { get; set; }
			public int BeforeLv { get; set; }
			public int AfterLv { get; set; }
			public int Exp { get; set; }
			public int TotalExp { get; set; }
			public int NextLvTotalExp { get; set; }
			public int HitPoint { get; set; }
			public int HitPointUp { get; set; }
			public int Attack { get; set; }
			public int AttackUp { get; set; }
			public int Defence { get; set; }
			public int DefenceUp { get; set; }
			public int Extra { get; set; }
			public int ExtraUp { get; set; }
		}

		/// <summary>
		/// 強化合成結果制御インターフェイス
		/// </summary>
		public interface IController
		{
			/// <summary>
			/// 更新できる状態かどうか
			/// </summary>
			bool CanUpdate { get; }

			/// <summary>
			/// アクティブ設定
			/// </summary>
			void SetActive(bool isActive, bool isTweenSkip);

			/// <summary>
			/// 初期設定
			/// </summary>
			void Setup(SetupParam p);
		}

		/// <summary>
		/// 強化合成結果制御
		/// </summary>
		public class Controller : IController
		{
			#region フィールド＆プロパティ
			// モデル
			readonly IModel _model;
			IModel Model { get { return _model; } }
			// ビュー
			readonly IView _view;
			IView View { get { return _view; } }
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

			// キャラボード
			readonly CharaBoard _charaBoard;
			CharaBoard CharaBoard { get { return _charaBoard; } }
			#endregion

			#region 初期化
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public Controller(IModel model, IView view, CharaBoard charaBoard)
			{
				if (model == null || view == null) return;

				this._charaBoard = charaBoard;

				// ビュー設定
				this._view = view;
				this.View.OnOK += this.HandleOK;

				// モデル設定
				this._model = model;
				// アバタータイプ
				this.Model.OnAvatarTypeChange += this.HandleAvatarTypeChange;
				// レベル
				this.Model.OnLvChange += this.HandleLvChange;
				this.Model.OnBeforeLvFormatChange += this.HandleBeforeLvFormatChange;
				this.Model.OnAfterLvFormatChange += this.HandleAfterLvFormatChange;
				// 経験値バー
				this.Model.OnExpSliderChange += this.HandleExpSliderChange;
				// 生命力
				this.Model.OnHitPointChange += this.HandleHitPointChange;
				this.Model.OnHitPointFormatChange += this.HandleHitPointFormatChange;
				this.Model.OnHitPointUpFormatChange += this.HandleHitPointUpFormatChange;
				//攻撃力
				this.Model.OnAttackChange += this.HandleAttackChange;
				this.Model.OnAttackFormatChange += this.HandleAttackFormatChange;
				this.Model.OnAttackUpFormatChange += this.HandleAttackUpFormatChange;
				// 防御力
				this.Model.OnDefenceChange += this.HandleDefenceChange;
				this.Model.OnDefenceFormatChange += this.HandleDefenceFormatChange;
				this.Model.OnDefenceUpFormatChange += this.HandleDefenceUpFormatChange;
				// 特殊能力
				this.Model.OnExtraChange += this.HandleExtraChange;
				this.Model.OnExtraFormatChange += this.HandleExtraFormatChange;
				this.Model.OnExtraUpFormatChange += this.HandleExtraUpFormatChange;

				// 同期
				this.SyncAvatarType();
				this.SyncLv();
				this.SyncExpSlider();
				this.SyncHitPoint();
				this.SyncAttack();
				this.SyncDefence();
				this.SyncExtra();
			}
			/// <summary>
			/// 初期設定
			/// </summary>
			public void Setup(SetupParam p)
			{
				if (this.CanUpdate)
				{
					if (this.Model.AvatarType == p.AvatarType)
					{
						this.View.ReplayBoard(true);
					}
					this.Model.AvatarType = p.AvatarType;
					this.Model.SetLv(p.BeforeLv, p.AfterLv);
					this.Model.SetExpSlider(p.Exp, p.TotalExp, p.NextLvTotalExp);
					this.Model.SetHitPoint(p.HitPoint, p.HitPointUp);
					this.Model.SetAttack(p.Attack, p.AttackUp);
					this.Model.SetDefence(p.Defence, p.DefenceUp);
					this.Model.SetExtra(p.Extra, p.ExtraUp);
				}
			}
			#endregion

			#region アクティブ設定
			/// <summary>
			/// アクティブ設定
			/// </summary>
			public void SetActive(bool isActive, bool isTweenSkip)
			{
				if (this.CanUpdate)
				{
					this.View.SetActive(isActive, isTweenSkip);
				}
			}
			#endregion

			#region OKボタンイベント
			void HandleOK(object sender, EventArgs e)
			{
				GUIController.Back();
			}
			#endregion

			#region アバタータイプ
			void HandleAvatarTypeChange(object sender, EventArgs e) { this.SyncAvatarType(); }
			void SyncAvatarType()
			{
				if (this.CanUpdate && this.CharaBoard != null)
				{
					if (this.Model.AvatarType != AvatarType.None)
					{
						this.CharaBoard.GetBoard(this.Model.AvatarType, this.Model.SkinId, false,
							(resource) =>
							{
								this.CreateBoard(resource, this.Model.AvatarType);
							});
					}
				}
			}
			void CreateBoard(GameObject resource, AvatarType avatarType)
			{
				if (!this.CanUpdate) return;

				// リソース読み込み完了
				if (resource == null) return;
				// インスタンス化
				var go = SafeObject.Instantiate(resource) as GameObject;
				if (go == null) return;

				// 読み込み中に別のキャラに変更していたら破棄する
				if (this.Model.AvatarType != avatarType)
				{
					UnityEngine.Object.Destroy(go);
					return;
				}

				// 名前設定
				go.name = resource.name;
				// 親子付け
				var t = go.transform;
				this.View.SetBoardRoot(t);
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;
				this.View.ReplayBoard(true);
			}
			#endregion

			#region レベル
			void HandleLvChange(object sender, EventArgs e) { this.SyncLv(); }
			void HandleBeforeLvFormatChange(object sender, EventArgs e) { this.SyncLv(); }
			void HandleAfterLvFormatChange(object sender, EventArgs e) { this.SyncLv(); }
			void SyncLv()
			{
				if (this.CanUpdate)
				{
					this.View.SetBeforeLv(this.Model.BeforeLv, this.Model.BeforeLvFormat);

					// レベルアップ表示
					var isLvUp = (this.Model.BeforeLv < this.Model.AfterLv);
					var format = (!isLvUp ? "" : this.Model.AfterLvFormat);
					this.View.SetAfterLv(this.Model.AfterLv, format);
					this.View.SetLvUpActive(isLvUp);
				}
			}
			#endregion

			#region 経験値バー
			void HandleExpSliderChange(object sender, EventArgs e) { this.SyncExpSlider(); }
			void SyncExpSlider()
			{
				if (this.CanUpdate)
				{
					this.View.SetExpSlider(this.Model.GetExpSlider());
				}
			}
			#endregion

			#region 生命力
			void HandleHitPointChange(object sender, EventArgs e) { this.SyncHitPoint(); }
			void HandleHitPointFormatChange(object sender, EventArgs e) { this.SyncHitPoint(); }
			void HandleHitPointUpFormatChange(object sender, EventArgs e) { this.SyncHitPoint(); }
			void SyncHitPoint()
			{
				if (this.CanUpdate)
				{
					this.View.SetHitPoint(this.Model.HitPoint, this.Model.HitPointFormat);

					var format = (this.Model.HitPointUp == 0 ? "" : this.Model.HitPointUpFormat);
					this.View.SetHitPointUp(this.Model.HitPointUp, format);
				}
			}
			#endregion

			#region 攻撃力
			void HandleAttackChange(object sender, EventArgs e) { this.SyncAttack(); }
			void HandleAttackFormatChange(object sender, EventArgs e) { this.SyncAttack(); }
			void HandleAttackUpFormatChange(object sender, EventArgs e) { this.SyncAttack(); }
			void SyncAttack()
			{
				if (this.CanUpdate)
				{
					this.View.SetAttack(this.Model.Attack, this.Model.AttackFormat);

					var format = (this.Model.AttackUp == 0 ? "" : this.Model.AttackUpFormat);
					this.View.SetAttackUp(this.Model.AttackUp, format);
				}
			}
			#endregion

			#region 防御力
			void HandleDefenceChange(object sender, EventArgs e) { this.SyncDefence(); }
			void HandleDefenceFormatChange(object sender, EventArgs e) { this.SyncDefence(); }
			void HandleDefenceUpFormatChange(object sender, EventArgs e) { this.SyncDefence(); }
			void SyncDefence()
			{
				if (this.CanUpdate)
				{
					this.View.SetDefence(this.Model.Defence, this.Model.DefenceFormat);

					var format = (this.Model.DefenceUp == 0 ? "" : this.Model.DefenceUpFormat);
					this.View.SetDefenceUp(this.Model.DefenceUp, format);
				}
			}
			#endregion

			#region 特殊能力
			void HandleExtraChange(object sender, EventArgs e) { this.SyncExtra(); }
			void HandleExtraFormatChange(object sender, EventArgs e) { this.SyncExtra(); }
			void HandleExtraUpFormatChange(object sender, EventArgs e) { this.SyncExtra(); }
			void SyncExtra()
			{
				if (this.CanUpdate)
				{
					this.View.SetExtra(this.Model.Extra, this.Model.ExtraFormat);

					var format = (this.Model.ExtraUp == 0 ? "" : this.Model.ExtraUpFormat);
					this.View.SetExtraUp(this.Model.ExtraUp, format);
				}
			}
			#endregion
		}
	}
}
