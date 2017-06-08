using UnityEngine;
using System.Collections;
using Asobimo.WebAPI;
using Scm.Common.XwMaster;
using Scm.Common.GameParameter;
using XDATA;
using System.Collections.Generic;

namespace XUI
{
    public class ItemShopDQ : MonoBehaviour 
    {
        public UILabel nName;
        public UILabel price;
        public UILabel zheKou;
        public UIButton btnGo;
        public ParticleSystem effect;

        private ProductStatus data;

        public void FillItem(ProductStatus data)
        {
            this.data = data;

            ShopItemMasterData sData;
            MasterData.TryGetShop(this.data.GameID, out sData);
            if(null == data)
            {
                Debug.LogError("===> MasterData can not find " + this.data.GameID);
                return;
            }
            
            this.price.text = this.data.Price + "";
            this.nName.text = sData.Description;

            this.btnGo.onClick.Clear();
            EventDelegate.Add(this.btnGo.onClick, this.OnBtnGoClick);
        }

        private void OnBtnGoClick()
        {
            Debug.Log("===> Buy");
            /*
            GUIShop.Instance.ShowTip(this.data.Price, ()=>{
                if (PlayerData.Instance.Coin < this.data.Price)
                {
                    GUITipMessage.Instance.Show("点券不够！");
                    return;
                }
                Net.Network.Instance.StartCoroutine(GUIShop.Instance.Buy(this.data.ProductID.ToString(), 1, ()=>{
                                this.PlayEffect();
                                }));
                    });
                    */

            GUIProductsWindow.Instance.OpenOkNo(this.data.Price, new List<int>() { this.data.GameID }, () => {
                    GUIShop.Instance.BuyDQ(this.data.Price, this.data.ProductID.ToString(), this.effect);
            });
            /*
            GUIShop.Instance.ShowTip(this.data.Price, 
                () => 
                {
                    GUIShop.Instance.ShowDQTip(this.data.GameID);
                }, 
                () => 
                {
                    GUIShop.Instance.BuyDQ(this.data.Price, this.data.ProductID.ToString(), this.effect);
                });
                */
        }

        private void PlayEffect()
        {
            this.effect.Play();
        }
    }
}

