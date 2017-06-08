using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using System.Collections.Generic;
using System;
using XUI.LobbyResident;
using Scm.Common.Master;

public class GUIRankMatch : Singleton<GUIRankMatch> {
    #region View
    [SerializeField]
    Rank view;
    public Rank View { get { return this.view; } }
    [System.Serializable]
    public class Rank
    {
        public GameObject root;
        public UILabel score;
        public UISprite filledRank;
        public UISprite iconRank;

        public UILabel rank;
        public UILabel maxRank;

        public UIGrid playerGroup;
        public GameObject itemPlayer;

        public UIButton btnStart;
    }
    #endregion

    CharaIcon CharaIcon { get { return ScmParam.Lobby.CharaIcon; } }

    bool isMatching = false;
    bool IsMatching {
        get { return this.isMatching; }
        set
        {
            this.isMatching = value;
            this.HideBtnStart(isMatching);
        }
    }

   
    string myInfo = "";
    string MyInfo { get { return this.myInfo; } set { this.myInfo = value; } }

    //int myCharacterId = 0;
    //int MyCharacterId { get { return this.myCharacterId; } set { this.myCharacterId = value; } }

    //int mySkinId

    void Awake()
    {
        base.Awake();
        this.RegistEvent();
        this.View.root.SetActive(false);
    }

    void RegistEvent()
    {
        EventDelegate.Add(this.View.btnStart.onClick, this.OnStartClick);
    }

    private void OnStartClick()
    {
        GUILobbyResident.Instance.Controller.RankMatchStart();
        this.IsMatching = true;
    }

    void HideBtnStart(bool isHide)
    {
        this.View.btnStart.gameObject.SetActive(!isHide);
    }

    public void Open()
    {
        Net.Network.Instance.StartCoroutine(this.Init());
    }

    IEnumerator Init()
    {
        //Set cancel callback
        GUIMatchingState.Instance.SetCancelCallback(() =>
        {
            this.IsMatching = false;
        });
        //Get info
        yield return Net.Network.GetPlayerStatusInfo(0, this.SetRank);
//        PanelManager.Instance.Open(this.View.root);

        TopBottom.Instance.OnIn = () =>
        {
            //transform.gameObject.SetActive(true);
            PanelManager.Instance.Open(this.View.root);
        };
        TopBottom.Instance.OnBack = (v) =>
        {
            //transform.gameObject.SetActive(false);
            PanelManager.Instance.Close(this.View.root);
            v();
        };
        TopBottom.Instance.In("排位赛");

        this.SetPlayerGroup(TeamMatchSlot.MemberParameters);
    }

    public void Close()
    {
        PanelManager.Instance.Close(this.View.root);
    }

    void SetRank(PlayerStatusRes res)
    {
        var info = res.GetPlayerStatusParam();
        
        
        var rank = PlayerRankMaster.Instance.GetRankByScore(info.RankExp);
        //if (info.RankExp == this.GetNeedScore(rank))
        //    rank++;
        this.SetCurScore(info.RankExp, rank);

        this.View.score.text = rank + "";
        this.View.iconRank.spriteName = rank + "";
        this.View.rank.text = rank + "";
        this.View.maxRank.text = info.MaxRank + "";
    }
    
    private void SetCurScore(long curScore, int rank)
    {
        var overScore = this.GetOverScore(curScore, rank);
        var needScore = this.GetNeedScore(rank);
        this.View.filledRank.fillAmount = (float)overScore / needScore * (float)0.755;
    }

    private float GetOverScore(long curScore, int rank)
    {
        float overScore = 0;
        if (rank == 1)
            overScore = curScore;
        else
            overScore = curScore - PlayerRankMaster.Instance.GetNextScore(--rank);
        return overScore;
    }

    private float GetNeedScore(int rank)
    {
        float result = 0;
        if (rank == 1)
            result = PlayerRankMaster.Instance.GetNextScore(rank);
        else
            result = PlayerRankMaster.Instance.GetNextScore(rank) - PlayerRankMaster.Instance.GetNextScore(--rank);
        return result;
    }

    void SetPlayerGroup(List<GroupMemberParameter> pMemberParameters)
    {
        //this.View.playerGroup.gameObject.DestroyChild();
        this.View.playerGroup.gameObject.DestroyChildImmediate();
        if (pMemberParameters.Count == 0)
        {
            var p = GameController.GetPlayer();
            
            this.AddPlayer(p.UserName, (int)p.AvatarType, p.SkinId);
        }
        else
        {
            pMemberParameters.ForEachI((v, i) => {
                this.AddPlayer(v.PlayerName, v.CharacterId, v.SkinId);
            });
            
            this.View.playerGroup.repositionNow = true;
            this.View.playerGroup.Reposition();
        }
    }

    void AddPlayer(string playerName, int charaId, int skinId)
    {
        GameObject go = NGUITools.AddChild(this.View.playerGroup.gameObject, this.View.itemPlayer);
        go.SetActive(true);
        var item = go.GetComponent<ItemRankPlayer>();

        item.username.text = playerName;
        /*CharaIcon.GetIcon(AvatarType, skinId, false,
                    (UIAtlas res, string name) =>
                    {
                        item.icon.atlas = res;
                        item.icon.spriteName = name;
                    });*/

        AvatarMasterData adata = null;
        MasterData.TryGetAvatar(charaId, skinId, out adata);
        CharaIcon.GetBustIcon((AvatarType)adata.CharacterId, adata.ID, false, (a, s) =>
        {
            item.icon.atlas = a;
            item.icon.spriteName = s;
        });

        item.SetFrame(adata.ShopItemTag);
    }

    public void ClickRankBoard()
    {
        RankBoardFrame.Instance.SetDetail();
    }
}
