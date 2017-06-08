/// <summary>
/// アチーブメント
/// 
/// 2016/04/26
/// </summary>

using UnityEngine;
using XUI.Achievement;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent( typeof( AchievementView ) )]
public class GUIAchievement : Singleton<GUIAchievement> {

	#region ==== フィールド ====

	/// <summary>
	/// ビュー
	/// </summary>
	[SerializeField]
	private AchievementView _viewAttach = null;

	/// <summary>
	/// アチーブメントページリスト
	/// </summary>
	[SerializeField]
	private GUIAchievementItemPageList achievementItemPageList = null;

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	[SerializeField]
	private bool _isStartActive = false;

	#endregion ==== フィールド ====

	#region ==== プロパティ ====

	/// <summary>
	/// ビュー
	/// </summary>
	private AchievementView ViewAttach { get { return _viewAttach; } }

	// コントローラー
	private IController Controller { get; set; }

	/// <summary>
	/// 開始時のアクティブ状態
	/// </summary>
	private bool IsStartActive { get { return _isStartActive; } }

	#endregion ==== プロパティ ====

	#region ==== 初期化 ====

	/// <summary>
	/// 起動
	/// </summary>
	protected override void Awake() {

		base.Awake();
		MemberInit();
	}

	/// <summary>
	/// 開始
	/// </summary>
	private void Start() {

		Construct();

		// 初期アクティブ設定
		SetActive( IsStartActive, true, IsStartActive );
	}

	/// <summary>
	/// シリアライズされていないメンバー初期化
	/// </summary>
	private void MemberInit() {

		Controller = null;
	}

	/// <summary>
	/// 作成
	/// </summary>
	private void Construct() {

		// モデル生成
		var model = new Model();

		// ビュー生成
		IView view = null;
		if( ViewAttach != null ) {
			view = ViewAttach.GetComponent( typeof( IView ) ) as IView;
			view.SetupTabLabel();
		}

		// コントローラー生成
		Controller = new Controller( model, view, achievementItemPageList );
		Controller.OnGetAllReward	+= handleGetAllReward;
	}

	#endregion ==== 初期化 ====

	#region ==== 破棄 ====

	/// <summary>
	/// 破棄
	/// </summary>
	private void OnDestroy() {

		if( Controller != null ) {
			Controller.Dispose();
		}
	}

	#endregion ==== 破棄 ====

	#region ==== アクティブ設定 ====

	/// <summary>
	/// 閉じる
	/// </summary>
	public static void Close() {

		if( Instance != null ) Instance.SetActive( false, false, false );
	}

	/// <summary>
	/// 開く
	/// </summary>
	public static void Open() {

		if( Instance != null ) Instance.SetActive( true, false, true );
	}

	/// <summary>
	/// 開き直す
	/// </summary>
	public static void ReOpen() {

		if( Instance != null ) Instance.SetActive( true, false, false );
	}

	/// <summary>
	/// アクティブ設定
	/// </summary>
	private void SetActive( bool isActive, bool isTweenSkip, bool isSetup ) {

		// セットアップ
		if( isSetup ) {
			Setup();
		}

		// コントローラのアクティブ設定
		if( Controller != null ) {
			Controller.SetActive( isActive, isTweenSkip );
		}

		// アチーブ一覧の要求
		if( isActive ) {
			achievementListRequest();
		}
	}

	#endregion ==== アクティブ設定 ====

	#region ==== 各種情報更新 ====

	/// <summary>
	/// 初期設定
	/// </summary>
	private void Setup() {

		// コントローラセットアップ
		if( Controller != null ) {
			Controller.Setup();
		}

		// 全タブ非表示
		if( ViewAttach != null ) {
			ViewAttach.SetTabEnabled( new bool[( int )AchievementTabType.Reward] );
		}
	}

	#endregion ==== 各種情報更新 ====

	#region ==== アクション ====

	/// <summary>
	/// リワード取得
	/// </summary>
	/// <param name="achieveMasterId"></param>
	public static void GetReward( int achieveMasterId ) {

		if( Instance != null ) {
			Instance.getReward( achieveMasterId );
		}
	}

	private void getReward( int rewardId ) {

		// リワード取得要求
		getRewardRequest( rewardId );
	}

	#endregion ==== アクション ====

	#region ==== 通信系 ====

	#region ---- アチーブ一覧取得 ----

	/// <summary>
	/// アチーブメント一覧を要求する
	/// </summary>
	private void achievementListRequest() {

		LobbyPacket.SendAchievement( achievementResponse );
	}

	/// <summary>
	/// 一覧要求結果
	/// </summary>
	/// <param name="args"></param>
	private void achievementResponse( LobbyPacket.AchievementResArgs args ) {

		// リスト一覧更新
		Controller.UpdateList( args.List );

		// ロビーのアチーブアイコン未取得数更新
		GUILobbyResident.SetAchieveUnreceived( args.AchievemrntRewardCount );
	}

	#endregion ---- アチーブ一覧取得 ----

	#region ---- アチーブ報酬受取 ----

	/// <summary>
	/// 指定リワードの受取要求
	/// </summary>
	/// <param name="rewardId">リワードID</param>
	private void getRewardRequest( int rewardId ) {

		LobbyPacket.SendReceiveAchievementReward( rewardId, achievementRewardResponse );
	}

