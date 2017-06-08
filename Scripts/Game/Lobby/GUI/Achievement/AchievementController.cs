/// <summary>
/// アチーブメント制御
/// 
/// 2016/04/25
/// </summary>
 
using System;
using System.Collections.Generic;
using XUI.AchievementItemPageList;

namespace XUI.Achievement {

	#region ==== イベント ====

	/// <summary>
	/// ページ変更イベント引数
	/// </summary>
	public class TabPageChangeEventArgs : EventArgs {

		/// <summary>
		/// ページ変更されたタブタイプ
		/// </summary>
		public AchievementTabType TabType { get; private set; }

		/// <summary>
		/// 変更後のページ
		/// </summary>
		public int Page { get; private set; }

		/// <summary>
		/// 変更後のページの最初のインデックス
		/// </summary>
		public int ItemIndex { get; private set; }

		/// <summary>
		/// 件数
		/// </summary>
		public int ItemCount { get; private set; }

		/// <summary>
		/// ページ変更
		/// </summary>
		/// <param name="tabType"></param>
		/// <param name="page"></param>
		/// <param name="itemIndex"></param>
		/// <param name="itemCount"></param>
		public TabPageChangeEventArgs( AchievementTabType tabType, int page, int itemIndex, int itemCount ) {

			TabType = tabType;
			Page = page;
			ItemIndex = itemIndex;
			ItemCount = itemCount;
		}
	}

	/// <summary>
	/// タブ変更イベント
	/// </summary>
	public class TabChangeEventArgs : EventArgs {

		public AchievementTabType TabType { get; set; }
	}

	/// <summary>
	/// タブカウント変更イベント
	/// </summary>
	public class TabCountChangeEventArgs : EventArgs {

		public AchievementTabType TabType { get; set; }
		public int TabCount { get; set; }
	}

	#endregion ==== イベント ====

	/// <summary>
	/// アチーブメント制御インターフェイス
	/// </summary>
	public interface IController {

		#region ==== 初期化 ====

		/// <summary>
		/// 初期化
		/// </summary>
		void Setup();

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== アクティブ設定 ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		void SetActive( bool isActive, bool isTweenSkip );

		#endregion ==== アクティブ設定 ====

		#region ==== リスト更新 ====

		/// <summary>
		/// リスト更新
		/// </summary>
		/// <param name="list"></param>
		void UpdateList( List<AchievementInfo> list );

		#endregion ==== リスト更新 ====

		#region ==== イベント ====

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		event EventHandler<TabPageChangeEventArgs> OnPageChange;

		/// <summary>
		/// 全報酬取得イベント
		/// </summary>
		event EventHandler<EventArgs> OnGetAllReward;

		#endregion ==== イベント ====
	}

	/// <summary>
	/// アチーブメント制御
	/// </summary>
	public class Controller : IController {

		#region ==== 文字列 ====

		private string screenTitle { get { return MasterData.GetText( TextType.TX443_Achievement_Title ); } }
		private string emergencyHelpMessage { get { return MasterData.GetText( TextType.TX479_Achievement_EmergencyHelp ); } }
		private string priorityHelpMessage { get { return MasterData.GetText( TextType.TX480_Achievement_PriorityHelp ); } }
		private string event_1_HelpMessage { get { return MasterData.GetText( TextType.TX444_Achievement_Event_1_Help ); } }
		private string event_2_HelpMessage { get { return MasterData.GetText( TextType.TX481_Achievement_Event_2_Help ); } }
		private string event_3_HelpMessage { get { return MasterData.GetText( TextType.TX482_Achievement_Event_3_Help ); } }
		private string event_4_HelpMessage { get { return MasterData.GetText( TextType.TX483_Achievement_Event_4_Help ); } }
		private string dailyHelpMessage { get { return MasterData.GetText( TextType.TX445_Achievement_DailyHelp ); } }
		private string achievementHelpMessage { get { return MasterData.GetText( TextType.TX446_Achievement_AchievementHelp ); } }
		private string reserveHelpMessage { get { return MasterData.GetText( TextType.TX484_Achievement_ReserveHelp ); } }
		private string rewardHelpMessage { get { return MasterData.GetText( TextType.TX447_Achievement_AllRewardHelp ); } }


		#endregion ==== 文字列 ====

		#region ==== フィールド ====

		// モデル
		private readonly IModel _model;

