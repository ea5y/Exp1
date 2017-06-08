//author: luwanzhong
//date: 2016-11-18
//desc: userInfo model

using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;
using System.Collections.Generic;
using Scm.Common.Packet;

namespace XUI.UserInfo{
    
    public class Model{
#region BasicData
        //base data struct
        public BaseDataStruct baseData;
        public struct BaseDataStruct
        {
            public bool update;
            
            public int charaId;
            public int lv;
            public int gameTimes;
            public int winTimes;
            public int killTimes;
            public int mvpTimes;
            public int rank;
            public int rankMax;
            public int currentScore;
            public int needScore;
            public int skinId;
        }
#endregion
        
#region CombatGains
        public CombatGainsStruct combatGains;
        public struct CombatGainsStruct
        {
            public bool updateRecords;
            public BattleHistoryRecord[] records;
            public bool updateDetailItems;
            public BattleHistoryDetailItem[] detailItems;
            
            public bool updatePlayerMatchInfo;
            public PlayerMatchInfoStruct playerMatchInfo;
        }

        public struct PlayerMatchInfoStruct
        {
            public PlayerMiscInfoRes rankInfo;
            public PlayerMatchParameter quickMatchInfo;
            public PlayerMatchParameter rankMatchInfo;
        }
#endregion

#region CharaStatistics
        public CharaStatisticsStruct charaStatistics;
        public struct CharaStatisticsStruct
        {
            public bool updateMyMatchInfo;
            public MyMatchInfoStruct myMatchInfo;
        }

        public struct MyMatchInfoStruct
        {
            public PlayerMatchParameter quickMatchInfo;
            public PlayerMatchParameter rankMatchInfo;
        }
#endregion
        
    }
}
