using UnityEngine;
using System.Collections;

namespace XUI
{
    [System.Serializable]
    public class ResultView : MonoBehaviour
    {
        public GameObject root;

        public ResultAll allView;
        public ResultDetail detailView;

        [System.Serializable]
        public class ResultAll
        {
            //onegame gameObject
            public GameObject root;

            //label scoreType
            public UILabel scoreType;

            //label gameTime
            public UILabel gameTime;

            //label gameMap
            public UILabel gameMap;

            //label result left
            public UISprite resultLeft;

            //label result right
            public UISprite resultRight;

            //group left
            public GameObject groupLeft;

            //group right
            public GameObject groupRight;

            //item info
            public GameObject itemInfo;

            public UILabel btnNextName;
        }

        [System.Serializable]
        public class ResultDetail
        {
            public GameObject root;

            public Left left;
            public ResultReward rewardView;
            public ResultRank rankView;            
        }

        [System.Serializable]
        public class Left
        {
            public GameObject portrait;
            public UILabel lblName;
        }

        [System.Serializable]
        public class ResultReward
        {
            public GameObject root;

            public GameObject itemChara;
            public GameObject itemReward;            
            public UIButton btnNext;
        }

        [System.Serializable]
        public class ResultRank
        {
            public GameObject root;

            public UISprite fillScore;
            public UILabel totalScore;
            public UILabel addScore;

            public UISprite rankIcon;
        }
    }

}
