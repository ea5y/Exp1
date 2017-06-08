using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using Scm.Common.Packet;

public class RankBoardGridItem : GridItemBase
{
    public UILabel Name;
    public UILabel Level;
    public UILabel Win;
    public UILabel Rank;
    public UISprite Icon;
    public UILabel Order;
    public UISprite OrderIcon;

    private CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private RankingItemParameter Status;
    public void SetDetail(RankingItemParameter pValue)
    {
        Status = pValue;
        gameObject.SetActive(true);
        Name.text = pValue.Name;
        Level.text = CharaStarMaster.Instance.GetLevelByID(pValue.StarId).ToString();
        Win.text = pValue.WinCount.ToString();
        Rank.text = pValue.Sequence.ToString();
        Order.text = pValue.Sequence.ToString();
        OrderIcon.spriteName = "paiminglan";

        CharaIcon.GetIcon((AvatarType)pValue.CharacterId, pValue.SkinId, false, (a, s) =>
        {
            Icon.atlas = a;
            Icon.spriteName = s;
        });

        if (NetworkController.ServerValue.PlayerId == pValue.PlayerId)
        {
            Name.text = "[FF6600]" + Name.text + "[-]";
            OrderIcon.spriteName = "paiminghong";
        }
    }

    protected override void OnReposition(int i)
    {
        base.OnReposition(i);
        //        int j = i / 5;
        int j = i;
        StartCoroutine(WaitTween(j++));
    }
    protected override IEnumerator WaitTween(int i)
    {
        //        yield return new WaitForSeconds(i * 0.5f);
        yield return new WaitForSeconds(i * 0.05f);
        Transform target = transform;
        target.localScale = LastLocalScale;
        Vector3 pos = target.localPosition;
        TweenPosition t = target.GetComponent<TweenPosition>();
        if (null == t)
        {
            t = target.gameObject.AddComponent<TweenPosition>();
        }
        t.enabled = false;
        t.ResetToBeginning();
        t.duration = 0.2f;
        t.from = new Vector3(pos.x, -800, pos.z);
        t.to = pos;
        t.PlayForward();
    }

    public void ClickAddFriend()
    {
        StartCoroutine(Net.Network.AddFriend(Status.PlayerId, (res) =>
        {
            Debug.Log("===> Add");
        }, false));
    }

    public void ClickChat()
    {

    }
}
