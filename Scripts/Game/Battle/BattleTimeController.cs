/// <summary>
/// 戦闘中の時間を制御するクラス
/// 
/// 2014/08/22
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;
using Scm.Common.GameParameter;

public class BattleTimeController
{
	#region フィールド&プロパティ

	/// <summary>
	/// 戦闘開始から戦闘終了するまでの時間
	/// </summary>
	public int TotalGameSecond { get; private set; }

	/// <summary>
	/// 残り時間
	/// </summary>
	private float RemainingTimeServer { get; set; }

	/// <summary>
	/// サーバとの残り時間のズレ(チート検出用)
	/// </summary>
	private float RemainingTimeGap { get; set; }

	/// <summary>
	/// 前回の状態
	/// </summary>
	private FieldStateType oldStateType;

	/// <summary>
	/// 経過時間情報
	/// </summary>
	private Stack<BattleFieldTimeEventMasterData> elapsedTimeMasterDataStack = new Stack<BattleFieldTimeEventMasterData>();

	/// <summary>
	/// 残り時間情報.
	/// </summary>
	private Stack<BattleFieldTimeEventMasterData> remainingTimeMasterDataStack = new Stack<BattleFieldTimeEventMasterData>();

    /// <summary>
    /// Is restarting a match?
    /// </summary>
    private bool isRestarting = false;

    #endregion

    #region コンストラクタ

    public BattleTimeController()
	{
		this.TotalGameSecond = 0;
		this.RemainingTimeServer = 0f;
		this.RemainingTimeGap = 0f;
	}
	
	#endregion

	#region 更新

	/// <summary>
	/// 毎フレーム更新させるもの
	/// </summary>
	public void Update()
	{
		// 残り時間カウンター
		this.RemainingTimeServer -= Time.deltaTime;
		this.RemainingTimeServer = Mathf.Max(0f, this.RemainingTimeServer);
	}

	#endregion

	#region 初期設定

	/// <summary>
	/// 時間の初期設定を行う
	/// </summary>
	public void SetupStartTimer(int battleFieldId)
	{
		BattleFieldMasterData bfData;
		if (MasterData.TryGetBattleField(battleFieldId, out bfData))
		{
			this.TotalGameSecond = bfData.GameTime;
			// バトルタイプ設定
			GUITacticalGauge.SetBattleType(bfData.BattleType);
		}
	}

	#endregion

	#region 残り時間セットアップ

	/// <summary>
	/// 残り時間のセット
	/// </summary>
	public void SetRemainingTime(float remainingSecond, float roundRemainingSecond, FieldStateType fieldStateType)
	{
		// 状態処理
		UpdateFieldState(remainingSecond, roundRemainingSecond, fieldStateType);

		if (this.oldStateType == fieldStateType)
		{
            // チート対策
            BattleFieldMasterData bfData;
            if (MasterData.TryGetBattleField((int)ScmParam.Battle.BattleFieldType, out bfData)) {
                if (bfData.BattleType != BattleType.Resident && bfData.BattleType != BattleType.Escort) {
                    //CheatCheck(remainingSecond, this.RemainingTimeServer);
                }
            }
			// タイマー更新
			this.RemainingTimeServer = remainingSecond;
		}
		else
		{
			// タイマー設定
			this.RemainingTimeServer = remainingSecond;
			this.RemainingTimeGap = 0;
			// 状態変更時処理
			ChangeFieldState(fieldStateType);
			// 状態更新
			this.oldStateType = fieldStateType;
		}
	}

	/// <summary>
	/// 毎回現在の状態の処理を行いたいものはここのメソッドで処理を行う
	/// </summary>
	private void UpdateFieldState(float remainingSecond, float roundRemainingSecond, FieldStateType fieldStateType)
	{
		// 戦略ゲージ更新
		GUITacticalGauge.RemainingTime(remainingSecond, roundRemainingSecond, fieldStateType);

        if (fieldStateType == FieldStateType.WaitingForChange) {
            GUINextRoundInfo.UpdateRemainingTime(remainingSecond);
        }

		// 戦闘中の時間更新処理
		if(fieldStateType != FieldStateType.Game) return;
		UpdateBattleTime(remainingSecond);
	}

