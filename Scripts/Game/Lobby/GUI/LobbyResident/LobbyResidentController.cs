/// <summary>
/// 常駐メニュー制御
/// 
/// 2016/05/27
/// </summary>
using System;
using System.Collections.Generic;

using Scm.Common.Packet;
using UnityEngine;

namespace XUI.LobbyResident
{
	/// <summary>
	/// 常駐メニュー制御インターフェイス
	/// </summary>
	public interface IController
	{
        void RankMatchStart();

		#region 初期化
		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();
		#endregion

		#region 更新
		/// <summary>
		/// 更新
		/// </summary>
		void Update();
		#endregion 更新

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive(bool isActive, bool isTweenSkip);
		#endregion

		#region 各状態を更新する
		/// <summary>
		/// ロビー選択の状態を更新する
		/// </summary>
		void UpdateLobbySelectActive();
		/// <summary>
		/// マッチングの状態を更新する
		/// </summary>
		void UpdateMatchingActive();
		/// <summary>
		/// 練習ボタンの状態を更新する
		/// </summary>
		void UpdateTrainingButtonEnable();

		/// <summary>
		/// チーム情報更新
		/// </summary>
		void UpdateTeamInfo(TeamParameter teamParam, List<GroupMemberParameter> memberParam);
		/// <summary>
		/// シングルボタンの状態を更新する
		/// </summary>
		void UpdateSingleButtonEnable();
		/// <summary>
		/// ショップメニューボタンの状態を更新する
		/// </summary>
		void UpdateShopMenuButtonEnable();
		#endregion 各状態を更新する

		#region 表示直結系
		#region ロビー選択
		/// <summary>
		/// ロビー番号設定
		/// </summary>
		void SetLobbyNo(int lobbyNo);
		#endregion ロビー番号

		#region ロビーメンバー
		/// <summary>
		/// ロビーの収容人数設定
		/// </summary>
		void SetLobbyMemberCapacity(int num);
		#endregion ロビーメンバー

		#region 通知系
		/// <summary>
		/// 未取得アチーブメント数設定
		/// </summary>
		void SetAchieveUnreceived(int num);

		/// <summary>
		/// 未読メール数設定
		/// </summary>
		void SetMailUnread(int num);

		/// <summary>
		/// 未処理申請数設定
		/// </summary>
		void SetApplyUnprocessed(int num);
		#endregion 通知系

		#region プレイヤー情報
		/// <summary>
		/// プレイヤー名設定
		/// </summary>
		void SetPlayerName(string name);

		/// <summary>
		/// プレイヤー勝利数設定
		/// </summary>
		void SetPlayerWin(int num);

		/// <summary>
		/// プレイヤー敗北数設定
		/// </summary>
		void SetPlayerLose(int num);
		#endregion プレイヤー情報
		#endregion 表示直結系
	}

	/// <summary>
	/// 常駐メニュー制御
	/// </summary>
	public class Controller : IController
	{
		#region 文字列
		string ScreenTitle { get { return MasterData.GetText(TextType.TX363_Slot_ScreenTitle); } }
		string BaseHelpMessage { get { return MasterData.GetText(TextType.TX364_Slot_Base_HelpMessage); } }
		string SlotHelpMessage { get { return MasterData.GetText(TextType.TX365_Slot_Slot_HelpMessage); } }
		string OKMessage { get { return MasterData.GetText(TextType.TX366_Slot_OK_Message); } }
		string SlotMessage { get { return MasterData.GetText(TextType.TX367_Slot_Slot_Message); } }
		#endregion

		#region フィールド＆プロパティ
		// モデル
		readonly IModel _model;
		IModel Model { get { return _model; } }
		// ビュー
		readonly IView _view;
        public IView View { get { return _view; } }
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

