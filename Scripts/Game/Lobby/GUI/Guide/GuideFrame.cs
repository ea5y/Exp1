#define GuideTrue
using System;
using UnityEngine;
using Scm.Common.Master;

public class GuideFrame : MonoBehaviour
{
    public static GuideFrame Instance;
    public GameObject ButtonRoot;
    public GameObject Tip;
    public GameObject Hand;
    public GameObject Yes;
    public GameObject No;
    public UILabel Name;
    public UILabel TipContent;
    public Transform CharaBoardRoot;
    private GuideType Status = GuideType.None;
    private GameObject CharaBoard;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SetDetail(GuideType pValue)
    {
#if GuideTrue
        Status = pValue;
        gameObject.SetActive(true);
        Hand.SetActive(false);
        GuideCoroutine.Instance.GuidSwitch[pValue]();
#endif
    }

    //设置详细但是不执行后续函数
    public void SetDetailWithoutExe(GuideType pValue)
    {
        Status = pValue;
        gameObject.SetActive(true);
    }

    public void SetNext()
    {
        GuideMasterData t = null;
        MasterData.TryGetGuideMasterData((int)Status, out t);
        if (null == t)
        {
            Debug.LogError("===> Wrong");
        }
        GuideMasterData next = null;
        MasterData.TryGetGuideMasterData((int)t.Next, out next);
        if (null == next)
        {
            Debug.LogError("===> Wrong");
        }
        SetDetail((GuideType)next.Type);
    }

    public void SetTip(string pName, string pText)
    {
        Tip.SetActive(true);
        Name.text = pName;
        TipContent.text = pText;
    }

    public void CloseTip()
    {
        Tip.SetActive(false);
        if (null != CharaBoard)
        {
            CharaBoard.SetActive(false);
        }
    }

    public void SetCharaBoard(string pChara = "ui_cb_e001_001")
    {
        ScmParam.Battle.CharaBoard.GetGuideBoard(pChara, false, (v) =>
        {
            if (null != CharaBoard)
            {
                Destroy(CharaBoard);
            }
            CharaBoard = NGUITools.AddChild(CharaBoardRoot.gameObject, v);
            CharaBoard.transform.SetParent(CharaBoardRoot);
            CharaBoard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            if (pChara == "ui_cb_e001_002")
            {
                CharaBoard.transform.localPosition = new Vector3(-470f, 0, 0);
            }
            else
            {
                CharaBoard.transform.localPosition = new Vector3(480f, 0, 0);
            }
        });
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public Action ActionClickYes = null; 
    public void OnClickYes()
    {
        if (null != ActionClickYes)
        {
            ActionClickYes();
        }
    }

    public Action ActionClickNo = null; 
    public void OnClickNo()
    {
        if (null != ActionClickNo)
        {
            ActionClickNo();
        }
    }
    #region Net
    public void GetGuideInfo()
    {
        CommonPacket.SendGetGuide((receive) =>
        {
            GuideMasterData t = null;
            MasterData.TryGetGuideMasterData(receive.step, out t);
            if (null != t)
            {
                SetDetail((GuideType)t.Type);
            }
            else
            {
                Debug.LogError("===> Can Not Find GuideData " + receive.step);
            }
        });
    }

    public void SendGuideInfo(Action pCallBack, bool pFinish = false)
    {
        if (pFinish)
        {
            CommonPacket.SendSetGuide((int) GuideType.StartGame, ((receive) =>
            {
                pCallBack();
            }));
        }
        else
        {
            CommonPacket.SendSetGuide((int) Status, ((receive) =>
            {
                pCallBack();
            }));
        }
    }
    #endregion
}

public enum GuideType
{
    None = 0,
    SelectOldNew,
    SelectLeader,
    Shop,
    StartGame,
}