		// ビュー
		private readonly IView _view;

		// リスト
		private GUIAchievementItemPageList achievementItemPageList;

		#endregion ==== フィールド ====

		#region ==== プロパティ ====

		// モデル
		private IModel Model { get { return _model; } }

		// ビュー
		private IView View { get { return _view; } }

		// リスト
		private GUIAchievementItemPageList AchievementItemPageList { get { return achievementItemPageList; } }

		/// <summary>
		/// 更新できる状態かどうか
		/// </summary>
		bool CanUpdate {
			get {
				if( this.Model == null ) return false;
				if( this.View == null ) return false;
				return true;
			}
		}

		#endregion ==== プロパティ ====

		#region ==== 初期化 ====

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Controller( IModel model, IView view, GUIAchievementItemPageList pageList ) {

			if( model == null || view == null || pageList == null ) return;

			// 初期化
			MemberInit();

			// モデル設定
			_model = model;

			// ビュー設定
			_view = view;
			View.OnHome			+= HandleHome;
			View.OnClose		+= HandleClose;
			View.OnTabChange	+= handleTabChange;
			View.OnGetAllReward	+= handleGetAllReward;

			// ページリスト
			achievementItemPageList = pageList;
			AchievementItemPageList.OnPageChange += handlePageChange;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Setup() {

			if( !CanUpdate ) return;

			// シリアライズされていないメンバーの初期化
			MemberInit();

			// セットアップ
			AchievementItemPageList.Setup();
		}

		/// <summary>
		/// シリアライズされていないメンバーの初期化
		/// </summary>
		private void MemberInit() {
		}

		#endregion ==== 初期化 ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			if( View != null ) {
				View.Dispose();
			}
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ設定 ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			if( this.CanUpdate ) {
				this.View.SetActive( isActive, isTweenSkip );

				// その他UIの表示設定
				GUILobbyResident.SetActive( !isActive );
				GUIScreenTitle.Play( isActive, screenTitle );
				GUIHelpMessage.Play( isActive, dailyHelpMessage );
			}
		}

		#endregion ==== アクティブ設定 ====

		#region ==== ホーム、閉じるボタンイベント ====

		void HandleHome( object sender, EventArgs e ) {

			GUIController.Clear();
		}

		void HandleClose( object sender, EventArgs e ) {

			GUIController.Back();
		}

		#endregion ==== ホーム、閉じるボタンイベント ====

		#region ==== リスト更新 ====

		/// <summary>
		/// リスト更新
		/// </summary>
		/// <param name="infos"></param>
		public void UpdateList( List<AchievementInfo> infos ) {

			bool[] tabCheck = new bool[( int )AchievementTabType.Reward];

			// アチーブ一覧の退避
			Model.InfoList = infos;

			// デフォルトタブはデイリー
			Model.TabType = AchievementTabType.DailyWeekly;

			// 一番小さいタブIDをデフォルトにする
			foreach( AchievementInfo info in infos ) {
				// タイプチェック
				if( Model.TabType > info.TabType ) {
					// デフォルトタブ変更
					Model.TabType = info.TabType;
				}
				// 有効タブ確認
				tabCheck[( int )info.TabType] = true;
			}
			// タブ有効設定
			View.SetTabEnabled( tabCheck );

			// 指定タブを現在のタブで開く
			openTab( Model.TabType );
		}

		#endregion

		#region ==== イベント ====

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		public event EventHandler<TabPageChangeEventArgs> OnPageChange = ( sender, e ) => { };

		/// <summary>
		/// 全報酬取得イベント
		/// </summary>
		public event EventHandler<EventArgs> OnGetAllReward = ( sender, e ) => { };

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// ページ変更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void handlePageChange( object sender, PageChangeEventArgs e ) {