		/// <summary>
		/// ロビー選択できるかどうか
		/// </summary>
		static bool CanLobbySelect { get { return GUILobbyResident.CanLobbySelect; } }
		/// <summary>
		/// マッチングできるかどうか
		/// </summary>
		static bool CanMatching
		{
			get
			{
				bool isActive = true;
				if (GUIMatchingState.IsMatching)
				{
					// マッチング中.
					isActive = false;
				}
				else if (NetworkController.ServerValue != null &&
				   NetworkController.ServerValue.IsJoinedTeam &&
				   !NetworkController.ServerValue.IsTeamLeader)
				{
					// チームを組んでいてリーダーではない.
					isActive = false;
				}

				return isActive;
			}
		}
		/// <summary>
		/// 練習ができるかどうか
		/// </summary>
		static bool CanTraining
		{
			get
			{
				bool isActive = true;
				if (GUIMatchingState.IsMatching)
				{
					// マッチング中
					isActive = false;
				}
				else if (NetworkController.ServerValue != null && NetworkController.ServerValue.IsJoinedTeam)
				{
					// チームに所属
					isActive = false;
				}

				return isActive;
			}
		}
		/// <summary>
		/// ショップメニューが押せるかどうか
		/// </summary>
		static bool CanShopMenu
		{
			get
			{
				bool isActive = true;
				if (GUIMatchingState.IsMatching)
				{
					// マッチング中
					isActive = false;
				}
				else if (NetworkController.ServerValue != null && NetworkController.ServerValue.IsJoinedTeam)
				{
					// チームに所属
					isActive = false;
				}
				return isActive;
			}
		}

		// シリアライズされていないメンバーの初期化
		void MemberInit()
		{
		}
		#endregion

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller(IModel model, IView view)
		{
			if (model == null || view == null) return;

			this.MemberInit();

			// ビュー設定
			this._view = view;
			this.View.OnLobbyMenu += this.HandleLobbyMenu;
			this.View.OnQuickMatching += this.HandleMatching;
            this.View.OnRankMatching += this.HandleRankMatching;
			this.View.OnLobbySelect += this.HandleLobbySelect;
			this.View.OnLobbyMember += this.HandleLobbyMember;
			this.View.OnShopMenu += this.HandleShopMenu;
			this.View.OnTraining += this.HandleTraining;
			this.View.OnAchievement += this.HandleAchievement;
			this.View.OnMail += this.HandleMail;
			this.View.OnApply += this.HandleApply;
#if OLD_TEAM_LOGIC
			this.View.OnTeamMenu += this.HandleTeamMenu;
            this.View.OnTeamMemberPlayer += this.HandleTeamMemberPlayer;
			this.View.OnTeamMember += this.HandleTeamMember;
#endif
            this.View.OnHelp += this.HandleHelp;
			this.View.OnDummyCharaGacha += this.HandleDummyCharaGacha;

			// モデル設定
			this._model = model;
			// ロビーセレクト
			this.Model.OnLobbyNoChange += this.HandleLobbyNoChange;
			this.Model.OnLobbyNoFormatChange += this.HandleLobbyNoFormatChange;
			// ロビーメンバー
			this.Model.OnLobbyMemberCountChange += this.HandleLobbyMemberCountChange;
			this.Model.OnLobbyMemberCapacityChange += this.HandleLobbyMemberCapacityChange;
			this.Model.OnLobbyMemberFormatChange += this.HandleLobbyMemberFormatChange;
			// 通知系
			this.Model.OnAchieveUnreceivedChange += this.HandleAchieveUnreceivedChange;
			this.Model.OnMailUnreadChange += this.HandleMailUnreadChange;
			this.Model.OnApplyUnprocessedChange += this.HandleApplyUnprocessedChange;
			this.Model.OnAlertFormatChange += this.HandleAlertFormatChange;
			// プレイヤー情報
			this.Model.OnPlayerNameChange += this.HandlePlayerNameChange;
			this.Model.OnPlayerWinChange += this.HandlePlayerWinChange;
			this.Model.OnPlayerLoseChange += this.HandlePlayerLoseChange;
			this.Model.OnPlayerWinLoseFormatChange += this.HandlePlayerWinLoseFormatChange;

			// 同期
			this.SyncLobbyNo();
			this.SyncLobbyMember();
			this.SyncAlert();
			this.SyncPlayerName();
			this.SyncPlayerWin();
			this.SyncPlayerLose();
		}

