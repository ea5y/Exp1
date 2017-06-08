using UnityEngine;
using System.Collections;
using Asobimo.WebAPI;
using Scm.Common.Master;
using Scm.Common.XwMaster;
using CharaMasterData = Scm.Common.Master.CharaMasterData;
namespace XUI
{
    public class ItemShopJS : MonoBehaviour
    {
        public UISprite icon;
        public UILabel nameJS;
        public UILabel coin;
        public UIButton btnGo;
        public ParticleSystem effect;

        public ProductStatus data;
        public ShopItemMasterData sData;
        private CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }
        public void FillItem(ProductStatus data)
        {
            this.data = data;

            ShopItemMasterData sData;
            MasterData.TryGetShop(this.data.GameID, out sData);

            this.SetTransformData(sData);
            this.SetIcon(sData);
            this.SetCoin();
            this.SetName(sData);
            this.SetEvent();            
        }

        private void SetEvent()
        {
            this.btnGo.onClick.Clear();
            EventDelegate.Add(this.btnGo.onClick, this.OnBtnGoClick);
        }

        private void SetName(ShopItemMasterData sData)
        {
            CharaMasterData cData;
            MasterData.TryGetChara(sData.CharacterId, out cData);
            if(null == cData)
            {
                Debug.LogError("===> Can not Find CharaMasterData " + this.data.GameID);
                return;
            }
            this.nameJS.text = cData.Name;
        }

        private void SetCoin()
        {
            this.coin.text = this.data.Price.ToString();
        }

        private void SetIcon(ShopItemMasterData sData)
        {
            CharaIcon.GetBustIcon((AvatarType)sData.CharacterId, sData.AvatarId, false, (a, s) => {
                this.icon.atlas = a;
                this.icon.spriteName = s;
            });
        }

        private void SetTransformData(ShopItemMasterData sData)
        {
            this.sData = sData;
        }

        private void OnBtnGoClick() 
        {
            Debug.Log("===> Go to Character");
            GUIShop.Instance.SetDetailJS(this);
        }
    }
}
