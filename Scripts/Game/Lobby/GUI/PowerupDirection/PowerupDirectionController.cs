using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.Packet;
using Scm.Common.GameParameter;
using System;

namespace XUI.PowerupDirection
{


	/// <summary>
	/// 強化演出パラメータ
	/// </summary>
	[Serializable]
	public class PowerupDirectionParam
	{
		[SerializeField]
		private int baseCharaId = 0;

		[SerializeField]
		private float startExpRate = 0;

		[SerializeField]
		private float endExpRate = 0;

		[SerializeField]
		private int lvUpCount = 0;

		[SerializeField]
		private bool lvMax = false;

		[SerializeField]
		private int [] baitCharaIds = null;

		[SerializeField]
		private Scm.Common.GameParameter.PowerupResult result = Scm.Common.GameParameter.PowerupResult.Good;





		/// <summary>
		/// 強化キャラID
		/// </summary>
		public int BaseCharaId
		{
			get { return baseCharaId; }
			set { baseCharaId = value; }
		}
		

		/// <summary>
		/// 開始時の経験値バーの位置
		/// </summary>
		public float StartExpRate
		{
			get { return startExpRate; }
			set { startExpRate = value; }
		}

		/// <summary>
		/// 終了時の経験値バーの位置
		/// </summary>
		public float EndExpRate
		{
			get { return endExpRate; }
			set { endExpRate = value; }
		}

		/// <summary>
		/// レベルアップカウント
		/// </summary>
		public int LvUpCount
		{
			get { return lvUpCount; }
			set { lvUpCount = value; }
		}

		/// <summary>
		/// レベル最大になるか
		/// </summary>
		public bool LvMax
		{
			get { return lvMax; }
			set { lvMax = value; }
		}

		/// <summary>
		/// エサのキャラIDリスト
		/// </summary>
		public int[] BaitCharaIds
		{
			get { return baitCharaIds; }
			set { baitCharaIds = value; }
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
		void Setup(PowerupDirectionParam param);
		
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

		private GUIPowerupDirectionText directionText;

		private CharaIcon charaIcon;

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


		public Controller(IModel model, IView view, GUIEffectDirection effectDirection, GUIPowerupDirectionText directionText, CharaIcon charaIcon)
		{
			if (model == null || view == null || effectDirection == null || directionText == null) return;

			// ビュー設定
			this.view = view;
			View.OnCharaCreated += HandleCharaCreated;
			View.OnSkip += HandleSkip;

			// モデル設定
			this.model = model;
			Model.OnBaseCharaIdChange += HandleBaseCharaIdChange;
			Model.OnBaitIdsChange += HandleBaitIdsChange;
			Model.OnResultChange += HandleResultChange;

			// エフェクト操作
			this.effectDirection = effectDirection;

			// 演出文字操作
			this.directionText = directionText;
			
			// キャラアイコン設定
			this.charaIcon = charaIcon;

		}


		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup(PowerupDirectionParam param)
		{
			if(!CanUpdate) return;
			if(param == null) return;

			loadingCounter = 0;

			Model.Setup();

			// リザルト
			Model.Result = param.Result;
			
			// 経験値
			Model.SetExpSlider(param.StartExpRate, param.EndExpRate, param.LvUpCount, param.LvMax);

			// エサセット
			Model.SetBaitIds(param.BaitCharaIds);
			
			// モデル
			Model.BaseCharaId = param.BaseCharaId;

		}



		
		/// <summary>
		/// オブジェクトの破棄
		/// </summary>
		public void Dispose()
		{
			OnIconLoaded = null;
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
		/// エサ変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleBaitIdsChange(object sender, EventArgs e)
		{
			if(!CanUpdate) return;
			
			var baitIconList = Model.BaitIds;
            var skinIdList = Model.SkinIds;

			loadingCounter = baitIconList.Count;

			for(int i = 0; i < View.BaitIconCount; i++) {
				if(i < baitIconList.Count) {
					View.SetBaitIconVisible(i, true);
					
					CharaIcon.GetIcon((AvatarType)baitIconList[i], skinIdList[i], false,
						(atlas, spriteName) =>
						{
							View.SetBaitIcon(i, atlas, spriteName);
							IconLoaded();
						}
					);

				} else {
					View.SetBaitIconVisible(i, false);
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
				res.UserName = "PowerUpChara";
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
			// レベルアップしていく
			directionText.LevelUpStart(Model.StartExpRate, Model.LvUpCount, Model.EndExpRate, Model.IsLvMax, OnFinished);
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
			GUIPowerupDirection.Close();
		}


		public void Close()
		{
			directionText.Close();

			effectDirection.Close();

			// アイコン消しておく

			// キャラ消して閉じる
			Model.BaseCharaId = 0;

			UI3DFXCamera.SwitchActive(false);

		}

	}
}

