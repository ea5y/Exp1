using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

namespace XUI
{
    public class ItemProduct_2_1 : MonoBehaviour, CustomControl.ToolFunc.IGridItem 
    {
        public UISprite imgIcon;
        public UILabel lblDesc;

        private ProductShowData data;
        public void Fill(object data)
        {
            this.data = (ProductShowData)data;

            if(this.data.type == ShopItemType.Gold)
            {
                this.SetGold();
            }

            if(this.data.type == ShopItemType.Energy)
            {
                this.SetEnergy();
            }

            if(this.data.type == ShopItemType.Ticket)
            {
                this.SetCoin();
            }
        }

        private void SetGold()
        {
            this.SetGoldIcon();
            this.SetGoldDesc();
        }

        private void SetGoldIcon()
        {
            if(this.data.iconStr == "gold1")
            {
                this.imgIcon.spriteName = "gold1";
                this.SetImgSize(170, 142);
            }
            else if(this.data.iconStr == "gold2")
            {
                this.imgIcon.spriteName = "gold2";
                this.SetImgSize(170, 142);
            }
            else if(this.data.iconStr == "gold3")
            {
                this.imgIcon.spriteName = "gold3";
                this.SetImgSize(170, 142);
            }
            else if(this.data.iconStr == "gold4")
            {
                this.imgIcon.spriteName = "gold4";
                this.SetImgSize(183, 127);
            }
        }

        private void SetImgSize(int w, int h)
        {
            this.imgIcon.width = w;
            this.imgIcon.height = h;
        }

        private void SetGoldDesc()
        {
            this.lblDesc.text = this.data.desc;
        }

        private void SetEnergy()
        {
            this.SetEnergyIcon();
            this.SetEnergyDesc();
        }

        private void SetEnergyIcon()
        {
            this.imgIcon.spriteName = "tili";
            this.SetImgSize(30, 50);
        }

        private void SetEnergyDesc()
        {
            this.lblDesc.text = this.data.desc;
        }

        private void SetCoin()
        {
            this.SetCoinIcon();
            this.SetCoinDesc();
        }

        private void SetCoinIcon()
        {
            this.imgIcon.spriteName = "coin2";
        }

        private void SetCoinDesc()
        {
            this.lblDesc.text = this.data.desc;
        }
    }
}

