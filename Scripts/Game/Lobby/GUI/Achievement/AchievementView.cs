/// <summary>
/// アチーブメント表示
/// 
/// 2016/04/25
/// </summary>

using System;
using UnityEngine;
using Scm.Common.XwMaster;

namespace XUI.Achievement {

	/// <summary>
	/// アチーブメント表示インターフェイス
	/// </summary>
	public interface IView {

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		void Dispose();

		#endregion ==== 破棄 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		void SetActive( bool isActive, bool isTweenSkip );

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		GUIViewBase.ActiveState GetActiveState();

		#endregion ==== アクティブ ====

		#region ==== ホーム、閉じるボタンイベント ====

		event EventHandler<EventArgs> OnHome;
		event EventHandler<EventArgs> OnClose;

		#endregion ==== ホーム、閉じるボタンイベント ====

		#region ==== イベント ====

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		event EventHandler<TabChangeEventArgs> OnTabChange;

		/// <summary>
		/// まとめて受け取るボタンクリックイベント
		/// </summary>
		event EventHandler<EventArgs> OnGetAllReward;

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// タブラベルの初期化
		/// </summary>
		void SetupTabLabel();

		/// <summary>
		/// タブの有効設定
		/// </summary>
		/// <param name="tabCheck"></param>
		void SetTabEnabled( bool[] tabCheck );

		/// <summary>
		/// タブ設定
		/// </summary>
		/// <param name="type"></param>
		/// <param name="tabCount"></param>
		void SetTab( AchievementTabType type, int[] tabCount );

		#endregion ==== アクション ====
	}

	/// <summary>
	/// アチーブメント表示
	/// </summary>
	public class AchievementView : GUIScreenViewBase, IView {

		#region ==== フィールド ====

		/// <summary>
		/// タブグリッド
		/// </summary>
		[SerializeField]
		private UIGrid uiTabGrid = null;

		/// <summary>
		/// 緊急タブ
		/// </summary>
		[SerializeField]
		private UIButton emergencyEventTabButton = null;

		[SerializeField]
		private UILabel emergencyEventTabLabel = null;

		[SerializeField]
		private UILabel emergencyEventCountLabel = null;

		/// <summary>
		/// 優先タブ
		/// </summary>
		[SerializeField]
		private UIButton priorityEventTabButton = null;

		[SerializeField]
		private UILabel priorityEventTabLabel = null;

		[SerializeField]
		private UILabel priorityEventCountLabel = null;

		/// <summary>
		/// イベント1
		/// </summary>
		[SerializeField]
		private UIButton eventTabButton_1 = null;

		[SerializeField]
		private UILabel eventTabLabel_1 = null;

		[SerializeField]
		private UILabel eventCountLabel_1 = null;

		/// <summary>
		/// イベント2
		/// </summary>
		[SerializeField]
		private UIButton eventTabButton_2 = null;

		[SerializeField]
		private UILabel eventTabLabel_2 = null;

		[SerializeField]
		private UILabel eventCountLabel_2 = null;

		/// <summary>
		/// イベント3
		/// </summary>
		[SerializeField]
		private UIButton eventTabButton_3 = null;

		[SerializeField]
		private UILabel eventTabLabel_3 = null;

		[SerializeField]
		private UILabel eventCountLabel_3 = null;

		/// <summary>
		/// イベント4
		/// </summary>
		[SerializeField]
		private UIButton eventTabButton_4 = null;

		[SerializeField]
		private UILabel eventTabLabel_4 = null;

		[SerializeField]
		private UILabel eventCountLabel_4 = null;

		/// <summary>
		/// デイリー・ウィークリー
		/// </summary>
		[SerializeField]
		private UIButton dailyWeeklyTabButton = null;

		[SerializeField]
		private UILabel dailyWeeklyTabLabel = null;

		[SerializeField]
		private UILabel dailyWeeklyCountLabel = null;

		/// <summary>
		/// アチーブメント
		/// </summary>
		[SerializeField]
		private UIButton achievementTabButton = null;

		[SerializeField]
		private UILabel achievementTabLabel = null;

		[SerializeField]
		private UILabel achievementCountLabel = null;

		/// <summary>
		/// 予備
		/// </summary>
		[SerializeField]
		private UIButton reserveTabButton = null;

		[SerializeField]
		private UILabel reserveTabLabel = null;

		[SerializeField]
		private UILabel reserveCountLabel = null;

		/// <summary>
		/// 全て取得
		/// </summary>
		[SerializeField]
		private UIButton allReciveTabButton = null;

		[SerializeField]
		private UILabel allReciveTabLabel = null;

		[SerializeField]
		private UILabel allReciveCountLabel = null;

		/// <summary>
		/// タブカウントフォーマット
		/// </summary>
		[SerializeField]
		private string tabCountFormat = "({0})";

		/// <summary>
		/// 全て取得ボタン
		/// </summary>
		[SerializeField]
		private UIButton allReceiveButton = null;

