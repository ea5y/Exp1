using UnityEngine;
using System.Collections;
using Scm.Common.XwMaster;

namespace XUI
{
    public class GUIGift: Singleton<GUIGift> 
    {
        public UILabel lblText;
        public UIButton btnGo;
        public UIButton btnClose;

        void Awake()
        {
            base.Awake();
            this.RegisterEventOnce();
            this.HideFirst();
        }

        private void RegisterEventOnce()
        {
            EventDelegate.Add(this.btnGo.onClick, this.OnBtnGoClick);
            EventDelegate.Add(this.btnClose.onClick, this.Close);
        }

        private void HideFirst()
        {
            this.gameObject.SetActive(false);
        }

        private void OnBtnGoClick()
        {
            var str = this.lblText.text;
            str = str.Trim();
            if (string.IsNullOrEmpty(str) || str == "请输入兑换码")
                return;
            Net.Network.Instance.StartCoroutine(Net.Network.GetGift(str, (res) => {
                        Debug.Log("GetGift: " + res);
                        if (res.Result == Scm.Common.ReturnCode.NotFound)
                        {
                            GUITipMessage.Instance.Show("兑换码无效！");
                            return;
                        }
                        if (res.Result == Scm.Common.ReturnCode.Overflow)
                        {
                            GUITipMessage.Instance.Show("你已经兑换过了! ");
                            return;
                        }
                        if (res.Result == Scm.Common.ReturnCode.Expired)
                        {
                            GUITipMessage.Instance.Show("兑换码已过期！");
                            return;
                        }
                        

                        ShopItemMasterData sData;
                        MasterData.TryGetShop(res.ShopId, out sData);
                        if (sData == null)
                        {
                            Debug.LogError("更新MasterData!");
                            return;
                        }
                        GUIProductsWindow.Instance.OpenOk(0, sData.PackageIds, this.gameObject);                    
                        }));
        }

        public void Open()
        {
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}