        public void RankMatchStart()
        {
            this.View.RankMatchStart ();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Setup()
		{
			this.MemberInit();
		}
		#endregion

		#region 破棄
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if (this.CanUpdate)
			{
				this.Model.Dispose();
			}
		}
		#endregion

		#region 更新
		/// <summary>
		/// 更新
		/// </summary>
		public void Update()
		{
			if (!this.CanUpdate) return;

			var count = 0;
			if (PersonManager.Instance != null) count += PersonManager.Instance.Count;
			if (GameController.GetPlayer() != null) count++;

			this.Model.LobbyMemberCount = count;
		}
		#endregion 更新

		#region アクティブ設定
		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive(bool isActive, bool isTweenSkip)
		{
			if (!this.CanUpdate) return;

			// ロビー番号更新
			this.Model.LobbyNo = (int)ScmParam.Lobby.LobbyType;

			// チームメンバー初期化
			this.View.TeamMemberReset();
			if (NetworkController.ServerValue != null)
			{
				this.UpdateTeamInfo(NetworkController.ServerValue.TeamParameter, NetworkController.ServerValue.Members);
			}

			// アクティブ設定
			this.View.SetActive(isActive, isTweenSkip);
		}
		#endregion

		#region 各状態を更新する
		/// <summary>
		/// ロビー選択の状態を更新する
		/// </summary>
		public void UpdateLobbySelectActive()
		{
			if (this.CanUpdate) this.View.SetLobbySelectButtonEnable(CanLobbySelect);
		}
		/// <summary>
		/// マッチングの状態を更新する
		/// </summary>
		public void UpdateMatchingActive()
		{
			if (this.CanUpdate)
			{
				bool isActive = CanMatching;
				this.View.SetMatchingButtonEnable(isActive);
				if (isActive) this.View.MatchingPlay(true);
			}
		}
		/// <summary>
		/// 練習ボタンの状態を更新する
		/// </summary>
		public void UpdateTrainingButtonEnable()
		{
			if (this.CanUpdate) this.View.SetTrainingButtonEnable(CanTraining);
		}

		/// <summary>
		/// チーム情報更新
		/// </summary>
		public void UpdateTeamInfo(TeamParameter teamParam, List<GroupMemberParameter> memberParam)
		{
			if (!this.CanUpdate) return;
			if (NetworkController.ServerValue == null) return;

			this.UpdateMatchingActive();
			this.UpdateTrainingButtonEnable();
//			this.View.SetPlayerLeaderIconEnable(false);

			if (teamParam == null || teamParam.TeamId == 0)
			{
				this.View.TeamMemberClose();
			}
			else
			{
				int playerId = NetworkController.ServerValue.PlayerId;
				for (int i = 0; i < memberParam.Count; ++i)
				{
					if (memberParam[i].PlayerId == playerId)
					{
						if (i == 0)
						{
							this.View.SetPlayerLeaderIconEnable(true);
						}
						this.View.TeamMemberOpen(teamParam.TeamName, memberParam, playerId);
						return;
					}
				}
				BugReportController.SaveLogFile("Player isn't included in a team member." + playerId + ", " + teamParam.TeamId);
			}
		}
		/// <summary>
		/// シングルボタンの状態を更新する
		/// </summary>
		public void UpdateSingleButtonEnable()
		{
			if (this.CanUpdate) this.View.UpdateSingleButtonEnable();
		}
		/// <summary>
		/// ショップメニューボタンの状態を更新する
		/// </summary>
		public void UpdateShopMenuButtonEnable()
		{
			if (this.CanUpdate) this.View.SetShopMenuButtonEnable(CanShopMenu);
		}
		#endregion 各状態を更新する

