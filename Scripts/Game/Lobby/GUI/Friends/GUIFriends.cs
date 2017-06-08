using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Packet;
using Scm.Common.Master;
using Temp;
using System;

namespace XUI
{
    public class GUIFriends : PanelBase<GUIFriends>
    {
        public FriendsView view;

        private CustomControl.TabPagesManager pages;
        private List<UIButton> tabMenuList;
        private enum PageType
        {
            Friends,
            Apply,
            Search
        }

        private CustomControl.ScrollView<ItemFriend> scrollFriends;
        private CustomControl.ScrollView<ItemApply> scrollApply;

        private CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

        long playerIdSearched = 0;
        

        void Awake()
        {
            base.Awake();
            this.CreateTabMenuList();
            this.CreatePages();
            this.RegisterEventOnce();
            this.SetUsername(NetworkController.ServerValue.PlayerInfo.UserName);
            this.HideFirst();
        }

        private void SetUsername(string p)
        {
            this.view.Search.Search.myUsername.text = p;
        }

        private void HideFirst()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.SwitchTo(PageType.Friends);
        }

        private void CreateTabMenuList()
        {
            if(this.tabMenuList == null)
                this.tabMenuList = new List<UIButton>();
            this.tabMenuList.Add(this.view.tabMenu.btnFriends);
            this.tabMenuList.Add(this.view.tabMenu.btnApply);
            this.tabMenuList.Add(this.view.tabMenu.btnSearch);
        }

        private void CreatePages()
        {
            if(this.pages == null)
                this.pages = new CustomControl.TabPagesManager();
            this.pages.AddPage(this.view.friends);
            this.pages.AddPage(this.view.Apply);
            this.pages.AddPage(this.view.Search);
        }

        private void SwitchTo(PageType type)
        {
            switch(type)
            {
                case PageType.Friends:
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabMenu.btnFriends, this.tabMenuList);
                    this.pages.SwitchTo(this.view.friends);
                    break;
                case PageType.Apply:
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabMenu.btnApply, this.tabMenuList);
                    this.pages.SwitchTo(this.view.Apply);
                    break;
                case PageType.Search:
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabMenu.btnSearch, this.tabMenuList);
                    this.pages.SwitchTo(this.view.Search);
                    break;
            }
        }

        public void Open()
        {
            Net.Network.Instance.StartCoroutine(this._Open());
        }

        private void OnDisable()
        {
            this.DeleteEvent();
        }

        private IEnumerator _Open()
        {
            this.RegisterEvent();
            yield return XDATA.PlayerData.Instance.GetFriendsList();
            yield return XDATA.PlayerData.Instance.GetApplyList();

            TopBottom.Instance.OnIn = () => {
                PanelManager.Instance.Open(this.view.root);
            };
            TopBottom.Instance.OnBack = (v) => {
                PanelManager.Instance.Close(this.view.root);
                v();
            };
            TopBottom.Instance.In("好友");
        }

        private void RegisterEvent()
        {
            XDATA.PlayerData.Instance.OnFriendsListChange += this.SetFriends;
            XDATA.PlayerData.Instance.OnApplyListChange += this.SetApply;
        }

        private void DeleteEvent()
        {
            XDATA.PlayerData.Instance.OnFriendsListChange -= this.SetFriends;
            XDATA.PlayerData.Instance.OnApplyListChange -= this.SetApply;
        }

        private void SetFriends(object sender, EventArgs e)
        {
            var infoList = ((XDATA.PlayerData)sender).friendsList;

            if(this.scrollFriends == null)
                this.scrollFriends = new CustomControl.ScrollView<ItemFriend>(this.view.friends.grid, this.view.friends.itemFriend);

            infoList.Sort((a, b) => {
                var r = a.LastLogoutSeconds.CompareTo(b.LastLogoutSeconds);
                return r;
            });
            this.scrollFriends.CreateWeight(infoList);
        }

        private void SetApply(object sender, EventArgs e)
        {
            var infoList = ((XDATA.PlayerData)sender).applyList;

            if(this.scrollApply == null)
                this.scrollApply = new CustomControl.ScrollView<ItemApply>(this.view.Apply.grid, this.view.Apply.itemFriendApply);

            //sort

            this.scrollApply.CreateWeight(infoList);
        }

        #region Event
        private void RegisterEventOnce()
        {
            EventDelegate.Add(this.view.tabMenu.btnFriends.onClick, this.OnBtnFriendsClick);
            EventDelegate.Add(this.view.tabMenu.btnApply.onClick, this.OnBtnApplyClick);
            EventDelegate.Add(this.view.tabMenu.btnSearch.onClick, this.OnBtnSearchClick);

            EventDelegate.Add(this.view.Search.Search.btnSearch.onClick, this.OnSearchPageBtnSearchClick); 
            EventDelegate.Add(this.view.Search.Result.btnOk.onClick, this.OnBtnAddClick);
            EventDelegate.Add(this.view.Search.Result.btnNo.onClick, this.OnBtnCancleClick);
        }

        private void OnBtnFriendsClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            this.SwitchTo(PageType.Friends);
        }

        private void OnBtnApplyClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            this.SwitchTo(PageType.Apply);
        }

        private void OnBtnSearchClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            this.SwitchTo(PageType.Search);
        }

        private void OnSearchPageBtnSearchClick()
        {
            SoundController.PlaySe(SoundController.SeID.WindowClose);
            if(!string.IsNullOrEmpty(this.view.Search.Search.input.value))
                StartCoroutine(this.SearchFriend());
        }

        private IEnumerator SearchFriend()
        {
            yield return Net.Network.SearchFriend(this.view.Search.Search.input.value.Trim(), this.SetResult, true);
        }

        private void SetResult(SearchPlayerRes res)
        {
 			Debug.Log ("Result!");
			var info = res.GetParameters ();
			//Set Icon
			this.CharaIcon.GetIcon((AvatarType)info[0].CharacterId, info[0].SkinId, false,
				(UIAtlas atlas, string name) =>
				{
					this.view.Search.Result.icon.atlas = atlas;
					this.view.Search.Result.icon.spriteName = name;
				});
			//Set name
			this.view.Search.Result.username.text = info [0].Name;
			//Set lv
            //TODO
			this.view.Search.Result.lv.text = PlayerLevelMaster.Instance.GetLevelByExp(info[0].StarID) + "";
			//Set winTimes
			this.view.Search.Result.winTimes.text = info[0].WinCount + "";
			//Set rank
			this.view.Search.Result.rank.text = PlayerRankMaster.Instance.GetRankByScore(info[0].Score) + "";
            //Set rank icon
            this.view.Search.Result.iconRank.spriteName = this.view.Search.Result.rank.text;

			//Set PlayerIdSearched
			this.playerIdSearched = info[0].FriendId;

			this.Goto (this.view.Search.Result.root, this.view.Search.Search.root);
	           
        }

        private void Goto(GameObject n, GameObject o)
        {
            o.SetActive(false);
            n.SetActive(true);
        }

        private void OnBtnAddClick()
        {
			StartCoroutine (Net.Network.AddFriend (this.playerIdSearched, (res) => {
				GUIMessageWindow.SetModeOK("请求已成功！", ()=>this.Goto(this.view.Search.Search.root, this.view.Search.Result.root)); 
			}, false));
        }

        private void OnBtnCancleClick()
        {
            this.Goto(this.view.Search.Search.root, this.view.Search.Result.root);
        }
        #endregion
    }
}