	/// <summary>
	/// 指定リワード要求結果
	/// </summary>
	/// <param name="args"></param>
	private void achievementRewardResponse( LobbyPacket.ReceiveAchievementRewardResArgs args ) {

		// ロビーのアチーブアイコン未取得数更新
		GUILobbyResident.SetAchieveUnreceived( args.AchievemrntRewardCount );

		// リワードの確認
		if( args.List.Count > 0 ) {
			// infoの取得
			AchievementRewardInfo info = args.List[0];

			// リワードアイテムの表示
			if( info.Reward.Count > 0 ) {
				// 結果確認
				if( info.Result ) {
					// リワードアイテムの取得
					AchievementRewardInfo.RewardItem item = info.Reward[0];

					// 取得リワードの表示
					GUIMessageWindow.SetModeOK( string.Format( MasterData.GetText( TextType.TX462_Achievement_GetOneMessage ), item.Name, item.Num ), true, null );

				} else {
					// 取得失敗
					GUIMessageWindow.SetModeOK( MasterData.GetText( TextType.TX469_Achievement_RewardGetFailure ), true, null );
				}
			}
		}

		// リスト再取得
		achievementListRequest();
	}

	#endregion ---- アチーブ報酬受取 ----

	#region ---- アチーブ報酬一括受取 ----

	/// <summary>
	/// 全リワード取得確認
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	void handleGetAllReward( object sender, EventArgs args ) {

		GUIMessageWindow.SetModeYesNo( MasterData.GetText( TextType.TX463_Achievement_GetAllCheckMessage ), achievementRewardAllRequest, null ); 
	}

	/// <summary>
	/// 全リワードの取得要求
	/// </summary>
	void achievementRewardAllRequest() {

		LobbyPacket.SendReceiveAchievementRewardAll( achievementRewardAllResponse );
	}

	/// <summary>
	/// 全リワードの要求結果
	/// </summary>
	/// <param name="args"></param>
	void achievementRewardAllResponse( LobbyPacket.ReceiveAchievementRewardAllResArgs args ) {

		// 取得件数の表示
		GUIMessageWindow.SetModeOK( string.Format( MasterData.GetText( TextType.TX464_Achievement_GetAllMessage ), args.ReceiveCount ), true, null );

		// ロビーのアチーブアイコン未取得数更新
		GUILobbyResident.SetAchieveUnreceived( 0 );

		// リスト再取得
		achievementListRequest();
	}

	#endregion ---- アチーブ報酬一括受取 ----

	#endregion ==== 通信系 ====



	#region ==== デバッグ ====

#if UNITY_EDITOR && XW_DEBUG

	[SerializeField]
	GUIDebugParam _debugParam = new GUIDebugParam();

	GUIDebugParam DebugParam { get { return _debugParam; } }

	[System.Serializable]
	public class GUIDebugParam : GUIDebugParamBase {
		public GUIDebugParam() {
		}

		[SerializeField]
		int listCreateNum = 30;
		public int ListCreateNum { get { return listCreateNum; } }

		[SerializeField]
		TemprateEvent _list = new TemprateEvent();

		public TemprateEvent List { get { return _list; } }

		[System.Serializable]
		public class TemprateEvent : IDebugParamEvent {
			public event System.Action Execute = delegate { };

			[SerializeField]
			bool execute = false;

			public void Update() {
				if( this.execute ) {
					this.execute = false;
					this.Execute();
				}
			}
		}
	}

	private void DebugInit() {

		var d = this.DebugParam;

		d.ExecuteClose += Close;
		d.ExecuteActive += () => {
			d.ReadMasterData();
			Open();
		};

		d.List.Execute += ( ) => {
			// デバッグ用適当リスト作成
			List<AchievementInfo> list = new System.Collections.Generic.List<AchievementInfo>();
			Scm.Common.GameParameter.AchievementCategory category = Scm.Common.GameParameter.AchievementCategory.Daily;
			AchievementTabType type = AchievementTabType.EmergencyEvent;
			for( int i = 0 ; i < DebugParam.ListCreateNum ; i++ ) {
				type = ( AchievementTabType )UnityEngine.Random.Range( 0, 9 );
				switch( type ) {
					case AchievementTabType.EmergencyEvent:
					case AchievementTabType.PriorityEvent:
					case AchievementTabType.Event_01:
					case AchievementTabType.Event_02:
					case AchievementTabType.Event_03:
					case AchievementTabType.Event_04:
						category = ( Scm.Common.GameParameter.AchievementCategory )UnityEngine.Random.Range( 6, 8 );
						break;
					case AchievementTabType.DailyWeekly:
						category = ( Scm.Common.GameParameter.AchievementCategory )UnityEngine.Random.Range( 1, 3 );
						break;
					case AchievementTabType.Achevement:
						category = ( Scm.Common.GameParameter.AchievementCategory )UnityEngine.Random.Range( 3, 6 );
						break;
					case AchievementTabType.Reserve:
						category = ( Scm.Common.GameParameter.AchievementCategory )UnityEngine.Random.Range( 1, 8 );
						break;
				}
				list.Add( new AchievementInfo( i, i, "テスト:" + i, "説明:" + i, category, null, type, false, false, false, UnityEngine.Random.Range( 0, 100 ), 100, ( AchievementInfo.RewardStatus )UnityEngine.Random.Range( 0, 3 ), "なにか:" + i + " x" + i, "期限:" + i ) );
			}
			Controller.UpdateList( list );
		};
	}

	bool _isDebugInit = false;

	private void DebugUpdate() {

		if( !this._isDebugInit ) {
			this._isDebugInit = true;
			this.DebugInit();
		}

		this.DebugParam.Update();
		this.DebugParam.List.Update();
	}

	/// <summary>
	/// OnValidate はInspector上で値の変更があった時に呼び出される
	/// GameObject がアクティブ状態じゃなくても呼び出されるのでアクティブ化するときには有効
	/// </summary>
	void OnValidate() {

		if( Application.isPlaying ) {
			this.DebugUpdate();
		}
	}

#endif

	#endregion ==== デバッグ ====
}