		#region 表示直結系
		#region ロビー選択
		/// <summary>
		/// ロビー番号設定
		/// </summary>
		public void SetLobbyNo(int lobbyNo) { if (this.CanUpdate) this.Model.LobbyNo = lobbyNo; }
		void HandleLobbyNoChange(object sender, EventArgs e) { this.SyncLobbyNo(); }
		void HandleLobbyNoFormatChange(object sender, EventArgs e) { this.SyncLobbyNo(); }
		void SyncLobbyNo()
		{
			if (this.CanUpdate)
			{
				this.View.SetLobbyNo(this.Model.LobbyNo, this.Model.LobbyNoFormat);
			}
		}
		#endregion ロビー番号

		#region ロビーメンバー
		/// <summary>
		/// ロビーの収容人数設定
		/// </summary>
		public void SetLobbyMemberCapacity(int num) { if (this.CanUpdate)this.Model.LobbyMemberCapacity = num; }
		void HandleLobbyMemberCountChange(object sender, EventArgs e) { this.SyncLobbyMember(); }
		void HandleLobbyMemberCapacityChange(object sender, EventArgs e) { this.SyncLobbyMember(); }
		void HandleLobbyMemberFormatChange(object sender, EventArgs e) { this.SyncLobbyMember(); }
		void SyncLobbyMember()
		{
			if (this.CanUpdate)
			{
				this.View.SetLobbyMember(this.Model.LobbyMemberCount, this.Model.LobbyMemberCapacity, this.Model.LobbyMemberFormat);
			}
		}
		#endregion ロビーメンバー

		#region 通知系
		/// <summary>
		/// 未取得アチーブメント数設定
		/// </summary>
		public void SetAchieveUnreceived(int num)
		{
			if (!this.CanUpdate) return;

			this.Model.AchieveUnreceived = num;
			this.View.SetAchieveUnreceivedEnable(num > 0);
		}
		void HandleAchieveUnreceivedChange(object sender, EventArgs e) { this.SyncAchieveUnreceived(); }
		void SyncAchieveUnreceived()
		{
			if (this.CanUpdate)
			{
				this.View.SetAchieveUnreceived(this.Model.AchieveUnreceived, this.Model.AlertFormat);
			}
		}

		/// <summary>
		/// 未読メール数設定
		/// </summary>
		public void SetMailUnread(int num)
		{
			if (!this.CanUpdate) return;

			this.Model.MailUnread = num;
			this.View.SetMailUnreadEnable(num > 0);
		}
		void HandleMailUnreadChange(object sender, EventArgs e) { this.SyncMailUnread(); }
		void SyncMailUnread()
		{
			if (this.CanUpdate)
			{
				this.View.SetMailUnread(this.Model.MailUnread, this.Model.AlertFormat);
			}
		}

		/// <summary>
		/// 未処理申請数設定
		/// </summary>
		public void SetApplyUnprocessed(int num)
		{
			if (!this.CanUpdate) return;

			this.Model.ApplyUnprocessed = num;
			this.View.SetApplyUnprocessedEnable(num > 0);
		}
		void HandleApplyUnprocessedChange(object sender, EventArgs e) { this.SyncApplyUnprocessed(); }
		void SyncApplyUnprocessed()
		{
			if (this.CanUpdate)
			{
				this.View.SetApplyUnprocessed(this.Model.ApplyUnprocessed, this.Model.AlertFormat);
			}
		}

		void HandleAlertFormatChange(object sender, EventArgs e) { this.SyncAlert(); }
		void SyncAlert()
		{
			this.SyncAchieveUnreceived();
			this.SyncMailUnread();
			this.SyncApplyUnprocessed();
		}
		#endregion 通知系

		#region プレイヤー情報
		/// <summary>
		/// プレイヤー名設定
		/// </summary>
		public void SetPlayerName(string name) { if (this.CanUpdate) this.Model.PlayerName = name; }
		void HandlePlayerNameChange(object sender, EventArgs e) { this.SyncPlayerName(); }
		void SyncPlayerName()
		{
			if (this.CanUpdate)
			{
				this.View.SetPlayerName(this.Model.PlayerName);                
			}
		}

        public LobbyResidentView GetView()
        {
            return this.View as LobbyResidentView;
        }

