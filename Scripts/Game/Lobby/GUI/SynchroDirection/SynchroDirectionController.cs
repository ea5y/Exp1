using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

namespace XUI.SynchroDirection
{


	/// <summary>
	/// 強化演出パラメータ
	/// </summary>
	[Serializable]
	public class SynchroDirectionParam
	{
		[SerializeField]
		private int baseCharaId = 0;

		[SerializeField]
		private int baitCharaId = 0;

		[SerializeField]
		private int hpUp = 0;

		[SerializeField]
		private bool isHpMax = false;

		[SerializeField]
		private int atkUp = 0;

		[SerializeField]
		private bool isAtkMax = false;

		[SerializeField]
		private int defUp = 0;

		[SerializeField]
		private bool isDefMax = false;

		[SerializeField]
		private int exUp = 0;

		[SerializeField]
		private bool isExMax = false;

		[SerializeField]
		private Scm.Common.GameParameter.PowerupResult result = Scm.Common.GameParameter.PowerupResult.Good;



		/// <summary>
		/// ベースキャラID
		/// </summary>
		public int BaseCharaId
		{
			get { return baseCharaId; }
			set { baseCharaId = value; }
		}

		public int BaitCharaId
		{
			get { return baitCharaId; }
			set { baitCharaId = value; }
		}

		public int HpUp
		{
			get { return hpUp; }
			set { hpUp = value; }
		}

		public bool IsHpMax
		{
			get { return isHpMax; }
			set { isHpMax = value; }
		}

		public int AtkUp
		{
			get { return atkUp; }
			set { atkUp = value; }
		}

		public bool IsAtkMax
		{
			get { return isAtkMax; }
			set { isAtkMax = value; }
		}
		
		public int DefUp
		{
			get { return defUp; }
			set { defUp = value; }
		}

		public bool IsDefMax
		{
			get { return isDefMax; }
			set { isDefMax = value; }
		}

		public int ExUp
		{
			get { return exUp; }
			set { exUp = value; }
		}

		public bool IsExMax
		{
			get { return isExMax; }
			set { isExMax = value; }
		}

		/// <summary>
		/// 合成結果
		/// </summary>
		public Scm.Common.GameParameter.PowerupResult Result
		{
			get { return result; }
			set { result = value; }
		}

	}

	public interface IController
	{
		#region === Event ===
		
		/// <summary>
		/// キャラ読み込み完了時
		/// </summary>
		event EventHandler OnCharaLoaded;

		#endregion === Event ===


		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup(SynchroDirectionParam param);
		
		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		void Dispose();


		/// <summary>
		/// 実行
		/// </summary>
		void Execute();

		/// <summary>
		/// 閉じる
		/// </summary>
		void Close();

	}

	public class Controller : IController
	{
	
		#region === Field ===

		// モデル
		private readonly IModel model;

		// ビュー
		private readonly IView view;

		// 演出用
		private GUIEffectDirection effectDirection;

		private GUISynchroDirectionText directionText;

		private TweenSync tweenSync;

		private int loadingCounter = 0;

		#endregion === Field ===

		#region === Property ===

		private IModel Model { get { return model; } }

		private IView View { get { return view; } }
		
		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		private bool CanUpdate
		{
			get
			{
				if (this.Model == null) return false;
				if (this.View == null) return false;
				return true;
			}
		}


		#endregion === Property ===


		#region === Event ===
		
		public event EventHandler OnCharaLoaded = (sender, e) => { };

		#endregion === Event ===


		public Controller(IModel model, IView view, GUIEffectDirection effectDirection, GUISynchroDirectionText directionText, TweenSync tweenSync)
		{
			if (model == null || view == null) return;

			// ビュー設定
			this.view = view;
			View.OnCharaCreated += HandleCharaCreated;
			View.OnSkip += HandleSkip;

			// モデル設定
			this.model = model;
			Model.OnBaseCharaIdChange += HandleBaseCharaIdChange;
			Model.OnBaitCharaIdChange += HandleSynchroCharaIdChange;
			Model.OnResultChange += HandleResultChange;

			// エフェクト操作
			this.effectDirection = effectDirection;
			
			// 演出文字操作
			this.directionText = directionText;

			this.tweenSync = tweenSync;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup(SynchroDirectionParam param)
		{
			if(!CanUpdate) return;
			if(param == null) return;

			loadingCounter = 2;

			View.Setup();

			Model.Setup();

			Model.HpUp = param.HpUp;
			Model.AtkUp = param.AtkUp;
			Model.DefUp = param.DefUp;
			Model.ExUp = param.ExUp;

			Model.IsHpMax = param.IsHpMax;
			Model.IsAtkMax = param.IsAtkMax;
			Model.IsDefMax= param.IsDefMax;
			Model.IsExMax= param.IsExMax;

			Model.Result = param.Result;

			Model.BaseCharaId = param.BaseCharaId;
			Model.BaitCharaId = param.BaitCharaId;
		}




		/// <summary>
		/// 結果変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleResultChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			View.SetResultMessage(Model.Result);
		}