		#endregion ==== フィールド ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {

			this.OnHome = null;
			this.OnClose = null;
			this.OnTabChange = null;
			this.OnGetAllReward = null;
		}

		#endregion ==== 破棄 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態にする
		/// </summary>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			this.SetRootActive( isActive, isTweenSkip );
		}

		/// <summary>
		/// 現在のアクティブ状態を取得する
		/// </summary>
		public GUIViewBase.ActiveState GetActiveState() {

			return this.GetRootActiveState();
		}

		#endregion ==== アクティブ ====

		#region ホーム、閉じるボタンイベント

		public event EventHandler<EventArgs> OnHome = ( sender, e ) => { };
		public event EventHandler<EventArgs> OnClose = ( sender, e ) => { };

		/// <summary>
		/// ホームボタンイベント
		/// </summary>
		public override void OnHomeEvent() {

			// 通知
			this.OnHome( this, EventArgs.Empty );
		}

		/// <summary>
		/// 閉じるボタンイベント
		/// </summary>
		public override void OnCloseEvent() {

			// 通知
			this.OnClose( this, EventArgs.Empty );
		}
		#endregion

		#region ==== イベント ====

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		public event EventHandler<TabChangeEventArgs> OnTabChange = ( sender, e ) => { };

		/// <summary>
		/// まとめて受け取るボタンクリックイベント
		/// </summary>
		public event EventHandler<EventArgs> OnGetAllReward = ( sender, e ) => { };

		#endregion ==== イベント ====

		#region ==== アクション ====

		/// <summary>
		/// タブラベルの設定
		/// </summary>
		public void SetupTabLabel() {

			AchievementTabMasterData data;

			// 緊急タブ
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.EmergencyEvent, out data ) ) {
				emergencyEventTabLabel.text = data.DisplayName;
			}
			// 優先タブ
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.PriorityEvent, out data ) ) {
				priorityEventTabLabel.text = data.DisplayName;
			}
			// イベントタブ1
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.Event_01, out data ) ) {
				eventTabLabel_1.text = data.DisplayName;
			}
			// イベントタブ2
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.Event_02, out data ) ) {
				eventTabLabel_2.text = data.DisplayName;
			}
			// イベントタブ3
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.Event_03, out data ) ) {
				eventTabLabel_3.text = data.DisplayName;
			}
			// イベントタブ4
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.Event_04, out data ) ) {
				eventTabLabel_4.text = data.DisplayName;
			}
			// デイリー・ウィークリータブ
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.DailyWeekly, out data ) ) {
				dailyWeeklyTabLabel.text = data.DisplayName;
			}
			// アチーブメントタブ
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.Achevement, out data ) ) {
				achievementTabLabel.text = data.DisplayName;
			}
			// 予備タブ
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.Reserve, out data ) ) {
				reserveTabLabel.text = data.DisplayName;
			}
			// 全て受け取りタブ
			if( MasterData.TryGetAchievementTab( ( int )AchievementTabType.Reward, out data ) ) {
				allReciveTabLabel.text = data.DisplayName;
			}
		}

		/// <summary>
		/// タブの有効設定
		/// </summary>
		/// <param name="tabCheck"></param>
		public void SetTabEnabled( bool[] tabCheck ) {

			// タブ表示設定
			emergencyEventTabButton.gameObject.SetActive( tabCheck[0] );
			priorityEventTabButton.gameObject.SetActive( tabCheck[1] );
			eventTabButton_1.gameObject.SetActive( tabCheck[2] );
			eventTabButton_2.gameObject.SetActive( tabCheck[3] );
			eventTabButton_3.gameObject.SetActive( tabCheck[4] );
			eventTabButton_4.gameObject.SetActive( tabCheck[5] );
			dailyWeeklyTabButton.gameObject.SetActive( tabCheck[6] );
			achievementTabButton.gameObject.SetActive( tabCheck[7] );
			reserveTabButton.gameObject.SetActive( tabCheck[8] );
		}

		/// <summary>
		/// タブの変更
		/// </summary>
		/// <param name="type"></param>
		/// <param name="tabCount"></param>
		public void SetTab( AchievementTabType type, int[] tabCount ) {

			int allTabCount = 0;

			// 全リワード数の確認
			for( int i = 0 ; i < tabCount.Length ; i++ ) {
				allTabCount += tabCount[i];
			}

			// タブ有効設定
			emergencyEventTabButton.isEnabled	= ( type != AchievementTabType.EmergencyEvent );
			priorityEventTabButton.isEnabled	= ( type != AchievementTabType.PriorityEvent );
			eventTabButton_1.isEnabled			= ( type != AchievementTabType.Event_01 );
			eventTabButton_2.isEnabled			= ( type != AchievementTabType.Event_02 );
			eventTabButton_3.isEnabled			= ( type != AchievementTabType.Event_03 );
			eventTabButton_4.isEnabled			= ( type != AchievementTabType.Event_04 );
			dailyWeeklyTabButton.isEnabled		= ( type != AchievementTabType.DailyWeekly );
			achievementTabButton.isEnabled		= ( type != AchievementTabType.Achevement );
			reserveTabButton.isEnabled			= ( type != AchievementTabType.Reserve );
			allReciveTabButton.isEnabled		= ( type != AchievementTabType.Reward );

			// タブカウント変更
			emergencyEventCountLabel.text	= ( tabCount[0] > 0 )?	string.Format( tabCountFormat, tabCount[0] ) : "";
			priorityEventCountLabel.text	= ( tabCount[1] > 0 )?	string.Format( tabCountFormat, tabCount[1] ) : "";
			eventCountLabel_1.text			= ( tabCount[2] > 0 )?	string.Format( tabCountFormat, tabCount[2] ) : "";
			eventCountLabel_2.text			= ( tabCount[3] > 0 )?	string.Format( tabCountFormat, tabCount[3] ) : "";
			eventCountLabel_3.text			= ( tabCount[4] > 0 )?	string.Format( tabCountFormat, tabCount[4] ) : "";
			eventCountLabel_4.text			= ( tabCount[5] > 0 )?	string.Format( tabCountFormat, tabCount[5] ) : "";
			dailyWeeklyCountLabel.text		= ( tabCount[6] > 0 )?	string.Format( tabCountFormat, tabCount[6] ) : "";
			achievementCountLabel.text		= ( tabCount[7] > 0 )?	string.Format( tabCountFormat, tabCount[7] ) : "";
			reserveCountLabel.text			= ( tabCount[8] > 0 )?	string.Format( tabCountFormat, tabCount[8] ) : "";
			allReciveCountLabel.text		= ( allTabCount > 0 )?	string.Format( tabCountFormat, allTabCount ) : "";

			// 全て取得ボタン表示設定
			allReceiveButton.gameObject.SetActive( type == AchievementTabType.Reward );
			allReceiveButton.isEnabled = ( allTabCount > 0 );

			// リポジション
			if( uiTabGrid != null ) {
				uiTabGrid.Reposition();
			}
		}

		/// <summary>
		/// タブ変更
		/// </summary>
		/// <param name="type"></param>
		private void tabChange( AchievementTabType type ) {

			// イベントタブを開く
			TabChangeEventArgs args = new TabChangeEventArgs() {
				TabType = type
			};

			OnTabChange( this, args );
		}

		#endregion ==== アクション ====

		#region ==== NGUIリフレクション ====

		/// <summary>
		/// 緊急イベントタブクリック
		/// </summary>
		public void OnEmergencyEventTabClick() {

			// タブ変更
			tabChange( AchievementTabType.EmergencyEvent );
		}

		/// <summary>
		/// 優先イベントタブクリック
		/// </summary>
		public void OnPriorityEventTabClick() {

			// タブ変更
			tabChange( AchievementTabType.PriorityEvent );
		}

		/// <summary>
		/// イベント1タブクリック
		/// </summary>
		public void OnEvent_01_TabClick() {

			// タブ変更
			tabChange( AchievementTabType.Event_01 );
		}

		/// <summary>
		/// イベント2タブクリック
		/// </summary>
		public void OnEvent_02_TabClick() {

			// タブ変更
			tabChange( AchievementTabType.Event_02 );
		}

		/// <summary>
		/// イベント3タブクリック
		/// </summary>
		public void OnEvent_03_TabClick() {

			// タブ変更
			tabChange( AchievementTabType.Event_03 );
		}

		/// <summary>
		/// イベント4タブクリック
		/// </summary>
		public void OnEvent_04_TabClick() {

			// タブ変更
			tabChange( AchievementTabType.Event_04 );
		}

		/// <summary>
		/// デイリー＆ウィークリータブクリック
		/// </summary>
		public void OnDailyWeeklyTabClick() {

			// タブ変更
			tabChange( AchievementTabType.DailyWeekly );
		}

		/// <summary>
		/// アチーブメントタブクリック
		/// </summary>
		public void OnAchievementTabClick() {

			// タブ変更
			tabChange( AchievementTabType.Achevement );
		}

		/// <summary>
		/// 予備タブクリック
		/// </summary>
		public void OnReserveTabClick() {

			// タブ変更
			tabChange( AchievementTabType.Reserve );
		}

		/// <summary>
		/// まとめて取得タブクリック
		/// </summary>
		public void OnGetAllRewardTabClick() {

			// タブ変更
			tabChange( AchievementTabType.Reward );
		}

		/// <summary>
		/// まとめて受け取るボタンクリック
		/// </summary>
		public void OnGetAllRewardButtonClick() {

			OnGetAllReward( this, EventArgs.Empty );
		}

		#endregion ==== NGUIリフレクション ====
	}
}
