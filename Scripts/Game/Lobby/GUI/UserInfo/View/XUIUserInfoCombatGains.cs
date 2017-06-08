using UnityEngine;
using System.Collections;

namespace XUI.UserInfo{
    //serialize the CombatGains class
    [System.Serializable]
    public class CombatGains : CustomControl.IPage {
#region Root
        //root
        [SerializeField]
        GameObject _root;
        public GameObject Root{ get { return _root; } }

        public void Init()
        {
            PanelManager.Instance.Close(this.OneGame.root);
            PanelManager.Instance.Close(this.Player.root);
        }
#endregion

#region CombatGains Base
        //serialize the object of base
        [SerializeField]
        CombatGainsBase _base;
        public CombatGainsBase Base{ get { return _base; } }
        //serialize the class of Base
        [System.Serializable]
        public class CombatGainsBase {
            //base gameObject
            public GameObject root;
            
            //scrollview
            public UIGrid grid;

            public GameObject arrowsUp;
            public GameObject arrowsDown;

            //item
            [SerializeField]
            ItemCombatGainsBase _itemBase;
            public ItemCombatGainsBase ItemBase { get { return _itemBase; } }
        }

        
        //serialize the prefab of scroll view
        [System.Serializable]
        public struct ItemCombatGainsBase
        {
            //item gameObject
            public GameObject root;
            
        };

        

#endregion

#region CombatGains one game
        //serialize the object of onegame
        [SerializeField]
        CombatGainsOneGame _oneGame;    
        public CombatGainsOneGame OneGame{ get { return _oneGame; } }

        //serialize the class of onegame
        [System.Serializable]
        public class CombatGainsOneGame {
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

            //btn close
            public UIButton btnClose;
        }
#endregion

#region CombatGains player
        //serialize the object of player
        [SerializeField]
        CombatGainsPlayer _player;
        public CombatGainsPlayer Player { get { return _player; } }

        //serialize the class of player
        [System.Serializable]
        public struct CombatGainsPlayer
        {
            //root
            public GameObject root;

            //portrait
            public GameObject portrait;

            //info
            public UILabel lv;
            public UILabel gameTimes;
            public UILabel winTimes;
            public UILabel killTimes;
            public UILabel mvpTimes;

            //rank
            public UILabel rank;
            public UISprite rankField;
            public UILabel maxRank;

            //btn
            public XUIButton btnAddFriends;
            public XUIButton btnMessage;
            public XUIButton btnInvite;
            public XUIButton btnClose;

            //detail
            [SerializeField]
            CombatGainsPlayerDetail _detail;
            public CombatGainsPlayerDetail Detail { get { return _detail; } }
        };

        //detail
        [System.Serializable]
        public struct CombatGainsPlayerDetail
        {
            //root
            public GameObject root;

            public UILabel currentScore;
            public UILabel needScore;
        };
#endregion
    }
}