			// ページ変更
			AchievementItemPageList.SetViewAchievementInfoList( Model.GetTabList( e.ItemIndex, e.ItemCount ));
		}

		/// <summary>
		/// タブ変更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void handleTabChange( object sender, TabChangeEventArgs e ) {

			openTab( e.TabType );
		}

		/// <summary>
		/// タブを開く
		/// </summary>
		/// <param name="type"></param>
		private void openTab( AchievementTabType type ) {

			int index = 0;
			int[] tabCount = new int[( int )AchievementTabType.Reward];
			string helpMsg = "";

			// 現在のタブタイプを変更
			Model.TabType = type;

			// リストクリア
			Model.TabList.Clear();

			// データ確認
			if( Model.InfoList != null ) {
				/*
				// リスト作成
				foreach( AchievementInfo i in Model.InfoList ) {

					if( i.Category == AchievementCategory.Event ||
						i.Category == AchievementCategory.EventDaily ) {
						// イベント・デイリーイベント
						if( type == AchievementTabType.Event_01 ) {
							// リストに追加
							addTabList( i, index++ );
						}
						if( i.RewardState == AchievementInfo.RewardStatus.Unacquired ) {
							// 未取得カウント
							eventTabCount++;

							// まとめて受け取る
							if( type == AchievementTabType.Reward ) {
								addTabList( i, index++ );
							}
						}
					} else if( i.Category == AchievementCategory.Daily ||
								i.Category == AchievementCategory.Weekly ) {
						// デイリー・ウィークリー
						if( type == AchievementTabType.DailyWeekly ) {
							// リストに追加
							addTabList( i, index++ );
						}
						if( i.RewardState == AchievementInfo.RewardStatus.Unacquired ) {
							// 未取得カウント
							dailyWeeklyTabCount++;

							// まとめて受け取る
							if( type == AchievementTabType.Reward ) {
								addTabList( i, index++ );
							}
						}
					} else if( i.Category == AchievementCategory.MedalBronze ||
								i.Category == AchievementCategory.MedalSilver ||
								i.Category == AchievementCategory.MedalGold ) {
						// アチーブ
						if( type == AchievementTabType.Achevement ) {
							// リストに追加
							addTabList( i, index++ );
						}
						if( i.RewardState == AchievementInfo.RewardStatus.Unacquired ) {
							// 未取得カウント
							achievementTabCount++;

							// まとめて受け取る
							if( type == AchievementTabType.Reward ) {
								addTabList( i, index++ );
							}
						}
					}
				}
				*/
				
				// リスト作成
				foreach( AchievementInfo i in Model.InfoList ) {
					// 未取得カウント
					if( i.RewardState == AchievementInfo.RewardStatus.Unacquired ) {
						// タブカウントアップ
						tabCount[( int )i.TabType]++;

						// まとめて取得タブならば、リストに追加
						if( type == AchievementTabType.Reward ) {
							addTabList( i, index++ );
						}
					}
					// 選択タブの場合、リストに追加
					if( i.TabType == type ) {
						addTabList( i, index++ );
					}
				}
			}
			// リストの数設定
			AchievementItemPageList.SetItemCount( Model.TabList.Count );

			// リストのセット
			AchievementItemPageList.SetViewAchievementInfoList( Model.GetTabList( 0, 20 ) );

			// タブ設定
			View.SetTab( Model.TabType, tabCount );

			// ヘルプメッセージの設定
			switch( type ) {
				case AchievementTabType.EmergencyEvent: helpMsg = emergencyHelpMessage;		break;
				case AchievementTabType.PriorityEvent:	helpMsg	= priorityHelpMessage;		break;
				case AchievementTabType.Event_01:		helpMsg	= event_1_HelpMessage;		break;
				case AchievementTabType.Event_02:		helpMsg = event_2_HelpMessage;		break;
				case AchievementTabType.Event_03:		helpMsg = event_3_HelpMessage;		break;
				case AchievementTabType.Event_04:		helpMsg = event_4_HelpMessage;		break;
				case AchievementTabType.DailyWeekly:	helpMsg	= dailyHelpMessage;			break;
				case AchievementTabType.Achevement:		helpMsg	= achievementHelpMessage;	break;
				case AchievementTabType.Reserve:		helpMsg = reserveHelpMessage;		break;
				case AchievementTabType.Reward:			helpMsg	= rewardHelpMessage;		break;
			}
			GUIHelpMessage.Play( true, helpMsg );
		}

		/// <summary>
		/// タブリストへアイテムを追加
		/// </summary>
		/// <param name="info"></param>
		/// <param name="index"></param>
		private void addTabList( AchievementInfo info, int index ) {

			info.Index = index;

			Model.TabList.Add( info );
		}

		/// <summary>
		/// 全リワードの取得
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void handleGetAllReward( object sender, EventArgs e ) {

			OnGetAllReward( sender, e );
		}

		#endregion ==== アクション ====
	}
}