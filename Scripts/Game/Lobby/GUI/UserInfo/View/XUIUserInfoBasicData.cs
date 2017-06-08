using UnityEngine;
using System.Collections;

namespace XUI.UserInfo{

    [System.Serializable]
    public class BasicData : CustomControl.IPage{
        
#region Root
        //root
        [SerializeField]
        GameObject _root;
        public GameObject Root{ get { return _root; } }

        public void Init()
        {

        }
#endregion

#region Portrait
        //portrait        
        [SerializeField]
        GameObject _portrait;
        public GameObject Portrait{ get { return _portrait; } }

        public TweenPosition portraitTween;
#endregion
        
#region Info
        //label lv
        [SerializeField]
        UILabel _lv;
        public UILabel Lv{ get { return _lv; } }

        //label gameTimes
        [SerializeField]
        UILabel _gameTimes; 
        public UILabel GameTimes{ get { return _gameTimes; } }

        //label winTimes
        [SerializeField]
        UILabel _winTimes;
        public UILabel WinTimes{ get { return _winTimes; } }

        //label killTimes
        [SerializeField]
        UILabel _killTimes;
        public UILabel KillTimes{ get { return _killTimes; } }

        //label mvpTimes
        [SerializeField]
        UILabel _mvpTimes;
        public UILabel MvpTime{ get { return _mvpTimes; } }

        public UILabel userName;
#endregion
        
#region Rank
        //rank filled
        [SerializeField]
        UISprite _rankFilled;
        public UISprite RankFilled{ get { return _rankFilled; } }

        //icon
        [SerializeField]
        UISprite _rankIcon;
        public UISprite RankIcon{ get { return _rankIcon; } }

        //label rank
        [SerializeField]
        UILabel _rankLabel;
        public UILabel RankLabel{ get { return _rankLabel; } }

        //use serializeField
#region Detail
        //group detail
        [SerializeField]
        DetailAttach _detail;
        public DetailAttach Detail{ get { return _detail; } }
        [System.Serializable]
        public class DetailAttach{
            public GameObject root; 
            public UILabel currentScore;
            public UILabel needScore;
        }
#endregion
        
        //label max rank
        [SerializeField]
        UILabel _maxRank;
        public UILabel MaxRank{ get { return _maxRank; } }
#endregion
    }
}
