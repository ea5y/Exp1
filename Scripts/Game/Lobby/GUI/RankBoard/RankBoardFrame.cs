using UnityEngine;
using Scm.Common.GameParameter;
using Scm.Common.Packet;
using System.Collections.Generic;

public class RankBoardFrame : MonoBehaviour
{
    public static RankBoardFrame Instance;
    public List<RankBoardGridItem> mItems;
    public UIGrid mGrid;
    void Awake()
    {
        gameObject.SetActive(false);
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDetail()
    {
        gameObject.SetActive(true);
        DisallItem();
        LobbyPacket.SendRanking(0, RankingPeriodType.Daily);
    }

    private RankingItemParameter[] Status;
    public void SetDetail(RankingItemParameter[] pValue)
    {
        Status = pValue;
        DisallItem();
        for (int i = 0; i < Status.Length; ++i)
        {
            if (i < mItems.Count)
            {
                mItems[i].SetDetail(Status[i]);
            }
            else
            {
                var item = NGUITools.AddChild(mGrid.gameObject, mItems[0].gameObject).GetComponent<RankBoardGridItem>();
                item.SetDetail(Status[i]);
                mItems.Add(item);
            }
        }
        mGrid.Reposition();
    }

    private void DisallItem()
    {
        mItems.ForEach(v =>
        {
            v.gameObject.SetActive(false);
        });
    }

    public void ClickClose()
    {
        gameObject.SetActive(false);
    }
}
