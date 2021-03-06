/// <summary>
/// バトルメインからリザルトメインへの情報を橋渡しする
/// 
/// 2013/06/14
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;
using Scm.Common.Packet;

public class BridgingResultInfo
{
	#region フィールド＆プロパティ
	public int InFieldPlayerID { get; private set; }
	public JudgeTypeClient JudgeTypeClient { get; private set; }

	public List<MemberInfo> MemberList { get; private set; }
	public List<RewardBattleResultParameter> RewardList { get; private set; }
	public bool IsPlayerInfo { get; private set; }
    #endregion

    #region セットアップ

    //LWZ:Add gamesecond, fieldId
    private long gameSecond = 0;
    public long GameSecond { get { return this.gameSecond; } set { this.gameSecond = value; } }

    private int fieldId = 0;
    public int FieldId { get { return this.fieldId; } set { this.fieldId = value; } }
    public ScoreType scoreType = ScoreType.QuickMatching;
    public GameResultCharacterParameter[] myCharas;

    public void Setup(long gameSecond, int fieldId, ScoreType type, GameResultCharacterParameter[] myCharas)
    {
        this.GameSecond = gameSecond;
        this.FieldId = fieldId;
        this.scoreType = type;
        this.myCharas = myCharas;
    }

	public void Setup(int inFieldPlayerID, JudgeType judgeType, GameResultPacketParameter[] gameResultPackets, RewardBattleResultParameter[] rewards)
	{
		this.IsPlayerInfo = true;
		this.InFieldPlayerID = inFieldPlayerID;
		this.JudgeTypeClient = judgeType.GetClientJudge();
       
		this.MemberList = new List<MemberInfo>();
		PlayerInfo playerInfo = NetworkController.ServerValue.PlayerInfo;
		foreach (var elem in gameResultPackets)
		{
			AvatarInfo entrant;
			Entrant.TryGetEntrant<AvatarInfo>(elem.InFieldId, out entrant);
			if (entrant == null)
			{
				// プレイヤー情報が取得出来なかった場合はServerValueから取得する
				// (主に再参戦時に1度も出撃しないで戦闘を終えた場合にこの処理が行われる)
				if(playerInfo != null)
				{
					if(playerInfo.InFieldId == elem.InFieldId)
					{
						entrant = playerInfo;
					}
					else
					{
						// 取得したいデータが他プレイヤーの場合は現在は取得する手段がない
						continue;
					}
				}
				else
				{
					this.IsPlayerInfo = false;
					continue;
				}
			}

			MemberInfo info = new MemberInfo();
			info.inFieldID = entrant.InFieldId;
			info.avatarType = (AvatarType)entrant.Id;
            info.skinId = entrant.SkinId;
			info.name = entrant.UserName;
			info.teamType = entrant.TeamType.GetClientTeam();
			info.rank = elem.Rank;
			info.battleRank = elem.BattleRank;
			info.score = elem.Score;
			info.baseScore = elem.SideGaugeScore;
			info.etcScore = 0;
			info.kill = elem.Kill;
			info.killScore = elem.VsPlayerScore;
			info.death = elem.Death;
			info.subTowerDefeatCount = elem.DefeatObjectCount;
			info.subTowerDefeatScore = elem.VsObjectScore;
			info.mainTowerDefeatCount = 0;
			info.mainTowerDefeatScore = 0;
			info.guardNpcDefeatCount = 0;
			info.guardNpcDefeatScore = 0;
			info.judge = judgeType;
			info.avatarInfo = entrant;
			info.winBonus = elem.WinBonus;
			info.startPlayerGradeID = elem.StartPlayerGradeId;
			info.endPlayerGradeID = elem.EndPlayerGradeId;
			info.startGradePoint = elem.StartGradePoint;
			info.endGradePoint = elem.EndGradePoint;
			info.playerGradeState = elem.PlayerGradeState;

            info.gainExp = elem.GainExp;
            info.gainEnergy = elem.GainEnergy;
            info.gainCoin = elem.GainCoin;
            info.gainGold = elem.GainGold;
            info.gainScore = elem.GainScore;
                        
            foreach(var c in elem.RewardCharacterList)
            {
                Debug.Log("Reward chara num: " + elem.RewardCharacterList);
                info.rewardChara = c;
            }
			MemberList.Add(info);
		}

		this.RewardList = new List<RewardBattleResultParameter>(rewards);
	}
	#endregion
}
