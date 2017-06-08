using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.Master;

public class GuideCoroutine : MonoBehaviour
{
    public Dictionary<GuideType, Action> GuidSwitch = new Dictionary<GuideType, Action>();
    public static GuideCoroutine Instance;
    void Awake()
    {
        Instance = this;
        //初始化switch
        GuidSwitch.Add(GuideType.None, () => { });
        GuidSwitch.Add(GuideType.SelectOldNew, () => { StartCoroutine(SelectOldNew()); });
        GuidSwitch.Add(GuideType.SelectLeader, () => { StartCoroutine(SelectLeader()); });
        GuidSwitch.Add(GuideType.Shop, () => { StartCoroutine(Shop()); });
        GuidSwitch.Add(GuideType.StartGame, () => { StartCoroutine(StartGame()); });
    }

    #region UI

    private IEnumerator WaitNotNull(string pName)
    {
        var go = GameObject.Find(pName);
        while (null == go)
        {
            yield return new WaitForSeconds(0.1f);
            go = GameObject.Find(pName);
        }
    }

    private IEnumerator WaitActive(string pName)
    {
        var go = GameObject.Find(pName);
        if (null == go)
        {
            Debug.LogError("===> Can Not Find " + pName);
            yield break;
        }
        while (!go.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator WaitNotActive(string pName)
    {
        var go = GameObject.Find(pName);
        if (null == go)
        {
            Debug.LogError("===> Can Not Find " + pName);
            yield break;
        }
        while (go.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    //高亮某Hierarchy路径下的物体
    private void HighLight(string pValue, Action pOnClick = null, bool pExeHoldClickEvent = true)
    {
        var go = GameObject.Find(pValue);
        if (null != go)
        {
            StartCoroutine(HighLight(go, pOnClick, pExeHoldClickEvent));
        }
        else
        {
            Debug.LogError("===> Can Not Find " + pValue);
        }
    }
    private IEnumerator WaitTween(string pValue)
    {
        var go = GameObject.Find(pValue);
        if (null == go)
        {
            Debug.LogError("===> Can Not Find " + pValue);
            yield break;
        }
        TweenPosition tp = go.GetComponent<TweenPosition>();
        if (null != tp)
        {
            while (tp.value != tp.to)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    //pExeHoldClickEvent 是否执行按钮缓存住的事件，还是只执行额外添加的事件
    private IEnumerator HighLight(GameObject pValue, Action pOnClick, bool pExeHoldClickEvent = true)
    {
        Transform parent = pValue.transform.parent;
        TweenPosition tp = pValue.GetComponent<TweenPosition>();
        if (null != tp)
        {
            while (tp.value != tp.to)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        pValue.transform.SetParent(GuideFrame.Instance.ButtonRoot.transform);
        GuideFrame.Instance.Hand.SetActive(true);
        GuideFrame.Instance.Hand.transform.localPosition = pValue.transform.localPosition + new Vector3(35f, -43f, 0);
        //Refresh UI
        ResfeshUI(pValue.transform);

        //这个将按钮恢复到原始位置之后再调用按钮事件，有的事件和自身的Hierarchy有关系
        var btn = pValue.GetComponent<UIButton>();
        List<EventDelegate> clickevent = btn.onClick;
        btn.onClick = new List<EventDelegate>();
        if (!pExeHoldClickEvent)
        {
            btn.onClick.Add(new EventDelegate(() =>
            {
                Reset(pValue, parent);
                btn.onClick = clickevent;
                if (null != pOnClick)
                {
                    pOnClick();
                }
            }));
        }
        else
        {
            btn.onClick.Add(new EventDelegate(() =>
            {
                Reset(pValue, parent);
                btn.onClick = clickevent;
                btn.onClick.ForEach(v =>
                {
                    v.Execute();
                });
                if (null != pOnClick)
                {
                    pOnClick();
                }
            }));
        }
    }

    private void Reset(GameObject pValue, Transform parent)
    {
        pValue.transform.SetParent(parent);
        //        GuideFrame.Instance.gameObject.SetActive(false);
        ResfeshUI(pValue.transform);
    }

    private void ResfeshUI(Transform pValue)
    {
        //暂时解决方案，改变Hierarchy会导致自身显示出现问题
        //强制刷新根会刷新自身
        //gameobject基类可能存在刷新函数,暂时没有发现，暂时使用开关方式,但是存在弊端
        //根部如有脚本运行需注意
        pValue.parent.gameObject.SetActive(false);
        pValue.parent.gameObject.SetActive(true);
    }
    #endregion

    #region Guid Switch
    public IEnumerator SelectOldNew()
    {
        GuideMasterData data = null;
        MasterData.TryGetGuideMasterData((int)GuideType.SelectOldNew, out data);
        string talk = data.Description;
        string[] talks = talk.Split('|');
        yield return new WaitForSeconds(0.5f);
        string buttonyes = "UIRoot - Common/Root/UITop - 145_Guide/UIPanel - UI2D/Tip/ButtonYes";
        string buttonno = "UIRoot - Common/Root/UITop - 145_Guide/UIPanel - UI2D/Tip/ButtonNo";
        var yes = GameObject.Find(buttonyes);
        var no = GameObject.Find(buttonno);
        yes.SetActive(true);
        no.SetActive(true);
        {
            string[] t = talks[0].Split('&');
            GuideFrame.Instance.SetTip(t[0], t[1]);
        }
        GuideFrame.Instance.ActionClickYes = () =>
        {
            Debug.Log("Yes");
            GuideFrame.Instance.SendGuideInfo(() =>
            {
                GuideFrame.Instance.CloseTip();
                GuideFrame.Instance.Close();
                yes.SetActive(false);
                no.SetActive(false);
            }, true);
        };

        GuideFrame.Instance.ActionClickNo = () =>
        {
            Debug.Log("No");
            GuideFrame.Instance.SendGuideInfo(() =>
            {
                GuideFrame.Instance.CloseTip();
                GuideFrame.Instance.SetNext();
                yes.SetActive(false);
                no.SetActive(false);
            });
        };
    }

    public IEnumerator SelectLeader()
    {
        GuideMasterData data = null;
        MasterData.TryGetGuideMasterData((int)GuideType.SelectLeader, out data);
        string talk = data.Description;
        string[] talks = talk.Split('|');
        yield return new WaitForSeconds(0.5f);
        string button = "LobbyMain/UIRoot - Lobby/Root/UITop - 030_LobbyResident(Clone)/UIPanel - UIBG/Top/Sprite/Zhenrong";
        HighLight(button);
        {
            string[] t = talks[0].Split('&');
            GuideFrame.Instance.SetTip(t[0], t[1]);
            GuideFrame.Instance.Hand.SetActive(false);
        }
        GuideFrame.Instance.SetCharaBoard("ui_cb_e001_002");

        string Left = "LobbyMain/UIRoot - Lobby/Root/UITop - 141_DeckEdit(Clone)/UIPanel - UI2D/Left";
        yield return StartCoroutine(WaitActive(Left));
        yield return StartCoroutine(WaitTween(Left));

        button = "LobbyMain/UIRoot - Lobby/Root/UITop - 141_DeckEdit(Clone)/UIPanel - UI2D/Left/Icon (1)";
        {
            string[] t = talks[1].Split('&');
            GuideFrame.Instance.SetTip(t[0], t[1]);
        }
        //等待网络请求获得数据
        if (null == DeckEdit.Instance.DeckInfo)
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        HighLight(button, () =>
        {
            button = "LobbyMain/UIRoot - Lobby/Root/UITop - 141_DeckEdit(Clone)/UIPanel - UI2D/Right/Vertical/Scroll View/UIGrid/Item (1)/Root/Sprite (1)";
            {
                string[] t = talks[2].Split('&');
                GuideFrame.Instance.SetTip(t[0], t[1]);
            }
            HighLight(button, () =>
            {
                button = "LobbyMain/UIRoot - Lobby/Root/UITop - 141_DeckEdit(Clone)/UIPanel - UI2D/Left/Button";
                {
                    string[] t = talks[3].Split('&');
                    GuideFrame.Instance.SetTip(t[0], t[1]);
                }
                HighLight(button, () =>
                {
                    GuideFrame.Instance.SendGuideInfo(() =>
                    {
                        GuideFrame.Instance.CloseTip();
                        GuideFrame.Instance.SetNext();
                    });
                });
            });
        });
    }

    public IEnumerator Shop()
    {
        GuideMasterData data = null;
        MasterData.TryGetGuideMasterData((int)GuideType.Shop, out data);
        string talk = data.Description;
        string[] talks = talk.Split('|');
        yield return new WaitForSeconds(0.5f);
        string buttonshop = "LobbyMain/UIRoot - Lobby/Root/UITop - 030_LobbyResident(Clone)/UIPanel - UIBG/Top/Sprite/Shop";
        GuideFrame.Instance.SetCharaBoard("ui_cb_e001_001");
        HighLight(buttonshop);
        {
            string[] t = talks[0].Split('&');
            GuideFrame.Instance.SetTip(t[0], t[1]);
            GuideFrame.Instance.Hand.SetActive(false);
        }
        string button = "LobbyMain/UIRoot - Lobby/Root/UITop - 350_Shop(Clone)/UIPanel - UI2D/LeftButton/Button (5)";
        yield return StartCoroutine(WaitActive(button));
        {
            string[] t = talks[1].Split('&');
            GuideFrame.Instance.SetTip(t[0], t[1]);
        }
        HighLight(button, () =>
        {
            {
                string[] t = talks[2].Split('&');
                GuideFrame.Instance.SetTip(t[0], t[1]);
            }
            button = "LobbyMain/UIRoot - Lobby/Root/UITop - 350_Shop(Clone)/UIPanel - UI2D/Dianquan/Grid (1)/Item (1)";
            HighLight(button, () =>
            {
                GuideFrame.Instance.CloseTip();
                button = "LobbyMain/UIRoot - Lobby/Root/UITop - 352_ProductsWindow(Clone)/Panel_Fg/Root/BtnGroup_2/BtnCancel";
                HighLight(button, () =>
                {
                    {
                        string[] t = talks[3].Split('&');
                        GuideFrame.Instance.SetTip(t[0], t[1]);
                    }
                    string buttonback = "LobbyMain/UIRoot - Lobby/Root/UITop - 142_TopBottom(Clone)/UIPanel - UI2D/Top/Top/Button (1)";
                    HighLight(buttonback, () =>
                    {
                        GuideFrame.Instance.SendGuideInfo(() =>
                        {
                            GuideFrame.Instance.SetNext();
                        });
                    });
                });
            });
        });

        //BUY
        //END
    }

    public IEnumerator StartGame()
    {
        GuideMasterData data = null;
        MasterData.TryGetGuideMasterData((int)GuideType.StartGame, out data);
        string talk = data.Description;
        string[] talks = talk.Split('|');
        string buttonstart = "LobbyMain/UIRoot - Lobby/Root/UITop - 030_LobbyResident(Clone)/UIPanel - UIBG/Right/Container/Game";
        string[] t = talks[0].Split('&');
        GuideFrame.Instance.SetTip(t[0], t[1]);
        GuideFrame.Instance.SetCharaBoard("ui_cb_e001_003");
        yield return new WaitForSeconds(0.5f);
        HighLight(buttonstart, () =>
        {
            LobbyPacket.SendGuideMatchEntry();
            GuideFrame.Instance.Hand.SetActive(false);
            GuideFrame.Instance.CloseTip();
            StartCoroutine(InGame());
        }, false);
    }

    public IEnumerator InGame()
    {
        GuideMasterData data = null;
        MasterData.TryGetGuideMasterData((int)GuideType.StartGame, out data);
        string talk = data.Description;
        string[] talks = talk.Split('|');
        yield return new WaitForSeconds(0.5f);
        string buttonskill = "UIRoot - Battle/Root/UITop - 420_Skill(Clone)/UIPanel - UIBG/Group_Top/Group_Button/Console/Button_1_Attack";
        yield return StartCoroutine(WaitNotNull(buttonskill));
        {
            string button = "UIRoot - Battle/Root/UITop - 400_TacticalGauge(Clone)/UIPanel - UIBG/Group_Top/Group_BattleType/Group_04_Solo/Button_Exit";
            var go = GameObject.Find(button);
            if (null != go)
            {
                go.SetActive(false);
            }
        }

        {
            string fade = "UIRoot - Common/Root/UITop - Fade";
            yield return StartCoroutine(WaitNotActive(fade));
        }

        GuideFrame.Instance.SetDetailWithoutExe(GuideType.StartGame);
        {
            string[] t = talks[1].Split('&');
            GuideFrame.Instance.SetTip(t[0], t[1]);
        }
        HighLight(buttonskill, () =>
        {
            string tip = "UIRoot - Common/Root/UITop - 145_Guide/UIPanel - UI2D/Tip/Sprite";
            {
                string[] t = talks[2].Split('&');
                GuideFrame.Instance.SetTip(t[0], t[1]);
            }
            HighLight(tip, () =>
            {
                {
                    string[] t = talks[3].Split('&');
                    GuideFrame.Instance.SetTip(t[0], t[1]);
                }
                HighLight(tip, () =>
                {
                    {
                        string[] t = talks[4].Split('&');
                        GuideFrame.Instance.SetTip(t[0], t[1]);
                    }
                    HighLight(tip, () =>
                    {
                        GuideFrame.Instance.CloseTip();
                        GuideFrame.Instance.Close();
                        StartCoroutine(InGame_WaitKill());
                    });
                });
            });
        });
        yield return 0;
    }

    private IEnumerator InGame_WaitKill()
    {
        string manager = "Manager/ObjectManager";
        var go = GameObject.Find(manager);
        if (null != go)
        {
            while (go.transform.childCount > 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        GuideFrame.Instance.SendGuideInfo(() =>
        {
            if (BattleMain.Instance != null)
            {
                BattleMain.Instance.OnResult();
                //            ResultMain.GotoNextScene();
            }
        });
    }
    #endregion
}
