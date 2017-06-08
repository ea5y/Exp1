using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Asobimo.WebAPI;
using Scm.Common.Master;
using Scm.Common.XwMaster;

namespace XUI
{
    public class ItemScrollShopJS : CustomControl.ScrollViewItem
    {
        public List<ItemShopJS> cellJSList;

        private List<ProductStatus> datas;
        public override void FillItem(IList datas, int index)
        {
            base.FillItem(datas, index);
            this.datas = (List<ProductStatus>)datas[index];
            for(int i = 0; i < cellJSList.Count; i++)
            {
                if(i < this.datas.Count)
                {
                    this.cellJSList[i].gameObject.SetActive(true);
                    this.cellJSList[i].FillItem(this.datas[i]);
                }
                else
                {
                    this.cellJSList[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