	/// <summary>
	/// フィールド状態が切り替わる瞬間に処理を行いたいものはこのメソッドで処理を行う
	/// </summary>
	private void ChangeFieldState(FieldStateType fieldStateType)
	{
		switch(fieldStateType)
		{
			case FieldStateType.Free:
				ChangeFree();
				break;
			case FieldStateType.Waiting:
				ChangeWaiting();
				break;
			case FieldStateType.Game:
				ChangeGame();
				break;
			case FieldStateType.Extra:
			case FieldStateType.Infinite:
				ChangeExtra();
				break;
            case FieldStateType.WaitingForChange:
                ChangeWaitingForChange();
                break;
		}
	}

	#endregion

	#region チート対策
	
	/// <summary>
	/// チートのチェックを行う
	/// </summary>
	private void CheatCheck(float remainingSecond, float oldRemainingSecond)
	{
		this.RemainingTimeGap += remainingSecond - oldRemainingSecond;
		if(30f < this.RemainingTimeGap)
		{
			CommonPacket.SendGmCommand("iac "+this.RemainingTimeGap.ToString());
			this.RemainingTimeGap = 0;
            Debug.LogError("CheatCheck");
		}
		this.RemainingTimeGap = Mathf.Max(0, this.RemainingTimeGap);
	}
	
	#endregion
	
	#region 状態更新時処理
	
	/// <summary>
	/// 戦闘中の時間更新処理を行う
	/// </summary>
	private void UpdateBattleTime(float remainingSecond)
	{
		// 経過時間判定
		if(this.elapsedTimeMasterDataStack.Count != 0)
		{
			BattleFieldTimeEventMasterData masterData = this.elapsedTimeMasterDataStack.Peek();
			if(remainingSecond <= (this.TotalGameSecond - masterData.Time))
			{
				// 経過時間処理
				masterData = this.elapsedTimeMasterDataStack.Pop();
				BattleTimeMasterDataProc(masterData);
			}
		}

		// 残り時間判定
		if(this.remainingTimeMasterDataStack.Count != 0)
		{
			BattleFieldTimeEventMasterData masterData = this.remainingTimeMasterDataStack.Peek();
			if(remainingSecond <= masterData.Time)
			{
				// 残り時間処理
				masterData = this.remainingTimeMasterDataStack.Pop();
				BattleTimeMasterDataProc(masterData);
			}
		}
	}
	
	/// <summary>
	/// 時間マスターデータから時間に関わる処理を行う
	/// </summary>
	private void BattleTimeMasterDataProc(BattleFieldTimeEventMasterData masterData)
	{
		// 残り時間指定か経過時間指定か判定してメッセージをセットする
		if(masterData.IsRemaining)
		{
			// 残り時間
			GUIEffectMessage.SetTimeLimit(masterData);
		}
		else
		{
			// 経過時間
			GUIEffectMessage.SetTimeLater(masterData);
		}

		// BGM再生
		SoundController.BgmID playBgmID = (SoundController.BgmID)masterData.MusicId;
		if(playBgmID != SoundController.BgmID.None)
		{
			BattleMain.Sound.PlayLimitBgm(playBgmID);
		}
	}

	#endregion

	#region 状態切り替え時処理群

	/// <summary>
	/// Free状態
	/// </summary>
	private void ChangeFree()
	{
		// BGM再生
		if (BattleMain.Sound != null)
			BattleMain.Sound.PlayFreeBgm();
	}

