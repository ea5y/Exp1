using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Asobimo.Purchase;
using Asobimo.WebAPI;
using Scm.Common.Master;
using Scm.Common.XwMaster;
using Scm.Common.GameParameter;
using XDATA;

namespace XUI
{
    public class GUIShop : Singleton<GUIShop>
    {
        #region Property
        public ShopView view;

        private CustomControl.TabPagesManager pages;
        private List<UIButton> tabBtnList;

        private enum PageType
        {
            XD,
            PF,
            JS,
            CZ,
            DQ,
            LB
        }

        private CustomControl.TabPagesManager detailPages;

        private List<UIButton> pfTabBtnList;
        private CustomControl.TabPagesManager pfPages;
        private enum PFPageType
        {
            Day3,
            Day7,
            Day30,
            Long
        }

        private List<UIButton> jsTabBtnList;
        private CustomControl.TabPagesManager jsPages;
        private enum JSPageType
        {
            Day3,
            Day7,
            Day30,
            Long
        }

        private ActivityProductList dataXD;
        private ProductStatusList dataPF;
        private ProductStatusList dataJS;
        private TicketList dataCZ;
        private ProductStatusList dataDQ;
        private ProductStatusList dataLB;
        private bool isReset;

        private bool isGetData = false;
        private bool isLoaded = false;

        public List<ItemShopPF> gridPFItems;
        public List<ItemShopJS> gridJSItems;

        private CustomControl.ScrollView<ItemScrollShopPF> scrollViewPF3Day;
        private CustomControl.ScrollView<ItemScrollShopPF> scrollViewPF7Day;
        private CustomControl.ScrollView<ItemScrollShopPF> scrollViewPF30Day;
        private CustomControl.ScrollView<ItemScrollShopPF> scrollViewPFLong;

        private CustomControl.ScrollView<ItemScrollShopJS> scrollViewJS3Day;
        private CustomControl.ScrollView<ItemScrollShopJS> scrollViewJS7Day;
        private CustomControl.ScrollView<ItemScrollShopJS> scrollViewJS30Day;
        private CustomControl.ScrollView<ItemScrollShopJS> scrollViewJSLong;

        private CustomControl.Radar radar;
        private SkillIcon skillIcon { get { return ScmParam.Battle.SkillIcon; } }

        private Camera camera3D;

        private int tipCoinCost;
        private Action TipOkAction;
        private CharaIcon CharaIcon{get{return ScmParam.Battle.CharaIcon;}}

        private Action OnDetailSureClick;

        private const int NEED_INIT = 20005;
        private const int CAN_BUY = 10000;
        private const int NO_PRODUCT = 20001;
        private const int NO_BUY_LEFT = 20002;
        private const int NO_MONEY = 20003;

        public class ItemPFData
        {
            public ProductStatus info;
            public int avatarId;
            public bool isHas;
        }

        private List<ItemPFData> shopPFDataList = new List<ItemPFData>();
        #endregion

        private void Awake()
        {
            base.Awake();
            Debug.Log("Run shop");
            this.CreatePages();
            this.CreateTabBtnList();

            this.CreateDetailPages();

            this.CreatePFTabBtnList();
            this.CreatePFPages();

            this.CreateJSTabBtnList();
            this.CreateJSPages();

            this.RegisterEventOnce();
            this.FirstHide();            
        }

        private void OnEnable()
        {

            this.SetCamera3D();
        }

        private void RegisterEvent()
        {
            XDATA.PlayerData.Instance.OnEnergyChange += this.SetDQEnergy;
        }

        private void DeleteEvent()
        {
            XDATA.PlayerData.Instance.OnEnergyChange -= this.SetDQEnergy;
        }

        private void OnDisable()
        {

            this.DeleteEvent();
            this.ResetCamera3D();
        }

