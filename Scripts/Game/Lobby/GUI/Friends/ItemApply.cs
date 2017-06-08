using UnityEngine;
using System.Collections;
using Scm.Common.Packet;
using Scm.Common.Master;

public class ItemApply : CustomControl.ScrollViewItem{
    public UISprite icon;
    public UILabel username;
    public UILabel lv;
    public UILabel winTimes;
    public UILabel rank;
    public UILabel InviteLabel;
    public UIAtlas rankAtlas;
    public UISprite rankIcon;

    public UIButton btnLeft;
    public UIButton btnRight;

    private FriendRequestParameter data;
    public void CountDown()
    {
        StartCoroutine(StartCountDown());
    }

    private bool IsCountDown = false;
    IEnumerator StartCountDown()
    {
        if (IsCountDown)
        {
            yield break;
        }
        IsCountDown = true;
        InviteLabel.text = "已邀请";
        float cd = 19f;
        float t = Time.time + cd;
        while (t > Time.time)
        {
            yield return new WaitForSeconds(0.05f);
        }
        InviteLabel.text = "邀请";
        IsCountDown = false;
        yield return 0;
    }

    public override void FillItem(IList datas, int index)
    {
        base.FillItem(datas, index);

        this.data = (FriendRequestParameter)datas[index];

        ScmParam.Lobby.CharaIcon.GetIcon((AvatarType)this.data.CharacterId, this.data.SkinId, false,
                (UIAtlas res, string name) => 
                {
                    this.icon.atlas = res;
                    this.icon.spriteName = name;
                });

        this.username.text = this.data.Name;

        Debug.Log("status: " + this.data.Status);
        if(this.data.Status > 0)
            this.lv.text = "离线";
        else
            this.lv.text = "在线";

        this.winTimes.text = this.data.WinCount + "";

        this.rank.text = PlayerRankMaster.Instance.GetRankByScore(this.data.Score) + "";

        //Set rank icon
        this.rankIcon.atlas = this.rankAtlas;
        this.rankIcon.spriteName = this.rank.text;
        
        var id = this.data.FriendId;
        this.btnLeft.onClick.Clear();
        this.btnRight.onClick.Clear();
        EventDelegate.Add(this.btnLeft.onClick, () => {
                this.OnBtnAcceptClick(id);
                });
        EventDelegate.Add(this.btnRight.onClick, () => {
                this.OnBtnRejectClick(id);
                });
    }

    private void OnDisable()
    {
        if (transform.parent == null)
            return;
        var grid = transform.parent.GetComponent<UIGrid>();
        if (grid == null)
            return;
        grid.repositionNow = true;
        grid.Reposition();
    }

    private void OnBtnAcceptClick(long id)
    {
       StartCoroutine(this._Accept(id));
    }

    private IEnumerator _Accept(long id)
    {
        yield return Net.Network.AcceptFriendRequest(id, (res) => {
            Net.Network.Instance.StartCoroutine(XDATA.PlayerData.Instance.GetFriendsList());
        });

        Destroy(this.gameObject);
    }

    private void OnBtnRejectClick(long id)
    {
        StartCoroutine(this._Reject(id));
    }

    private IEnumerator _Reject(long id)
    {
        yield return Net.Network.RejectFriendRequest(id, null);
        Destroy(this.gameObject);
    }
}
