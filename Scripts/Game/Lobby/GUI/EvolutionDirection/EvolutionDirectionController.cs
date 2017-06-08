using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

namespace XUI.EvolutionDirection
{


	/// <summary>
	/// 強化演出パラメータ
	/// </summary>
	[Serializable]
	public class EvolutionDirectionParam
	{
		[SerializeField]
		private int baseCharaId = 0;

		[SerializeField]
		private int oldRank = 0;

		[SerializeField]
		private int newRank = 0;

		[SerializeField]
		private int synchroRemainUpCount = 0;

		[SerializeField]
		private int[] materialIds = null;

        [SerializeField]
        private int[] skinIds = null;

		/// <summary>
		/// ベースキャラID
		/// </summary>
		public int BaseCharaId
		{
			get { return baseCharaId; }
			set { baseCharaId = value; }
		}

		public int OldRank
		{
			get { return oldRank; }
			set { oldRank = value; }
		}

		public int NewRank
		{
			get { return newRank; }
			set { newRank = value; }
		}

		public int SynchroRemainUpCount
		{
			get { return synchroRemainUpCount; }
			set { synchroRemainUpCount = value; }
		}

		public int[] MaterialIds
		{
			get { return materialIds; }
			set { materialIds = value; }
		}

        public int[] SkinIds {
            get { return skinIds; }
            set { skinIds = value; }
        }
	}

	public interface IController
	{
		#region === Event ===

		/// <summary>
		/// アイコン読み込み完了時
		/// </summary>
		event EventHandler OnIconLoaded;

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
		void Setup(EvolutionDirectionParam param);
		
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

		private GUIEvolutionDirectionText directionText;

		private CharaIcon charaIcon;

		private TweenSync tweenSync;

		private int loadingCounter = 0;

		#endregion === Field ===

		#region === Property ===

		private IModel Model { get { return model; } }

		private IView View { get { return view; } }
		
		// キャラアイコン
		private CharaIcon CharaIcon { get { return charaIcon; } }

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

		public event EventHandler OnIconLoaded = (sender, e) => { };

		public event EventHandler OnCharaLoaded = (sender, e) => { };

		#endregion === Event ===


		public Controller(IModel model, IView view, GUIEffectDirection effectDirection, GUIEvolutionDirectionText directionText, CharaIcon charaIcon, TweenSync tweenSync)
		{
			if (model == null || view == null) return;

			// ビュー設定
			this.view = view;
			View.OnCharaCreated += HandleCharaCreated;
			View.OnSkip += HandleSkip;

			// モデル設定
			this.model = model;
			Model.OnBaseCharaIdChange += HandleBaseCharaIdChange;
			Model.OnMaterialIdsChange += HandleMaterialIdsChange;
			Model.OnRankChange += HandleRankChange;
			Model.OnSynchroRemainUpCountChange += HandleSynchroRemainUpCountChange;

			// エフェクト操作
			this.effectDirection = effectDirection;
			
			// 演出文字操作
			this.directionText = directionText;

			this.charaIcon = charaIcon;

			this.tweenSync = tweenSync;
		}

		private void HandleSynchroRemainUpCountChange(object sender, EventArgs e)
		{

		}

		private void HandleRankChange(object sender, EventArgs e)
		{

		}


		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup(EvolutionDirectionParam param)
		{
			if(!CanUpdate) return;
			if(param == null) return;

			Model.Setup();
			
			Model.SynchroRemainUpCount = param.SynchroRemainUpCount;

			Model.SetRank(param.OldRank, param.NewRank);

			Model.SetMaterialIds(param.MaterialIds, param.SkinIds);

			Model.BaseCharaId = param.BaseCharaId;
		}




		/// <summary>
		/// 素材変更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleMaterialIdsChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;

			var matIconList = Model.MaterialIds;
            var skinList = Model.SkinIds;

			loadingCounter = matIconList.Count;

			for(int i = 0; i < View.MaterialIconCount; i++) {
				if(i < matIconList.Count) {
					View.SetMaterialIconVisible(i, true);

					CharaIcon.GetIcon((AvatarType)matIconList[i], skinList[i], false,
						(atlas, spriteName) =>
						{
							View.SetMaterialIcon(i, atlas, spriteName);
							IconLoaded();
						}
					);

				} else {
					View.SetMaterialIconVisible(i, false);
				}
			}
		}

		/// <summary>
		/// アイコンの読み込み終了
		/// </summary>
		private void IconLoaded()
		{
			if(--loadingCounter <= 0) {
				loadingCounter = 0;
				OnIconLoaded(this, EventArgs.Empty);
			}
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
				res.UserName = "EvolutionChara";
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

		/// <summary>
		/// キャラの読み込み終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleCharaCreated(object sender, EventArgs e)
		{
			OnCharaLoaded(this, EventArgs.Empty);
		}




		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
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
		}

		/// <summary>
		/// 演出部終了時
		/// </summary>
		private void OnDirectionFinished()
		{
			// 文字出す。
			directionText.EffectStart(Model.OldRank, Model.NewRank, Model.SynchroRemainUpCount);

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
			GUIEvolutionDirection.Close();
		}

		public void Close()
		{
			effectDirection.Close();

			directionText.Close();

			// キャラ消して閉じる
			Model.BaseCharaId = 0;

			UI3DFXCamera.SwitchActive(false);
		}
	}
}

