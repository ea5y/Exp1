using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XDATA;

public class DeckEdit : MonoBehaviour
{
    public DeckEditAttach Root;
    public UIPlayTween mPlayTween;
    public UICenterOnChild mUICenterOnChild;
    public UIAtlas CommonAtlas;
    public GameObject effect;

    public static DeckEdit Instance;
    private DeckEditAttach Right;
    private DeckEditAttach Left;
    private List<DeckEditAttach> ScrollItems;
    private List<DeckEditAttach> SlotItems;
    private List<DeckEditAttach> RightItems = new List<DeckEditAttach>();

    public DeckInfo DeckInfo = null;
    private List<ulong> OldDeckIds = new List<ulong>();
    private LobbyPacket.PlayerCharacterAllResArgs RightInfo = null;
    private ulong LeaderUUID = 0;
    private DeckEditAttach LastSelectSlotItem = null;
    private DeckEditAttach LastSelectRightItem = null;

    private CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }
    private SkillIcon SkillIcon { get { return ScmParam.Battle.SkillIcon; } }

    void OnActive()
    {
        mPlayTween.Play(true);
    }
    void OnDeactive()
    {
        mPlayTween.Play(false);
    }

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        Right = Root.mRoot.Right;
        Left = Root.mRoot.Left;
        ScrollItems = Right.mRight.ScrollItems;
        SlotItems = Left.mLeft.SlotItems;
        //        ScrollItems.ForEach(v =>
        //        {
        //            v.mScrollItem.Items.ForEach(i => RightItems.Add(i));
        //        });
        gameObject.SetActive(false);

