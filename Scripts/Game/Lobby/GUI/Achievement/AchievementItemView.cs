/// <summary>
/// メールアイテム表示
/// 
/// 2016/05/16
/// </summary>

using UnityEngine;
using System;
using Scm.Common.GameParameter;

namespace XUI.AchievementItem {

	public interface IView {

		#region ==== イベント ====

		/// <summary>
		/// 取得ボタンが押下された時
		/// </summary>
		event EventHandler<EventArgs> OnGetButtonClick;

		#endregion ==== イベント ====

		#region ==== 初期化 ====

		/// <summary>
		/// セットアップ
		/// </summary>
		void Setup();

		#endregion ==== 初期化 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ状態の取得
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="isActive"></param>
		/// <param name="isTweemSkip"></param>
		void SetActive( bool isActive, bool isTweemSkip );

		/// <summary>
		/// アクティブ状態の取得
		/// </summary>
		/// <returns></returns>
		GUIViewBase.ActiveState GetActiveState();

		#endregion ==== アクティブ ====

		#region ==== 各種項目設定 ====

		/// <summary>
		/// アチーブメント設定
		/// </summary>
		/// <param name="info"></param>
		void SetAchievementInfo( AchievementInfo info );

		#endregion ==== 各種項目設定 ====
	}

	[DisallowMultipleComponent]
	public class AchievementItemView : GUIViewBase, IView {

		#region ==== フィールド ====

		#region ---- 共通項目 ----

		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private UILabel titleLabel = null;

		/// <summary>
		/// 達成条件
		/// </summary>
		[SerializeField]
		private UILabel requirementsLabel = null;

		/// <summary>
		/// 報酬内容
		/// </summary>
		[SerializeField]
		private UILabel rewardDetailsLabel = null;

		#endregion ---- 共通項目 ----

		#region ---- イベントデイリー ----

		/// <summary>
		/// イベントデイリータグ
		/// </summary>
		[SerializeField]
		private GameObject categoryEventDaily = null;

		/// <summary>
		/// イベントデイリーID
		/// </summary>
		[SerializeField]
		private UILabel categoryEventDailyLabel = null;

		#endregion ---- イベントデイリー ----

		#region ---- イベント ----

		/// <summary>
		/// イベントタグ
		/// </summary>
		[SerializeField]
		private GameObject categoryEvent = null;

		/// <summary>
		/// イベントID
		/// </summary>
		[SerializeField]
		private UILabel categoryEventLabel = null;

		#endregion ---- イベント ----

		#region ---- デイリー ----

		/// <summary>
		/// デイリータグ
		/// </summary>
		[SerializeField]
		private GameObject categoryDaily = null;

		/// <summary>
		/// デイリーID
		/// </summary>
		[SerializeField]
		private UILabel categoryDailyLabel = null;

		#endregion ---- デイリー ----

		#region ---- ウィークリー ----

		/// <summary>
		/// ウィークリータグ
		/// </summary>
		[SerializeField]
		private GameObject categoryWeekly = null;

		/// <summary>
		/// ウィークリーID
		/// </summary>
		[SerializeField]
		private UILabel categoryWeeklyLabel = null;

		#endregion ---- ウィークリー ----

		#region ---- ブロンズメダル ----

		/// <summary>
		/// ブロンズメダルタグ
		/// </summary>
		[SerializeField]
		private GameObject categoryMedalBronze = null;

		/// <summary>
		/// ブロンズメダルID
		/// </summary>
		[SerializeField]
		private UILabel categoryMedalBronzeLabel = null;

		#endregion

		#region ---- シルバーメダル ----

		/// <summary>
		/// シルバーメダルタグ
		/// </summary>
		[SerializeField]
		private GameObject categoryMedalSilver = null;

		/// <summary>
		/// シルバーメダルID
		/// </summary>
		[SerializeField]
		private UILabel categoryMedalSilverLabel = null;

		#endregion ---- シルバーメダル ----

		#region ---- ゴールドメダル ----

		/// <summary>
		/// ゴールドメダルタグ
		/// </summary>
		[SerializeField]
		private GameObject categoryMedalGold = null;

		/// <summary>
		/// ゴールドメダルID
		/// </summary>
		[SerializeField]
		private UILabel categoryMedalGoldLabel = null;

		#endregion ---- ゴールドメダル ----

		#region ---- 進捗 ----

		/// <summary>
		/// 未達成
		/// </summary>
		[SerializeField]
		private GameObject progressPercent = null;

		[SerializeField]
		private UILabel progressPercentNumLabel = null;

		[SerializeField]
		private UISlider progressPercentSlider = null;

		/// <summary>
		/// 達成済み
		/// </summary>
		[SerializeField]
		private GameObject progressReceiveTerm = null;

		[SerializeField]
		private UILabel progressReceiveTermNumLabel = null;

		/// <summary>
		/// 取得済み
		/// </summary>
		[SerializeField]
		private GameObject progressCompleted = null;

		#endregion ---- 進捗 ----

		#region ---- 取得ボタン ----

		/// <summary>
		/// 取得ボタン
		/// </summary>
		[SerializeField]
		private UIButton button = null;

