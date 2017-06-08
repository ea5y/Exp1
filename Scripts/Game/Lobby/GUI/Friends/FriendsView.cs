using UnityEngine;
using System.Collections;

namespace Temp
{
    [System.Serializable]
    public class FriendsView : MonoBehaviour {
        public GameObject root;
        public UISprite frameBg;

        public TabMenu tabMenu;
        public Friends friends;
        public FriendsApply Apply;
        public FriendsSearch Search;

        [System.Serializable]
        public class TabMenu
        {
            public UIButton btnFriends;
            public UIButton btnApply;
            public UIButton btnSearch;
        }

        [System.Serializable]
        public class Friends : CustomControl.IPage
        {
            public GameObject root;
            public UIGrid grid;
            public GameObject itemFriend;

            public GameObject Root
            {
                get { return this.root; }
            }

            public void Init()
            {
                //None
            }
        }

        [System.Serializable]
        public class FriendsApply : CustomControl.IPage
        {
            public GameObject root;
            public UIGrid grid;
            public GameObject itemFriendApply;

            public GameObject Root
            {
                get { return this.root; }
            }

            public void Init()
            {
                //None
            }
        }

        [System.Serializable]
        public class FriendsSearch : CustomControl.IPage
        {
            public GameObject root;

            [SerializeField]
            SearchW search;
            public SearchW Search { get { return this.search; } }

            [SerializeField]
            ResultW result;
            public ResultW Result { get { return this.result; } }

            public GameObject Root
            {
                get { return this.root; }
            }

            public void Init()
            {
                //None
            }
        }

        [System.Serializable]
        public class SearchW
        {
            public GameObject root;
            public UILabel myUsername;
            public UIInput input;
            public UIButton btnSearch;
        }

        [System.Serializable]
        public class ResultW
        {
            public GameObject root;
            public UISprite icon;
            public UILabel username;
            public UILabel lv;
            public UILabel winTimes;
            public UILabel rank;
            public UISprite iconRank;

            public UIButton btnOk;
            public UIButton btnNo;
        }
    }
}

