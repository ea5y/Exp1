using UnityEngine;
using System.Collections;
using Asobimo.WebAPI;
using Scm.Common.Master;
using Scm.Common.XwMaster;
using Scm.Common.GameParameter;

namespace XUI
{
    public class ItemShopXD : MonoBehaviour
    {
        public UISprite energy;
        public UISprite heroIcon;
        public UISprite gold;
        public GameObject content;
        public UISprite none;

        public UILabel lblName;
        private CharaIcon CharaIcon { get { return ScmParam.Battle.CharaIcon; } }

        public ParticleSystem effectBlue;
        public ParticleSystem effectYellow;

        public bool isShow = false;
        public bool IsShow
        {
            get { return this.isShow; }
            set
            {
                this.isShow = value;
                if (this.isShow)
                    this.counter = 1;
                else
                    this.counter = 0;
                this.isRotated = false;
                this.rotateEnable = true;
            }
        }
        public float speed = 0.02f;
        public bool isRotated = false;
        public bool rotateEnable = false;
        

        public float counter = 0;
        private ShopItemMasterData sData;

        private string GetExpireText(ShopItemMasterData sData) {
            if (sData.Expire > 0) {
                return "(" + sData.Expire + "天)";
            }
            return string.Empty;
        }

        public void FillItem(ActivityProduct data, bool canBuy)
        {
            this.energy.gameObject.SetActive(false);
            this.heroIcon.gameObject.SetActive(false);
            this.gold.gameObject.SetActive(false);

            ShopItemMasterData sData;
            MasterData.TryGetShop(int.Parse(data.game_inside_id), out sData);
            this.sData = sData;
            
            if(ShopItemType.Character == sData.ShopItemType)
            {
                CharaIcon.GetBustIcon((AvatarType)sData.CharacterId, sData.AvatarId, false, (a, s) => {
                    this.heroIcon.atlas = a;
                    this.heroIcon.spriteName = s;
                });
                this.heroIcon.gameObject.SetActive(true);

                Scm.Common.Master.CharaMasterData cData;
                MasterData.TryGetChara(sData.CharacterId, out cData);
                this.lblName.text = "角色" + GetExpireText(sData) + "\n" + cData.Name;
            }
            
            if(ShopItemType.Gold == sData.ShopItemType)
            {
                this.gold.gameObject.SetActive(true);
                this.lblName.text = sData.Description;
            }

            if(ShopItemType.Energy == sData.ShopItemType)
            {
                this.energy.gameObject.SetActive(true);
                this.lblName.text = sData.Description;
            }

            if(ShopItemType.Avatar == sData.ShopItemType)
            {
                AvatarMasterData aData;
                MasterData.TryGetAvatar(sData.CharacterId, sData.AvatarId, out aData);
                
                CharaIcon.GetBustIcon((AvatarType)aData.CharacterId, aData.ID, false, (a, s)=>{
                        this.heroIcon.atlas = a;
                        this.heroIcon.spriteName = s;
                        }); 
                this.lblName.text = "皮肤" + GetExpireText(sData) + sData.Description;
                this.heroIcon.gameObject.SetActive(true);
            }

            if(canBuy)
            {
                if ("1" == data.buyed)
                    this.isShow = true;
                else
                    this.isShow = false;
                if(this.IsShow)
                {
                    this.ForceRotate(new Vector3(0, 0, 0));
                }
                else
                {
                    this.ForceRotate(new Vector3(0, 180, 0));
                }
                
                this.IsActive(this.IsShow);
            }
            else
            {
                this.ForceRotate(new Vector3(0, 0, 0));
                this.IsActive(true);
            }
        }

        private void ForceRotate(Vector3 value)
        {
            transform.rotation = Quaternion.Euler(value);
        }
        
        private void IsActive(bool isShow)
        {
            this.content.gameObject.SetActive(isShow);
            this.none.gameObject.SetActive(!isShow);
        }

        private void Update()
        {
            if (this.rotateEnable)
            {
                Net.Network.Instance.StartCoroutine(this.RotateTest());
                if (this.isRotated)
                    this.IsActive(this.IsShow);
            }            
        }

        public Vector3 rotationEnd = new Vector3(0, 180, 0);
        private void Rotate()
        {
            if (this.isShow)
                rotationEnd = new Vector3(0, 0, 0);
            else
                rotationEnd = new Vector3(0, 180, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationEnd), Time.deltaTime * 2f);
            if (transform.rotation.Equals(rotationEnd))
                this.isRotated = true;
            Debug.Log("RRRRRRRRRR" + rotationEnd.y);
        }

        private IEnumerator RotateTest()
        {
            Debug.Log("Counter: " + this.counter);
            while(this.counter <= 1 && this.counter >= 0)
            {
                if (this.isShow)
                {
                    this.counter -= Time.deltaTime * this.speed;
                    if (this.counter < 0.5f)
                    {
                        this.isRotated = true;
                        this.PlayEffect();
                    }
                }                    
                else
                {
                    this.counter += Time.deltaTime * this.speed;
                    if (this.counter > 0.5f)
                    {
                        this.isRotated = true;
                    }
                }
                    
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 180, 0), this.counter);
                yield return null;
            }

            this.rotateEnable = false;
        }

        private void PlayEffect()
        {
            if(ShopItemType.Gold == sData.ShopItemType || ShopItemType.Energy == sData.ShopItemType)
                this.effectYellow.Play();
            else
                this.effectBlue.Play();
        }
    }
}
