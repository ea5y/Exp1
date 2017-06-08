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
	/// 強化メニュー制御インターフェイス
	/// </summary>
	public interface IController
	{
		#region 初期化
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();
		#endregion

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

	}

	/// <summary>
	/// 強化メニュー制御
	/// </summary>
	public class Controller : IController
	{

		#region 文字列
		string ScreenTitle { get{ return MasterData.GetText(TextType.TX341_PowerupMenu_ScreenTitle); } }
		string HelpMessage { get { return MasterData.GetText(TextType.TX342_PowerupMenu_HelpMessage); } }
		#endregion

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
		bool CanUpdate
		{
			get
			{
				if (this.Model == null) return false;
				if (this.View == null) return false;
				return true;
			}
		}

		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view )
		{
			if (model == null || view == null) return;

			// ビュー設定
			this._view = view;
			this.View.OnHome += this.HandleHome;
			this.View.OnClose += this.HandleClose;

			this.View.OnPowerup += this.HandlePowerup;
			this.View.OnSynchro += this.HandleSynchro;
			this.View.OnEvolution += this.HandleEvolution;
			this.View.OnSlot += this.HandleSlot;
			this.View.OnSkill += this.HandleSkill;


			// モデル設定
			this._model = model;

			// 同期

		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup()
		{
			// 初期化

			// 同期
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

				// その他UIの表示設定
				GUILobbyResident.SetActive(!isActive);
				GUIScreenTitle.Play(isActive, this.ScreenTitle);
				GUIHelpMessage.Play(isActive, this.HelpMessage);

			}
		}
		#endregion

		#region ホーム、閉じるボタンイベント
		void HandleHome(object sender, EventArgs e)
		{
			GUIController.Clear();
		}
		void HandleClose(object sender, EventArgs e)
		{
			GUIController.Back();
		}
		#endregion

		#region 強化合成ボタンイベント
		void HandlePowerup(object sender,EventArgs e)
		{
			GUIController.Open(new GUIScreen(GUIPowerup.Open, GUIPowerup.Close, GUIPowerup.ReOpen));
		}
		#endregion

		#region シンクロボタンイベント
		void HandleEvolution(object sender, EventArgs e)
		{
			GUIController.Open(new GUIScreen(GUIEvolution.Open, GUIEvolution.Close, GUIEvolution.ReOpen));

		}
		#endregion

		#region シンクロボタンイベント
		void HandleSynchro(object sender, EventArgs e)
		{
			GUIController.Open(new GUIScreen(GUISynchro.Open, GUISynchro.Close, GUISynchro.ReOpen));
		}
		#endregion

		#region スロットイベント
		void HandleSlot(object sender, EventArgs e)
		{
			GUIController.Open(new GUIScreen(GUIPowerupSlot.Open, GUIPowerupSlot.Close, GUIPowerupSlot.ReOpen));
		}
		#endregion

		#region スキルイベント
		void HandleSkill(object sender, EventArgs e)
		{
			// 未実装
		}
		#endregion

	}
}
