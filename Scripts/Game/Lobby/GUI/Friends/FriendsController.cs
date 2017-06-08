using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.Master;

namespace XUI.Friends
{
    public class FriendsController : Singleton<FriendsController>
    {
        #region View
        [SerializeField]
        FriendsView friendsView;
        public FriendsView View { get { return this.friendsView; } }

        [System.Serializable]
        public class FriendsView
        {
            public GameObject root;
			public UISprite frameBg;
            [SerializeField]
            TabMenu tabMenu;
            public TabMenu TabMenu { get { return this.tabMenu; } }

            [SerializeField]
            Friends friends;
            public Friends Friends { get { return this.friends; } }

            [SerializeField]
            FriendsApply apply;
            public FriendsApply Apply { get { return this.apply; } }

            [SerializeField]
            FriendsSearch search;
            public FriendsSearch Search { get { return this.search; } }
        }
        
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

           // public UIWidget Search
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
        #endregion

        #region Controller
        //Property
        CustomControl.TabPagesManager pages;
        CustomControl.TabPagesManager Pages { get { return this.pages; } set { this.pages = value; } }

        List<UIButton> tabMenuList;
        List<UIButton> TabMenuList { get { return this.tabMenuList; } set { this.tabMenuList = value; } }

        CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }
		long playerIdSearched = 0;
		long PlayerIdSearched{ get{ return this.playerIdSearched; } set { this.playerIdSearched = value; } }

		float uiScale = 0;
		float UIScale { get { return this.uiScale; } set { this.uiScale = value; } }
        enum Page
        {
            Friends,
            Apply,
            Search
        }

        void Awake()
        {
            base.Awake();
            this.Init();
            this.RegistViewEvent();

			this.UIAdapte();
            this.View.root.SetActive(false);
        }

		private void UIAdapte()
		{
//			Debug.Log ("ScreenW:" + Screen.width);
//			Debug.Log ("ScreenH:" + Screen.height);
//			this.UIScale = (this.View.Friends.grid.transform.parent.GetComponent<UIPanel> ().width / 1060);
//			Debug.Log ("scale:" + this.UIScale);
		}

        void OnEnable()
        {
			this.UIAdapte ();
            this.SwitchTo(Page.Friends);
        }

        void Init()
        {
            this.TabMenuList = new List<UIButton>();
            this.TabMenuList.Add(this.View.TabMenu.btnFriends);
            this.TabMenuList.Add(this.View.TabMenu.btnApply);
            this.TabMenuList.Add(this.View.TabMenu.btnSearch);

            Pages = new CustomControl.TabPagesManager();
            Pages.AddPage(this.View.Friends);
            Pages.AddPage(this.View.Apply);
            Pages.AddPage(this.View.Search);
            this.SwitchTo(Page.Friends);

            this.SetUsername(NetworkController.ServerValue.PlayerInfo.UserName);
        }

        private void SetUsername(string p)
        {
            this.View.Search.Search.myUsername.text = p;
        }

        void SwitchTo(Page page)
        {
            switch (page)
            {
                case Page.Friends:
                    this.Pages.SwitchTo(this.View.Friends);
                    CustomControl.ToolFunc.BtnSwitchTo(this.View.TabMenu.btnFriends, this.TabMenuList);
                    break;
                case Page.Apply:
                    this.Pages.SwitchTo(this.View.Apply);
                    CustomControl.ToolFunc.BtnSwitchTo(this.View.TabMenu.btnApply, this.TabMenuList);
                    break;
                case Page.Search:
                    this.Pages.SwitchTo(this.View.Search);
                    CustomControl.ToolFunc.BtnSwitchTo(this.View.TabMenu.btnSearch, this.TabMenuList);
                    break;
            }
        }

        public void Show()
        {
            Net.Network.Instance.StartCoroutine(this.Open());
        }

        public IEnumerator Open()
        {
            yield return this.GetFriendsList();
            TopBottom.Instance.OnIn = () =>
            {
                //gameObject.SetActive(true);
                PanelManager.Instance.Open(this.View.root);
            };
            TopBottom.Instance.OnBack = (v) =>
            {
                //gameObject.SetActive(false);
                PanelManager.Instance.Close(this.View.root);
                v();
            };
            TopBottom.Instance.In("好友");
//            PanelManager.Instance.Open(this.View.root, false, false);
            yield return this.GetApplyList();
            
        }

