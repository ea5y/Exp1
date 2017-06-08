using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Scm.Common.GameParameter;
using Scm.Common.Master;
using Scm.Common.XwMaster;

namespace XUI
{
    public struct ProductShowData
    {
        public int characterId;
        public int skinId;
        public ShopItemType type;
        public ShopItemTag tag;

        public string desc;
        public int expire; 
        public string iconStr;

        public int price;
        public string tip;
    };

    public enum ProdectType
    {
        Gold,
        Coin,
        Energy,
        Character,
        Skin
    }

    public class GUIProductsWindow : Singleton<GUIProductsWindow>
    {
        private Action OkFunc;
        private Action CancelFunc;

        public UILabel lblTitle;
        public UILabel lblTip;
        public UIButton btnOk_1;
        public UIButton btnOk_2;
        public UIButton btnCancel;
        
        public GameObject productsGroup;
        public GameObject itemProduct_1;
        public GameObject itemProduct_2;

        public GameObject btnGroup_1;
        public GameObject btnGroup_2;

        private void Awake()
        {
            base.Awake();
            this.RegisterEventOnce();
            this.HideSelf();
        }

        private void HideSelf()
        {
            this.gameObject.SetActive(false);
        }

        private void OpenSelf()
        {
            this.gameObject.SetActive(true);
        }

        private void RegisterEventOnce()
        {
            EventDelegate.Add(this.btnOk_1.onClick, () =>{
                    this.OkFunc();
                    this.OkFunc = null;
                    });
            EventDelegate.Add(this.btnOk_2.onClick, () =>{
                    this.OkFunc();
                    this.OkFunc = null;
                    });
            EventDelegate.Add(this.btnCancel.onClick, () =>{
                    this.HideSelf();
                    });
        }

        #region ShopItem专用
        public void OpenOk(int price, List<int> gameIds, GameObject root)
        {
            var preRoot = root;
            if (preRoot != null)
            {
                preRoot.SetActive(false);
                this.OkFunc = () => { this.HideSelf(); preRoot.SetActive(true); };
            }
            else
                this.OkFunc = () => { this.HideSelf(); };

            this.SetTip(price);
            this.SetBtnNum(1);
            this.AddProduct(gameIds);

            this.OpenSelf();
        }

        public void OpenOkNo(int price, List<int> gameIds, Action okFunc)
        {
            this.SetTip(price);
            this.SetBtnNum(2);

            this.AddProduct(gameIds);

            this.OkFunc = () => { okFunc(); this.HideSelf(); };

            this.OpenSelf();
        }

        private void SetBtnNum(int n)
        {
            this.btnGroup_1.SetActive(n == 1);
            this.btnGroup_2.SetActive(n == 2);
        }

        private void SetTip(int price)
        {
            if(price == 0)
            {
                this.lblTip.text = "恭喜获得：";
            }
            else
            {
                this.lblTip.text = "是否花费 " + price + " 点券够买？";
            }
        }

        private void SetTip(string tipStr)
        {
            this.lblTip.text = tipStr;
        }

        private void AddProduct(List<int> gameIds)
        {
            //NGUITools.DestroyChildren(this.productsGroup.transform);
            this.productsGroup.DestroyChildImmediate();
            for (int i = 0; i < gameIds.Count; i++)
            {
                ShopItemMasterData sData;
                MasterData.TryGetShop(gameIds[i], out sData);

                if(sData.ShopItemType == ShopItemType.Avatar || sData.ShopItemType == ShopItemType.Character)
                {
                    this.AddItem(itemProduct_1, gameIds[i]);
                }
                else
                {
                    this.AddItem(itemProduct_2, gameIds[i]);
                }
            }

            //response
            CustomControl.ToolFunc.ResponseGrid(this.productsGroup, 190);
        }

        private void AddItem(GameObject itemProduct, int gameId)
        {
            //1. add 
            var go = NGUITools.AddChild(this.productsGroup, itemProduct);
            go.SetActive(true);
            
            //2. fill
            var data = this.PackProductData(gameId);
            CustomControl.ToolFunc.IGridItem item = go.GetComponent<CustomControl.ToolFunc.IGridItem>();
            item.Fill(data);
        }

        public ProductShowData PackProductData(int ids)
        {
                ProductShowData data = new ProductShowData();
                ShopItemMasterData sDataTemp;
                MasterData.TryGetShop(ids, out sDataTemp);

                data.characterId = sDataTemp.CharacterId;
                data.skinId = sDataTemp.AvatarId;
                data.type = sDataTemp.ShopItemType;
                data.tag = sDataTemp.ShopItemTag;
                data.desc = sDataTemp.Description;
                data.expire = sDataTemp.Expire;
                data.iconStr = sDataTemp.Icon;
                return data;
        }
        #endregion

        #region 通用（数据自己打包）
        public void OpenOK(ProductShowData data, GameObject root)
        {
            var preRoot = root;
            if (preRoot != null)
            {
                preRoot.SetActive(false);
                this.OkFunc = () => { this.HideSelf(); preRoot.SetActive(true); };
            }
            else
                this.OkFunc = () => { this.HideSelf(); };

            this.SetTip(data.price);
            this.SetBtnNum(1);
            this.AddProduct(data);

            this.OpenSelf();
        }

        public void OpenOkNo(ProductShowData data, Action okFunc)
        {
            this.SetTip(data.tip);
            
            this.SetBtnNum(2);

            this.AddProduct(data);

            this.OkFunc = () => { okFunc(); this.HideSelf(); };

            this.OpenSelf();
        }

        private void AddProduct(ProductShowData data)
        {
            //NGUITools.DestroyChildren(this.productsGroup.transform);
            this.productsGroup.DestroyChildImmediate();
            if (data.type == ShopItemType.Avatar || data.type == ShopItemType.Character)
            {
                this.AddItem(this.itemProduct_1, data);
            }
            else
            {
                this.AddItem(this.itemProduct_2, data);
            }
            //response
            CustomControl.ToolFunc.ResponseGrid(this.productsGroup, 190);
        }

        private void AddItem(GameObject itemProduct, ProductShowData data)
        {
            //1. add 
            var go = NGUITools.AddChild(this.productsGroup, itemProduct);
            go.SetActive(true);

            //2. fill
            CustomControl.ToolFunc.IGridItem item = go.GetComponent<CustomControl.ToolFunc.IGridItem>();
            item.Fill(data);
        }
        #endregion
    }
}

