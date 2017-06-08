using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Asobimo.WebAPI;
using Scm.Common.Master;
using Scm.Common.XwMaster;

namespace XUI
{
    public class ItemScrollShopPF : CustomControl.ScrollViewItem
    {
        public List<ItemShopPF> cellPFList;

        private List<GUIShop.ItemPFData> datas;
        public override void FillItem(IList datas, int index)
        {
            base.FillItem(datas, index);
            this.datas = (List<GUIShop.ItemPFData>)datas[index];
            for(int i = 0; i < cellPFList.Count; i++)
            {
                if(i < this.datas.Count)
                {
                    this.cellPFList[i].gameObject.SetActive(true);
                    this.cellPFList[i].FillItem(this.datas[i]);
                }
                else
                {
                    this.cellPFList[i].gameObject.SetActive(false);
                }
            }
        }
    }
}

