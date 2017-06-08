using UnityEngine;
using System.Collections;
using Asobimo.WebAPI;
using Scm.Common.Master;
using Scm.Common.XwMaster;
using Scm.Common.GameParameter;

namespace XUI
{
    public class ItemShopPF : MonoBehaviour
    {
        public UISprite icon;
        public UILabel namePF;
        public UILabel coin;
        public UIButton btnGo;
        public ParticleSystem effect;

        public UISprite tag_1;
        public UISprite tag_2;
        public UISprite tag_2_num;

        private enum TagType
        {
            None,
            Tag1,
            Tag2
        }

        public GUIShop.ItemPFData data;
        public ShopItemMasterData sData;
        public AvatarMasterData aData;
        private CharaIcon CharaIcon{get{return ScmParam.Battle.CharaIcon;}}

        public void FillItem(GUIShop.ItemPFData data)
        {
            this.data = data;

            ShopItemMasterData sData;
            MasterData.TryGetShop(this.data.info.GameID, out sData);
            
            AvatarMasterData aData;
            MasterData.TryGetAvatar(sData.CharacterId, sData.AvatarId, out aData);

            this.SetTransformData(sData, aData);
            this.SetFrame(sData);
            this.SetTag(sData);
            this.SetCoin();
            this.SetName(sData);
            this.SetBtnEvent();
            this.SetIcon(aData);
            this.SetIsHas();
        }

        public void SetIsHas()
        {
            this.btnGo.enabled = !this.data.isHas;
            this.SetIconGery(this.data.isHas);
            if(this.data.isHas)
            {
                this.btnGo.SetState(UIButton.State.Disabled, true);
            }
            else
            {
                this.btnGo.SetState(UIButton.State.Normal, true);
            }
        }

        private void SetIconGery(bool isGery)
        {
            this.icon.color = isGery ? Color.grey : Color.white;
        }

        private void SetTransformData(ShopItemMasterData sData, AvatarMasterData aData)
        {
            this.sData = sData;
            this.aData = aData;
        }

        private void SetCoin()
        {
            this.coin.text = this.data.info.Price.ToString();
        }

        private void SetName(ShopItemMasterData sData)
        {
            this.namePF.text = sData.Description;
        }

        private void SetBtnEvent()
        {
            this.btnGo.onClick.Clear();
            EventDelegate.Add(this.btnGo.onClick, this.OnBtnGoClick);
        }

        private void SetIcon(AvatarMasterData aData)
        {
            CharaIcon.GetBustIcon((AvatarType)aData.CharacterId, aData.ID, false, (a, s) => {
                this.icon.atlas = a;
                this.icon.spriteName = s;
            });
        }

        private void SetFrame(ShopItemMasterData sData)
        {
            var sp = this.btnGo.tweenTarget.GetComponent<UISprite>();
            if(sData.ShopItemTag == Scm.Common.GameParameter.ShopItemTag.Light)
            {
                sp.spriteName = "kuang_b01";
                sp.width = 150;
                sp.height = 172;
                sp.transform.localPosition = new Vector3(6, 0, 0);
                this.btnGo.normalSprite = "kuang_b01";
                this.btnGo.pressedSprite = "kuang_y01";
            }
            else
            {

                sp.spriteName = "lankuang";
                sp.width = 142;
                sp.height = 170;
                sp.transform.localPosition = new Vector3(0, 0, 0);
                this.btnGo.normalSprite = "lankuang";
                this.btnGo.pressedSprite = "huangkuang";
            }
        }

        private void SetTag(ShopItemMasterData sData)
        {
            var tag = this.tag_1.parent.gameObject;
            switch(sData.ShopItemTag)
            {
                case ShopItemTag.None:
                    this.ShowTag(TagType.None);
                    break;
                case ShopItemTag.Light:
                    this.ShowTag(TagType.Tag1);
                    this.tag_1.spriteName = "b_shanguang";
                    break;
                case ShopItemTag.Hot:
                    this.ShowTag(TagType.Tag1);
                    this.tag_1.spriteName = "b_hot";
                    break;
                case ShopItemTag.Special:
                    this.ShowTag(TagType.Tag1);
                    this.tag_1.spriteName = "b_teshu"; 
                    break;
                case ShopItemTag.Trial:
                    this.ShowTag(TagType.Tag1);
                    this.tag_1.spriteName = "b_tiyan";
                    break;
                case ShopItemTag.Off5:
                    this.ShowTag(TagType.Tag2);
                    this.tag_2_num.spriteName = "5";
                    break;
                case ShopItemTag.Off6:
                    this.ShowTag(TagType.Tag2);
                    this.tag_2_num.spriteName = "6";
                    break;
                case ShopItemTag.Off7:
                    this.ShowTag(TagType.Tag2);
                    this.tag_2_num.spriteName = "7";
                    break;
                case ShopItemTag.Off8:
                    this.ShowTag(TagType.Tag2);
                    this.tag_2_num.spriteName = "8";
                    break;
                case ShopItemTag.Off9:
                    this.ShowTag(TagType.Tag2);
                    this.tag_2_num.spriteName = "9";
                    break;
            }
        }

        private void ShowTag(TagType type)
        {
            switch(type)
            {
                case TagType.None:
                    tag_1.gameObject.SetActive(false);
                    tag_2.gameObject.SetActive(false);
                    break;
                case TagType.Tag1:
                    tag_1.gameObject.SetActive(true);
                    tag_2.gameObject.SetActive(false);
                    break;
                case TagType.Tag2:
                    tag_1.gameObject.SetActive(false);
                    tag_2.gameObject.SetActive(true);
                    break;
            }
        }

        private void OnBtnGoClick()
        {
            Debug.Log("===> Go to avatar");
            GUIShop.Instance.SetDetailPF(this);
        }

        private void Test()
        {
            GUIShop.Instance.OpenToCZ();
        }
    }
}
