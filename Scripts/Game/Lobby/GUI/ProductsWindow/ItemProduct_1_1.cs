using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scm.Common.GameParameter;
using Scm.Common.Master;

namespace XUI
{
    public class ItemProduct_1_1 : MonoBehaviour, CustomControl.ToolFunc.IGridItem 
    {
        public UISprite imgIcon;
        public UISprite imgFrame;
        public UILabel lblName;

        public ProductShowData data;
        private CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }

        public void Fill(object data)
        {
            this.data = (ProductShowData)data;

            if(this.data.type == ShopItemType.Avatar)
            {
                this.SetAvatar();
            }
            
            if(this.data.type == ShopItemType.Character)
            {
                this.SetChara();
            }
        }

        private void SetChara()
        {
            this.SetCharaIcon();
            this.lblName.text = "角色"; //data.desc;
        }

        private void SetCharaIcon()
        {
            AvatarMasterData aData;
            MasterData.TryGetAvatar(this.data.characterId, this.data.skinId, out aData);
            CharaIcon.GetBustIcon((AvatarType)aData.CharacterId, aData.ID, false, (a, s) => {
                this.imgIcon.atlas = a;
                this.imgIcon.spriteName = s;
            });
        }

        private void SetAvatar()
        {
            this.SetAvatarIcon();
            this.SetFrame(this.data.tag);
            this.lblName.text = "皮肤";//data.desc;
        }

        private void SetAvatarIcon()
        {
            AvatarMasterData aData;
            MasterData.TryGetAvatar(this.data.characterId, this.data.skinId, out aData);
            CharaIcon.GetBustIcon((AvatarType)aData.CharacterId, aData.ID, false, (a, s) => {
                this.imgIcon.atlas = a;
                this.imgIcon.spriteName = s;
            });
        }

        private void SetFrame(ShopItemTag tag)
        {
            if(tag == ShopItemTag.Light)
            {
                this.imgFrame.spriteName = "kuang_b01";
                this.imgFrame.width = 150;
                this.imgFrame.height = 172;
                this.imgFrame.transform.localPosition = new Vector3(6, 0, 0);
            }
            else
            {
                this.imgFrame.spriteName = "lankuang";
                this.imgFrame.width = 142;
                this.imgFrame.height = 170;
                this.imgFrame.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}