		/// <summary>
		/// プレイヤー勝利数設定
		/// </summary>
		public void SetPlayerWin(int num) { if (this.CanUpdate) this.Model.PlayerWin = num; }
		void HandlePlayerWinChange(object sender, EventArgs e) { this.SyncPlayerWin(); }
		void SyncPlayerWin()
		{
			if (this.CanUpdate)
			{
				this.View.SetPlayerWin(this.Model.PlayerWin, this.Model.PlayerWinLoseFormat);
			}
		}

		/// <summary>
		/// プレイヤー敗北数設定
		/// </summary>
		public void SetPlayerLose(int num) { if (this.CanUpdate) this.Model.PlayerLose = num; }
		void HandlePlayerLoseChange(object sender, EventArgs e) { this.SyncPlayerLose(); }
		void SyncPlayerLose()
		{
			if (this.CanUpdate)
			{
				//this.View.SetPlayerLose(this.Model.PlayerLose, this.Model.PlayerWinLoseFormat);
				this.View.SetPlayerLose(0, this.Model.PlayerWinLoseFormat);
			}
		}

		void HandlePlayerWinLoseFormatChange(object sender, EventArgs e) { this.SyncPlayerWinLoseFormat(); }
		void SyncPlayerWinLoseFormat()
		{
			this.SyncPlayerWin();
			this.SyncPlayerLose();
		}
		#endregion プレイヤー情報
		#endregion 表示直結系

		#region ボタン系
		#region ロビーメニューボタン
		void HandleLobbyMenu(object sender, EventArgs e)
		{
			GUILobbyMenu.Toggle();
		}
		#endregion ロビーメニューボタン

		#region マッチングボタン
		void HandleMatching(object sender, EventArgs e)
		{
            //GUIMatching.SetActive(true);
            // UNDONE:マッチングするバトルフィールドIDは固定
            //LobbyPacket.SendMatchingEntry(BattleFieldType.BF008_Shiwasu2);
            //LobbyPacket.SendMatchingEntry(BattleFieldType.BF004_Training);
            if (ScmParam.SelectBattleField) {
                GUIBattleFieldSelect.SetScoreType(Scm.Common.GameParameter.ScoreType.QuickMatching);
                GUIBattleFieldSelect.SetActive(true);
            } else {
                LobbyPacket.SendMatchingEntry(BattleFieldType.None, Scm.Common.GameParameter.ScoreType.QuickMatching);
            }
        }

        void HandleRankMatching(object sender, EventArgs e) {
            if (ScmParam.SelectBattleField) {
                GUIBattleFieldSelect.SetScoreType(Scm.Common.GameParameter.ScoreType.Ranking);
                GUIBattleFieldSelect.SetActive(true);
            } else {
                LobbyPacket.SendMatchingEntry(BattleFieldType.None, Scm.Common.GameParameter.ScoreType.Ranking);
            }
        }
        #endregion マッチングボタン

        #region ロビー選択ボタン
        void HandleLobbySelect(object sender, EventArgs e)
		{
			// ロビーメニューを非表示
			GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.None);

			GUILobbySelect.SetActive(true, true, true,
				null,
				null,
				(currentLobbyID, lobbyInfo) =>
				{
					if (currentLobbyID == lobbyInfo.LobbyID) return;
					LobbyMain.LobbyChange(lobbyInfo.LobbyID);
				});
		}
		#endregion ロビー選択ボタン

		#region ロビーメンバーボタン
		void HandleLobbyMember(object sender, EventArgs e)
		{
			// ロビーメニューを非表示
			GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.None);