//        ScrollItems[0].mScrollItem.Items[0].mItem.Level1.spriteName = "Xworld_chara_font_02_0";
//        ScrollItems[0].mScrollItem.Items[0].mItem.Level2.spriteName = "Xworld_chara_font_02_0";
        //	    Root["Right"]["ScrollItem"][0]["Item"][0].
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void SetDetail(DeckInfo info)
    {
        if (null == Instance)
        {
            Debug.LogError("Not Init");
            return;
        }
        Instance._SetDetail(info);

        {
            TopBottom.Instance.OnIn = () =>
            {
                //没有tween的话会关掉，所以有了我再打开下面的代码（playtween 代码是这样写的，估计是谁改的吧）
                //                Instance.OnActive();
                //Instance.gameObject.SetActive(true);
                PanelManager.Instance.Open(Instance.gameObject);
            };
            TopBottom.Instance.OnBack = (v) =>
            {
                //                Instance.OnDeactive();
                //Instance.gameObject.SetActive(false);
                PanelManager.Instance.Close(Instance.gameObject);
                Instance.LastSelectSlotItem = null;
                Instance.LastSelectRightItem = null;
                v();
            };
            TopBottom.Instance.In("阵容");
        }
    }

    public void _SetDetail(DeckInfo info)
    {
        DeckInfo = info;
        OldDeckIds = GetSlotIDs();
        for (int i = 0, max = DeckInfo.CharaInfoList.Count; i < max; ++i)
        {
            var t = DeckInfo.CharaInfoList[i];
            t.DeckSlotIndex = i;
        }
        LeaderUUID = DeckInfo.CharaInfoList[0].UUID;
        RequestDeckCharasAll();
    }

    private void LeftSetup()
    {
        var charaList = DeckInfo.CharaInfoList;

        SlotItems.ForEachI((v, i) =>
        {
            if (i < charaList.Count)
            {
                v.mSlotItem.CharaInfo = charaList[i];
                CharaIcon.GetIcon(charaList[i].AvatarType, charaList[i].SkinId, false, (a, s) =>
                {
                    v.mSlotItem.Icon.atlas = a;
                    v.mSlotItem.Icon.spriteName = s;
                    v.mSlotItem.ToogleSprite.gameObject.SetActive(false);

                    if (null == LastSelectSlotItem)
                    {
                        SelectSlotItem(SlotItems[0], true);
                    }
                    else if (v.mSlotItem.CharaInfo.UUID == LastSelectSlotItem.mSlotItem.CharaInfo.UUID)
                    {
                        SelectSlotItem(v, false);
                    }
                });

                //Lv
                v.mSlotItem.lv.text = charaList[i].Level + "";
            }
            else
            {
                v.mSlotItem.Level1.spriteName = "";
                v.mSlotItem.Level2.spriteName = "";
            }
        });
    }

    private void RightSetup(LobbyPacket.PlayerCharacterAllResArgs pRes)
    {
        RightInfo = pRes;
        var ids = GetSlotIDs();
        var charaList = pRes.List;

        charaList.Sort((a, b) =>
        {
            int t = b.Level.CompareTo(a.Level);
            if (t == 0)
            {
                return b.UUID.CompareTo(a.UUID);
            }
            return t;
        });

        //Make RightItems
        int L = Mathf.CeilToInt(charaList.Count / 4.0f);
        int N = L - ScrollItems.Count;
        for (int i = 0; i < N; ++i)
        {
            ScrollItems.Add(NGUITools.AddChild(Right.mRight.Grid.gameObject, ScrollItems[0].gameObject).GetComponent<DeckEditAttach>());
        }
        Right.mRight.Grid.Reposition();
        RightItems.Clear();
        ScrollItems.ForEach(v =>
        {
            v.mScrollItem.Items.ForEach(i => RightItems.Add(i));
        });

        RightItems.ForEachI((v, i) =>
        {
            if (i < charaList.Count)
            {
                v.mItem.CharaInfo = charaList[i];
                CharaIcon.GetIcon(charaList[i].AvatarType, charaList[i].SkinId, false, (a, s) =>
                {
                    v.mItem.Icon.atlas = a;
                    v.mItem.Icon.spriteName = s;
                    v.mItem.ToogleSprite.enabled = false;
                    v.mItem.HighLightSprite.gameObject.SetActive(false);

                    if (ids.Contains(v.mItem.CharaInfo.UUID))
                    {
                        v.mItem.ToogleSprite.enabled = true;
                    }

                    if (null != LastSelectSlotItem &&
                        v.mItem.CharaInfo.UUID == LastSelectSlotItem.mSlotItem.CharaInfo.UUID)
                    {
                        SelectRightItem(v);
                    }

                    v.gameObject.SetActive(true);
                });

                //lv
                v.mItem.lv.text = charaList[i].Level + "";
                v.mItem.WillExpire = charaList[i].TotalTime > 0;
            }
            else
            {
                v.gameObject.SetActive(false);
                v.mItem.Icon.atlas = CommonAtlas;
                v.mItem.Icon.spriteName = "wenhao";
                v.mItem.ToogleSprite.enabled = false;
                v.mItem.HighLightSprite.gameObject.SetActive(false); ;
            }
        });
    }

    #region Button Event
    public void Back()
    {
        TopBottom.Instance.Back();
    }

    public void SelectSlotItem(DeckEditAttach pItem, bool pIsClick = false)
    {
        //        if(pItem == LastSelectSlotItem)
        //        {
        //            return;
        //        }
        if (null != LastSelectSlotItem)
        {
            LastSelectSlotItem.mSlotItem.ToogleSprite.gameObject.SetActive(false);
        }
        pItem.mSlotItem.ToogleSprite.gameObject.SetActive(true);
        LastSelectSlotItem = pItem;

        if (pIsClick)
        {
            RightSetup(RightInfo);
        }

        //        for (int i = 0; i < ScrollItems.Count; ++i)
        //        {
        //            for (int j = 0; j < ScrollItems[i].mScrollItem.Items.Count; ++j)
        //            {
        //                if (ScrollItems[i].mScrollItem.Items[j].mItem.CharaInfo.UUID == pItem.mSlotItem.CharaInfo.UUID)
        //                {
        //                    mUICenterOnChild.CenterOn(ScrollItems[i].transform);
        //                    return;
        //                }
        //            }
        //        }
    }

    public void SelectRightItem(DeckEditAttach pItem, bool pIsClick = false)
    {
        //        if (pItem == LastSelectRightItem)
        //        {
        //            return;
        //        }
        if (pItem.mItem.CharaInfo.UUID < 1)
        {
            return;
        }
        if (null != LastSelectRightItem)
        {
            LastSelectRightItem.mItem.HighLightSprite.gameObject.SetActive(false);
        }
        pItem.mItem.HighLightSprite.gameObject.SetActive(true);
        effect.SetActive(false);
        effect.transform.parent = pItem.mItem.HighLightSprite.transform;
        effect.transform.localPosition = Vector3.zero;
        StartCoroutine(DelayShow());
        LastSelectRightItem = pItem;
        if (!pIsClick)
        {
            return;
        }
        int Index = LastSelectSlotItem.mSlotItem.CharaInfo.DeckSlotIndex;
        SlotItems.ForEach(v =>
        {
            if (pItem.mItem.CharaInfo.UUID == v.mSlotItem.CharaInfo.UUID)
            {
                LastSelectSlotItem.mSlotItem.CharaInfo.DeckSlotIndex = v.mSlotItem.CharaInfo.DeckSlotIndex;
                DeckInfo.CharaInfoList[v.mSlotItem.CharaInfo.DeckSlotIndex] = LastSelectSlotItem.mSlotItem.CharaInfo;
            }
        });
        pItem.mItem.CharaInfo.DeckSlotIndex = Index;
        //Be careful of refrence
        DeckInfo.CharaInfoList[Index] = pItem.mItem.CharaInfo;
        LeftSetup();
    }

    IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(0.1f);
        effect.SetActive(true);
    }

    public void Submit()
    {
        var ids = GetSlotIDs();
        bool changed = false;
        ids.ForEachI((v, i) =>
        {
            if (v != OldDeckIds[i])
            {
                changed = true;
            }
        });
        if (changed)
        {
            var deckInfo = DeckInfo;
            if (deckInfo != null && deckInfo.CharaInfoList != null)
            {
                int max = deckInfo.CharaInfoList.Count;
                ulong[] charaIDs = new ulong[max];
                for (int i = 0; i < max; i++)
                {
                    var info = deckInfo.CharaInfoList[i];
                    if (info.IsDeckSlotEmpty)
                        charaIDs[i] = 0;	// スロットが空なら0
                    else
                        charaIDs[i] = info.UUID;
                }
                CommonPacket.SendSetCharacterDeck(deckInfo.DeckID, deckInfo.DeckName, charaIDs);
            }
        }

        if (LeaderUUID != SlotItems[0].mSlotItem.CharaInfo.UUID)
        {
            CommonPacket.SendSetSymbolPlayerCharacterReq(SlotItems[0].mSlotItem.CharaInfo.UUID, v =>
            {
                Back();
            });
        }
        else
        {
            Back();
        }
    }

    public List<ulong> GetSlotIDs()
    {
        List<ulong> ids = new List<ulong>();
        DeckInfo.CharaInfoList.ForEach(v =>
        {
            ids.Add(v.UUID);
        });
        return ids;
    }
    #endregion

    #region Net
    //LWZ:change to Open()
    /* static void RequestDeckInfo()
    {
        CommonPacket.SendCharacterDeckNum();
    }*/


    public static void RequestDeckCharasAll()
    {
        LobbyPacket.SendPlayerCharacterAll((v) =>
        {
            DeckEdit.Instance.RightSetup(v);
            //After RightSetUp Do LeftSetUp, Cause i want selcet right when select left
            DeckEdit.Instance.LeftSetup();
        });
    }

    #endregion

    //LWZ:add sync req
    public void Open()
    {
        Net.Network.Instance.StartCoroutine(this._Open());
    }

    private IEnumerator _Open()
    {
        this.RegisterEvent();
        yield return PlayerData.Instance.GetDeckInfo();

        SetDetail(this.DeckInfo);
    }

    private void RegisterEvent()
    {
        PlayerData.Instance.OnDeckInfoChange += this.GetDeckInfo;
    }

    private void GetDeckInfo(object sender, EventArgs e)
    {
        this.DeckInfo = ((PlayerData)sender).deckInfo;
    }

    private void DeleteEvent()
    {
        PlayerData.Instance.OnDeckInfoChange -= this.GetDeckInfo;
    }
    
    private void OnDisable()
    {
        this.DeleteEvent();
    }
}

public static class ListExtend
{
    public static void ForEachI<T>(this List<T> pList, Action<T, int> pAction)
    {
        for (int i = 0; i < pList.Count; ++i)
        {
            pAction(pList[i], i);
        }
    }
}
