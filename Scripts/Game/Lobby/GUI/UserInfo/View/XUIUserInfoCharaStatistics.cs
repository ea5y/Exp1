using UnityEngine;
using System.Collections;

namespace XUI.UserInfo{
    //serialize the CharaStatistics class
    [System.Serializable]
    public class CharaStatistics : CustomControl.IPage {
        //root
        [SerializeField]
        GameObject _root;
        public GameObject Root { get { return _root; } }

        public void Init()
        {
            //None
        }

        //btn detail
        [SerializeField]
        XUIButton _btnDetail;
        public XUIButton BtnDetail{ get { return _btnDetail; } }

        //btn slider
        [SerializeField]
        XUIButton _btnSlider;
        public XUIButton BtnSlider{ get { return _btnSlider; } }

        //btn home
        [SerializeField]
        XUIButton _btnHome;
        public XUIButton BtnHome{ get { return _btnHome; } }

#region Group Detail
        //serialize the object of detail
        [SerializeField]
        CharaStatisticsDetail _detail;
        public CharaStatisticsDetail Detail{ get { return _detail; } }

        //serialize the class of detail
        [System.Serializable]
        public class CharaStatisticsDetail : CustomControl.IPage {
            //group detail gameObject
            public GameObject root;
            public GameObject Root { get { return this.root; } }

            public void Init()
            {

            }

            //detail left
            [SerializeField]
            DetailLeft _left;
            public DetailLeft Left{ get { return _left; } }
            [System.Serializable]
            public struct DetailLeft{
                public UILabel gameType;
                public UILabel gameTimes;
                public UILabel winTimes;
                public UILabel killTimes;
                public UILabel maxKillTimesOneGame;
                public UILabel damageSum;
                public UILabel maxDamageOneGame;
                public UILabel gameTime;
                public UILabel mvpTimes;
            };
            
            //detail right
            [SerializeField]
            DetailRight _right;
            public DetailRight Right{ get { return _right; } }
            [System.Serializable]
            public struct DetailRight{
                public UILabel gameType;
                public UILabel gameTimes;
                public UILabel winTimes;
                public UILabel killTimes;
                public UILabel maxKillTimesOneGame;
                public UILabel damageSum;
                public UILabel maxDamageOneGame;
                public UILabel gameTime;
                public UILabel mvpTimes;
            };
        }
#endregion

#region Group Slider
        //serialize the object of slider
        [SerializeField]
        CharaStatisticsSlider _slider;
        public CharaStatisticsSlider Slider{ get { return _slider; } }
        
        //serialize the class of slider
        [System.Serializable]
        public class CharaStatisticsSlider : CustomControl.IPage {
            //slider gameObject
            public GameObject root;
            public GameObject Root { get { return this.root; } }

            public void Init()
            {

            }

            //Reword
            
            //Lv
            public UILabel lv;

            //lv filled
            public GameObject lvFilled;

            //detail
            [SerializeField]
            SliderDetail _detail;
            public SliderDetail Detail{ get { return _detail; } }

            //serialize the struct of detail
            [System.Serializable]
            public struct SliderDetail{
                public GameObject root;
                public UILabel currentExp; 
                public UILabel needExp;
            };
        }
#endregion
    }
}