		/// <summary>
		/// キャラID変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleBaseCharaIdChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			if(Model.BaseCharaId == 0) {
				// モデル無し
				View.SetBaseCharaInfo(null);
			} else {
				// キャラレベルマスターデータ取得
				CharaLevelMasterData lvMasterData;
				if(!MasterData.TryGetCharaLv(Model.BaseCharaId, 1, out lvMasterData)) {
					return;
				}

				// EntrantResパケット生成
				EntrantRes res = new EntrantRes();
				res.AreaType = AreaType.Lobby;
				res.AreaId = 1;
				res.InFieldId = 0;
				res.UserName = "SynchroBaseChara";
				res.Id = Model.BaseCharaId;
				res._position = new short[] { 0, 0, 0 };
				res._rotation = 0;
				res.StatusType = StatusType.Normal;
				res.EffectType = EffectType.None;
				res.HitPoint = lvMasterData.HitPoint;
				res.MaxHitPoint = lvMasterData.HitPoint;
				res.EntrantType = EntrantType.Pc;
				res.TeamType = TeamType.Unknown;
				res.TacticalId = 1;
				res.BattleLevel = (byte)lvMasterData.Level;

				var info = EntrantInfo.Create(res, false);
				info.CreateObject();

				// キャラセット
				View.SetBaseCharaInfo(info);
			}
		}

		private void HandleSynchroCharaIdChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			if(Model.BaitCharaId == 0) {
				// モデル無し
				View.SetBaitCharaInfo(null);
			} else {
				// キャラレベルマスターデータ取得
				CharaLevelMasterData lvMasterData;
				if(!MasterData.TryGetCharaLv(Model.BaseCharaId, 1, out lvMasterData)) {
					return;
				}

				// EntrantResパケット生成
				EntrantRes res = new EntrantRes();
				res.AreaType = AreaType.Lobby;
				res.AreaId = 1;
				res.InFieldId = -1;
				res.UserName = "SynchroChara";
				res.Id = Model.BaseCharaId;
				res._position = new short[] { 0, 0, 0 };
				res._rotation = 0;
				res.StatusType = StatusType.Normal;
				res.EffectType = EffectType.None;
				res.HitPoint = lvMasterData.HitPoint;
				res.MaxHitPoint = lvMasterData.HitPoint;
				res.EntrantType = EntrantType.Pc;
				res.TeamType = TeamType.Unknown;
				res.TacticalId = 1;
				res.BattleLevel = (byte)lvMasterData.Level;

				var info = EntrantInfo.Create(res, false);
				info.CreateObject();

				// キャラセット
				View.SetBaitCharaInfo(info);
			}
		}

		/// <summary>
		/// キャラの読み込み終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleCharaCreated(object sender, EventArgs e)
		{
			if(--loadingCounter <= 0) {
				loadingCounter = 0;
				OnCharaLoaded(this, EventArgs.Empty);
			}
		}




		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			OnCharaLoaded = null;

			// モデル破棄
			this.Model.Dispose();
		}
		
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


		/// <summary>
		/// 実行
		/// </summary>
		public void Execute()
		{
			UI3DFXCamera.SwitchActive(true);

			// 実行する
			effectDirection.ExecuteEffect(OnDirectionFinished);
			
			View.MoveStart();
		}



		/// <summary>
		/// 演出部終了時
		/// </summary>
		private void OnDirectionFinished()
		{
			// 文字出す。
			directionText.EffectStart(
				Model.HpUp,		Model.IsHpMax,
				Model.AtkUp,	Model.IsAtkMax,
				Model.DefUp,	Model.IsDefMax,
				Model.ExUp,		Model.IsExMax
			);

			if(tweenSync != null) {
				tweenSync.DynamicSyncListUpdate();
			}
		}


		/// <summary>
		/// 全終了時
		/// </summary>
		private void OnFinished()
		{
			// タッチ待つので何もしない
		}



		/// <summary>
		/// スキップ処理時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleSkip(object sender, EventArgs e)
		{
			GUISynchroDirection.Close();
		}

		public void Close()
		{
			View.MoveEnd();

			effectDirection.Close();

			directionText.Close();

			// キャラ消して閉じる
			Model.BaseCharaId = 0;
			Model.BaitCharaId = 0;

			UI3DFXCamera.SwitchActive(false);
		}
	}
}