        private void SwitchTo(PageType type)
        {
            switch (type)
            {
                case PageType.XD:
                    this.pages.SwitchTo(this.view.xd);
                    this.SetXD();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabBtn.btnXD, this.tabBtnList);
                    break;
                case PageType.PF:
                    this.pages.SwitchTo(this.view.pf);
                    this.SetPF();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabBtn.btnPF, this.tabBtnList);
                    break;
                case PageType.JS:
                    this.pages.SwitchTo(this.view.js);
                    this.SetJS();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabBtn.btnJS, this.tabBtnList);
                    break;
                case PageType.CZ:
                    this.pages.SwitchTo(this.view.cz);
                    this.SetCZ();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabBtn.btnCZ, this.tabBtnList);
                    break;
                case PageType.DQ:
                    this.pages.SwitchTo(this.view.dq);
                    this.SetDQ();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabBtn.btnDQ, this.tabBtnList);
                    break;
                case PageType.LB:
                    this.pages.SwitchTo(this.view.lb);
                    this.SetLB();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.tabBtn.btnLB, this.tabBtnList);
                    break;
            }
        }

        private void GoTo(GameObject to, GameObject from)
        {
            from.SetActive(false);
            to.SetActive(true);
        }

        private void SetName(string name, UILabel lblName)
        {
            lblName.text = name;
        }

        private void SetDetailTime(int time)
        {
            if (time == 0)
                this.view.detail.lblTime.text = "有效期： 永久";
            else
                this.view.detail.lblTime.text = "有效期： " + time + "天";

        }

        private void SetPortriat(AvatarType avatarType, int skinId, GameObject portriat)
        {
            //portriat.DestroyChild();
            portriat.DestroyChildImmediate();
            CharaBoard charaBoard = ScmParam.Lobby.CharaBoard;
            if (charaBoard != null)
            {
                charaBoard.GetBoard(avatarType, skinId, false,
                    (res) =>
                    {
                        if (res == null)
                            return;
                        var go = SafeObject.Instantiate(res) as GameObject;
                        if (go == null)
                            return;
                        go.name = res.name;
                        var t = go.transform;
                        t.parent = portriat.transform;

                        if (skinId == 0)
                        {
                            skinId = AvatarMaster.Instance.GetDefaultAvatarId((int)avatarType);
                        }
                        AvatarMasterData data;
                        AvatarMaster.Instance.TryGetMasterData(skinId, out data);

                        if (data.OffsetX == 0 && data.OffsetY == 0 && data.Scale == 0)
                        {
                            t.localPosition = new Vector3(data.OffsetX, data.OffsetY, 0);
                            t.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                            return;
                        }
                        Debug.Log("CharaId: " + (int)avatarType);
                        Debug.Log("SkinId: " + skinId);
                        t.localPosition = new Vector3(data.OffsetX, data.OffsetY, 0);
                        t.localScale = new Vector3(data.Scale, data.Scale, data.Scale);
                        Debug.Log("Scale: " + data.Scale);
                    });
            }
        }

        private void SetPrice(UILabel lblPrice, int price)
        {
            lblPrice.text = price + "";
        }

        #region Init
        private void CreateTabBtnList()
        {
            if (this.tabBtnList == null)
            {
                this.tabBtnList = new List<UIButton>();

                this.tabBtnList.Add(this.view.tabBtn.btnXD);
                this.tabBtnList.Add(this.view.tabBtn.btnPF);
                this.tabBtnList.Add(this.view.tabBtn.btnJS);
                this.tabBtnList.Add(this.view.tabBtn.btnCZ);
                this.tabBtnList.Add(this.view.tabBtn.btnDQ);
                this.tabBtnList.Add(this.view.tabBtn.btnLB);
            }
        }

        private void CreatePages()
        {
            if (this.pages == null)
            {
                this.pages = new CustomControl.TabPagesManager();
                this.pages.AddPage(this.view.xd);
                this.pages.AddPage(this.view.pf);
                this.pages.AddPage(this.view.js);
                this.pages.AddPage(this.view.cz);
                this.pages.AddPage(this.view.dq);
                this.pages.AddPage(this.view.lb);

                this.view.js.action = () => {
                    this.GoTo(this.view.js.root, this.view.detail.root);
                };
                this.view.pf.action = () => {
                    this.GoTo(this.view.pf.root, this.view.detail.root);
                };
            }
        }

        private void CreateDetailPages()
        {
            if (this.detailPages == null)
            {
                this.detailPages = new CustomControl.TabPagesManager();
                this.detailPages.AddPage(this.view.detail.detailJS);
                this.detailPages.AddPage(this.view.detail.detailPF);
            }
        }

        private void CreatePFTabBtnList()
        {
            if (this.pfTabBtnList == null)
            {
                this.pfTabBtnList = new List<UIButton>();

                this.pfTabBtnList.Add(this.view.pf.btn3Day);
                this.pfTabBtnList.Add(this.view.pf.btn7Day);
                this.pfTabBtnList.Add(this.view.pf.btn30Day);
                this.pfTabBtnList.Add(this.view.pf.btnLong);
            }
        }

        private void CreatePFPages()
        {
            if (this.pfPages == null)
            {
                this.pfPages = new CustomControl.TabPagesManager();
                this.pfPages.AddPage(this.view.pf.page3Day);
                this.pfPages.AddPage(this.view.pf.page7Day);
                this.pfPages.AddPage(this.view.pf.page30Day);
                this.pfPages.AddPage(this.view.pf.pageLong);
            }
        }

        private void CreateJSTabBtnList()
        {
            if(this.jsTabBtnList == null)
            {
                this.jsTabBtnList = new List<UIButton>();

                this.jsTabBtnList.Add(this.view.js.btn3Day);
                this.jsTabBtnList.Add(this.view.js.btn7Day);
                this.jsTabBtnList.Add(this.view.js.btn30Day);
                this.jsTabBtnList.Add(this.view.js.btnLong);
            }
        }

        private void CreateJSPages()
        {
            if(this.jsPages == null)
            {
                this.jsPages = new CustomControl.TabPagesManager();

                this.jsPages.AddPage(this.view.js.page3Day);
                this.jsPages.AddPage(this.view.js.page7Day);
                this.jsPages.AddPage(this.view.js.page30Day);
                this.jsPages.AddPage(this.view.js.pageLong);
            }
        }

        private void FirstHide()
        {
            this.gameObject.SetActive(false);
        }

        private void RegisterEventOnce()
        {
            EventDelegate.Add(this.view.tabBtn.btnXD.onClick, () => {
                this.SwitchTo(PageType.XD);
            });
            EventDelegate.Add(this.view.tabBtn.btnPF.onClick, () => {
                this.SwitchTo(PageType.PF);
            });
            EventDelegate.Add(this.view.tabBtn.btnJS.onClick, () => {
                this.SwitchTo(PageType.JS);
            });
            EventDelegate.Add(this.view.tabBtn.btnCZ.onClick, () => {
                this.SwitchTo(PageType.CZ);
            });
            EventDelegate.Add(this.view.tabBtn.btnDQ.onClick, () => {
                this.SwitchTo(PageType.DQ);
            });
            EventDelegate.Add(this.view.tabBtn.btnLB.onClick, () => {
                this.SwitchTo(PageType.LB);
            });

            EventDelegate.Add(this.view.xd.btnStart.onClick, () => {
                Debug.Log("Preview");
                this.XDStart();
            });

            EventDelegate.Add(this.view.xd.btnShowOne.onClick, () => {
                Debug.Log("ShowOne");

                if (XDATA.PlayerData.Instance.Coin < int.Parse(this.view.xd.lblPrice.text))
                {
                    GUITipMessage.Instance.Show("点券不够！");
                    return;
                }
                Net.Network.Instance.StartCoroutine(this.BuyXD());
            });

            EventDelegate.Add(this.view.xd.btnReset.onClick, () => {
                Net.Network.Instance.StartCoroutine(this.ResetXD());
            });

            EventDelegate.Add(this.view.detail.detailJS.btnBuy.onClick, () => {
                Debug.Log("BuyJS: ready!");
                if (this.OnDetailSureClick != null)
                {
                    Debug.Log("BuyJS: go!");
                    this.OnDetailSureClick();
                }
            });

            EventDelegate.Add(this.view.xd.btnTip.onClick, () =>{
                this.OpenXDTip();
            });

            EventDelegate.Add(this.view.xd.btnTipClose.onClick, () => {
                this.CloseXDTip();
            });

            EventDelegate.Add(this.view.detail.detailJS.btnCancel.onClick, () => {
                this.GoTo(this.view.js.root, this.view.detail.root);
                this.OnDetailSureClick = null;
            });

            EventDelegate.Add(this.view.detail.detailPF.btnBuy.onClick, () => {
                Debug.Log("BuyPF: ready!");
                if (this.OnDetailSureClick != null)
                {
                    Debug.Log("BuyPF: go!");
                    this.OnDetailSureClick();
                }
            });

            EventDelegate.Add(this.view.detail.detailPF.btnCancel.onClick, () => {
                this.GoTo(this.view.pf.root, this.view.detail.root);
                this.OnDetailSureClick = null;
            });

            EventDelegate.Add(this.view.pf.btn3Day.onClick, () => {
                this.PFSwitchTo(PFPageType.Day3);
            });
            EventDelegate.Add(this.view.pf.btn7Day.onClick, () => {
                this.PFSwitchTo(PFPageType.Day7);
            });
            EventDelegate.Add(this.view.pf.btn30Day.onClick, () => {
                this.PFSwitchTo(PFPageType.Day30);
            });
            EventDelegate.Add(this.view.pf.btnLong.onClick, () => {
                this.PFSwitchTo(PFPageType.Long);
            });

            EventDelegate.Add(this.view.js.btn3Day.onClick, () => {
                this.JSSwitchTo(JSPageType.Day3);
            });
            EventDelegate.Add(this.view.js.btn7Day.onClick, () => {
                this.JSSwitchTo(JSPageType.Day7);    
            });
            EventDelegate.Add(this.view.js.btn30Day.onClick, () => {
                this.JSSwitchTo(JSPageType.Day30);
            });
            EventDelegate.Add(this.view.js.btnLong.onClick, () => {
                this.JSSwitchTo(JSPageType.Long);
            });
            EventDelegate.Add(this.view.lb.btnGo.onClick, () => {
                var gameId = this.dataLB.ProductArray[0].GameID;
                ShopItemMasterData sData;
                MasterData.TryGetShop(gameId, out sData);
                GUIProductsWindow.Instance.OpenOkNo(this.dataLB.ProductArray[0].Price, new List<int>(sData.PackageIds), () => {
                    this.BuyLB();
                });
              
            });

            this.RegisterSkillTouchEvent();
        }
        #endregion

        #region OpenWay
        public void OpenToXD()
        {
            Net.Network.Instance.StartCoroutine(this._OpenToXD());
        }

        public void OpenToDQ()
        {
            Net.Network.Instance.StartCoroutine(this._OpenToDQ());
        }

        public void OpenToCZ()
        {
            Net.Network.Instance.StartCoroutine(this._OpenToCZ());
        }

        private IEnumerator _Open(Action func)
        {
            this.RegisterEvent();
            if (!this.isGetData)
                yield return this.GetData();
            yield return this.GetEnergy();
            yield return this.SetPFIsHas();
            func();
            TopBottom.Instance.OnIn = () => {
                PanelManager.Instance.Open(this.view.root, true, false);
            };
            TopBottom.Instance.OnBack = (v) => {
                PanelManager.Instance.Close(this.view.root);
                v();
            };
            TopBottom.Instance.In("商城");
        }

        private IEnumerator _OpenToXD()
        {

            yield return this._Open(() => {
                this.SwitchTo(PageType.XD);
            });

        }

        private IEnumerator _OpenToCZ()
        {

            yield return this._Open(() => {
                this.SwitchTo(PageType.CZ);
            });
        }

        private IEnumerator _OpenToDQ()
        {

            yield return this._Open(() => {
                this.SwitchTo(PageType.DQ);
            });

        }
        #endregion

        #region CZ
        private void SetCZ()
        {
            Debug.Log("=======>Length: " + this.dataCZ.ProductArray.Length);
            for (int i = 0; i < this.view.cz.items.Count; ++i)
            {
                if (i < this.dataCZ.ProductArray.Length)
                    this.view.cz.items[i].FillItem(this.dataCZ.ProductArray[i], i);
                else
                    this.view.cz.items[i].gameObject.SetActive(false);
            }
        }
        #endregion

        #region XD
        private void SetXD()
        {
            this.SetXDLeft();
            this.SetXDStatus();
            this.FillSudoku(this.dataXD.product_obtain_code == CAN_BUY);
            this.SetXDTime();
        }

        private void SetXDLeft()
        {
            if (this.dataXD.product != null && this.dataXD.product.Length > 0)
            {
                int shopID = 0;
                if (int.TryParse(this.dataXD.product[this.dataXD.product.Length - 1].game_inside_id, out shopID))
                {
                    Scm.Common.XwMaster.ShopItemMasterData data;
                    if (MasterData.TryGetShop(shopID, out data))
                    {
                        int characterId = data.CharacterId;
                        int avatarId = Scm.Common.Master.AvatarMaster.Instance.GetDefaultAvatarId(characterId);

                        //this.view.xd.portriat.DestroyChild();
                        this.view.xd.portriat.DestroyChildImmediate();

                        //Add portrait
                        this.SetPortriat((AvatarType)characterId, avatarId, this.view.xd.portriat);
                        //set desc
                        this.SetXDName(data.Description);
                    }
                }
            }
        }

        private void SetXDName(string name)
        {
            this.view.xd.lblName.text = name;
        }

        private void SetXDTime()
        {
            var timeAry = CustomControl.ToolFunc.TimeSecondIntToString(this.dataXD.date_remain, true).Split(':');

            this.view.xd.lblTime.text = timeAry[0] + " 小时 " + timeAry[1] + " 分之后刷新";
        }

        private bool SetXDStatus()
        {
            bool result = true;
            Debug.Log("CodeStauts：" + this.dataXD.product_obtain_code);
            switch (this.dataXD.product_obtain_code)
            {
                case NEED_INIT:
                    this.SetXDBtnStatus(XDStatus.Init);
                    this.SetResetPrice(this.dataXD.reset_coin + "");
                    break;
                case CAN_BUY:
                    this.SetXDBtnStatus(XDStatus.Active);
                    this.SetXDPrice(this.dataXD.purchase_next_price + "");

                    if (this.dataXD.purchase_next_price == 0)
                    {
                        this.SetXDBtnStatus(XDStatus.Inactive);
                        this.SetResetPrice(this.dataXD.reset_coin + "");
                        this.dataXD.product_obtain_code = NO_BUY_LEFT;
                    }
                    break;
                case NO_PRODUCT:
                    break;
                case NO_BUY_LEFT:
                    this.SetXDBtnStatus(XDStatus.Inactive);
                    this.SetResetPrice(this.dataXD.reset_coin + "");
                    result = false;
                    break;
                case NO_MONEY:
                    GUITipMessage.Instance.Show("点券不够！");
                    result = false;
                    break;
            }
            return result;
        }

        private void FillSudoku(bool canBuy)
        {
            if(this.view.xd.items.Count != 9)
            {
                Debug.LogError("九宫格配置数量不对！");
                return;
            }

            for (int i = 0; i < this.view.xd.items.Count; ++i)
            {
                this.view.xd.items[i].FillItem(this.dataXD.product[i], canBuy);
            }
        }

        private void XDStart()
        {
            Net.Network.Instance.StartCoroutine(this.InitXD());
        }

        private enum XDStatus
        {
            Init,
            Active,
            Inactive
        }
        
        private void SetXDBtnStatus(XDStatus status)
        {
            this.view.xd.btnStart.gameObject.SetActive(status == XDStatus.Init);
            this.view.xd.btnShowOne.gameObject.SetActive(status == XDStatus.Active);
            this.view.xd.btnReset.gameObject.SetActive(status == XDStatus.Inactive);
        }

        private void XDCanBuy(bool canBuy)
        {
            this.view.xd.btnStart.gameObject.SetActive(!canBuy);
            this.view.xd.btnShowOne.gameObject.SetActive(canBuy);
            this.view.xd.btnReset.gameObject.SetActive(canBuy);
        }

        private void DisableXDBtn(bool isDisable)
        {
            this.view.xd.btnStart.enabled = !isDisable;
            if (isDisable)
                this.view.xd.btnStart.SetState(UIButton.State.Disabled, true);
            else
                this.view.xd.btnStart.SetState(UIButton.State.Normal, true);
        }

        private void SetXDPrice(string price)
        {
            this.view.xd.lblPrice.text = price;
        }

        private void SetResetPrice(string price)
        {
            this.view.xd.lblResetPrice.text = price;
            if(this.dataXD.product_obtain_code != NEED_INIT)
            {
                if(price == "-1" || price == "0")
                {
                    this.SetXDBtnStatus(XDStatus.Init);
                    this.DisableXDBtn(true);
                }
            }
        }

        private void OpenXDTip()
        {
            this.view.xd.groupTip.SetActive(true);
        }

        private void CloseXDTip()
        {
            this.view.xd.groupTip.SetActive(false);
        }
        #endregion

        #region PF
        private void SetPF()
        {
            this.PFSwitchTo(PFPageType.Long);
        }
        
        private List<ProductStatus> testDatas(List<ProductStatus> datas)
        {
            List<ProductStatus> result = new List<ProductStatus>();
            for(int i = 0; i < 67; i++)
            {
                result.Add(datas[0]);
            }
            return result;
        }

        private void SetPF3Day()
        {
            var datas = this.PackPFData(this.PFFilter(3));
            //var datas = this.PackPFData(this.testDatas(this.PFFilter(3)));
            Debug.Log("DatasPF: " + datas);
            if (this.scrollViewPF3Day == null)
            {
                this.scrollViewPF3Day = new CustomControl.ScrollView<ItemScrollShopPF>(this.view.pf.page3Day.grid, this.view.pf.page3Day.itemPF);
                this.scrollViewPF3Day.SetCachNum(2);
            }
            this.scrollViewPF3Day.CreateWeight(datas);
        }

        private void SetPF7Day()
        {
            var datas = this.PackPFData(this.PFFilter(7));
            //var datas = this.PackPFData(this.testDatas(this.PFFilter(7)));
            Debug.Log("DatasPF: " + datas);
            if (this.scrollViewPF7Day == null)
            {
                this.scrollViewPF7Day = new CustomControl.ScrollView<ItemScrollShopPF>(this.view.pf.page7Day.grid, this.view.pf.page3Day.itemPF);
                this.scrollViewPF7Day.SetCachNum(2);
            }
            this.scrollViewPF7Day.CreateWeight(datas);
        }

        private void SetPF30Day()
        {
            var datas = this.PackPFData(this.PFFilter(30));
            //var datas = this.PackPFData(this.testDatas(this.PFFilter(30)));
            Debug.Log("DatasPF: " + datas);
            if (this.scrollViewPF30Day == null)
            {
                this.scrollViewPF30Day = new CustomControl.ScrollView<ItemScrollShopPF>(this.view.pf.page30Day.grid, this.view.pf.page3Day.itemPF);
                this.scrollViewPF30Day.SetCachNum(2);
            }
            this.scrollViewPF30Day.CreateWeight(datas);
       }

        private void SetPFLong()
        {
            var datas = this.PackPFData(this.PFFilter(0));
            //var datas = this.PackPFData(this.testDatas(this.PFFilter(0)));
            Debug.Log("DatasPF: " + datas);
            if (this.scrollViewPFLong == null)
            {
                this.scrollViewPFLong = new CustomControl.ScrollView<ItemScrollShopPF>(this.view.pf.pageLong.grid, this.view.pf.page3Day.itemPF);
                this.scrollViewPFLong.SetCachNum(2);
            }
            this.scrollViewPFLong.CreateWeight(datas);
        }

        private List<ItemPFData> PFFilter(int day)
        {
            var result = new List<ItemPFData>();
            for(int i = 0; i < this.shopPFDataList.Count; i++)
            {
                ShopItemMasterData sData;
                MasterData.TryGetShop(this.shopPFDataList[i].info.GameID, out sData);
                if(sData == null)
                {
                    Debug.LogError("==>皮肤：master数据没取到！\n GameId: " + this.shopPFDataList[i].info.GameID);
                    return result;
                }
                if (sData.Expire == day)
                    result.Add(this.shopPFDataList[i]);
            }
            return result;
        }

        private List<List<ItemPFData>> PackPFData(List<ItemPFData> datas)
        {
            List<List<ItemPFData>> pfDataList = new List<List<ItemPFData>>();
            List<ItemPFData> pfGroupDataList = new List<ItemPFData>();
            List<ItemPFData> bufferList = new List<ItemPFData>(datas);

            for(int i = 0; i < Mathf.CeilToInt((float)datas.Count / 5); i++)
            {
                pfGroupDataList = new List<ItemPFData>();
                for(int j = 0; j < 5; j++)
                {
                    if(bufferList.Count != 0)
                    {
                        pfGroupDataList.Add(bufferList[0]);
                        bufferList.RemoveAt(0);
                    }
                }

                pfDataList.Add(pfGroupDataList);
            }
            return pfDataList;
        }

        private void PFSwitchTo(PFPageType type)
        {
            switch (type)
            {
                case PFPageType.Day3:
                    this.pfPages.SwitchTo(this.view.pf.page3Day);
                    //this.SetPF3Day();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.pf.btn3Day, this.pfTabBtnList);
                    break;
                case PFPageType.Day7:
                    this.pfPages.SwitchTo(this.view.pf.page7Day);
                    this.SetPF7Day();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.pf.btn7Day, this.pfTabBtnList);
                    break;
                case PFPageType.Day30:
                    this.pfPages.SwitchTo(this.view.pf.page30Day);
                    this.SetPF30Day();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.pf.btn30Day, this.pfTabBtnList);
                    break;
                case PFPageType.Long:
                    this.pfPages.SwitchTo(this.view.pf.pageLong);
                    this.SetPFLong();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.pf.btnLong, this.pfTabBtnList);
                    break;
            }
        }

        #region DetailPF
        public void SetDetailPF(ItemShopPF itemPF)
        {
            Debug.Log("avatarType: " + itemPF.aData.CharacterId);
            Debug.Log("skinId: " + itemPF.aData.ID);

            this.GoTo(this.view.detail.root, this.view.pf.root);
            this.detailPages.SwitchTo(this.view.detail.detailPF);
            this.SetPortriat((AvatarType)itemPF.aData.CharacterId, itemPF.sData.AvatarId, this.view.detail.portriat);
            this.SetPortriat3D((AvatarType)itemPF.aData.CharacterId, itemPF.sData.AvatarId, this.view.detail.detailPF.portriat3D);
            this.SetPrice(this.view.detail.detailPF.lblPrice, itemPF.data.info.Price);

            Scm.Common.Master.CharaMasterData cData;
            MasterData.TryGetChara(itemPF.aData.CharacterId, out cData);
            this.SetName(cData.Name, this.view.detail.lblName);
            this.SetDetailTime(itemPF.sData.Expire);

            this.OnDetailSureClick = () => {
                var gId = itemPF.data.info.GameID;
                var item = itemPF;

                GUIProductsWindow.Instance.OpenOkNo(this.GetProductShowData(itemPF), () => {
                    this.BuyPF(item);
                    itemPF.data.isHas = true;
                    itemPF.SetIsHas();
                });
            };
        }

        private XUI.ProductShowData GetProductShowData(ItemShopPF itemPF)
        {
            var result = new XUI.ProductShowData();
            result.type = ShopItemType.Avatar;
            result.tip = "是否花费 " + itemPF.data.info.Price + " 点券购买？";
            result.characterId = (int)itemPF.sData.CharacterId;
            result.skinId = itemPF.sData.AvatarId;
            result.desc = "皮肤\n" + itemPF.sData.Description;
            return result;
        }

        private void SetCamera3D()
        {
            if (this.camera3D == null)
                this.camera3D = GameObject.Find("2_UI3DCamera").GetComponent<Camera>();
            this.camera3D.transform.localPosition = new Vector3(1.08f, 0.25f, -1.22f);
            camera3D.fieldOfView = 90;
            camera3D.rect = new Rect(0.685f, 0.24f, 0.245f, 0.59f);
        }

        private void ResetCamera3D()
        {
            if (this.camera3D != null)
            {
                this.camera3D.transform.localPosition = new Vector3(0.0f, 0.0f, -1.8f);
                camera3D.fieldOfView = 60;
                camera3D.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            }

        }

        private void SetPortriat3D(AvatarType avatarType, int skinId, GameObject portriat3D)
        {
            //portriat3D.DestroyChild();
            portriat3D.DestroyChildImmediate();
            var charaInfo = new CharaInfo(avatarType, skinId);
            CharaModel.Create(portriat3D, charaInfo);
            
            this.SetCamera3DDetail(charaInfo);
        }

        private void SetCamera3DDetail(CharaInfo info)
        {
            AvatarMasterData aData;
            MasterData.TryGetAvatar(info.CharacterMasterID, info.SkinId, out aData);
            
            //strArray[0] = localPosition.x, strArray[1] = localPosition.y, strArray[2] = localPosition.z
            //strArray[3] = localRotation.x, strArray[4] = localRotation.y, strArray[5] = Field of View
            var strArray = aData.CameraStr.Split(';');
            Debug.Log("str: " + aData.CameraStr);

            if(strArray.Length > 0)
            {
                this.camera3D.transform.localPosition = new Vector3(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));
                this.camera3D.transform.localRotation = Quaternion.Euler(float.Parse(strArray[3]), float.Parse(strArray[4]), 0);
                this.camera3D.fieldOfView = float.Parse(strArray[5]);
            }
        }
        #endregion
        #endregion

        #region JS
        private void SetJS()
        {
            this.JSSwitchTo(JSPageType.Long);
        }

        private void SetJS3Day()
        {
            var datas = this.PackJSData(this.JSFilter(3));
            //var datas = this.PackJSData(this.testDatas(this.JSFilter(3)));
            Debug.Log("DatasJS: " + datas);
            if(this.scrollViewJS3Day == null)
                this.scrollViewJS3Day = new CustomControl.ScrollView<ItemScrollShopJS>(this.view.js.page3Day.grid, this.view.js.page3Day.itemJS);
            this.scrollViewJS3Day.SetCachNum(2);
            //此功能有bug,暂时不用了
            //this.scrollViewJS3Day.SetDragFillDelay(0);
            this.scrollViewJS3Day.CreateWeight(datas);
        }

        private void SetJS7Day()
        {
            var datas = this.PackJSData(this.JSFilter(7));
            //var datas = this.PackJSData(this.testDatas(this.JSFilter(7)));
            Debug.Log("DatasJS: " + datas);
            if(this.scrollViewJS7Day == null)
                this.scrollViewJS7Day = new CustomControl.ScrollView<ItemScrollShopJS>(this.view.js.page7Day.grid, this.view.js.page3Day.itemJS);
            this.scrollViewJS7Day.SetCachNum(2);
            //this.scrollViewJS7Day.SetDragFillDelay(0);
            this.scrollViewJS7Day.CreateWeight(datas);
        }

        private void SetJS30Day()
        {
            var datas = this.PackJSData(this.JSFilter(30));
            //var datas = this.PackJSData(this.testDatas(this.JSFilter(30)));
            Debug.Log("DatasJS: " + datas);
            if(this.scrollViewJS30Day == null)
                this.scrollViewJS30Day = new CustomControl.ScrollView<ItemScrollShopJS>(this.view.js.page30Day.grid, this.view.js.page3Day.itemJS);
            this.scrollViewJS30Day.SetCachNum(2);
            this.scrollViewJS30Day.CreateWeight(datas);
        }

        private void SetJSLong()
        {
            var datas = this.PackJSData(this.JSFilter(0));
            //var datas = this.PackJSData(this.testDatas(this.JSFilter(0)));
            Debug.Log("DatasJS: " + datas);
            if(this.scrollViewJSLong == null)
                this.scrollViewJSLong = new CustomControl.ScrollView<ItemScrollShopJS>(this.view.js.pageLong.grid, this.view.js.page3Day.itemJS);
            this.scrollViewJSLong.SetCachNum(2);
            this.scrollViewJSLong.CreateWeight(datas);
       }

        private List<ProductStatus> JSFilter(int day)
        {
            var result = new List<ProductStatus>();
            for(int i = 0; i < this.dataJS.ProductArray.Length; i++)
            {
                ShopItemMasterData sData;
                MasterData.TryGetShop(this.dataJS.ProductArray[i].GameID, out sData);
                if (sData.Expire == day)
                    result.Add(this.dataJS.ProductArray[i]);
            }
            return result;
        }

        private List<List<ProductStatus>> PackJSData(List<ProductStatus> datas)
        {
            List<List<ProductStatus>> jsDataList = new List<List<ProductStatus>>();
            List<ProductStatus> jsGroupDataList = new List<ProductStatus>();
            List<ProductStatus> bufferList = new List<ProductStatus>(datas);

            Debug.Log("JSCount: " + datas.Count);
            for(int i = 0; i < Mathf.CeilToInt((float)datas.Count / 5); i++)
            {
                jsGroupDataList = new List<ProductStatus>();
                for(int a = 0; a < 5; a++)
                {
                    if(bufferList.Count != 0)
                    {
                        jsGroupDataList.Add(bufferList[0]);
                        bufferList.RemoveAt(0);
                    }
                }

                jsDataList.Add(jsGroupDataList);
            }

            return jsDataList;
        }

        private void JSSwitchTo(JSPageType type)
        {
            switch(type)
            {
                case JSPageType.Day3:
                    this.jsPages.SwitchTo(this.view.js.page3Day);
                    this.SetJS3Day();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.js.btn3Day, this.jsTabBtnList);
                break;
                case JSPageType.Day7:
                    this.jsPages.SwitchTo(this.view.js.page7Day);
                    this.SetJS7Day();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.js.btn7Day, this.jsTabBtnList);
                break;
                case JSPageType.Day30:
                    this.jsPages.SwitchTo(this.view.js.page30Day);
                    this.SetJS30Day();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.js.btn30Day, this.jsTabBtnList);
                break;
                case JSPageType.Long:
                    this.jsPages.SwitchTo(this.view.js.pageLong);
                    this.SetJSLong();
                    CustomControl.ToolFunc.BtnSwitchTo(this.view.js.btnLong, this.jsTabBtnList);
                break;
            }
        }
        #region DetailJS
        public void SetDetailJS(ItemShopJS itemJS)
        {
            Debug.Log("avatarType: " + itemJS.sData.CharacterId);
            Debug.Log("skinId: " + itemJS.sData.AvatarId);
            this.GoTo(this.view.detail.root, this.view.js.root);
            this.detailPages.SwitchTo(this.view.detail.detailJS);

            Scm.Common.Master.CharaMasterData cData;
            MasterData.TryGetChara(itemJS.sData.CharacterId, out cData);
            this.SetName(cData.Name, this.view.detail.lblName);
            this.SetDetailTime(itemJS.sData.Expire);

            this.SetPortriat((AvatarType)itemJS.sData.CharacterId, itemJS.sData.AvatarId, this.view.detail.portriat);
            this.SetRadar((AvatarType)itemJS.sData.CharacterId);
            this.SetSkill((AvatarType)itemJS.sData.CharacterId);
            this.SetPrice(this.view.detail.detailJS.lblPrice, itemJS.data.Price);

            this.OnDetailSureClick = () => {
                var e = itemJS.effect;
                var id = itemJS.data.ProductID.ToString();
                var p = itemJS.data.Price;

                GUIProductsWindow.Instance.OpenOkNo(this.GetProductShowData(itemJS), () => {
                    this.BuyJS(p, id, e);
                });
            };
        }

        private XUI.ProductShowData GetProductShowData(ItemShopJS itemJS)
        {
            var result = new XUI.ProductShowData();
            result.type = ShopItemType.Character;
            result.tip = "是否花费 " + itemJS.data.Price + " 点券购买？";
            result.characterId = itemJS.sData.CharacterId;
            result.skinId = itemJS.sData.AvatarId;
            Scm.Common.Master.CharaMasterData cData;
            MasterData.TryGetChara(itemJS.sData.CharacterId, out cData);
            result.desc = "角色\n" + cData.Name;
            return result;
        }

        private void SetRadar(AvatarType avatarType)
        {
            if (this.radar == null)
                this.radar = new CustomControl.Radar(this.view.detail.detailJS.radar, this.view.detail.detailJS.mtRadar);

            CharaProfileMasterData data;
            MasterData.TryGetCharaProfileMasterData((int)avatarType, out data);
            float baseNum = 10;
            this.radar.SetValues(data.Ctrl / baseNum, data.Atk / baseNum, data.Def / baseNum, data.Spd / baseNum, data.Aid / baseNum);
        }

        private string skillName1;
        private string skillName2;
        private string skillName3;
        private string skillName4;

        private string skillDesc1;
        private string skillDesc2;
        private string skillDesc3;
        private string skillDesc4;
        private void SetSkill(AvatarType avatarType)
        {
            CharaLevelMasterData lData;
            if (MasterData.TryGetCharaLv((int)avatarType, 1, out lData))
            {
                CharaButtonSetMasterData bsData;
                //bsData.TechnicalSkillButton
                if (MasterData.TryGetCharaButtonSet(lData, out bsData))
                {
                    skillIcon.GetSkillIcon(bsData, 1, SkillButtonType.Normal, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, this.view.detail.detailJS.skill_1); });
                    skillIcon.GetSkillIcon(bsData, 1, SkillButtonType.Skill1, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, this.view.detail.detailJS.skill_2); });
                    skillIcon.GetSkillIcon(bsData, 1, SkillButtonType.Skill2, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, this.view.detail.detailJS.skill_3); });
                    skillIcon.GetSkillIcon(bsData, 1, SkillButtonType.SpecialSkill, false, (atlas, spriteName) => { this.SetSkillIcon(atlas, spriteName, this.view.detail.detailJS.skill_4); });
                }

                //Set desc
                this.skillName1 = bsData.AttackButton.ButtonName;
                this.skillName2 = bsData.Skill1Button.ButtonName;
                this.skillName3 = bsData.Skill2Button.ButtonName;
                this.skillName4 = bsData.SpecialSkillButton.ButtonName;
                this.skillDesc1 = this.GetSkillDesc(bsData.AttackButton);
                this.skillDesc2 = this.GetSkillDesc(bsData.Skill1Button);
                this.skillDesc3 = this.GetSkillDesc(bsData.Skill2Button);
                this.skillDesc4 = this.GetSkillDesc(bsData.SpecialSkillButton);
            }
        }

        private void SetSkillIcon(UIAtlas atlas, string spriteName, UISprite sp)
        {
            if (sp == null)
                return;

            sp.atlas = atlas;
            sp.spriteName = spriteName;

            if (sp.GetAtlasSprite() == null)
            {
                if (atlas != null && !string.IsNullOrEmpty(spriteName))
                {
                    Debug.LogWarning(string.Format(
                        "SetIconSprite:\r\n" +
                        "Sprite Not Found!! AvatarType = {0} SpriteName = {1}", 1, spriteName));//this.CharaInfo.AvatarType
                }
            }
        }

        private string GetSkillDesc(CharaButtonMasterData skilBtn)
        {
            var result = CharaButtonDescMaster.Instance.GetByCharaButtonId(skilBtn.ID);
            if(result == null)
            {
                Debug.LogError("更新Master数据！");
                return "";
            }
            return result.Desc;
        }

        private void RegisterSkillTouchEvent()
        {
            UIEventListener.Get(this.view.detail.detailJS.skill_1.gameObject).onPress = this.OnSkill1Press;
            UIEventListener.Get(this.view.detail.detailJS.skill_2.gameObject).onPress = this.OnSkill2Press;
            UIEventListener.Get(this.view.detail.detailJS.skill_3.gameObject).onPress = this.OnSkill3Press;
            UIEventListener.Get(this.view.detail.detailJS.skill_4.gameObject).onPress = this.OnSkill4Press;
        }

        private void OnSkillPress(bool isDown, Action func)
        {
            if(isDown)
            {
                //Show Desc
                if (this.view.detail.detailJS.skillDesc.activeSelf == false)
                {
                    this.view.detail.detailJS.skillDesc.SetActive(true);
                    //Set text
                    func();
                }
            }
            else
            {
                //Hide
                this.view.detail.detailJS.skillDesc.SetActive(false);
            }
            
        }      

        private void OnSkill1Press(GameObject obj, bool isDown)
        {
            this.OnSkillPress(isDown, () => {
                this.view.detail.detailJS.lblSkillDesc.text = this.skillDesc1;
                this.view.detail.detailJS.lblSkillName.text = this.skillName1;
            });
        }

        private void OnSkill2Press(GameObject obj, bool isDown)
        {
            this.OnSkillPress(isDown, () => {
                this.view.detail.detailJS.lblSkillDesc.text = this.skillDesc2;
                this.view.detail.detailJS.lblSkillName.text = this.skillName2;
            });
        }

        private void OnSkill3Press(GameObject obj, bool isDown)
        {
            this.OnSkillPress(isDown, () => {
                this.view.detail.detailJS.lblSkillDesc.text = this.skillDesc3;
                this.view.detail.detailJS.lblSkillName.text = this.skillName3;
            });
        }

        private void OnSkill4Press(GameObject obj, bool isDown)
        {
            this.OnSkillPress(isDown, () => {
                this.view.detail.detailJS.lblSkillDesc.text = this.skillDesc4;
                this.view.detail.detailJS.lblSkillName.text = this.skillName4;
            });
        }
        #endregion
        #endregion

        #region DQ
        private void SetDQ()
        {
            this.SetDQItem();
            //this.GetEnergy();
        }

        private void SetDQItem()
        {
            for (int i = 0; i < this.view.dq.items.Count; ++i)
            {
                this.view.dq.items[i].FillItem(this.dataDQ.ProductArray[i]);
            }
        }

        private void SetDQEnergy(object s, EventArgs e)
        {
            this.view.dq.lblEnergy.text = XDATA.PlayerData.Instance.Energy + " / " + XDATA.PlayerData.Instance.MaxEnergy;
        }
        #endregion

        #region LB
        public CustomControl.ScrollPage<ItemShopLB> lbPage;
        private void SetLB()
        {
            if (this.lbPage == null)
            {
                this.lbPage = new CustomControl.ScrollPage<ItemShopLB>(this.view.lb.container);
                this.lbPage.movX = 1024;
            }

            StartCoroutine(this.GetActivityTexture());
        }

        private IEnumerator GetActivityTexture()
        {
            var activityData = SaleEventMaster.Instance.GetLatest(System.DateTime.Now);

            string[] datas = activityData.ImgPath.Split(';');
            yield return this.lbPage.Create(this.view.lb.imgParent, datas);
            this.lbPage.SelectHalf(1);

            this.SetLbExpired(activityData.End);
        }

        private void SetLbExpired(System.DateTime endTime)
        {
            var now = System.DateTime.Now;
            Debug.Log("End: " + endTime);
            Debug.Log("Now: " + now);
            this.SetLbBtnStatus(endTime.CompareTo(now) != -1);
        }

        private void SetLbBtnStatus(bool isEnable)
        {
            var btn = this.view.lb.btnGo;
            btn.enabled = isEnable;

            if (isEnable)
            {
                btn.GetComponentInChildren<UILabel>().text = "购买";
                btn.SetState(UIButton.State.Normal, true);
            }
            else
            {
                btn.GetComponentInChildren<UILabel>().text = "过期";
                btn.SetState(UIButton.State.Disabled, true);
            }
        }
        #endregion

        #region Net
        private IEnumerator GetData()
        {
            yield return this.NetReq(this.GetDataXD);
            yield return this.NetReq(this.GetDataPF);
            yield return this.NetReq(this.GetDataJS);
            yield return this.NetReq(this.GetDataCZ);
            yield return this.NetReq(this.GetDataDQ);
            yield return this.NetReq(this.GetDataLB);

            this.isGetData = true;
        }

        private void GetDataXD()
        {
            AsobimoWebAPI.Instance.GetShopList<ActivityProductList>((v) => {
                if (v == null)
                {
                    Debug.LogError("DataXD == null;");
                    return;
                }
                dataXD = v.Result;
            
                this.isLoaded = true;
            });
        }

        private void GetDataPF()
        {
            AsobimoWebAPI.Instance.GetProductStatusList<ProductStatusList>("", "3", "", 0, (v) => {
                if (v == null)
                {
                    Debug.LogError("DataPF == null;");
                    return;
                }
                this.dataPF = v.Result;
                this.PackPFList(v.Result.ProductArray);
                this.isLoaded = true;
            });
        }

        private void PackPFList(ProductStatus[] arr)
        {
            for(int i = 0; i < arr.Length; i++)
            {
                ShopItemMasterData sData;
                MasterData.TryGetShop(arr[i].GameID, out sData);
                AvatarMasterData aData;
                MasterData.TryGetAvatar(sData.CharacterId, sData.AvatarId, out aData);

                var data = new ItemPFData();
                data.info = arr[i];
                data.isHas = false;
                data.avatarId = aData.ID;
                this.shopPFDataList.Add(data);
            }
        }

        private void GetDataJS()
        {
            AsobimoWebAPI.Instance.GetProductStatusList<ProductStatusList>("", "2", "", 0, (v) => {
                if (v == null)
                {
                    Debug.LogError("DataJS == null;");
                    return;
                }
                this.dataJS = v.Result;
                this.isLoaded = true;
            });
        }

        private void GetDataCZ()
        {
            AsobimoWebAPI.Instance.GetTicketList<TicketList>((v) => {
                if (v.Result == null)
                {
                    Debug.LogError("DataCZ == null;");
                    return;
                }
                this.dataCZ = v.Result;
                this.isLoaded = true;
            });
        }

        private void GetDataDQ()
        {
            AsobimoWebAPI.Instance.GetProductStatusList<ProductStatusList>("", "4", "", 0, (v) => {
                if (v.Result == null)
                {
                    Debug.LogError("DataDQ == null;");
                    return;
                }
                this.dataDQ = v.Result;
                this.isLoaded = true;
            });
        }

        private void GetDataLB()
        {
            AsobimoWebAPI.Instance.GetProductStatusList<ProductStatusList>("", "5", "", 0, (v) => {
                if (v.Result == null)
                {
                    Debug.LogError("DataLB == null;");
                    return;
                }
                this.dataLB = v.Result;
                this.isLoaded = true;
            });
        }

        private IEnumerator SetPFIsHas()
        {
            Debug.Log("Shop avatarIds: " + this.GetShopPFIds());
            yield return Net.Network.GetClashAvatarWithShopProductPF(this.GetShopPFIds(), (res) => {
                var ids = res.GetObtainedCharacterAvatarParameter();
                this._SetPFIsHas(ids);
            });
        }

        private void _SetPFIsHas(Scm.Common.Packet.ObtainedCharacterAvatarParameter[] arr)
        {
            for(int i = 0; i < arr.Length; i++)
            {
                Debug.Log("Clash avatarId: " + arr[i].Id);
            }
            for(int i = 0; i < this.shopPFDataList.Count; i++)
            {
                for(int j = 0; j < arr.Length; j++)
                {
                    if (this.shopPFDataList[i].avatarId == arr[j].Id)
                        this.shopPFDataList[i].isHas = true;
                }
            }
        }

        private int[] GetShopPFIds()
        {
            var arr = new List<int>();
            for(int i = 0; i < this.shopPFDataList.Count; i++)
            {
                arr.Add(this.shopPFDataList[i].avatarId);
            }
            return arr.ToArray();
        }

        private IEnumerator NetReq(Action func)
        {
            this.isLoaded = false;            
            LoadingIconController.Instance.Show();
            
            func();

            while (!this.isLoaded)
                yield return null;

            LoadingIconController.Instance.Hide();
        }

        public IEnumerator Buy(string productId, int num, Action callback)
        {
            Debug.Log("Buy---->ProductId: " + productId);
            bool flag = false;
            bool isSuccess = false;
            LoadingIconController.Instance.Show();

            AsobimoWebAPI.Instance.BuyProduct<PurchaseProduct>(productId, num, (res) => {
                if(res.Result.ProductObtainCode == ObtainCode.PointShortage)
                {
                    GUITipMessage.Instance.Show("点券不够!");
                    flag = true;
                    return;
                }
                if (res.Result.ProductObtainCode == ObtainCode.Completion)
                {
                    isSuccess = true;
                    Net.Network.Instance.StartCoroutine(Net.Network.AfterBuy((v) => {
                        flag = true;
                        Debug.Log("AfterBuy" + v.Result);
                    }));
                }

            });

            while (!flag)
                yield return null;

            LoadingIconController.Instance.Hide();

            if (isSuccess)
            {
                callback();
            }
            //else
            //    GUITipMessage.Instance.Show("购买失败！");
        }

        public IEnumerator InitXD()
        {
            bool flag = false;
            LoadingIconController.Instance.Show();
            AsobimoWebAPI.Instance.ActivityInit<ActivityProductInit>((v) => {
                //Updata data
                this.dataXD.product_obtain_code = v.Result.product_obtain_code;
                this.dataXD.purchase_next_price = int.Parse(v.Result.currentPrice);

                //Set Status
                this.SetXDStatus();

                //Hide
                for (int i = 0; i < this.view.xd.items.Count; ++i)
                {
                    this.view.xd.items[i].IsShow = false;
                }
                flag = true;
            });

            while (!flag)
                yield return null;
            LoadingIconController.Instance.Hide();
        }

        private void GetDataResetXD()
        {
            this.isReset = false;
            AsobimoWebAPI.Instance.ActivityReset<ActivityProductReset>((res) => {
                if(res.Result.result == 0)
                {
                    GUITipMessage.Instance.Show("复位失败！");
                    this.isLoaded = true;
                    return;
                }
                else
                {
                    this.isReset = true;
                    XDATA.PlayerData.Instance.Coin = res.Result.coin;
                    this.isLoaded = true;
                }
            });
        }

        private IEnumerator ResetXD()
        {
            yield return this.NetReq(this.GetDataResetXD);

            if(this.isReset == true)
            {
                yield return this.NetReq(this.GetDataXD);
                this.SetXD();
            }
        }

        private IEnumerator BuyXD()
        {
            bool flag = false;
            LoadingIconController.Instance.Show();
            AsobimoWebAPI.Instance.ActivityBuy<ActivityBuy>((v) => {
                flag = true;

                //I can ignore the status about no money, i had filter. 

                //Update data
                this.dataXD.product_obtain_code = v.Result.product_obtain_code;
                this.dataXD.purchase_next_price = v.Result.purchase_next_price;
                this.dataXD.reset_coin = v.Result.reset_coin;
                Debug.Log("ResetCoin: " + v.Result.reset_coin);
                var index = v.Result.product_id_index;
                this.dataXD.product[index].buyed = "1";

                //Set Status
                if (!this.SetXDStatus())
                    return;

                //Show XDItem
                this.view.xd.items[index].IsShow = true;
                Net.Network.Instance.StartCoroutine(Net.Network.AfterBuy((res) => {
                    Debug.Log(res.Result);
                }));
            });

            while (!flag)
                yield return null;

            LoadingIconController.Instance.Hide();
        }

        private IEnumerator GetEnergy()
        {
            yield return XDATA.PlayerData.Instance.GetEnergy();
        }

        public enum IosBuySpendLocal
        {
            coin10 = 0,
            coin60,
            coin425,
            coin725,
            coin1245,
            coin2100,
            coin3690,
            coin6868,
        }

        public IEnumerator Paypre(string productId, int price, int payType)
        {
            Debug.Log("Paypre->productId: " + productId + "price:" + price);
            NetworkController.IsForceService = true;
#if UNITY_ANDROID
            bool flag = false;
            LoadingIconController.Instance.Show();
            AsobimoWebAPI.Instance.Pay<PaypreRes>(productId, (v) => {
                if (v.Result == null)
                {
                    Debug.Log("Result: " + v.Result);
                    return;
                }
                Debug.Log("Result: " + v.Result.result);
                Debug.Log("OrderID: " + v.Result.order_id);
                flag = true;
                APaymentHelperDemo.Instance.Pay(1, "dianquan", price, v.Result.order_id + ";" + productId);
            });

            while (!flag)
                yield return null;
            LoadingIconController.Instance.Hide();

#elif UNITY_IOS
            
            //payType == IosBuySpendLocal
            //public enum IosBuySpendLocal
            //{
            //    coin10 = 0,
            //    coin60,
            //    coin425,
            //    coin725,
            //    coin1245,
            //    coin2100,
            //    coin3690,
            //    coin6868,
            //}
            IAPBuy.Instance.Buy(payType, productId, 
					(TransationID, Receipt)=>{
                    Debug.Log("Result>TransationID:" + TransationID + "Receipt:" + Receipt);
                    //var receiptBase64 = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Receipt as string));
                        var data = new PurchaseData(
                            new ProductData(productId, PurchaseItemType.Inapp, "1", price.ToString(), price, "1", "ticket01", "1"), 
                            TransationID,
                            Receipt,
                            ""
                            );
                        PurchaseManager.Instance.OnPurchased(PurchaseState.Complete, data, ()=>{
                            Net.Network.Instance.StartCoroutine(Net.Network.AfterBuy((v) => {
                            Debug.Log("AfterBuy" + v.Result);
                    }));
                        });
                        
					},
					(FailureReason)=>{
                        Debug.Log("Result>FailureReason:" + FailureReason);
					}
				);
            yield return null;
#else
            yield break;
#endif
        }
        #endregion

        #region Buy
        private void BuyPF(ItemShopPF itemPF)
        {
            if (XDATA.PlayerData.Instance.Coin < itemPF.data.info.Price)
            {
                GUITipMessage.Instance.Show("点券不够！");
                this.GoTo(this.view.pf.root, this.view.detail.root);
                return;
            }
            Net.Network.Instance.StartCoroutine(this.Buy(itemPF.data.info.ProductID.ToString(), 1, () => {
                GUITipMessage.Instance.Show("购买成功！");
                this.GoTo(this.view.pf.root, this.view.detail.root);
                itemPF.effect.Play();
                itemPF.data.isHas = true;
                itemPF.SetIsHas();
            }));
        }

        private void BuyJS(int price, string productId, ParticleSystem effect)
        {
            if (XDATA.PlayerData.Instance.Coin < price)
            {
                GUITipMessage.Instance.Show("点券不够！");
                this.GoTo(this.view.js.root, this.view.detail.root);
                return;
            }
            Net.Network.Instance.StartCoroutine(this.Buy(productId, 1, () => {
                this.GoTo(this.view.js.root, this.view.detail.root);
                effect.Play();
            }));
        }

        public void BuyDQ(int price, string  productId, ParticleSystem effect)
        {
            if(XDATA.PlayerData.Instance.Coin < price)
            {
                GUITipMessage.Instance.Show("点券不够！");
                return;
            }
            Net.Network.Instance.StartCoroutine(this.Buy(productId, 1, () => {
                effect.Play();
            }));
        }

        public void BuyLB()
        {
            if (XDATA.PlayerData.Instance.Coin < this.dataLB.ProductArray[0].Price)
            {
                GUITipMessage.Instance.Show("点券不够！");
                return;
            }
            Net.Network.Instance.StartCoroutine(this.Buy(this.dataLB.ProductArray[0].ProductID.ToString(), 1, ()=>{
                GUITipMessage.Instance.Show("购买成功！");
                }));
        }
        #endregion
    }

}
