using UnityEngine;
using System.Collections;
using Scm.Common.GameParameter;

namespace XUI.Shop
{
    public class ItemProduct_2 : MonoBehaviour, CustomControl.ToolFunc.IGridItem 
    {
        public UISprite imgIcon;
        public UILabel lblDesc;

        private GoodsTipData data;
        public void Fill(object data)
        {
            this.data = (GoodsTipData)data;

            if(this.data.type == ShopItemType.Gold)
            {
                this.SetGold();
            }

            if(this.data.type == ShopItemType.Energy)
            {
                this.SetEnergy();
            }
        }

        private void SetGold()
        {
            this.SetGoldIcon();
            this.SetGoldDesc();
        }

        private void SetGoldIcon()
        {
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
        }

        private void SetEnergyDesc()
        {
            this.lblDesc.text = this.data.desc;
        }
    }
}