		/// <summary>
		/// 取得ボタンラベル
		/// </summary>
		[SerializeField]
		private UILabel buttonLabel = null;

		#endregion ---- 取得ボタン ----

		#region ---- 文字列フォーマット ----

		/// <summary>
		/// 進捗フォーマット
		/// </summary>
		[SerializeField]
		private string progressNumFormat = "{0}/{1}";

		#endregion ---- 文字列フォーマット ----

		#endregion ==== フィールド ====

		#region ==== イベント ====

		/// <summary>
		/// 取得ボタンを押下した時
		/// </summary>
		public event EventHandler<EventArgs> OnGetButtonClick = ( sender, e ) => { };

		#endregion ==== イベント ====

		#region ==== 初期化 ====

		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup() {

			// 表示設定とか
		}

		#endregion ==== 初期化 ====

		#region ==== アクティブ ====

		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="isActive"></param>
		/// <param name="isTweenSkip"></param>
		public void SetActive( bool isActive, bool isTweenSkip ) {

			this.SetRootActive( isActive, isTweenSkip );
		}

		/// <summary>
		/// アクティブ状態の取得
		/// </summary>
		/// <returns></returns>
		public ActiveState GetActiveState() {

			return GetRootActiveState();
		}

		#endregion ==== アクティブ ====

		#region ==== 破棄 ====

		/// <summary>
		/// 破棄
		/// </summary>
		private void OnDestroy() {

			OnGetButtonClick = null;
		}

		#endregion ==== 破棄 ====

		#region ==== NGUIリフレクション ====

		public void OnGetButton() {

			OnGetButtonClick( this, EventArgs.Empty );
		}

		#endregion ==== NGUIリフレクション ====

		#region ==== 各種項目設定 ====

		/// <summary>
		/// アイテムの項目設定
		/// </summary>
		/// <param name="info"></param>
		public void SetAchievementInfo( AchievementInfo info ) {

			if( info == null ) return;

			GameObject category = null;
			UILabel label = null;

			// 設定クリア
			resetInfo();

			// カテゴリ確認
			switch( info.Category ) {
				case AchievementCategory.EventDaily:	category = categoryEventDaily;	label = categoryEventDailyLabel;	break;
				case AchievementCategory.Event:			category = categoryEvent;		label = categoryEventLabel;			break;
				case AchievementCategory.Daily:			category = categoryDaily;		label = categoryDailyLabel;			break;
				case AchievementCategory.Weekly:		category = categoryWeekly;		label = categoryWeeklyLabel;		break;
				case AchievementCategory.MedalBronze:	category = categoryMedalBronze;	label = categoryMedalBronzeLabel;	break;
				case AchievementCategory.MedalSilver:	category = categoryMedalSilver;	label = categoryMedalSilverLabel;	break;
				case AchievementCategory.MedalGold:		category = categoryMedalGold;	label = categoryMedalGoldLabel;		break;
			}

			if( ( category != null ) && ( label != null ) ) {
				// カテゴリ設定
				category.SetActive( true );
				label.text = info.AchievementID.ToString();

				// 進捗処理
				if( info.RewardState == AchievementInfo.RewardStatus.NotAchieved ) {
					// 未達成
					progressPercent.SetActive( true );
					progressPercentNumLabel.text = string.Format( progressNumFormat, info.Progress, info.ProgressThreshold );
					progressPercentSlider.value = ( float )info.Progress / info.ProgressThreshold;

					// 取得ボタン
					button.gameObject.SetActive( false );

				} else if( info.RewardState == AchievementInfo.RewardStatus.Unacquired ) {
					// 未取得
					progressReceiveTerm.SetActive( true );
					progressReceiveTermNumLabel.text = info.RewardDeadline;

					// 取得ボタン
					button.gameObject.SetActive( true );
					button.isEnabled = true;
					buttonLabel.text = MasterData.GetText( TextType.TX465_Achievement_GetButton );

				} else {
					// 取得済み
					progressCompleted.SetActive( true );

					// 取得ボタン
					button.gameObject.SetActive( true );
					button.isEnabled = false;
					buttonLabel.text = MasterData.GetText( TextType.TX466_Achievement_AcquiredButton );
				}

				// タイトル設定
				titleLabel.text = info.Title;

				// 達成条件設定
				requirementsLabel.text = info.Description;

				// 報酬内容設定
				rewardDetailsLabel.text = info.RewardContent;
			}
			// アクティブ化
			SetActive( true, true );
		}

		/// <summary>
		/// アイテムの項目リセット
		/// </summary>
		private void resetInfo() {

			// カテゴリ
			categoryEventDaily.SetActive( false );
			categoryEvent.SetActive( false );
			categoryDaily.SetActive( false );
			categoryWeekly.SetActive( false );
			categoryMedalBronze.SetActive( false );
			categoryMedalSilver.SetActive( false );
			categoryMedalGold.SetActive( false );

			// 未達成
			progressPercent.SetActive( false );

			// 未取得
			progressReceiveTerm.SetActive( false );

			// 取得済み
			progressCompleted.SetActive( false );

			// 取得ボタン
			button.gameObject.SetActive( false );
		}

		#endregion ==== 各種項目設定 ====
	}
}