        public IEnumerator GetApplyList()
        {
            yield return Net.Network.GetApplyList(this.SetApplyList, false);
        }

        public IEnumerator GetFriendsList()
        {
            yield return Net.Network.GetFriendsList(this.SetFriendsList);
        }
        #region Friends
        void SetFriendsList(FriendRes response)
        {
            if (response == null)
                return;

            var infoList = response.GetFriendListParameters();

            //this.View.Friends.grid.gameObject.DestroyChild();
            this.View.Friends.grid.gameObject.DestroyChildImmediate();

            foreach (var info in infoList)
            {
                var go = NGUITools.AddChild(this.View.Friends.grid.gameObject, this.View.Friends.itemFriend);
                ItemFriend item = go.GetComponent<ItemFriend>();
                
                //Set Icon
                this.CharaIcon.GetIcon((AvatarType)info.CharacterId, info.SkinId, false,
                        (UIAtlas res, string name) =>
                        {
                            item.icon.atlas = res;
                            item.icon.spriteName = name;
                        });

                //Set name
                item.username.text = info.Name;

                //Set lv
                //TODO
                item.lv.text = PlayerLevelMaster.Instance.GetLevelByExp(info.StarID) + "";

                //Set winTimes
                item.winTimes.text = info.WinCount + "";

                //Set rank
                item.rank.text = PlayerRankMaster.Instance.GetRankByScore(info.Score) + "";

                //Set rank icon
                //@TODO

                item.InviteLabel.text = "邀请";
                item.btnLeft.gameObject.SetActive(info.LastLogoutSeconds == 0);
                item.btnRight.gameObject.SetActive(info.LastLogoutSeconds == 0);
                var friendId = info.FriendId;
                var friendName = info.Name;
                //Set btn team
                EventDelegate.Add(item.btnLeft.onClick, () => { this.OnBtnTeamClick(friendId); item.CountDown(); });

                //Set btn chat
                EventDelegate.Add(item.btnRight.onClick, () => { this.OnBtnChatClick(friendId, friendName); });
            }

            this.View.Friends.grid.repositionNow = true;
            this.View.Friends.grid.Reposition();
        }

        private void OnBtnChatClick(long friendId, string name)
        {
            //GUIChat.Instance.OnInputSelect();
            //GUIChat.Instance.OnChatTypeWis(friendId, name);
            GUIChatFrameController.Instance.Show();
            GUIChatFrameController.Instance.OnChatTypeWis(friendId, name);
            Debug.Log("===>Chat click");
        }

        private void OnBtnTeamClick(long friendId)
        {
            //TODO
            Debug.Log("===>Team click");
            CommonPacket.SendTeamInvite(new[] { friendId });
        }
        #endregion

        #region Apply
        void SetApplyList(FriendRes response)
        {
            if (response == null)
                return;
            var infoList = response.GetRequestListParameters();

            //this.View.Apply.grid.gameObject.DestroyChild();
            this.View.Apply.grid.gameObject.DestroyChildImmediate();

            foreach (var info in infoList)
            {
                var go = NGUITools.AddChild(this.View.Apply.grid.gameObject, this.View.Apply.itemFriendApply);
                ItemFriend item = go.GetComponent<ItemFriend>();

				UISprite sp = go.GetComponent<UISprite> ();
				//sp.width = (int)(sp.width * (this.View.Friends.grid.transform.parent.GetComponent<UIPanel>().width / 1060));
                //Set Icon
                this.CharaIcon.GetIcon((AvatarType)info.CharacterId, info.SkinId, false,
                        (UIAtlas res, string name) =>
                        {
                            item.icon.atlas = res;
                            item.icon.spriteName = name;
                        });

                //Set name
                item.username.text = info.Name;

                //Set lv
                //TODO
                item.lv.text = PlayerLevelMaster.Instance.GetLevelByExp(info.StarID) + "";

                //Set winTimes
                item.winTimes.text = info.WinCount + "";

                //Set rank
                item.rank.text = PlayerRankMaster.Instance.GetRankByScore(info.Score) + "";

                //Set rank icon
                //@TODO

                var id = info.FriendId;
                
                //Set btn accept
                EventDelegate.Add(item.btnLeft.onClick,
                    () =>
                    {
                        this.OnBtnAcceptClick(id, go);
                    });

                //Set btn reject
                EventDelegate.Add(item.btnRight.onClick,
                    () =>
                    {
                        this.OnBtnRejectClick(id, go);
                    });
            }
            this.View.Apply.grid.repositionNow = true;
            this.View.Apply.grid.Reposition();
        }