	/// <summary>
	/// Waiting状態
	/// </summary>
	private void ChangeWaiting()
	{
        GUINextRoundInfo.Show(false);

        // BGM再生
        if (BattleMain.Sound != null)
			BattleMain.Sound.PlayFreeBgm();

		// 戦闘開始までのメッセージ表示
		if(ScmParam.Battle != null)
		{
			GUIEffectMessage.SetBattleWait(this.RemainingTimeServer, ScmParam.Battle.BattleFieldType);
		}

		// ブリーフィング画面表示

		// ブリーフィング中に再参戦すると既にリスポーンモードで画面が開いてる場合があるので
		// 既にリスポーンモードの時は上書きしないようにする
		if( GUIMapWindow.Mode != GUIMapWindow.MapMode.Respawn )
			GUIMapWindow.SetMode(GUIMapWindow.MapMode.Briefing);

        if (isRestarting) {

            // 弾丸リストのクリア.
            BattleMain.Instance.BulletMonitor.Clear();

            // 通信情報を元にUI系をセット.
            var serverValue = NetworkController.ServerValue;

            // チームタイプ設定
            BattleMain.Instance.SetTeamType(serverValue.TeamType);

            // 共通情報
            // 現在の残り時間を取得する
            BattlePacket.SendRemainingTime(serverValue.FieldId);

            // タワー戦情報
            // チームスキルポイントを取得する
            BattlePacket.SendTeamSkillPoint();

            // サイドゲージ情報
            // サイドゲージ系の情報を取得する
            BattlePacket.SendSideGauge();

            CommonPacket.SendEntrantAll(MapManager.Instance.FieldId);
        }
    }

	/// <summary>
	/// Game状態
	/// </summary>
	private void ChangeGame()
	{
		// 前回の状態が戦闘前開始準備の場合は
		// ゲーム開始タイミングなのでその処理をする
		if (this.oldStateType == FieldStateType.Waiting)
		{
            Debug.LogError("===> GameStart");
			GUIEffectMessage.SetGameStart();
			SoundController.PlayJingle(SoundController.JingleID.GameStart);
			Player p = GameController.GetPlayer();
			if(p != null)
			{
				p.ResetCoolTime();
			}

			// ブリーフィング画面閉じ判定
			{
				if( GUIMapWindow.Mode == GUIMapWindow.MapMode.Briefing )
				{
					GUIMapWindow.SetMode(GUIMapWindow.MapMode.Off);
				}
				// バトル切り替わり時にオプション画面を開いていた時は、戻る時の画面をバトルモードに切り替える
				//else if( GUIMapWindow.LastWindowMode == GUIMapWindow.MapMode.Briefing && GUIMapWindow.Mode == GUIMapWindow.MapMode.Off )
				//{
				//	GUIMapWindow.LastWindowMode = GUIMapWindow.MapMode.Battle;
				//}
			}
		}
		
		// BGM再生
		if (BattleMain.Sound != null)
			BattleMain.Sound.PlayNormalBgm();

		// 戦闘中の時間の設定
		SetupGameTimer(this.RemainingTimeServer);
	}

	/// <summary>
	/// Extra状態
	/// </summary>
	private void ChangeExtra()
	{
		// 前回の状態が戦闘前開始準備の場合は
		// ゲーム開始タイミングなのでその処理をする
		if (this.oldStateType == FieldStateType.Waiting)
		{
			GUIEffectMessage.SetGameStart();
			SoundController.PlayJingle(SoundController.JingleID.GameStart);
			Player p = GameController.GetPlayer();
			if(p != null)
			{
				p.ResetCoolTime();
			}
		}
		
		// BGM再生
		if (BattleMain.Sound != null)
			BattleMain.Sound.PlayNormalBgm();
	}

    private void ChangeWaitingForChange() {
        GUINextRoundInfo.Show(true);
        isRestarting = true;
    }

	#endregion

	#region 戦闘状態切り替え時の時間処理

