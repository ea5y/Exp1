/// <summary>
/// 戦闘開始までの待ち時に表示するUI
/// 例外として戦闘開始メッセージが表示されるまでこのオブジェクトを消さないようにしている
/// 
/// 2014/08/25
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using Scm.Common.Master;

public class GUIBattleWaitMessageItem : GUIEffectMessageItem
{
	#region アタッチオブジェクト

	[System.Serializable]
	public class AttachWaitObject
	{
		/// <summary>
		/// 勝利条件メッセージ
		/// </summary>
		[SerializeField]
		private UILabel conditionMessageLabel;
		public UILabel ConditionMessageLabel { get { return conditionMessageLabel; } }

		/// <summary>
		/// 削除用再生エフェクト
		/// </summary>
		[SerializeField]
		private UIPlayTween playDeleteEffect;
		public UIPlayTween PlayDeleteEffect { get { return playDeleteEffect; } }

		/// <summary>
		/// 戦闘開始カウント
		/// </summary>
		[SerializeField]
		private UILabel startCountLabel;
		public UILabel StartCountLabel { get { return startCountLabel; } }

	    /// <summary>
	    /// 戦闘開始カウント
	    /// </summary>
	    [SerializeField]
	    private UILabel mapNameLabel;
        public UILabel MapNameLabel { get { return mapNameLabel; } }
	}

	#endregion

	#region フィールド&プロパティ

	/// <summary>
	/// アタッチオブジェクト
	/// </summary>
	[SerializeField]
	private AttachWaitObject attachWaitObj;
	public AttachWaitObject AttachWaiObj { get{ return attachWaitObj; } }

	/// <summary>
	/// 削除エフェクトの時間
	/// </summary>
	[SerializeField]
	private float deleteEffectTime = 1.0f;

	/// <summary>
	/// 勝利条件メッセージの削除時間
	/// 戦闘開始 - 削除エフェクトの時間
	/// </summary>
	private float deleteMsgTime;

	/// <summary>
	/// エフェクト処理用
	/// </summary>
	private Action effectProc = ()=>{};
	
	#endregion

	#region セットアップ

	/// <summary>
	/// 待ち時のメッセージ関係をセットする
	/// </summary>
	public void Setup(float startTime, BattleFieldType fieldType)
	{
		// 戦闘開始までの時間
		base.time = startTime;

		// 勝利条件メッセージの削除時間を求める
		this.deleteMsgTime = Mathf.Max(startTime - this.deleteEffectTime, 0);

		// 勝利条件メッセージ関係のセットアップ
		ConditionSetup(fieldType);

		// 戦闘開始までのカウント関係のセットアップ
		StartCountSetup(startTime);
	}

	/// <summary>
	/// 勝利条件メッセージ関係のセットアップ
	/// </summary>
	private void ConditionSetup(BattleFieldType fieldType)
	{
		// 削除エフェクト処理をセット
		this.effectProc = PlayDeleteEffect;

		// マスターデータからメッセージを取得しセットする
		if(this.AttachWaiObj.ConditionMessageLabel != null)
		{
			string message = string.Empty;
			BattleFieldMasterData battleFieldData;
			if(MasterData.TryGetBattleField((int)fieldType, out battleFieldData))
			{
				if(battleFieldData.BattleRule != null)
				{
                    message = BattleMain.GetRuleMessage(BattleMain.GetPlayerTeamType(), battleFieldData.BattleRule);
                }
				else
				{
					BugReportController.SaveLogFile(
							string.Format("NotFound BattleRuleMasterData BattleFieldType = {0} BattleRuleId",
						              fieldType, battleFieldData.BattleType)
						);
					return;
				}
			}
			else
			{
				BugReportController.SaveLogFile(string.Format("NotFound BattleFieldType = {0}", fieldType));
				return;
			}
			this.AttachWaiObj.ConditionMessageLabel.text = message;
		    if (null != this.AttachWaiObj.MapNameLabel)
		    {
		        BattleFieldMapMasterData mapData;
		        BattleFieldMapMaster.Instance.TryGetMasterData(battleFieldData.MapID, out mapData);
                if(null != mapData)
		        this.AttachWaiObj.MapNameLabel.text = mapData.Name;
		    }
		}
	}

	/// <summary>
	/// 戦闘開始までのカウント関係のセットアップ
	/// </summary>
	private void StartCountSetup(float startTime)
	{
		// 戦闘開始カウント
		if(this.AttachWaiObj.StartCountLabel != null)
		{
			startTime = Mathf.Max(0f, startTime);
			int startCount = (int)Mathf.Ceil(startTime);
			this.AttachWaiObj.StartCountLabel.text = startCount.ToString();
		}
	}

	#endregion

	#region 更新

	/// <summary>
	/// 更新処理
	/// </summary>
	protected override void Update ()
	{
		// 時間更新
		TimeUpdate();

		// エフェクト処理
		this.effectProc();
	}

	/// <summary>
	/// 時間更新
	/// </summary>
	private void TimeUpdate()
	{
		base.time -= Time.deltaTime;
		base.time = Mathf.Max(0f, base.time);
		int remainingTime = Mathf.FloorToInt(base.time);

		// 戦闘開始カウント更新
		if(this.AttachWaiObj.StartCountLabel != null)
		{
			this.AttachWaiObj.StartCountLabel.text = remainingTime.ToString();
		}
	}

	#endregion

	#region エフェクト処理

	/// <summary>
	/// 削除エフェクト処理
	/// </summary>
	private void PlayDeleteEffect()
	{
		// 削除エフェクトを再生する時間かどうか
		if(base.time <= this.deleteMsgTime)
		{
			if(this.AttachWaiObj.PlayDeleteEffect != null)
			{
				this.AttachWaiObj.PlayDeleteEffect.Play(true);
			}

			// エフェクト処理終了
			this.effectProc = ()=>{};
		}
	}

	#endregion
}
