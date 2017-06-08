using UnityEngine;
using System.Collections;
using Asobimo.WebAPI;
using Scm.Common.Master;
namespace XUI
{
    public class ItemShopCZ : MonoBehaviour 
    {
        public UILabel nameCZ;
        public UILabel price;
        public UILabel zheKou;

        public UIButton btnGo;
        public UILabel extraCount;
            
        private TicketItem data;
        private int index;
        public void FillItem(TicketItem data, int index)
        {
            this.data = data;
            this.index = index;
            this.price.text = "¥" + this.data.price_total;

            this.zheKou.text = "八折";
            var discountData = ChargeDiscountMaster.Instance.GetChargeDiscountByPaymentId(data.onetime_payment_id);
            if (discountData != null && discountData.ExtraCount > 0) {
                this.extraCount.text = "另赠" + discountData.ExtraCount.ToString() + "点券";
            } else {
                this.extraCount.text = string.Empty;
            }
            this.nameCZ.text = this.data.point - discountData.ExtraCount + "点券";

            this.btnGo.onClick.Clear();
            EventDelegate.Add(this.btnGo.onClick, this.OnBtnGoClick);
        }

        private void OnBtnGoClick()
        {
            int yijiePrice = Mathf.CeilToInt(100*data.price_total);
            Debug.Log("===> Buy price_total:" + this.data.price_total + ",count:" + yijiePrice);
            
#if ANDROID_XY
            NetworkController.IsForceService = true;
            if(AuthEntry.Instance.AuthMethod != null)
            {
                Debug.Log("ProductId: " + this.data.product_id);
                AuthEntry.Instance.AuthMethod.Purchase(this.data.onetime_payment_type_code);
            }

#else
            Net.Network.Instance.StartCoroutine(GUIShop.Instance.Paypre(this.data.onetime_payment_type_code, yijiePrice, index));
#endif
        }
    }
}

