/// <summary>
/// シンクロ合成結果制御
/// 
/// 2016/03/08
/// </summary>
using UnityEngine;
using System;

namespace XUI.SynchroResult
{
	/// <summary>
	/// 初期化パラメータ
	/// </summary>
	public struct SetupParam
	{
		public AvatarType AvatarType { get; set; }
		public int HitPoint { get; set; }
		public int SynchroHitPoint { get; set; }
		public int HitPointUp { get; set; }
		public int Attack { get; set; }
		public int SynchroAttack { get; set; }
		public int AttackUp { get; set; }
		public int Defence { get; set; }
		public int SynchroDefence { get; set; }
		public int DefenceUp { get; set; }
		public int Extra { get; set; }
		public int SynchroExtra { get; set; }
		public int ExtraUp { get; set; }
	}

	/// <summary>
	/// シンクロ合成結果制御インターフェイス
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
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);

		/// <summary>
		/// 初期設定
		/// </summary>
		void Setup(SetupParam param);
	}

	/// <summary>
	/// シンクロ合成結果制御
	/// </summary>
	public class Controller : IController
	{
		#region フィールド&プロパティ
		/// <summary>
		/// モデル
		/// </summary>
		private readonly IModel _model;
		private IModel Model { get { return _model; } }

		/// <summary>
		/// ビュー
		/// </summary>
		private readonly IVIew _view;
		private IVIew View { get { return _view; } }

		/// <summary>
		/// 更新できる状態化どうか
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

		/// <summary>
		/// キャラボード
		/// </summary>
		private readonly CharaBoard _charaBoard;
		private CharaBoard CharaBoard { get { return _charaBoard; } }
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IVIew view, CharaBoard charaBoard)
		{
			if (model == null || view == null) { return; }

			this._charaBoard = charaBoard;

			// ビュー設定
			this._view = view;
			this.View.OnOK += this.HandleOK;

			// モデル設定
			this._model = model;
			// アバタータイプ
			this.Model.OnAvatarTypeChange += this.HandleAvatarTypeChange;
			// 生命力
			this.Model.OnHitPointChange += this.HandleHitPointChange;
			this.Model.OnHitPointFormatChange += this.HandleHitPointFormatChange;
			this.Model.OnSynchroHitPointFormatChange += this.HandleSynchroHitPointFormatChange;
			this.Model.OnHitPointUpFormatChange += this.HandleHitPointUpFormat;
			// 攻撃力
			this.Model.OnAttackChange += this.HandleAttackChange;
			this.Model.OnAttackFormatChange += this.HandleAttackFormatChange;
			this.Model.OnSynchroAttackFormatChange += this.HandleSynchroAttackFormatChange;
			this.Model.OnAttackUpFormatChange += this.HandleAttackUpFormatChange;
			// 防御力
			this.Model.OnDefenceChange += this.HandleDefenceChange;
			this.Model.OnDefenceFormatChange += this.HandleDefenceFormatChange;
			this.Model.OnSynchroDefenceFormatChange += this.HandleSynchroDefenceFormatChange;
			this.Model.OnDefenceUpFormatChange += this.HandleDefenceUpFormatChange;
			// 特殊能力
			this.Model.OnExtraChange += this.HandleExtraChange;
			this.Model.OnExtraFormatChange += this.HandleExtraFormatChange;
			this.Model.OnSynchroExtraFormatChange += this.HandleSynchroExtraFormatChange;
			this.Model.OnExtraUpFormatChange += this.HandleExtraUpFormatChange;
			// 合計シンクロボーナス
			this.Model.OnTotalSynchroBonusChange += this.HandleTotalSynchroBonus;

			// 同期
			this.SyncAvatarType();
			this.SyncHitPoint();
			this.SyncAttack();
			this.SyncDefence();
			this.SyncExtra();
			this.SyncTotalSynchroBonus();
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
		}

		/// <summary>
		/// 初期設定
		/// </summary>
		public void Setup(SetupParam param)
		{
			if (!this.CanUpdate) { return; }

			if(this.Model.AvatarType == param.AvatarType)
			{
				this.View.ReplayBoard(true);
			}
			this.Model.AvatarType = param.AvatarType;
			this.Model.SetHitPoint(param.HitPoint, param.SynchroHitPoint, param.HitPointUp);
			this.Model.SetAttack(param.Attack, param.SynchroAttack, param.AttackUp);
			this.Model.SetDefence(param.Defence, param.SynchroDefence, param.DefenceUp);
			this.Model.SetExtra(param.Extra, param.SynchroExtra, param.ExtraUp);
			this.Model.TotalSynchroBonus = param.SynchroHitPoint + param.SynchroAttack + param.SynchroDefence + param.SynchroExtra;
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
		/// <summary>
		/// OKボタンが押された時に呼び出される
		/// </summary>
		private void HandleOK(object sender, EventArgs e)
		{
			GUIController.Back();
		}
		#endregion

		#region アバタータイプ
		/// <summary>
		/// アバタータイプが変更された時に呼び出される
		/// </summary>
		private void HandleAvatarTypeChange(object sender, EventArgs e)
		{
			this.SyncAvatarType();
		}
		/// <summary>
		/// アバタータイプ同期
		/// </summary>
		private void SyncAvatarType()
		{
			if (!this.CanUpdate || this.CharaBoard == null) { return; }

			if (this.Model.AvatarType != AvatarType.None)
			{
				this.CharaBoard.GetBoard(this.Model.AvatarType, this.Model.SkinId, false,
					(resource) =>
					{
						this.CreateBoard(resource, this.Model.AvatarType);
					});
			}
		}
		/// <summary>
		/// 立ち絵生成
		/// </summary>
		private void CreateBoard(GameObject resource, AvatarType avatarType)
		{
			if (!this.CanUpdate) { return; }

			// リソース読み込み完了
			if (resource == null) { return; }
			// インスタンス化
			var go = SafeObject.Instantiate(resource) as GameObject;
			if (go == null) { return; }

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

		#region 生命力
		#region ハンドラー
		private void HandleHitPointChange(object sender, EventArgs e)
		{
			this.SyncHitPoint();
		}
		private void HandleHitPointFormatChange(object sender, EventArgs e)
		{
			this.SyncHitPoint();
		}
		private void HandleSynchroHitPointFormatChange(object sender, EventArgs e)
		{
			this.SyncHitPoint();
		}
		private void HandleHitPointUpFormat(object sender, EventArgs e)
		{
			this.SyncHitPoint();
		}
		#endregion

		/// <summary>
		/// 生命力同期
		/// </summary>
		private void SyncHitPoint()
		{
			if (!this.CanUpdate) { return; }
			string upFormat;
			bool isUpEffect;
			if (this.Model.HitPointUp == 0)
			{
				upFormat = "";
				isUpEffect = false;
			}
			else
			{
				upFormat = this.Model.HitPointUpFormat;
				isUpEffect = true;
			}
			this.View.SetSynchroHitPoint(this.Model.SynchroHitPoint, this.Model.SynchroHitPointFormat, isUpEffect);
			this.View.SetHitPointUp(this.Model.HitPointUp, upFormat);
		}
		#endregion

		#region 攻撃力
		#region ハンドラー
		private void HandleAttackChange(object sender, EventArgs e)
		{
			this.SyncAttack();
		}
		private void HandleAttackFormatChange(object sender, EventArgs e)
		{
			this.SyncAttack();
		}
		private void HandleSynchroAttackFormatChange(object sender, EventArgs e)
		{
			this.SyncAttack();
		}
		private void HandleAttackUpFormatChange(object sender, EventArgs e)
		{
			this.SyncAttack();
		}
		#endregion

		/// <summary>
		/// 攻撃力同期
		/// </summary>
		private void SyncAttack()
		{
			if (!this.CanUpdate) { return; }
			string upFormat;
			bool isUpEffect;
			if (this.Model.AttackUp == 0)
			{
				upFormat = "";
				isUpEffect = false;
			}
			else
			{
				upFormat = this.Model.AttackUpFormat;
				isUpEffect = true;
			}
			this.View.SetSynchroAttack(this.Model.SynchroAttack, this.Model.SynchroAttackFormat, isUpEffect);
			this.View.SetAttackUp(this.Model.AttackUp, upFormat);
		}
		#endregion

		#region 防御力
		#region　ハンドラー
		private void HandleDefenceChange(object sender, EventArgs e)
		{
			this.SyncDefence();
		}
		private void HandleDefenceFormatChange(object sender, EventArgs e)
		{
			this.SyncDefence();
		}
		private void HandleSynchroDefenceFormatChange(object sender, EventArgs e)
		{
			this.SyncDefence();
		}
		private void HandleDefenceUpFormatChange(object sender, EventArgs e)
		{
			this.SyncDefence();
		}
		#endregion

		/// <summary>
		/// 防御力同期
		/// </summary>
		private void SyncDefence()
		{
			if (!this.CanUpdate) { return; }
			string upFormat;
			bool isUpEffect;
			if (this.Model.DefenceUp == 0)
			{
				upFormat = "";
				isUpEffect = false;
			}
			else
			{
				upFormat = this.Model.DefenceUpFormat;
				isUpEffect = true;
			}
			this.View.SetSynchroDefence(this.Model.SynchroDefence, this.Model.SynchroDefenceFormat, isUpEffect);
			this.View.SetDefenceUp(this.Model.DefenceUp, upFormat);
		}
		#endregion

		#region 特殊能力
		#region ハンドラー
		private void HandleExtraChange(object sender, EventArgs e)
		{
			this.SyncExtra();
		}
		private void HandleExtraFormatChange(object sender, EventArgs e)
		{
			this.SyncExtra();
		}
		private void HandleSynchroExtraFormatChange(object sender, EventArgs e)
		{
			this.SyncExtra();
		}
		private void HandleExtraUpFormatChange(object sender, EventArgs e)
		{
			this.SyncExtra();
		}
		#endregion

		/// <summary>
		/// 特殊能力同期
		/// </summary>
		private void SyncExtra()
		{
			if (!this.CanUpdate) { return; }
			string upFormat;
			bool isUpEffect;
			if (this.Model.ExtraUp == 0)
			{
				upFormat = "";
				isUpEffect = false;
			}
			else
			{
				upFormat = this.Model.ExtraUpFormat;
				isUpEffect = true;
			}
			this.View.SetSynchroExtra(this.Model.SynchroExtra, this.Model.SynchroExtraFormat, isUpEffect);
			this.View.SetExtraUp(this.Model.ExtraUp, upFormat);
		}
		#endregion

		#region 合計シンクロボーナス
		/// <summary>
		/// 合計シンクロボーナスハンドラー
		/// </summary>
		private void HandleTotalSynchroBonus(object sender, EventArgs e)
		{
			this.SyncTotalSynchroBonus();
		}

		/// <summary>
		/// 合計シンクロボーナス同期
		/// </summary>
		private void SyncTotalSynchroBonus()
		{
			if (!CanUpdate) { return; }

			string msg = string.Empty;
			if(this.Model.TotalSynchroBonus >= CharaInfo.GetTotalMaxSynchroBonus())
			{
				// ボーナス値が最大まで強化していればメッセージを表示
				msg = MasterData.GetText(TextType.TX335_SynchroResult_BonusMax_Message);
			}
			this.View.SetWarningMessage(msg);
		}
		#endregion
	}
}