        private void OnBtnRejectClick(long id, GameObject go)
        {
            StartCoroutine(this.Reject(id));
            Destroy(go);
            this.View.Apply.grid.repositionNow = true;
            this.View.Apply.grid.Reposition();
        }

        private IEnumerator Reject(long id)
        {
            yield return Net.Network.RejectFriendRequest(id, null);
        }

        private void OnBtnAcceptClick(long id, GameObject item)
        {
            StartCoroutine(this.Accept(id));
            Destroy(item);
            this.View.Apply.grid.repositionNow = true;
            this.View.Apply.grid.Reposition();
        }

        private IEnumerator Accept(long id)
        {
            yield return Net.Network.AcceptFriendRequest(id, this.OnNetAccept);
        }

        private void OnNetAccept(FriendRes obj)
        {
            if (obj == null)
                return;
        }
        #endregion
        

        

        public void Close()
        {
            PanelManager.Instance.Close(this.View.root);
        }

        #region View Event
        void RegistViewEvent()
        {
            //TabMenu
            EventDelegate.Add(this.View.TabMenu.btnFriends.onClick, this.OnBtnFriendsClick);
            EventDelegate.Add(this.View.TabMenu.btnApply.onClick, this.OnBtnApplyClick);
            EventDelegate.Add(this.View.TabMenu.btnSearch.onClick, this.OnBtnSearchClick);
            //Friends
            //Apply
            //Search
            EventDelegate.Add(this.View.Search.Search.btnSearch.onClick, this.OnSearchPageBtnSearchClick);
			EventDelegate.Add (this.View.Search.Result.btnOk.onClick, this.OnBtnAddClick);
			EventDelegate.Add (this.View.Search.Result.btnNo.onClick, this.OnBtnCancelClick);
        }

		private void OnBtnAddClick()
		{
			StartCoroutine (Net.Network.AddFriend (this.PlayerIdSearched, (res) => {
				GUIMessageWindow.SetModeOK("请求已成功！", ()=>this.Goto(this.View.Search.Search.root, this.View.Search.Result.root)); 
			}, false));
		}

		private void OnBtnCancelClick()
		{
			this.Goto (this.View.Search.Search.root, this.View.Search.Result.root);
		}

        //Search
        private void OnSearchPageBtnSearchClick()
        {
            SoundController.PlaySe(SoundController.SeID.Select);
            if(!string.IsNullOrEmpty(this.View.Search.Search.input.value))
				StartCoroutine(this.SearchFriend());
        }
         
        private IEnumerator SearchFriend()
        {
            //TODO
			yield return Net.Network.SearchFriend(this.View.Search.Search.input.value.Trim(), this.SetResult, true);


        }

		private void Goto(GameObject n, GameObject o)
		{
			o.SetActive (false);
			n.SetActive (true);
		}	

		private void SetResult(SearchPlayerRes res)
		{
			Debug.Log ("Result!");
			var info = res.GetParameters ();
			//Set Icon
			this.CharaIcon.GetIcon((AvatarType)info[0].CharacterId, info[0].SkinId, false,
				(UIAtlas atlas, string name) =>
				{
					this.View.Search.Result.icon.atlas = atlas;
					this.View.Search.Result.icon.spriteName = name;
				});
			//Set name
			this.View.Search.Result.username.text = info [0].Name;
			//Set lv
            //TODO
			this.View.Search.Result.lv.text = PlayerLevelMaster.Instance.GetLevelByExp(info[0].StarID) + "";
			//Set winTimes
			this.View.Search.Result.winTimes.text = info[0].WinCount + "";
			//Set rank
			this.View.Search.Result.rank.text = PlayerRankMaster.Instance.GetRankByScore(info[0].Score) + "";
			//Set rank icon
			//@TODO

			//Set PlayerIdSearched
			this.playerIdSearched = info[0].FriendId;

			this.Goto (this.View.Search.Result.root, this.View.Search.Search.root);
		}

        //TabMenu
        private void OnBtnSearchClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            this.SwitchTo(Page.Search);
        }

        private void OnBtnApplyClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            this.SwitchTo(Page.Apply);
        }

        private void OnBtnFriendsClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            this.SwitchTo(Page.Friends);
        }
        #endregion
        #endregion

    }

}