	/// <summary>
	/// 戦闘開始時の時間の設定を行う
	/// </summary>
	private void SetupGameTimer(float remainingSecond)
	{
		// 登録されているマスターデータから経過時間の長い順に格納されているリストがあるので順に
		// 現在の経過時間と比べてすでに経過された時間かどうか判定し
		// 経過されていなければマスターデータをキューに格納していく
		BattleFieldTimeEventMasterData lastElapsedTimeData = null;
		float elapsedTime = this.TotalGameSecond - remainingSecond;
		List<BattleFieldTimeEventMasterData> elapsedTimeData = BattleFieldTimeEventMaster.Instance.GetElapsedTimeMasterData();
		foreach(BattleFieldTimeEventMasterData data in elapsedTimeData)
		{
			if(elapsedTime >= data.Time)
			{
				// すでに経過したデータに再生するMusicIdが設定されているかどうか判定する
				if(data.MusicId > 0)
				{
					lastElapsedTimeData = data;
					break;
				}
			}
			else
			{
				// 経過時間データ追加
				this.elapsedTimeMasterDataStack.Push(data);
			}
		}

		// 登録されているマスターデータから残り時間の短い順に格納されているリストがあるので順に
		// 現在の残り時間と比べてすでに経過された残り時間かどうか判定し
		// 経過されていなければマスターデータをキューに格納していく
		BattleFieldTimeEventMasterData lastRemainingData = null;
		List<BattleFieldTimeEventMasterData> remainingTimeData = BattleFieldTimeEventMaster.Instance.GetRemainingTimeMasterData();
		foreach(BattleFieldTimeEventMasterData data in remainingTimeData)
		{
			if(remainingSecond <= data.Time)
			{
				// すでに経過したデータに再生するMusicIdが設定されているかどうか判定する
				if(data.MusicId > 0)
				{
					lastRemainingData = data;
					break;
				}
			}
			else
			{
				// 残り時間データ追加
				this.remainingTimeMasterDataStack.Push(data);
			}
		}

		// ゲーム開始時のBGM再生
		StartPlayBGM(lastElapsedTimeData, lastRemainingData);
	}

	/// <summary>
	/// ゲーム開始時(再参戦の開始時も含めて)に時間マスターデータからBGMを再生するデータがないか検索し
	/// データが存在する場合はBGMを再生させる
	/// </summary>
	/// <param name="lastElapsedTimeData">
	/// 最後に経過した経過指定のマスターデータ
	/// </param>
	/// <param name="lastRemainingData">
	/// 最後に経過した残り指定のマスターデータ
	/// </param>
	private void StartPlayBGM(BattleFieldTimeEventMasterData lastElapsedTimeData, BattleFieldTimeEventMasterData lastRemainingData)
	{
		// すでに経過したマスターデータに再生するMusicIdが設定されていないので何もミュージックを再生させない
		if(lastElapsedTimeData == null && lastRemainingData == null)
			return;
		
		// 最後に経過した時間のデータの方のミュージックを再生させる
		if(lastElapsedTimeData == null)
		{
			// 最後に経過したデータが残り時間指定のマスターデータ
			BattleMain.Sound.PlayLimitBgm((SoundController.BgmID)lastRemainingData.MusicId);
			return;
		}
		if(lastRemainingData == null)
		{
			// 最後に経過したデータが経過時間指定のマスターデータ
			BattleMain.Sound.PlayLimitBgm((SoundController.BgmID)lastElapsedTimeData.MusicId);
			return;
		}

		// 経過指定と残り指定のタイミングのデータを比べて最後に経過した方のBGMを再生させる
		if(lastElapsedTimeData.Time > (this.TotalGameSecond - lastRemainingData.Time))
		{
			// 最後に経過したデータが経過時間指定のマスターデータ
			BattleMain.Sound.PlayLimitBgm((SoundController.BgmID)lastElapsedTimeData.MusicId);
			return;
		}
		else
		{
			// 最後に経過したデータが残り時間指定のマスターデータ
			BattleMain.Sound.PlayLimitBgm((SoundController.BgmID)lastRemainingData.MusicId);
			return;
		}
	}

	#endregion
}