			GUILobbyMemberList.SetActive(true);
		}
		#endregion ロビーメンバーボタン

		#region ショップメニューボタン
		void HandleShopMenu( object sender, EventArgs e ) {

			// ロビーメニューを非表示
			GUILobbyMenu.SetMode( GUILobbyMenu.MenuMode.None );

			// 年齢認証スキップ確認
			if( ConfigFile.Instance.Data.System.AgeVerificationSkip ) {
				// ショップを開く
				GUIController.Open( new GUIScreen( GUICharaTicket.Open, GUICharaTicket.Close, GUICharaTicket.ReOpen ) );

			} else {
				// 年齢認証画面を開く
				GUIController.Open( new GUIScreen( GUIAgeVerification.Open, GUIAgeVerification.Close, GUIAgeVerification.ReOpen ) );
			}
		}
		#endregion ショップメニューボタン

		#region 練習ボタン
		void HandleTraining(object sender, EventArgs e)
		{
			GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX129_NextTraining),
				() =>
				{
					ScmParam.Battle.BattleFieldType = BattleFieldType.BF004_Training;
					LobbyMain.NextScene_Battle();
				},
				() => { }
			);
		}
		#endregion 練習ボタン

		#region アチーブメントボタン
		void HandleAchievement(object sender, EventArgs e)
		{
			// ロビーメニューを非表示
			GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.None);

			GUIController.Open(new GUIScreen(GUIAchievement.Open, GUIAchievement.Close, GUIAchievement.ReOpen));
		}
		#endregion アチーブメントボタン

		#region メールボタン
		void HandleMail(object sender, EventArgs e)
		{
			// ロビーメニューを非表示
			GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.None);

			GUIController.Open(new GUIScreen(GUIMail.Open, GUIMail.Close, GUIMail.ReOpen));
		}
		#endregion メールボタン

		#region 申請ボタン
		void HandleApply(object sender, EventArgs e)
		{
			// ロビーメニューを非表示
			GUILobbyMenu.SetMode(GUILobbyMenu.MenuMode.None);

			//GUIController.Open(new GUIScreen(GUIMail.Open, GUIMail.Close, GUIMail.ReOpen));
		}
		#endregion 申請ボタン
#if OLD_TEAM_LOGIC
		#region チーム情報ボタン
        void HandleTeamMenu(object sender, EventArgs e)
		{
			if (NetworkController.ServerValue == null) return;

			if (NetworkController.ServerValue.IsJoinedTeam)
			{
				if (NetworkController.ServerValue.IsTeamLeader)
				{
					// リーダメニュー.
					GUITeamMenu.SetMode(GUITeamMenu.TeamMenuMode.LeaderMenu);
				}
				// メンバーの場合は何も起きない.
			}
			else
			{
				// シングルメニュー.
				GUITeamMenu.SetMode(GUITeamMenu.TeamMenuMode.SingleMenu);
			}
		}
		void HandleTeamMemberPlayer(object sender, EventArgs e)
		{
			if (NetworkController.ServerValue == null) return;
			if (!NetworkController.ServerValue.IsJoinedTeam) return;

			GUIMessageWindow.SetModeYesNo(MasterData.GetText(TextType.TX097_TeamSecession_WindowTitle), () => CommonPacket.SendTeamRemoveMember(NetworkController.ServerValue.PlayerId), null);
		}
		void HandleTeamMember(object sender, TeamMemberEventArgs e)
		{
			if (NetworkController.ServerValue == null) return;
			if (!NetworkController.ServerValue.IsJoinedTeam) return;
			if (e.Member == null) return;

			if (NetworkController.ServerValue.IsTeamLeader)
			{
				long playerId = e.Member.PlayerId;
				if (0 < playerId)
				{
					// リーダメニュー.(今はキックのみ).
					string title = MasterData.GetText(TextType.TX098_TeamKick_WindowTitle, new string[] { e.Member.PlayerName });
					GUIMessageWindow.SetModeYesNo(title, () => CommonPacket.SendTeamRemoveMember(playerId), null);
				}
			}
			else
			{
				// メンバーメニュー.
			}
		}
		#endregion チーム情報ボタン
#endif
		#region ヘルプボタン
        void HandleHelp(object sender, EventArgs e)
		{
			GUIWebView.OpenHelp();
		}
		#endregion ヘルプボタン

		#region 審査会用
		void HandleDummyCharaGacha(object sender, EventArgs e)
		{
			CommonPacket.SendGmCommand("gacha");
		}

        
        #endregion 審査会用
        #endregion ボタン系
    }
}
